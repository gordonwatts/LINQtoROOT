using LINQToTTreeLib.QueryVisitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;

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
        public void TestSimpleCreator2()
        {
            MakeTupleWithNArgs(2);
        }

        [TestMethod]
        public void TestSimpleCreator4()
        {
            MakeTupleWithNArgs(4);
        }

        private static void MakeTupleWithNArgs(int n)
        {
            var createGeneric = typeof(Tuple).GetMethods().Where(m => m.Name == "Create" && m.GetGenericArguments().Length == n).First();
            Assert.IsNotNull(createGeneric);
            var createMethod = createGeneric.MakeGenericMethod(Enumerable.Range(0, n).Select(i => typeof(int)).ToArray());
            Assert.IsNotNull(createMethod);

            var i1 = Expression.Constant(10);
            var i2 = Expression.Constant(20);

            var args = Enumerable.Range(0, n).Select(i => Expression.Constant(i * 10)).ToArray();
            var methodExpr = Expression.Call(null, createMethod, args);
            Assert.IsNotNull(methodExpr);

            var t = new CreateTupleExpressionTransformer();
            var r = t.Transform(methodExpr);

            Assert.IsInstanceOfType(r, typeof(NewExpression), "expression type");
            var ne = r as NewExpression;
            Assert.AreEqual(n, ne.Arguments.Count, "# of arguments to the new expression");
            Assert.IsTrue(args.Zip(ne.Arguments, (f, s) => f == s).All(ty => true), "args are the same");

            Assert.AreEqual(string.Format("Tuple`{0}", n), ne.Type.Name);
            var ga = ne.Type.GetGenericArguments();
            Assert.AreEqual(n, ga.Length, "# of generic arguments to the type");
            Assert.IsTrue(ga.All(ty => ty == typeof(int)), "all type ");
        }
    }
}
