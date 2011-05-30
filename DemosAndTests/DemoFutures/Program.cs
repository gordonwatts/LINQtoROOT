
using System;
using System.IO;
using System.Linq;
using LINQToTTreeLib;

namespace DemoFutures
{
    class Program
    {
        /// <summary>
        /// This demo shows the use of Future's. This allows you to batch up a number of queries have have them
        /// all execute at once. There is, obviously, a huge speed advantage to this.
        /// This project also uses the VS build task to convert the .ntup and .ntupom into C# code. You'll have to
        /// look at the raw project file to see how this is done.
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
            var qu = ROOTLINQ.QueryableCollectionTree.Create(f);

            ///
            /// Create a bunch of things we want to know
            /// 

            var count = qu.FutureCount();
            var trackPts = qu.SelectMany(evt => evt.Tracks).Select(trk => trk.pt / 1000.0).FuturePlot("trackPt", "Track Pts", 100, 0.0, 20.0);
            var jetPts = qu.SelectMany(evt => evt.jet04).Select(j => j.Et / 1000.0).FuturePlot("jetET", "Jet E_{T}", 100, 0.0, 40.0);

            ///
            /// And now the output file which all this stuff will be saved in. The first ".Value" will cause
            /// everything requested so far to run all at once. THe other ".Value"'s shoudl execute without
            /// delay.
            /// 

            var output = ROOTNET.NTFile.Open("DemoFuturesOutput.root", "RECREATE");
            trackPts.Value.SaveToROOTDirectory(output);
            jetPts.Value.SaveToROOTDirectory(output);

            Console.WriteLine("There were {0} events we processed.", count.Value);

            output.Write();
            output.Close();
        }
    }
}
