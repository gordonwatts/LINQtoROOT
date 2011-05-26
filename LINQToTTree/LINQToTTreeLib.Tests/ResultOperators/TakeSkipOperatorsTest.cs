// <copyright file="TakeSkipOperatorsTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>This class contains parameterized unit tests for TakeSkipOperators</summary>
    [PexClass(typeof(ROTakeSkipOperators))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class TakeSkipOperatorsTest
    {
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

        /// <summary>Test stub for ProcessResultOperator(ResultOperatorBase, QueryModel, IGeneratedCode)</summary>
        [PexMethod]
        [PexUseType(typeof(TakeResultOperator))]
        [PexUseType(typeof(SkipResultOperator)), PexAllowedException(typeof(ArgumentException))]
        internal IVariable ProcessResultOperator(
            [PexAssumeUnderTest]ROTakeSkipOperators target,
            ResultOperatorBase resultOperator,
            QueryModel queryModel,
            [PexAssumeNotNull]IGeneratedQueryCode codeEnv
        )
        {
            if (codeEnv.ResultValue != null)
                throw new ArgumentException("this should not b enull for this test");

            ///
            /// We always expect to be inside a loop - and depend on it for doing our declares, so add something...
            /// 

            if (codeEnv != null)
                codeEnv.Add(new StatementInlineBlock());

            ///
            /// Get the env setup and run it
            /// 

            CodeContext c = new CodeContext();
            c.SetLoopVariable(Expression.Variable(typeof(int), "d"));

            target.ProcessResultOperator(resultOperator, queryModel, codeEnv, c, null);

            ///
            /// First, there should be a counter now declared and ready to go in the current variable block - which will
            /// be the outter one for this test
            /// 

            Assert.AreEqual(1, codeEnv.CodeBody.DeclaredVariables.Count(), "Expected only 1 variable to be declared");
            Assert.IsInstanceOfType(codeEnv.CodeBody.DeclaredVariables.First(), typeof(VarInteger), "Expected it to be a counter");

            var statements = codeEnv.CodeBody.Statements.First() as StatementInlineBlock;

            Assert.AreEqual(2, statements.Statements.Count(), "Expected an if block and an increment!");
            Assert.IsInstanceOfType(statements.Statements.First(), typeof(StatementIncrementInteger), "increment statement not found!");
            Assert.IsInstanceOfType(statements.Statements.Skip(1).First(), typeof(StatementIfOnCount), "if statement not found!");

            var s = statements.Statements.Skip(1).First() as StatementIfOnCount;

            string count = "";
            if (resultOperator is SkipResultOperator)
            {
                count = (resultOperator as SkipResultOperator).Count.ToString();
            }
            else if (resultOperator is TakeResultOperator)
            {
                count = (resultOperator as TakeResultOperator).Count.ToString();
            }
            Assert.AreEqual(count, s.ValRight.RawValue, "bad count made it through");

            ///
            /// Finally, the current loop variable should be identical, and there should be no result set.
            /// 

            Assert.IsNull(codeEnv.ResultValue, "result value");
            Assert.IsInstanceOfType(c.LoopVariable, typeof(ParameterExpression), "loop variable type");
            var lv = c.LoopVariable as ParameterExpression;
            Assert.AreEqual("d", lv.Name, "loop variable name");

            return null;
        }

        [TestMethod]
        public void TestBasicTakeSkip()
        {
            GeneratedCode gc = new GeneratedCode();
            var skipper = new SkipResultOperator(Expression.Constant(10));
            ProcessResultOperator(new ROTakeSkipOperators(), skipper, null, gc);
        }

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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestTakeSkipInLINQ()
        {
            ///
            /// The below is invalid because the "Take" is at the top level - we are taking only a certian
            /// number of events. That is not legal! So we need to throw when that happens!
            /// 

            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         select d;
            var c = result.Take(1).Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            var e = result.Skip(1).Count();
        }
    }
}
