using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
// <copyright file="CodeContextTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for CodeContext</summary>
    [TestClass]
    public partial class CodeContextTest
    {
        [TestMethod]
        public void TestLoopVarCTor()
        {
            var c = new CodeContext();
            Assert.IsNull(c.LoopVariable, "ctor isn't null");
        }

        [TestMethod]
        public void TestLoopVarSetting()
        {
            var c = new CodeContext();
            var v = Expression.Variable(typeof(int), "d");
            c.SetLoopVariable(v, null);
            Assert.AreEqual(v, c.LoopVariable, "set didn't work");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLoopVarSetNull()
        {
            var c = new CodeContext();
            c.SetLoopVariable(null, null);
        }

        [TestMethod]
        public void TestSetExpressionAndPop()
        {
            var c = new CodeContext();
            Assert.IsNull(c.GetReplacement("d"), "initally get");
            var myvar = Expression.Variable(typeof(int), "fork");
            var p = c.Add("d", myvar);
            Assert.AreEqual(myvar, c.GetReplacement("d"), "callback after store");
            p.Pop();
            Assert.IsNull(c.GetReplacement("d"), "Final get");
        }

        [TestMethod]
        public void TestExpressionHiding()
        {
            var c = new CodeContext();
            var myvar1 = Expression.Variable(typeof(int), "d");
            var myvar2 = Expression.Variable(typeof(float), "dude");

            c.Add("p", myvar1);
            var p = c.Add("p", myvar2);
            Assert.AreEqual(myvar2, c.GetReplacement("p"), "replacement check");
            p.Pop();
            Assert.AreEqual(myvar1, c.GetReplacement("p"), "poped state");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddNullExpression()
        {
            var c = new CodeContext();
            c.Add("dude", (Expression)null);
        }

        [TestMethod]
        public void TestRemoveAndPopRemove()
        {
            var c = new CodeContext();
            var myvar = Expression.Variable(typeof(int), "d");
            c.Add("dude", myvar);

            var popper = c.Remove("dude");
            Assert.IsNull(c.GetReplacement("dude"), "incorrect dummy name");
            popper.Pop();
            Assert.AreEqual("d", (c.GetReplacement("dude") as ParameterExpression).Name, "incorrect dummy name");
        }


        [TestMethod]
        public void TestAddQueryReferenceSource()
        {
            var c = new CodeContext();
            var qs = new DummyQueryReference();
            var v = Expression.Constant(10);
            c.Add(qs, v);

            Assert.AreEqual(v, c.GetReplacement(qs), "Query Source Lookup.");
        }

        [TestMethod]
        public void TestQSRecordLookup()
        {
            var c = new CodeContext();
            var qs = new DummyQueryReference();
            var v = Expression.Constant(10);
            c.Add(qs, v);

            Assert.AreEqual(0, c.GetAndResetQuerySourceLookups().Length, "# of query source look ups isn't zero initially");

            // Look up bogus and make sure it is null

            var qsNotStored = new DummyQueryReference();
            c.GetReplacement(qsNotStored);
            Assert.AreEqual(0, c.GetAndResetQuerySourceLookups().Length, "Dummy lookup shouldn't cache reference.");

            // Look up good one and make sure it worked.

            c.GetReplacement(qs);
            var qsReferenced = c.GetAndResetQuerySourceLookups();
            Assert.AreEqual(1, qsReferenced.Length, "# of qs lookups after a real reference");
            Assert.AreEqual(qs, qsReferenced[0], "QS recorded in lookup");
        }

        [TestMethod]
        public void TestQSResetsRecordLookup()
        {
            var c = new CodeContext();
            var qs = new DummyQueryReference();
            var v = Expression.Constant(10);
            c.Add(qs, v);

            c.GetReplacement(qs);
            var qsReferenced = c.GetAndResetQuerySourceLookups();
            Assert.AreEqual(0, c.GetAndResetQuerySourceLookups().Length, "# of qs lookups after a reset");
        }

        [TestMethod]
        public void TestQSReloadLookup()
        {
            var c = new CodeContext();
            var qs1 = new DummyQueryReference();
            var qs2 = new DummyQueryReference();
            var qsList = new IQuerySource[] { qs1, qs2 };

            c.RestoreQuerySourceLookups(qsList);
            Assert.AreEqual(2, c.GetAndResetQuerySourceLookups().Length, "# after a restore");
        }

        [TestMethod]
        public void TestPopQM()
        {
            var c = new CodeContext();
            var qm = new QueryModel(new Remotion.Linq.Clauses.MainFromClause("dude", typeof(int), Expression.Parameter(typeof(IEnumerable<int>))),
                new Remotion.Linq.Clauses.SelectClause(Expression.Parameter(typeof(int))));

            Assert.IsNull(c.GetReplacement(qm), "should not be there yet");

            var e = Expression.Parameter(typeof(int));
            var s = c.Add(qm, e);
            Assert.AreEqual(e, c.GetReplacement(qm), "bad lookup");

            s.Pop();
            Assert.IsNull(c.GetReplacement(qm), "should be gone now.");
        }

        /// <summary>
        /// This will almost never happen, I suppose, but the logic is there, so we should make sure it is robust.
        /// </summary>
        [TestMethod]
        public void TestPopReplaceQM()
        {
            var c = new CodeContext();
            var qm = new QueryModel(new Remotion.Linq.Clauses.MainFromClause("dude", typeof(int), Expression.Parameter(typeof(IEnumerable<int>))),
                new Remotion.Linq.Clauses.SelectClause(Expression.Parameter(typeof(int))));

            var e1 = Expression.Parameter(typeof(int));
            var s1 = c.Add(qm, e1);
            Assert.AreEqual(e1, c.GetReplacement(qm), "bad lookup");
            var e2 = Expression.Parameter(typeof(int));
            var s2 = c.Add(qm, e2);

            Assert.AreEqual(e2, c.GetReplacement(qm), "high level lookup");
            s2.Pop();
            Assert.AreEqual(e1, c.GetReplacement(qm), "second level lookup");
        }

        [TestMethod]
        public void TestQMStoreResetsQSLookupList()
        {
            var c = new CodeContext();
            var qm = new QueryModel(new Remotion.Linq.Clauses.MainFromClause("dude", typeof(int), Expression.Parameter(typeof(IEnumerable<int>))),
                new Remotion.Linq.Clauses.SelectClause(Expression.Parameter(typeof(int))));

            var qs = new DummyQueryReference();
            c.Add(qs, Expression.Constant(10));
            c.GetReplacement(qs);

            var e1 = Expression.Parameter(typeof(int));
            var s1 = c.Add(qm, e1);

            Assert.AreEqual(0, c.GetAndResetQuerySourceLookups().Length, "# of qs references after a lookup");
        }

        [TestMethod]
        public void TestQMInvalidedByQSChange()
        {
            var c = new CodeContext();
            var qm = new QueryModel(new Remotion.Linq.Clauses.MainFromClause("dude", typeof(int), Expression.Parameter(typeof(IEnumerable<int>))),
                new Remotion.Linq.Clauses.SelectClause(Expression.Parameter(typeof(int))));

            var qs = new DummyQueryReference();
            c.Add(qs, Expression.Constant(10));
            c.GetReplacement(qs);

            var e1 = Expression.Parameter(typeof(int));
            var s1 = c.Add(qm, e1);

            Assert.AreEqual(e1, c.GetReplacement(qm), "Initial lookup");
            c.Add(qs, Expression.Constant(20));
            Assert.IsNull(c.GetReplacement(qm), "Lookup after referenced QS has changed");
        }

        /// <summary>
        /// Dummy QR for testing the caching.
        /// </summary>
        private class DummyQueryReference : IQuerySource
        {
            public string ItemName
            {
                get { return "this"; }
            }

            public Type ItemType
            {
                get { return typeof(int); }
            }
        }


        [TestMethod]
        public void TestRemoveOfNothing()
        {
            var c = new CodeContext();
            var popper = c.Remove("dude");
            popper.Pop();
        }
    }
}
