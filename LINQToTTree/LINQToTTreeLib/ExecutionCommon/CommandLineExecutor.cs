using LinqToTTreeInterfacesLib;
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
    public class CommandLineExecutorFactory : IQueryExecutorFactory
    {
        public string Scheme => "localwin";

        public IQueryExectuor Create(IExecutionEnvironment exeReq, string[] referencedLeafNames)
        {
            return new CommandLineExecutor() { Environment = exeReq, LeafNames = referencedLeafNames };
        }
    }

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
        /// Return a string so that error messages know what kind of executor this is.
        /// </summary>
        protected override string ExecutorName => "Windows Command Line Executor";

        /// <summary>
        /// Can we locate ROOT?
        /// </summary>
        /// <returns></returns>
        internal override bool CheckForROOTInstall(Action<string> dumpLine, bool verbose)
        {
            if (verbose) { dumpLine?.Invoke($"Looking for root to exist at {GetROOTPath()}."); }
            return File.Exists(GetROOTPath());
        }

        /// <summary>
        /// Configure the remote execution guy
        /// </summary>
        /// <param name="startInfo"></param>
        /// <param name="rootMacroFilePath"></param>
        protected override object ConfigureProcessExecution(ProcessStartInfo startInfo, string rootMacroFilePath)
        {
            // Figure out where root is that we should be executing against
            var rootPath = GetROOTPath();
            if (!File.Exists(rootPath))
            {
                throw new ROOTExecutableNotFoundException("Unable to find root.exe. This is probably because ROOTSYS is not defined.");
            }

            // Configure it to run directly.
            startInfo.FileName = rootPath;
            startInfo.Arguments = $"-b -q {rootMacroFilePath}";

            return null;
        }

        /// <summary>
        /// Return the path where we expect to find the ROOT executable
        /// </summary>
        /// <returns></returns>
        private static string GetROOTPath()
        {
            return System.Environment.ExpandEnvironmentVariables($"%ROOTSYS%\\bin\\root.exe");
        }

        /// <summary>
        /// Convert a file path for writing out.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected override string NormalizeFileForTarget(Uri finfo)
        {
            // Make sure the Uri is in the file scheme. This is b.c. otherwise
            // a UNC path (in particlar) isn't rendered the same way.
            var u = finfo.Scheme == "file"
                ? finfo
                : new UriBuilder(finfo) { Scheme = "file" }.Uri;
            return u.LocalPath.Replace("\\", "\\\\");
        }

        /// <summary>
        /// Convert a file path for writing out.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected override string NormalizeFileForTarget(DirectoryInfo finfo)
        {
            return finfo.FullName.Replace("\\", "\\\\");
        }

        /// <summary>
        /// We are being asked to install ROOT. This is impossible.
        /// </summary>
        internal override void InstallROOT(Action<string> dumpLine, bool verbose = false)
        {
            throw new InvalidOperationException("ROOT not found in the PATH - but in order to run this code it must have loaded root libraries from the PATH - so this is impossible!");
        }

        /// <summary>
        /// Called after the command has run so we can do any required clean up.
        /// </summary>
        /// <param name="context"></param>
        protected override void PostProcessExecution(StringBuilder resultData, object context)
        {
        }
    }
}
