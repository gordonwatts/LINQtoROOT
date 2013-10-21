
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Runs single threaded, in the local process, and does all the ntuples we need.
    /// </summary>
    class LocalExecutor : IQueryExectuor
    {
        /// <summary>
        /// The execution environment
        /// </summary>
        public ExecutionEnvironment Environment { set; get; }

        /// <summary>
        /// Get/Set the list of leaf names that this query references. Used to configure the cache
        /// more efficiently.
        /// </summary>
        public string[] LeafNames { get; set; }

        /// <summary>
        /// Given a request, run it. No need to clean up afterwards as we are already there.
        /// </summary>
        /// <param name="remotePacket">The basic info about this run</param>
        /// <returns></returns>
        public System.Collections.Generic.IDictionary<string, ROOTNET.Interface.NTObject> Execute(
            FileInfo templateFile,
            DirectoryInfo queryDirectory,
            IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            //
            // Get the environment setup for this call
            //

            ExecutionUtilities.Init();
            PreExecutionInit(Environment.ClassesToDictify);

            TraceHelpers.TraceInfo(12, "ExecuteQueuedQueries: Loading all extra objects");
            AssembleAndLoadExtraObjects(Environment.ExtraComponentFiles);

            //
            // Load the query up
            //

            if (Environment.BreakToDebugger)
                System.Diagnostics.Debugger.Break();
            CompileAndLoad(templateFile);

            //
            // To help with possible debugging and other things, if a pdb was generated, then copy it over and rename it
            // correctly.
            //

            if (File.Exists("vc100.pdb"))
            {
                File.Copy("vc100.pdb", Path.Combine(queryDirectory.FullName, Path.GetFileNameWithoutExtension(templateFile.Name) + ".pdb"));
            }

            //
            // Get the file name of the selector.
            //

            TraceHelpers.TraceInfo(14, "ExecuteQueuedQueries: Startup - Running the code");
            var localFiles = Environment.RootFiles.Select(u => new FileInfo(u.LocalPath)).ToArray();
            var results = RunNtupleQuery(Path.GetFileNameWithoutExtension(templateFile.Name), varsToTransfer, Environment.TreeName, localFiles);

            //
            // And cleanup!
            //

            TraceHelpers.TraceInfo(16, "ExecuteQueuedQueries: unloading all results");
            ExecutionUtilities.UnloadAllModules(_loadedModuleNames);
            if (Environment.CleanupQuery)
            {
                queryDirectory.Delete(true);
            }

            return results;
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
            }

            var result = gSystem.CompileMacro(templateRunner.FullName, buildFlags);

            /// This should never happen - but we are depending on so many different things to go right here!
            if (result != 1)
                throw new InvalidOperationException("Failed to compile '" + templateRunner.FullName + "' - This is a very bad internal error - inspect the file to see if you can see what went wrong and report!!!");

            _loadedModuleNames.Add(templateRunner.Name.Replace(".", "_"));
        }

        #endregion
    }
}
