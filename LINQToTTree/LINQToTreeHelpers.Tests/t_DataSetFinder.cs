﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void TestComplexDSName()
        {
            string dsspec = "machine junk { J1-1_1 (ttbar) = *.root }";
            DataSetFinder.ParseSpecFromString(dsspec);
            DataSetFinder.MachineName = "junk";

            var ds = DataSetFinder.DatasetNamesForTag("ttbar");
            Assert.AreEqual(1, ds.Length, "# of datasets");
            Assert.AreEqual("J1-1_1", ds[0], "ds name");

        }

        [TestMethod]
        public void TestProofInDS()
        {
            string dsspec = "machine junk { J1 = proof://tev11.washington.edu/LINQToTTreeTestDS }";
            DataSetFinder.ParseSpecFromString(dsspec);
            DataSetFinder.MachineName = "junk";

            var ds = DataSetFinder.FindROOTFilesForDS("J1");
            Assert.AreEqual(1, ds.Length, "# of files found");
            Assert.AreEqual("proof://tev11.washington.edu/LINQToTTreeTestDS", ds[0].OriginalString, "DS name");
        }

        [TestMethod]
        public void TestComplexDSNameWithWildcards()
        {
            string dsspec = "machine junk { ggH12020 = mc11_7TeV.105377.Pythia_HV_ggH_mH120_mVPI20*/*.root.1 }";
            DataSetFinder.ParseSpecFromString(dsspec);
            DataSetFinder.MachineName = "junk";
        }

        [TestMethod]
        public void TestProofInDSProduction()
        {
            string dsspec = @"
machine BOGUS
{
}

machine SEVENUP
{
  macro solDir = ""C:\Users\gwatts\Documents\Visual Studio 2010\Projects\HVAssociatedTests""

  ggH12020p = proof://tev11.phys.washington.edu/HV_ggH_mH120_mVPI20
  testMC = ""$solDir\EVNT-short.root""
}

machine HIGGS
{
  macro solDir = ""C:\Users\gwatts\Documents\ATLAS\Projects\HVAssociatedTests""
  macro dataDir = \\tango.phys.washington.edu\tev-scratch3\users\gwatts\mc\

  testMC = ""$solDir\EVNT-short.root""
  ggH12020 = ""$dataDir\mc11_7TeV.105377.Pythia_HV_ggH_mH120_mVPI20*\*.root.1""
  ggH12020p = ""proof://tev11.washington.edu/HV_ggH_mH120_mVPI20""
}";
            DataSetFinder.ParseSpecFromString(dsspec);
            DataSetFinder.MachineName = "HIGGS";

            var ds = DataSetFinder.FindROOTFilesForDS("ggH12020p");
            Assert.AreEqual(1, ds.Length, "# of files found");
            Assert.AreEqual("proof://tev11.washington.edu/HV_ggH_mH120_mVPI20", ds[0].OriginalString, "DS name");
        }

        [TestMethod]
        [DeploymentItem("dataset-list.txt")]
        public void TestInputFromAFile()
        {
            DataSetFinder.MachineName = "SEVENUP";
            DataSetFinder.AddFile("dataset-list.txt");
            var ds = DataSetFinder.FindROOTFilesForDS("ggH12020p");
            Assert.AreEqual(1, ds.Length, "# of files found");
            Assert.AreEqual("proof://tev11.washington.edu/HV_ggH_mH120_mVPI20", ds[0].OriginalString, "DS name");
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

        [TestMethod]
        public void TestNewLines()
        {
            string dsspec = "machine junk {\n"
                + "J1 (ttbar, dude) = *.root\n"
                + "}";
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

        [TestMethod]
        public void TestWhiteLines()
        {
            string dsspec =
                "  \n"
                + "machine junk {\n"
                + "  \n"
                + "J1 (ttbar, dude) = *.root\n"
                + "  \n"
                + "}"
                + "  \n";
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

        [TestMethod]
        public void TestNewLineAtEnd()
        {
            string dsspec =
                "  \n"
                + "machine junk {\n"
                + "  \n"
                + "J1 (ttbar, dude) = *.root\n"
                + "  \n"
                + "}\n";
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

        [TestMethod]
        public void TestSampleFail()
        {
            string dsspec =
                "machine HIGGS\n"
                + "{\n"
                + "  macro dsloc = \\tango.phys.washington.edu\\tev-scratch3\\users\\btag\\d3pd\n"
                + "  macro localdata = \"D:\\users\\gwatts\\Shared Data\\JetBackToBack_006\"\n"
                + "  macro dataloc = \\tevdisk2\\scratch3\\users\\HV\\JetBackToBack_006\n"
                + " \n"
                + "  JetStream = $dataloc\\user.Gordon.data11_7TeV*\\*.root*\n"
                + "  \n"
                + "  \n"
                + "  \n"
                + "  JetStream = $dataloc\\user.Gordon.data11_7TeV.period*\\*.root*\n"
                + "}\n";

            // Was causing an exceptoin previously!
            DataSetFinder.ParseSpecFromString(dsspec);
        }
    }
}
