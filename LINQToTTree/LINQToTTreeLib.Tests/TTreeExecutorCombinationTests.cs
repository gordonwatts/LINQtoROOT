using System;
using System.Linq;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NVelocity.App;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// Ntuple with emptys for everything.
    /// </summary>
    public class ntuple
    {
        public static string[] _gObjectFiles = { };
        public static string[] _gCINTLines = { };
        public static string[] _gClassesToDeclare = { };
        public static string[] _gClassesToDeclareIncludes = { };

        internal static void Reset()
        {
            _gObjectFiles = new string[0];
        }
    }

    /// <summary>
    /// Tests making sure that the code generated for combined
    /// queries is correct.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"Templates\TSelectorTemplate.cxx")]
    public class TTreeExecutorCombinationTests
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();

            ntuple.Reset();
            ntuple._gCINTLines = null;
            ntuple._gObjectFiles = null;
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        [TestMethod]
        public void TestSimpleLoopAndFilterCombine()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestUtils.TestNtupeArr>();
            var dudeQ1 = from evt in q
                         where (evt.myvectorofint.Max() > 5)
                         select evt;
            var dude1 = dudeQ1.Count();
            var query1 = DummyQueryExectuor.LastQueryModel;

            var dudeQ2 = from evt in q
                         where (evt.myvectorofint.Max() > 5)
                         select evt;
            var dude2 = dudeQ2.Count();
            var query2 = DummyQueryExectuor.LastQueryModel;

            //
            // Convert to future's
            //

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestUtils.TestNtupeArr));
            var q1future = exe.ExecuteScalarAsFuture<int>(query1);
            var q2future = exe.ExecuteScalarAsFuture<int>(query2);

            //
            // Run them!
            //

            exe.ExecuteQueuedQueries().Wait();

            //
            // And check
            //

            Assert.AreEqual(q1future.Value, numberOfIter);
            Assert.AreEqual(q2future.Value, numberOfIter);
        }
    }
}
