using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.relinq;
using LINQToTTreeLib.ResultOperators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Tests.ResultOperators
{
    [TestClass]
    public partial class ROUniqueCombinationsTest
    {
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

#if false
        [PexMethod]
        internal bool CanHandle(
            [PexAssumeUnderTest]ROUniqueCombinations target,
            Type resultOperatorType
        )
        {
            bool result = target.CanHandle(resultOperatorType);

            if (resultOperatorType == typeof(UniqueCombinationsResultOperator))
                Assert.IsTrue(result, "Should be good to go!");
            else
                Assert.IsFalse(result, "Bad input type - should not have taken it!");

            return result;
        }

        [PexMethod]
        internal void ProcessResultOperator(
            [PexAssumeUnderTest]ROUniqueCombinations target,
            ResultOperatorBase resultOperator,
            QueryModel queryModel,
            CodeContext cc,
            [PexAssumeNotNull]GeneratedCode codeEnv
        )
        {
            target.ProcessResultOperator(resultOperator, queryModel, codeEnv, cc, null);
        }
#endif

        class ntupArray
        {
#pragma warning disable 0649
            public int[] run;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestBasicUniqueCombo()
        {
            var q = new QueriableDummy<ntupArray>();
            var results = from evt in q
                          select evt.run.UniqueCombinations().Count();
            var total = results.Aggregate(0, (seed, val) => seed + val);
            //var total = results.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "expecing some code to have been generated");
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            var code = DummyQueryExectuor.FinalResult.CodeBody as IBookingStatementBlock;
            Assert.AreEqual(1, code.Statements.Count(), "# fo statements");

            Assert.AreEqual(1, DummyQueryExectuor.FinalResult.Functions.Count(), "# of functiosn");
            code = DummyQueryExectuor.FinalResult.Functions.First().StatementBlock as IBookingStatementBlock;
            var innerloop = code.Statements.Skip(2).First() as IBookingStatementBlock;
            Assert.IsNotNull(innerloop, "inner loop");

            Assert.AreEqual(1, innerloop.Statements.Count(), "# of statements in the inner loop - the push statement");

            var last = code.Statements.Skip(3).First();
            Assert.IsInstanceOfType(last, typeof(LINQToTTreeLib.Statements.StatementPairLoop), "last statement incorrect");

            var res = DummyQueryExectuor.FinalResult.ResultValue;
            Assert.IsNotNull(res, "final result");
            Assert.AreEqual(typeof(int), res.Type, "final result type");

        }

        [TestMethod]
        public void TestBasicUniqueComboCheckItemAccess()
        {
            var q = new QueriableDummy<ntupArray>();
            var combos = from evt in q
                         select (from cmb in evt.run.UniqueCombinations()
                                 select cmb.Item1 - cmb.Item2).Aggregate(0, (seed, val) => seed + val);
            var arg = combos.Aggregate(0, (seed, val) => seed + val);

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "expecing some code to have been generated");
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            var res = DummyQueryExectuor.FinalResult.ResultValue;
            Assert.IsNotNull(res, "final result");
            Assert.AreEqual(typeof(int), res.Type, "final result type");
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
        public void TestWithTranslatedTypes()
        {
            var q = new QueriableDummy<SourceType1>();
            var combos = from evt in q
                         select (from cmb in evt.jets.UniqueCombinations()
                                 select cmb.Item1.val1 - cmb.Item2.val1).Aggregate(0, (seed, val) => seed + val);
            var arg = combos.Aggregate(0, (seed, val) => seed + val);

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "expecing some code to have been generated");
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            var res = DummyQueryExectuor.FinalResult.ResultValue;
            Assert.IsNotNull(res, "final result");
            Assert.AreEqual(typeof(int), res.Type, "final result type");
        }

        [TestMethod]
        public void TestWithTranslatedAndAnonTypes()
        {
            var q = new QueriableDummy<SourceType1>();
            var funnyJets = from evt in q
                            select from j in evt.jets
                                   select new
                                   {
                                       Value = j.val1
                                   };
            var combos = from evt in funnyJets
                         select (from cmb in evt.UniqueCombinations()
                                 select cmb.Item1.Value - cmb.Item2.Value).Aggregate(0, (seed, val) => seed + val);
            var arg = combos.Aggregate(0, (seed, value) => seed + value);
            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "expecing some code to have been generated");
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();
        }
    }
}
