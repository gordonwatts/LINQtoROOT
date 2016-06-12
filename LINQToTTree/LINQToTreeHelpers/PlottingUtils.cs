
using LINQToTreeHelpers.FutureUtils;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ROOTNET.Interface;
using LINQToTTreeLib.Utils;

namespace LINQToTreeHelpers
{
    /// <summary>
    /// Utilities (extension methods, static methods, etc.) to make plotting more
    /// uniform. Builds on the FuturePlot type of routines.
    /// </summary>
    /// <remarks>
    /// Use the MakeXXX methods to create a plot stub. This allows you to define
    /// </remarks>
    public static class PlottingUtils
    {
        /// <summary>
        /// Base interface for a plotter of a stream of object T. If you have a new kind of plot, and want to
        /// use the same infrastructure, implement this interface.
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
            /// Get the weighting function. This is applied to each item before it is plotted, as the
            /// weight in the Fill call to a TH histogram.
            /// </summary>
            Expression<Func<T, double>> Weight { get; }

            /// <summary>
            /// Get the bin function. This applied to T will give you the bin number in the histogram.
            /// </summary>
            Expression<Func<T, ROOTNET.Interface.NTH1, int>> Bin { get; }

            /// <summary>
            /// Combine the plots to make a new plot. For example, combine two 1D plots.
            /// </summary>
            /// <param name="otherPlot"></param>
            /// <returns></returns>
            IPlotSpec<T> CombinePlotAxes(IPlotSpec<T> otherPlot);

            /// <summary>
            /// Return a future value for the plot, with a full name and title as specified, from
            /// a sequence of events. Don't call this yourself, rather use the plot methods below
            /// to do the work.
            /// </summary>
            /// <param name="nameString">Fully formatted and replaced string to represent the plot name</param>
            /// <param name="titleString">Fully formatted and replaced string to represent the plot title</param>
            /// <param name="goodEvents">The LINQToTTree sequence of events to plot (prefiltered)</param>
            /// <returns>A future value representing the plot</returns>
            IFutureValue<ROOTNET.Interface.NTH1> MakeFuturePlot(string nameString, string titleString, IQueryable<Tuple<T, double>> goodEvents);

            /// <summary>
            /// Return an immediate plot. Really, not very useful for most people.
            /// </summary>
            /// <param name="nameString"></param>
            /// <param name="titleString"></param>
            /// <param name="goodEvents">A list of the events along with any additional weights to apply</param>
            /// <returns></returns>
            ROOTNET.Interface.NTH1 MakePlot(string nameString, string titleString, IQueryable<Tuple<T, double>> goodEvents);

            /// <summary>
            /// Return a IPlotSpec of the same type, but with the weight function reset to 1.
            /// </summary>
            /// <returns></returns>
            IPlotSpec<T> ResetWeight();

            /// <summary>
            /// Make an exact copy of this plot spec. This is to keep in tune with the
            /// functional nature of the new world! This is a shallow copy.
            /// </summary>
            /// <returns></returns>
            IPlotSpec<T> Clone();
        }

        /// <summary>
        /// Deal with making a plot (of some sort) over a sequence of items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class PlotSpecSequence<T> : IPlotSpec<IEnumerable<T>>
        {
            /// <summary>
            /// The name format string for this plot.
            /// </summary>
            public string NameFormat { get; set; }

            /// <summary>
            /// The title format string for this plot
            /// </summary>
            public string TitleFormat { get; set; }

            /// <summary>
            /// The filter that is run for this plot.
            /// </summary>
            public Expression<Func<IEnumerable<T>, bool>> Filter { get; set; }

            /// <summary>
            /// The weighting function for this sequence.
            /// </summary>
            public Expression<Func<IEnumerable<T>, double>> Weight { get; set; }

            /// <summary>
            /// Make a future plot from the sequence.
            /// </summary>
            public IPlotSpec<T> Plotter { get; set; }

            /// <summary>
            /// Make a copy of this
            /// </summary>
            /// <returns></returns>
            public IPlotSpec<IEnumerable<T>> Clone()
            {
                return new PlotSpecSequence<T>()
                {
                    NameFormat = this.NameFormat,
                    TitleFormat = this.TitleFormat,
                    Filter = this.Filter,
                    Weight = this.Weight,
                    Plotter = this.Plotter
                };
            }

            /// <summary>
            /// Combine this plotter along two axes. We do the one below us, and then build this guy up.
            /// </summary>
            /// <param name="otherPlot"></param>
            /// <returns></returns>
            public IPlotSpec<IEnumerable<T>> CombinePlotAxes(IPlotSpec<IEnumerable<T>> otherPlot)
            {
                var oth = otherPlot as PlotSpecSequence<T>;
                if (oth == null)
                {
                    throw new InvalidOperationException($"Do not know how to combine axies with a PlotSpecSequence and {otherPlot.GetType().FullyQualifiedName()}.");
                }

                var newPlotter = Plotter.CombinePlotAxes(oth.Plotter);
                return new PlotSpecSequence<T>()
                {
                    NameFormat = this.NameFormat,
                    TitleFormat = this.TitleFormat,
                    Filter = this.Filter,
                    Weight = this.Weight,
                    Plotter = newPlotter
                };
            }

            /// <summary>
            /// Getting a single bin for a histogram that comes from a sequence doesn't
            /// make sense. So we are going to fail. Hard.
            /// </summary>
            public Expression<Func<IEnumerable<T>, NTH1, int>> Bin
            {
                get
                {
                    throw new InvalidOperationException("Don't know how to get the bin number for a sequence of items.");
                }
            }

            /// <summary>
            /// Make a future plot from the sequence of filtered events. We use the full blow plotter guy below
            /// in order to make sure that we are properly dealing with filters in the plot we are making here!
            /// </summary>
            /// <param name="nameString"></param>
            /// <param name="titleString"></param>
            /// <param name="goodEvents"></param>
            /// <returns></returns>
            public IFutureValue<ROOTNET.Interface.NTH1> MakeFuturePlot(string nameString, string titleString, IQueryable<Tuple<IEnumerable<T>, double>> goodEvents)
            {
                return goodEvents
                    .SelectMany(seq => seq.Item1.Select(e => new Tuple<T, double>(e, seq.Item2)))
                    .FuturePlot(nameString, titleString, Plotter);
            }

            /// <summary>
            /// Make a plot from the sequence. We will use the full blown plotter guy below in order
            /// to make sure that we are properly dealing with filters in the plot we are making here!
            /// </summary>
            /// <param name="nameString"></param>
            /// <param name="titleString"></param>
            /// <param name="goodEvents"></param>
            /// <returns></returns>
            public ROOTNET.Interface.NTH1 MakePlot(string nameString, string titleString, IQueryable<Tuple<IEnumerable<T>, double>> goodEvents)
            {
                return goodEvents
                    .SelectMany(seq => seq.Item1.Select(e => new Tuple<T, double>(e, seq.Item2)))
                    .Plot(nameString, titleString, Plotter);
            }

            /// <summary>
            /// Reset the weight function to 1 by default.
            /// </summary>
            /// <returns></returns>
            public IPlotSpec<IEnumerable<T>> ResetWeight()
            {
                var r = Clone() as PlotSpecSequence<T>;
                r.Weight = e => 1.0;
                return r;
            }
        }

        /// <summary>
        /// Plot spec class that will convert from type U to type T for plotting!
        /// </summary>
        /// <typeparam name="T">The old plotter sequence type</typeparam>
        /// <typeparam name="U">The new plotter sequence type</typeparam>
        private class PlotSpecConverter<T, U> : IPlotSpec<U>
        {
            public string NameFormat { get; set; }

            public string TitleFormat { get; set; }

            public Expression<Func<U, bool>> Filter { get; set; }

            public IPlotSpec<T> Plotter { get; set; }

            public Expression<Func<U, T>> Converter { get; set; }

            /// <summary>
            /// An additional weight function to add in.
            /// </summary>
            public Expression<Func<U, double>> Weight { get; set; }

            /// <summary>
            /// Make a clone of this.
            /// </summary>
            /// <returns></returns>
            public IPlotSpec<U> Clone()
            {
                return new PlotSpecConverter<T, U>()
                {
                    NameFormat = this.NameFormat,
                    TitleFormat = this.TitleFormat,
                    Filter = this.Filter,
                    Plotter = this.Plotter,
                    Converter = this.Converter,
                    Weight = this.Weight
                };
            }

            /// <summary>
            /// Given two plots, see if we can combine them.
            /// </summary>
            /// <param name="otherPlot"></param>
            /// <returns></returns>
            public IPlotSpec<U> CombinePlotAxes(IPlotSpec<U> otherPlot)
            {
                // See if the other one is a converter too. If not, then we have to give up.
                var oth = otherPlot as PlotSpecConverter<T, U>;
                if (oth == null)
                {
                    throw new InvalidOperationException($"Do not know how to combine a PlotSpecConverter with ${otherPlot.GetType().FullyQualifiedName()}.");
                }


                // Ok, combine the plotters, and then do the conversion.
                var newPlotter = Plotter.CombinePlotAxes(oth.Plotter);
                return new PlotSpecConverter<T, U>()
                {
                    NameFormat = this.NameFormat,
                    TitleFormat = this.TitleFormat,
                    Filter = this.Filter,
                    Plotter = newPlotter,
                    Converter = this.Converter,
                    Weight = this.Weight
                };
            }

            /// <summary>
            /// Make a future plot - this is mostly doing the conversion and passing it on down.
            /// </summary>
            /// <param name="nameString"></param>
            /// <param name="titleString"></param>
            /// <param name="goodEvents"></param>
            /// <returns></returns>
            public IFutureValue<ROOTNET.Interface.NTH1> MakeFuturePlot(string nameString, string titleString, IQueryable<Tuple<U, double>> goodEvents)
            {
                return ConvertEvents(goodEvents)
                    .FuturePlot(nameString, titleString, Plotter);
            }

            /// <summary>
            /// Convert the events and add in a weight.
            /// </summary>
            /// <param name="goodEvents"></param>
            /// <returns></returns>
            private IQueryable<Tuple<T, double>> ConvertEvents(IQueryable<Tuple<U, double>> goodEvents)
            {
                return goodEvents.Select(e => new Tuple<T, double>(Converter.Invoke(e.Item1), Weight.Invoke(e.Item1) * e.Item2));
            }

            public ROOTNET.Interface.NTH1 MakePlot(string nameString, string titleString, IQueryable<Tuple<U, double>> goodEvents)
            {
                return ConvertEvents(goodEvents).Plot(nameString, titleString, Plotter);
            }

            /// <summary>
            /// Return a version of this plotter with the weight reset to 1.0
            /// </summary>
            /// <returns></returns>
            public IPlotSpec<U> ResetWeight()
            {
                var r = Clone() as PlotSpecConverter<T, U>;
                r.Weight = e => 1.0;
                return r;
            }

            /// <summary>
            /// Get the bin for this guy by running the object through the converter.
            /// </summary>
            public Expression<Func<U, NTH1, int>> Bin
            {
                get
                {
                    Expression<Func<U, NTH1, int>> r = (o, h) => Plotter.Bin.Invoke(Converter.Invoke(o), h);
                    return r;

                }
            }
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
            /// Apply the weight to the Fill for the histogram.
            /// </summary>
            public Expression<Func<T, double>> Weight { get; set; }

            /// <summary>
            /// The format for the plot name string.
            /// </summary>
            public string NameFormat { get; set; }

            /// <summary>
            /// Format for the plot title string.
            /// </summary>
            public string TitleFormat { get; set; }

            /// <summary>
            /// Make a shallow clone of this PlotSpec1D object.
            /// </summary>
            /// <returns></returns>
            public IPlotSpec<T> Clone()
            {
                return new PlotSpec1D<T>()
                {
                    Filter = this.Filter,
                    NameFormat = this.NameFormat,
                    nbins = this.nbins,
                    Weight = this.Weight,
                    TitleFormat = this.TitleFormat,
                    xmax = this.xmax,
                    xmin = this.xmin,
                    getter = this.getter
                };
            }

            /// <summary>
            /// Combine another plot into this one to make a new one.
            /// </summary>
            /// <param name="otherPlot"></param>
            /// <returns></returns>
            public IPlotSpec<T> CombinePlotAxes(IPlotSpec<T> otherPlot)
            {
                var other1D = otherPlot as PlotSpec1D<T>;
                if (other1D == null)
                {
                    throw new InvalidOperationException($"Do not know how to combine a 1D plot and a {otherPlot.GetType().FullyQualifiedName()}.");
                }

                // Simple builder
                return MakePlotterSpec(nbins, xmin, xmax, getter,
                    other1D.nbins, other1D.xmin, other1D.xmax, other1D.getter,
                    nameFormat: NameFormat, titleFormat: TitleFormat,
                    filter: Filter,
                    weight: Weight);
            }
            /// <summary>
            /// Return an expression that will extract the bin
            /// </summary>
            public Expression<Func<T, NTH1, int>> Bin
            {
                get
                {
                    Expression<Func<T, NTH1, int>> r = (o, h) => h.FindBin(getter.Invoke(o));
                    return r;
                }
            }

            /// <summary>
            /// Return a future value for the plot over the sequence of good events.
            /// </summary>
            /// <remarks>
            /// Assume that our events have already been filtered (see below).
            /// </remarks>
            /// <param name="nameString">Fully formatted name to use for the plot</param>
            /// <param name="titleString">Fully formatted title to use for the plot</param>
            /// <param name="goodEvents">The sequence of good (prefiltered) events to use for this plot</param>
            /// <returns></returns>
            public IFutureValue<ROOTNET.Interface.NTH1> MakeFuturePlot(string nameString, string titleString, IQueryable<Tuple<T, double>> goodEvents)
            {
                return goodEvents.Select(e => new Tuple<T, double>(e.Item1, e.Item2 * Weight.Invoke(e.Item1)))
                    .FuturePlot(nameString, titleString, nbins, xmin, xmax, xValue: e => getter.Invoke(e.Item1), weight: e => Weight.Invoke(e.Item1) * e.Item2)
                    .ExtractValue(p => p as ROOTNET.Interface.NTH1);
            }

            /// <summary>
            /// Return a plot. Use the Plot method below rather than this directly.
            /// </summary>
            /// <remarks>
            /// Assume that our events have already been filtered (see below).
            /// </remarks>
            /// <param name="nameString">Fully formatted name to use for the plot</param>
            /// <param name="titleString">Fully formatted title to use for the plot</param>
            /// <param name="goodEvents">The sequence of good (prefiltered) events to use for this plot</param>
            /// <returns></returns>
            public ROOTNET.Interface.NTH1 MakePlot(string nameString, string titleString, IQueryable<Tuple<T, double>> goodEvents)
            {
                return goodEvents
                    .Plot(nameString, titleString, nbins, xmin, xmax, e => getter.Invoke(e.Item1), weight: e => Weight.Invoke(e.Item1) * e.Item2);
            }

            /// <summary>
            /// return a version of this with the weight reset to 1.
            /// </summary>
            /// <returns></returns>
            public IPlotSpec<T> ResetWeight()
            {
                var r = Clone() as PlotSpec1D<T>;
                r.Weight = e => 1.0;
                return r;
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
            /// Weight function that is applied to each Fill
            /// </summary>
            public Expression<Func<T, double>> Weight { get; set; }

            /// <summary>
            /// The string.Format string that represents the name of the plot.
            /// </summary>
            public string NameFormat { get; set; }

            /// <summary>
            /// The string.Format string that represents the title of the plot.
            /// </summary>
            public string TitleFormat { get; set; }

            /// <summary>
            /// Return the bin number for a 2D histogram.
            /// </summary>
            public Expression<Func<T, NTH1, int>> Bin
            {
                get
                {
                    Expression<Func<T, NTH1, int>> r = (o, h) => h.FindBin(xgetter.Invoke(o), ygetter.Invoke(o));
                    return r;
                }
            }

            /// <summary>
            /// Return a FuturePlot that will turn into the final 2D plot.
            /// </summary>
            /// <param name="nameString">The formatted name of the plot</param>
            /// <param name="titleString">The formatted title of the plot</param>
            /// <param name="goodEvents">The sequence of items we should be plotting</param>
            /// <returns></returns>
            public IFutureValue<ROOTNET.Interface.NTH1> MakeFuturePlot(string nameString, string titleString, IQueryable<Tuple<T, double>> goodEvents)
            {
                return goodEvents
                    .FuturePlot(nameString, titleString, nxbins, xmin, xmax, e => xgetter.Invoke(e.Item1),
                        nybins, ymin, ymax, e => ygetter.Invoke(e.Item1), weight: e => Weight.Invoke(e.Item1) * e.Item2)
                    .ExtractValue(p => p as ROOTNET.Interface.NTH1);
            }

            /// <summary>
            /// Return a 2D plot.
            /// </summary>
            /// <param name="nameString">The formatted name of the plot</param>
            /// <param name="titleString">The formatted title of the plot</param>
            /// <param name="goodEvents">The sequence of items we should be plotting</param>
            /// <returns></returns>
            public ROOTNET.Interface.NTH1 MakePlot(string nameString, string titleString, IQueryable<Tuple<T, double>> goodEvents)
            {
                return goodEvents
                    .Plot(nameString, titleString, nxbins, xmin, xmax, e => xgetter.Invoke(e.Item1), nybins, ymin, ymax, e => ygetter.Invoke(e.Item1), weight: e => Weight.Invoke(e.Item1) * e.Item2);
            }

            /// <summary>
            /// Return a version of this with weight set to 1.0
            /// </summary>
            /// <returns></returns>
            public IPlotSpec<T> ResetWeight()
            {
                var r = Clone() as PlotSpec2D<T>;
                r.Weight = e => 1.0;
                return r;
            }

            /// <summary>
            /// Clone an exact copy of this guy
            /// </summary>
            /// <returns></returns>
            public IPlotSpec<T> Clone()
            {
                return new PlotSpec2D<T>()
                {
                    Filter = this.Filter,
                    NameFormat = this.NameFormat,
                    nxbins = this.nxbins,
                    nybins = this.nybins,
                    Weight = this.Weight,
                    TitleFormat = this.TitleFormat,
                    xgetter = this.xgetter,
                    xmax = this.xmax,
                    xmin = this.xmin,
                    ygetter = this.ygetter,
                    ymax = this.ymax,
                    ymin = this.ymin
                };
            }

            /// <summary>
            /// Since we don't support anything more than a 2D plot at the moment, we fail here.
            /// </summary>
            /// <param name="otherPlot"></param>
            /// <returns></returns>
            public IPlotSpec<T> CombinePlotAxes(IPlotSpec<T> otherPlot)
            {
                throw new InvalidOperationException("Do not know how to build a new plot starting from a 2D plot!");
            }
        }

        /// <summary>
        /// Create a plot spec that runs on a sequence of a sequence objects like T from a plotter than runs on a sequence of T
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
        /// </remarks>        /// <typeparam name="T">The type of the plotter you wish to extend to run on a sequence</typeparam>
        /// <typeparam name="U">The type you wish the new plotter to run on</typeparam>
        /// <param name="source">The plot spec to be extended to run on a sequence</param>
        /// <param name="argumentPrefix">Added to the argument, null by default (means nothing added).</param>
        /// <param name="converter">Convert from an object of type U to a sequences of objects of type T</param>
        /// <param name="filter">Only let through objects of type U that satisfy this filter</param>
        /// <returns>Plot spec able to run on a sequence</returns>
        public static IPlotSpec<U> FromManyOfType<T, U>(this IPlotSpec<T> source, Expression<Func<U, IEnumerable<T>>> converter, string argumentPrefix = null, Expression<Func<U, bool>> filter = null)
        {
            string newNameFormat = source.NameFormat;
            string newTitleFormat = source.TitleFormat;
            if (argumentPrefix != null)
            {
                var nf = string.IsNullOrWhiteSpace(source.NameFormat) ? "{0}" : source.NameFormat;
                var tf = string.IsNullOrWhiteSpace(source.TitleFormat) ? "{0}" : source.TitleFormat;
                newNameFormat = string.Format(nf, argumentPrefix + "{0}");
                newTitleFormat = string.Format(tf, argumentPrefix + " {0}");
            }

            // First, create the plotter that can deal with the sequence it self.
            var result = new PlotSpecSequence<T>()
            {
                NameFormat = newNameFormat,
                TitleFormat = newTitleFormat,
                Plotter = source,
            };

            // It would be nice to avoid this line if U and IEnumerable<T> were the same, however
            // the compiler doesn't know ahead of time, so we can't (unless we code up
            // a second method to do that). It should translate to a null operation in C++, however!

            var convertedResult = result.FromType(converter, argumentPrefix, filter);

            return convertedResult;
        }

        /// <summary>
        /// Create a plotter that will convert from one type to another type.
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
        /// </remarks>        /// <typeparam name="U">The new plotter sequence type</typeparam>
        /// <typeparam name="T">The old plotter sequence type</typeparam>
        /// <param name="source">The original plotter specification</param>
        /// <param name="converter">Convert from objects of type U to objects of type T</param>
        /// <param name="argumentPrefix">Prefix to add to the name and title arguments</param>
        /// <param name="filter">Only let through objects of type U that satisfy this filter</param>
        /// <param name="weight">Additional weight to be applied multiplicatively to any other weights already set</param>
        /// <returns></returns>
        public static IPlotSpec<U> FromType<T, U>(this IPlotSpec<T> source, Expression<Func<U, T>> converter, string argumentPrefix = "",
            Expression<Func<U, bool>> filter = null, Expression<Func<U, double>> weight = null)
        {
            string newNameFormat = string.Format(source.NameFormat, argumentPrefix + "{0}");
            string newTitleFormat = string.Format(source.TitleFormat, argumentPrefix + " {0}");

            var result = new PlotSpecConverter<T, U>
            {
                NameFormat = newNameFormat,
                TitleFormat = newTitleFormat,
                Plotter = source,
                Converter = converter,
                Filter = filter,
                Weight = weight == null ? e => 1.0 : weight
            };

            return result;
        }

        /// <summary>
        /// Modify a plot by adding a weight or a filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="argumentPrefix"></param>
        /// <param name="filter"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static IPlotSpec<T> FromType<T>(this IPlotSpec<T> source, string argumentPrefix = "",
            Expression<Func<T, bool>> filter = null, Expression<Func<T, double>> weight = null)
        {
            string newNameFormat = string.Format(source.NameFormat, argumentPrefix + "{0}");
            string newTitleFormat = string.Format(source.TitleFormat, argumentPrefix + " {0}");

            var result = new PlotSpecConverter<T, T>
            {
                NameFormat = newNameFormat,
                TitleFormat = newTitleFormat,
                Plotter = source,
                Converter = e => e,
                Filter = filter,
                Weight = weight == null ? e => 1.0 : weight
            };

            return result;
        }

        /// <summary>
        /// Creates a plotter specification for a 1D (TH1F) plotter for a double
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
        /// <param name="weight">The weight to be applied during plotting</param>
        /// <returns></returns>
        public static IPlotSpec<T> MakePlotterSpec<T>(int nXBins, double XMin, double XMax, Expression<Func<T, double>> xGetter,
            string nFormat = "", string tFormat = "", Expression<Func<T, bool>> filter = null,
            Expression<Func<T, double>> weight = null)
        {
            return new PlotSpec1D<T>()
            {
                nbins = nXBins,
                xmin = XMin,
                xmax = XMax,
                getter = xGetter,
                NameFormat = nFormat,
                TitleFormat = tFormat,
                Filter = filter,
                Weight = weight == null ? e => 1.0 : weight
            };
        }

        /// <summary>
        /// Creates a plotter specification for a 1D (TH1F) plotter for a sequence of doubles
        /// </summary>
        /// <remarks>
        /// The # of bins, xmin, and xmax are passed directly to root. If you want to have auto limits,
        /// for example, then you can just specify xmax > xmin.
        /// </remarks>
        /// <typeparam name="T">The type of the sequence that this plotter will be plotting over</typeparam>
        /// <param name="nXBins">Number of bins along the X axis</param>
        /// <param name="XMin">The minimum value of the x axis.</param>
        /// <param name="XMax">The maximum value of the x axis</param>
        /// <param name="xGetter">Return the sequence of values to plot from the sequence</param>
        /// <param name="nFormat">The format specification for the plot name string</param>
        /// <param name="tFormat">The format specification for the plot title string</param>
        /// <param name="filter">A filter that will remove sequence items you don't want plotted</param>
        /// <returns></returns>
        public static IPlotSpec<T> MakePlotterSpec<T>(int nXBins, double XMin, double XMax, Expression<Func<T, IEnumerable<double>>> xGetter,
            string nFormat = null, string tFormat = null, Expression<Func<T, bool>> filter = null)
        {
            var basePlotter = new PlotSpec1D<double>() { nbins = nXBins, xmin = XMin, xmax = XMax, getter = x => x, NameFormat = nFormat, TitleFormat = tFormat, Weight = e => 1.0 };
            return basePlotter.FromManyOfType(xGetter, "", filter);
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
        /// <param name="weight">Weight function to be applied to each item filled</param>
        /// <returns></returns>
        public static IPlotSpec<T> MakePlotterSpec<T>(int nXBins, double XMin, double XMax, Expression<Func<T, double>> xGetter,
            int nYBins, double YMin, double YMax, Expression<Func<T, double>> yGetter,
            string nameFormat = null, string titleFormat = null,
            Expression<Func<T, bool>> filter = null,
            Expression<Func<T, double>> weight = null)
        {
            return new PlotSpec2D<T>()
            {
                nxbins = nXBins,
                xmin = XMin,
                xmax = XMax,
                xgetter = xGetter,
                nybins = nYBins,
                ymin = YMin,
                ymax = YMax,
                ygetter = yGetter,
                NameFormat = nameFormat,
                TitleFormat = titleFormat,
                Filter = filter,
                Weight = weight == null ? e => 1.0 : weight
            };
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

            var titleString = Utils.FormatStringWithoutDoubleSpaces(title, nameAndTitleFormatArgs);
            var nameString = string.Format(name, nameAndTitleFormatArgs).FixupForROOTName();

            var goodEvents = source;
            if (plotSpecification.Filter != null)
                goodEvents = source.Where(plotSpecification.Filter);

            return plotSpecification.MakeFuturePlot(nameString, titleString, goodEvents.Select(e => new Tuple<T, double>(e, 1.0)));
        }

        /// <summary>
        /// Make a future for a plot that is a sequence of items along with partner weights.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceWithWeights"></param>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="plotSpecification"></param>
        /// <param name="nameAndTitleFormatArgs"></param>
        /// <returns></returns>
        public static IFutureValue<ROOTNET.Interface.NTH1> FuturePlot<T>(this IQueryable<Tuple<T, double>> sourceWithWeights, string name, string title, IPlotSpec<T> plotSpecification, params string[] nameAndTitleFormatArgs)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Name is null");
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("Title is null");

            var titleString = Utils.FormatStringWithoutDoubleSpaces(title, nameAndTitleFormatArgs);
            var nameString = string.Format(name, nameAndTitleFormatArgs).FixupForROOTName();

            var goodEvents = sourceWithWeights;
            if (plotSpecification.Filter != null)
                goodEvents = sourceWithWeights.Where(e => plotSpecification.Filter.Invoke(e.Item1));

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

            var titleString = Utils.FormatStringWithoutDoubleSpaces(title, nameAndTitleFormatArgs);
            var nameString = string.Format(name, nameAndTitleFormatArgs).FixupForROOTName();

            var goodEvents = source;
            if (plotSpecification.Filter != null)
                goodEvents = source.Where(plotSpecification.Filter);

            return plotSpecification.MakePlot(nameString, titleString, goodEvents.Select(e => new Tuple<T, double>(e, 1.0)));
        }

        /// <summary>
        /// Use a plot specification to generate a plot from a given sequence. THe sequence consists of the object to be plotted, plus an
        /// extra additional weight to be applied.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceWithWeights"></param>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="plotSpecification"></param>
        /// <param name="nameAndTitleFormatArgs"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 Plot<T>(this IQueryable<Tuple<T, double>> sourceWithWeights, string name, string title, IPlotSpec<T> plotSpecification, params string[] nameAndTitleFormatArgs)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Name is null");
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException("Title is null");

            var titleString = Utils.FormatStringWithoutDoubleSpaces(title, nameAndTitleFormatArgs);
            var nameString = string.Format(name, nameAndTitleFormatArgs).FixupForROOTName();

            var goodEvents = sourceWithWeights;
            if (plotSpecification.Filter != null)
                goodEvents = sourceWithWeights.Where(e => plotSpecification.Filter.Invoke(e.Item1));

            return plotSpecification.MakePlot(nameString, titleString, goodEvents);
        }
    }
}
