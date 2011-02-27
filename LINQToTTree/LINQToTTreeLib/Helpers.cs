using System;
using System.Linq;
using System.Linq.Expressions;

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
        /// Aggregate, but don't return an item from our function... a secret way around a limitation in how
        /// Expression Trees are parsed.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="seed"></param>
        /// <param name="apply"></param>
        /// <returns></returns>
        public static TResult Aggregate<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TResult>> seed, Action<TResult, TSource> apply)
        {
            return source.Aggregate(seed, (s, n) => ApplyReturnFirst(s, n, (s1, n1) => apply(s1, n1)));
        }

        public static TResult ApplyToObject<TSource, TResult>(this IQueryable<TSource> source, TResult seed, Expression<Action<TResult, TSource>> apply)
        {
            return source.Aggregate(seed, (s, n) => ApplyReturnFirst(s, n, apply));
        }
    }
}
