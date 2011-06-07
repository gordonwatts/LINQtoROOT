using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTreeHelpers.Tests
{


    /// <summary>
    ///This is a test class for TestDataSetFinder and is intended
    ///to contain all TestDataSetFinder Unit Tests
    ///</summary>
    [TestClass()]
    public class TestDataSetFinder
    {
        [TestInitialize]
        public void TestInit()
        {
            /// Don't try to load a ds spec from a default place!
            DataSetFinder.ClearFileList();
            DataSetFinder.ResetCache();
        }

        [TestMethod]
        public void TestTagInDS()
        {
            string dsspec = "machine junk { J1 (ttbar) = *.root }";
            DataSetFinder.ParseSpecFromString(dsspec);
            DataSetFinder.MachineName = "junk";

            var ds = DataSetFinder.DatasetNamesForTag("ttbar");
            Assert.AreEqual(1, ds.Length, "# of datasets");
            Assert.AreEqual("J1", ds[0], "ds name");
        }

        [TestMethod]
        public void TestTagsInDS()
        {
            string dsspec = "machine junk { J1 (ttbar, dude) = *.root }";
            DataSetFinder.ParseSpecFromString(dsspec);
            DataSetFinder.MachineName = "junk";

            var ds = DataSetFinder.DatasetNamesForTag("dude");
            Assert.AreEqual(1, ds.Length, "# of datasets for tag dude");
            Assert.AreEqual("J1", ds[0], "ds name");

            ds = DataSetFinder.DatasetNamesForTag("ttbar");
            Assert.AreEqual(1, ds.Length, "# of datasets for tag ttbar");

            ds = DataSetFinder.DatasetNamesForTag("ttbar", "dude");
            Assert.AreEqual(1, ds.Length, "# of datasets for tag ttbar and dude");

        }
    }
}
