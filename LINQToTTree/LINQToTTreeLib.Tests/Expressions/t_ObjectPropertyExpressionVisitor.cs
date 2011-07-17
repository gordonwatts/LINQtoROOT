using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LINQToTTreeLib.Expressions;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for ObjectPropertyExpressionVisitorTest and is intended
    ///to contain all ObjectPropertyExpressionVisitorTest Unit Tests
    ///</summary>
    [TestClass()]
    [PexClass(typeof(ObjectPropertyExpressionVisitor))]
    public class ObjectPropertyExpressionVisitorTest
    {
        [TestInitialize]
        public void Setup()
        {
            MEFUtilities.MyClassInit();
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>
        /// Run the expression.
        /// </summary>
        /// <param name="xpr"></param>
        /// <returns></returns>
        Expression DoTranslate(Expression expr)
        {
            // Remove Tuple's and similar things.
            var objlift = new ObjectPropertyExpressionVisitor();
            var exprObjsRemoved = objlift.VisitExpression(expr);
            return exprObjsRemoved;
        }

        [TestMethod]
        public void TestTranslateNewPair1()
        {
            Expression<Func<int>> lambaExpr = () => new Tuple<int, int>(5, 10).Item1;
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambaExpr.Body, caches);
            Assert.IsInstanceOfType(result, typeof(ConstantExpression), "Expression type");
            Assert.AreEqual(typeof(int), result.Type, "result type not right");
            Assert.AreEqual(5, (result as ConstantExpression).Value, "value incorrect");
        }

        [TestMethod]
        public void TestTranslateNewPair2()
        {
            Expression<Func<int>> lambaExpr = () => new Tuple<int, int>(5, 10).Item2;
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambaExpr.Body, caches);
            Assert.IsInstanceOfType(result, typeof(ConstantExpression), "Expression type");
            Assert.AreEqual(typeof(int), result.Type, "result type not right");
            Assert.AreEqual(10, (result as ConstantExpression).Value, "value incorrect");
        }

        [TestMethod]
        public void TestAccessAnonObjPropery()
        {
            Expression<Func<int>> lambaExpr = () => new { Item1 = 5, Item2 = 10 }.Item2;
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambaExpr.Body, caches);
            Assert.IsInstanceOfType(result, typeof(ConstantExpression), "Expression type");
            Assert.AreEqual(typeof(int), result.Type, "result type not right");
            Assert.AreEqual(10, (result as ConstantExpression).Value, "value incorrect");
        }

        [TestMethod]
        public void TestAccessAnonObjProperyRev()
        {
            Expression<Func<int>> lambaExpr = () => new { Item2 = 5, Item1 = 10 }.Item2;
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambaExpr.Body, caches);
            Assert.IsInstanceOfType(result, typeof(ConstantExpression), "Expression type");
            Assert.AreEqual(typeof(int), result.Type, "result type not right");
            Assert.AreEqual(5, (result as ConstantExpression).Value, "value incorrect");
        }
    }
}
