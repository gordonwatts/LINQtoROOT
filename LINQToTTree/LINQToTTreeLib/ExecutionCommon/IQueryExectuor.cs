
using System;
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
        /// <param name="queryDirectory"></param>
        /// <param name="templateFile">Path to the main runner file</param>
        /// <param name="varsToTransfer">A list of variables that are used as input to the routine.</param>
        /// <returns>A list of objects and names pulled from the output root file</returns>
        IDictionary<string, ROOTNET.Interface.NTObject> Execute(
            FileInfo templateFile,
            DirectoryInfo queryDirectory,
            IEnumerable<KeyValuePair<string, object>> varsToTransfer);

        /// <summary>
        /// Set the execution envrionment. Must be done before the call, should not
        /// change after the first setting.
        /// </summary>
        ExecutionEnvironment Environment { set; }

        /// <summary>
        /// Generate a set of proxy files for these root files in the given directory, and return the main .h file.
        /// </summary>
        /// <param name="rootFiles"></param>
        /// <param name="queryDirectory"></param>
        /// <returns></returns>
        FileInfo GenerateProxyFile(Uri[] rootFiles, string treeName, DirectoryInfo queryDirectory);
    }
}
