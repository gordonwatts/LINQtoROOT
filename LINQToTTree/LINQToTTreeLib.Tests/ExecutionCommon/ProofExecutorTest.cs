using System;
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

        static Uri CreateProofRef (string dsName)
        {
            return new Uri(string.Format("proff://{0}/{1}", proofTestNode, dsName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestForBogusDS()
        {
            var targetr = new ProofExecutor();
            var env = CreateSimpleEnvironment();
            env.RootFiles = new[] { CreateProofRef("bogusdatasetname") };

            targetr.Environment = env;
            targetr.Execute(null, null, null);
        }

        private ExecutionEnvironment CreateSimpleEnvironment()
        {
            var result = new ExecutionEnvironment();
            result.RootFiles = new[] { CreateProofRef("LINQTest") };

            return result;
        }

        [TestMethod]
        public void TestForSimpleRun()
        {
            // This should work all the way through.
            var targetr = new ProofExecutor();
            var env = CreateSimpleEnvironment();

            targetr.Environment = env;
            targetr.Execute(null, null, null);

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
