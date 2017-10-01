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
        public static string ROOTVersionNumber { get; set; } = "v6-08-06";

        /// <summary>
        /// Install area where ROOT is located. The version number is the next directory.
        /// </summary>
        public static string ROOTInstallArea { get; set; } = "/home/gwatts/ATLAS/root-source";

        /// <summary>
        /// Build the root location
        /// </summary>
        /// <returns></returns>
        private string GetROOTExeucatblePath()
        {
            return $"{ROOTInstallArea}/{ROOTVersionNumber}/bin/root.exe";
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
            startInfo.Arguments = $"-c \"{GetROOTExeucatblePath()} -b -q {new FileInfo(rootMacroFilePath).ConvertToBash()}\"";
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
