using System;
using System.Linq;
using System.Linq.Expressions;
using LINQToTTreeLib.QueryVisitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTreeHelpers.Tests
{


    /// <summary>
    ///This is a test class for TestCreateTupleExpressionTransformer and is intended
    ///to contain all TestCreateTupleExpressionTransformer Unit Tests
    ///</summary>
    [TestClass]
    public class TestCreateTupleExpressionTransformer
    {
        [TestMethod]
        public void TestSimpleCreator()
        {
            var i1 = Expression.Constant(10);
            var i2 = Expression.Constant(20);

            var createGeneric = typeof(Tuple).GetMethods().Where(m => m.Name == "Create" && m.GetGenericArguments().Length == 2).First();
            Assert.IsNotNull(createGeneric);
            var createMethod = createGeneric.MakeGenericMethod(new Type[] { typeof(int), typeof(int) });
            Assert.IsNotNull(createMethod);

            var methodExpr = Expression.Call(null, createMethod, i1, i2);
            Assert.IsNotNull(methodExpr);

            var t = new CreateTupleExpressionTransformer();
            var r = t.Transform(methodExpr);

            Assert.IsInstanceOfType(r, typeof(NewExpression), "expression type");
            var ne = r as NewExpression;
            Assert.AreEqual(2, ne.Arguments.Count, "# of arguments to the new expression");
            Assert.AreEqual(i1, ne.Arguments[0], "arg 1 value");
            Assert.AreEqual(i2, ne.Arguments[1], "arg 2 value");

            Assert.AreEqual("Tuple`2", ne.Type.Name);
            var ga = ne.Type.GetGenericArguments();
            Assert.AreEqual(2, ga.Length, "# of generic arguments to the type");
            Assert.AreEqual(typeof(int), ga[0], "type 0");
            Assert.AreEqual(typeof(int), ga[1], "type 1");
        }
    }
}
