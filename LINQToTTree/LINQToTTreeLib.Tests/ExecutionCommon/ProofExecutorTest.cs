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

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestForBogusDS()
        {
            var targetr = new ProofExecutor();
            var env = CreateSimpleEnvironment();
            env.RootFiles = new[] { new Uri("proof://tev03.phys.washington.edu/bogusdatasetname") };

            targetr.Environment = env;
            targetr.Execute(null, null, null);
        }

        private ExecutionEnvironment CreateSimpleEnvironment()
        {
            var result = new ExecutionEnvironment();
            result.RootFiles = new[] { new Uri("proof://tev03.phys.washington.edu/LINQTest") };

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
