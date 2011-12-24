using System;
using System.Diagnostics;
using System.Linq.Expressions;
using LINQToTTreeLib.Expressions;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.ResultOperators
{
    [TestClass]
    [PexClass(typeof(SubExpressionReplacement))]
    public partial class SubExpressionReplacementTest
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

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public Expression TestReplacement(Expression source, Expression pattern, Expression replacement)
        {
            return source.ReplaceSubExpression(pattern, replacement);
        }

        [TestMethod]
        public void TestSimpleReplacement()
        {
            var arr = Expression.Parameter(typeof(int[]), "myarr");
            var param = Expression.Parameter(typeof(int), "dude");

            var expr = Expression.ArrayIndex(arr, param);

            var rep = Expression.Parameter(typeof(int), "fork");
            var result = expr.ReplaceSubExpression(param, rep);

            Trace.WriteLine("Expression: " + result.ToString());
            Assert.IsFalse(result.ToString().Contains("dude"), "Contains the dude variable");
        }
    }
}
