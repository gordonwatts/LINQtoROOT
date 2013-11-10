using LINQToTTreeLib.QMFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LINQToTTreeLib.Tests.QMFunctions
{
    /// <summary>
    /// Test the QM Function Finder and Resolvers.
    /// </summary>
    [TestClass]
    public class QMFuncFinderTest
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

        [TestMethod]
        public void TestNoneQMFunc()
        {
            var q = new QueriableDummy<ntup>();
            var r1 = q.Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(0, sf.Count(), "# of qm functions");
        }

        [TestMethod]
        public void TestSingleQMFuns()
        {
            var q = new QueriableDummy<dummyntup>();
            var a = from evt in q
                    where evt.valC1D.Where(i => i > 5).Count() > 5
                    select evt;
            var r1 = a.Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(1, sf.Count(), "# of qm functions");
        }
    }
}
