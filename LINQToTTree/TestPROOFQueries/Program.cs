
using System;
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
            FileInfo runner = new FileInfo("queryTestSimpleQuery.cxx");
            if (!runner.Exists)
            {
                Console.WriteLine("Not able to find query cxx file");
                return;
            }

            var targetr = new ProofExecutor();
            var env = CreateSimpleQueryEnvironment();

            targetr.Environment = env;
            var r = targetr.Execute(runner, new DirectoryInfo(System.Environment.CurrentDirectory), null);

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
            result.RootFiles = new[] { CreateProofRef("LINQTest") };

            return result;
        }

        /// <summary>
        /// Creates the query for a simple execution file.
        /// </summary>
        /// <returns></returns>
        static private ExecutionEnvironment CreateSimpleQueryEnvironment()
        {
            var result = CreateSimpleEnvironment();

            result.TreeName = "btagd3pd";

            return result;
        }
    }
}
