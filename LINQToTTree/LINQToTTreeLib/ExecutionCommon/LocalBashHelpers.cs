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
        public static void RunROOTInBash (string prefix, string commands, DirectoryInfo tempDirectory, Action<string> dumpLine = null, bool verbose = false)
        {
            // Get ROOT installed if it hasn't been already.
            var le = BuildExecutor();

#warning Fix this async result.
            if (!le.CheckForROOTInstall(dumpLine, verbose).Result)
            {
                le.InstallROOT(dumpLine, verbose).Wait();
            }

            // Run in ROOT.
#warning fix this async result
            le.ExecuteRootScript(prefix, commands, tempDirectory, dumpLine, verbose).Wait();
        }

        /// <summary>
        /// Build a local executor
        /// </summary>
        /// <returns></returns>
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
        public static void RunBashCommand(string fnameRoot, string commands, Action<string> dumpLine = null, bool verbose = false)
        {
            var le = BuildExecutor();
#warning fix this async result
            le.ExecuteBashScript(fnameRoot, commands, dumpLine, verbose).Wait();
        }
    }
}
