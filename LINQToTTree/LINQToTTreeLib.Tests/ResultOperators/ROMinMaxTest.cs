// <copyright file="ROMinMaxTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>This class contains parameterized unit tests for ROMinMax</summary>
    [PexClass(typeof(ROMinMax))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ROMinMaxTest
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
        internal bool CanHandle([PexAssumeUnderTest]ROMinMax target, Type resultOperatorType)
        {
            bool result = target.CanHandle(resultOperatorType);
            return result;
            // TODO: add assertions to method ROMinMaxTest.CanHandle(ROMinMax, Type)
        }

        /// <summary>Test stub for ProcessResultOperator(ResultOperatorBase, QueryModel, IGeneratedQueryCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod]
        internal IVariable ProcessResultOperator(
            [PexAssumeUnderTest]ROMinMax target,
            ResultOperatorBase resultOperator,
            QueryModel queryModel,
            IGeneratedQueryCode gc,
            ICodeContext cc,
            CompositionContainer container
        )
        {
            IVariable result
               = target.ProcessResultOperator(resultOperator, queryModel, gc, cc, container);
            return result;
            // TODO: add assertions to method ROMinMaxTest.ProcessResultOperator(ROMinMax, ResultOperatorBase, QueryModel, IGeneratedQueryCode, ICodeContext, CompositionContainer)
        }

        [TestMethod]
        public void TestMaxOfSimpleSequence()
        {
            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         select d.run;
            var c = result.Max();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();

            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestMaxOfInlineSimpleSequence()
        {
            var q = new QueriableDummy<ntup>();

            var result = q.Select(n => n.run).Max();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();

            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestMinOfSequenceWithIf()
        {
            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         where d.run > 10
                         select d.run;
            var c = result.Max();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();
            Assert.Inconclusive();
        }

        public class ntup2
        {
            public int[] run;
        }

        [TestMethod]
        public void TestMaxInsideOtherExpression()
        {
            var q = new QueriableDummy<ntup2>();

            var result = from evt in q
                         where (from r in evt.run select r).Max() > 10
                         select evt;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();

            Assert.Inconclusive();
        }
    }
}
