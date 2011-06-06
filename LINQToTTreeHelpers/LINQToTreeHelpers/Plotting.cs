﻿using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib;

namespace LINQToTreeHelpers
{
    /// <summary>
    /// Helper functions that deal with plots
    /// </summary>
    public static class Plotting
    {
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
        public static ROOTNET.NTH1F Plot<TSource>
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
        public static IFutureValue<ROOTNET.NTH1F> FuturePlot<TSource>
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
            return source.FutureApplyToObject(new ROOTNET.NTH1F(plotID, plotTitle, nbins, lowBin, highBin) as ROOTNET.NTH1F, lambda);
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
        public static ROOTNET.NTH2F Plot<TSource>
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
        public static IFutureValue<ROOTNET.NTH2F> FuturePlot<TSource>
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
            var interfaceobj = new ROOTNET.NTH2F(plotID, plotTitle, xNBins, xLowBin, xHighBin, yNBins, yLowBin, yHighBin);
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
        public static ROOTNET.NTH1F Plot<T>
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
        public static IFutureValue<ROOTNET.NTH1F> FuturePlot<T>
            (
            this IQueryable<T> source,
            string plotID, string plotTitle,
            int nbins, double lowBin, double highBin
            )
            where T : IConvertible
        {
            return source.FuturePlot(plotID, plotTitle, nbins, lowBin, highBin, v => Convert.ToDouble(v));
        }

    }
}
