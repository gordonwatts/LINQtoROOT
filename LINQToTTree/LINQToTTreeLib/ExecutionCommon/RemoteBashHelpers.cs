﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LINQToTTreeLib.ExecutionCommon
{
    public static class RemoteBashHelpers
    {
        /// <summary>
        /// Execute a command in ROOT that is running on bash. If ROOT isn't installed, it will be.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="commands"></param>
        /// <param name="tempDirectory"></param>
        /// <param name="dumpLine">Called with each output line</param>
        public static async Task RunROOTInBashAsync(string prefix, string commands, DirectoryInfo tempDirectory, Action<string> dumpLine = null, bool verbose = false,
            IEnumerable<FileInfo> filesToSend = null, IEnumerable<FileInfo> filesToReceive = null, TimeSpan? timeout = null)
        {
            // Get ROOT installed if it hasn't been already.
            var le = BuildExecutor(verbose);

            if (!(await le.CheckForROOTInstall(dumpLine, verbose)))
            {
                await le.InstallROOT(dumpLine, verbose);
            }

            // Run in ROOT.
            await le.ExecuteRootScript(prefix, commands, tempDirectory, dumpLine, verbose,
                extraFiles: filesToSend?.Select(f => new Uri(f.FullName)), receiveFiles: filesToReceive?.Select(f => new Uri(f.FullName)),
                timeout: timeout);
        }

        /// <summary>
        /// Build a local executor
        /// </summary>
        /// <returns></returns>
        private static RemoteBashExecutor BuildExecutor(bool verbose)
        {
            return new RemoteBashExecutor
            {
                Environment = new ExecutionEnvironment() { CompileDebug = false, Verbose = verbose }
            };
        }

        /// <summary>
        /// Run a command. Throw if the command doens't succeed.
        /// </summary>
        /// <param name="fnameRoot">Root of the script filename we should use (prebuild, or install, etc.)</param>
        /// <param name="commands">Bash script, using \n as the seperator</param>
        public static async Task RunBashCommandAsync(string fnameRoot, string commands, Action<string> dumpLine = null, bool verbose = false)
        {
            var le = BuildExecutor(verbose);
            await le.ExecuteBashScriptAsync(fnameRoot, commands, dumpLine, verbose);
        }
    }
}
