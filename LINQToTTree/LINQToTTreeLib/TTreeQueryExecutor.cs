using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.ExecutionCommon;
using LINQToTTreeLib.Optimization;
using LINQToTTreeLib.QueryVisitors;
using LINQToTTreeLib.Utils;
using Nito.AsyncEx;
using NVelocity;
using NVelocity.App;
using Remotion.Linq;
using ROOTNET.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Executes the query.
    /// </summary>
    public class TTreeQueryExecutor : IQueryExecutor
    {
        private ExecutionEnvironment _exeReq = new ExecutionEnvironment();

        /// <summary>
        /// CINT commands we want to make sure are in our headers
        /// </summary>
        private string[] _cintLines;

        /// <summary>
        /// How many times the query cache has been hit
        /// </summary>
        public int CountCacheHits { get; set; }

        /// <summary>
        /// How many times we've actually run root to get a result.
        /// </summary>
        public int CountExecutionRuns { get; set; }

        /// <summary>
        /// Get/Set a flag that tells the system to re-check the file dates each time a query
        /// is run, rather than just the first query. Over a network this can dramatically slow
        /// down the system if checking is turned on.
        /// </summary>
        public bool RecheckFileDatesOnEachQuery { get; set; }

        /// <summary>
        /// Hold onto the original root files before we did resolution.
        /// </summary>
        private Uri[] _originalRootFiles = null;

        /// <summary>
        /// After the originalRootFiles are parsed and resolved into individual root files, we cache them here
        /// for easy use, organised by scheme.
        /// </summary>
        private (string scheme, Uri[] files)[] _resolvedRootFiles = null;

        /// <summary>
        /// Get/Set query cleanup control. If false, the files won't be deleted.
        /// </summary>
        public bool CleanupQuery
        {
            get { return _exeReq.CleanupQuery; }
            set { _exeReq.CleanupQuery = value; }
        }

        /// <summary>
        /// Compile with full debug infomration when possible.
        /// </summary>
        public bool CompileDebug
        {
            get { return _exeReq.CompileDebug; }
            set { _exeReq.CompileDebug = value; }
        }

        /// <summary>
        /// Should we dump verbose messages as we process something to Console?
        /// </summary>
        public bool Verbose
        {
            get { return _exeReq.Verbose; }
            set { _exeReq.Verbose = value; }
        }

        /// <summary>
        /// Get/Set query cache control. If set true then the query cache will be ignored and all quieries will be re-run.
        /// </summary>
        public bool IgnoreQueryCache { get; set; }

        /// <summary>
        /// Break into the debugger just before calling TTree::Process.
        /// </summary>
        /// <remarks>The break occurs just before the call to TTree::Process. The C++ code should be loaded at this point and this will allow you to load up the source file and set break points in the generated code.</remarks>
        public bool BreakToDebugger
        {
            get { return _exeReq.BreakToDebugger; }
            set { _exeReq.BreakToDebugger = value; }
        }

        /// <summary>
        /// Statement optimization changes execution order. Use this for debugging, but code will run significantly longer.
        /// </summary>
        public bool UseStatementOptimizer { get; set; }


        [Serializable]
        public class BadUriException : Exception
        {
            public BadUriException() { }
            public BadUriException(string message) : base(message) { }
            public BadUriException(string message, Exception inner) : base(message, inner) { }
            protected BadUriException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// We are going to be executing over a particular file and tree
        /// </summary>
        /// <param name="rootFiles"></param>
        /// <param name="treeName"></param>
        /// <param name="baseNtupleObject">The type of the ntuple object that we are translating *to*, contains all the proxy definitions, etc.</param>
        /// <param name="startingNtupeObjectType">The type of teh ntuple that started this translation, if different - mostly used for testing.</param>
        public TTreeQueryExecutor(Uri[] rootFiles, string treeName, Type baseNtupleObject, Type startingNtupeObjectType = null)
        {
            TraceHelpers.TraceInfo(2, "Initializing TTreeQueryExecutor");
            CleanupQuery = true;
            IgnoreQueryCache = false;
            RecheckFileDatesOnEachQuery = false;
            BreakToDebugger = false;
            UseStatementOptimizer = true;

            CountCacheHits = 0;
            CountExecutionRuns = 0;

            //
            // Basic checks
            // 

            // The tree name and object type.
            if (string.IsNullOrWhiteSpace(treeName))
                throw new ArgumentException("The tree name must be valid");
            if (baseNtupleObject == null)
                throw new ArgumentNullException("baseNtupleObject");

            // The uri's for the files.
            if (rootFiles == null || rootFiles.Length == 0)
                throw new ArgumentException("The TTree Query Exector was given an empty array of root files - a valid root files is required to work!");

            var nullFiles = (from f in rootFiles
                             where f == null
                             select f).ToArray();
            if (nullFiles.Length > 0)
            {
                throw new ArgumentNullException("Null FileInfo passed to LINKToTTree");
            }

            var badFiles = (from f in rootFiles
                            where !UriGood(f)
                            select f).ToArray();
            if (badFiles.Length > 0)
            {
                StringBuilder bld = new StringBuilder();
                bld.Append("The following URI(s) could not be processed by their scheme handlers: ");
                foreach (var f in badFiles)
                {
                    bld.AppendFormat("{0} ", f.OriginalString);
                }
                throw new BadUriException(bld.ToString());
            }

            // Make sure the object we are using is correct, and that it has non-null values
            // for the things passed in. We do this now so we don't have to have checks later on.

            if (baseNtupleObject.GetField("_gObjectFiles") == null)
                throw new ArgumentException("_gObjectFiles - object is not a member of " + baseNtupleObject.ToString());
            if (baseNtupleObject.GetField("_gCINTLines") == null)
                throw new ArgumentException("_gCINTLines - object is not a member of " + baseNtupleObject.ToString());

            var extraFiles = baseNtupleObject.GetField("_gObjectFiles").GetValue(null) as string[];
            if (extraFiles == null)
            {
                _exeReq.ExtraComponentFiles = new FileInfo[0];
            }
            else
            {
                _exeReq.ExtraComponentFiles = (from spath in extraFiles
                                               select new FileInfo(spath)).ToArray();
            }
            var badfiles = from f in _exeReq.ExtraComponentFiles
                           where !f.Exists
                           select f;
            string bad = "";
            foreach (var badf in badfiles)
            {
                bad = bad + "'" + badf.FullName + "' ";
            }
            if (bad.Length != 0)
                throw new ArgumentException("Extra component files were missing: " + bad + "");

            var cintLines = baseNtupleObject.GetField("_gCINTLines").GetValue(null) as string[];
            if (cintLines == null)
            {
                cintLines = new string[0];
            }

            ///
            /// For the includes it is ok if the files aren't good enough yet
            /// 

            if (baseNtupleObject.GetField("_gClassesToDeclare") == null)
            {
                _exeReq.ClassesToDictify = new string[0][];
            }
            else
            {
                var rawClasses = baseNtupleObject.GetField("_gClassesToDeclare").GetValue(null) as string[];
                var rawIncludes = baseNtupleObject.GetField("_gClassesToDeclareIncludes").GetValue(null) as string[];
                if (rawClasses.Length != rawIncludes.Length)
                    throw new InvalidOperationException("The classes and includes are of different length");

                var asPairs = rawClasses.Zip(rawIncludes, (sCls, sInc) => new string[] { sCls, sInc });
                _exeReq.ClassesToDictify = asPairs.ToArray();
            }

            //
            // Save the ntuple object we are using.
            //

            _baseNtupleObjectType = startingNtupeObjectType;
            if (_baseNtupleObjectType == null)
                _baseNtupleObjectType = baseNtupleObject;

            ///
            /// Save the values
            /// 

            _originalRootFiles = rootFiles;
            _exeReq.TreeName = treeName;
            _cintLines = cintLines;
            TraceHelpers.TraceInfo(3, "Done Initializing TTreeQueryExecutor");
        }

        /// <summary>
        /// Resolve the ROOT files from our current Uri's to real ones.
        /// </summary>
        private async Task ResolveROOTFiles()
        {
            if (_resolvedRootFiles != null)
            {
                return;
            }

            // The Uri's that come in may not be the ones we actually need to run over. Resolve them.
            var resolvedRootFilesAll = _originalRootFiles
                .Select(async u => await ResolveDatasetUri(u))
                .ToArray();

            _resolvedRootFiles = (await Task.WhenAll(resolvedRootFilesAll))
                .SelectMany(u => u)
                .GroupBy(u => u.Scheme)
                .Select(grp => (grp.Key, grp.ToArray()))
                .ToArray();
        }

#pragma warning disable CS0649
        [ImportMany(typeof(IDataFileSchemeHandler))]
        IEnumerable<IDataFileSchemeHandler> _dataSchemeHandlers;
#pragma warning disable CS0649

        [Serializable]
        public class DataSchemeNotKnownException : Exception
        {
            public DataSchemeNotKnownException() { }
            public DataSchemeNotKnownException(string message) : base(message) { }
            public DataSchemeNotKnownException(string message, Exception inner) : base(message, inner) { }
            protected DataSchemeNotKnownException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Find a data scheme. Fail badly if we can't.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private IDataFileSchemeHandler GetDataHandler(Uri f)
        {
            Init();
            LocalInit();
            return _dataSchemeHandlers
                .Where(d => d.Scheme == f.Scheme)
                .FirstOrDefault()
                .ThrowIfNull(() => new DataSchemeNotKnownException($"Uri with scheme '{f.Scheme}' can't be processed - don't know how to deal with the Uri scheme (only know about {AllSchemes()}!"));
        }

        /// <summary>
        /// Return pretty-printed copy of the schemes we know about.
        /// </summary>
        /// <returns></returns>
        private string AllSchemes()
        {
            var sb = new StringBuilder();

            foreach (var d in _dataSchemeHandlers)
            {
                sb.Append(" " + d.Scheme);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Check to make sure the URI is a good one. Currently we only deal
        /// with file URI's, so this will x-check that.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private bool UriGood(Uri f)
        {
            return GetDataHandler(f)
                .GoodUri(f);
        }

        /// <summary>
        /// Resolve the incoming Uri's into whatever they will get used for in the end.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        /// <remarks>
        /// Recursively try to resolve the Uri's until they stop changing their format.
        /// </remarks>
        private async Task<IEnumerable<Uri>> ResolveDatasetUri(Uri u)
        {
            var scheme = u.Scheme;
            var resolved = (await GetDataHandler(u).ResolveUri(u))
                .ToArray();

            // See if there were any changes. If not, then we just return it.
            if (resolved.Length == 1 && resolved[0].OriginalString == u.OriginalString)
            {
                return resolved;
            }
            else
            {
                var lst = new List<Uri>();
                foreach (var rUri in resolved)
                {
                    foreach (var rrUri in await ResolveDatasetUri(rUri))
                    {
                        lst.Add(rrUri);
                    }
                }
                return lst;
            }
        }

        /// <summary>
        /// Return a collection. We currently don't support this, so it remains a
        /// bomb! And it is not likely one would want to move a TB of info back from a
        /// File to local .NET memory... now, writing it out to a file is a possibility, and there
        /// is a AsTTree result operator now that will do just this. However, it returns a TTree or a FileInfo,
        /// which means that it is a scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        private IQueryResultCache _cache = new QueryResultCache();

        /// <summary>
        /// Execute a scalar result. Things like Count and Aggragate.
        /// This is a direct request by the LINQ system - the user didn't use "Future". However,
        /// internally we just queue it up as a Future request and thus run it along with anything
        /// else the user had requeseted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            TraceHelpers.TraceInfo(4, "Executing Scalar directly");
            try
            {
                var r = ExecuteScalarAsFuture<T>(queryModel);
                return r.Value;
            }
            finally
            {
                TraceHelpers.TraceInfo(5, "Done Executing Scalar directly");
            }
        }

        /// <summary>
        /// Interface to allow use easy access to cached queries.
        /// </summary>
        interface IQueuedQuery
        {
            IExecutableCode Code { get; set; }

            /// <summary>
            /// We have a list of results - add them all together and extract them.
            /// </summary>
            /// <param name="results"></param>
            Task ExtractResult(IDictionary<string, RunInfo>[] results);

            /// <summary>
            /// Rename an output object if it is a global resource so we don't step on anything else.
            /// </summary>
            /// <param name="result"></param>
            /// <param name="cycle"></param>
            void RenameForCycle(IDictionary<string, ROOTNET.Interface.NTObject> result, int cycle, DirectoryInfo queryDirectory);

            /// <summary>
            /// Push the results into the cache
            /// </summary>
            /// <param name="results"></param>
            Task CacheResults(IDictionary<string, RunInfo>[] results);
        }


        [Serializable]
        public class BadResultFromQueryException : InvalidOperationException
        {
            public BadResultFromQueryException() { }
            public BadResultFromQueryException(string message) : base(message) { }
            public BadResultFromQueryException(string message, Exception inner) : base(message, inner) { }
            protected BadResultFromQueryException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Enough info to run a query at a later date.
        /// </summary>
        class QueuedQuery<RType> : IQueuedQuery
        {

            public IExecutableCode Code { get; set; }

            public IQueryResultCacheKey CacheKey { get; set; }

            public FutureValue<RType> Future { get; set; }

            /// <summary>
            /// Go through the list and extract the results that are needed
            /// </summary>
            /// <param name="results"></param>
            public async Task ExtractResult(IDictionary<string, RunInfo>[] results)
            {

                var finalResultList = await Task.WhenAll(results
                    .Select(async indR => await Future.TreeExecutor.ExtractResult<RType>(Code.ResultValues.FirstOrDefault(), indR)));

                if (finalResultList.Where(fr => fr == null).Any())
                {
                    var names = results
                        .SelectMany(kv => kv.Keys)
                        .Aggregate((s_accum, s_new) => s_accum + "," + s_new);
                    throw new BadResultFromQueryException($"A query has returned a null result. This is an internal error and needs some low level debugging (for type {typeof(RType).FullName} and names {names}).");
                }

                if (finalResultList.Length == 1)
                {
                    Future.SetValue(finalResultList[0]);
                } else
                {
                    var adder = Future.TreeExecutor._resultAdders
                        .ThrowIfNull(() => new InvalidOperationException("Result Adders has not be composed!"))
                        .Where(a => a.CanHandle(typeof(RType)))
                        .FirstOrDefault()
                        .ThrowIfNull(() => new InvalidOperationException($"Unable to find an IAddResult object for type '{typeof(RType).Name}' - so can't add them together! Please provide MEF export."));

                    Future.SetValue(finalResultList.Aggregate((acc, newval) => adder.Update(acc, newval)));
                }
            }

            /// <summary>
            /// Cache the results for everything.
            /// </summary>
            /// <param name="results"></param>
            Task IQueuedQuery.CacheResults(IDictionary<string, RunInfo>[] results)
            {
                return Future.TreeExecutor.CacheResults(Code.ResultValues.FirstOrDefault(), CacheKey, results);
            }

            /// <summary>
            /// Run the rename on the varaible cycle.
            /// </summary>
            /// <param name="result"></param>
            /// <param name="cycle"></param>
            public void RenameForCycle(IDictionary<string, NTObject> result, int cycle, DirectoryInfo queryDirectory)
            {
                Future.TreeExecutor.RenameForCycle<RType>(Code.ResultValues.FirstOrDefault(), result, cycle, queryDirectory);
            }
        }

        /// <summary>
        /// The list of queires that haven't been run yet.
        /// </summary>
        List<IQueuedQuery> _queuedQueries = new List<IQueuedQuery>();

#pragma warning disable CS0649
        [ImportMany]
        IEnumerable<IAddResult> _resultAdders;
#pragma warning restore

        /// <summary>
        /// Future Value for adders. NOTE: This is not functional. If you update the result of something returned here,
        /// then this will return the updated result - it holds onto an object reference!!!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class AddedFutureValue<T> : IFutureValue<T>
        {
            private IFutureValue<T> _accumulator;
            private IAddResult _adder;
            private IFutureValue<T> _o2;
            private bool _added = false;
            private T _val;

            public AddedFutureValue(IFutureValue<T> accumulator, IFutureValue<T> o2, IAddResult adder)
            {
                _accumulator = accumulator;
                _o2 = o2;
                _adder = adder;
            }
            
            /// <summary>
            /// Are they both ready?
            /// </summary>
            public bool HasValue
            {
                get
                {
                    return _accumulator.HasValue && _o2.HasValue;
                }
            }

            /// <summary>
            /// Returns the value.
            /// </summary>
            public T Value
            {
                get
                {
                    if (!_added)
                    {
                        _val = _adder.Update(_accumulator.Value, _o2.Value);
                        _added = true;
                    }
                    return _val;
                }
            }

            /// <summary>
            /// Return a task that will fire when our inputs are done.
            /// </summary>
            /// <returns></returns>
            public Task GetAvailibleTask()
            {
                return Task.WhenAll(_accumulator.GetAvailibleTask(), _o2.GetAvailibleTask());
            }
        }

        /// <summary>
        /// Do first cloning of the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class CloneFutureValue<T> : IFutureValue<T>
        {
            private IAddResult _adder;
            private IFutureValue<T> _held;
            private bool _cloned = false;
            private T _val;

            public CloneFutureValue(IFutureValue<T> held, IAddResult adder)
            {
                _adder = adder;
                _held = held;
            }

            /// <summary>
            /// If we have a value, then we have a value.
            /// </summary>
            public bool HasValue { get { return _held.HasValue; } }

            /// <summary>
            /// Clone the value in place if it is needed (we do a lazy clone).
            /// </summary>
            public T Value
            {
                get
                {
                    CloneValue().Wait();
                    return _val;
                }
            }

            private async Task CloneValue()
            {
                if (!_cloned)
                {
                    _val = await _adder.Clone(_held.Value);
                    _cloned = true;
                }
            }

            /// <summary>
            /// Return a task that will fire when a value is rendered.
            /// </summary>
            /// <returns></returns>
            public Task GetAvailibleTask()
            {
                async Task WaitTillDone()
                {
                    await _held;
                    await CloneValue();
                }
                return WaitTillDone();
            }
        }

        /// <summary>
        /// Internal method to return a future to a query.
        /// We first check the cache, if there is a hit, we assign the result
        /// right away. Otherwise we queue up the query to be executed next time
        /// all queries are executed.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="qm"></param>
        /// <returns></returns>
        internal IFutureValue<TResult> ExecuteScalarAsFuture<TResult>(QueryModel queryModel)
        {
            ///
            /// We have to init everything - which means using MEF!
            /// 

            TraceHelpers.TraceInfo(6, "ExecuteScalarAsFuture: Startup");
            Init();
            LocalInit();

            // If the user has put together several different sources with Concat, we need to
            // split those out. We will call back into ourselves to evalute each QM - this ends
            // the updating if this is multiple QM's.
            var r = ExecuteScalarConcatQM<TResult>(queryModel);
            if (r != null)
            {
                return r;
            }

            ///
            /// The query visitor is what we will use to scan the actual guys.
            /// The funny MEF logic is to make sure that if we are called twice we
            /// don't re-compose this object.
            /// 

            var result = new GeneratedCode();
            var codeContext = new CodeContext() { BaseNtupleObjectType = _baseNtupleObjectType };
            codeContext.SetTopLevelQuery(queryModel);

            // Future to return the cache key. Put some coding protection in there.
            IQueryResultCacheKey key = null;
            codeContext.CacheKeyFuture = () =>
            {
                if (key == null)
                {
                    throw new InvalidOperationException("Call to fetch the cache key before it is set");
                }
                return key;
            };

            var qv = new QueryVisitor(result, codeContext, _gContainer);
            _gContainer.SatisfyImportsOnce(qv);

            ///
            /// Parse the query
            /// 

            TraceHelpers.TraceInfo(7, "ExecuteScalarAsFuture: Visiting query model");
            qv.VisitQueryModel(queryModel);


            // Normalize the root files for the query. These will be used for cache lookup, the query, etc.
            var normalizedRootFiles = _originalRootFiles
                .Select(u => GetDataHandler(u).Normalize(u))
                .ToArray();

            // see if we have a cache for this
            TraceHelpers.TraceInfo(8, "ExecuteScalarAsFuture: Getting cache key");
            {
                object[] inputs = result.VariablesToTransfer.Select(x => x.Value).ToArray();
                key = _cache.GetKey(normalizedRootFiles, _exeReq.TreeName, inputs, codeContext.CacheCookies.ToArray(), queryModel,
                    dateChecker: u => GetDataHandler(u).GetUriLastModificationDate(u));
            }
            if (!IgnoreQueryCache)
            {
                TraceHelpers.TraceInfo(9, "ExecuteScalarAsFuture: Looking for cache hit");
                var cacheHit = _cache.Lookup<TResult>(key,
                    _varSaver.Get(result.ResultValueAsVaraible),
                    result.ResultValueAsVaraible,
                    () => _resultAdders.Where(a => a.CanHandle(typeof(TResult))).FirstOrDefault()).Result;
                if (cacheHit.Item1)
                {
                    CountCacheHits++;
                    TraceHelpers.TraceInfo(9, "ExecuteScalarAsFuture: Returning cache hit");
                    return new FutureValue<TResult>(cacheHit.Item2);
                }
            }
            else
            {
                Console.WriteLine("WARNING: IgnoreQueryCache is set to true - we are not looking for cache hits on this LINQtoTTree query!");
            }

            ///
            /// Ok, no cache hit. Optimize and queue up the run.
            /// 

            if (UseStatementOptimizer)
                Optimizer.Optimize(result);
            TraceHelpers.TraceInfo(10, "ExecuteScalarAsFuture: Queuing scalar execution");
            var cq = new QueuedQuery<TResult>() { Code = result, CacheKey = key, Future = new FutureValue<TResult>(this) };
            _queuedQueries.Add(cq);
            return cq.Future;
        }

        /// <summary>
        /// Evaluate the QM if it has Concat result operators in it. This can mean that we are dealing with multiple
        /// sources of data, so we will need to execute those queries privately.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private IFutureValue<TResult> ExecuteScalarConcatQM<TResult>(QueryModel queryModel)
        {
            var qmsConcated = ConcatSplitterQueryVisitor.Split(queryModel);
            if (qmsConcated.Length > 1)
            {
                // Get the future values for all of these.
                var qmValues = qmsConcated.Select(q => ExecuteUnknownQM<TResult>(q)).ToArray();

                // Get the addition operator for these folks
                var adder = _resultAdders
                    .ThrowIfNull(() => new InvalidOperationException("Result Adders has not be composed!"))
                    .Where(a => a.CanHandle(typeof(TResult)))
                    .FirstOrDefault()
                    .ThrowIfNull(() => new InvalidOperationException($"Unable to find an IAddResult object for type '{typeof(TResult).Name}' - so can't add them together! Please provide MEF export."));

                var fsum = qmValues
                    .Skip(1)
                    .Aggregate(new CloneFutureValue<TResult>(qmValues.First(), adder) as IFutureValue<TResult>, (accum, value) => new AddedFutureValue<TResult>(accum, value, adder));

                return fsum;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Execute or queue an unknown query model. If this is a QM that contains one of our own, 
        /// then we just skip out and call back into execute future.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        private IFutureValue<TResult> ExecuteUnknownQM<TResult>(QueryModel q)
        {
            var provider = q.FindQueryProvider()
                .ThrowIfNull(() => new InvalidOperationException($"I can't determine the provider for the QueryModel '{q.ToString()}' - giving up!"));

            // See if it is "us"
            if ((provider as DefaultQueryProvider)?.Executor is TTreeQueryExecutor)
            {
                var exe = (provider as DefaultQueryProvider).Executor as TTreeQueryExecutor;
                return exe.ExecuteScalarAsFuture<TResult>(q);
            } else
            {
                // It is not, so we will just execute it right here.
                //TResult r = provider.
                throw new InvalidOperationException("Currently unable to directly execute queries on other providers");
            }
        }

        /// <summary>
        /// Lock to prevent executing multiple times.
        /// </summary>
        private AsyncLock _queryExecuteLock = new AsyncLock();

        /// <summary>
        /// Called when it is time to execute all the queries queued against this executor.
        /// </summary>
        /// <remarks>
        /// All queued queries will be finished when this task completes.
        /// </remarks>
        internal async Task ExecuteQueuedQueries()
        {
            // Because we can do many resultes, we might get asked from many places to actually run the queried results. So
            // we need a way to make sure that we don't try to run multiple times. We do that here.
            using (var exeLockWaiter = await _queryExecuteLock.LockAsync())
            {
                // Make sure no one else has cleaned up the queue for us.
                if (_queuedQueries.Count <= 0)
                {
                    return;
                }

                // Log where we are.
                LogExecutionStart();

                // We now need the actual root files - so resolve them.
                await ResolveROOTFiles();

                // Get all the queries together, combined, and ready to run.
                TraceHelpers.TraceInfo(11, "ExecuteQueuedQueries: Startup - combining all code");
                var combinedInfo = new CombinedGeneratedCode();
                foreach (var cq in _queuedQueries)
                {
                    combinedInfo.AddGeneratedCode(cq.Code);
                }

                // Optimize the whole thing (this can be a rather expensive operation)
                if (UseStatementOptimizer)
                    Optimizer.Optimize(combinedInfo);

                // Execute the queries over all the schemes, and sort their results by value, and then combine them into a single dictionary.
                int cycle = -1;
                int cycle_counter() => Interlocked.Increment(ref cycle);
                var combinedResultsTasks = _resolvedRootFiles
                    .SelectMany(sch => ExecuteQueuedQueriesForASchemeFileBatch(sch.scheme, sch.files, combinedInfo, cycle_counter))
                    .ToArray();

                var combinedResults = await Task.WhenAll(combinedResultsTasks);

                // Extract all the variables! And save in the cache, and set the
                // future value so everyone else can use them!
                TraceHelpers.TraceInfo(15, $"ExecuteQueuedQueries: Extracting the query results from {_resolvedRootFiles.Length} runs.");
                foreach (var cq in _queuedQueries)
                {
                    await cq.ExtractResult(combinedResults);
                    await cq.CacheResults(combinedResults);
                }
                _queuedQueries.Clear();

                // Ok, we are all done. Delete the directory that we were just using
                // after unloading all the modules
                LogExecutionFinish();
            }
        }

        /// <summary>
        /// Execute on a single scheme for a batch of files. We have the oporitunity to split them up here.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="files"></param>
        /// <param name="combinedInfo"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private IEnumerable<Task<IDictionary<string, RunInfo>>> ExecuteQueuedQueriesForASchemeFileBatch(string scheme, Uri[] files,
            CombinedGeneratedCode combinedInfo, 
            Func<int> cycle)
        {
            // Get the query executor
            var referencedLeafNames = combinedInfo.ReferencedLeafNames.ToArray();
            var localMaker = CreateQueryExecutor(scheme, referencedLeafNames);
            TraceHelpers.TraceInfo(13, $"ExecuteQueuedQueriesForAScheme: Will part out {files.Length} Uri's to run on {scheme}.", opt: TraceEventType.Start);

            // First, we let the executor tell us if it needs to split things up. Most executors will not split things up at all,
            // as everything looks the same. But you could imagine that a machine name is encoded and you want to send things to one machine, or
            // to another.
            using (var local = localMaker())
            {
                var batchedByExecutor = local.BatchInputUris(files);

                // Next, lets see how to split things up inside each of these batches.
                var batchedFiles = batchedByExecutor
                    .SelectMany(bf =>
                    {
                        int nBatches = local.SuggestedNumberOfSimultaniousProcesses(bf);
                        var subBatchFiles = (bf.Length == 1 || nBatches == 1)
                            ? new[] { bf }
                            : SplitFilesIntoBatches(bf, nBatches);
                        return subBatchFiles;
                    });

                // Next, we need actually run them!
                return batchedFiles
                    .Select((bf, index) => ExecuteQueuedQueriesForAScheme(scheme, bf, combinedInfo, cycle, localMaker, referencedLeafNames));
            }
        }

        /// <summary>
        /// Return the file list split into batches.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="nBatches"></param>
        /// <returns></returns>
        private Uri[][] SplitFilesIntoBatches(Uri[] files, int nBatches)
        {
            var grps = files
                .Select((u, index) => (u, index))
                .GroupBy(info => info.index % nBatches)
                .Select(lst => lst.Select(f => f.u).ToArray());
            return grps.ToArray();
        }


        /// <summary>
        /// Run a query over a single scheme of Uri's.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="files"></param>
        /// <param name="combinedInfo"></param>
        /// <remarks>
        /// This is running in its own directory. This is important because there are times when this
        /// query, split up, might produce the same files. At the end, if that happens, we do a rename
        /// by cycle number.
        /// </remarks>
        private async Task<IDictionary<string, RunInfo>> ExecuteQueuedQueriesForAScheme(string scheme, Uri[] files,
                CombinedGeneratedCode combinedInfo, Func<int> cycleFetcher, Func<IQueryExectuor> localMaker, string[] referencedLeafNames)
        {
            var queryDirectory = GenerateQueryDirectory();
            try
            {
                // Keep track of how often we run. Mostly for testing reasons, actually.
                CountExecutionRuns++;

                using (var local = localMaker())
                {
                    TraceHelpers.TraceInfo(13, $"ExecuteQueuedQueriesForAScheme: Start run on Uri scheme {scheme}, {files.Length} files.", opt: TraceEventType.Start);
                    foreach (var u in files)
                    {
                        TraceHelpers.TraceInfo(13, $"  -> {u.OriginalString}");
                    }
                    // Next, generate and slim the proxy file and the TSelector file
                    var proxyFile = await local.GenerateProxyFile(files, _exeReq.TreeName, queryDirectory);
                    var slimedProxyFile = SlimProxyFile(referencedLeafNames, proxyFile, queryDirectory);
                    TraceHelpers.TraceInfo(14, "ExecuteQueuedQueries: Startup - building the TSelector");
                    var templateRunner = WriteTSelector(slimedProxyFile.Name, Path.GetFileNameWithoutExtension(proxyFile.Name), combinedInfo, queryDirectory);

                    // Run the actual query.
                    var r = await local.Execute(files, templateRunner, queryDirectory, combinedInfo.VariablesToTransfer);

                    // Rename by cycle for those that need it. This allows, for example, file names that are the same in different runs
                    // of the code to be copied back into the same directory.
                    var cycle = cycleFetcher();
                    foreach (var cq in _queuedQueries)
                    {
                        cq.RenameForCycle(r, cycle, queryDirectory);
                    }

                    // Done - return everything, converted to RunInfo
                    return r
                        .ToDictionary(er => er.Key, er => new RunInfo() { _cycle = cycle, _result = er.Value });
                }
            } finally
            {
                CleanUpQuery(queryDirectory);
                TraceHelpers.TraceInfo(13, $"ExecuteQueuedQueriesForAScheme: Finished run on Uri scheme {scheme}.", opt: TraceEventType.Stop);
            }
        }

        /// <summary>
        /// Log that we are about to start query execution
        /// </summary>
        private void LogExecutionStart()
        {
            if (_originalRootFiles != null)
            {
                var m = new StringBuilder();
                foreach (var f in _originalRootFiles)
                {
                    if (m.Length > 0)
                        m.Append(", ");
                    m.Append($"{f.OriginalString}");
                }
                TraceHelpers.TraceInfo(30, $"LINQToTTree Executor: running {_queuedQueries.Count} queries on {m.ToString()}", TraceEventType.Start);
            }
        }

        /// <summary>
        /// Log to our event source that we are done.
        /// </summary>
        private void LogExecutionFinish()
        {
            if (_originalRootFiles != null)
            {
                var m = new StringBuilder();
                foreach (var f in _originalRootFiles)
                {
                    if (m.Length > 0)
                        m.Append(", ");
                    m.Append($"{f.OriginalString}");
                }
                TraceHelpers.TraceInfo(30, $"LINQToTTree Executor: finishing query on {m.ToString()}", TraceEventType.Stop);
            }
        }

        /// <summary>
        /// Thrown when we have no way to "execute" file.
        /// </summary>
        [Serializable]
        public class UnsupportedUriSchemeException : NotSupportedException
        {
            public UnsupportedUriSchemeException() { }
            public UnsupportedUriSchemeException(string message) : base(message) { }
            public UnsupportedUriSchemeException(string message, Exception inner) : base(message, inner) { }
            protected UnsupportedUriSchemeException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// List of query exectuor factories
        /// </summary>
#pragma warning disable 0649
        [ImportMany(typeof(IQueryExecutorFactory))]
        IEnumerable<IQueryExecutorFactory> _queryExecutorList;
#pragma warning restore 0649

        /// <summary>
        /// Create the query executor tha twill handle these files
        /// </summary>
        /// <param name="referencedLeafNames">List of leaves that are referenced by the query</param>
        /// <returns></returns>
        private Func<IQueryExectuor> CreateQueryExecutor(string scheme, string[] referencedLeafNames)
        {
            // Now find a query executor.
            var qefactory = _queryExecutorList
                .Where(qex => qex.Scheme == scheme).FirstOrDefault()
                .ThrowIfNull(() => new UnsupportedUriSchemeException($"Unable to process files of scheme '{scheme}' - no supported executor"));

            return () => qefactory.Create(_exeReq, referencedLeafNames);
        }

        /// <summary>
        /// Given the list of leaf names that are used in this query, we will slim down the
        /// proxy file so that only those leaves are defined. In a large ntuple (with 100's of items) this can
        /// change compile times from 45 seconds to 8 seconds - so is a big deal during incremental testing, etc.
        /// </summary>
        private FileInfo SlimProxyFile(string[] leafNames, FileInfo proxyFile, DirectoryInfo queryDirectory)
        {
            //
            // Create search's for the various proxy names
            //

            var goodLeafFinders = (from l in leafNames
                                   select new Regex(string.Format(@"\b{0}\b", l))).ToArray();

            //
            // Copy over the file, emitting only the lines we need to emit.
            //

            FileInfo destFile = new FileInfo($"{queryDirectory.FullName}\\slim-{proxyFile.Name}");
            using (var writer = destFile.CreateText())
            {
                // State variables
                bool proxydef = false; // True when we are looking at code to define the proxy files
                bool proxyinit = false; // True when we are scanning the proxying ctor variables

                List<string> proxyInitStatements = new List<string>();

                foreach (var line in proxyFile.EnumerateTextFile())
                {
                    bool writeLine = true;

                    //
                    // Alter our state at start of line reading
                    //

                    if (proxydef && string.IsNullOrWhiteSpace(line))
                        proxydef = false;
                    if (proxyinit && line.Contains("{") && line.Contains("}"))
                    {
                        proxyinit = false;
                        if (proxyInitStatements.Count > 0)
                        {
                            var lastinit = proxyInitStatements[proxyInitStatements.Count - 1];
                            proxyInitStatements[proxyInitStatements.Count - 1] = lastinit.Substring(0, lastinit.Length - 1);
                            foreach (var l in proxyInitStatements)
                            {
                                writer.WriteLine(l);
                            }
                        }
                    }

                    //
                    // If in the proxy def state, only include lines that have a proxy defined!
                    //

                    if (proxydef)
                    {
                        writeLine = goodLeafFinders.Where(lf => lf.Match(line).Success).Any();
                    }
                    if (proxyinit)
                    {
                        writeLine = false;
                        var keepline = goodLeafFinders.Where(lf => lf.Match(line).Success).Any();
                        if (keepline)
                        {
                            var tline = line.TrimEnd();
                            if (!tline.EndsWith(","))
                                tline = tline + ",";
                            proxyInitStatements.Add(tline);
                        }
                    }

                    //
                    // Alter state @ end of line reading
                    //

                    if (line.Contains("Proxy for each of the branches, leaves and friends of the tree"))
                        proxydef = true;

                    if (line.Contains("fClass") && line.Contains("(TClass::GetClass("))
                    {
                        proxyinit = true;
                        writeLine = false;
                        proxyInitStatements.Add(line);
                    }

                    //
                    // Write out the line if it was marked as "good"
                    //

                    if (writeLine)
                        writer.WriteLine(line);

                }
            }

            //
            // Finally, copy over any included files
            //

            ExecutionUtilities.CopyIncludedFilesToDirectory(proxyFile, queryDirectory);
            return destFile;
        }

        /// <summary>
        /// Keep track of local init for this one executor.
        /// </summary>
        private bool _executorInited = false;

        /// <summary>
        /// Do any init that this object needs done.
        /// </summary>
        private void LocalInit()
        {
            if (_executorInited)
                return;

            ///
            /// Compose us!
            /// 

            _gContainer.ComposeParts(this);

            _executorInited = true;
        }

        /// <summary>
        /// Do the work of translating the code into C++
        /// </summary>
#pragma warning disable 0649
        [Import]
        private CPPTranslator _cppTranslator;
#pragma warning restore 0649

        /// <summary>
        /// Keep track of the variable i/o code.
        /// </summary>
#pragma warning disable 0649
        [Import]
        private IVariableSaverManager _varSaver;
#pragma warning restore 0649

        /// <summary>
        /// Extract the value for iVariable from the results file.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        private async Task<T> ExtractResult<T>(IDeclaredParameter iVariable, IDictionary<string, RunInfo> results)
        {
            // Load the object and try to extract whatever info we need to from it
            var s = _varSaver.Get(iVariable);
            var objs = ExtractQueryReturnedObjectsForVariable(iVariable, results, s);

            // Return the result in a form that the code wants.
            return await s.LoadResult<T>(iVariable, objs);
        }

        /// <summary>
        /// Lets see fi we can cache the results for the object - write them all out!
        /// </summary>
        /// <param name="iVariable"></param>
        /// <param name="key"></param>
        /// <param name="results"></param>
        private Task CacheResults(IDeclaredParameter iVariable, IQueryResultCacheKey key, IDictionary<string, RunInfo>[] results)
        {
            // Get the results all out.
            var s = _varSaver.Get(iVariable);
            var objs = results
                .Select(r => ExtractQueryReturnedObjectsForVariable(iVariable, r, s))
                .ToArray();
            return _cache.CacheItem(key, objs);
        }

        /// <summary>
        /// Returns the objects from the queury that match the result object we are looking at.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <param name="results"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private static RunInfo[] ExtractQueryReturnedObjectsForVariable(IDeclaredParameter iVariable, IDictionary<string, RunInfo> results, IVariableSaver s)
        {
            var allNames = s.GetCachedNames(iVariable);

            if (allNames.Where(n => !results.Keys.Contains(n)).Any())
                throw new InvalidOperationException(string.Format("The result list from the query did not contains an object named '{0}'.", iVariable.RawValue));

            var objs = allNames.Select(n => results[n]).ToArray();
            return objs;
        }

        private static NTObject[] ExtractQueryReturnedObjectsForVariable(IDeclaredParameter iVariable, IDictionary<string, NTObject> results, IVariableSaver s)
        {
            var allNames = s.GetCachedNames(iVariable);

            if (allNames.Where(n => !results.Keys.Contains(n)).Any())
                throw new InvalidOperationException(string.Format("The result list from the query did not contains an object named '{0}'.", iVariable.RawValue));

            var objs = allNames.Select(n => results[n]).ToArray();
            return objs;
        }
        
        /// <summary>
        /// Get the saver to rename the variable coming back to match the cycle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iVariable"></param>
        /// <param name="result"></param>
        /// <param name="cycle"></param>
        private void RenameForCycle<T>(IDeclaredParameter iVariable, IDictionary<string, NTObject> result, int cycle, DirectoryInfo queryDirectory)
        {
            var s = _varSaver.Get(iVariable);
            var objs = ExtractQueryReturnedObjectsForVariable(iVariable, result, s);

            s.RenameForQueryCycle(iVariable, objs, cycle, queryDirectory);
        }

        /// <summary>
        /// We are responsible for finding a given file in the infrastructure directory. We go boom if
        /// we can't file the file.
        /// </summary>
        /// <param name="filename">The name (including extension) of the file to find</param>
        /// <returns></returns>
        private FileInfo GetInfrastructureFile(string filename)
        {
            var f = new FileInfo(InfrastructureFile(filename));
            if (!f.Exists)
                throw new FileNotFoundException("Unable to find infrastructure source file '" + f.FullName + "'");
            return f;
        }

        /// <summary>
        /// Generate a path for the infrastructure file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string InfrastructureFile(string filename)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LINQToTTree\\InfrastructureSourceFiles\\" + filename;
        }

        /// <summary>
        /// Keep track of the queires we've done so we can track where they go.
        /// </summary>
        private static int _gQueryCounter = 0;

        /// <summary>
        /// Write out the TSelector file derived from the already existing query. This now includes all the code we need
        /// in order to actually run the thing! Use the template to do it.
        /// </summary>
        /// <param name="p"></param>
        private FileInfo WriteTSelector(string proxyFileName, string proxyObjectName, IExecutableCode code,
            DirectoryInfo queryDirectory)
        {
            ///
            /// Get the template engine all setup
            /// 

            VelocityEngine eng = new VelocityEngine();
            eng.Init();

            string template;
            using (var reader = File.OpenText(TemplateDirectory("TSelectorTemplate.cxx")))
            {
                template = reader.ReadToEnd();
            }

            var context = new VelocityContext();
            context.Put("baseClassInclude", proxyFileName);
            context.Put("baseClassName", proxyObjectName);
            context.Put("CINTLines", _cintLines);

            /// In order to avoid conflicts having to do with root map files pointing to DLL's and some other weird bookeeping things
            /// that root does, we will name the query to be something different each time. This isn't a problem unless something goes
            /// wrong, but it can make whatever went wrong pretty confusing!
            context.Put("QueryIndex", _gQueryCounter);
            string queryFileName = "query" + _gQueryCounter.ToString();
            _gQueryCounter++;

            ///
            /// Now translate all the code we are looking at
            /// 

            var trans = _cppTranslator.TranslateGeneratedCode(code);
            foreach (var item in trans)
            {
                context.Put(item.Key, item.Value);
            }

            ///
            /// Output all the include files, from everywhere we have managed to collect them.
            /// 

            context.Put("IncludeFiles", code.IncludeFiles.Where(f => !f.StartsWith("<")).ToArray());
            context.Put("SystemIncludeFiles", code.IncludeFiles.Where(f => f.StartsWith("<")).ToArray());

            ///
            /// Now do it!
            /// 

            var ourSelector = new FileInfo(queryDirectory + "\\" + queryFileName + ".cxx");
            using (var writer = ourSelector.CreateText())
            {
                eng.Evaluate(context, writer, null, template);
            }

            return ourSelector;
        }

        /// <summary>
        /// We have some templates we use to run. Find them!
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string TemplateDirectory(string templateName)
        {
            var assDir = new FileInfo(Assembly.GetCallingAssembly().Location);
            var assGuess = string.Format(@"{0}\Templates\{1}", assDir.DirectoryName, templateName);
            if (File.Exists(assGuess))
                return assGuess;

            var assGuess1 = string.Format(@"{0}\{1}", assDir.DirectoryName, templateName);
            if (File.Exists(assGuess1))
                return assGuess1;

            var guess = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LINQToTTree\\Templates\\" + templateName;
            if (File.Exists(guess))
                return guess;

            throw new FileNotFoundException($"Unable to locatoin LINQToTTree template file '{templateName}' in any standard location (tried '{assGuess}' and '{assGuess1}' and '{guess}'.");
        }

        /// <summary>
        /// Get/Set the directory where we create our local query info. Defaults to the users' temp directory.
        /// </summary>
        public static DirectoryInfo QueryCreationDirectory = new DirectoryInfo(Path.GetTempPath() + "\\LINQToTTree");

        /// <summary>
        /// When true, we will first attempt to delete all files in our main query directory
        /// scratch space before creating and running everything.
        /// </summary>
        static bool gFirstQuerySetup = true;

        /// <summary>
        /// Create a new query directory for a query. Also, clean up old queries to keep the
        /// disk properly pruned.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Don't kill off recent queires - as we may have multiple copies of this program running on this
        /// machine.
        /// </remarks>
        private DirectoryInfo GenerateQueryDirectory()
        {
            var baseQueryDirectory = new DirectoryInfo(QueryCreationDirectory.FullName + "\\" + Process.GetCurrentProcess().ProcessName);
            if (gFirstQuerySetup)
            {
                gFirstQuerySetup = false;
                try
                {
                    var removeDirectories = baseQueryDirectory
                        .EnumerateDirectories()
                        .Where(d => (DateTime.Now - d.CreationTime) > TimeSpan.FromDays(7));
                    foreach (var d in removeDirectories)
                    {
                        d.Delete(true);
                    }
                }
                catch { }
            }
            var queryDirectory = new DirectoryInfo(baseQueryDirectory.FullName + "\\" + Path.GetRandomFileName());
            queryDirectory.Create();
            return queryDirectory;
        }

        /// <summary>
        /// Clean up the query - keep user's disk clean!
        /// </summary>
        private void CleanUpQuery(DirectoryInfo queryDirectory)
        {
            TraceHelpers.TraceInfo(16, "ExecuteQueuedQueries: unloading all results");
            if (_exeReq.CleanupQuery)
            {
                // If we can't do the clean up, don't worry about it.
                try
                {
                    queryDirectory.Delete(true);
                }
                catch
                { }
            }
        }

        /// <summary>
        /// We compose ourselves out of this
        /// </summary>
        static CompositionContainer _gContainer = null;
        
        /// <summary>
        /// Hold onto the assembly catalog
        /// </summary>
        static AggregateCatalog _gAggCat = null;

        /// <summary>
        /// Track assemblies that need to be added to this guy for composition.
        /// </summary>
        static Lazy<List<Assembly>> _gAssembliesToCompose = new Lazy<List<Assembly>>(() => new List<Assembly>());

        public static void AddAssemblyForPlugins (Assembly a)
        {
            _gAssembliesToCompose.Value.Add(a);
            Reset();
        }

        /// <summary>
        /// Return the container. Mostly for testing
        /// </summary>
        public static CompositionContainer CContainer
        {
            get
            {
                InitContainer();
                return _gContainer;
            }
        }

        /// <summary>
        /// Reset the global state of the object. Designed for testing.
        /// </summary>
        internal static void Reset()
        {
            _gContainer?.Dispose();
            _gAggCat?.Dispose();
            _gContainer = null;
            _gAggCat = null;
        }

        /// <summary>
        /// Get our compositon stuff setup
        /// </summary>
        private static void InitContainer()
        {
            if (_gContainer != null)
                return;

            // Get MEF setup with everything in our assembly.
            if (_gAggCat == null)
            {
                _gAggCat = new AggregateCatalog();
                _gAggCat.Catalogs.Add(new AssemblyCatalog(Assembly.GetCallingAssembly()));
                if (_gAssembliesToCompose.IsValueCreated)
                {
                    foreach (var a in _gAssembliesToCompose.Value)
                    {
                        _gAggCat.Catalogs.Add(new AssemblyCatalog(a));
                    }
                }
            }

            // Build the container now
            _gContainer = new CompositionContainer(_gAggCat);
            CompositionBatch b = new CompositionBatch();
            b.AddPart(new TypeHandlers.TypeHandlerCache());
            _gContainer.Compose(b);
        }

        /// <summary>
        /// The base type of the ntuple we are looping over.
        /// </summary>
        private Type _baseNtupleObjectType;

        private static bool _rootInited = false;

        /// <summary>
        /// Run init for this class.
        /// </summary>
        private void Init()
        {
            InitContainer();

            if (!_rootInited)
            {
                _rootInited = true;
                // Make sure TApplication has been started. It will init a bunch of stuff
                ROOTNET.NTApplication.CreateApplication();

                // Turn off ROOTMAP generation - since we are constantly unloading things it just gets things
                // confused. :-)
                ROOTNET.NTEnv.gEnv.SetValue("ACLiC.LinkLibs", 1);
            }
        }

        /// <summary>
        /// Returns the first item... not yet implemented.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryModel"></param>
        /// <param name="returnDefaultWhenEmpty"></param>
        /// <returns></returns>
        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            throw new NotImplementedException();
        }
    }
}
