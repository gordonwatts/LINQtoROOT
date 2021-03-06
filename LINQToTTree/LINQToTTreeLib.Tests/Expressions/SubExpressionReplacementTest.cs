using LINQToTTreeLib.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Tests.ResultOperators
{
    [TestClass]
    public partial class SubExpressionReplacementTest
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

#if false
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public Expression TestReplacement(Expression source, Expression pattern, Expression replacement)
        {
            return source.ReplaceSubExpression(pattern, replacement);
        }
#endif

        [TestMethod]
        public void TestSimpleReplacement()
        {
            var arr = Expression.Parameter(typeof(int[]), "myarr");
            var param = Expression.Parameter(typeof(int), "dude");

            var expr = Expression.ArrayIndex(arr, param);

            var rep = Expression.Parameter(typeof(int), "fork");
            var result = expr.ReplaceSubExpression(param, rep);

            Debug.WriteLine("Expression: " + result.ToString());
            Assert.IsFalse(result.ToString().Contains("dude"), "Contains the dude variable");
        }
    }
}
