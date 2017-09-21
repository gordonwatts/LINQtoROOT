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
        /// Location of the root executable
        /// </summary>
        const string _rootExeLocation = @"/home/gwatts/ATLAS/root-source/v6-08-06/bin/root.exe";

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
            startInfo.Arguments = $"-c \"{_rootExeLocation} -b -q {new FileInfo(rootMacroFilePath).ConvertToBash()}\"";
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
