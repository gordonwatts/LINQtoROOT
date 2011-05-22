// <copyright file="ROFirstLastTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
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
        [PexMethod]
        public IVariable ProcessResultOperator(
            [PexAssumeUnderTest]ROFirstLast target,
            ResultOperatorBase resultOperator,
            QueryModel queryModel,
            IGeneratedCode _codeEnv,
            ICodeContext _codeContext,
            CompositionContainer container
        )
        {
            IVariable result = target.ProcessResultOperator
                                   (resultOperator, queryModel, _codeEnv, _codeContext, container);
            return result;
            // TODO: add assertions to method ROFirstLastTest.ProcessResultOperator(ROFirstLast, ResultOperatorBase, QueryModel, IGeneratedCode, ICodeContext, CompositionContainer)
        }

        [TranslateToClass(typeof(targetntupBase))]
        public class ntupBase
        {
            [TTreeVariableGrouping]
            public PV[] PVs;
        }

        public class PV
        {
            [TTreeVariableGrouping]
            public int nTracks;
        }

        public class targetntupBase : IExpressionHolder
        {
            public targetntupBase(Expression h)
            {

            }
            public int[] nTracks;

            public System.Linq.Expressions.Expression HeldExpression
            {
                get { throw new NotImplementedException(); }
            }
        }

        [TestMethod]
        public void TestFirstSecondVarReplacement()
        {
            /// A bug encountered outside - we are (were, I hope!) doing something incorrect with
            /// our variable replacement. This re-creates the bug.

            var q = new QueriableDummy<ntupBase>();
            var result = from d in q
                         select d.PVs.First();

            var h = result.Select(pv => pv.nTracks).Plot("hi", "there", 10, 0.0, 10.0);

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            Assert.Inconclusive("not done yet");
        }
    }
}
