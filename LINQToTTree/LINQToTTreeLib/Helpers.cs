using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqToTTreeInterfacesLib;
using Remotion.Linq;

namespace LINQToTTreeLib
{
    public static class Helpers
    {
        /// <summary>
        /// Implement the Take operator, and do it just as we would the normal take operator, but we are explicitly allowing it
        /// to happen after a Concat operator that joins multiple sources (e.g. 3000 jets from each different MC sample).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The series of objects to count.</param>
        /// <param name="count">The number of events to count per source. If there are n sources, then n*count is number of objects that will make it through.</param>
        /// <returns></returns>
        public static IQueryable<T> TakePerSource<T> (this IQueryable<T> source, int count)
        {
            return source.Provider.CreateQuery<T>(
                Expression.Call(((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T)),
                source.Expression, Expression.Constant(count)));
        }

        /// <summary>
        /// Implement the Skip operator, and do it just as we would the normal take operator, but we are explicitly allowing it
        /// to happen after a Concat operator that joins multiple sources (e.g. skip the first 3000 jets from each different MC sample).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The series of objects to count.</param>
        /// <param name="count">The number of events to skip at the start of each source.</param>
        /// <returns></returns>
        public static IQueryable<T> SkipPerSource<T>(this IQueryable<T> source, int count)
        {
            return source.Provider.CreateQuery<T>(
                Expression.Call(((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T)),
                source.Expression, Expression.Constant(count)));
        }

        /// <summary>
        /// Make a nicely formatted print of the complete query up to this point. This is useful when you are trying
        /// to archive for later lookup how a value was calculated. It is not meant to be machine readable - human
        /// readable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string PrettyPrintQuery<T>(this IQueryable<T> source)
        {
            // If we can't figure out how to get to the query parser, then return a simple string.
            var qp = source.Provider as DefaultQueryProvider;
            if (qp == null)
            {
                return source.Expression.ToString();
            }

            // Parse the query
            var query = qp.QueryParser.GetParsedQuery(source.Expression);
            return query.ToString();
        }

        /// <summary>
        /// A helper function that allows one to pop this into our LINQ translation. This way h.Fill() can be done
        /// s.t. it returns h (and use the Aggregate function). This is translated magically by our translator!! and
        /// should not be used inline!
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        public static T1 ApplyReturnFirst<T1, T2>(T1 first, T2 second, Expression<Action<T1, T2>> act)
        {
            throw new NotImplementedException("This method should never be called on the host!");
            //act(first, second);
            //return first;
        }

        /// <summary>
        /// Applied to a sequence of items, this will return a list of pairs - every unique pair. For example, if
        /// the sequence o1, o2, o3 comes in, it will return (o1, o2), (o1, o3), (o2, o3). Can be used, for example,
        /// it examine all pairs of jets in an event.
        /// WARNING: do not use @ top level (i.e. to make all pairs of events).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IQueryable<Tuple<T, T>> UniqueCombinations<T>(this IQueryable<T> source)
        {
            // Template for code from fabian's blog: https://www.re-motion.org/blogs/mix/2010/10/28/re-linq-extensibility-custom-query-operators
            // This is a full-on result operator, so it is parsed more deeply by the linq infrastructure and the re-linq infrastructure.
            return source.Provider.CreateQuery<Tuple<T, T>>(
                Expression.Call(((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T)),
                source.Expression));
        }

        /// <summary>
        /// Applied to a sequence of items, this will return a list of pairs - every unique pair. For example, if
        /// the sequence o1, o2, o3 comes in, it will return (o1, o2), (o1, o3), (o2, o3). Can be used, for example,
        /// it examine all pairs of jets in an event.
        /// WARNING: do not use @ top level (i.e. to make all pairs of events).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<T, T>> UniqueCombinations<T>(this IEnumerable<T> source)
        {
            var inputs = source.ToArray();
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = i + 1; j < inputs.Length; j++)
                {
                    yield return Tuple.Create(inputs[i], inputs[j]);
                }
            }
        }

        /// <summary>
        /// Test each jet in comparison with the other. It must satisfy the test with every other item
        /// in the list. If it passes the test with all other objects then it is "good" and it is
        /// returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="mustBeGoodForAll"></param>
        /// <returns></returns>
        public static IEnumerable<T> PairWiseAll<T>(this IEnumerable<T> source, Expression<Func<T, T, bool>> mustBeGoodForAll)
        {
            throw new NotImplementedException();
#if false
            var inputs = source.ToArray();
            for (int i = 0; i < inputs.Length; i++)
            {
                bool onefalse = false;
                for (int j = 0; j < inputs.Length; j++)
                {
                    if (i != j)
                    {
                        if (!mustBeGoodForAll(inputs[i], inputs[j]))
                        {
                            onefalse = true;
                        }
                    }
                }
                if (!onefalse) yield return inputs[i];
            }
#endif
        }

        public static IQueryable<T> PairWiseAll<T>(this IQueryable<T> source, Expression<Func<T, T, bool>> mustBeGoodForAll)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// given an object, apply each iteration to that object using a func, and return the object when done. Can
        /// be used much like Aggregate (o.Fill(arg)).
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="seed"></param>
        /// <param name="apply"></param>
        /// <returns></returns>
        public static TResult ApplyToObject<TSource, TResult>(this IQueryable<TSource> source, TResult seed, Expression<Action<TResult, TSource>> apply)
        {
            var applyMethod = typeof(Helpers).GetMethod("ApplyReturnFirst").MakeGenericMethod(typeof(TResult), typeof(TSource));

            var sParameter = Expression.Parameter(typeof(TResult), "s");
            var nParameter = Expression.Parameter(typeof(TSource), "n");

            var func = Expression.Lambda<Func<TResult, TSource, TResult>>(
                Expression.Call(applyMethod, sParameter, nParameter, apply),
                sParameter,
                nParameter);

            return source.Aggregate(seed, func);
        }

        /// <summary>
        /// given an object, apply each iteration to that object using a func, and return the object when done. Can
        /// be used much like Aggregate (o.Fill(arg)). Result is in the future.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="seed"></param>
        /// <param name="apply"></param>
        /// <returns></returns>
        public static IFutureValue<TResult> FutureApplyToObject<TSource, TResult>(this IQueryable<TSource> source, TResult seed, Expression<Action<TResult, TSource>> apply)
        {
            var applyMethod = typeof(Helpers).GetMethod("ApplyReturnFirst").MakeGenericMethod(typeof(TResult), typeof(TSource));

            var sParameter = Expression.Parameter(typeof(TResult), "s");
            var nParameter = Expression.Parameter(typeof(TSource), "n");

            var func = Expression.Lambda<Func<TResult, TSource, TResult>>(
                Expression.Call(applyMethod, sParameter, nParameter, apply),
                sParameter,
                nParameter);

            return source.FutureAggregate(seed, func);
        }

        #region Invoke Helpers

        /// <summary>
        /// Invoke an expression with no arguments. The expression will be substituted (at the expression tree level) into the query.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        public static R Invoke<R>(this Expression<Func<R>> function)
        {
            throw new NotSupportedException("No direct call allowed");
        }

        /// <summary>
        /// Invoke an expression with arguments. The expression will be substituted (at the expression tree level) into the query.
        /// </summary>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="function"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static R Invoke<A1, R>(this Expression<Func<A1, R>> function, A1 arg1)
        {
            throw new NotSupportedException("No direct call allowed");
        }

        /// <summary>
        /// Invoke an expression with arguments. The expression will be substituted (at the expression tree level) into the query.
        /// </summary>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="function"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static R Invoke<A1, A2, R>(this Expression<Func<A1, A2, R>> function, A1 arg1, A2 arg2)
        {
            throw new NotSupportedException("No direct call allowed");
        }

        /// <summary>
        /// Invoke an expression with arguments. The expression will be substituted (at the expression tree level) into the query.
        /// </summary>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <typeparam name="A3"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="function"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public static R Invoke<A1, A2, A3, R>(this Expression<Func<A1, A2, A3, R>> function, A1 arg1, A2 arg2, A3 arg3)
        {
            throw new NotSupportedException("No direct call allowed");
        }

        /// <summary>
        /// Invoke an expression with arguments. The expression will be substituted (at the expression tree level) into the query.
        /// </summary>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <typeparam name="A3"></typeparam>
        /// <typeparam name="A4"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="function"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <returns></returns>
        public static R Invoke<A1, A2, A3, A4, R>(this Expression<Func<A1, A2, A3, A4, R>> function, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
        {
            throw new NotSupportedException("No direct call allowed");
        }

        /// <summary>
        /// Invoke an expression with arguments. The expression will be substituted (at the expression tree level) into the query.
        /// </summary>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <typeparam name="A3"></typeparam>
        /// <typeparam name="A4"></typeparam>
        /// <typeparam name="A5"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="function"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <returns></returns>
        public static R Invoke<A1, A2, A3, A4, A5, R>(this Expression<Func<A1, A2, A3, A4, A5, R>> function, A1 arg1, A2 arg2, A3 arg3, A4 arg4, A5 arg5)
        {
            throw new NotSupportedException("No direct call allowed");
        }

        /// <summary>
        /// Invoke an expression with arguments. The expression will be substituted (at the expression tree level) into the query.
        /// </summary>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <typeparam name="A3"></typeparam>
        /// <typeparam name="A4"></typeparam>
        /// <typeparam name="A5"></typeparam>
        /// <typeparam name="A6"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="function"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <returns></returns>
        public static R Invoke<A1, A2, A3, A4, A5, A6, R>(this Expression<Func<A1, A2, A3, A4, A5, A6, R>> function, A1 arg1, A2 arg2, A3 arg3, A4 arg4, A5 arg5, A6 arg6)
        {
            throw new NotSupportedException("No direct call allowed");
        }

        /// <summary>
        /// Invoke an expression with arguments. The expression will be substituted (at the expression tree level) into the query.
        /// </summary>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <typeparam name="A3"></typeparam>
        /// <typeparam name="A4"></typeparam>
        /// <typeparam name="A5"></typeparam>
        /// <typeparam name="A6"></typeparam>
        /// <typeparam name="A7"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="function"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <returns></returns>
        public static R Invoke<A1, A2, A3, A4, A5, A6, A7, R>(this Expression<Func<A1, A2, A3, A4, A5, A6, A7, R>> function, A1 arg1, A2 arg2, A3 arg3, A4 arg4, A5 arg5, A6 arg6, A7 arg7)
        {
            throw new NotSupportedException("No direct call allowed");
        }

        /// <summary>
        /// Invoke an expression with arguments. The expression will be substituted (at the expression tree level) into the query.
        /// </summary>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <typeparam name="A3"></typeparam>
        /// <typeparam name="A4"></typeparam>
        /// <typeparam name="A5"></typeparam>
        /// <typeparam name="A6"></typeparam>
        /// <typeparam name="A7"></typeparam>
        /// <typeparam name="A8"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="function"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <param name="arg8"></param>
        /// <returns></returns>
        public static R Invoke<A1, A2, A3, A4, A5, A6, A7, A8, R>(this Expression<Func<A1, A2, A3, A4, A5, A6, A7, A8, R>> function, A1 arg1, A2 arg2, A3 arg3, A4 arg4, A5 arg5, A6 arg6, A7 arg7, A8 arg8)
        {
            throw new NotSupportedException("No direct call allowed");
        }

        /// <summary>
        /// Invoke an expression with arguments. The expression will be substituted (at the expression tree level) into the query.
        /// </summary>
        /// <typeparam name="A1"></typeparam>
        /// <typeparam name="A2"></typeparam>
        /// <typeparam name="A3"></typeparam>
        /// <typeparam name="A4"></typeparam>
        /// <typeparam name="A5"></typeparam>
        /// <typeparam name="A6"></typeparam>
        /// <typeparam name="A7"></typeparam>
        /// <typeparam name="A8"></typeparam>
        /// <typeparam name="A9"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="function"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <param name="arg8"></param>
        /// <param name="arg9"></param>
        /// <returns></returns>
        public static R Invoke<A1, A2, A3, A4, A5, A6, A7, A8, A9, R>(this Expression<Func<A1, A2, A3, A4, A5, A6, A7, A8, A9, R>> function, A1 arg1, A2 arg2, A3 arg3, A4 arg4, A5 arg5, A6 arg6, A7 arg7, A8 arg8, A9 arg9)
        {
            throw new NotSupportedException("No direct call allowed");
        }

        /// <summary>
        /// Returns true if a pointer in one objects points to a valid item.
        /// For example if the array index is -1 it will return false.
        /// Used when you have an TTree leaf that is an index into another array in the TTree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool IsGoodIndex<T>(this T arg)
        {
            throw new NotSupportedException("No direct call allowed - this is a marker for the translation process in LINQToROOT");
        }
        #endregion
    }
}
