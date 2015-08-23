using System;
using System.Linq;
using System.Linq.Expressions;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.ResultOperators;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib
{
    [TestClass]
    public partial class ROCountTest
    {
        [TestInitialize]
        public void Setup()
        {
            TestUtils.ResetLINQLibrary();
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

#if false
        [PexMethod]
        [PexUseType(typeof(GeneratedCode)), PexAllowedException(typeof(InvalidOperationException))]
        internal Expression ProcessResultOperator(
            [PexAssumeUnderTest]ROCount target,
            CountResultOperator resultOperator,
            QueryModel queryModel,
            GeneratedCode codeEnv
        )
        {
            int origCount = 0;
            if (codeEnv != null)
                origCount = codeEnv.CodeBody.Statements.Count();
            CodeContext c = new CodeContext();
            Expression result = target.ProcessResultOperator(resultOperator, queryModel, codeEnv, c, null);
            Assert.AreEqual(origCount + 1, codeEnv.CodeBody.Statements.Count(), "Expected an added statement!");
            Assert.IsInstanceOfType(codeEnv.CodeBody.Statements.Last(), typeof(StatementAggregate), "Statement to inc the integer must have been done!");
            Assert.AreEqual(result.Type, typeof(int), "Expected to be calculating an integer");
            return result;
        }

        [PexMethod]
        internal bool CanHandle([PexAssumeUnderTest]ROCount target, Type resultOperatorType)
        {
            bool result = target.CanHandle(resultOperatorType);
            Assert.IsTrue(result || resultOperatorType != typeof(CountResultOperator), "Bad response!");
            return result;
        }
#endif

        public class ntup2
        {
            public int[] run;
        }

        [TestMethod]
        public void TestSimpleCountReturn()
        {
            var q = new QueriableDummy<ntup2>();
            var result = q.Count();
            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "final result");
            var r = DummyQueryExectuor.FinalResult.ResultValue;
            Assert.AreEqual(typeof(int), r.Type, "result type");
            Assert.AreEqual(DeclarableParameter.ExpressionType, r.NodeType, "Expression type incorrect");
            var dv = r as DeclarableParameter;
            Assert.IsTrue(dv.InitialValue == null || dv.InitialValue.RawValue == "0", "Initial value incorrect");
        }
    }
}
