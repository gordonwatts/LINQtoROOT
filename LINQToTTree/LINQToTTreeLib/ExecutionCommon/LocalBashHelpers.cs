using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Some utilities for running root bash stuff. Makes it easy to use this code elsewhere
    /// along with inside this code here.
    /// </summary>
    public class LocalBashHelpers
    {
        /// <summary>
        /// Execute a command in ROOT that is running on bash. If ROOT isn't installed, it will be.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="commands"></param>
        /// <param name="tempDirectory"></param>
        /// <param name="dumpLine">Called with each output line</param>
        public static async Task RunROOTInBashAsync (string prefix, string commands, DirectoryInfo tempDirectory, Action<string> dumpLine = null, bool verbose = false)
        {
            // Get ROOT installed if it hasn't been already.
            using (var le = BuildExecutor())
            {

                if (!(await le.CheckForROOTInstall(dumpLine, verbose)))
                {
                    await le.InstallROOT(dumpLine, verbose);
                }

                // Run in ROOT.
                await le.ExecuteRootScript(prefix, commands, tempDirectory, dumpLine, verbose);
            }
        }

        /// <summary>
        /// Build a local executor
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static LocalBashExecutor BuildExecutor()
        {
            return new LocalBashExecutor
            {
                Environment = new ExecutionEnvironment() { CompileDebug = false }
            };
        }

        /// <summary>
        /// Run a command. Throw if the command doens't succeed.
        /// </summary>
        /// <param name="fnameRoot">Root of the script filename we should use (prebuild, or install, etc.)</param>
        /// <param name="commands">Bash script, using \n as the seperator</param>
        public static async Task RunBashCommandAsync(string fnameRoot, string commands, Action<string> dumpLine = null, bool verbose = false)
        {
            using (var le = BuildExecutor())
            {
                await le.ExecuteBashScript(fnameRoot, commands, dumpLine, verbose);
            }
        }
    }
}
