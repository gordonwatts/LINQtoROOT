// <copyright file="FormattingQueryVisitorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;

namespace LINQToTTreeLib.QueryVisitors
{
    /// <summary>This class contains parameterized unit tests for FormattingQueryVisitor</summary>
    [PexClass(typeof(FormattingQueryVisitor))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class FormattingQueryVisitorTest
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            MEFUtilities.AddPart(new TypeHandlers.TypeHandlerConvert());
            DummyQueryExectuor.GlobalInitalized = false;
            QueryResultCacheTest.SetupCacheDir();
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>Test stub for Format(QueryModel)</summary>
        [PexMethod]
        internal string Format(QueryModel query)
        {
            string result = FormattingQueryVisitor.Format(query);
            Assert.IsNotNull(result, "null return from query string");
            Assert.AreNotEqual(0, result.Length, "query string length");
            return result;
            // TODO: add assertions to method FormattingQueryVisitorTest.Format(QueryModel)
        }

        public class arrayntup
        {
            public int run;
            public int[] vals;
        }

        /// <summary>
        /// Create a query model that we cna use for tests. We have several possible ones we can create that should be different.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private QueryModel MakeQuery(int queryIndex)
        {
            if (queryIndex == 0)
            {
                var q = new QueriableDummy<ntup>();
                var result = from d in q
                             select d;
                var c = result.Count();

                return DummyQueryExectuor.LastQueryModel;
            }

            if (queryIndex == 1)
            {
                var q = new QueriableDummy<ntup>();
                var result = from d in q
                             where d.run > 20
                             select d;
                var c = result.Count();

                return DummyQueryExectuor.LastQueryModel;
            }

            if (queryIndex == 2)
            {
                var q = new QueriableDummy<ntup>();
                var result = from d in q
                             select d.run;
                var r = result.Plot("hi", "there", 20, 0.0, 10.0);
                return DummyQueryExectuor.LastQueryModel;
            }
            if (queryIndex == 3)
            {
                var q = new QueriableDummy<ntup>();
                var result = from d in q
                             select d.run;
                var r = result.Plot("hi", "there", 40, 0.0, 10.0);
                return DummyQueryExectuor.LastQueryModel;
            }
            if (queryIndex == 4)
            {
                var q = new QueriableDummy<ntup>();
                var result = from d in q
                             select d.run;
                var r = result.Plot("hi1", "there is no sppon", 20, 0.0, 10.0);
                return DummyQueryExectuor.LastQueryModel;
            }

            if (queryIndex == 5)
            {
                var q = new QueriableDummy<arrayntup>();
                var result = from d in q
                             from e in d.vals
                             select e;
                var r = result.Count();
                return DummyQueryExectuor.LastQueryModel;
            }

            if (queryIndex == 6)
            {
                var q = new QueriableDummy<arrayntup>();
                var result = from d in q
                             select (d.vals.Count());
                var r = result.Count();
                return DummyQueryExectuor.LastQueryModel;
            }

            if (queryIndex == 7)
            {
                var q = new QueriableDummy<arrayntup>();
                var result = from d in
                                 (from s in q select s.vals.Count())
                             select d;
                var r = result.Count();
                return DummyQueryExectuor.LastQueryModel;
            }

            return null;
        }

        [PexMethod(MaxBranches = 80000)]
        public void TestQueryDoesntCrash(int queryIndex)
        {
            var q = MakeQuery(queryIndex);
            var ans = Format(q);
            Console.WriteLine("The imporeve answer is {0}", ans);
            if (q != null)
                Console.WriteLine("The normal way reuslt is {0}", q.ToString());
        }

        [TestMethod]
        public void TestQuerySimple()
        {
            var q = MakeQuery(0);
            var str = FormattingQueryVisitor.Format(q);
            Console.WriteLine("The result is {0}", str);
            Console.WriteLine("The normal way reuslt is {0}", q.ToString());
            Assert.IsNotNull(str, "null return");
            Assert.AreNotEqual(0, str.Length, "zero length guy");
        }

        [TestMethod]
        public void TestForSelect()
        {
            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         where d.run > 20
                         select d;
            var c = result.Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var str = FormattingQueryVisitor.Format(qm);
            Console.WriteLine("result: {0}", str);
            Assert.IsTrue(str.Contains("select"), "Missing select in '" + str + "'.");
        }

        [TestMethod]
        public void TestForWhere()
        {
            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         where d.run > 20
                         select d;
            var c = result.Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var str = FormattingQueryVisitor.Format(qm);
            Console.WriteLine("result: {0}", str);
            Assert.IsTrue(str.Contains("where"), "Missing select in '" + str + "'.");
        }

        [TestMethod]
        public void TestForNestedFrom()
        {
            var q = new QueriableDummy<arrayntup>();
            var result = from d in q
                         from e in d.vals
                         select e;
            var r = result.Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var str = FormattingQueryVisitor.Format(qm);
            Console.WriteLine("result: {0}", str);
            Assert.IsTrue(str.Contains("vals"), "Missing vals in '" + str + "'.");
        }
    }
}
