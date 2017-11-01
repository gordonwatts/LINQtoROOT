using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using Polly;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Craete the executor when needed
    /// </summary>
    [Export(typeof(IQueryExecutorFactory))]
    class LocalBashExecutorFactory : IQueryExecutorFactory
    {
        public string Scheme => "localbash";

        public IQueryExectuor Create(IExecutionEnvironment exeReq, string[] referencedLeafNames)
        {
            return new LocalBashExecutor() { Environment = exeReq, LeafNames = referencedLeafNames };
        }
    }

    /// <summary>
    /// Run on a root installed under bash
    /// </summary>
    /// <remarks>
    /// In WSL you need to execute the following commands:
    ///   apt install libxpm-dev
    ///   apt install libatlas-base-dev
    ///   apt install build-essential
    /// </remarks>
    class LocalBashExecutor : CommandLineCommonExecutor, IQueryExectuor
    {

        /// <summary>
        /// Version of ROOT we will be using.
        /// </summary>
        public static string ROOTVersionNumber { get; set; } = "v6.10.02";

        /// <summary>
        /// Install area where ROOT is located. The version number is the next directory.
        /// </summary>
        public static string ROOTInstallArea { get; set; } = "~/root-binaries";

        /// <summary>
        /// Build the root location
        /// </summary>
        /// <returns></returns>
        private string GetROOTExeucatblePath()
        {
            return $"{GetROOTBinaryPath()}/root.exe";
        }

        /// <summary>
        /// Reset to default values. Used mostly for testing.
        /// </summary>
        internal static void ResetLocalBashExecutor()
        {
            ROOTVersionNumber = "v6.10.02";
            ROOTInstallArea = "~/root-binaries";
        }

        /// <summary>
        /// Return the path where the binaries for ROOT are located.
        /// </summary>
        /// <returns></returns>
        private string GetROOTBinaryPath()
        {
            return $"{ROOTInstallArea}/{ROOTVersionNumber}/bin";
        }

        /// <summary>
        /// Return the name of this executor so in an error message the user can tell
        /// where the error occured.
        /// </summary>
        protected override string ExecutorName => "Local bash shell on this machine";

        /// <summary>
        /// Is ROOT installed on this machine?
        /// </summary>
        /// <returns></returns>
        internal override bool CheckForROOTInstall(Action<string> dumpLine = null, bool verbose = false)
        {
            var cmd = new StringBuilder();
            cmd.AppendLine("int i = 10;");

            try
            {
                ExecuteRootScript("testForRoot", cmd.ToString(), new DirectoryInfo(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)), dumpLine, verbose);
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Thrown when we cna't install ROOT for some reason.
        /// </summary>
        [Serializable]
        public class FailedToInstallROOTException : Exception
        {
            public FailedToInstallROOTException() { }
            public FailedToInstallROOTException(string message) : base(message) { }
            public FailedToInstallROOTException(string message, Exception inner) : base(message, inner) { }
            protected FailedToInstallROOTException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Download the approprate version of root and unpack it in the proper area.
        /// </summary>
        /// <remarks>
        /// We are called only if CheckInstall has returned false.
        /// </remarks>
        internal override void InstallROOT(Action<string> dumpLine, bool verbose)
        {
            var cmds = new StringBuilder();
            cmds.Append($"mkdir -p {ROOTInstallArea}\n");
            cmds.Append($"cd {ROOTInstallArea}\n");
            cmds.Append($"mkdir {ROOTVersionNumber}\n");
            cmds.Append($"cd {ROOTVersionNumber}\n");
            var archiveName = $"root_{ROOTVersionNumber}.Linux-ubuntu16-x86_64-gcc5.4.tar.gz";
            cmds.Append($"wget https://root.cern.ch/download/{archiveName}\n");
            cmds.Append($"tar -xf {archiveName} root/ --strip-components=1\n");
            cmds.Append($"rm {archiveName}\n");

            try
            {
                ExecuteBashScript("downlaodroot", cmds.ToString(), dumpLine, verbose);
            } catch (Exception e)
            {
                throw new FailedToInstallROOTException($"Unable to download and install ROOT version {ROOTVersionNumber}.", e);
            }
        }

        /// <summary>
        /// Run a short bash script
        /// </summary>
        /// <param name="cmds"></param>
        internal void ExecuteBashScript(string reason, string cmds, Action<string> dumpLine = null, bool verbose = false)
        {
            // Dump the script
            var tmpDir = new DirectoryInfo(System.IO.Path.GetTempPath());
            var cmdFile = Path.Combine(tmpDir.FullName, $"{reason}.sh");
            using (var writer = File.CreateText(cmdFile))
            {
                writer.Write(cmds);
            }

            // We will write a log file out
            var logFile = new FileInfo($"{tmpDir.FullName}\\{reason}.log");

            // Create the process info.
            var proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.ErrorDialog = false;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.WorkingDirectory = tmpDir.FullName;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            proc.StartInfo.FileName = FindBash();
            proc.StartInfo.Arguments = $"-c {NormalizeFileForTarget(tmpDir)}/{reason}.sh &> {NormalizeFileForTarget(logFile)}";

            // Start it.
            var resultData = new StringBuilder();
            proc.ErrorDataReceived += (sender, e) => RecordLine(resultData, e.Data, dumpLine);
            proc.OutputDataReceived += (sender, e) => RecordLine(resultData, e.Data, dumpLine);

            proc.Start();
            //proc.BeginOutputReadLine();
            //proc.BeginErrorReadLine();

            // Wait for it to end.
            proc.WaitForExit();

            // Now, pick up the file
            foreach (var line in logFile.EnumerateTextFile())
            {
                RecordLine(resultData, line, dumpLine);
            }

            // Make sure the result is "good"
            if (proc.ExitCode != 0)
            {
                throw new CommandLineExecutionException($"Failed to execute step {reason} - process executed with error code {proc.ExitCode}. Text dump from process: {resultData.ToString()}");
            }
        }

        /// <summary>
        /// Locate bash on our system
        /// </summary>
        /// <returns></returns>
        static string FindBash()
        {
            var path = Path.GetFullPath(System.Environment.ExpandEnvironmentVariables(@"%windir%\SysWow64\bash.exe"));
            if (File.Exists(path))
                return path;

            path = Path.GetFullPath(System.Environment.ExpandEnvironmentVariables(@"%windir%\sysnative\bash.exe"));
            if (File.Exists(path))
                return path;

            path = Path.GetFullPath(System.Environment.ExpandEnvironmentVariables(@"%windir%\System32\bash.exe"));
            if (File.Exists(path))
                return path;

            throw new FileNotFoundException("Could not find a path to the bash executable");
        }
        
        /// <summary>
        /// Configure the process that is going to run the actual root thing.
        /// </summary>
        /// <param name="startInfo"></param>
        /// <param name="rootMacroFilePath"></param>
        protected override object ConfigureProcessExecution(ProcessStartInfo startInfo, string rootMacroFilePath)
        {
            // Run bash directly.
            startInfo.FileName = FindBash();

            // We are going to write out a log file
            FileInfo macroFile = new FileInfo(rootMacroFilePath);
            var logFile = new FileInfo(macroFile.FullName + "-log");

            // Run root with the path as an argument.
            startInfo.Arguments = $"-c \". {GetROOTBinaryPath()}/thisroot.sh; root -l -b -q {macroFile.ConvertToBash()} &> {logFile.ConvertToBash()}\"";

            // Get the start info stuff configured properly
            startInfo.LoadUserProfile = true;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;

            return logFile;
        }

        /// <summary>
        /// Called to do clean up.
        /// </summary>
        /// <param name="context"></param>
        protected override void PostProcessExecution(StringBuilder resultData, object context, Action<string> dumpLine = null)
        {
            var logFile = context as FileInfo;

            // Now, just dump it!
            Polly.Policy
                .Handle<IOException>()
                .WaitAndRetry(new[] { TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200) })
                .Execute(() =>
                {
                    foreach (var line in logFile.EnumerateTextFile())
                    {
                        RecordLine(resultData, line, dumpLine);
                    }
                });
        }

        /// <summary>
        /// Convert a filename into a bash path.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected override string NormalizeFileForTarget(Uri finfo)
        {
            return new FileInfo(finfo.LocalPath).ConvertToBash();
        }

        /// <summary>
        /// Convert a filename into a bash path.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected override string NormalizeFileForTarget(DirectoryInfo finfo)
        {
            return finfo.ConvertToBash();
        }
    }
    static class LocalBashExecutorHelpers
    {
        /// <summary>
        /// Convert the file path from Windows to bash
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertToBash(this FileInfo path)
        {
            var splt = path.FullName.Split(':');
            var rootPath = splt[1].Replace('\\', '/');
            return $"/mnt/{splt[0].ToLower()}{rootPath}";
        }

        public static string ConvertToBash(this DirectoryInfo path)
        {
            var splt = path.FullName.Split(':');
            var rootPath = splt[1].Replace('\\', '/');
            return $"/mnt/{splt[0].ToLower()}{rootPath}";
        }
    }
}
