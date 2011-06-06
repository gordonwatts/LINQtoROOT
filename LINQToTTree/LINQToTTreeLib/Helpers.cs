﻿using System;
using System.Linq;
using System.Linq.Expressions;
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
