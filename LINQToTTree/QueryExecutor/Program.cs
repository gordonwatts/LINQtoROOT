using System;
using System.IO;
using LINQToTTreeLib.ExecutionCommon.ParallelExes;

namespace QueryExecutor
{
    class Program
    {
        /// <summary>
        /// Simple program to execute a query
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ROOTNET.NTEnv.gEnv.SetValue("TFile.Recover", "0");
            if (args.Length != 1)
                throw new ArgumentException("Incorrect number of args");

            var input = new FileInfo(args[0]);
            if (!input.Exists)
                throw new FileNotFoundException("Unable to find file", input.FullName);

            var executor = new SubProcessRunner();
            executor.Run(input);
        }
    }
}
