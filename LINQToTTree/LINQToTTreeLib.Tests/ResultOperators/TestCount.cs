using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.ResultOperators
{
    [TestClass]
    public class TestCount
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

        [TestMethod]
        public void TestBasicCount()
        {
            Assert.Inconclusive();
#if false
            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         select d;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            /// Return type is correct
            Assert.IsNotNull(res.ResultValue, "Expected a result from the count!");
            Assert.IsInstanceOfType(res.ResultValue, typeof(VarInteger), "integer return type expected");
#endif
        }
    }
}
