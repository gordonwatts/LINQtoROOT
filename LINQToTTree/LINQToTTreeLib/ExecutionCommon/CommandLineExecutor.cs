using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROOTNET.Interface;
using System.Diagnostics;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Execute via the command line ROOT locally.
    /// 1. Move files to a set of locally known placeses
    /// 2. Write a .C script
    /// 3. Run Root and execute that .C script in a sub-process.
    /// 4. Grab the return file and load up the objects and return them.
    /// </summary>
    public class CommandLineExecutor : IQueryExectuor
    {
        /// <summary>
        /// The environment we will use when it is time to execute everything
        /// </summary>
        public ExecutionEnvironment Environment { get; set; }

        /// <summary>
        /// Get/Set the list of leaf names that this query references. Used to configure the cache
        /// more efficiently.
        /// </summary>
        public string[] LeafNames { get; set; }

        /// <summary>
        /// List of things to do with log info coming back
        /// </summary>
        private static List<Action<string>> _logDumpers = new List<Action<string>>();

        /// <summary>
        /// Add a line dumper
        /// </summary>
        /// <param name="logger"></param>
        public static void AddLogEndpoint (Action<string> logger)
        {
            _logDumpers.Add(logger);
        }

        /// <summary>
        /// Reset the executor to its initial state.
        /// </summary>
        public static void ResetCommandLineExecutor()
        {
            _logDumpers = new List<Action<string>>();
        }

        /// <summary>
        /// Package everythiing up and run it.
        /// </summary>
        /// <param name="queryFile">The C++ file we are going to run the query against</param>
        /// <param name="queryDirectory">Directory where we run the query</param>
        /// <param name="varsToTransfer">Variables we need to move over to the query</param>
        /// <returns></returns>
        public IDictionary<string, NTObject> Execute(FileInfo queryFile, DirectoryInfo queryDirectory, IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            // Setup for building a command
            ExecutionUtilities.Init();
            var cmds = new StringBuilder();

            // Load up extra objects & dictionaries
            LoadExtraCPPFiles(queryDirectory, cmds);
            LoadExtraDictionaries(Environment.ClassesToDictify, cmds);

            // Compile the macro
            CompileAndLoad(queryFile, cmds);

            // Run the query
            var localFiles = Environment.RootFiles.Select(u => new FileInfo(u.LocalPath)).ToArray();
            var resultsFile = new FileInfo(Path.Combine(queryDirectory.FullName, "selector_results.root"));
            RunNtupleQuery(cmds, resultsFile, Path.GetFileNameWithoutExtension(queryFile.Name), varsToTransfer, Environment.TreeName, localFiles);

            // Run the root script
            cmds.AppendLine("exit(0);");
            ExecuteRootScript("RunTSelector", cmds, queryDirectory);

            // Get back results
            var results = LoadSelectorResults(resultsFile);

            // Clean up
            CleanUpQuery(queryDirectory);

            return results;
        }

        /// <summary>
        /// Sometimes we have to generate some class dictionaries on the fly. This code will do that.
        /// </summary>
        /// <param name="classesToDictify"></param>
        /// <param name="cmds"></param>
        private void LoadExtraDictionaries(string[][] classesToDictify, StringBuilder cmds)
        {
            if (classesToDictify != null)
            {
                foreach (var clsPair in classesToDictify)
                {
                    if (string.IsNullOrWhiteSpace(clsPair[1]))
                    {
                        cmds.AppendLine($"gInterpreter->GenerateDictionary(\"{clsPair[0]}\");");
                    }
                    else
                    {
                        cmds.AppendLine($"gInterpreter->GenerateDictionary(\"{clsPair[0]}\", \"{clsPair[1]}\");");
                    }
                }
            }
        }

        /// <summary>
        /// Compile/Load in extra C++ files and move the include files locally so they can be easily
        /// referenced by the query code.
        /// </summary>
        /// <param name="queryDirectory">Location where we move the files to compiling</param>
        private void LoadExtraCPPFiles(DirectoryInfo queryDirectory, StringBuilder cmds)
        {
            // Move everything over, and then compile!
            if (Environment.ExtraComponentFiles != null)
            {
                foreach (var fd in Environment.ExtraComponentFiles)
                {
                    var output = ExecutionUtilities.CopyToDirectory(fd, queryDirectory);
                    try
                    {
                        CompileAndLoad(output, cmds);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to build {0}. Ignoring and crossing fingers.", output.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Clean up the query - keep user's disk clean!
        /// </summary>
        private void CleanUpQuery(DirectoryInfo queryDirectory)
        {
            TraceHelpers.TraceInfo(16, "ExecuteQueuedQueries: unloading all results");
            if (Environment.CleanupQuery)
            {
                queryDirectory.Delete(true);
            }
        }

        /// <summary>
        /// Emit the code to run the actual query
        /// </summary>
        /// <param name="v"></param>
        /// <param name="varsToTransfer"></param>
        /// <param name="treeName"></param>
        /// <param name="localFiles"></param>
        /// <returns></returns>
        private void RunNtupleQuery(StringBuilder cmds, FileInfo queryResultsFile, string selectClass, IEnumerable<KeyValuePair<string, object>> varsToTransfer, string treeName, FileInfo[] localFiles)
        {
            // Init the selector
            cmds.AppendLine($"selector = new {selectClass}();");
            WriteInputVariablesForTransfer(cmds, queryResultsFile, varsToTransfer);

            // Get the root files all into a chain
            cmds.AppendLine($"t = new TChain(\"{treeName}\");");
            foreach (var f in localFiles)
            {
                var fname = f.FullName.Replace("\\", "\\\\");
                cmds.AppendLine($"t->Add(\"{fname}\");");
            }

            // Configure the TTree cache
            ConfigureTTreeReaderCache(cmds);

            // Always Do the async prefetching (this is off by default for some reason, but...).
            cmds.AppendLine("gEnv->SetValue(\"TFile.AsynchPrefetching\", 1);");

            // Run the whole thing
            cmds.AppendLine("cout << \"Starting run...\" << endl;");
            cmds.AppendLine($"t->Process(selector);");
            cmds.AppendLine("cout << \"Done with run...\" << endl;");

            // If debug, dump some stats...
            if (Environment.CompileDebug)
            {
                cmds.AppendLine("cout << \"Printing TTree Cache Statistics\" << endl;");
                cmds.AppendLine("t->PrintCacheStats();");
            }

            // Get the results and put them into a map for safe keeping!
            // To move them back we need to use a TFile.
            var resultfileFullName = queryResultsFile.FullName.Replace("\\", "\\\\");
            cmds.AppendLine($"rf = TFile::Open(\"{resultfileFullName}\", \"RECREATE\");");
            cmds.AppendLine("rf->WriteTObject(selector->GetOutputList(), \"output\");");
            cmds.AppendLine("rf->Close();");
        }

        /// <summary>
        /// Configure the TTree cache for fast operation.
        /// </summary>
        /// <param name="cmds"></param>
        private void ConfigureTTreeReaderCache(StringBuilder cmds)
        {
            cmds.AppendLine("t->SetCacheSize(1024 * 1024 * 100); // 100 MB cache");
            if (LeafNames == null)
            {
                cmds.AppendLine("t->AddBranchToCache(\"*\", true);");
            }
            else
            {
                foreach (var leaf in LeafNames)
                {
                    cmds.AppendLine($"t->AddBranchToCache(\"{leaf}\", true);");
                }
            }
            cmds.AppendLine("t->StopCacheLearningPhase();");
        }

        /// <summary>
        /// Put all variables we have to move over to the running root in a file so it can be open and read.
        /// </summary>
        /// <param name="cmds"></param>
        /// <param name="queryResultsFile"></param>
        /// <param name="varsToTransfer"></param>
        private static void WriteInputVariablesForTransfer(StringBuilder cmds, FileInfo queryResultsFile, IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            // Objects that are headed over need to be stored in a file and then loaded into the selector.
            if (varsToTransfer != null && varsToTransfer.Count() > 0)
            {
                TraceHelpers.TraceInfo(20, "RunNtupleQuery: Saving the objects we are going to ship over");
                var inputFilesFilename = new FileInfo(Path.Combine(queryResultsFile.DirectoryName, "TSelectorInputFiles.root"));
                var outgoingVariables = ROOTNET.NTFile.Open(inputFilesFilename.FullName, "RECREATE");

                var safeInputFilename = inputFilesFilename.FullName.Replace("\\", "\\\\");
                cmds.AppendLine($"varsInFile = TFile::Open(\"{safeInputFilename}\", \"READ\");");
                cmds.AppendLine("selector->SetInputList(new TList());");

                var objInputList = new ROOTNET.NTList();
                var oldHSet = ROOTNET.NTH1.AddDirectoryStatus();
                try
                {
                    ROOTNET.NTH1.AddDirectory(false);
                    foreach (var item in varsToTransfer)
                    {
                        var obj = item.Value as ROOTNET.Interface.NTObject;
                        var cloned = obj.Clone(item.Key);
                        outgoingVariables.WriteTObject(cloned);
                        cmds.AppendLine($"selector->GetInputList()->Add(varsInFile->Get(\"{item.Key}\"));");
                    }
                }
                finally
                {
                    ROOTNET.NTH1.AddDirectory(oldHSet);
                    outgoingVariables.Close();
                }
            }
        }

        /// <summary>
        /// Called after running to load the results.
        /// </summary>
        /// <returns></returns>
        private IDictionary<string, NTObject> LoadSelectorResults(FileInfo queryResultsFile)
        {
            if (!queryResultsFile.Exists)
            {
                throw new FileNotFoundException($"Unable to find the file ");
            }

            // Read the data from the file.
            var results = new Dictionary<string, ROOTNET.Interface.NTObject>();
            var f = ROOTNET.NTFile.Open(queryResultsFile.FullName);
            try
            {
                var list = f.Get("output") as NTList;
                foreach (var o in list)
                {
                    results[o.Name] = o;
                }
            }
            finally
            {
                f.Close();
            }

            return results;
        }

        /// <summary>
        /// Generate commands to build and load the template
        /// </summary>
        /// <param name="templateRunner"></param>
        /// <param name="cmds"></param>
        private void CompileAndLoad(FileInfo templateRunner, StringBuilder cmds)
        {
            var gSystem = ROOTNET.NTSystem.gSystem;

            // Extra compile flags
            string buildFlags = "k";
            if (Environment.CompileDebug)
            {
                buildFlags += "g";
                gSystem.FlagsDebug = "-Zi";
                gSystem.FlagsOpt = "-Zi";
            }

            // Code up the call
            var tmpFName = templateRunner.FullName.Replace("\\", "\\\\");
            cmds.AppendLine($"int r = gSystem->CompileMacro(\"{tmpFName}\", \"{buildFlags}\");");
            cmds.AppendLine("if (r != 1) { exit(1); }");
        }

        /// <summary>
        /// This happens when we can't successfully execute a command
        /// </summary>
        [Serializable]
        public class CommandLineExecutionException : Exception
        {
            public CommandLineExecutionException() { }
            public CommandLineExecutionException(string message) : base(message) { }
            public CommandLineExecutionException(string message, Exception inner) : base(message, inner) { }
            protected CommandLineExecutionException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }


        [Serializable]
        public class ROOTExecutableNotFoundException : Exception
        {
            public ROOTExecutableNotFoundException() { }
            public ROOTExecutableNotFoundException(string message) : base(message) { }
            public ROOTExecutableNotFoundException(string message, Exception inner) : base(message, inner) { }
            protected ROOTExecutableNotFoundException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// A list of commands are written to a script and then executed.
        /// We throw if we don't return success
        /// </summary>
        /// <param name="cmds"></param>
        private void ExecuteRootScript(string prefix, StringBuilder cmds, DirectoryInfo tmpDir)
        {
            // Dump the script
            var cmdFile = Path.Combine(tmpDir.FullName, $"{prefix}.C");
            using (var writer = File.CreateText(cmdFile))
            {
                writer.WriteLine($"void {prefix}() {{");
                writer.Write(cmds.ToString());
                writer.WriteLine("}");
            }

            // Figure out where root is that we should be executing against
            var rootPath = System.Environment.ExpandEnvironmentVariables($"%ROOTSYS%\\bin\\root.exe");
            if (!File.Exists(rootPath))
            {
                throw new ROOTExecutableNotFoundException("Unable to find root.exe. This is probably because ROOTSYS is not defined.");
            }

            // Create the process info.
            var proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.ErrorDialog = false;
            proc.StartInfo.FileName = rootPath;
            proc.StartInfo.Arguments = $"-q {cmdFile}";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = tmpDir.FullName;

            // Start it.
            var resultData = new StringBuilder();
            proc.ErrorDataReceived += (sender, e) => RecordLine(resultData, e.Data);
            proc.OutputDataReceived += (sender, e) => RecordLine(resultData, e.Data);

            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            // Wait for it to end.
            proc.WaitForExit();

            // Make sure the result is "good"
            if (proc.ExitCode != 0)
            {
                throw new CommandLineExecutionException($"Failed to execute step {prefix} - process executed with error code {proc.ExitCode}. Text dump from process: {resultData.ToString()}");
            }
        }

        /// <summary>
        /// Careful thread safe recording of a line
        /// </summary>
        /// <param name="resultData">Where we shoudl record it</param>
        /// <param name="line">Line of text to record</param>
        private void RecordLine(StringBuilder resultData, string line)
        {
            if (line == null)
                return;

            lock(resultData)
            {
                resultData.AppendLine(line);
                if (Environment.CompileDebug)
                {
                    Console.WriteLine(line);
                }
                foreach (var logger in _logDumpers)
                {
                    logger(line);
                }
            }
        }
    }
}
