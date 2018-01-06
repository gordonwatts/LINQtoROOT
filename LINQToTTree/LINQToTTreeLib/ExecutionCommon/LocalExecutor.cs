
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Craete the executor when needed
    /// </summary>
    [Export(typeof(IQueryExecutorFactory))]
    public class LocalExecutorFactory : IQueryExecutorFactory
    {
        public string Scheme => "file";

        public IQueryExectuor Create(IExecutionEnvironment exeReq, string[] referencedLeafNames)
        {
            return new LocalExecutor() { Environment = exeReq, LeafNames = referencedLeafNames };
        }
    }

    /// <summary>
    /// Runs single threaded, in the local process, and does all the ntuples we need.
    /// This is married to the version of root that LINQToTTree is built against.
    /// </summary>
    class LocalExecutor : IQueryExectuor
    {
        /// <summary>
        /// The execution environment
        /// </summary>
        public IExecutionEnvironment Environment { set; get; }

        /// <summary>
        /// Get/Set the list of leaf names that this query references. Used to configure the cache
        /// more efficiently.
        /// </summary>
        public string[] LeafNames { get; set; }

        /// <summary>
        /// Used to make sure that we don't try to run more than one at a time.
        /// </summary>
        private AsyncLock _globalLock = new AsyncLock();

        /// <summary>
        /// Given a request, run it. No need to clean up afterwards as we are already there.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// We can't run more than one of these at a time as root is not able to
        /// run more than a thread at a time without significant work. Other
        /// executors get to take care of this, sadly.
        /// </remarks>
        public async Task<IDictionary<string, ROOTNET.Interface.NTObject>> Execute(
            Uri[] files,
            FileInfo templateFile,
            DirectoryInfo queryDirectory,
            IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            using (var waiter = await _globalLock.LockAsync())
            {
                // Remove flag characters in teh source file - parse for clean up subsitutions we can do
                // only when we know how we are going to do the execution.
                ReWritePathsInQuery(templateFile);

                // Get the environment setup for this call
                await ExecutionUtilities.Init();
                PreExecutionInit(Environment.ClassesToDictify);

                TraceHelpers.TraceInfo(12, "ExecuteQueuedQueries: Loading all extra objects");
                AssembleAndLoadExtraObjects(Environment.ExtraComponentFiles);

                //
                // Load the query up
                //

                if (Environment.BreakToDebugger)
                    System.Diagnostics.Debugger.Break();
                CompileAndLoad(templateFile);

                try
                {
                    // To help with possible debugging and other things, if a pdb was generated, then copy it over and rename it
                    // correctly.
                    if (File.Exists("vc100.pdb"))
                    {
                        File.Copy("vc100.pdb", Path.Combine(queryDirectory.FullName, Path.GetFileNameWithoutExtension(templateFile.Name) + ".pdb"));
                    }

                    // Get the file name of the selector.
                    TraceHelpers.TraceInfo(14, "ExecuteQueuedQueries: Startup - Running the code");
                    var localFiles = files.Select(u => new FileInfo(u.LocalPath)).ToArray();

                    return RunNtupleQuery(Path.GetFileNameWithoutExtension(templateFile.Name), varsToTransfer, Environment.TreeName, localFiles);
                }
                finally
                {
                    // And cleanup (even if something goes wrong during the run).
                    TraceHelpers.TraceInfo(16, "ExecuteQueuedQueries: unloading all results");
                    ExecutionUtilities.UnloadAllModules(_loadedModuleNames);
                    if (Environment.CleanupQuery)
                    {
                        queryDirectory.Delete(true);
                    }
                }
            }
        }

        /// <summary>
        /// Look through each input line for a path in the query that needs to be "fixed up".
        /// </summary>
        /// <param name="queryFile"></param>
        private void ReWritePathsInQuery(FileInfo queryFile)
        {
            var tmpFile = new FileInfo($"{queryFile.FullName}-tmp");
            var replacement = new Regex("<><>(.*)<><>");
            using (var writer = tmpFile.CreateText())
            {
                foreach (var line in queryFile.EnumerateTextFile())
                {
                    var wline = line;
                    var m = replacement.Match(line);
                    if (m.Success)
                    {
                        var fixedFile = m.Groups[1].Value;
                        wline = wline.Replace(m.Value, fixedFile);
                    }
                    writer.WriteLine(wline);
                }
                writer.Close();
            }
            queryFile.Delete();
            tmpFile.MoveTo(queryFile.FullName);
        }
        
        /// <summary>
        /// The detailed code that runs the query.
        /// </summary>
        /// <param name="tSelectorClassName">Name of the TSelector object</param>
        /// <param name="outputFileInfo">Where the output results should be written for eventual reading</param>
        private Dictionary<string, ROOTNET.Interface.NTObject> RunNtupleQuery(string tSelectorClassName, IEnumerable<KeyValuePair<string, object>> variablesToLoad,
            string treeName, FileInfo[] rootFiles)
        {
            ///
            /// Create a new TSelector to run
            /// 

            TraceHelpers.TraceInfo(18, "RunNtupleQuery: Startup - doing selector lookup");
            var cls = ROOTNET.NTClass.GetClass(tSelectorClassName);
            if (cls == null)
                throw new InvalidOperationException("Unable find class '" + tSelectorClassName + "' in the ROOT TClass registry that was just successfully compiled - can't run ntuple query - major inconsistency");

            var selector = cls.New() as ROOTNET.Interface.NTSelector;

            ///
            /// Create the chain and load file files into it.
            /// 

            TraceHelpers.TraceInfo(19, "RunNtupleQuery: Creating the TChain");
            var tree = new ROOTNET.NTChain(treeName);
            foreach (var f in rootFiles)
            {
                tree.Add(f.FullName);
            }

            ///
            /// If there are any objects we need to send to the selector, then send them on now
            /// 

            TraceHelpers.TraceInfo(20, "RunNtupleQuery: Saving the objects we are going to ship over");
            var objInputList = new ROOTNET.NTList();
            selector.InputList = objInputList;

            var oldHSet = ROOTNET.NTH1.AddDirectoryStatus();
            ROOTNET.NTH1.AddDirectory(false);
            foreach (var item in variablesToLoad)
            {
                var obj = item.Value as ROOTNET.Interface.NTNamed;
                if (obj == null)
                    throw new InvalidOperationException("Can only deal with named objects");
                var cloned = obj.Clone(item.Key);
                objInputList.Add(cloned);
            }
            ROOTNET.NTH1.AddDirectory(oldHSet);

            //
            // Setup the cache for more efficient reading. We assume we are on a machine with plenty of memory
            // for this.
            //

            tree.CacheSize = 1024 * 1024 * 100; // 100 MB cache
            if (LeafNames == null)
            {
                tree.AddBranchToCache("*", true);
            }
            else
            {
                foreach (var leaf in LeafNames)
                {
                    tree.AddBranchToCache(leaf, true);
                }
            }
            tree.StopCacheLearningPhase();

            // Always Do the async prefetching (this is off by default for some reason, but...).

            ROOTNET.Globals.gEnv.Value.SetValue("TFile.AsynchPrefetching", 1);

            ///
            /// Finally, run the whole thing
            /// 

            TraceHelpers.TraceInfo(21, "RunNtupleQuery: Running TSelector");
            if (Environment.BreakToDebugger)
                System.Diagnostics.Debugger.Break();
            tree.Process(selector);
            TraceHelpers.TraceInfo(22, "RunNtupleQuery: Done");

            // If debug, dump some stats...

            if (Environment.CompileDebug)
            {
                tree.PrintCacheStats();
            }

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

        #region Support Routines
        /// <summary>
        /// True after we've done pre-execution initalization.
        /// </summary>
        private bool _preExeInitDone = false;

        /// <summary>
        /// Keep track of what classes we've auto-generated.
        /// </summary>
        private static HashSet<string> _gAutoGeneratedClasses = new HashSet<string>();

        /// <summary>
        /// This init needs to be done before we actually compile anything!
        /// </summary>
        private void PreExecutionInit(string[][] classesToDictify)
        {
            if (_preExeInitDone)
                return;
            _preExeInitDone = true;

            ///
            /// Generate any dictionaries that are requested. We do this in the root
            /// temp directory rather than the local directory.
            /// 

            foreach (var clsPair in classesToDictify)
            {
                if (!_gAutoGeneratedClasses.Contains(clsPair[0]))
                {
                    _gAutoGeneratedClasses.Add(clsPair[0]);
                    var curDir = System.Environment.CurrentDirectory;
                    System.Environment.CurrentDirectory = ExecutionUtilities.DictDirectory.FullName;
                    try
                    {
                        if (string.IsNullOrWhiteSpace(clsPair[1]))
                        {
                            ROOTNET.NTInterpreter.Instance().GenerateDictionary(clsPair[0]);
                        }
                        else
                        {
                            ROOTNET.NTInterpreter.Instance().GenerateDictionary(clsPair[0], clsPair[1]);
                        }
                    }
                    finally
                    {
                        System.Environment.CurrentDirectory = curDir;
                    }
                }
            }
        }

        /// <summary>
        /// If there are some extra files we need to be loading, go after them here.
        /// </summary>
        private void AssembleAndLoadExtraObjects(FileInfo[] extraComponentFiles)
        {
            ///
            /// First, do the files that are part of our infrastructure.
            /// We currently have no infrastructure files needed.
            /// 

            ///
            /// Next, build any files that are required to build run this ntuple
            /// 

            foreach (var fd in extraComponentFiles)
            {
                var output = ExecutionUtilities.CopyToCommonDirectory(fd);
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
        /// Keep track of all modules that we've loaded
        /// </summary>
        private List<string> _loadedModuleNames = new List<string>();

        /// <summary>
        /// Thrown when we can't compile a query - this is really bad!
        /// </summary>
        [Serializable]
        public class FailedToCompileException : Exception
        {
            public FailedToCompileException() { }
            public FailedToCompileException(string message) : base(message) { }
            public FailedToCompileException(string message, Exception inner) : base(message, inner) { }
            protected FailedToCompileException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Compile and load a file
        /// </summary>
        /// <param name="templateRunner"></param>
        private void CompileAndLoad(FileInfo templateRunner)
        {
            var gSystem = ROOTNET.NTSystem.gSystem;

            string buildFlags = "k";
            if (Environment.CompileDebug)
            {
                buildFlags += "g";
                gSystem.FlagsDebug = "-Zi";
                gSystem.FlagsOpt = "-Zi";
            }

            var result = gSystem.CompileMacro(templateRunner.FullName, buildFlags);

            /// This should never happen - but we are depending on so many different things to go right here!
            if (result != 1)
                throw new FailedToCompileException("Failed to compile '" + templateRunner.FullName + "' - This is a very bad internal error - inspect the file to see if you can see what went wrong and report!!!");

            _loadedModuleNames.Add(templateRunner.Name.Replace(".", "_"));
        }

        /// <summary>
        /// Thrown if we can't find a tree we need to fine.
        /// </summary>
        [Serializable]
        public class TreeDoesNotExistException : Exception
        {
            public TreeDoesNotExistException() { }
            public TreeDoesNotExistException(string message) : base(message) { }
            public TreeDoesNotExistException(string message, Exception inner) : base(message, inner) { }
            protected TreeDoesNotExistException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// We must build a proxy file for use with a query. Use the MakeProxy guy to do this.
        /// </summary>
        /// <param name="rootFiles"></param>
        /// <param name="queryDirectory"></param>
        /// <returns></returns>
        public async Task<FileInfo> GenerateProxyFile(Uri[] rootFiles, string treeName, DirectoryInfo queryDirectory)
        {
            using (var lockWaiter = await _globalLock.LockAsync())
            {
                // Argument checks
                if (rootFiles == null || rootFiles.Length == 0)
                {
                    throw new ArgumentException("Query must be run on some files - argument to GenerateProxyFile was null or zero length");
                }
                if (queryDirectory == null || !queryDirectory.Exists)
                {
                    throw new ArgumentException("The directory were we should create a TTree proxy file should not be null or non-existant!");
                }

                // Open the first file and generate the proxy from that.
                var rootFilePath = rootFiles.First().LocalPath;
                var tfile = ROOTNET.NTFile.Open(rootFilePath, "READ");
                try
                {
                    var tree = tfile.Get(treeName) as ROOTNET.Interface.NTTree;
                    if (tree == null)
                    {
                        throw new TreeDoesNotExistException($"Unable to fine tree '{treeName}' in file '{rootFilePath}'");
                    }

                    // Root does everything local, so...
                    var oldEnv = System.Environment.CurrentDirectory;
                    try
                    {
                        System.Environment.CurrentDirectory = queryDirectory.FullName;

                        // Write the dummy selection file that is required (WHY!!!???).
                        using (var w = File.CreateText("junk.C"))
                        {
                            w.Write("int junk() {return 10.0;}");
                            w.Close();
                        }

                        tree.MakeProxy("runquery", "junk.C", null, "nohist");
                        return new FileInfo("runquery.h");
                    }
                    finally
                    {
                        System.Environment.CurrentDirectory = oldEnv;
                    }
                }
                finally
                {
                    tfile.Close();
                }
            }
        }

        /// <summary>
        /// We can only deal with one thing at a time - so no need to split up the incoming flies.
        /// </summary>
        /// <param name="rootFiles"></param>
        /// <returns></returns>
        public int SuggestedNumberOfSimultaniousProcesses(Uri[] rootFiles)
        {
            return 1;
        }

        /// <summary>
        /// We do not need to split things up, so we return the full thing.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public IEnumerable<Uri[]> BatchInputUris(Uri[] files)
        {
            return new[] { files };
        }

        #endregion
    }
}
