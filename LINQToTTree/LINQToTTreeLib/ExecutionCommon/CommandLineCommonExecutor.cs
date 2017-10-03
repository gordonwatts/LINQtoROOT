using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ROOTNET.Interface;
using System.Threading.Tasks;
using System.Diagnostics;
using LINQToTTreeLib.Utils;
using System.Text.RegularExpressions;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Base class to help with implementing code that needs to be run for a
    /// executor that is based on writing macro files and executing them by
    /// running commands.
    /// </summary>
    public abstract class CommandLineCommonExecutor
    {
        /// <summary>
        /// The execution environment - config for a job we are running
        /// </summary>
        public ExecutionEnvironment Environment { set; get; }

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
        public static void AddLogEndpoint(Action<string> logger)
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
        /// Careful thread safe recording of a line
        /// </summary>
        /// <param name="resultData">Where we shoudl record it</param>
        /// <param name="line">Line of text to record</param>
        protected void RecordLine(StringBuilder resultData, string line)
        {
            if (line == null)
                return;

            lock (resultData)
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

            // Rewrite the query if it contains special file paths
            ReWritePathsInQuery(queryFile);

            // Put our run-directory in the list of includes.
            var includePath = NormalizeFileForTarget(new DirectoryInfo(System.Environment.CurrentDirectory));
            cmds.AppendLine($"gSystem->AddIncludePath(\"-I\\\"{includePath}\\\"\");");

            // Load up extra objects & dictionaries
            LoadExtraCPPFiles(queryDirectory, cmds);
            LoadExtraDictionaries(Environment.ClassesToDictify, cmds);

            // Compile the macro
            CompileAndLoad(queryFile, cmds);

            // Run the query in a second file.
            var subfileCommands = new StringBuilder();
            var localFiles = Environment.RootFiles.Select(u => new FileInfo(u.LocalPath)).ToArray();
            var resultsFile = new FileInfo(Path.Combine(queryDirectory.FullName, "selector_results.root"));
            RunNtupleQuery(subfileCommands, resultsFile, Path.GetFileNameWithoutExtension(queryFile.Name), varsToTransfer, Environment.TreeName, localFiles);

            // Write out the temp file.
            using (var secondFile = File.CreateText(Path.Combine(queryDirectory.FullName, "RunTSelector1.C")))
            {
                secondFile.WriteLine("void RunTSelector1() {");
                secondFile.Write("  " + subfileCommands.ToString());
                secondFile.WriteLine("}");
                secondFile.Close();
            }
            cmds.AppendLine("gROOT->ProcessLine(\".X RunTSelector1.C\");");

            // Run the root script
            cmds.AppendLine("exit(0);");
            ExecuteRootScript("RunTSelector", cmds.ToString(), queryDirectory);

            // Get back results
            var results = LoadSelectorResults(resultsFile);

            // Clean up
            CleanUpQuery(queryDirectory);

            return results;
        }

        /// <summary>
        /// Look through each input line for a path in the query that needs to be "fixed up".
        /// </summary>
        /// <param name="queryFile"></param>
        internal void ReWritePathsInQuery(FileInfo queryFile)
        {
            var tmpFile = new FileInfo($"{queryFile.FullName}-tmp");
            using (var writer = tmpFile.CreateText())
            {
                foreach (var line in ReWritePathInQueryIterator(queryFile.EnumerateTextFile()))
                {
                    writer.WriteLine(line);
                }
                writer.Close();
            }
            queryFile.Delete();
            tmpFile.MoveTo(queryFile.FullName);
        }

        /// <summary>
        /// Rewrite the "file" in memory.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal string ReWritePathsInQuery(string line)
        {
            var writer = new StringBuilder();
            foreach (var tline in ReWritePathInQueryIterator(ChunkStringAsLines(line)))
            {
                writer.AppendLine(tline);
            }
            return writer.ToString();
        }

        /// <summary>
        /// Chunk a string as lines.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private IEnumerable<string> ChunkStringAsLines (string input)
        {
            using (var reader = new StringReader(input))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    yield return line;
                    line = reader.ReadLine();
                }
            }
        }

        /// <summary>
        /// Iterator to help abstract the above
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private IEnumerable<string> ReWritePathInQueryIterator(IEnumerable<string> source)
        {
            var replacement = new Regex("<><>(.*)<><>");
            foreach (var line in source)
            {
                var wline = line;
                var m = replacement.Match(line);
                if (m.Success)
                {
                    var fixedFile = NormalizeFileForTarget(new FileInfo(m.Groups[1].Value));
                    wline = wline.Replace(m.Value, fixedFile);
                }
                yield return wline;
            }
        }

        /// <summary>
        /// Unable to generate a proxy for this root tuple.
        /// </summary>
        [Serializable]
        public class ProxyGenerationException : Exception
        {
            public ProxyGenerationException() { }
            public ProxyGenerationException(string message) : base(message) { }
            public ProxyGenerationException(string message, Exception inner) : base(message, inner) { }
            protected ProxyGenerationException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Generate a proxy file for a root file.
        /// </summary>
        /// <param name="rootFiles"></param>
        /// <param name="treeName"></param>
        /// <param name="queryDirectory"></param>
        /// <returns></returns>
        public FileInfo GenerateProxyFile(Uri[] rootFiles, string treeName, DirectoryInfo queryDirectory)
        {
            // Check the environment
            MakeSureROOTIsInstalled();

            // Simple argument checks
            if (rootFiles == null || rootFiles.Length == 0)
            {
                throw new ArgumentException("Query must be run on some files - argument to GenerateProxyFile was null or zero length");
            }
            if (queryDirectory == null || !queryDirectory.Exists)
            {
                throw new ArgumentException("The directory were we should create a TTree proxy file should not be null or non-existant!");
            }

            // Commands to generate a proxy
            var cmds = new StringBuilder();
            var rootFilePath = NormalizeFileForTarget(new FileInfo(rootFiles.First().LocalPath));
            cmds.AppendLine($"TFile *f = TFile::Open(\"{rootFilePath}\", \"READ\");");
            cmds.AppendLine($"TTree *t = (TTree*) f->Get(\"{treeName}\");");
            cmds.AppendLine("t->MakeProxy(\"runquery\", \"junk.C\", 0, \"nohist\");");

            // Write the dummy selection file that is required (WHY!!!???).
            var fname = Path.Combine(queryDirectory.FullName, "junk.C");
            using (var w = File.CreateText(fname))
            {
                w.Write("int junk() {return 10.0;}");
                w.Close();
            }

            // Run the commands
            ExecuteRootScript("proxy", cmds.ToString(), queryDirectory);

            // Return the file.
            var header = new FileInfo(Path.Combine(queryDirectory.FullName, "runquery.h"));
            if (!header.Exists)
            {
                throw new ProxyGenerationException($"Failed to generate a proxy from the command line");
            }
            return header;
        }


        [Serializable]
        public class CantFindROOTException : Exception
        {
            public CantFindROOTException() { }
            public CantFindROOTException(string message) : base(message) { }
            public CantFindROOTException(string message, Exception inner) : base(message, inner) { }
            protected CantFindROOTException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Make sure ROOT is installed. If not, attempt to install it.
        /// </summary>
        private void MakeSureROOTIsInstalled()
        {
            if (!CheckForROOTInstall())
            {
                InstallROOT();
                if (!CheckForROOTInstall())
                {
                    throw new CantFindROOTException($"ROOT isn't installed on target machine ({ExecutorName}).");
                }
            }
        }

        /// <summary>
        /// Attempt to install ROOT.
        /// </summary>
        internal abstract void InstallROOT();

        /// <summary>
        /// Return the name of the executor - to be used in error messages and the like.
        /// </summary>
        protected abstract string ExecutorName { get; }

        /// <summary>
        /// Check to see if ROOT has been installed or not. Return TRUE if it has, FALSE otherwise.
        /// </summary>
        /// <returns></returns>
        internal abstract bool CheckForROOTInstall();

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
            cmds.AppendLine($"TSelector *selector = new {selectClass}();");
            WriteInputVariablesForTransfer(cmds, queryResultsFile, varsToTransfer);

            // Get the root files all into a chain
            cmds.AppendLine($"TChain *t = new TChain(\"{treeName}\");");
            foreach (var f in localFiles)
            {
                var fname = NormalizeFileForTarget(f);
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
                //cmds.AppendLine("t->PrintCacheStats();");
            }

            // Get the results and put them into a map for safe keeping!
            // To move them back we need to use a TFile.
            var resultfileFullName = NormalizeFileForTarget(queryResultsFile);
            cmds.AppendLine($"TFile *rf = TFile::Open(\"{resultfileFullName}\", \"RECREATE\");");
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
        private void WriteInputVariablesForTransfer(StringBuilder cmds, FileInfo queryResultsFile, IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            // Objects that are headed over need to be stored in a file and then loaded into the selector.
            if (varsToTransfer != null && varsToTransfer.Count() > 0)
            {
                TraceHelpers.TraceInfo(20, "RunNtupleQuery: Saving the objects we are going to ship over");
                var inputFilesFilename = new FileInfo(Path.Combine(queryResultsFile.DirectoryName, "TSelectorInputFiles.root"));
                var outgoingVariables = ROOTNET.NTFile.Open(inputFilesFilename.FullName, "RECREATE");

                var safeInputFilename = NormalizeFileForTarget(inputFilesFilename);
                cmds.AppendLine($"TFile *varsInFile = TFile::Open(\"{safeInputFilename}\", \"READ\");");
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
                throw new FileNotFoundException($"Unable to find the file {queryResultsFile.FullName} - it should contain the results of the query.");
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
        /// Track the number of results we get back. So we never repeat.
        /// </summary>
        private int _result_index = 0;

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
            var tmpFName = NormalizeFileForTarget(templateRunner);
            cmds.AppendLine($"int r{_result_index} = gSystem->CompileMacro(\"{tmpFName}\", \"{buildFlags}\");");
            cmds.AppendLine($"if (r{_result_index} != 1) {{ exit(1); }}");
            _result_index++;
        }

        /// <summary>
        /// Return the file name converted for the proper target
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected abstract string NormalizeFileForTarget(FileInfo finfo);

        /// <summary>
        /// Return a directory path suitable for including in a string
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected abstract string NormalizeFileForTarget(DirectoryInfo finfo);

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
        internal void ExecuteRootScript(string prefix, string cmds, DirectoryInfo tmpDir)
        {
            // Dump the script
            var cmdFile = Path.Combine(tmpDir.FullName, $"{prefix}.C");
            using (var writer = File.CreateText(cmdFile))
            {
                writer.WriteLine($"void {prefix}() {{");
                writer.Write(cmds);
                writer.WriteLine("}");
            }

            // Create the process info.
            var proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.ErrorDialog = false;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = tmpDir.FullName;
            ConfigureProcessExecution(proc.StartInfo, cmdFile);

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
        /// This must configure the process StartInfo object. It has been pre-configured
        /// before this call for everything but the filename and arguments. Those should be filled in.
        /// </summary>
        /// <param name="p"></param>
        abstract protected void ConfigureProcessExecution(ProcessStartInfo p, string rootMacroFilePath);
    }
}
