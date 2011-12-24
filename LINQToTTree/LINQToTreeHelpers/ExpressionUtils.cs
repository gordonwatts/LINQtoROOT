
using System;
using System.Linq.Expressions;
namespace LINQToTreeHelpers
{
    /// <summary>
    /// Some handly utilities to combine expressions. These get used
    /// because one wants to parameterize expressions.
    /// </summary>
    public static class ExpressionUtils
    {
        /// <summary>
        /// Given two expressions, create a composition. Makes code in others plces much simpler! :-)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static Expression<Func<T1, T3>> C<T1, T2, T3>(this Expression<Func<T1, T2>> f1, Expression<Func<T2, T3>> f2)
        {
            var param = Expression.Parameter(typeof(T1), "p");
            var f1Call = Expression.Invoke(f1, param);
            var f2Call = Expression.Invoke(f2, f1Call);
            var result = Expression.Lambda(f2Call, param) as Expression<Func<T1, T3>>;
            return result;
        }

        /// <summary>
        /// And two functions together that have a single input parameter and return a bool.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static Expression<Func<T1, bool>> And<T1>(this Expression<Func<T1, bool>> f1, Expression<Func<T1, bool>> f2)
        {
            var param = Expression.Parameter(typeof(T1), "p");
            var f1Call = Expression.Invoke(f1, param);
            var f2Call = Expression.Invoke(f2, param);
            var and = Expression.AndAlso(f1Call, f2Call);
            var result = Expression.Lambda(and, param) as Expression<Func<T1, bool>>;
            return result;
        }
    }
}
