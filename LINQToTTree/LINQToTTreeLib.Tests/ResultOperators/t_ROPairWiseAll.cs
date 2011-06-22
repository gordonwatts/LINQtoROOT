using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.relinq;
using LINQToTTreeLib.ResultOperators;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Microsoft.Pex.Framework.Validation;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for ROPairWiseAllTest and is intended
    ///to contain all ROPairWiseAllTest Unit Tests
    ///</summary>
    [TestClass]
    [PexClass(typeof(ROUniqueCombinations))]
    public partial class ROPairWiseAllTest
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

        /// <summary>
        ///A test for ROPairWiseAll Constructor
        ///</summary>
        [TestMethod]
        public void ROPairWiseAllConstructorTest()
        {
            ROPairWiseAll target = new ROPairWiseAll();
        }

        /// <summary>
        ///A test for CanHandle
        ///</summary>
        [PexMethod]
        internal bool CanHandle(
            [PexAssumeUnderTest]ROUniqueCombinations target,
            Type resultOperatorType
        )
        {
            bool result = target.CanHandle(resultOperatorType);

            Assert.AreEqual(resultOperatorType == typeof(PairWiseAllResultOperator), result, "bad return from CanHandle");

            return result;
        }

        /// <summary>
        ///A test for ProcessResultOperator
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        internal IVariable ProcessResultOperator(
            [PexAssumeUnderTest]ROUniqueCombinations target,
            ResultOperatorBase resultOperator,
            QueryModel queryModel,
            CodeContext cc,
            [PexAssumeNotNull]GeneratedCode codeEnv
        )
        {
            target.ProcessResultOperator(resultOperator, queryModel, codeEnv, cc, null);
            return null;
        }

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
                          select evt.run.PairWiseAll((r1, r2) => r1 != r2).Count();
            var total = results.Aggregate(0, (seed, val) => seed + val);
            //var total = results.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "expecing some code to have been generated");
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

#if false
            var code = DummyQueryExectuor.FinalResult.CodeBody as IBookingStatementBlock;
            Assert.AreEqual(4, code.Statements.Count(), "# fo statements");
            var innerloop = code.Statements.Skip(1).First() as IBookingStatementBlock;
            Assert.IsNotNull(innerloop, "inner loop");

            Assert.AreEqual(1, innerloop.Statements.Count(), "# of statements in the inner loop - the push statement");

            var last = code.Statements.Skip(2).First();
            Assert.IsInstanceOfType(last, typeof(LINQToTTreeLib.Statements.StatementPairLoop), "last statement incorrect");

            var res = DummyQueryExectuor.FinalResult.ResultValue;
            Assert.IsNotNull(res, "final result");
            Assert.AreEqual(typeof(int), res.Type, "final result type");
#endif

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
    }
}
