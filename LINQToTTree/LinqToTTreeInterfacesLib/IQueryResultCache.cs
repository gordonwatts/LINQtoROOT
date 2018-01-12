using System;
using Remotion.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <param name="crumbs"></param>
        /// <param name="dateChecker">Given a Uri, return a date for it</param>
        /// <returns></returns>
        IQueryResultCacheKey GetKey(Uri[] rootfiles, string treename, object[] inputObjects, string[] crumbs, QueryModel query, bool recheckDates = false,
            Func<Uri, DateTime> dateChecker = null);

        /// <summary>
        /// Have we made this query before? Check the file date, the query string. If there is a match,
        /// then extract the value and return it. Returns false for the first arg of the tuple if nothing found,
        /// and if true the second arg contains the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_rootFile"></param>
        /// <param name="queryModel"></param>
        /// <param name="iVariable"></param>
        /// <param name="generateAdder">Called to generate an adder if it is needed to sum together cycles</param>
        /// <returns>The value</returns>
        /// <remarks>
        /// How the item is cached may be in a different type than T - this will use the variable saver infrastructure to do
        /// the conversion.
        /// 
        /// Further, several results may have gone into this value - this will use the Adder framework to do the combination.
        /// </remarks>
        Task<Tuple<bool, T>> Lookup<T>(IQueryResultCacheKey key, IVariableSaver varSaver, IDeclaredParameter theVar,
            Func<IAddResult> generateAdder = null);

        /// <summary>
        /// Save a single item for later lookup and retrieval.
        /// </summary>
        /// <param name="_rootFile"></param>
        /// <param name="qm"></param>
        /// <param name="o"></param>
        Task CacheItem(IQueryResultCacheKey key, RunInfo[] o);

        /// <summary>
        /// A single item has a few results that go into it. Cache them all.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cycleOfItems"></param>
        Task CacheItem(IQueryResultCacheKey key, IEnumerable<RunInfo[]> cycleOfItems);
    }
}
