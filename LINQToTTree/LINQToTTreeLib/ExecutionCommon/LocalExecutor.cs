
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

            Init();
            PreExecutionInit(Environment.ClassesToDictify);

            TraceHelpers.TraceInfo(12, "ExecuteQueuedQueries: Loading all extra objects");
            AssembleAndLoadExtraObjects(Environment.ExtraComponentFiles);

            //
            // Load the query up
            //

            CompileAndLoad(templateFile);

            //
            // Get the file name of the selector.
            //

            TraceHelpers.TraceInfo(14, "ExecuteQueuedQueries: Startup - Running the code");
            var results = RunNtupleQuery(Path.GetFileNameWithoutExtension(templateFile.Name), varsToTransfer, Environment.TreeName, Environment.RootFiles);

            //
            // And cleanup!
            //

            TraceHelpers.TraceInfo(16, "ExecuteQueuedQueries: unloading all results");
            UnloadAllModules();
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

            foreach (var item in variablesToLoad)
            {
                var obj = item.Value as ROOTNET.Interface.NTNamed;
                if (obj == null)
                    throw new InvalidOperationException("Can only deal with named objects");
                var cloned = obj.Clone(item.Key);
                objInputList.Add(cloned);
            }

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

            ///
            /// Finally, run the whole thing
            /// 

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

        /// <summary>
        /// Has the global initialization been run for local execution yet?
        /// </summary>
        static bool gGlobalInit = false;

        /// <summary>
        /// The location where we put temp files we need to build against, etc. and then
        /// ship off and run... No perm stuff here (so no results, etc.).
        /// </summary>
        public static DirectoryInfo TempDirectory = null;

        public DirectoryInfo DictDirectory
        {
            get
            {
                return new DirectoryInfo(string.Format(@"{0}\DictGeneration", TempDirectory.FullName));
            }
        }

        /// <summary>
        /// Global init for the image. Things like the temp directory, etc.
        /// </summary>
        private void Init()
        {
            if (gGlobalInit)
                return;
            gGlobalInit = true;

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

            //
            // Make sure the environment is setup correctly!
            //

            SetupENV();
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
                    System.Environment.CurrentDirectory = DictDirectory.FullName;
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
        /// Generate the common directory. Called only after the temp directory has been created!!
        /// </summary>
        /// <returns></returns>
        private DirectoryInfo CommonSourceDirectory()
        {
            return new DirectoryInfo(TempDirectory.FullName + "\\CommonFiles");
        }

        /// <summary>
        /// Copy this source file (along with any includes in it) to
        /// our common area.
        /// </summary>
        /// <param name="sourceFile"></param>
        private FileInfo CopyToCommonDirectory(FileInfo sourceFile)
        {
            return ExecutionUtilities.CopyToDirectory(sourceFile, CommonSourceDirectory());
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
        /// Make sure the environment is setup to run the C++ compiler. If it isn't adjust it.
        /// </summary>
        /// <remarks>
        /// We should be called only once per execution, though I guess we are protected!
        /// </remarks>
        private void SetupENV()
        {
            //
            // If "cl" is already visible, then we don't have to do anything.
            //

            if (FindFileInEnv("PATH", "cl.exe"))
                return;

            //
            // Ok - it isn't in there. Now we need to actually load it in.
            //

            // Get the install directory

            var vcInstallDir = GetVCRegistryEntry(@"Microsoft\VisualStudio\SxS\VC7", "10.0");
            if (vcInstallDir == null)
                throw new NotSupportedException("Visual Studio C++ v10.0 must be installed or already setup otherwise we cannot run!");

            var vsInstallDir = GetVCRegistryEntry(@"Microsoft\VisualStudio\SxS\VS7", "10.0");
            if (vsInstallDir == null)
                throw new NotSupportedException("Visual Studio IDE v10.0 must be installed already otherwise setup cannot run!");

            var winSDKDir = GetVCRegistryEntry(@"Microsoft\Microsoft SDKs\Windows\v7.0A", "InstallationFolder");
            if (winSDKDir == null)
                throw new NotSupportedException("Unable to locate the windows SDK directory to link against! Cannot run!");

            AddToPathEnv("PATH", string.Format(@"{0}\bin", vcInstallDir));
            AddToPathEnv("PATH", string.Format(@"{0}\Common7\IDE", vsInstallDir));
            AddToPathEnv("INCLUDE", string.Format(@"{0}\include", winSDKDir));
            AddToPathEnv("INCLUDE", string.Format(@"{0}\include", vcInstallDir));
            AddToPathEnv("LIB", string.Format(@"{0}\lib", winSDKDir));
            AddToPathEnv("LIB", string.Format(@"{0}\lib", vcInstallDir));
            AddToPathEnv("LIBPATH", string.Format(@"{0}\lib", vcInstallDir));

            if (!FindFileInEnv("PATH", "cl.exe"))
                throw new InvalidOperationException("Despite defining PATH variabels to the compiler, we can't find cl.exe!");
        }

        /// <summary>
        /// Add to a semi-colon seperated environment variable
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        private void AddToPathEnv(string envName, string newPath)
        {
            if (!Directory.Exists(newPath))
                throw new InvalidOperationException(string.Format("Path does not exist - will not add '{0}'", newPath));

            var oldEnv = System.Environment.GetEnvironmentVariable(envName);
            var newEnv = newPath.Replace(@"\\", @"\");
            if (oldEnv != null)
                newEnv = String.Format("{0};{1}", newEnv, oldEnv);

            System.Environment.SetEnvironmentVariable(envName, newEnv);
        }

        /// <summary>
        /// Search the registry entry for a particular key to load.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        /// <returns></returns>
        private string GetVCRegistryEntry(string baseRegPath, string keyName)
        {
            // A little tricky because we have 64 bit and 32 bit stuff!

            var r = GetVCRegistryEntryAbs(string.Format(@"SOFTWARE\{0}", baseRegPath), keyName);
            if (r == null)
                return GetVCRegistryEntryAbs(string.Format(@"SOFTWARE\Wow6432Node\{0}", baseRegPath), keyName);
            return r;
        }

        /// <summary>
        /// Load in the proper registry item here
        /// </summary>
        /// <param name="regPath"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        private string GetVCRegistryEntryAbs(string regPath, string keyName)
        {
            using (var reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(regPath, false))
            {
                if (reg == null)
                    return null;
                var regv = reg.GetValue(keyName);
                return regv as string;
            }
        }

        private object GetVCRegistryEntryAbs(string p)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search the environment for a file.
        /// </summary>
        /// <param name="envVariable">Name of the environment variable, with paths, seperated by semi-colons.</param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private bool FindFileInEnv(string envVariable, string filename)
        {
            var foundFiles = from dir in System.Environment.GetEnvironmentVariable(envVariable).Split(';')
                             let fstr = string.Format(@"{0}\{1}", dir, filename)
                             where File.Exists(fstr)
                             select fstr;
            return foundFiles.Any();
        }

        #endregion
    }
}
