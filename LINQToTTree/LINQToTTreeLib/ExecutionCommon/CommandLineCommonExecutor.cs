using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using Nito.AsyncEx;
using Polly;
using ROOTNET.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        public IExecutionEnvironment Environment { set; get; }

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
        protected void RecordLine(StringBuilder resultData, string line, Action<string> dumpLine = null)
        {
            if (line == null)
                return;

            dumpLine?.Invoke(line);

            if (resultData != null)
            {
                lock (resultData)
                {
                    RecordLineUnprotected(resultData, line);
                }
            } else
            {
                RecordLineUnprotected(null, line);
            }
        }

        /// <summary>
        /// In case we aren't worried about locking.. NO ONE ELSE SHOULD CALL THIS.
        /// </summary>
        /// <param name="resultData"></param>
        /// <param name="line"></param>
        private void RecordLineUnprotected(StringBuilder resultData, string line)
        {
            resultData?.AppendLine(line);
            if (Environment.CompileDebug | Environment.Verbose)
            {
                Console.WriteLine(line);
            }
            foreach (var logger in _logDumpers)
            {
                logger(line);
            }
        }

        /// <summary>
        /// Execute a TSelector script on a group of files.
        /// </summary>
        /// <param name="queryFile">The C++ file we are going to run the query against</param>
        /// <param name="queryDirectory">Directory where we run the query</param>
        /// <param name="varsToTransfer">Variables we need to move over to the query</param>
        /// <returns>The results from running the TSelector</returns>
        /// <remarks>
        /// Some exception conditions are handled:
        ///   - Once in a while while executing we have a corrupt datafile that is corrupt due to networking, not because it is a bad
        ///     file. This happens often when things are over shares. We will retry three time if we detect root is having trouble with
        ///     bad data.
        /// </remarks>
        public virtual async Task<IDictionary<string, NTObject>> Execute(Uri[] files, FileInfo queryFile, DirectoryInfo queryDirectory, IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            // Setup for building a command
            await ExecutionUtilities.Init();
            var cmds = new StringBuilder();
            cmds.AppendLine("{");

            // Rewrite the query if it contains special file paths
            ReWritePathsInQuery(queryFile);

            // Put our run-directory in the list of includes.
            var includePath = await NormalizeFileForTarget(new DirectoryInfo(System.Environment.CurrentDirectory));
            cmds.AppendLine($"gSystem->AddIncludePath(\"-I\\\"{includePath}\\\"\");");

            // Load up extra objects & dictionaries
            await LoadExtraCPPFiles(queryDirectory, cmds);
            LoadExtraDictionaries(Environment.ClassesToDictify, cmds);

            // Compile the macro
            await CompileAndLoad(queryFile, cmds);

            // Run the query in a second file.
            var subfileCommands = new StringBuilder();
            var resultsFile = new FileInfo(Path.Combine(queryDirectory.FullName, "selector_results.root"));
            await RunNtupleQuery(subfileCommands, resultsFile, Path.GetFileNameWithoutExtension(queryFile.Name), varsToTransfer,
                Environment.TreeName, files);

            // Write out the temp file.
            using (var secondFile = File.CreateText(Path.Combine(queryDirectory.FullName, "RunTSelector1.C")))
            {
                secondFile.WriteLine("void RunTSelector1() {");
                secondFile.Write("  " + subfileCommands.ToString());
                secondFile.WriteLine("}");
                secondFile.Close();
            }
            cmds.AppendLine("gROOT->ProcessLine(\".X RunTSelector1.C\");");

            // Run the root script. Retry if we detect an understood error condition.
            cmds.AppendLine("exit(0);");
            cmds.AppendLine("}");
            await NormalizeFileForTarget(queryDirectory);
            await Policy
                .Handle<CommandLineExecutionException>(ex => ex.Message.Contains("error reading from file"))
                .RetryAsync(3)
                .ExecuteAsync(async () =>
                {
                    await ExecuteRootScript("Query", cmds.ToString(), queryDirectory,
                        fetchFiles: new[] { new Uri(resultsFile.FullName) },
                        timeout: TimeSpan.FromHours(4));
                });

            // Get back results
            var results = await LoadSelectorResults(resultsFile);

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
                    var fixedFile = NormalizeFileForTarget(new Uri(m.Groups[1].Value)).Result;
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
        public virtual async Task<FileInfo> GenerateProxyFile(Uri[] rootFiles, string treeName, DirectoryInfo queryDirectory)
        {
            Action<string> dumpLine = Environment.Verbose
                ? l => Console.WriteLine(l)
                : (Action<string>)null;

            // Check the environment
            await MakeSureROOTIsInstalled(dumpLine, Environment.Verbose);

            // Generate the proxy.
            return await GenerateProxyFileInternal(rootFiles, treeName, queryDirectory, dumpLine);
        }

        /// <summary>
        /// Generate the proxy. We are assured that ROOT exists on the remote system at this point.
        /// </summary>
        /// <param name="rootFiles"></param>
        /// <param name="treeName"></param>
        /// <param name="queryDirectory"></param>
        /// <param name="dumpLine"></param>
        /// <returns></returns>
        protected virtual async Task<FileInfo> GenerateProxyFileInternal(Uri[] rootFiles, string treeName, DirectoryInfo queryDirectory, Action<string> dumpLine)
        {
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
            var rootFilePath = rootFiles.First();
            cmds.AppendLine("{");
            cmds.AppendLine($"TFile *f = TFile::Open(\"<><>{rootFilePath.OriginalString}<><>\", \"READ\");");
            cmds.AppendLine($"TTree *t = (TTree*) f->Get(\"{treeName}\");");
            cmds.AppendLine("t->MakeProxy(\"runquery\", \"junk.C\", 0, \"nohist\");");
            cmds.AppendLine("}");

            // Write the dummy selection file that is required (WHY!!!???).
            var fname = Path.Combine(queryDirectory.FullName, "junk.C");
            using (var w = File.CreateText(fname))
            {
                w.Write("int junk() {return 10.0;}");
                w.Close();
            }

            // Run the commands
            var header = new FileInfo(Path.Combine(queryDirectory.FullName, "runquery.h"));
            await ExecuteRootScript("Proxy", cmds.ToString(), queryDirectory, dumpLine,
                extraFiles: new[] { new Uri(fname) },
                fetchFiles: new[] { new Uri(header.FullName) });

            // Return the file.
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
        private async Task MakeSureROOTIsInstalled(Action<string> dumpLine = null, bool verbose = false)
        {
            if (!(await CheckForROOTInstall(dumpLine, verbose)))
            {
                await InstallROOT(dumpLine, verbose);
                if (!(await CheckForROOTInstall(dumpLine, verbose)))
                {
                    throw new CantFindROOTException($"ROOT isn't installed on target machine ({ExecutorName}).");
                }
            }
        }

        /// <summary>
        /// Attempt to install ROOT.
        /// </summary>
        internal abstract Task InstallROOT(Action<string> dumpLine, bool verbose);

        /// <summary>
        /// Return the name of the executor - to be used in error messages and the like.
        /// </summary>
        protected abstract string ExecutorName { get; }

        /// <summary>
        /// Check to see if ROOT has been installed or not. Return TRUE if it has, FALSE otherwise.
        /// </summary>
        /// <returns></returns>
        internal abstract Task<bool> CheckForROOTInstall(Action<string> dumpLine, bool verbose);

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
        private async Task LoadExtraCPPFiles(DirectoryInfo queryDirectory, StringBuilder cmds)
        {
            // Move everything over, and then compile!
            if (Environment.ExtraComponentFiles != null)
            {
                foreach (var fd in Environment.ExtraComponentFiles)
                {
                    var output = ExecutionUtilities.CopyToDirectory(fd, queryDirectory);
                    try
                    {
                        await CompileAndLoad(output, cmds);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to build {0}. Ignoring and crossing fingers.", output.Name);
                    }
                }
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
        private async Task RunNtupleQuery(StringBuilder cmds, FileInfo queryResultsFile, string selectClass, IEnumerable<KeyValuePair<string, object>> varsToTransfer, string treeName, Uri[] localFiles)
        {
            // Init the selector
            cmds.AppendLine($"TSelector *selector = new {selectClass}();");
            await WriteInputVariablesForTransfer(cmds, queryResultsFile, varsToTransfer);

            // Get the root files all into a chain
            cmds.AppendLine($"TChain *t = new TChain(\"{treeName}\");");
            foreach (var f in localFiles)
            {
                var fname = await NormalizeFileForTarget(f);
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
            var resultfileFullName = await NormalizeFileForTarget(queryResultsFile);
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
        private async Task WriteInputVariablesForTransfer(StringBuilder cmds, FileInfo queryResultsFile, IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            // Objects that are headed over need to be stored in a file and then loaded into the selector.
            if (varsToTransfer != null && varsToTransfer.Count() > 0)
            {
                TraceHelpers.TraceInfo(20, "RunNtupleQuery: Saving the objects we are going to ship over");
                var inputFilesFilename = new FileInfo(Path.Combine(queryResultsFile.DirectoryName, "TSelectorInputFiles.root"));
                var outgoingVariables = ROOTNET.NTFile.Open(inputFilesFilename.FullName, "RECREATE");

                // Write out the code to load them and stash them remotely if need be.
                var safeInputFilename = await NormalizeFileForTarget(inputFilesFilename);
                cmds.AppendLine($"TFile *varsInFile = TFile::Open(\"{safeInputFilename}\", \"READ\");");
                cmds.AppendLine("selector->SetInputList(new TList());");

                // Next, move through and actually write everything out.
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
        private async Task<IDictionary<string, NTObject>> LoadSelectorResults(FileInfo queryResultsFile)
        {
            using (var holder = await ROOTLock.Lock.LockAsync())
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
        private async Task CompileAndLoad(FileInfo templateRunner, StringBuilder cmds)
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
            var tmpFName = await NormalizeFileForTarget(templateRunner);
            cmds.AppendLine($"int r{_result_index} = gSystem->CompileMacro(\"{tmpFName}\", \"{buildFlags}\");");
            cmds.AppendLine($"if (r{_result_index} != 1) {{ exit(1); }}");
            _result_index++;
        }

        /// <summary>
        /// Return the file name converted for the proper target
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected abstract Task<string> NormalizeFileForTarget(Uri finfo);

        /// <summary>
        /// Helper function to speed the conversion.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected Task<string> NormalizeFileForTarget(FileInfo finfo)
        {
            return NormalizeFileForTarget(new Uri(finfo.FullName));
        }

        /// <summary>
        /// Return a directory path suitable for including in a string
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected abstract Task<string> NormalizeFileForTarget(DirectoryInfo finfo);

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
        /// <param name="extraFiles">List of extra files that might be needed.</param>
        /// <param name="fetchFiles">List of files that should be fetched. Assuemd to be written in the local area</param>
        internal virtual async Task ExecuteRootScript(string prefix, string cmds, DirectoryInfo tmpDir, Action<string> dumpLine = null, bool verbose = false, IEnumerable<Uri> extraFiles = null, IEnumerable<Uri> fetchFiles = null,
            TimeSpan? timeout = null)
        {
            // Parse the commands for replacements
            var tcommands = ReWritePathsInQuery(cmds);

            // Dump the script
            var cmdFile = Path.Combine(tmpDir.FullName, $"{prefix}.C");
            using (var writer = File.CreateText(cmdFile))
            {
                writer.WriteLine($"void {prefix}() {{");
                writer.Write(tcommands);
                writer.WriteLine("}");
            }

            // Create the process info.
            var proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.ErrorDialog = false;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.WorkingDirectory = tmpDir.FullName;
            var context = ConfigureProcessExecution(proc.StartInfo, cmdFile);

            if (verbose)
            {
                dumpLine?.Invoke($"About to run program {proc.StartInfo.FileName} with arguments {proc.StartInfo.Arguments}.");
            }

            // Start it.
            var resultData = new StringBuilder();
            proc.ErrorDataReceived += (sender, e) => RecordLine(resultData, e.Data, dumpLine);
            proc.OutputDataReceived += (sender, e) => RecordLine(resultData, e.Data, dumpLine);

            var completionTask = proc.StartAsync();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            // Wait for it to end.
            if (verbose) dumpLine?.Invoke("Waiting for process to exit");
            var r = await Task.WhenAny(completionTask, Task.Delay(timeout.HasValue ? (int)timeout.Value.TotalMilliseconds : int.MaxValue));
            if (r != completionTask)
            {
                // Timeout occured. Kill it!
                proc.Kill();
            }
            if (verbose) dumpLine?.Invoke($"Process result is {proc.ExitCode}.");
            PostProcessExecution(resultData, context, dumpLine);

            // Make sure the result is "good"; throw a reasonable error message if not.
            if (proc.ExitCode != 0)
            {
                throw new CommandLineExecutionException($"Failed to execute step {prefix} - process executed with error code {proc.ExitCode}. Text dump from process: {ReformatLog(resultData)}");
            } else if (r != completionTask)
            {
                throw new TimeoutException($"Failed to execute step { prefix } - process was killed due to timeout ({timeout.Value.TotalSeconds} seconds). Text dump from process: { ReformatLog(resultData)}");
            }
        }

        /// <summary>
        /// Reformat a log dump that has come back from some remote shell operation so that it can be cleanly put in
        /// an error message.
        /// </summary>
        /// <param name="resultData">String containing the log that came back from the calling process</param>
        /// <returns>Indented and formatted block of text</returns>
        protected static string ReformatLog(StringBuilder resultData)
        {
            var result = new StringBuilder();
            result.AppendLine();
            using (var rdr = new StringReader(resultData.ToString()))
            {
                foreach (var line in rdr.EnumerateLines())
                {
                    result.AppendLine($"  -> {line}");
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Called after the process has finished, in case there are other things that need to be done.
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="cmdFile"></param>
        virtual protected void PostProcessExecution(StringBuilder resultData, object context, Action<string> dumpLine = null)
        {
        }

        /// <summary>
        /// This must configure the process StartInfo object. It has been pre-configured
        /// before this call for everything but the filename and arguments. Those should be filled in.
        /// </summary>
        /// <param name="p"></param>
        virtual protected object ConfigureProcessExecution(ProcessStartInfo p, string rootMacroFilePath)
        {
            return null;
        }
    }
}
