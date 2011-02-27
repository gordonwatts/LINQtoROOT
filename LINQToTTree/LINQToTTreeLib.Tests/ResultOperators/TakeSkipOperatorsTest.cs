// <copyright file="TakeSkipOperatorsTest.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
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
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;

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
        [PexUseType(typeof(SkipResultOperator))]
        internal IVariable ProcessResultOperator(
            [PexAssumeUnderTest]ROTakeSkipOperators target,
            ResultOperatorBase resultOperator,
            QueryModel queryModel,
            IGeneratedCode codeEnv
        )
        {
            CodeContext c = new CodeContext();
            c.SetLoopVariable(new VarSimple(typeof(int)));

            IVariable result
               = target.ProcessResultOperator(resultOperator, queryModel, codeEnv, c, null);

            ///
            /// First, there should be a counter now declared and ready to go in the current variable block - which will
            /// be the outter one for this test
            /// 

            Assert.AreEqual(1, codeEnv.CodeBody.DeclaredVariables.Count(), "Expected only 1 variable to be declared");
            Assert.IsInstanceOfType(codeEnv.CodeBody.DeclaredVariables.First(), typeof(VarInteger), "Expected it to be a counter");

            Assert.AreEqual(2, codeEnv.CodeBody.Statements.Count(), "Expected an if block and an increment!");
            Assert.IsInstanceOfType(codeEnv.CodeBody.Statements.First(), typeof(StatementIncrementInteger), "increment statement not found!");
            Assert.IsInstanceOfType(codeEnv.CodeBody.Statements.Skip(1).First(), typeof(StatementIfOnCount), "if statement not found!");

            var s = codeEnv.CodeBody.Statements.Skip(1).First() as StatementIfOnCount;

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
            /// Finally, we should be getting back a sequence operator of some sort
            /// 

            Assert.IsNotNull(result, "Expected a non-null result");
            Assert.IsInstanceOfType(result, typeof(ISequenceAccessor), "It needs to be a sequence accessor of some sort");

            return result;
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
        public void TestTakeSkipInLINQ()
        {
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
