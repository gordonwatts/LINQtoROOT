
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
        /// to look at the GenerateArrayNtupleXMLSpec for the generation of the items.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var f = new FileInfo(@"..\..\..\btag-slim.root");
            if (!f.Exists)
            {
                Console.WriteLine("could not find btag input files: " + f.FullName);
                return;
            }

            var evts = ROOTTTreeDataModel.Queryablevtuple.Create(f);

            var trackPt = from e in evts
                          from t in e.tracks
                          select t.pt / 1000.0;

            var output = new ROOTNET.NTFile("output.root", "RECREATE");
            output.Add(trackPt.Plot("trackpt", "Track p_{T}", 100, 0.0, 200.0));

            output.Write();
            output.Clone();

        }
    }
}
