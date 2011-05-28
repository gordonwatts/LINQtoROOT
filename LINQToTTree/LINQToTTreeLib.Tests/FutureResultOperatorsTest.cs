// <copyright file="FutureResultOperatorsTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.IO;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void TestSetup()
        {
            using (var proxyWriter = File.CreateText("test.h"))
            {
                proxyWriter.WriteLine("You'd better never compile this!! :-)");
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
            public int run;

            public static string _gProxyFile = "test.h";
            public static string[] _gObjectFiles = null;
            public static string[] _gCINTLines = null;
        }

        private QueriableTTree<SimpleEventNtup> NewTestQueryTTree()
        {
            return new QueriableTTree<SimpleEventNtup>(new FileInfo[] { new FileInfo(@"..\..\..\LINQToTTreeLib.Tests\testfile_intonly.root") }, "dude");
        }

        [TestMethod]
        public void TestSimpleSingleQuery()
        {
            ///
            /// Create a simple query, and make sure that nothing happen.s
            /// 

            var q = NewTestQueryTTree();
            var dude = q.FutureCount();
            Assert.IsNotNull(dude, "expected a future value back!");

            Assert.IsNull(DummyQueryExectuor.LastQueryModel, "no query should have been executed up to now!");
            var temp = dude.Value;
            Assert.IsNotNull(DummyQueryExectuor.LastQueryModel, "a query should have been executed at this point");
            Assert.IsTrue(dude.HasValue, "future should be marked as having a value");
        }

        [TestMethod]
        public void TestWhereQuery()
        {
            ///
            /// Create a simple query, and make sure that nothing happen.s
            /// 

            var q = NewTestQueryTTree();
            var dude = q.Where(v => v.run > 10).FutureCount();

            Assert.IsNull(DummyQueryExectuor.LastQueryModel, "no query should have been executed up to now!");
            var temp = dude.Value;
            Assert.IsNotNull(DummyQueryExectuor.LastQueryModel, "a query should have been executed at this point");
        }

        [TestMethod]
        public void TestLinkedQuery()
        {
            ///
            /// Create a simple query, and make sure that nothing happen.s
            /// 

            var q = NewTestQueryTTree();
            var dude1 = q.FutureCount();
            var dude2 = q.Where(v => v.run > 10).FutureCount();

            Assert.IsNull(DummyQueryExectuor.LastQueryModel, "no query should have been executed up to now!");
            var temp = dude1.Value;
            Assert.IsTrue(dude2.HasValue, "Linked future should have also been evaluated");
        }
    }
}
