using System;
using System.IO;
using System.Linq;
using LINQToTTreeLib.ExecutionCommon;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for TestProofExecutor and is intended
    ///to contain all TestProofExecutor Unit Tests
    ///</summary>
    [TestClass()]
    [PexClass(typeof(ProofExecutor))]
    public class TestProofExecutor
    {
        /// <summary>
        /// Machine we can use for testing.
        /// </summary>
        const string proofTestNode = "tev11.phys.washington.edu";

        static Uri CreateProofRef(string dsName)
        {
            return new Uri(string.Format("proof://{0}/{1}", proofTestNode, dsName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem("ExecutionCommon\\queryTestSimpleQuery.cxx")]
        public void TestForBogusDS()
        {
            var targetr = new ProofExecutor();
            var env = CreateSimpleEnvironment();
            env.RootFiles = new[] { CreateProofRef("bogusdatasetname") };

            targetr.Environment = env;
            FileInfo runner = new FileInfo("queryTestSimpleQuery.cxx");
            targetr.Execute(runner, null, null);
        }

        private ExecutionEnvironment CreateSimpleEnvironment()
        {
            var result = new ExecutionEnvironment();
            result.RootFiles = new[] { CreateProofRef("LINQTest") };

            return result;
        }

        /// <summary>
        /// Creates the query for a simple execution file.
        /// </summary>
        /// <returns></returns>
        private ExecutionEnvironment CreateSimpleQueryEnvironment()
        {
            var result = CreateSimpleEnvironment();

            result.TreeName = "CollectionTree";

            return result;
        }

        [TestMethod]
        [DeploymentItem("ExecutionCommon\\queryTestSimpleQuery.cxx")]
        public void TestSimpleQuery()
        {
            FileInfo runner = new FileInfo("queryTestSimpleQuery.cxx");
            Assert.IsTrue(runner.Exists, "Main C++ file missing");
            var targetr = new ProofExecutor();
            var env = CreateSimpleQueryEnvironment();

            targetr.Environment = env;
            var r = targetr.Execute(runner, null, null);

            Assert.IsNotNull(r, "nothing came back!");
            Assert.AreEqual(1, r.Count, "# of returned values from query");
            Assert.AreEqual("aInt32_1", r.Keys.First(), "Key name incorrect");
            var o = r["aInt32_1"];
            Assert.IsInstanceOfType(o, typeof(ROOTNET.NTH1I), "return histo type");
            var h = o as ROOTNET.NTH1I;
            Assert.AreEqual(2000, (int)h.GetBinContent(1), "Answer from query");
        }

        [PexMethod]
        public void TestForSaveExeEnvironment([PexAssumeNotNull]ExecutionEnvironment env)
        {
            var target = new ProofExecutor();
            target.Environment = env;

            Assert.Inconclusive();
        }
    }
}
