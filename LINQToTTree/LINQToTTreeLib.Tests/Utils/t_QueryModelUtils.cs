using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using static LINQToTTreeLib.TTreeQueryExecutorTest;

namespace LINQToTTreeLib.Tests.Utils
{
    [TestClass]
    public class t_QueryModelUtils
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();

            ntuple.Reset();
            ntuple._gCINTLines = null;
            ntuple._gObjectFiles = null;
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }


        [TestMethod]
        public void TopLevelResultOperatorHiddenSkip()
        {
            // A query that should come back with 10 items.
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Take(10).Where(i => i.run > 0).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            Assert.IsTrue(query.HasStatefulOperator());
        }

        [TestMethod]
        public void TopLevelResultOperatorSkip()
        {
            // A query that should come back with 10 items.
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Take(10).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            Assert.IsTrue(query.HasStatefulOperator());
        }

        [TestMethod]
        public void SelectForJetsAndTake()
        {
            // A query that should come back with 10 items.
            var q = new QueriableDummy<TestNtupeArr>();
            var dude = q.SelectMany(e => e.myvectorofint).Take(10).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            Assert.IsTrue(query.HasStatefulOperator());
        }

        [TestMethod]
        public void SelectForJetsAndTakeForOnlySingleEvent()
        {
            // A query that should come back with 10 items.
            var q = new QueriableDummy<TestNtupeArr>();
            var dude = q.Where(e => e.myvectorofint.Take(2).Count() > 2).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            Assert.IsFalse(query.HasStatefulOperator());
        }
    }
}
