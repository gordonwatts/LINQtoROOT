using System;
using Remotion.Linq;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// The key object returned from the caching interface.
    /// </summary>
    public interface IQueryResultCacheKey
    {
        /// <summary>
        /// Return a hash string for this query. It should be unique.
        /// </summary>
        /// <returns></returns>
        string GetUniqueHashString();
    }

    /// <summary>
    /// Caches query results - and allows for later lookup.
    /// </summary>
    public interface IQueryResultCache
    {
        /// <summary>
        /// Get the key for the cache entry for the object we are interested in.
        /// </summary>
        /// <param name="rootfiles"></param>
        /// <param name="treename"></param>
        /// <param name="inputObjects"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        IQueryResultCacheKey GetKey(Uri[] rootfiles, string treename, object[] inputObjects, string[] crumbs, QueryModel query, bool recheckDates = false);

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
        Tuple<bool, T> Lookup<T>(IQueryResultCacheKey key, IVariableSaver varSaver, IDeclaredParameter theVar);

        /// <summary>
        /// Save an item for later lookup and retrieval.
        /// </summary>
        /// <param name="_rootFile"></param>
        /// <param name="qm"></param>
        /// <param name="o"></param>
        void CacheItem(IQueryResultCacheKey key, ROOTNET.Interface.NTObject[] o);
    }
}
