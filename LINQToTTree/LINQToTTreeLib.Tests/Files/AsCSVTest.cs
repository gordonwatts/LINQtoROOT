using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQToTTreeLib.Files;
using System.IO;

namespace LINQToTTreeLib.Tests.Files
{
    /// <summary>
    /// Test various AsCSV extensions to make sure they work ok.
    /// </summary>
    [TestClass]
    public class AsCSVTest
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

        /// <summary>
        /// Very simple ntuple for testing.
        /// </summary>
        public class singleIntNtuple
        {
            public double run;
        }

        [TestMethod]
        public void SingleDoubleStreamToCSVFile()
        {
            var q = new QueriableDummy<singleIntNtuple>();

            q
                .Select(e => e.run)
                .AsCSV(new FileInfo("hi.csv"), "firstCol");

            var query1 = DummyQueryExectuor.FinalResult;
            Assert.IsNotNull(query1);

            query1.DumpCodeToConsole();

            Assert.Inconclusive();
        }
    }
}
