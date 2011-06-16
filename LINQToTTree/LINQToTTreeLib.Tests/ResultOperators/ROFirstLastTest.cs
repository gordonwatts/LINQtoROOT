// <copyright file="ROFirstLastTest.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
using System;
using System.ComponentModel.Composition.Hosting;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>This class contains parameterized unit tests for ROFirstLast</summary>
    [PexClass(typeof(ROFirstLast))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ROFirstLastTest
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;
            QueryResultCacheTest.SetupCacheDir();

            MEFUtilities.AddPart(new TypeHandlers.TypeHandlerConvert());
            MEFUtilities.AddPart(new ROFirstLast());
            MEFUtilities.AddPart(new ROAggregate());
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>Test stub for CanHandle(Type)</summary>
        [PexMethod]
        public bool CanHandle([PexAssumeUnderTest]ROFirstLast target, Type resultOperatorType)
        {
            bool result = target.CanHandle(resultOperatorType);
            return result;
            // TODO: add assertions to method ROFirstLastTest.CanHandle(ROFirstLast, Type)
        }

        /// <summary>Test stub for ProcessResultOperator(ResultOperatorBase, QueryModel, IGeneratedCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod, PexAllowedException(typeof(NotImplementedException))]
        public IVariable ProcessResultOperator(
            [PexAssumeUnderTest]ROFirstLast target,
            ResultOperatorBase resultOperator,
            QueryModel queryModel,
            IGeneratedQueryCode _codeEnv,
            ICodeContext _codeContext,
            CompositionContainer container
        )
        {
            IVariable result = target.ProcessResultOperator
                                   (resultOperator, queryModel, _codeEnv, _codeContext, container);
            return result;
            // TODO: add assertions to method ROFirstLastTest.ProcessResultOperator(ROFirstLast, ResultOperatorBase, QueryModel, IGeneratedCode, ICodeContext, CompositionContainer)
        }
#if false
        public class ntup2
        {
            public int[] run;
        }

        [TestMethod]
        public void TestSimpleFirst()
        {
            var q = new QueriableDummy<ntup2>();

            var result = from evt in q
                         where evt.run.First() > 10
                         select evt;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();

            Assert.Inconclusive();
        }

        [TranslateToClass(typeof(ResultType1))]
        class SourceType1
        {
#pragma warning disable 0649
            [TTreeVariableGrouping]
            public SourceType1SubType[] jets;
#pragma warning restore 0649
        }

        class SourceType1SubType
        {
#pragma warning disable 0649
            [TTreeVariableGrouping]
            public int val1;
#pragma warning restore 0649
        }

        class ResultType1 : IExpressionHolder
        {
            public ResultType1(Expression holder)
            { HeldExpression = holder; }

#pragma warning disable 0649
            public int[] val1;
#pragma warning restore 0649

            public Expression HeldExpression
            {
                get;
                private set;
            }
        }

        [TestMethod]
        public void TestTranslatedObjectFirst()
        {
            var q = new QueriableDummy<SourceType1>();

            var result = from evt in q
                         where (evt.jets.First().val1 > 5)
                         select evt;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();

            Assert.Inconclusive();
        }
#endif
    }
}
