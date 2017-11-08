﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public static void RunROOTInBash(string prefix, string commands, DirectoryInfo tempDirectory, Action<string> dumpLine = null, bool verbose = false,
            IEnumerable<FileInfo> filesToSend = null, IEnumerable<FileInfo> filesToReceive = null, TimeSpan? timeout = null)
        {
            // Get ROOT installed if it hasn't been already.
            var le = BuildExecutor(verbose);

            if (!le.CheckForROOTInstall(dumpLine, verbose))
            {
                le.InstallROOT(dumpLine, verbose);
            }

            // Run in ROOT.
            le.ExecuteRootScript(prefix, commands, tempDirectory, dumpLine, verbose,
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
        public static void RunBashCommand(string fnameRoot, string commands, Action<string> dumpLine = null, bool verbose = false)
        {
            var le = BuildExecutor(verbose);
            le.ExecuteBashScript(fnameRoot, commands, dumpLine, verbose);
        }
    }
}