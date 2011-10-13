
using System;
using System.Linq;
using System.Linq.Expressions;
using LINQToTreeHelpers.FutureUtils;
using LinqToTTreeInterfacesLib;

namespace LINQToTreeHelpers
{
    /// <summary>
    /// Utilities (extention methods, static methods, etc.) to make plotting more
    /// uniform. Builds on the FuturePlot type of routines.
    /// </summary>
    /// <remarks>
    /// Use the MakeXXX methods to create a plot stub. This allows you to define
    /// </remarks>
    public static class PlottingUtils
    {
        /// <summary>
        /// Base interface for a plotter of a stream of object T. If you have a new kind of plot, and want to
        /// use the same infrastrucutre, implement this interface.
        /// </summary>
        public interface IPlotSpec<T>
        {
            /// <summary>
            /// Returns the name, can be a string.Format type string.
            /// </summary>
            string NameFormat { get; }

            /// <summary>
            /// Returns a title for the plot, can be a string.Format type string.
            /// </summary>
            string TitleFormat { get; }

            /// <summary>
            /// Returns a filter function. This can be used to do filtering just before
            /// the plot. Return null (or a function that returns true) if that is what
            /// you want. Must be able to translate into a LINQ query!
            /// </summary>
            Expression<Func<T, bool>> Filter { get; }

            /// <summary>
            /// Return a future value for the plot, with a full name and title as specified, from
            /// a sequence of events. Don't call this yourself, rather use the plot methods below
            /// to do the work.
            /// </summary>
            /// <param name="nameString">Fully formatted and replaced string to represent the plot name</param>
            /// <param name="titleString">Fully formatted and replaced string to represent the plot title</param>
            /// <param name="goodEvents">The LINQToTTree sequence of events to plot (prefiltered)</param>
            /// <returns>A future value representing the plot</returns>
            IFutureValue<ROOTNET.Interface.NTH1> MakeFuturePlot(string nameString, string titleString, IQueryable<T> goodEvents);

            /// <summary>
            /// Return an immediate plot. Really, not very useful for most people.
            /// </summary>
            /// <param name="nameString"></param>
            /// <param name="titleString"></param>
            /// <param name="goodEvents"></param>
            /// <returns></returns>
            ROOTNET.Interface.NTH1 MakePlot(string nameString, string titleString, IQueryable<T> goodEvents);

            /// <summary>
            /// Convert this plot to another plot that consumes a different sequence. In addtion, allow
            /// an argument to be appended to the first argument passed in.
            /// </summary>
            /// <remarks>
            /// This is useful if you have a plot of pT, and you want to convert it to make a plot of pT's of a
            /// jet object. Use the converter argument to convert from jet objects to double's.
            /// 
            /// The extra argument text can be used in the following way. If your "NameFOrmat" is "{0}pT" and you pass
            /// in "jet" for the argument, then the new plot should have a NameFormat of "{0}jetpT".
            /// 
            /// There is some boiler plate when implementing this method. See library source code and start from there to
            /// get it "right".
            /// </remarks>
            /// <typeparam name="U">The type of the sequence that the new plot should be run over</typeparam>
            /// <param name="converter">An expression that converts from the type U to type T</param>
            /// <param name="extraArgText">This will be pre-pended to the first argument in each argument in the name and title</param>
            /// <returns></returns>
            IPlotSpec<U> FromData<U>(Expression<Func<U, T>> converter, string extraArgText);
        }

        /// <summary>
        /// The specification for a 1D plot (TH1F).
        /// </summary>
        /// <remarks>
        /// Use the MakePlot method to actually create one of these objects.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        public class PlotSpec1D<T> : IPlotSpec<T>
        {
            /// <summary>
            /// Number of bins.
            /// </summary>
            public int nbins { get; set; }

            /// <summary>
            /// X axis minimum.
            /// </summary>
            public double xmin { get; set; }

            /// <summary>
            /// X axis maximum
            /// </summary>
            public double xmax { get; set; }

            /// <summary>
            /// Given a sequence of type T, return the double that we can actually feed
            /// to the plot.
            /// </summary>
            public Expression<Func<T, double>> getter;

            /// <summary>
            /// Only events in the sequence of T that pass this filter will be plotted.
            /// Null if everything should be plotted.
            /// </summary>
            public Expression<Func<T, bool>> Filter { get; set; }

            /// <summary>
            /// The format for the plot name string.
            /// </summary>
            public string NameFormat { get; set; }

            /// <summary>
            /// Format for the plot title string.
            /// </summary>
            public string TitleFormat { get; set; }

            /// <summary>
            /// Return a future value for the plot. Use the Plot method below rather than this directly.
            /// </summary>
            /// <param name="nameString">Fully formatted name to use for the plot</param>
            /// <param name="titleString">Fully formatted title to use for the plot</param>
            /// <param name="goodEvents">The sequence of good (prefiltered) events to use for this plot</param>
            /// <returns></returns>
            public IFutureValue<ROOTNET.Interface.NTH1> MakeFuturePlot(string nameString, string titleString, IQueryable<T> goodEvents)
            {
                return goodEvents.FuturePlot(nameString, titleString, nbins, xmin, xmax, getter).ExtractValue(p => p as ROOTNET.Interface.NTH1);
            }

            /// <summary>
            /// Return a plot. Use the Plot method below rather than this directly.
            /// </summary>
            /// <param name="nameString">Fully formatted name to use for the plot</param>
            /// <param name="titleString">Fully formatted title to use for the plot</param>
            /// <param name="goodEvents">The sequence of good (prefiltered) events to use for this plot</param>
            /// <returns></returns>
            public ROOTNET.Interface.NTH1 MakePlot(string nameString, string titleString, IQueryable<T> goodEvents)
            {
                return goodEvents.Plot(nameString, titleString, nbins, xmin, xmax, getter);
            }
            
            /// <summary>
            /// Create a new 1D plot specification that instead of running over a sequence of type T
            /// will run over a sequence of type U.
            /// </summary>
            /// <typeparam name="U"></typeparam>
            /// <param name="selector"></param>
            /// <param name="extraArgText"></param>
            /// <returns></returns>
            public IPlotSpec<U> FromData<U>(Expression<Func<U, T>> selector, string extraArgText)
            {
                var result = new PlotSpec1D<U>()
                {
                    nbins = nbins,
                    xmax = xmax,
                    xmin = xmin,
                    Filter = null,
                    NameFormat = string.Format(NameFormat, extraArgText + "{0}"),
                    TitleFormat = string.Format(TitleFormat, extraArgText + " {0}")
                };

                result.getter = selector.C(getter);
                if (Filter != null)
                    result.Filter = selector.C(Filter);

                return result;
            }
        }

        /// <summary>
        /// The specification for a 2D plot.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class PlotSpec2D<T> : IPlotSpec<T>
        {
            /// <summary>
            /// Number of bins along the X axis
            /// </summary>
            public int nxbins { get; set; }

            /// <summary>
            /// The X axis minimum value
            /// </summary>
            public double xmin { get; set; }

            /// <summary>
            /// The X axis maximum value
            /// </summary>
            public double xmax { get; set; }

            /// <summary>
            /// Number of bins along the Y axis
            /// </summary>
            public int nybins { get; set; }

            /// <summary>
            /// The Y axis minimum value
            /// </summary>
            public double ymin { get; set; }

            /// <summary>
            /// The Y axis maximum value
            /// </summary>
            public double ymax { get; set; }

            /// <summary>
            /// Turn the sequence we are running over into double's that we feed to the plotter.
            /// </summary>
            public Expression<Func<T, double>> xgetter;

            /// <summary>
            /// Turn the sequence we are running over into double's that we feed to the plotter.
            /// </summary>
            public Expression<Func<T, double>> ygetter;

            /// <summary>
            /// Only events that pass this filter should be plotted.
            /// </summary>
            public Expression<Func<T, bool>> Filter { get; set; }

            /// <summary>
            /// The string.Format string that represents the name of the plot.
            /// </summary>
            public string NameFormat { get; set; }

            /// <summary>
            /// The string.Format string that represents the title of the plot.
            /// </summary>
            public string TitleFormat { get; set; }

            /// <summary>
            /// Return a FuturePlot that will turn into the final 2D plot.
            /// </summary>
            /// <param name="nameString">The formatted name of the plot</param>
            /// <param name="titleString">The formatted title of the plot</param>
            /// <param name="goodEvents">The sequence of items we should be plotting</param>
            /// <returns></returns>
            public IFutureValue<ROOTNET.Interface.NTH1> MakeFuturePlot(string nameString, string titleString, IQueryable<T> goodEvents)
            {
                return goodEvents.FuturePlot(nameString, titleString, nxbins, xmin, xmax, xgetter,
                    nybins, ymin, ymax, ygetter).ExtractValue(p => p as ROOTNET.Interface.NTH1);
            }

            /// <summary>
            /// Return a 2D plot.
            /// </summary>
            /// <param name="nameString">The formatted name of the plot</param>
            /// <param name="titleString">The formatted title of the plot</param>
            /// <param name="goodEvents">The sequence of items we should be plotting</param>
            /// <returns></returns>
            public ROOTNET.Interface.NTH1 MakePlot(string nameString, string titleString, IQueryable<T> goodEvents)
            {
                return goodEvents.Plot(nameString, titleString, nxbins, xmin, xmax, xgetter,
                    nybins, ymin, ymax, ygetter);
            }

            /// <summary>
            /// Create a new plot specification that runs over a sequence of type U rather than type T.
            /// </summary>
            /// <typeparam name="U">New sequence to run over</typeparam>
            /// <param name="converter">Converts from the new sequence, U, to the old one, T</param>
            /// <param name="argumentPrefix">Text to add to name and title</param>
            /// <returns></returns>
            public IPlotSpec<U> FromData<U>(Expression<Func<U, T>> converter, string argumentPrefix)
            {
                var result = new PlotSpec2D<U>()
                {
                    nxbins = nxbins,
                    xmax = xmax,
                    xmin = xmin,
                    nybins = nybins,
                    ymax = ymax,
                    ymin = ymin,
                    Filter = null,
                    NameFormat = string.Format(NameFormat, argumentPrefix + "{0}"),
                    TitleFormat = string.Format(TitleFormat, argumentPrefix + " {0}")
                };

                result.xgetter = converter.C(xgetter);
                result.ygetter = converter.C(ygetter);

                if (Filter != null)
                    result.Filter = converter.C(Filter);

                return result;
            }
        }

        /// <summary>
        /// Creates a plotter specification for a 1D (TH1F) plotter.
        /// </summary>
        /// <remarks>
        /// The # of bins, xmin, and xmax are passed directly to root. If you want to have auto limits,
        /// for example, then you can just specify xmax > xmin.
        /// </remarks>
        /// <typeparam name="T">The type of the sequence that this plotter will be plotting over</typeparam>
        /// <param name="nXBins">Number of bins along the X axis</param>
        /// <param name="XMin">The minimum value of the x axis.</param>
        /// <param name="XMax">The maximum value of the x axis</param>
        /// <param name="xGetter">Return the value to plot from the sequence</param>
        /// <param name="nFormat">The format specification for the plot name string</param>
        /// <param name="tFormat">The format specification for the plot title string</param>
        /// <param name="filter">A filter that will remove sequence items you don't want plotted</param>
        /// <returns></returns>
        public static IPlotSpec<T> MakePlotterSpec<T>(int nXBins, double XMin, double XMax, Expression<Func<T, double>> xGetter,
            string nFormat = null, string tFormat = null, Expression<Func<T, bool>> filter = null)
        {
            return new PlotSpec1D<T>() { nbins = nXBins, xmin = XMin, xmax = XMax, getter = xGetter, NameFormat = nFormat, TitleFormat = tFormat, Filter = filter };
        }

        /// <summary>
        /// Creates a plotter specification for a 2D (TH2F) plotter.
        /// </summary>
        /// <remarks>
        /// The # of bins, xmin, and xmax are passed directly to root. If you want to have auto limits,
        /// for example, then you can just specify xmax > xmin.
        /// </remarks>
        /// <typeparam name="T">THe type of the sequence that this plotter will be plotting over</typeparam>
        /// <param name="nXBins">Number of bins along the X axis</param>
        /// <param name="XMin">The minimum value of the x axis</param>
        /// <param name="XMax">The maximum value of the x axis</param>
        /// <param name="xGetter">Return the value to plot from the sequence for the X axis</param>
        /// <param name="nYBins">Number of bins along the Y axis</param>
        /// <param name="YMin">The minimum value of the y axis</param>
        /// <param name="YMax">The maximum value of the y axis</param>
        /// <param name="yGetter">Return the value to plot from the sequence for the Y axis</param>
        /// <param name="nameFormat">The format specification for the plot name string</param>
        /// <param name="titleFormat">The format specification for the title string</param>
        /// <param name="filter">A filter that will remove sequence items you don't want plotted</param>
        /// <returns></returns>
        public static IPlotSpec<T> MakePlotterSpec<T>(int nXBins, double XMin, double XMax, Expression<Func<T, double>> xGetter,
            int nYBins, double YMin, double YMax, Expression<Func<T, double>> yGetter,
            string nameFormat = null, string titleFormat = null, Expression<Func<T, bool>> filter = null)
        {
            return new PlotSpec2D<T>() { nxbins = nXBins, xmin = XMin, xmax = XMax, xgetter = xGetter, nybins = nYBins, ymin = YMin, ymax = YMax, ygetter = yGetter, NameFormat = nameFormat, TitleFormat = titleFormat, Filter = filter };
        }

        /// <summary>
        /// Use a plot specification to generate a plot from the given sequence.
        /// </summary>
        /// <remarks>
        /// This is an extension method so you can use it easily on the sequence.
        /// </remarks>
        /// <typeparam name="T">The type of sequence that the plotter specification runs over</typeparam>
        /// <param name="source">The sequence to plot over</param>
        /// <param name="plotSpecification">The plot specification to guide the creation of the plot</param>
        /// <param name="nameAndTitleFormatArgs">Arguments to be passed to format the name and title of the plot</param>
        /// <returns></returns>
        public static IFutureValue<ROOTNET.Interface.NTH1> FuturePlot<T>(this IQueryable<T> source, IPlotSpec<T> plotSpecification, params string[] nameAndTitleFormatArgs)
        {
            return FuturePlot(source, plotSpecification.NameFormat, plotSpecification.TitleFormat, plotSpecification, nameAndTitleFormatArgs);
        }

        /// <summary>
        /// Use a plot specification to generate a plot from the given sequence. Use this version if you wish
        /// to have a custom name or title that will override the values of name and title normally associated
        /// with this plot specification.
        /// </summary>
        /// <typeparam name="T">The type of sequence that the plotter specification runs over</typeparam>
        /// <param name="source">The sequence to plot over</param>
        /// <param name="plotSpecification">The plot specification to guide the creation of the plot</param>
        /// <param name="nameAndTitleFormatArgs">Arguments to be passed to format the name and title of the plot</param>
        /// <param name="name">The plot name string to be used. Can contain string.Format specifiers</param>
        /// <param name="title">The plot title string to be used. Can contain string.Format specifiers</param>
        /// <returns></returns>
        public static IFutureValue<ROOTNET.Interface.NTH1> FuturePlot<T>(this IQueryable<T> source, string name, string title, IPlotSpec<T> plotSpecification, params string[] nameAndTitleFormatArgs)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Name is null");
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("Title is null");

            var titleString = string.Format(title, nameAndTitleFormatArgs);
            var nameString = string.Format(name, nameAndTitleFormatArgs).FixupForROOTName();

            var goodEvents = source;
            if (plotSpecification.Filter != null)
                goodEvents = source.Where(plotSpecification.Filter);

            return plotSpecification.MakeFuturePlot(nameString, titleString, goodEvents);
        }

        /// <summary>
        /// Use a plot specification to generate a plot from the given sequence.
        /// </summary>
        /// <remarks>
        /// This is an extension method so you can use it easily on the sequence.
        /// </remarks>
        /// <typeparam name="T">The type of sequence that the plotter specification runs over</typeparam>
        /// <param name="source">The sequence to plot over</param>
        /// <param name="plotSpecification">The plot specification to guide the creation of the plot</param>
        /// <param name="nameAndTitleFormatArgs">Arguments to be passed to format the name and title of the plot</param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 Plot<T>(this IQueryable<T> source, IPlotSpec<T> plotSpecification, params string[] nameAndTitleFormatArgs)
        {
            return Plot(source, plotSpecification.NameFormat, plotSpecification.TitleFormat, plotSpecification, nameAndTitleFormatArgs);
        }

        /// <summary>
        /// Use a plot specification to generate a plot from the given sequence. Use this version if you wish
        /// to have a custom name or title that will override the values of name and title normally associated
        /// with this plot specification.
        /// </summary>
        /// <typeparam name="T">The type of sequence that the plotter specification runs over</typeparam>
        /// <param name="source">The sequence to plot over</param>
        /// <param name="plotSpecification">The plot specification to guide the creation of the plot</param>
        /// <param name="nameAndTitleFormatArgs">Arguments to be passed to format the name and title of the plot</param>
        /// <param name="name">The plot name string to be used. Can contain string.Format specifiers</param>
        /// <param name="title">The plot title string to be used. Can contain string.Format specifiers</param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 Plot<T>(this IQueryable<T> source, string name, string title, IPlotSpec<T> plotSpecification, params string[] nameAndTitleFormatArgs)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Name is null");
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("Title is null");

            var titleString = string.Format(title, nameAndTitleFormatArgs);
            var nameString = string.Format(name, nameAndTitleFormatArgs).FixupForROOTName();

            var goodEvents = source;
            if (plotSpecification.Filter != null)
                goodEvents = source.Where(plotSpecification.Filter);

            return plotSpecification.MakePlot(nameString, titleString, goodEvents);
        }
    }
}
