using System;
using System.IO;
using Remotion.Data.Linq;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Caches query results - and allows for later lookup.
    /// </summary>
    public interface IQueryResultCache
    {
        /// <summary>
        /// Have we made this query before? Check the file date, the query string. If there is a match,
        /// then extract the value and return it. Returns false for the first arg of the tuple if nothing found,
        /// and if true the second arg contains the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_rootFile"></param>
        /// <param name="queryModel"></param>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        Tuple<bool, T> Lookup<T>(FileInfo[] _rootFiles, QueryModel queryModel, IVariableSaver varSaver, IVariable theVar);

        /// <summary>
        /// Save an item for later lookup and retreival.
        /// </summary>
        /// <param name="_rootFile"></param>
        /// <param name="qm"></param>
        /// <param name="o"></param>
        void CacheItem(FileInfo[] _rootFiles, QueryModel qm, ROOTNET.Interface.NTObject o);
    }
}
