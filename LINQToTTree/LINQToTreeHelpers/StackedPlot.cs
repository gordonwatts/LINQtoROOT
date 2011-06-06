using System;
using System.Linq;

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
        /// <param name="histos"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTCanvas PlotStacked(this ROOTNET.Interface.NTH1F[] histos, string canvasName, string canvasTitle,
            bool logy = false,
            bool normalize = false,
            bool sparifyTitles = true,
            bool colorize = true)
        {
            if (histos == null || histos.Length == 0)
                return null;

            ///
            /// Always build a clone... because that way if the histogram is modified after we look at it, the plot will be what
            /// the user intended.
            /// 

            var hToPlot = (from h in histos select h.Clone(string.Format("{0}{1}", h.Name, canvasName)) as ROOTNET.Interface.NTH1F).ToArray();
            foreach (var h in hToPlot)
            {
                h.SetDirectory(null);
            }

            ///
            /// If we have to normalize first, we need to normalize first!
            /// 

            if (normalize)
            {
                hToPlot = (from h in hToPlot
                           select h.Normalize()).ToArray();
            }

            ///
            /// Reset the colors on these guys
            /// 

            if (colorize)
            {
                var cloop = new ColorLoop();
                foreach (var h in hToPlot)
                {
                    h.LineColor = cloop.NextColor();
                }
            }

            ///
            /// Remove common words from the titles.
            /// 

            if (sparifyTitles && hToPlot.Length > 1)
            {
                var splitTitles = from h in hToPlot
                                  select h.Title.Split();

                var wordList = from index in Enumerable.Range(0, splitTitles.Select(ar => ar.Count()).Min())
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

            ///
            /// Use the nice ROOT utility THStack to make the plot. Once we do this, the plot is now owned by the TCanvas.
            /// 

            var stack = new ROOTNET.NTHStack(canvasName + "Stack", canvasTitle);
            foreach (var h in hToPlot)
            {
                stack.Add(h);
                h.SetNull();
            }

            ///
            /// Now do the plotting. Use the THStack to get all the axis stuff correct.
            /// If we are plotting a log plot, then make sure to set that first before
            /// calling it as it will use that information during its painting.
            /// 

            var result = new ROOTNET.NTCanvas(canvasName, canvasTitle);
            result.FillColor = ROOTNET.NTStyle.gStyle.FrameFillColor; // This is not a sticky setting!
            if (logy)
                result.Logy = 1;
            stack.Draw("nostack");

            ///
            /// And a legend!
            /// 

            result.BuildLegend();

            ///
            /// Return the canvas so it can be saved to the file (or whatever).
            /// 

            return result;
        }

        /// <summary>
        /// Reformat the name.
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        private static ROOTNET.Interface.NTH1F AppendName(ROOTNET.Interface.NTH1F h, string format)
        {
            h.Name = string.Format(format, h.Name);
            return h;
        }

        /// <summary>
        /// Normalize this histo and return it.
        /// </summary>
        /// <param name="histo"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1F Normalize(this ROOTNET.Interface.NTH1F histo, double toArea = 1.0)
        {
            histo.Scale(toArea / histo.Integral());
            return histo;
        }
    }
}
