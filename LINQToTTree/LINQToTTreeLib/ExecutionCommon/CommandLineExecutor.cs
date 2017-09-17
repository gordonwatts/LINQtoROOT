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

            // Compile the macro
            CompileAndLoad(queryFile, cmds);

            // Run the root script
            cmds.AppendLine("exit(0);");
            ExecuteRootScript("RunTSelector", cmds, queryDirectory);

            // Get back results
            return null;
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
            cmds.AppendLine("if (r != 1) { exit(r); }");
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
            var cmdFile = $"{System.IO.Path.GetTempPath()}{prefix}.C";
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
            lock(resultData)
            {
                resultData.AppendLine(line);
            }
        }
    }
}
