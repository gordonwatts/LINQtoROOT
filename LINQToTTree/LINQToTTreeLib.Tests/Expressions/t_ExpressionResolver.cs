using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.TypeHandlers;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for TestExpressionResolver and is intended
    ///to contain all TestExpressionResolver Unit Tests
    ///</summary>
    [TestClass, PexClass(typeof(ExpressionResolver))]
    public class TestExpressionResolver
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            LINQToTTreeLib.Utils.TypeUtils._variableNameCounter = 0;
            DummyQueryExectuor.GlobalInitalized = false;
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }


        [PexMethod]
        public Expression RunResolve(Expression source, IGeneratedQueryCode gc, ICodeContext cc)
        {
            return ExpressionResolver.Resolve(source, gc, cc, MEFUtilities.MEFContainer);
        }

        /// <summary>
        /// Test that when we have a sub-expression it only gets emitted into the code once.
        ///</summary>
        [TestMethod]
        public void TestResolveSubExpression()
        {
            var q = new QueriableDummy<LINQToTTreeLib.QueryVisitorTest.ntupWithObjects>();
            var result1 = from evt in q
                          where (from jet in evt.jets where jet.var1 > 1.0 select jet).Count() == 1
                          select evt;
            var c = result1.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            // Extract the code and count the number of loops. There should be just one for that "where" sub-expression.

            var code = DummyQueryExectuor.FinalResult.CodeBody.Statements;
            var loopCount = code.Where(s => s is StatementForLoop).Count();
            Assert.AreEqual(1, loopCount, "# of loops incorrect");
        }

        public class SourceType2Container
        {
            [TTreeVariableGrouping]
            public int val;

            [TTreeVariableGrouping]
            public int[] val2D;
        }

        public class SourceType2Container1
        {
            [TTreeVariableGrouping]
            [RenameVariable("val2D")]
            public int[] others;
        }

        [TranslateToClass(typeof(ResultType2))]
        public class SourceType2
        {
            [TTreeVariableGrouping]
            public SourceType2Container[] jets;

            [TTreeVariableGrouping]
            public SourceType2Container1[] others;
        }

        public class ResultType2
        {
            public ResultType2(Expression holder)
            {
            }

            public int[] val;
            public int[][] val2D;
        }

        [TestMethod]
        public void TestObjectArrayCompare()
        {
            MEFUtilities.Compose(new TypeHandlerCache());
            Expression<Func<SourceType2, int, int, bool>> lambaExpr = (s, a1, a2) => s.jets[a1] == s.jets[a2];
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var result = ExpressionResolver.Resolve(lambaExpr.Body, gc, cc, MEFUtilities.MEFContainer);

            Assert.IsInstanceOfType(result, typeof(BinaryExpression), "Expression type");
            Assert.AreEqual(ExpressionType.Equal, result.NodeType, "Expected an equal");

            var b = result as BinaryExpression;
            Assert.IsInstanceOfType(b.Left, typeof(ParameterExpression), "Left expr");
            Assert.IsInstanceOfType(b.Right, typeof(ParameterExpression), "Right expr");

            var r = b.Right as ParameterExpression;
            var l = b.Left as ParameterExpression;

            Assert.AreEqual("a1", l.Name, "Left paramter name");
            Assert.AreEqual("a2", r.Name, "Right paramter name");
        }

        [TestMethod]
        public void TestObjectArrayCompareToNull()
        {
            MEFUtilities.Compose(new TypeHandlerCache());
            Expression<Func<SourceType2, int, int, bool>> lambaExpr = (s, a1, a2) => s.jets[a1] == null;
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var result = ExpressionResolver.Resolve(lambaExpr.Body, gc, cc, MEFUtilities.MEFContainer);

            Assert.IsInstanceOfType(result, typeof(BinaryExpression), "Expression type");
            Assert.AreEqual(ExpressionType.Equal, result.NodeType, "Expected an equal");

            var b = result as BinaryExpression;
            Assert.IsInstanceOfType(b.Left, typeof(ParameterExpression), "Left expr");
            Assert.IsInstanceOfType(b.Right, typeof(ConstantExpression), "Right expr");

            var r = b.Right as ParameterExpression;
            var l = b.Left as ConstantExpression;

            //Assert.Inconclusive("Do we want to allow the user to write this - what does it mean??");
            // The way this get coded up is pretty harmless. So I guess we let it go...
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestObjectArrayToConstNullCompare()
        {
            MEFUtilities.Compose(new TypeHandlerCache());
            Expression<Func<SourceType2, bool>> lambaExpr = (s) => s.jets[0] == null;
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var result = ExpressionResolver.Resolve(lambaExpr.Body, gc, cc, MEFUtilities.MEFContainer);
        }

    }
}
