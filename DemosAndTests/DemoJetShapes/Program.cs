///
/// Simple demo program that shows one how to do some very basic things
/// with LINQ and a local ntuple.
/// 

using System;
using System.IO;
using System.Linq;
using LINQToTTreeLib;
using ROOTTTreeDataModel;

namespace DemoJetShapes
{
    class Program
    {
        /// <summary>
        /// Simple demo to look at jets in a local root-tuple. This demo is based on the feature
        /// set that can be found in version 0.1. Basic cuts, Count, and histogram making.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            FileInfo rootFile = new FileInfo(@"..\..\..\output.root");
            if (!rootFile.Exists)
            {
                Console.WriteLine("Unable to find the input file '" + rootFile.FullName + "'");
                return;
            }

            var rf1 = Queryablebtag.Create(rootFile);
            //rf1.IgnoreQueryCache = true;
            //rf1.CleanupQuery = false;

            ///
            /// Count the # of events in the file
            /// 

            int count = rf1.Count();
            Console.WriteLine("The number of events in the ntuple, counted the hard way is {0}.", count);


            ///
            /// Next, count the number of jets in the file
            /// 

            var alljets = from e in rf1
                          from j in e.jets
                          select j;

            Console.WriteLine("The number of jets in the ntuple is {0}.", alljets.Count());

            var jetsPerEvent = from e in rf1
                               select e.jets;
            var firstJetPerEvent = from jts in jetsPerEvent
                                   from j in jts.Take(1)
                                   select j;
            var secondJetPerEvent = from jts in jetsPerEvent
                                    from j in jts.Skip(1).Take(1)
                                    select j;

            ///
            /// Apply some very simple cuts
            /// 

            var ptCutJets = from j in alljets
                            where j.Pt() / 1000.0 > 30.0
                            select j;
            Console.WriteLine("The number of jets with a pT greater than 30 GeV is {0}", ptCutJets.Count());

            var etaCutJets = from j in alljets
                             where Math.Abs(j.Eta()) < 1.0
                             select j;
            Console.WriteLine("The number of jets with an eta less than 1.0 is {0}", etaCutJets.Count());

            ///
            /// Make some histos of all of these.
            /// 

            var outputFile = new ROOTNET.NTFile("result.root", "RECREATE");
            MakeGenericHistos(alljets, "alljets", outputFile);
            MakeGenericHistos(ptCutJets, "ptjets", outputFile);
            MakeGenericHistos(etaCutJets, "etajets", outputFile);

            MakeGenericHistos(firstJetPerEvent, "firstjet", outputFile);
            MakeGenericHistos(secondJetPerEvent, "secondjet", outputFile);

            var jetCount = jetsPerEvent.AggregateNoReturn(new ROOTNET.NTH1F("njets", "N_{J}", 10, 0.0, 10.0), (h, x) => h.Fill(x.Count()));
            jetCount.SetDirectory(outputFile);

            outputFile.Write();
            outputFile.Close();

        }

        /// <summary>
        /// Make pT and Eta histograms
        /// </summary>
        /// <param name="alljets"></param>
        /// <param name="p"></param>
        private static void MakeGenericHistos(IQueryable<ROOTNET.Interface.NBTagJet> alljets, string namePrefix, ROOTNET.Interface.NTDirectory dir)
        {
            var heta = alljets.AggregateNoReturn(new ROOTNET.NTH1F(namePrefix + "_eta", "\\Eta for " + namePrefix, 50, -5.0, 5.0), (h, x) => h.Fill(x.Eta()));
            heta.SetDirectory(dir);
            var hpt = alljets.AggregateNoReturn(new ROOTNET.NTH1F(namePrefix + "_pt", "p_{T} for " + namePrefix, 50, 0.0, 150.0), (h, x) => h.Fill(x.Pt() / 1000.0));
            hpt.SetDirectory(dir);
        }
    }
}
