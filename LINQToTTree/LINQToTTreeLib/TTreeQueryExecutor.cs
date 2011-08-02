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
using LINQToTTreeLib.ExecutionCommon;
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
        private ExecutionEnvironment _exeReq = new ExecutionEnvironment();

        /// <summary>
        /// The header file make by the MakeProxy macro.
        /// </summary>
        private FileInfo _proxyFile;

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
        /// We are going to be executing over a particular file and tree
        /// </summary>
        /// <param name="rootFiles"></param>
        /// <param name="treeName"></param>
        public TTreeQueryExecutor(Uri[] rootFiles, string treeName, Type baseNtupleObject)
        {
            TraceHelpers.TraceInfo(2, "Initializing TTreeQueryExecutor");
            CleanupQuery = true;
            IgnoreQueryCache = false;
            RecheckFileDatesOnEachQuery = false;

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
                bld.Append("The following URI(s) do not exist or are not recognized and so can't be processed: ");
                foreach (var f in badFiles)
                {
                    bld.AppendFormat("{0} ", f.LocalPath);
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

            ///
            /// Save the values
            /// 

            _exeReq.RootFiles = rootFiles;
            _exeReq.TreeName = treeName;
            _cintLines = cintLines;
            TraceHelpers.TraceInfo(3, "Done Initializing TTreeQueryExecutor");
        }

        /// <summary>
        /// Check to make sure the URI is a good one. Currently we only deal
        /// with file URI's, so this will x-check that.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private bool UriGood(Uri f)
        {
            if (f.Scheme != "file")
                return false;

            if (!File.Exists(f.LocalPath))
                return false;

            return true;
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
                key = _cache.GetKey(_exeReq.RootFiles, _exeReq.TreeName, inputs, codeContext.CacheCookies.ToArray(), queryModel);
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
            /// Get all the queries together, combined, and ready to run.
            /// 

            TraceHelpers.TraceInfo(11, "ExecuteQueuedQueries: Startup - combining all code");
            var combinedInfo = new CombinedGeneratedCode();
            foreach (var cq in _queuedQueries)
            {
                combinedInfo.AddGeneratedCode(cq.Code);
            }

            ///
            /// Keep track of how often we run. Mostly for testing reasons, actually.
            /// 

            CountExecutionRuns++;

            ///
            /// Now that those are loaded, we need to go after the
            /// proxy. We create a new file which is dependent on the
            /// one we have been given.
            /// 

            TraceHelpers.TraceInfo(13, "ExecuteQueuedQueries: Startup - copying over proxy file");
            var referencedLeafNames = combinedInfo.ReferencedLeafNames.ToArray();
            SlimProxyFile(referencedLeafNames);
            TraceHelpers.TraceInfo(14, "ExecuteQueuedQueries: Startup - building the TSelector");
            var templateRunner = WriteTSelector(_proxyFile.Name, Path.GetFileNameWithoutExtension(_proxyFile.Name), combinedInfo);

            ///
            /// Fantastic! We've made sure everything now can be built locally. Next job, get the run instructions packet
            /// together in order to have them run remotely!
            /// 

            IQueryExectuor local = new LocalExecutor() { Environment = _exeReq, LeafNames = referencedLeafNames };
            var results = local.Execute(templateRunner, GetQueryDirectory(), combinedInfo.VariablesToTransfer);

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

            _queryDirectory = null;
            TraceHelpers.TraceInfo(17, "ExecuteQueuedQueries: Done");
        }

        /// <summary>
        /// Build the execution request - the info that someone else (or us) will need in order to run this request!
        /// </summary>
        /// <returns></returns>
        private ExecutionEnvironment BuildRemoteExecutionRequest()
        {
            var result = new ExecutionEnvironment();

            return result;
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

            ExecutionUtilities.CopyIncludedFilesToDirectory(_proxyFile, GetQueryDirectory());
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
        /// Get/Set query cleanup control. If false, the files won't be deleted.
        /// </summary>
        public bool CleanupQuery
        {
            get { return _exeReq.CleanupQuery; }
            set { _exeReq.CleanupQuery = value; }
        }

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
            return ExecutionUtilities.CopyToDirectory(sourceFile, GetQueryDirectory());
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
