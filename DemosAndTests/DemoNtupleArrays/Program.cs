
using System;
using System.IO;
using System.Linq;

/// Includes things in the Helper object, which has some nice extension methods
using LINQToTTreeLib;

namespace DemoNtupleArrays
{
    class Program
    {
        /// <summary>
        /// This demo shows how to work with an ntuple that contains a large number of arrays that
        /// are actually linked together (that is. pt, eta, phi, one entry for each jet). You will need
        /// to look at the Genq
        /// erateArrayNtupleXMLSpec for the generation of the items.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var f = new FileInfo(@"..\..\..\hvsample.root");
            if (!f.Exists)
            {
                Console.WriteLine("could not find btag input files: " + f.FullName);
                return;
            }

            var evts = ROOTTTreeDataModel.QueryableCollectionTree.Create(f);

            ///
            /// Make a plot of pt of a track (a renamed and grouped variable). This is with the full
            /// select statement done out.
            ///

            var trackPt = from e in evts
                          from t in e.tracks
                          select t.pt / 1000.0;

            var output = new ROOTNET.NTFile("output.root", "RECREATE");
            output.Add(trackPt.Plot("trackpt", "Track p_{T}", 100, 0.0, 200.0));

            ///
            /// Create a stream of tracks so we can do more "interesting" things on it.
            /// 

            var allTracks = from e in evts
                            from t in e.tracks
                            select t;

            PlotTracks(output, allTracks, "all", "All");

            var centralTracks = from t in allTracks
                                where Math.Abs(t.eta) < 1.0
                                select t;
            PlotTracks(output, centralTracks, "central", "Central");

            output.Write();
            output.Clone();

        }

        /// <summary>
        /// Make some basic plots about tracks.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="allTracks"></param>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        private static void PlotTracks(ROOTNET.NTFile output, IQueryable<ROOTTTreeDataModel.CollectionTreetracks> tracks, string nameAdd, string titleAdd)
        {
            ///
            /// Dump out pt and eta and phi. Note that we are using the form of the Plot method that takes a lambda to select what we want to plot.
            /// 

            output.Add(tracks.Plot(nameAdd + "TracksPt", titleAdd + " Track p_{T}", 100, 0.0, 100.0, t => t.pt / 1000.0));
            output.Add(tracks.Plot(nameAdd + "TracksPtZoom", titleAdd + " Track p_{T}", 100, 0.0, 20.0, t => t.pt / 1000.0));
            output.Add(tracks.Plot(nameAdd + "TracksEta", titleAdd + " Track \\eta", 100, -3.0, 3.0, t => t.eta));
            output.Add(tracks.Plot(nameAdd + "TracksPhi", titleAdd + " Track \\phi", 100, -Math.PI, Math.PI, t => t.phi));
        }
    }
}
