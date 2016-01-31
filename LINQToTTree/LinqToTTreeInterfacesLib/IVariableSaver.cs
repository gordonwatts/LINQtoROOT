
using System.Collections.Generic;
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Deals with saving something to a file and reading it back
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
        /// <typeparam name="T1"></typeparam>
        /// <param name="iVariable"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        T LoadResult<T>(IDeclaredParameter iVariable, ROOTNET.Interface.NTObject[] obj);

        /// <summary>
        /// Returns a list of names that should be stored together in a cache
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        string[] GetCachedNames(IDeclaredParameter iVariable);
    }
}
