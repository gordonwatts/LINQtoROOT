
using System;
using System.Collections.Generic;
using System.IO;
using LINQToTTreeLib.ExecutionCommon;
namespace TestPROOFQueries
{
    class Program
    {
        /// <summary>
        /// This is used to test PROOF queires. We have it b/c many initial PROOF errors come back
        /// as text printed to the screen. And our test harness can't capture those messages, unfortunately.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            FileInfo runner = new FileInfo("query0.cxx");
            if (!runner.Exists)
            {
                Console.WriteLine("Not able to find query cxx file");
                return;
            }

            var targetr = new ProofExecutor();
            var env = CreateSimpleQueryEnvironment();


            //
            // Create the query directory
            //

            var qDir = new DirectoryInfo(".\\query");
            if (qDir.Exists)
            {
                Console.WriteLine("Deleting existing query directory!!");
                qDir.Delete(true);
            }

            qDir.Create();
            File.Copy("query0.cxx", ".\\query\\query0.cxx");
            File.Copy("ntuple_CollectionTree.h", ".\\query\\ntuple_CollectionTree.h");
            File.Copy("junk_macro_parsettree_CollectionTree.C", "query\\junk_macro_parsettree_CollectionTree.C");

            runner = new FileInfo("query\\query0.cxx");

            //
            // Now, the histos we are going to transfer over
            //

            var histos = new Dictionary<string, object>();
            histos["aNTH1F_4"] = new ROOTNET.NTH1F("nothing", "this is it", 100, 0.0, 100.0);
            histos["aNTH1F_8"] = new ROOTNET.NTH1F("nothing1", "this is it", 100, 0.0, 100.0);
            histos["aNTH1F_11"] = new ROOTNET.NTH1F("nothing2", "this is it", 100, 0.0, 100.0);
            histos["aNTH1F_19"] = new ROOTNET.NTH1F("nothing3", "this is it", 100, 0.0, 100.0);
            histos["aNTH1F_27"] = new ROOTNET.NTH1F("nothing4", "this is it", 100, 0.0, 100.0);
            histos["aNTH1F_31"] = new ROOTNET.NTH1F("nothing5", "this is it", 100, 0.0, 100.0);
            histos["aNTH1F_34"] = new ROOTNET.NTH1F("nothing6", "this is it", 100, 0.0, 100.0);
            histos["aNTH1F_42"] = new ROOTNET.NTH1F("nothing7", "this is it", 100, 0.0, 100.0);

            //
            // Finally, run it.
            //

            targetr.Environment = env;
            var r = targetr.Execute(runner, qDir, histos);

            qDir.Refresh();
            if (qDir.Exists)
            {
                Console.WriteLine("!!! Query directory still exists!");
            }

            if (r == null)
            {
                Console.WriteLine("Hey - nothing came back!");
                return;
            }

            Console.WriteLine("# of returned items: {0}", r.Count);
#if false
            Assert.AreEqual("aInt32_1", r.Keys.First(), "Key name incorrect");
            var o = r["aInt32_1"];
            Assert.IsInstanceOfType(o, typeof(ROOTNET.NTH1I), "return histo type");
            var h = o as ROOTNET.NTH1I;
            Assert.AreEqual(2000, (int)h.GetBinContent(1), "Answer from query");
#endif
        }

        const string proofTestNode = "tev11.phys.washington.edu";

        static Uri CreateProofRef(string dsName)
        {
            return new Uri(string.Format("proof://{0}/{1}", proofTestNode, dsName));
        }

        static private ExecutionEnvironment CreateSimpleEnvironment()
        {
            var result = new ExecutionEnvironment();
            result.RootFiles = new[] { CreateProofRef("HV_ggH_mH120_mVPI20") };

            return result;
        }

        /// <summary>
        /// Creates the query for a simple execution file.
        /// </summary>
        /// <returns></returns>
        static private ExecutionEnvironment CreateSimpleQueryEnvironment()
        {
            var result = CreateSimpleEnvironment();

            result.TreeName = "CollectionTree";
            result.CleanupQuery = true;

            return result;
        }
    }
}
