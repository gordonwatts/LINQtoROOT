using System;
using System.Linq.Expressions;
using LINQToTTreeLib.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    ///This is a test class for TestArrayExpressionParser and is intended
    ///to contain all TestArrayExpressionParser Unit Tests
    ///</summary>
    [TestClass()]
    public class TestArrayExpressionParser
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestForNonArray()
        {
            ArrayExpressionParser.ParseArrayExpression(Expression.Variable(typeof(int), "d"));
        }

        [TestMethod]
        public void TestRunForNormalArray()
        {
            ArrayExpressionParser.ParseArrayExpression(Expression.Variable(typeof(int[]), "d"));
        }
    }
}
