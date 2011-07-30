
using System.Collections.Generic;
using System.IO;
namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Runs an execution request
    /// </summary>
    interface IQueryExectuor
    {
        /// <summary>
        /// Run request, and return the results
        /// </summary>
        /// <param name="remotePacket"></param>
        /// <returns></returns>
        IDictionary<string, ROOTNET.Interface.NTObject> Execute(
            FileInfo templateFile,
            DirectoryInfo queryDirectory,
            IEnumerable<KeyValuePair<string, object>> varsToTransfer);

        /// <summary>
        /// Set the execution envrionment. Must be done before the call, should not
        /// change after the first setting.
        /// </summary>
        ExecutionEnvironment Environment { set; }
    }
}
