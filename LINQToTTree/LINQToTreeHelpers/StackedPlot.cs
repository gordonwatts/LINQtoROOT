﻿using LINQToTTreeLib.Utils;
using System;
using System.Linq;
using System.Text;

namespace LINQToTreeHelpers
{
    /// <summary>
    /// Helper class to make a stacked plot of a sequence of histograms.
    /// </summary>
    public static class StackedPlot
    {
        /// <summary>
        /// Create a canvas that is a set of stacked plots.
        /// </summary>
        /// <remarks>
        /// Only TH1F plots are dealt with properly here. Everything else is ignored and no stacked plot will be emitted.
        /// </remarks>
        /// <param name="histos"></param>
        /// <param name="canvasName">Name given to the canvas</param>
        /// <param name="canvasTitle">Title that will be put at the top of the canvas</param>
        /// <param name="colorize">True if colors should be automattically assigned to the canvas.</param>
        /// <param name="logy">True if the y axis should be log scale</param>
        /// <param name="normalize">True if the histograms should be set to normal area (1) before being plotted</param>
        /// <param name="legendContainsOnlyUniqueTitleWords">If true, then common words in the  histogram titles are removed before they are used for the legend</param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTCanvas PlotStacked(this ROOTNET.Interface.NTH1[] histos, string canvasName, string canvasTitle,
            bool logy = false,
            bool normalize = false,
            bool legendContainsOnlyUniqueTitleWords = true,
            bool colorize = true)
        {
            if (histos == null || histos.Length == 0)
                return null;

            // Always build a clone... because that way if the histogram is modified after we look at it, the plot will be what
            // the user intended.
            using (ROOTLock.Lock())
            {
                var hToPlot = (from h in histos where (h as ROOTNET.Interface.NTH1) != null select h.Clone(string.Format("{0}{1}", h.Name, canvasName)) as ROOTNET.Interface.NTH1).ToArray();
                if (hToPlot.Length == 0)
                {
                    var msg = new StringBuilder();
                    msg.Append("Warning: Only able to build a stacked plot for TH1F type plots (");
                    foreach (var p in histos)
                    {
                        msg.AppendFormat(" {0}", p.Name);
                    }
                    msg.Append(")");
                    Console.WriteLine(msg.ToString());
                    return null;
                }

                foreach (var h in hToPlot)
                {
                    h.SetDirectory(null);
                }

                //
                // If we have to normalize first, we need to normalize first!
                // 

                if (normalize)
                {
                    hToPlot = (from h in hToPlot
                               select h.Normalize()).ToArray();
                }

                //
                // Reset the colors on these guys
                // 

                if (colorize)
                {
                    var cloop = new ColorLoop();
                    foreach (var h in hToPlot)
                    {
                        h.LineColor = cloop.NextColor();
                    }
                }

                //
                // Remove common words from the titles.
                // 

                if (legendContainsOnlyUniqueTitleWords && hToPlot.Length > 1)
                {
                    var splitTitles = from h in hToPlot
                                      select h.Title.Split();

                    var wordList = from index in Enumerable.Range(0, splitTitles.Select(ar => ar.Count()).Max())
                                   select (from titleWords in splitTitles select titleWords.Skip(index).FirstOrDefault()).ToArray();

                    var isTheSame = (from wl in wordList
                                     select (wl.All(tword => tword == wl.First()))).ToArray();

                    var fixedTitleStrings = from twords in splitTitles
                                            select (
                                            from h in twords.Zip(isTheSame, (tword, issame) => issame ? "" : tword)
                                            where !string.IsNullOrWhiteSpace(h)
                                            select h
                                            );

                    foreach (var histAndTitle in hToPlot.Zip(fixedTitleStrings, (h, strArr) => Tuple.Create(h, strArr)))
                    {
                        string title = string.Join(" ", histAndTitle.Item2);
                        histAndTitle.Item1.Title = title;
                    }
                }

                //
                // Grab the x and y axis titles from the first histogram
                //

                var xaxisTitle = hToPlot[0].Xaxis.Title;
                var yaxisTitle = hToPlot[0].Yaxis.Title;

                //
                // Use the nice ROOT utility THStack to make the plot. Once we do this, the plot is now owned by the TCanvas.
                // 

                var stack = new ROOTNET.NTHStack(canvasName + "Stack", canvasTitle.ReplaceLatexStrings());
                foreach (var h in hToPlot)
                {
                    stack.Add(h);
                    h.SetNull();
                }

                //
                // Now do the plotting. Use the THStack to get all the axis stuff correct.
                // If we are plotting a log plot, then make sure to set that first before
                // calling it as it will use that information during its painting.
                // 

                var result = new ROOTNET.NTCanvas(canvasName, canvasTitle.ReplaceLatexStrings())
                {
                    FillColor = ROOTNET.NTStyle.gStyle.FrameFillColor // This is not a sticky setting!
                };
                if (logy)
                    result.Logy = 1;
                stack.Draw("nostack");

                if (!string.IsNullOrWhiteSpace(xaxisTitle))
                    stack.Xaxis.Title = xaxisTitle;
                if (!string.IsNullOrWhiteSpace(yaxisTitle))
                    stack.Yaxis.Title = yaxisTitle;

                stack.Draw("nostack");

                //
                // The stack is now "attached" to the canvas. This means the canvas now owns it. So we
                // definately don't want the GC to delete it - so here we need to turn off the delete.
                // 

                stack.SetNull();

                //
                // And a legend!
                // 

                result.BuildLegend();

                //
                // Return the canvas so it can be saved to the file (or whatever).
                // 

                return result;
            }
        }

        /// <summary>
        /// Reformat the name.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="format">string.Format argument, arg {0} will be the old histogram name</param>
        /// <returns></returns>
        private static ROOTNET.Interface.NTH1 AppendName(ROOTNET.Interface.NTH1 h, string format)
        {
            h.Name = string.Format(format, h.Name);
            return h;
        }

        /// <summary>
        /// Normalize this histo and return it.
        /// </summary>
        /// <param name="histo"></param>
        /// <param name="toArea">The area the histogram should be noramlized to</param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 Normalize(this ROOTNET.Interface.NTH1 histo, double toArea = 1.0)
        {
            histo.Scale(toArea / histo.Integral());
            return histo;
        }
    }
}
