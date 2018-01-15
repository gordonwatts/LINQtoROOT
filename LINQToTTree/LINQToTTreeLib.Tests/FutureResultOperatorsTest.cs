using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NVelocity.App;
using LINQToTTreeLib.ResultOperators;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for FutureResultOperators</summary>
    [TestClass]
    [DeploymentItem(@"Templates\TSelectorTemplate.cxx")]
    public partial class FutureResultOperatorsTest
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupe
        {
#pragma warning disable 0169
            public int run;
#pragma warning restore 0169
        }

        /// <summary>
        /// Ntuple with emptys for everything.
        /// </summary>
        public class ntuple
        {
            public static string[] _gObjectFiles = { };
            public static string[] _gCINTLines = { };

            internal static void Reset()
            {
                _gObjectFiles = new string[0];
            }
        }

        class SimpleEventNtup
        {
#pragma warning disable 0649
            public int run;
#pragma warning restore 0649

            public static string[] _gObjectFiles = null;
            public static string[] _gCINTLines = null;
        }

        [TestMethod]
        public void TestSimpleSingleQuery()
        {
            int numberOfIter = 10;

            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            var result = exe.ExecuteScalarAsFuture<int>(query);

            Assert.IsNotNull(result, "future should exist!");
            Assert.IsFalse(result.HasValue, "future shoudl not have executed by now!");

            var val = result.Value;
            Assert.AreEqual(numberOfIter, val, "incorrect result came back.");
            Assert.IsTrue(result.HasValue, "value should be marked by now!");
        }

        [TestMethod]
        public void SumInFutureQuery()
        {
            int numberOfIter = 10;

            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Select(evt => evt.run).Sum();
            var query = DummyQueryExectuor.LastQueryModel;

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            var result = exe.ExecuteScalarAsFuture<int>(query);

            Assert.IsNotNull(result, "future should exist!");
            Assert.IsFalse(result.HasValue, "future shoudl not have executed by now!");

            var val = result.Value;
            Assert.AreEqual(numberOfIter*10, val, "incorrect result came back.");
            Assert.IsTrue(result.HasValue, "value should be marked by now!");
        }

        [TestMethod]
        [ExpectedException(typeof(AverageNotAllowedAtTopLevelException))]
        public void AverageAsFutureQuery()
        {
            int numberOfIter = 10;

            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Select(e => e.run).Average();
            var query = DummyQueryExectuor.LastQueryModel;

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            var result = exe.ExecuteScalarAsFuture<int>(query);

            Assert.IsNotNull(result, "future should exist!");
            Assert.IsFalse(result.HasValue, "future shoudl not have executed by now!");

            var val = result.Value;
            Assert.AreEqual(numberOfIter, val, "incorrect result came back.");
            Assert.IsTrue(result.HasValue, "value should be marked by now!");
        }

        [TestMethod]
        public void TestWhereQuery()
        {
            int numberOfIter = 10;

            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Where(v => v.run > 0).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            var result = exe.ExecuteScalarAsFuture<int>(query);

            Assert.IsNotNull(result, "future should exist!");
            Assert.IsFalse(result.HasValue, "future shoudl not have executed by now!");

            var val = result.Value;
            Assert.AreEqual(numberOfIter, val, "incorrect result came back.");
            Assert.IsTrue(result.HasValue, "value should be marked by now!");
        }

        [TestMethod]
        public void TestLinkedQuery()
        {
            int numberOfIter = 10;

            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);
            var q = new QueriableDummy<TestNtupe>();
            var dude1 = q.Count();
            var query1 = DummyQueryExectuor.LastQueryModel;

            var dude2 = q.Where(v => v.run > 20).Count();
            var query2 = DummyQueryExectuor.LastQueryModel;

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            var result1 = exe.ExecuteScalarAsFuture<int>(query1);
            var result2 = exe.ExecuteScalarAsFuture<int>(query2);

            Assert.IsFalse(result1.HasValue, "r1 should not have a value yet");
            Assert.IsFalse(result2.HasValue, "r2 should not have a value yet");

            var r1v = result1.Value;

            Assert.IsTrue(result1.HasValue, "r1 should have a value");
            Assert.IsTrue(result2.HasValue, "r2 should have a value");

            Assert.AreEqual(0, result2.Value, "incorrect r2");
            Assert.AreEqual(numberOfIter, result1.Value, "incorrect r1");
        }
    }
}
