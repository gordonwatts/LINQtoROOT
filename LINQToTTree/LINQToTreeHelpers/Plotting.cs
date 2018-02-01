using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib;
using LINQToTTreeLib.Utils;

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
        /// <typeparam name="TSource">The type of the sequence that the plot will be run over</typeparam>
        /// <param name="source">The sequence over which a plot should be made. There will be one entry per item in the sequence.</param>
        /// <param name="plotName">The histogram will be created with this name</param>
        /// <param name="plotTitle">The histogram will be created with this title</param>
        /// <param name="nbins">Number of bins this histogram should have</param>
        /// <param name="lowBin">The xmin value for this histogram</param>
        /// <param name="highBin">The xmax value for this histogram</param>
        /// <param name="xValue">A lambda that returns the xvalue for each sequence item.</param>
        /// <param name="weight">A lambda that returns the weight for each sequence item. By default every entry has a weight of 1.</param>
        /// <returns></returns>
        public static ROOTNET.NTH1F Plot<TSource>
            (
            this IQueryable<TSource> source,
            string plotName, string plotTitle,
            int nbins, double lowBin, double highBin,
            Expression<Func<TSource, double>> xValue,
            Expression<Func<TSource, double>> weight = null)
        {
            using (ROOTLock.Lock())
            {
                if (weight == null)
                {
                    Expression<Func<TSource, double>> constWeight = s => 1.0;
                    weight = constWeight;
                }

                var hParameter = Expression.Parameter(typeof(ROOTNET.NTH1F), "h");
                var vParameter = Expression.Parameter(typeof(TSource), "v");

                // h.Fill(getter(v), weight(v)) is what we want to code up

                var callGetter = Expression.Invoke(xValue, vParameter);
                var callWeight = Expression.Invoke(weight, vParameter);

                var fillMethod = typeof(ROOTNET.NTH1F).GetMethod("Fill", new[] { typeof(double), typeof(double) });
                var callFill = Expression.Call(hParameter, fillMethod, callGetter, callWeight);

                var lambda = Expression.Lambda<Action<ROOTNET.NTH1F, TSource>>(callFill, hParameter, vParameter);
                var h = new ROOTNET.NTH1F(plotName, plotTitle.ReplaceLatexStrings(), nbins, lowBin, highBin);
                ConfigureHisto(h);
                return source.ApplyToObject(h, lambda);
            }
        }

        /// <summary>
        /// Configure the histogram - sumw2, for example.
        /// </summary>
        /// <param name="h"></param>
        private static void ConfigureHisto(ROOTNET.Interface.NTH1 h)
        {
            // Keep track of statistics
            h.Sumw2();

            // Make sure it doesn't get associated with a file. This is ok after it comes back from
            // a query.. :-)

            h.Directory = null;
        }

        /// <summary>
        /// Create a TH1F plot from a stream of objects (with a lambda function to give flexability in conversion).
        /// For future evaluation
        /// </summary>
        /// <typeparam name="TSource">The type of the sequence that the plot will be run over</typeparam>
        /// <param name="source">The sequence over which a plot should be made. There will be one entry per item in the sequence.</param>
        /// <param name="plotName">The histogram will be created with this name</param>
        /// <param name="plotTitle">The histogram will be created with this title</param>
        /// <param name="nbins">Number of bins this histogram should have</param>
        /// <param name="lowBin">The xmin value for this histogram</param>
        /// <param name="highBin">The xmax value for this histogram</param>
        /// <param name="xValue">A lambda that returns the xvalue for each sequence item.</param>
        /// <param name="weight">A lambda that returns the weight for each sequence item. By default every entry has a weight of 1.</param>
        /// <returns></returns>
        public static IFutureValue<ROOTNET.NTH1F> FuturePlot<TSource>
            (
            this IQueryable<TSource> source,
            string plotName, string plotTitle,
            int nbins, double lowBin, double highBin,
            Expression<Func<TSource, double>> xValue,
            Expression<Func<TSource, double>> weight = null)
        {
            using (ROOTLock.Lock())
            {
                if (weight == null)
                {
                    Expression<Func<TSource, double>> constWeight = s => 1.0;
                    weight = constWeight;
                }

                var hParameter = Expression.Parameter(typeof(ROOTNET.NTH1F), "h");
                var vParameter = Expression.Parameter(typeof(TSource), "v");

                // h.Fill(getter(v), weight(v)) is what we want to code up

                var callGetter = Expression.Invoke(xValue, vParameter);
                var callWeight = Expression.Invoke(weight, vParameter);

                var fillMethod = typeof(ROOTNET.NTH1F).GetMethod("Fill", new[] { typeof(double), typeof(double) });
                var callFill = Expression.Call(hParameter, fillMethod, callGetter, callWeight);

                var lambda = Expression.Lambda<Action<ROOTNET.NTH1F, TSource>>(callFill, hParameter, vParameter);
                var h = new ROOTNET.NTH1F(plotName, plotTitle.ReplaceLatexStrings(), nbins, lowBin, highBin);
                ConfigureHisto(h);
                return source.FutureApplyToObject(h, lambda);
            }
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
        /// <param name="weight">Func that calculates the weight to fill this entry with</param>
        /// <returns></returns>
        public static ROOTNET.NTH2F Plot<TSource>
            (
            this IQueryable<TSource> source,
            string plotID, string plotTitle,
            int xNBins, double xLowBin, double xHighBin,
            Expression<Func<TSource, double>> xGetter,
            int yNBins, double yLowBin, double yHighBin,
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
            var h = new ROOTNET.NTH2F(plotID, plotTitle.ReplaceLatexStrings(), xNBins, xLowBin, xHighBin, yNBins, yLowBin, yHighBin);
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
        /// <param name="xGetter">A lambda that calculates the X value for the 2D histogram for each item in the sequence</param>
        /// <param name="yGetter">A lambda that calculates the Y value for the 2D histogram for each item in the sequence</param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static IFutureValue<ROOTNET.NTH2F> FuturePlot<TSource>
            (
            this IQueryable<TSource> source,
            string plotID, string plotTitle,
            int xNBins, double xLowBin, double xHighBin,
            Expression<Func<TSource, double>> xGetter,
            int yNBins, double yLowBin, double yHighBin,
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
            var interfaceobj = new ROOTNET.NTH2F(plotID, plotTitle.ReplaceLatexStrings(), xNBins, xLowBin, xHighBin, yNBins, yLowBin, yHighBin);
            ConfigureHisto(interfaceobj);
            return source.FutureApplyToObject(interfaceobj, lambda);
        }

        /// <summary>
        /// Fill a 2D profile plot with one entry per item in the sequence we run over. Lambda's convert each item
        /// to x and y value and (optionally) a weight.
        /// </summary>
        /// <typeparam name="TSource">Type of the sequence that will be plotted</typeparam>
        /// <param name="source">Sequence of items to plot, one entry in the histogram per item.</param>
        /// <param name="plotName">Name of the profile plot as declared to ROOT.</param>
        /// <param name="plotTitle">Title of the plot as declared to ROOT.</param>
        /// <param name="xNBins">Number of bins in along the x-axis</param>
        /// <param name="xLowBin">x axis low edge</param>
        /// <param name="xHighBin">x axis high edge</param>
        /// <param name="yLowBin">y axis low edge</param>
        /// <param name="yHighBin">y axis high edge</param>
        /// <param name="xGetter">Func that calculates the X value for the 2D histogram</param>
        /// <param name="yGetter">Func that calculates the X value for the 2D histogram</param>
        /// <param name="weight">Func that calculates the weight to fill this entry with</param>
        /// <returns></returns>
        public static ROOTNET.NTProfile Profile<TSource>
            (
            this IQueryable<TSource> source,
            string plotName, string plotTitle,
            int xNBins, double xLowBin, double xHighBin,
            Expression<Func<TSource, double>> xGetter,
            double yLowBin, double yHighBin,
            Expression<Func<TSource, double>> yGetter,
            Expression<Func<TSource, double>> weight = null
            )
        {
            if (weight == null)
            {
                Expression<Func<TSource, double>> constWeight = s => 1.0;
                weight = constWeight;
            }

            var hParameter = Expression.Parameter(typeof(ROOTNET.NTProfile), "h");
            var vParameter = Expression.Parameter(typeof(TSource), "v");

            var callXGetter = Expression.Invoke(xGetter, vParameter);
            var callYGetter = Expression.Invoke(yGetter, vParameter);
            var callWeight = Expression.Invoke(weight, vParameter);
            var fillMethod = typeof(ROOTNET.NTProfile).GetMethod("Fill", new[] { typeof(double), typeof(double), typeof(double) });
            var callFill = Expression.Call(hParameter, fillMethod, callXGetter, callYGetter, callWeight);

            var lambda = Expression.Lambda<Action<ROOTNET.NTProfile, TSource>>(callFill, hParameter, vParameter);
            var h = new ROOTNET.NTProfile(plotName, plotTitle.ReplaceLatexStrings(), xNBins, xLowBin, xHighBin, yLowBin, yHighBin);
            ConfigureHisto(h);
            return source.ApplyToObject(h, lambda);
        }

        /// <summary>
        /// Fill a 2D plot in the future
        /// </summary>
        /// <typeparam name="TSource">Type of the sequence that will be plotted</typeparam>
        /// <param name="source">Sequence of items to plot, one entry in the histogram per item.</param>
        /// <param name="plotName">Name of the profile plot as declared to ROOT.</param>
        /// <param name="plotTitle">Title of the plot as declared to ROOT.</param>
        /// <param name="xNBins">Number of bins in along the x-axis</param>
        /// <param name="xLowBin">x axis low edge</param>
        /// <param name="xHighBin">x axis high edge</param>
        /// <param name="yLowBin">y axis low edge</param>
        /// <param name="yHighBin">y axis high edge</param>
        /// <param name="xGetter">Func that calculates the X value for the 2D histogram</param>
        /// <param name="yGetter">Func that calculates the X value for the 2D histogram</param>
        /// <param name="weight">Func that calculates the weight to fill this entry with</param>
        /// <returns></returns>
        public static IFutureValue<ROOTNET.NTProfile> FutureProfile<TSource>
            (
            this IQueryable<TSource> source,
            string plotName, string plotTitle,
            int xNBins, double xLowBin, double xHighBin,
            Expression<Func<TSource, double>> xGetter,
            double yLowBin, double yHighBin,
            Expression<Func<TSource, double>> yGetter,
            Expression<Func<TSource, double>> weight = null
            )
        {
            if (weight == null)
            {
                Expression<Func<TSource, double>> constWeight = s => 1.0;
                weight = constWeight;
            }
            var hParameter = Expression.Parameter(typeof(ROOTNET.NTProfile), "h");
            var vParameter = Expression.Parameter(typeof(TSource), "v");

            var callXGetter = Expression.Invoke(xGetter, vParameter);
            var callYGetter = Expression.Invoke(yGetter, vParameter);
            var callWeight = Expression.Invoke(weight, vParameter);

            var fillMethod = typeof(ROOTNET.NTProfile).GetMethod("Fill", new[] { typeof(double), typeof(double), typeof(double) });
            var callFill = Expression.Call(hParameter, fillMethod, callXGetter, callYGetter, callWeight);

            var lambda = Expression.Lambda<Action<ROOTNET.NTProfile, TSource>>(callFill, hParameter, vParameter);
            var interfaceobj = new ROOTNET.NTProfile(plotName, plotTitle.ReplaceLatexStrings(), xNBins, xLowBin, xHighBin, yLowBin, yHighBin);
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
