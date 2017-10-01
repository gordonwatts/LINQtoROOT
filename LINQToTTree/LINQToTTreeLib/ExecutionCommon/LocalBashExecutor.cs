using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROOTNET.Interface;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Run on a root installed under bash
    /// </summary>
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
        protected override bool CheckForROOTInstall()
        {
            var cmd = new StringBuilder();
            cmd.AppendLine("int i = 10;");

            try
            {
                ExecuteRootScript("testForRoot", cmd, new DirectoryInfo(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)));
                return true;
            }
            catch { }
            return false;
        }


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
        protected override void InstallROOT()
        {
            var cmds = new StringBuilder();
            cmds.Append($"mkdir {ROOTInstallArea}\n");
            cmds.Append($"cd {ROOTInstallArea}\n");
            cmds.Append($"mkdir {ROOTVersionNumber}\n");
            cmds.Append($"cd {ROOTVersionNumber}\n");
            var archiveName = $"root_{ROOTVersionNumber}.Linux-ubuntu16-x86_64-gcc5.4.tar.gz";
            cmds.Append($"wget https://root.cern.ch/download/{archiveName}\n");
            cmds.Append($"tar -xf {archiveName} root/ --strip-components=1\n");
            cmds.Append($"rm {archiveName}\n");

            try
            {
                ExecuteBashScript("downlaodroot", cmds);
            } catch (Exception e)
            {
                throw new FailedToInstallROOTException($"Unable to download and install ROOT version {ROOTVersionNumber}.", e);
            }
        }

        /// <summary>
        /// Run a short bash script
        /// </summary>
        /// <param name="cmds"></param>
        private void ExecuteBashScript(string reason, StringBuilder cmds)
        {
            // Dump the script
            var tmpDir = new DirectoryInfo(System.IO.Path.GetTempPath());
            var cmdFile = Path.Combine(tmpDir.FullName, $"{reason}.sh");
            using (var writer = File.CreateText(cmdFile))
            {
                writer.Write(cmds.ToString());
            }

            // Create the process info.
            var proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.ErrorDialog = false;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.WorkingDirectory = tmpDir.FullName;

            proc.StartInfo.FileName = System.Environment.ExpandEnvironmentVariables(@"%windir%\sysnative\bash.exe");
            proc.StartInfo.Arguments = $"-c {NormalizeFileForTarget(tmpDir)}/{reason}.sh";

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
                throw new CommandLineExecutionException($"Failed to execute step {reason} - process executed with error code {proc.ExitCode}. Text dump from process: {resultData.ToString()}");
            }
        }

        /// <summary>
        /// Configure the process that is going to run the actual root thing.
        /// </summary>
        /// <param name="startInfo"></param>
        /// <param name="rootMacroFilePath"></param>
        protected override void ConfigureProcessExecution(ProcessStartInfo startInfo, string rootMacroFilePath)
        {
            // Run bash directly.
            startInfo.FileName = System.Environment.ExpandEnvironmentVariables(@"%windir%\sysnative\bash.exe");

            // Run root with the path as an argument.
            startInfo.Arguments = $"-c \". {GetROOTBinaryPath()}/thisroot.sh; root -b -q {new FileInfo(rootMacroFilePath).ConvertToBash()}\"";
        }

        /// <summary>
        /// Convert a filename into a bash path.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected override string NormalizeFileForTarget(FileInfo finfo)
        {
            return finfo.ConvertToBash();
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
