// <copyright file="TakeSkipOperatorsTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.ResultOperators;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib.Statements;
using Microsoft.Pex.Framework.Using;
using LINQToTTreeLib.Tests;

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
            IVariable result
               = target.ProcessResultOperator(resultOperator, queryModel, codeEnv);

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

            return result;
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
