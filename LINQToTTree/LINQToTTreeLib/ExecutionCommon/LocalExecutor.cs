
using System;
using System.Collections.Generic;
using System.IO;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Runs single threaded, in the local process, and does all the ntuples we need.
    /// </summary>
    class LocalExecutor : LocalBuildBase, IQueryExectuor
    {
        /// <summary>
        /// The execution environment
        /// </summary>
        public ExecutionEnvironment Environment { set; get; }

        /// <summary>
        /// Given a request, run it. No need to clean up afterwards as we are already there.
        /// </summary>
        /// <param name="remotePacket">The basic info about this run</param>
        /// <returns></returns>
        public System.Collections.Generic.IDictionary<string, ROOTNET.Interface.NTObject> Execute(
            FileInfo templateFile,
            DirectoryInfo queryDirectory,
            IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            //
            // Get the environment setup for this call
            //

            Init();
            PreExecutionInit(Environment.ClassesToDictify);

            TraceHelpers.TraceInfo(12, "ExecuteQueuedQueries: Loading all extra objects");
            AssembleAndLoadExtraObjects(Environment.ExtraComponentFiles);

            //
            // Load the query up
            //

            CompileAndLoad(templateFile);

            //
            // Get the file name of the selector.
            //

            TraceHelpers.TraceInfo(14, "ExecuteQueuedQueries: Startup - Running the code");
            var results = RunNtupleQuery(Path.GetFileNameWithoutExtension(templateFile.Name), varsToTransfer, Environment.TreeName, Environment.RootFiles);

            //
            // And cleanup!
            //

            TraceHelpers.TraceInfo(16, "ExecuteQueuedQueries: unloading all results");
            UnloadAllModules();
            if (Environment.CleanupQuery)
            {
                queryDirectory.Delete(true);
            }

            return results;
        }

    }
}
