using System;
using System.IO;
using System.Linq;
using LINQToTTreeLib;
using LINQToTTreeLib.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NVelocity.App;

namespace LINQToTreeHelpers.Tests
{
    [TestClass]
    [DeploymentItem(@"Templates\TSelectorTemplate.cxx")]
    public class t_ROOTUtils
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;

            /// Get the path for the other nutple guy correct! Since Pex and tests run from different places in the directory structure we have to
            /// do some work to find the top leve!

            var currentDir = new DirectoryInfo(Environment.CurrentDirectory);
            while (currentDir.FindAllFiles("LINQToTTree.sln").Count() == 0)
            {
                currentDir = currentDir.Parent;
            }
            var projectDir = currentDir.Parent;

            ntuple._gCINTLines = null;
            ntuple._gObjectFiles = null;
            ntuple._gProxyFile = null;

            var eng = new VelocityEngine();
            eng.Init();

            TestUtils.ResetCacheDir();
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

#if false
        /// <summary>
        /// Make sure DeltaR2 compiles!
        /// </summary>
        [TestMethod]
        public void TestDeltaR2()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupe>();
            var listing = from evt in q
                          let tlz = ROOTUtils.CreateTLZ(evt.run * 2.2, 1.0, 0.0)
                          where tlz.DeltaR2(tlz) < 1.0
                          select evt;
            var dude = listing.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }
#endif
    }
}
