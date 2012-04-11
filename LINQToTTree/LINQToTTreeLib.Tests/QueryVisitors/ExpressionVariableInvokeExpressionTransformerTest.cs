using System;
using System.Linq.Expressions;
using LINQToTTreeLib.QueryVisitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for TestExpressionVariableInvokeExpressionTransformer and is intended
    ///to contain all TestExpressionVariableInvokeExpressionTransformer Unit Tests
    ///</summary>
    [TestClass]
    public class TestExpressionVariableInvokeExpressionTransformer
    {
        [TestMethod]
        public void TestSimpleExpression()
        {
            Expression<Func<int, int>> adder = i => i + 1;

            Expression<Func<int>> doit = () => adder.Invoke(5);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as MethodCallExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual(ExpressionType.Add, r.NodeType, "node type of result");
            var ad = r as BinaryExpression;
            Assert.AreEqual(5, (ad.Left as ConstantExpression).Value, "left value");
            Assert.AreEqual(1, (ad.Right as ConstantExpression).Value, "right value");
        }

        [TestMethod]
        public void TestSimpleExpressionCompile()
        {
            Expression<Func<int, int>> adder = i => i + 1;

            Expression<Func<int>> doit = () => adder.Compile()(5);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as InvocationExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual(ExpressionType.Add, r.NodeType, "node type of result");
            var ad = r as BinaryExpression;
            Assert.AreEqual(5, (ad.Left as ConstantExpression).Value, "left value");
            Assert.AreEqual(1, (ad.Right as ConstantExpression).Value, "right value");
        }

        /// <summary>
        /// for a static property access test below.
        /// </summary>
        static Expression<Func<int, int>> gAdder = i => i + 1;

        [TestMethod]
        public void TestExpressionInStaticVariable()
        {
            Expression<Func<int>> doit = () => gAdder.Invoke(5);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as MethodCallExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual(ExpressionType.Add, r.NodeType, "node type of result");
            var ad = r as BinaryExpression;
            Assert.AreEqual(5, (ad.Left as ConstantExpression).Value, "left value");
            Assert.AreEqual(1, (ad.Right as ConstantExpression).Value, "right value");
        }

        class tempProp
        {
            public Expression<Func<int, int>> myAdderProp { get; private set; }
            public Expression<Func<int, int>> myAdderField;
            public tempProp()
            {
                myAdderProp = i => i + 1;
                myAdderField = i => i + 1;
            }
        }

        [TestMethod]
        public void TestExpressionInProperyOfObject()
        {
            var t = new tempProp();

            Expression<Func<int>> doit = () => t.myAdderProp.Invoke(5);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as MethodCallExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual(ExpressionType.Add, r.NodeType, "node type of result");
            var ad = r as BinaryExpression;
            Assert.AreEqual(5, (ad.Left as ConstantExpression).Value, "left value");
            Assert.AreEqual(1, (ad.Right as ConstantExpression).Value, "right value");
        }

        [TestMethod]
        public void TestExpressionInFieldOfObject()
        {
            var t = new tempProp();

            Expression<Func<int>> doit = () => t.myAdderField.Invoke(5);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as MethodCallExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual(ExpressionType.Add, r.NodeType, "node type of result");
            var ad = r as BinaryExpression;
            Assert.AreEqual(5, (ad.Left as ConstantExpression).Value, "left value");
            Assert.AreEqual(1, (ad.Right as ConstantExpression).Value, "right value");
        }

        [TestMethod]
        public void TestNestedFunction()
        {
            Expression<Func<int, int>> add1 = i => i + 1;
            Expression<Func<int, int>> add2 = i => add1.Invoke(i) + 2;

            Expression<Func<int>> doit = () => add2.Invoke(5);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as MethodCallExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual("((5 + 1) + 2)", r.ToString(), "Expression result");
        }

        [TestMethod]
        public void TestNestedFunctionCompile()
        {
            Expression<Func<int, int>> add1 = i => i + 1;
            Expression<Func<int, int>> add2 = i => add1.Compile()(i) + 2;

            Expression<Func<int>> doit = () => add2.Compile()(5);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as InvocationExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual("((5 + 1) + 2)", r.ToString(), "Expression result");
        }

        [TestMethod]
        public void TestNestedFunctionMixed1()
        {
            Expression<Func<int, int>> add1 = i => i + 1;
            Expression<Func<int, int>> add2 = i => add1.Invoke(i) + 2;

            Expression<Func<int>> doit = () => add2.Compile()(5);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as InvocationExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual("((5 + 1) + 2)", r.ToString(), "Expression result");
        }

        [TestMethod]
        public void TestNestedFunctionMixed2()
        {
            Expression<Func<int, int>> add1 = i => i + 1;
            Expression<Func<int, int>> add2 = i => add1.Compile()(i) + 2;

            Expression<Func<int>> doit = () => add2.Invoke(5);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as MethodCallExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual("((5 + 1) + 2)", r.ToString(), "Expression result");
        }

        [TestMethod]
        public void TestSimpleExpressionWithComplexArg()
        {
            Expression<Func<int, int>> add1 = i => i + 1;
            Expression<Func<int, int>> add2 = i => add1.Invoke(i + 3) + 2;

            Expression<Func<int>> doit = () => add2.Invoke(5);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as MethodCallExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual("(((5 + 3) + 1) + 2)", r.ToString(), "Expression result");
        }

        [TestMethod]
        public void TestSimpleExpressionWithComplexArgCompile()
        {
            Expression<Func<int, int>> add1 = i => i + 1;
            Expression<Func<int, int>> add2 = i => add1.Compile()(i + 3) + 2;

            Expression<Func<int>> doit = () => add2.Compile()(5);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as InvocationExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual("(((5 + 3) + 1) + 2)", r.ToString(), "Expression result");
        }

        [TestMethod]
        public void TestNestedArgExpression()
        {
            Expression<Func<int, int>> adder = i => i + 1;

            Expression<Func<int>> doit = () => adder.Invoke(adder.Invoke(2));
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as MethodCallExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual("((2 + 1) + 1)", r.ToString(), "Expression result");
        }

        [TestMethod]
        public void TestNestedArgExpressionCompile()
        {
            Expression<Func<int, int>> adder = i => i + 1;

            Expression<Func<int>> doit = () => adder.Compile()(adder.Compile()(2));
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as InvocationExpression);
            Assert.IsNotNull(r);
            Assert.AreEqual("((2 + 1) + 1)", r.ToString(), "Expression result");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRecursionInvokation()
        {
            Expression<Func<int, int>> adder = null;
            adder = i => adder.Invoke(i) + 1;

            Expression<Func<int>> doit = () => adder.Invoke(1);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as MethodCallExpression);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestRecursionInvokationCompile()
        {
            Expression<Func<int, int>> adder = null;
            adder = i => adder.Compile()(i) + 1;

            Expression<Func<int>> doit = () => adder.Compile()(1);
            var expr = doit.Body;

            var target = new ExpressionVariableInvokeExpressionTransformer();
            var r = target.Transform(expr as InvocationExpression);
        }
    }
}

