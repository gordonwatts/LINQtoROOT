// <copyright file="ParameterReplacementExpressionVisitorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq.Expressions;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>This class contains parameterized unit tests for ParameterReplacementExpressionVisitor</summary>
    [PexClass(typeof(ParameterReplacementExpressionVisitor))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ParameterReplacementExpressionVisitorTest
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        [TestMethod]
        public void TestNoReplacement()
        {
            var cc = new CodeContext();
            var myvar = Expression.Variable(typeof(int), "d");
            var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(myvar, cc);

            var asp = expr as ParameterExpression;
            Assert.IsNotNull(asp, "expected the returned expression to be correct");
            Assert.AreEqual(typeof(int), asp.Type, "bad type coming back");
            Assert.AreEqual("d", asp.Name, "variable name");
        }

        [TestMethod]
        public void TestSimpleReplacement()
        {
            var cc = new CodeContext();
            cc.Add("d", Expression.Constant(20));

            var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(Expression.Variable(typeof(int), "d"), cc);

            var asconst = expr as ConstantExpression;
            Assert.IsNotNull(asconst, "constant replacement");
            Assert.AreEqual(20, asconst.Value, "value of translation");
        }

        [TestMethod]
        public void TestSimpleNestedReplacement()
        {
            var cc = new CodeContext();
            cc.Add("d", Expression.Variable(typeof(int), "f"));
            cc.Add("f", Expression.Constant(20));

            var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(Expression.Variable(typeof(int), "d"), cc);

            var asconst = expr as ConstantExpression;
            Assert.IsNotNull(asconst, "constant replacement");
            Assert.AreEqual(20, asconst.Value, "value of translation");
        }

        [TestMethod]
        public void TestArrayReplacement()
        {
            var myvar = Expression.Variable(typeof(int[]), "d");
            var myref = Expression.ArrayIndex(myvar, Expression.Constant(1));

            var cc = new CodeContext();
            cc.Add("d", Expression.Variable(typeof(int[]), "dude"));

            var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(myref, cc);

            var be = expr as BinaryExpression;
            Assert.AreEqual(ExpressionType.ArrayIndex, be.NodeType, "index bad");
            var pe = be.Left as ParameterExpression;
            Assert.IsNotNull(pe, "bad array value");
            Assert.AreEqual("dude", pe.Name, "array parameter name");
        }

        [TestMethod]
        public void TestArrayIndexReplacement()
        {
            var myarray = Expression.Variable(typeof(int[]), "darray");
            var myindex = Expression.Variable(typeof(int), "d");
            var myref = Expression.ArrayIndex(myarray, myindex);

            var cc = new CodeContext();
            cc.Add("d", Expression.Constant(10));

            var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(myref, cc);

            var be = expr as BinaryExpression;
            Assert.AreEqual(ExpressionType.ArrayIndex, be.NodeType, "index bad");
            var ce = be.Right as ConstantExpression;
            Assert.IsNotNull(ce, "reference to constant failed");
            Assert.AreEqual(10, ce.Value, "value of array index");
        }

        class dummyQuerySource : IQuerySource
        {
            public string ItemName
            {
                get { return "d"; }
            }

            public Type ItemType
            {
                get { return typeof(int); }
            }
        }

        [TestMethod]
        public void TestSubqueryPatternReplacement()
        {
            var qexpr = new QuerySourceReferenceExpression(new dummyQuerySource());

            CodeContext cc = new CodeContext();
            cc.Add("d", Expression.Variable(typeof(int), "f"));
            cc.Add(qexpr.ReferencedQuerySource, Expression.Parameter(typeof(int), "d"));

            var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(qexpr, cc);

            var asp = expr as ParameterExpression;
            Assert.IsNotNull(asp, "expected the returned expression to be of type ParameterExpression");
            Assert.AreEqual(typeof(int), asp.Type, "bad type coming back");
            Assert.AreEqual("f", asp.Name, "variable name");
        }

        [TestMethod]
        public void TestSubqueryNestedPatternReplacement()
        {
            var qexpr = new QuerySourceReferenceExpression(new dummyQuerySource());

            CodeContext cc = new CodeContext();
            cc.Add("d", Expression.Variable(typeof(int), "f"));
            cc.Add("f", Expression.Variable(typeof(int), "fork"));
            cc.Add(qexpr.ReferencedQuerySource, Expression.Parameter(typeof(int), "d"));

            var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(qexpr, cc);

            var asp = expr as ParameterExpression;
            Assert.IsNotNull(asp, "expected the returned expression to be of type ParameterExpression");
            Assert.AreEqual(typeof(int), asp.Type, "bad type coming back");
            Assert.AreEqual("fork", asp.Name, "variable name");
        }

        /// <summary>
        /// Helper classes for the next test.
        /// </summary>
        public class TestSubExpressionBuriedArgumentClass
        {
            public TestSubExpressionBuriedArgumentPVClass[] PVs;
        }

        public class TestSubExpressionBuriedArgumentPVClass
        {
            public int nTracks;
        }

#if false
        [TestMethod]
        public void TestSubExpressionBuriedArgument()
        {
            ////
            /// This is a regression we found in code. Bummer. There seems to be a problem with
            /// doing parameter replacement that is a subquery expression.
            /// 

            var cc = new CodeContext();

            ///
            /// Create the sub query expression that is
            /// d.PVs => First() - so take the first of an array.
            /// To create the sub query expression we need a query expression!
            /// We then thake .nTracks, and that becomes the argubment "v".
            /// 

            var q = new QueriableDummy<TestSubExpressionBuriedArgumentClass>();
            var result = (from d in q
                          select d.PVs.First()).Count();
            var qm = DummyQueryExectuor.LastQueryModel;
            var squery = qm.SelectClause.Selector as SubQueryExpression;
            var ntracks = Expression.MakeMemberAccess(squery, typeof(TestSubExpressionBuriedArgumentPVClass).GetMember("nTracks").First());
            Console.WriteLine("The internal subquery ntrack expression: {0}", ntracks.ToString());
            Console.WriteLine("And the query model for the squery expression above is {0}.", squery.QueryModel.ToString());
            Assert.Inconclusive();
        }
#endif

        class testLambdaSimple
        {
#pragma warning disable 0649
            public float pt;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestLambda()
        {
            Expression<Func<testLambdaSimple, float>> lambdaExpr = t => t.pt / ((float)100.0);

            CodeContext cc = new CodeContext();
            var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(lambdaExpr, cc);

            Assert.AreEqual(lambdaExpr.ToString(), expr.ToString(), "lambda changed");
        }

        [TestMethod]
        public void TestLambdaWithPredefinedName()
        {
            /// This is basically a scope test! :-)
            Expression<Func<testLambdaSimple, float>> lambdaExpr = t => t.pt / ((float)100.0);

            CodeContext cc = new CodeContext();
            cc.Add("t", Expression.Variable(typeof(testLambdaSimple), "fook"));
            var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(lambdaExpr, cc);

            Assert.AreEqual(lambdaExpr.ToString(), expr.ToString(), "lambda changed");
            Assert.AreEqual("fook", (cc.GetReplacement("t") as ParameterExpression).Name, "code context was altered");
        }

        [TestMethod]
        public void TestLambdaWithPredefinedNameAsParam()
        {
            /// This is basically a scope test! :-)
            Expression<Func<testLambdaSimple, float>> lambdaExpr = t => t.pt / ((float)100.0);

            CodeContext cc = new CodeContext();
            cc.Add("t", new Variables.ValSimple("fook", typeof(testLambdaSimple)));
            var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(lambdaExpr, cc);

            Assert.AreEqual(lambdaExpr.ToString(), expr.ToString(), "lambda changed");
            Assert.AreEqual("fook", cc.GetReplacement("t", typeof(testLambdaSimple)).RawValue, "code context was altered");
        }
    }
}
