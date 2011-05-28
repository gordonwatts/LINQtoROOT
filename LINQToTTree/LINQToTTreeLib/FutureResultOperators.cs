using System;
using System.Linq;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Operators that return results that are Futures. This allows the user
    /// to batch up a bunch of queries and then have them execute as a batch.
    /// For docs see: 
    /// </summary>
    public static class FutureResultOperators
    {
        /// <summary>
        /// Returns a future that will give the number of elements in a sequence.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IFutureValue<int> FutureCount<TSource>(this IQueryable<TSource> query)
        {
            var q = query as QueriableTTree<TSource>;
            if (q == null)
                throw new ArgumentException("Query must be on a TTree in order to use futures!");

            return new FutureValue<int>();
        }

    }
}
