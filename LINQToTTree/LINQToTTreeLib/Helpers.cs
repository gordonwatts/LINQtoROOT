using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib
{
    public static class Helpers
    {
        /// <summary>
        /// Returns all files below the base directory whose name (including extension) match the regex pattern.
        /// </summary>
        /// <param name="baseDir"></param>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> FindAllFiles(this DirectoryInfo baseDir, string pattern)
        {
            var subfiles = from subdir in baseDir.EnumerateDirectories()
                           from f in subdir.FindAllFiles(pattern)
                           select f;

            Regex matcher = new Regex(pattern);
            var goodFiles = from f in baseDir.EnumerateFiles()
                            where matcher.Match(f.Name).Success
                            select f;

            var allfiles = subfiles.Concat(goodFiles);

            return allfiles;
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
        /// Create a TH1F plot from a stream of objects (with a lambda function to give flexability in conversion).
        /// For future evaluation
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
        public static IFutureValue<ROOTNET.Interface.NTH1F> FuturePlot<TSource>
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

            var lambda = Expression.Lambda<Action<ROOTNET.Interface.NTH1F, TSource>>(callFill, hParameter, vParameter);
            return source.FutureApplyToObject(new ROOTNET.NTH1F(plotID, plotTitle, nbins, lowBin, highBin), lambda);
        }

        /// <summary>
        /// Fill a 2D plot
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="plotID"></param>
        /// <param name="plotTitle"></param>
        /// <param name="xNBins"></param>
        /// <param name="xLowBin"></param>
        /// <param name="xHighBin"></param>
        /// <param name="yNBins"></param>
        /// <param name="yLowBin"></param>
        /// <param name="yHighBin"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH2F Plot<TSource>
            (
            this IQueryable<TSource> source,
            string plotID, string plotTitle,
            int xNBins, double xLowBin, double xHighBin,
            int yNBins, double yLowBin, double yHighBin,
            Expression<Func<TSource, double>> xGetter,
            Expression<Func<TSource, double>> yGetter
            )
        {
            var hParameter = Expression.Parameter(typeof(ROOTNET.NTH2F), "h");
            var vParameter = Expression.Parameter(typeof(TSource), "v");

            var callXGetter = Expression.Invoke(xGetter, vParameter);
            var callYGetter = Expression.Invoke(yGetter, vParameter);

            var fillMethod = typeof(ROOTNET.NTH2F).GetMethod("Fill", new Type[] { typeof(double), typeof(double) });
            var callFill = Expression.Call(hParameter, fillMethod, callXGetter, callYGetter);

            var lambda = Expression.Lambda<Action<ROOTNET.NTH2F, TSource>>(callFill, hParameter, vParameter);
            return source.ApplyToObject(new ROOTNET.NTH2F(plotID, plotTitle, xNBins, xLowBin, xHighBin, yNBins, yLowBin, yHighBin), lambda);
        }

        /// <summary>
        /// Fill a 2D plot in the future
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="plotID"></param>
        /// <param name="plotTitle"></param>
        /// <param name="xNBins"></param>
        /// <param name="xLowBin"></param>
        /// <param name="xHighBin"></param>
        /// <param name="yNBins"></param>
        /// <param name="yLowBin"></param>
        /// <param name="yHighBin"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static IFutureValue<ROOTNET.Interface.NTH2F> FuturePlot<TSource>
            (
            this IQueryable<TSource> source,
            string plotID, string plotTitle,
            int xNBins, double xLowBin, double xHighBin,
            int yNBins, double yLowBin, double yHighBin,
            Expression<Func<TSource, double>> xGetter,
            Expression<Func<TSource, double>> yGetter
            )
        {
            var hParameter = Expression.Parameter(typeof(ROOTNET.Interface.NTH2F), "h");
            var vParameter = Expression.Parameter(typeof(TSource), "v");

            var callXGetter = Expression.Invoke(xGetter, vParameter);
            var callYGetter = Expression.Invoke(yGetter, vParameter);

            var fillMethod = typeof(ROOTNET.NTH2F).GetMethod("Fill", new Type[] { typeof(double), typeof(double) });
            var callFill = Expression.Call(hParameter, fillMethod, callXGetter, callYGetter);

            var lambda = Expression.Lambda<Action<ROOTNET.Interface.NTH2F, TSource>>(callFill, hParameter, vParameter);
            ROOTNET.Interface.NTH2F interfaceobj = new ROOTNET.NTH2F(plotID, plotTitle, xNBins, xLowBin, xHighBin, yNBins, yLowBin, yHighBin);
            return source.FutureApplyToObject(interfaceobj, lambda);
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
        public static ROOTNET.Interface.NTH1F Plot<T>
            (
            this IQueryable<T> source,
            string plotID, string plotTitle,
            int nbins, double lowBin, double highBin
            )
            where T : IConvertible
        {
            return source.Plot(plotID, plotTitle, nbins, lowBin, highBin, v => Convert.ToDouble(v));
        }

        /// <summary>
        /// Generate a TH1F (in the future) from a stream of T's that can be converted to a double.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="plotID"></param>
        /// <param name="plotTitle"></param>
        /// <param name="nbins"></param>
        /// <param name="lowBin"></param>
        /// <param name="highBin"></param>
        /// <returns></returns>
        public static IFutureValue<ROOTNET.Interface.NTH1F> FuturePlot<T>
            (
            this IQueryable<T> source,
            string plotID, string plotTitle,
            int nbins, double lowBin, double highBin
            )
            where T : IConvertible
        {
            return source.FuturePlot(plotID, plotTitle, nbins, lowBin, highBin, v => Convert.ToDouble(v));
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
        /// 
        /// Temp Fix: We need to set the object owner so that this object won't be cleaned up during
        /// GC, however, ROOT.NET doesn't support it yet. So instead we will just set it to null and
        /// return null for now. To be fixed when we update ROOT.NET.
        /// </summary>
        /// <param name="hist"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 SaveToROOTDirectory(this ROOTNET.Interface.NTH1 hist, ROOTNET.Interface.NTDirectory dir)
        {
            hist.SetDirectory(dir);
            hist.SetNull();
            return null;
        }
    }
}
