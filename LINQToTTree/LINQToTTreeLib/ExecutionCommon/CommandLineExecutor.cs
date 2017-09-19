using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class CommandLineExecutor : CommandLineCommonExecutor, IQueryExectuor
    {
        /// <summary>
        /// Configure the remote execution guy
        /// </summary>
        /// <param name="startInfo"></param>
        /// <param name="rootMacroFilePath"></param>
        protected override void ConfigureProcessExecution(ProcessStartInfo startInfo, string rootMacroFilePath)
        {
            // Figure out where root is that we should be executing against
            var rootPath = System.Environment.ExpandEnvironmentVariables($"%ROOTSYS%\\bin\\root.exe");
            if (!File.Exists(rootPath))
            {
                throw new ROOTExecutableNotFoundException("Unable to find root.exe. This is probably because ROOTSYS is not defined.");
            }

            // Configure it to run directly.
            startInfo.FileName = rootPath;
            startInfo.Arguments = $"-b -q {rootMacroFilePath}";
        }

        /// <summary>
        /// Convert a file path for writing out.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected override string NormalizeFileForTarget(FileInfo finfo)
        {
            return finfo.FullName.Replace("\\", "\\\\");
        }
    }
}
