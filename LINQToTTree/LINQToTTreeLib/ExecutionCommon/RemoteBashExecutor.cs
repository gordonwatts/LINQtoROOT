using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Used to execute remotely (via an ssh connection)
    /// </summary>
    class RemoteBashExecutor : CommandLineCommonExecutor, IQueryExectuor
    {
        protected override string ExecutorName => throw new NotImplementedException();

        /// <summary>
        /// Reset all of our internal variables. Used for
        /// testing.
        /// </summary>
        internal static void ResetRemoteBashExecutor()
        {
        }

        protected override object ConfigureProcessExecution(ProcessStartInfo p, string rootMacroFilePath)
        {
            throw new NotImplementedException();
        }

        protected override string NormalizeFileForTarget(FileInfo finfo)
        {
            throw new NotImplementedException();
        }

        protected override string NormalizeFileForTarget(DirectoryInfo finfo)
        {
            throw new NotImplementedException();
        }

        protected override void PostProcessExecution(StringBuilder resultData, object context)
        {
            throw new NotImplementedException();
        }

        internal override bool CheckForROOTInstall(Action<string> dumpLine, bool verbose)
        {
            throw new NotImplementedException();
        }

        internal override void InstallROOT(Action<string> dumpLine, bool verbose)
        {
            throw new NotImplementedException();
        }
    }
}
