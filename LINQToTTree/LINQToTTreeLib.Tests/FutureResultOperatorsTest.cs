// <copyright file="FutureResultOperatorsTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NVelocity.App;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for FutureResultOperators</summary>
    [PexClass(typeof(FutureResultOperators))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class FutureResultOperatorsTest
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;

            var eng = new VelocityEngine();
            eng.Init();

            QueryResultCacheTest.SetupCacheDir();
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
            public static string _gProxyFile = "";
            public static string[] _gObjectFiles = { };
            public static string[] _gCINTLines = { };

            internal static void Reset()
            {
                _gProxyFile = "";
                _gObjectFiles = new string[0];
            }
        }

        /// <summary>Test stub for FutureCount(IQueryable`1&lt;!!0&gt;)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        public IFutureValue<int> FutureCount<TSource>(IQueryable<TSource> source)
        {
            IFutureValue<int> result = FutureResultOperators.FutureCount<TSource>(source);
            return result;
            // TODO: add assertions to method FutureResultOperatorsTest.FutureCount(IQueryable`1<!!0>)
        }

        class SimpleEventNtup
        {
#pragma warning disable 0649
            public int run;
#pragma warning restore 0649

            public static string _gProxyFile = "test.h";
            public static string[] _gObjectFiles = null;
            public static string[] _gCINTLines = null;
        }

        [TestMethod]
        public void TestSimpleSingleQuery()
        {
            int numberOfIter = 10;

            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);
            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple));

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
            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Where(v => v.run > 0).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple));

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
            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");
            var q = new QueriableDummy<TestNtupe>();
            var dude1 = q.Count();
            var query1 = DummyQueryExectuor.LastQueryModel;

            var dude2 = q.Where(v => v.run > 20).Count();
            var query2 = DummyQueryExectuor.LastQueryModel;

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple));

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
