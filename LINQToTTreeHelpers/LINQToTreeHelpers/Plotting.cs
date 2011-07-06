using System;
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
            Expression<Func<TSource, double>> getter,
            Expression<Func<TSource, double>> weight = null)
        {
            if (weight == null)
            {
                Expression<Func<TSource, double>> constWeight = s => 1.0;
                weight = constWeight;
            }

            var hParameter = Expression.Parameter(typeof(ROOTNET.NTH1F), "h");
            var vParameter = Expression.Parameter(typeof(TSource), "v");

            // h.Fill(getter(v), weight(v)) is what we want to code up

            var callGetter = Expression.Invoke(getter, vParameter);
            var callWeight = Expression.Invoke(weight, vParameter);

            var fillMethod = typeof(ROOTNET.NTH1F).GetMethod("Fill", new[] { typeof(double), typeof(double) });
            var callFill = Expression.Call(hParameter, fillMethod, callGetter, callWeight);

            var lambda = Expression.Lambda<Action<ROOTNET.NTH1F, TSource>>(callFill, hParameter, vParameter);
            var h = new ROOTNET.NTH1F(plotID, plotTitle, nbins, lowBin, highBin);
            ConfigureHisto(h);
            return source.ApplyToObject(h, lambda);
        }

        /// <summary>
        /// Configure the histogram - sumw2, for example.
        /// </summary>
        /// <param name="h"></param>
        private static void ConfigureHisto(ROOTNET.Interface.NTH1 h)
        {
            h.Sumw2();
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
            Expression<Func<TSource, double>> getter,
            Expression<Func<TSource, double>> weight = null)
        {
            if (weight == null)
            {
                Expression<Func<TSource, double>> constWeight = s => 1.0;
                weight = constWeight;
            }

            var hParameter = Expression.Parameter(typeof(ROOTNET.NTH1F), "h");
            var vParameter = Expression.Parameter(typeof(TSource), "v");

            // h.Fill(getter(v), weight(v)) is what we want to code up

            var callGetter = Expression.Invoke(getter, vParameter);
            var callWeight = Expression.Invoke(weight, vParameter);

            var fillMethod = typeof(ROOTNET.NTH1F).GetMethod("Fill", new[] { typeof(double), typeof(double) });
            var callFill = Expression.Call(hParameter, fillMethod, callGetter, callWeight);

            var lambda = Expression.Lambda<Action<ROOTNET.NTH1F, TSource>>(callFill, hParameter, vParameter);
            var h = new ROOTNET.NTH1F(plotID, plotTitle, nbins, lowBin, highBin);
            ConfigureHisto(h);
            return source.FutureApplyToObject(h, lambda);
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
        /// <param name="xGetter">Func that calculates the X value for the 2D histogram</param>
        /// <param name="yGetter">Func that calculates the X value for the 2D histogram</param>
        /// <returns></returns>
        public static ROOTNET.NTH2F Plot<TSource>
            (
            this IQueryable<TSource> source,
            string plotID, string plotTitle,
            int xNBins, double xLowBin, double xHighBin,
            int yNBins, double yLowBin, double yHighBin,
            Expression<Func<TSource, double>> xGetter,
            Expression<Func<TSource, double>> yGetter,
            Expression<Func<TSource, double>> weight = null
            )
        {
            if (weight == null)
            {
                Expression<Func<TSource, double>> constWeight = s => 1.0;
                weight = constWeight;
            }

            var hParameter = Expression.Parameter(typeof(ROOTNET.NTH2F), "h");
            var vParameter = Expression.Parameter(typeof(TSource), "v");

            var callXGetter = Expression.Invoke(xGetter, vParameter);
            var callYGetter = Expression.Invoke(yGetter, vParameter);
            var callWeight = Expression.Invoke(weight, vParameter);
            var fillMethod = typeof(ROOTNET.NTH2F).GetMethod("Fill", new[] { typeof(double), typeof(double), typeof(double) });
            var callFill = Expression.Call(hParameter, fillMethod, callXGetter, callYGetter, callWeight);

            var lambda = Expression.Lambda<Action<ROOTNET.NTH2F, TSource>>(callFill, hParameter, vParameter);
            var h = new ROOTNET.NTH2F(plotID, plotTitle, xNBins, xLowBin, xHighBin, yNBins, yLowBin, yHighBin);
            ConfigureHisto(h);
            return source.ApplyToObject(h, lambda);
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
        /// <param name="xGetter">Func that calculates the X value for the 2D histogram</param>
        /// <param name="yGetter">Func that calculates the X value for the 2D histogram</param>
        /// <returns></returns>
        public static IFutureValue<ROOTNET.NTH2F> FuturePlot<TSource>
            (
            this IQueryable<TSource> source,
            string plotID, string plotTitle,
            int xNBins, double xLowBin, double xHighBin,
            int yNBins, double yLowBin, double yHighBin,
            Expression<Func<TSource, double>> xGetter,
            Expression<Func<TSource, double>> yGetter,
            Expression<Func<TSource, double>> weight = null
            )
        {
            if (weight == null)
            {
                Expression<Func<TSource, double>> constWeight = s => 1.0;
                weight = constWeight;
            }
            var hParameter = Expression.Parameter(typeof(ROOTNET.NTH2F), "h");
            var vParameter = Expression.Parameter(typeof(TSource), "v");

            var callXGetter = Expression.Invoke(xGetter, vParameter);
            var callYGetter = Expression.Invoke(yGetter, vParameter);
            var callWeight = Expression.Invoke(weight, vParameter);

            var fillMethod = typeof(ROOTNET.NTH2F).GetMethod("Fill", new[] { typeof(double), typeof(double), typeof(double) });
            var callFill = Expression.Call(hParameter, fillMethod, callXGetter, callYGetter, weight);

            var lambda = Expression.Lambda<Action<ROOTNET.NTH2F, TSource>>(callFill, hParameter, vParameter);
            var interfaceobj = new ROOTNET.NTH2F(plotID, plotTitle, xNBins, xLowBin, xHighBin, yNBins, yLowBin, yHighBin);
            ConfigureHisto(interfaceobj);
            return source.FutureApplyToObject(interfaceobj, lambda);
        }

        /// <summary>
        /// Generate a TH1F from a stream of T's that can be converted to a double.
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
        /// <typeparam name="T"></typeparam>
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
