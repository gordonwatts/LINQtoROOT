using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.ResultOperators;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for ROSumTest and is intended
    ///to contain all ROSumTest Unit Tests
    ///</summary>
    [TestClass()]
    [PexClass]
    public partial class ROSumTest
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            MEFUtilities.AddPart(new TypeHandlers.TypeHandlerConvert());
            MEFUtilities.AddPart(new ROSum());
            DummyQueryExectuor.GlobalInitalized = false;
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>
        ///A test for ROSum Constructor
        ///</summary>
        [TestMethod()]
        public void ROSumConstructorTest()
        {
            ROSum target = new ROSum();
        }

        [PexMethod]
        public bool TestCanHandle([PexAssumeUnderTest] ROSum target, Type obj)
        {
            var value = target.CanHandle(obj);
            Assert.AreEqual(obj == typeof(SumResultOperator), value, "incorrect return!");

            return value;
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public IVariable TestProcessResultOperator([PexAssumeUnderTest]ROSum target,
            SumResultOperator sumro,
            QueryModel qm,
            IGeneratedQueryCode gc,
            ICodeContext cc)
        {
            var result = target.ProcessResultOperator(sumro, qm, gc, cc, MEFUtilities.MEFContainer);
            return result;
        }

        [TestMethod]
        public void TestSimpleSum()
        {
            var q = new QueriableDummy<ntup>();
            var s = q.Sum(i => i.run);

            var result = DummyQueryExectuor.FinalResult;
            result.DumpCodeToConsole();

            //
            // Look for the assignment operator in the loop
            //

            var toplevel = result.CodeBody.Statements;
            var assign = toplevel.First() as LINQToTTreeLib.Statements.StatementAssign;
            Assert.IsNotNull(assign, "Assign statement missing");

            Assert.IsTrue(assign.Expression.RawValue.Contains("+"), "the plus sign is missing");
        }
    }
}
