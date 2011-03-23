// <copyright file="ParameterReplacementExpressionVisitorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq.Expressions;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>This class contains parameterized unit tests for ParameterReplacementExpressionVisitor</summary>
    [PexClass(typeof(ParameterReplacementExpressionVisitor))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ParameterReplacementExpressionVisitorTest
    {
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
    }
}
