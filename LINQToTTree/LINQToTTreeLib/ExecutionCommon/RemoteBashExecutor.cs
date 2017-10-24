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

        /// <summary>
        /// Need to see if the remote machine has ROOT installed.
        /// </summary>
        /// <param name="dumpLine"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        internal override bool CheckForROOTInstall(Action<string> dumpLine, bool verbose)
        {
            // Simple script to execute
            var cmd = new StringBuilder();
            cmd.AppendLine("int i = 10;");

            try
            {
                ExecuteRootScript("testForRoot", cmd.ToString(), new DirectoryInfo(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)), dumpLine, verbose);
            } catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// The config for a machine. We load it and hold it in memory.
        /// </summary>
        private class MachineConfig
        {
            /// <summary>
            /// Contains the connection string that we will use to "dial" up the other location
            /// (that we pass to AtlasSSH).
            /// </summary>
            string RemoteSSHConnectionString;

            /// <summary>
            /// Lines we need to execute in order to configure the
            /// the remote machine to be ready to run ROOT.
            /// </summary>
            string[] ConfigureLines;
        }

        /// <summary>
        /// Load in the config info for the machines
        /// </summary>
        /// <returns></returns>
        private object LoadMachineConfig()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Thrown when we try to install root remotely.
        /// </summary>
        [Serializable]
        public class ROOTCantBeInstalledRemotelyException : Exception
        {
            public ROOTCantBeInstalledRemotelyException() { }
            public ROOTCantBeInstalledRemotelyException(string message) : base(message) { }
            public ROOTCantBeInstalledRemotelyException(string message, Exception inner) : base(message, inner) { }
            protected ROOTCantBeInstalledRemotelyException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// We have been requested to install ROOT on the remote system.
        /// Currently we fail at this - we expect it to be properly installed.
        /// </summary>
        /// <param name="dumpLine"></param>
        /// <param name="verbose"></param>
        /// <remarks>
        /// We aren't installing root because these are often big machines where ROOT is already present.
        /// If we need this, this functionality can be added.
        /// </remarks>
        internal override void InstallROOT(Action<string> dumpLine, bool verbose)
        {
            throw new ROOTCantBeInstalledRemotelyException("Unable to install root on a remote system over ssh");
        }
    }
}
