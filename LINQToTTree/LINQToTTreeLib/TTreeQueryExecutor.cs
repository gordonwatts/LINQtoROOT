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
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using NVelocity;
using NVelocity.App;
using Remotion.Linq;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Executes the query.
    /// </summary>
    public class TTreeQueryExecutor : IQueryExecutor
    {
        /// <summary>
        /// The root files that this exector operates on
        /// </summary>
        private FileInfo[] _rootFiles;

        /// <summary>
        /// The tree name that in the root file that this guy operates on.
        /// </summary>
        private string _treeName;

        /// <summary>
        /// The header file make by the MakeProxy macro.
        /// </summary>
        private FileInfo _proxyFile;

        /// <summary>
        /// Extra objects we might need to load
        /// </summary>
        private FileInfo[] _extraComponentFiles;

        /// <summary>
        /// CINT commands we want to make sure are in our headers
        /// </summary>
        private string[] _cintLines;

        /// <summary>
        /// Classes and includes to be passed to GenerateDictionary.
        /// </summary>
        private string[][] _classToDictify;

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
        /// We are going to be executing over a particular file and tree
        /// </summary>
        /// <param name="rootFiles"></param>
        /// <param name="treeName"></param>
        public TTreeQueryExecutor(FileInfo[] rootFiles, string treeName, Type baseNtupleObject)
        {
            TraceHelpers.TraceInfo(2, "Initializing TTreeQueryExecutor");
            CleanupQuery = true;
            IgnoreQueryCache = false;
            RecheckFileDatesOnEachQuery = false;

            CountCacheHits = 0;
            CountExecutionRuns = 0;

            ///
            /// Basic checks
            /// 

            if (string.IsNullOrWhiteSpace(treeName))
                throw new ArgumentException("The tree name must be valid");
            if (baseNtupleObject == null)
                throw new ArgumentNullException("baseNtupleObject");

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
                            where !f.Exists
                            select f).ToArray();
            if (badFiles.Length > 0)
            {
                StringBuilder bld = new StringBuilder();
                bld.Append("The following file(s) do not exist and so can't be processed: ");
                foreach (var f in badFiles)
                {
                    bld.AppendFormat("{0} ", f.FullName);
                }
                throw new FileNotFoundException(bld.ToString());

            }

            ///
            /// Make sure the object we are using is correct, and that it has non-null values
            /// for the things passed in. We do this now so we don't have to have checks later on.
            /// 

            if (baseNtupleObject.GetField("_gProxyFile") == null)
                throw new ArgumentException("_gProxyFile - object is not a member of " + baseNtupleObject.ToString());
            if (baseNtupleObject.GetField("_gObjectFiles") == null)
                throw new ArgumentException("_gObjectFiles - object is not a member of " + baseNtupleObject.ToString());
            if (baseNtupleObject.GetField("_gCINTLines") == null)
                throw new ArgumentException("_gCINTLines - object is not a member of " + baseNtupleObject.ToString());

            var proxyFileName = baseNtupleObject.GetField("_gProxyFile").GetValue(null) as string;
            if (string.IsNullOrWhiteSpace(proxyFileName))
                throw new ArgumentException("_gProxyFile - points to a null file - must be a real file");
            _proxyFile = new FileInfo(proxyFileName);
            if (!File.Exists(proxyFileName))
                throw new FileNotFoundException("_gProxyFile - '" + proxyFileName + "' was not found.");

            var extraFiles = baseNtupleObject.GetField("_gObjectFiles").GetValue(null) as string[];
            if (extraFiles == null)
            {
                _extraComponentFiles = new FileInfo[0];
            }
            else
            {
                _extraComponentFiles = (from spath in extraFiles
                                        select new FileInfo(spath)).ToArray();
            }
            var badfiles = from f in _extraComponentFiles
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
                _classToDictify = new string[0][];
            }
            else
            {
                var rawClasses = baseNtupleObject.GetField("_gClassesToDeclare").GetValue(null) as string[];
                var rawIncludes = baseNtupleObject.GetField("_gClassesToDeclareIncludes").GetValue(null) as string[];
                if (rawClasses.Length != rawIncludes.Length)
                    throw new InvalidOperationException("The classes and includes are of different length");

                var asPairs = rawClasses.Zip(rawIncludes, (sCls, sInc) => new string[] { sCls, sInc });
                _classToDictify = asPairs.ToArray();
            }

            ///
            /// Save the values
            /// 

            _rootFiles = rootFiles;
            _treeName = treeName;
            _cintLines = cintLines;
            TraceHelpers.TraceInfo(3, "Done Initializing TTreeQueryExecutor");
        }

        /// <summary>
        /// Return a collection. We currently don't support this, so it remains a
        /// bomb! And it is not likely one would want to move a TB of info back from a
        /// File to another... now, writing it out to a file is a possibility - but
        /// that would be a seperate scalar result. :-)
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

            void ExecuteQuery();

            IExecutableCode Code { get; set; }

            void ExtractResult(IDictionary<string, ROOTNET.Interface.NTObject> results);
        }

        /// <summary>
        /// Enough info to run a query at a later date.
        /// </summary>
        class QueuedQuery<RType> : IQueuedQuery
        {

            public IExecutableCode Code { get; set; }

            public IQueryResultCacheKey CacheKey { get; set; }

            public FutureValue<RType> Future { get; set; }

            public void ExecuteQuery()
            {
                Future.TreeExecutor.ExecuteQueuedQueries();
            }


            public void ExtractResult(IDictionary<string, ROOTNET.Interface.NTObject> results)
            {
                var final = Future.TreeExecutor.ExtractResult<RType>(Code.ResultValues.FirstOrDefault(), CacheKey, results);
                Future.SetValue(final);
            }
        }

        /// <summary>
        /// The list of queires that haven't been run yet.
        /// </summary>
        List<IQueuedQuery> _queuedQueries = new List<IQueuedQuery>();

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

            ///
            /// The query visitor is what we will use to scan the actual guys.
            /// The funny MEF logic is to make sure that if we are called twice we
            /// don't re-compose this object.
            /// 

            var result = new GeneratedCode();
            var codeContext = new CodeContext();

            var qv = new QueryVisitor(result, codeContext, _gContainer);
            _gContainer.SatisfyImportsOnce(qv);

            ///
            /// Parse the query
            /// 

            TraceHelpers.TraceInfo(7, "ExecuteScalarAsFuture: Visiting query model");
            qv.VisitQueryModel(queryModel);

            ///
            /// Next, see if we have a cache for this
            /// 

            TraceHelpers.TraceInfo(8, "ExecuteScalarAsFuture: Getting cache key");
            IQueryResultCacheKey key = null;
            {
                object[] inputs = result.VariablesToTransfer.Select(x => x.Value).ToArray();
                key = _cache.GetKey(_rootFiles, _treeName, inputs, codeContext.CacheCookies.ToArray(), queryModel);
            }
            if (!IgnoreQueryCache)
            {
                TraceHelpers.TraceInfo(9, "ExecuteScalarAsFuture: Looking for cache hit");
                var cacheHit = _cache.Lookup<TResult>(key, _varSaver.Get(result.ResultValue), result.ResultValue);
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
            /// Ok, no cache hit. So queue up the run.
            /// 

            TraceHelpers.TraceInfo(10, "ExecuteScalarAsFuture: Queuing scalar execution");
            var cq = new QueuedQuery<TResult>() { Code = result, CacheKey = key, Future = new FutureValue<TResult>(this) };
            _queuedQueries.Add(cq);
            return cq.Future;
        }

        /// <summary>
        /// Called when it is time to execut all the queries
        /// </summary>
        internal void ExecuteQueuedQueries()
        {
            ///
            /// Make sure we are setup and ready to go.
            /// 

            PreExecutionInit();

            ///
            /// Get all the queries together, combined, and ready to run.
            /// 

            TraceHelpers.TraceInfo(11, "ExecuteQueuedQueries: Startup - combining all code");
            var combinedInfo = new CombinedGeneratedCode();
            foreach (var cq in _queuedQueries)
            {
                combinedInfo.AddGeneratedCode(cq.Code);
            }

            ///
            /// Now, do the general running.
            /// 

            CountExecutionRuns++;

            ///
            /// If we got back from that without an error, it is time to assemble the files and templates
            /// 

            TraceHelpers.TraceInfo(12, "ExecuteQueuedQueries: Loading all extra objects");
            AssembleAndLoadExtraObjects();

            ///
            /// Now that those are loaded, we need to go after the
            /// proxy. We create a new file which is dependent on the
            /// one we have been given.
            /// 

            TraceHelpers.TraceInfo(13, "ExecuteQueuedQueries: Startup - copying over proxy file");
            SlimProxyFile(combinedInfo.ReferencedLeafNames.ToArray());
            TraceHelpers.TraceInfo(14, "ExecuteQueuedQueries: Startup - building the TSelector");
            var templateRunner = WriteTSelector(_proxyFile.Name, Path.GetFileNameWithoutExtension(_proxyFile.Name), combinedInfo);
            CompileAndLoad(templateRunner);

            ///
            /// Fantastic! Now we need to run the object!
            /// 

            TraceHelpers.TraceInfo(14, "ExecuteQueuedQueries: Startup - Running the code");
            var results = RunNtupleQuery(Path.GetFileNameWithoutExtension(templateRunner.Name), combinedInfo.VariablesToTransfer);

            ///
            /// Last job, extract all the variables! And save in the cache, and set the
            /// future value so everyone else can use them!
            /// 

            TraceHelpers.TraceInfo(15, "ExecuteQueuedQueries: Extracting the query results");
            foreach (var cq in _queuedQueries)
            {
                cq.ExtractResult(results);
            }
            _queuedQueries.Clear();

            ///
            /// Ok, we are all done. Delete the directory that we were just using
            /// after unloading all the modules
            /// 

            TraceHelpers.TraceInfo(16, "ExecuteQueuedQueries: unloading all results");
            UnloadAllModules();
            if (CleanupQuery)
            {
                GetQueryDirectory().Delete(true);
            }
            _queryDirectory = null;
            TraceHelpers.TraceInfo(17, "ExecuteQueuedQueries: Done");
        }

        /// <summary>
        /// Given the list of leaf names that are used in this query, we will slim down the
        /// proxy file so that only those leaves are defined. In a large ntuple (with 100's of items) this can
        /// change compile times from 45 seconds to 8 seconds - so is a big deal during incremental testing, etc.
        /// </summary>
        private void SlimProxyFile(string[] leafNames)
        {
            //
            // Create search's for the various proxy names
            //

            var goodLeafFinders = (from l in leafNames
                                   select new Regex(string.Format(@"\b{0}\b", l))).ToArray();

            //
            // Copy over the file, emitting only the lines we need to emit.
            //

            FileInfo destFile = new FileInfo(GetQueryDirectory().FullName + "\\" + _proxyFile.Name);
            using (var writer = destFile.CreateText())
            {
                // State variables
                bool proxydef = false; // True when we are looking at code to define the proxy files
                bool proxyinit = false; // True when we are scanning the proxying ctor variables

                List<string> proxyInitStatements = new List<string>();

                foreach (var line in _proxyFile.EnumerateTextFile())
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
                writer.Close();
            }

            //
            // Finally, copy over any included files
            //

            CopyIncludedFilesToDirectory(_proxyFile, GetQueryDirectory());
        }

        /// <summary>
        /// Unload all modules that we've loaded. This should have root release the lock on everything.
        /// </summary>
        private void UnloadAllModules()
        {
            ///
            /// The library names are a simple "_" replacement. However, the full path must be given to the
            /// unload function. To avoid any issues we just scan the library list that ROOT has right now, find the
            /// ones we care about, and unload them. In general this is not a good idea, so when there are random
            /// crashes this might be a good place to come first! :-)
            /// 

            var gSystem = ROOTNET.NTSystem.gSystem;
            var libraries = gSystem.Libraries.Split(' ');
            _loadedModuleNames.Reverse();

            var full_lib_names = from m in _loadedModuleNames
                                 from l in libraries
                                 where l.Contains(m)
                                 select l;

            ///
            /// Before unloading we need to make sure that we aren't
            /// holding onto any pointers back to these guys!
            /// 

            GC.Collect();
            GC.WaitForPendingFinalizers();

            ///
            /// Now that we have them, unload them. Since repeated unloading
            /// cases erorr messages to the concole, clear the list so we don't
            /// make a mistake later.
            /// 

            foreach (var m in full_lib_names)
            {
                gSystem.Unload(m);
            }

            _loadedModuleNames.Clear();
        }

        /// <summary>
        /// Keep track of local init for this one executor.
        /// </summary>
        private bool _executorInited = false;

        private static HashSet<string> _gAutoGeneratedClasses = new HashSet<string>();

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
        /// True after we've done pre-execution initalization.
        /// </summary>
        private bool _preExeInitDone = false;

        /// <summary>
        /// This init needs to be done before we actually compile anything!
        /// </summary>
        private void PreExecutionInit()
        {
            if (_preExeInitDone)
                return;
            _preExeInitDone = true;

            ///
            /// Generate any dictionaries that are requested.
            /// 

            foreach (var clsPair in _classToDictify)
            {
                if (!_gAutoGeneratedClasses.Contains(clsPair[0]))
                {
                    _gAutoGeneratedClasses.Add(clsPair[0]);
                    if (string.IsNullOrWhiteSpace(clsPair[1]))
                    {
                        ROOTNET.NTInterpreter.Instance().GenerateDictionary(clsPair[0]);
                    }
                    else
                    {
                        ROOTNET.NTInterpreter.Instance().GenerateDictionary(clsPair[0], clsPair[1]);
                    }
                }
            }
        }

        /// <summary>
        /// Get/Set query cleanup control. If false, the files won't be deleted.
        /// </summary>
        public bool CleanupQuery { get; set; }

        /// <summary>
        /// Get/Set query cache control. If set true then the query cache will be ignored and all quieries will be re-run.
        /// </summary>
        public bool IgnoreQueryCache { get; set; }

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
        private T ExtractResult<T>(IVariable iVariable, IQueryResultCacheKey key, IDictionary<string, ROOTNET.Interface.NTObject> results)
        {
            ///
            /// Load the object and try to extract whatever info we need to from it
            /// 

            if (!results.Keys.Contains(iVariable.RawValue))
                throw new InvalidOperationException(string.Format("The result list from the query did not contains an object named '{0}'.", iVariable.RawValue));

            var o = results[iVariable.RawValue];

            _cache.CacheItem(key, o);
            var s = _varSaver.Get(iVariable);
            return s.LoadResult<T>(iVariable, o);
        }

        /// <summary>
        /// We actually run the query!
        /// </summary>
        /// <param name="tSelectorClassName">Name of the TSelector object</param>
        /// <param name="outputFileInfo">Where the output results should be written for eventual reading</param>
        private IDictionary<string, ROOTNET.Interface.NTObject> RunNtupleQuery(string tSelectorClassName, IEnumerable<KeyValuePair<string, object>> variablesToLoad)
        {
            //
            // This gets in the way of multi-threading, unfortunately!
            //

            ROOTNET.NTH1.AddDirectory(false);

            ///
            /// Create the chain and load file files into it.
            /// 

            var subjobs = (from f in _rootFiles.AsParallel()
                           let r = RunNtupleQueryOnTree(f, variablesToLoad, tSelectorClassName)
                           where r != null
                           select r).ToArray();

            if (subjobs.Length == 0)
                throw new InvalidOperationException("No files return results for the query!");

            //
            // Avoid the combination if we don't need it!
            //

            if (subjobs.Length == 1)
            {
                return subjobs[0];
            }

            //
            // Now we have the complete results from the sub-job's. We have to combine them!
            // Assume the first file has every object we need and start from there.
            //

            var firstFile = subjobs[0];
            var objCollection = from sjResults in subjobs
                                from keyvalue in sjResults
                                group keyvalue.Value by keyvalue.Key;

            var results = new Dictionary<string, ROOTNET.Interface.NTObject>();
            foreach (var objList in objCollection)
            {
                results[objList.Key] = CombineObjectList(objList);
            }

            return results;
        }

        /// <summary>
        /// Given a list of objects attempt to merge them into a single one and return the combined object. Note
        /// that we modify the first object in the list by doing the merge!
        /// </summary>
        /// <param name="objList"></param>
        /// <returns></returns>
        private ROOTNET.Interface.NTObject CombineObjectList(IEnumerable<ROOTNET.Interface.NTObject> objList)
        {
            //
            // Create the list we will be passing along!
            //

            var list = new ROOTNET.NTList();
            list.SetOwner(false);
            foreach (var item in objList.Skip(1))
            {
                list.Add(item);
            }

            //
            // This requires some dynamic shinanigins. We don't have that implemented right now, so we will just assume this is
            // a histogram and fail otherwise! :-)
            //

            var ash = objList.First() as ROOTNET.Interface.NTH1;
            if (ash == null)
                throw new InvalidOperationException(string.Format("Unable to merge objects of type '{0}'.", ash.GetType().Name));

            ash.Merge(list);

            return ash;
        }

        /// <summary>
        /// Given an input file and the input variables, run, and return a file that contains
        /// the output of the query. Meant to run safely in a multi-threaded environment.
        /// </summary>
        /// <param name="variablesToLoad"></param>
        /// <param name="selectorClassName"></param>
        /// <param name="inputfile"></param>
        /// <returns></returns>
        private IDictionary<string, ROOTNET.Interface.NTObject> RunNtupleQueryOnTree(FileInfo inputFile, IEnumerable<KeyValuePair<string, object>> variablesToLoad, string selectorClassName)
        {
            //
            // We will have to weave in and out of some objects lifetimes
            // and a lock on opening the input files. As a result, we need
            // to declare some variables at the top.
            //

            ROOTNET.Interface.NTFile inputROOTFile = null;
            ROOTNET.Interface.NTTree tree = null;
            ROOTNET.Interface.NTList objInputList = null;
            ROOTNET.Interface.NTSelector selector = null;

            //
            // Create the output file that we will be writing the results to...
            //

            string outputFile = string.Format(@"{0}/sub_{1}.root", GetQueryDirectory().FullName, Path.GetRandomFileName());

            //
            // For opening the files and loading the tree we need to make sure that nothing else
            // is going on. This is, I think, a CINT issue: meta data for the tree gets loaded and
            // that can corrupt things. Lets hope! This is from some discussions with Philippe Canal.
            //

            lock (this)
            {
                //
                // WARNING: inside this lock make sure we don't have any new variables for objects that
                // must reamin around after the lock scope terminates (for the query). For example,
                // it might be possible that the objInputList, though not needed outside this lock block,
                // its scoping might be such that if the GC were to run exactly at the right spot, it might
                // be removed and deleted, leaving a dangling C++ reference.
                //

                TraceHelpers.TraceInfo(18, "RunNtupleQuery: Startup - doing selector lookup");
                var cls = ROOTNET.NTClass.GetClass(selectorClassName);
                if (cls == null)
                    throw new InvalidOperationException("Unable find class '" + selectorClassName + "' in the ROOT TClass registry that was just successfully compiled - can't run ntuple query - major inconsistency");

                selector = cls.New() as ROOTNET.Interface.NTSelector;

                ///
                /// If there are any objects we need to send to the selector, then send them on now
                /// 

                TraceHelpers.TraceInfo(20, "RunNtupleQuery: Saving the objects we are going to ship over");
                objInputList = new ROOTNET.NTList();
                selector.InputList = objInputList;

                foreach (var item in variablesToLoad)
                {
                    var obj = item.Value as ROOTNET.Interface.NTNamed;
                    if (obj == null)
                        throw new InvalidOperationException("Can only deal with named objects");
                    var cloned = obj.Clone(item.Key);
                    objInputList.Add(cloned);
                }

                //
                // Open the input file and get the tree
                //

                inputROOTFile = ROOTNET.NTFile.Open(inputFile.FullName);

                if (inputROOTFile == null || !inputROOTFile.IsOpen())
                    return null;

                tree = inputROOTFile.Get(_treeName) as ROOTNET.NTTree;
            }

            //
            // Check to see if the file is null...
            //

            try
            {
                if (tree == null)
                    return null;

                //
                // Run the selector on this guy
                //

                TraceHelpers.TraceInfo(21, "RunNtupleQuery: Running TSelector");
                tree.Process(selector);
                TraceHelpers.TraceInfo(22, "RunNtupleQuery: Done");

                //
                // Get the results and put them into a map for safe keeping!
                // Also, since we want the results to live beyond this guy, make sure that when
                // the selector is deleted the objects don't go away!
                //

                var results = new Dictionary<string, ROOTNET.Interface.NTObject>();
                foreach (var o in selector.OutputList)
                {
                    results[o.Name] = o;
                }
                selector.OutputList.SetOwner(false);

                return results;
            }
            finally
            {
                lock (this)
                {
                    inputROOTFile.Close();
                }
            }
        }

        /// <summary>
        /// Keep track of all modules that we've loaded
        /// </summary>
        private List<string> _loadedModuleNames = new List<string>();

        /// <summary>
        /// Compile and load a file
        /// </summary>
        /// <param name="templateRunner"></param>
        private void CompileAndLoad(FileInfo templateRunner)
        {
            var gSystem = ROOTNET.NTSystem.gSystem;

            var result = gSystem.CompileMacro(templateRunner.FullName, "k");

            /// This should never happen - but we are depending on so many different things to go right here!
            if (result != 1)
                throw new InvalidOperationException("Failed to compile '" + templateRunner.FullName + "' - make sure command 'cl' is defined!!!");

            _loadedModuleNames.Add(templateRunner.Name.Replace(".", "_"));
        }

        /// <summary>
        /// If there are some extra files we need to be loading, go after them here.
        /// </summary>
        private void AssembleAndLoadExtraObjects()
        {
            ///
            /// First, do the files that are part of our infrastructure.
            /// 

            var f = CopyToCommonDirectory(GetInfrastructureFile("FlowOutputObject.cpp"));
            CompileAndLoad(f);

            ///
            /// Next, build any files that are required to build run this ntuple
            /// 

            foreach (var fd in _extraComponentFiles)
            {
                var output = CopyToCommonDirectory(fd);
                try
                {
                    CompileAndLoad(output);
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to build {0}. Ignoring and crossing fingers.", output.Name);
                }
            }
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
        private FileInfo WriteTSelector(string proxyFileName, string proxyObjectName, IExecutableCode code)
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
                reader.Close();
            }

            var context = new VelocityContext();
            context.Put("baseClassInclude", proxyFileName);
            context.Put("baseClassName", proxyObjectName);
            context.Put("CINTLines", _cintLines);

            ///
            /// In order to avoid conflicts having to do with root map files pointing to DLL's and some other weird bookeeping things
            /// that root does, we will name the query to be something different each time. This isn't a problem unless something goes
            /// wrong, but it can make whatever went wrong pretty confusing!
            /// 

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

            context.Put("IncludeFiles", code.IncludeFiles);

            ///
            /// Now do it!
            /// 

            var ourSelector = new FileInfo(GetQueryDirectory() + "\\" + queryFileName + ".cxx");
            using (var writer = ourSelector.CreateText())
            {
                eng.Evaluate(context, writer, null, template);
                writer.Close();
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
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LINQToTTree\\Templates\\" + templateName;
        }

        /// <summary>
        /// Get/Set the directory where we create our local query info. Defaults to the users' temp directory.
        /// </summary>
        public static DirectoryInfo QueryCreationDirectory = new DirectoryInfo(Path.GetTempPath() + "\\LINQToTTree");

        /// <summary>
        /// Copy a file to a directory that contains files for this query.
        /// </summary>
        /// <param name="sourceFile"></param>
        private FileInfo CopyToQueryDirectory(FileInfo sourceFile)
        {
            return CopyToDirectory(sourceFile, GetQueryDirectory());
        }

        /// <summary>
        /// Copies a source file to a directory. Also copies over any "valid" includes we can find.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destDirectory"></param>
        private FileInfo CopyToDirectory(FileInfo sourceFile, DirectoryInfo destDirectory)
        {
            ///
            /// See if the dest file is already there. If so, don't copy over
            /// 

            FileInfo destFile = new FileInfo(destDirectory.FullName + "\\" + sourceFile.Name);
            if (destFile.Exists)
            {
                if (destFile.LastWriteTime >= sourceFile.LastWriteTime
                    && destFile.Length == sourceFile.Length)
                {
                    return destFile;
                }
            }
            sourceFile.CopyTo(destFile.FullName, true);

            ///
            /// Next, if there are any include files we need to move
            /// 

            CopyIncludedFilesToDirectory(sourceFile, destDirectory);

            ///
            /// Return what we know!
            /// 

            destFile.Refresh();
            return destFile;
        }

        /// <summary>
        /// Copy over any files that are included in the source file to the destination
        /// directory
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destDirectory"></param>
        private void CopyIncludedFilesToDirectory(FileInfo sourceFile, DirectoryInfo destDirectory)
        {
            var includeFiles = FindIncludeFiles(sourceFile);
            var goodIncludeFiles = from f in includeFiles
                                   where !Path.IsPathRooted(f)
                                   let full = new FileInfo(sourceFile.DirectoryName + "\\" + f)
                                   where full.Exists
                                   select full;

            foreach (var item in goodIncludeFiles)
            {
                CopyToDirectory(item, destDirectory);
            }
        }

        /// <summary>
        /// Copy this source file (along with any includes in it) to
        /// our common area.
        /// </summary>
        /// <param name="sourceFile"></param>
        private FileInfo CopyToCommonDirectory(FileInfo sourceFile)
        {
            return CopyToDirectory(sourceFile, CommonSourceDirectory());
        }

        /// <summary>
        /// Return the include files that we find in this guy.
        /// </summary>
        /// <param name="_proxyFile"></param>
        /// <returns></returns>
        private IEnumerable<string> FindIncludeFiles(FileInfo _proxyFile)
        {
            Regex reg = new Regex("#include \"(?<file>[^\"]+)\"");
            using (var reader = _proxyFile.OpenText())
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        continue;

                    var m = reg.Match(line);
                    if (m.Success)
                    {
                        var s = m.Groups["file"].Value;
                        yield return s;
                    }
                }
            }
        }

        /// <summary>
        /// The local query
        /// </summary>
        private DirectoryInfo _queryDirectory;

        /// <summary>
        /// When true, we will first attempt to delete all files in our main query directory
        /// scratch space before creating and running everything.
        /// </summary>
        static bool gFirstQuerySetup = true;

        /// <summary>
        /// Get the directory for the current query in progress. If this is the first time
        /// we attempt to first clean up from the last program run.
        /// </summary>
        /// <returns></returns>
        private DirectoryInfo GetQueryDirectory()
        {
            if (_queryDirectory == null)
            {
                var baseQueryDirectory = new DirectoryInfo(QueryCreationDirectory.FullName + "\\" + Process.GetCurrentProcess().ProcessName);
                if (gFirstQuerySetup)
                {
                    gFirstQuerySetup = false;
                    try
                    {
                        baseQueryDirectory.Delete(true);
                    }
                    catch { }
                }
                _queryDirectory = new DirectoryInfo(baseQueryDirectory.FullName + "\\" + Path.GetRandomFileName());
                _queryDirectory.Create();
            }
            return _queryDirectory;
        }

        /// <summary>
        /// When the class has been initalized, we set this to true. Make sure we run MEF.
        /// </summary>
        static CompositionContainer _gContainer = null;

        /// <summary>
        /// The location where we put temp files we need to build against, etc. and then
        /// ship off and run... No perm stuff here (so no results, etc.).
        /// </summary>
        public static DirectoryInfo TempDirectory = null;

        /// <summary>
        /// Run init for this class.
        /// </summary>
        private void Init()
        {
            if (_gContainer != null)
                return;

            ///
            /// Get MEF setup with everything in our assembly.
            /// 

            AggregateCatalog aggCat = new AggregateCatalog();
            aggCat.Catalogs.Add(new AssemblyCatalog(Assembly.GetCallingAssembly()));
            _gContainer = new CompositionContainer(aggCat);
            CompositionBatch b = new CompositionBatch();
            b.AddPart(new TypeHandlers.TypeHandlerCache());
            _gContainer.Compose(b);

            ///
            /// A directory where we can store all of the temp files we need to create
            /// 

            TempDirectory = new DirectoryInfo(Path.GetTempPath() + "\\LINQToROOT");
            if (!TempDirectory.Exists)
            {
                TempDirectory.Create();
                TempDirectory.Refresh();
            }

            ///
            /// Next the common source files. Make sure that the include files passed to the old compiler has
            /// this common file directory in there!
            /// 

            var cf = CommonSourceDirectory();
            if (!cf.Exists)
            {
                cf.Create();
            }

            if (!ROOTNET.NTSystem.gSystem.IncludePath.Contains(cf.FullName))
            {
                ROOTNET.NTSystem.gSystem.AddIncludePath("-I\"" + cf.FullName + "\"");
            }

            ///
            /// Make sure TApplication has been started. It will init a bunch of stuff
            /// 

            ROOTNET.NTApplication.CreateApplication();

            ///
            /// Turn off ROOTMAP generation - since we are constantly unloading things it just gets things
            /// confused. :-)
            /// 

            ROOTNET.NTEnv.gEnv.SetValue("ACLiC.LinkLibs", 1);
        }

        /// <summary>
        /// Generate the common directory. Called only after the temp directory has been created!!
        /// </summary>
        /// <returns></returns>
        private DirectoryInfo CommonSourceDirectory()
        {
            return new DirectoryInfo(TempDirectory.FullName + "\\CommonFiles");
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
