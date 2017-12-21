
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Runs an execution request
    /// </summary>
    public interface IQueryExectuor
    {
        /// <summary>
        /// Run request, and return the results
        /// </summary>
        /// <param name="rootFiles">The files we are to run over</param>
        /// <param name="queryDirectory"></param>
        /// <param name="templateFile">Path to the main runner file</param>
        /// <param name="varsToTransfer">A list of variables that are used as input to the routine.</param>
        /// <returns>A list of objects and names pulled from the output root file</returns>
        Task<IDictionary<string, ROOTNET.Interface.NTObject>> Execute(
            Uri[] rootFiles,
            FileInfo templateFile,
            DirectoryInfo queryDirectory,
            IEnumerable<KeyValuePair<string, object>> varsToTransfer);

        /// <summary>
        /// The leaves that are referenced by this query
        /// </summary>
        string[] LeafNames { get; }

        /// <summary>
        /// Generate a set of proxy files for these root files in the given directory, and return the main .h file.
        /// </summary>
        /// <param name="rootFiles"></param>
        /// <param name="queryDirectory"></param>
        /// <returns></returns>
        Task<FileInfo> GenerateProxyFile(Uri[] rootFiles, string treeName, DirectoryInfo queryDirectory);
    }
}
