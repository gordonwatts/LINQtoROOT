using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.relinq;
using LINQToTTreeLib.ResultOperators;
using LINQToTTreeLib.Statements;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;

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
        }

        [TestMethod]
        public void TestBasicUniqueComboCheckItemAccess()
        {
            var q = new QueriableDummy<ntupArray>();
            var combos = from evt in q
                         select (from cmb in evt.run.PairWiseAll((r1, r2) => r1 != r2)
                                 select cmb).Aggregate(0, (seed, val) => seed + val);
            var arg = combos.Aggregate(0, (seed, val) => seed + val);

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "expecing some code to have been generated");
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            var res = DummyQueryExectuor.FinalResult.ResultValue;
            Assert.IsNotNull(res, "final result");
            Assert.AreEqual(typeof(int), res.Type, "final result type");
        }

        [CPPHelperClass]
        public static class CPPHelperFunctions
        {
            [CPPCode(Code = new string[] { "Calc = arg*2;" })]
            public static int Calc(int arg)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void TestScope()
        {
            // Make sure that if anything funky needs to be done outside our loop,
            // it gets declared correctly.

            var q = new QueriableDummy<ntupArray>();
            var results = from evt in q
                          select evt.run.PairWiseAll((r1, r2) => CPPHelperFunctions.Calc(r1) != r2).Count();
            var total = results.Aggregate(0, (seed, val) => seed + val);
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // The *2 should be burried in the loop. However, we've seen it popped up to the top
            // in a bug. So test for that regression.
            //

            var seen2 = DummyQueryExectuor.FinalResult.CodeBody.Statements.Where(s => s is StatementSimpleStatement).Cast<StatementSimpleStatement>().Where(s => s.ToString().Contains("]*2")).Any();
            Assert.IsFalse(seen2, "Saw a ']*2' in a top level code statement!");
        }

        [TestMethod]
        public void TestAnonObjects()
        {
            var q = new QueriableDummy<ntupArray>();
            var anon = from evt in q
                       select (from r in evt.run
                               select new
                               {
                                   Value = r
                               });
            var results = from evt in anon
                          select evt.PairWiseAll((r1, r2) => r1.Value != r2.Value).Count();
            var tot = results.Aggregate(0, (seed, val) => seed + val);
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();
        }
    }
}
