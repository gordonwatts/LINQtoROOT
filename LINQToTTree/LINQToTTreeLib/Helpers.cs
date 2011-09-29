using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib
{
    public static class Helpers
    {
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
    }
}
