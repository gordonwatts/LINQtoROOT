﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.ResultOperators
{
    [TestClass]
    public class TestCount
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

        [TestMethod]
        public void TestBasicCount()
        {
            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         select d;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            /// Return type is correct
            Assert.IsNotNull(res.ResultValue, "Expected a result from the count!");
            Assert.AreEqual(res.ResultValue.Type, typeof(int), "integer return type expected");
        }
    }
}
