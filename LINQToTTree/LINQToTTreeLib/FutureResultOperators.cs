using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using Remotion.Linq;

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
            ///
            /// Really - we can deal with only *one* type of querable!!
            /// 

            var q = query as QueriableTTree<TSource>;
            if (q == null)
                throw new ArgumentException("Query must be on a TTree in order to use futures!");

            ///
            /// Build up the count expression.
            /// typeof(Queryable).GetMethod("Count", new Type[] { typeof(IQueryable<>) }) doesn't work?
            /// 

            var countMethodGeneric = typeof(Queryable).GetMethods().Where(m => m.Name == "Count").Where(m => m.GetParameters().Length == 1).First();
            var countMethod = countMethodGeneric.MakeGenericMethod(new Type[] { typeof(TSource) });
            var expr = Expression.Call(null, countMethod, query.Expression);

            ///
            /// Generate the query model
            /// 

            var qpb = q.Provider as QueryProviderBase;
            var qm = qpb.GenerateQueryModel(expr);

            ///
            /// Now, cache it and queue it up for building
            /// 

            return new FutureValue<int>(); /// coming soon
        }

    }
}
