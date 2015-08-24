// <copyright file="TakeSkipOperatorsTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq;
using System.Linq.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.TypeHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using LINQToTTreeLib.Expressions;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>This class contains parameterized unit tests for TakeSkipOperators</summary>
    [TestClass]
    public partial class TakeSkipOperatorsTest
    {
#if false
        /// <summary>Test stub for CanHandle(Type)</summary>
        [PexMethod]
        internal bool CanHandle(
            [PexAssumeUnderTest]ROTakeSkipOperators target,
            Type resultOperatorType
        )
        {
            bool result = target.CanHandle(resultOperatorType);

            if (resultOperatorType == typeof(TakeResultOperator)
                || resultOperatorType == typeof(SkipResultOperator))
                Assert.IsTrue(result, "Should be good to go!");
            else
                Assert.IsFalse(result, "Bad input type - should not have taken it!");

            return result;
        }

#endif

        /// <summary>Test stub for ProcessResultOperator(ResultOperatorBase, QueryModel, IGeneratedCode)</summary>
        internal GeneratedCode ProcessResultOperator(
            ROTakeSkipOperators target,
            ResultOperatorBase resultOperator,
            QueryModel queryModel,
            GeneratedCode codeEnv
        )
        {
            if (codeEnv.ResultValue != null)
                throw new ArgumentException("this should not be null for this test");
            if (codeEnv.CodeBody.DeclaredVariables == null)
                throw new ArgumentException("Need this declare variables to be defined");

            ///
            /// We always expect to be inside a loop - and depend on it for doing our declares, so add something...
            /// 

            if (codeEnv != null)
                codeEnv.Add(new StatementInlineBlock());

            ///
            /// Get the environment setup and run it
            /// 

            CodeContext c = new CodeContext();
            c.SetLoopVariable(Expression.Variable(typeof(int), "d"), null);

            target.ProcessResultOperator(resultOperator, queryModel, codeEnv, c, MEFUtilities.MEFContainer);

            ///
            /// First, there should be a counter now declared and ready to go in the current variable block - which will
            /// be the outer one for this test
            /// 

            Assert.AreEqual(1, codeEnv.CodeBody.DeclaredVariables.Count(), "Expected only 1 variable to be declared");
            Assert.IsInstanceOfType(codeEnv.CodeBody.DeclaredVariables.First(), typeof(DeclarableParameter), "Expected it to be a counter");

            var statements = codeEnv.CodeBody.Statements.First() as StatementInlineBlock;

            Assert.AreEqual(1, statements.Statements.Count(), "Expected an if block/increment!");
            Assert.IsInstanceOfType(statements.Statements.First(), typeof(StatementIfOnCount), "if statement not found!");

            var s = statements.Statements.First() as StatementIfOnCount;

            string count = "";
            if (resultOperator is SkipResultOperator)
            {
                count = (resultOperator as SkipResultOperator).Count.ToString();
            }
            else if (resultOperator is TakeResultOperator)
            {
                count = (resultOperator as TakeResultOperator).Count.ToString();
            }
            Assert.AreEqual(count, s.Limit.RawValue, "bad count made it through");

            ///
            /// Finally, the current loop variable should be identical, and there should be no result set.
            /// 

            Assert.IsNull(codeEnv.ResultValue, "result value");
            Assert.IsInstanceOfType(c.LoopVariable, typeof(ParameterExpression), "loop variable type");
            var lv = c.LoopVariable as ParameterExpression;
            Assert.AreEqual("d", lv.Name, "loop variable name");

            //
            // Dump everything and return. To force it out, add a dummy statement
            // (because if statements, etc., are smart enough to not print anything if they
            // are empty).
            //

            codeEnv.Add(new StatementSimpleStatement("fork = left"));
            codeEnv.DumpCodeToConsole();

            return codeEnv;
        }

        [TestMethod]
        public void TestBasicTakeSkip()
        {
            var t = new TypeHandlerCache();
            MEFUtilities.Compose(t);
            GeneratedCode gc = new GeneratedCode();
            gc.Add(new LINQToTTreeLib.Statements.StatementInlineBlock());
            gc.Add(new LINQToTTreeLib.Tests.TestUtils.SimpleLoop());
            var skipper = new SkipResultOperator(Expression.Constant(10));
            ProcessResultOperator(new ROTakeSkipOperators(), skipper, null, gc);
        }

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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTakeSkipInLINQ()
        {
            ///
            /// The below is invalid because the "Take" is at the top level - we are taking only a certain
            /// number of events. That is not legal! So we need to throw when that happens!
            /// 

            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         select d;
            var c = result.Take(1).Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;
        }
    }
}
