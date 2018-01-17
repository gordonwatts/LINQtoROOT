
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Runs an execution request
    /// </summary>
    public interface IQueryExectuor : IDisposable
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

        /// <summary>
        /// Return the suggested number of ways to split up a job. All things being equal, this might
        /// be the number of processors on a machine, or similar.
        /// </summary>
        /// <param name="rootFiles">Files that we will split, incase there is some heuristic that can be gleaned from the list.</param>
        /// <returns>Number of ways to split</returns>
        /// <remarks>No single file is split up</remarks>
        int SuggestedNumberOfSimultaniousProcesses(Uri[] rootFiles);

        /// <summary>
        /// Split the Uri's into batches that have to be executed in seperate versions of the local executor.
        /// </summary>
        /// <returns>A list of batches. Each one will be executed with a new IQueryExecutor instance. And may be run at the same time.</returns>
        /// <remarks></remarks>
        IEnumerable<Uri[]> BatchInputUris(Uri[] files);
    }
}
