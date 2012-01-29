using System.IO;
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
        public static string _gProxyFile = "";
        public static string[] _gObjectFiles = { };
        public static string[] _gCINTLines = { };

        internal static void Reset()
        {
            _gProxyFile = "";
            _gObjectFiles = new string[0];
        }
    }

    /// <summary>
    /// Tests making sure that the code generated for combined
    /// queries is correct.
    /// </summary>
    [TestClass]
    public class TTreeExecutorCombinationTests
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;

            var eng = new VelocityEngine();
            eng.Init();

            QueryResultCacheTest.SetupCacheDir();

            TypeUtils._variableNameCounter = 0;

            ntuple._gCINTLines = null;
            ntuple._gObjectFiles = null;
            ntuple._gProxyFile = null;

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
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

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

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new FileInfo[] { rootFile }, "dude", typeof(ntuple), typeof(TestUtils.TestNtupeArr));
            var q1future = exe.ExecuteScalarAsFuture<int>(query1);
            var q2future = exe.ExecuteScalarAsFuture<int>(query2);

            //
            // Run them!
            //

            exe.ExecuteQueuedQueries();

            //
            // And check
            //

            Assert.AreEqual(q1future.Value, numberOfIter);
            Assert.AreEqual(q2future.Value, numberOfIter);
        }
    }
}
