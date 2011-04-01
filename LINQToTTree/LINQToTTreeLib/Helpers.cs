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
#if false
            return source.Aggregate(seed, (s, n) => ApplyReturnFirst(s, n, apply));
#endif
        }

        /// <summary>
        /// Create a TH1F plot from a stream of objects (with a lambda function to give flexability in conversion).
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="plotID"></param>
        /// <param name="plotTitle"></param>
        /// <param name="nbins"></param>
        /// <param name="lowBin"></param>
        /// <param name="highBin"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1F Plot<TSource>
            (
            this IQueryable<TSource> source,
            string plotID, string plotTitle,
            int nbins, double lowBin, double highBin,
            Expression<Func<TSource, double>> getter)
        {
            var hParameter = Expression.Parameter(typeof(ROOTNET.NTH1F), "h");
            var vParameter = Expression.Parameter(typeof(TSource), "v");

            /// h.Fill(getter(v)) is what we want to code up

            var callGetter = Expression.Invoke(getter, vParameter);

            var fillMethod = typeof(ROOTNET.NTH1F).GetMethod("Fill", new Type[] { typeof(double) });
            var callFill = Expression.Call(hParameter, fillMethod, callGetter);

            var lambda = Expression.Lambda<Action<ROOTNET.NTH1F, TSource>>(callFill, hParameter, vParameter);
            return source.ApplyToObject(new ROOTNET.NTH1F(plotID, plotTitle, nbins, lowBin, highBin), lambda);
        }

        /// <summary>
        /// Generate a TH1F from a stream of T's that can be converted to a double.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="plotID"></param>
        /// <param name="plotTitle"></param>
        /// <param name="nbins"></param>
        /// <param name="lowBin"></param>
        /// <param name="highBin"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1F Plot
            (
            this IQueryable<double> source,
            string plotID, string plotTitle,
            int nbins, double lowBin, double highBin)
        {
            return source.Plot(plotID, plotTitle, nbins, lowBin, highBin, v => v);
        }
        public static ROOTNET.Interface.NTH1F Plot
            (
            this IQueryable<float> source,
            string plotID, string plotTitle,
            int nbins, double lowBin, double highBin)
        {
            return source.Plot(plotID, plotTitle, nbins, lowBin, highBin, v => v);
        }

        public static ROOTNET.Interface.NTH1F Plot
            (
            this IQueryable<int> source,
            string plotID, string plotTitle,
            int nbins, double lowBin, double highBin)
        {
            return source.Plot(plotID, plotTitle, nbins, lowBin, highBin, v => v);
        }

        /// <summary>
        /// Add a histogram to a ROOT directory.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="h"></param>
        public static void Add(this ROOTNET.Interface.NTDirectory dir, ROOTNET.Interface.NTH1 h)
        {
            h.SetDirectory(dir);
        }

        /// <summary>
        /// Save a plot to a TDirectory. Return the plot so it can also be used in other
        /// places.
        /// </summary>
        /// <param name="hist"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 SaveToROOTDirectory(this ROOTNET.Interface.NTH1 hist, ROOTNET.Interface.NTDirectory dir)
        {
            hist.SetDirectory(dir);
            return hist;
        }
    }
}
