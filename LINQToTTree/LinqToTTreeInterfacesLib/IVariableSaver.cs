
using ROOTNET.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LinqToTTreeInterfacesLib
{

    /// <summary>
    /// Store the result for a particular query name.
    /// This gives unique access to a sub-query's results in the cache.
    /// </summary>
    public struct RunInfo
    {
        /// <summary>
        /// The cycle number that was run here
        /// </summary>
        public int _cycle { get; set; }

        /// <summary>
        /// The result for that cycle number.
        /// </summary>
        public NTObject _result { get; set; }
    }
    
    /// <summary>
    /// Manages sending data back and forth over (the wire, the file system, memory) for data from the query to
    /// what is used by the TSelector.
    /// </summary>
    public interface IVariableSaver
    {
        /// <summary>
        /// Return true if you know how to generate stuff for the variable. This decision can
        /// only be based on type!
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        bool CanHandle(IDeclaredParameter iVariable);

        /// <summary>
        /// Return a sequence of C++ code that will end with the var being cached. This should
        /// occur in SlaveTerminate in TSelector.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        IEnumerable<string> SaveToFile(IDeclaredParameter iVariable);

        /// <summary>
        /// Return the include files that this saver needs loaded into the query script
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        IEnumerable<string> IncludeFiles(IDeclaredParameter iVariable);

        /// <summary>
        /// Given an object, translate the result to the item we are going to be saving.
        /// </summary>
        /// <typeparam name="T1">The type that this should return - which will be matched to this type.</typeparam>
        /// <param name="iVariable">the parameter that this result is stored in, in the code.</param>
        /// <param name="cycle">Which cycle number of query is this?</param>
        /// <param name="obj">The returned object from the code</param>
        /// <returns>The object itself - the integer, the double, the histogram, the FileInfo, etc.</returns>
        Task<T> LoadResult<T>(IDeclaredParameter iVariable, RunInfo[] obj);

        /// <summary>
        /// Returns a list of names that should be stored together in a cache
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        string[] GetCachedNames(IDeclaredParameter iVariable);

        /// <summary>
        /// Called after query is done - make a global resource unique in name. No need to do this
        /// if resource is in memory.
        /// </summary>
        /// <param name="cycle">The cycle of this item - so it can be appended to a filename</param>
        /// <param name="alternateDirectory">An alternate directory where this file might exist</param>
        /// <param name="iVariable">The declared variable we are using against this storage</param>
        /// <param name="obj">The result objects that contains info about the variable</param>
        /// <remarks>
        /// There are cases where the identical code is to be produced for multiple runs. In most cases,
        /// nothing extra needs to be done. But if something global is modified - say a file is written out rather than
        /// an object returned in memory. In those cases, when multiple queires are run, the files will be identical and will
        /// step on each other. In short - bad. So once the query is done, this method provides an oportunity to rename the
        /// file (or object, or whatever). For in memory items that are sent via the TSelect input list, there is no need.
        /// </remarks>
        void RenameForQueryCycle(IDeclaredParameter iVariable, ROOTNET.Interface.NTObject[] obj, int cycle, DirectoryInfo alternateDirectory);
    }
}
