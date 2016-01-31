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
            var q = CheckSource<TSource>(query);

            ///
            /// Build up the count expression.
            /// typeof(Queryable).GetMethod("Count", new Type[] { typeof(IQueryable<>) }) doesn't work?
            /// 

            var countMethodGeneric = typeof(Queryable).GetMethods().Where(m => m.Name == "Count").Where(m => m.GetParameters().Length == 1).First();
            var countMethod = countMethodGeneric.MakeGenericMethod(new Type[] { typeof(TSource) });
            var expr = Expression.Call(null, countMethod, query.Expression);

            return FutureExecuteScalarHelper<TSource, int>(q, expr);
        }

        /// <summary>
        /// Helper method to execute a scalar as a future.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="q"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        internal static IFutureValue<TResult> FutureExecuteScalarHelper<TSource, TResult>(QueriableTTree<TSource> q, MethodCallExpression expr)
        {
            ///
            /// Generate the query model
            /// 

            var qpb = q.Provider as QueryProviderBase;
            var qm = qpb.GenerateQueryModel(expr);

            ///
            /// Use the queriable t tree to get the value. This may include
            /// doing a cache lookup, btw.
            /// 

            var ttree = qpb.Executor as TTreeQueryExecutor;
            return ttree.ExecuteScalarAsFuture<TResult>(qm);
        }

        /// <summary>
        /// Check and convert the source to the proper type. Throw
        /// if it isn't "right" for us to process with a future.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        internal static QueriableTTree<TSource> CheckSource<TSource>(IQueryable<TSource> query)
        {
            ///
            /// Really - we can deal with only *one* type of querable!!
            /// 

            var q = query as QueriableTTree<TSource>;
            if (q == null)
                throw new ArgumentException("Query must be on a TTree in order to use futures!");
            return q;
        }

        /// <summary>
        /// Run the future aggregate function
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <param name="source"></param>
        /// <param name="seed"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IFutureValue<TAccumulate> FutureAggregate<TSource, TAccumulate>(this IQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func)
        {
            ///
            /// Make sure the source is in good shape.
            /// 

            var q = CheckSource(source);

            ///
            /// Build the aggragate expression.
            /// 

            var aggregateMethodGeneric = typeof(Queryable).GetMethods().Where(m => m.Name == "Aggregate").Where(m => m.GetParameters().Length == 3).First();

            var aGenericArgs = new Type[]
            {
                typeof(TSource),
                typeof(TAccumulate)
            };
            var aggregateMethod = aggregateMethodGeneric.MakeGenericMethod(aGenericArgs);
            var expr = Expression.Call(null, aggregateMethod, source.Expression, Expression.Constant(seed), Expression.Quote(func));

            ///
            /// Finally, turn this into a query and run the scalar
            /// 

            return FutureExecuteScalarHelper<TSource, TAccumulate>(q, expr);
        }
    }
}
