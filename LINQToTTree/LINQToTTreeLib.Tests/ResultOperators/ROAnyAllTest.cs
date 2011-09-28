// <copyright file="ROAnyAllTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>This class contains parameterized unit tests for ROAnyAll</summary>
    [PexClass(typeof(ROAnyAll))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ROAnyAllTest
    {
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

        /// <summary>Test stub for CanHandle(Type)</summary>
        [PexMethod]
        internal bool CanHandle([PexAssumeUnderTest]ROAnyAll target, Type resultOperatorType)
        {
            bool result = target.CanHandle(resultOperatorType);
            return result;
            // TODO: add assertions to method ROAnyAllTest.CanHandle(ROAnyAll, Type)
        }

        /// <summary>Test stub for ProcessResultOperator(ResultOperatorBase, QueryModel, IGeneratedQueryCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod]
        internal Expression ProcessResultOperator(
            [PexAssumeUnderTest]ROAnyAll target,
            ResultOperatorBase resultOperator,
            QueryModel queryModel,
            IGeneratedQueryCode _codeEnv,
            ICodeContext _codeContext,
            CompositionContainer container
        )
        {
            Expression result = target.ProcessResultOperator
                                   (resultOperator, queryModel, _codeEnv, _codeContext, container);
            return result;
            // TODO: add assertions to method ROAnyAllTest.ProcessResultOperator(ROAnyAll, ResultOperatorBase, QueryModel, IGeneratedQueryCode, ICodeContext, CompositionContainer)
        }

        [TestMethod]
        public void TestSimpleAny()
        {
            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         select d.run;
            var dude = result.Any(c => c > 5);

            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();

            Assert.AreEqual(0, res.CodeBody.DeclaredVariables.Count(), "# declared");
        }

        class ntup2
        {
#pragma warning disable 0649
            public int[] run;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestNestedAny()
        {
            var q = new QueriableDummy<ntup2>();
            var result = from evt in q
                         where (evt.run.Any(c => c > 5))
                         select evt;
            var dude = result.Count();

            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestSimpleAll()
        {
            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         select d.run;
            var dude = result.All(c => c > 5);

            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();
        }
    }
}
