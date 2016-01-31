using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQToTTreeLib.Files;
using System.IO;
using Remotion.Linq;

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

        [TestMethod]
        public void SingleDoubleStreamToCSVFileGeneratesOutput()
        {
            GeneratedCode query1 = GeneratedCodeFor(SimpleSingleRunQuery);

            // Check that we have a cout somewhere in the statemnt.
            Assert.IsTrue(query1.DumpCode().Where(l => l.Contains("<<") && l.Contains(".run")).Any(), "At least one cout statement.");
        }

        [TestMethod]
        public void AsCSVSetsResultVariable()
        {
            // Check that the QM returns a "good" result.
            var gc = GeneratedCodeFor(SimpleSingleRunQuery);
            Assert.IsNotNull(gc.ResultValue, "Result Value should be a FileInfo");
            Assert.AreEqual(typeof(FileInfo), gc.ResultValue.Type);
        }

        [TestMethod]
        public void AsCSVIncludeFiles()
        {
            var gc = GeneratedCodeFor(SimpleSingleRunQuery);
            Assert.IsTrue(gc.IncludeFiles.Contains("<fstream>"), "Output file stream header missing");
        }

        [TestMethod]
        public void AsCSVInitialValueProper()
        {
            // Check that the QM returns a "good" result.
            var gc = GeneratedCodeFor(SimpleSingleRunQuery);
            Assert.IsNotNull(gc.ResultValueAsVaraible);
            Assert.IsNotNull(gc.ResultValueAsVaraible.InitialValue);
            Assert.IsTrue(gc.ResultValueAsVaraible.InitialValue.RawValue.Contains("hi.csv"), $"Initial value doesn't have hi.csv in it: {gc.ResultValueAsVaraible.InitialValue.RawValue}");
            Assert.IsTrue(gc.ResultValueAsVaraible.InitialValue.RawValue.Contains("\\\\"), "No double back slashes in file path - that seems... odd");
        }

        [TestMethod]
        public void SingleDoubleStreamCompiled()
        {
            // Remove file if it exists
            CleanUpFile(new FileInfo("hi.csv"));

            // Test a full round trip for a really simple CSV dump.
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var query = QueryModelFor(SimpleSingleRunQuery);

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(singleIntNtuple));
            var result = exe.ExecuteScalar<FileInfo>(query);

            Assert.AreEqual("hi.csv", result.Name);
            Assert.IsTrue(result.Exists, "File exists");

            // Check the contents of the resulting file. It should have the 10 lines from the root
            // file plus a column header.
            var lines = result.ReadAllLines().ToArray();
            Assert.AreEqual(11, lines.Length);
            Assert.AreEqual("firstCol", lines[0]);
            Assert.AreEqual("10", lines[1]);
        }

        /// <summary>
        /// Delete file if it exists.
        /// </summary>
        /// <param name="fileInfo"></param>
        private void CleanUpFile(FileInfo fileInfo)
        {
            if (fileInfo.Exists)
                fileInfo.Delete();
        }

        #region Test Query Generation
        /// <summary>
        /// Very simple ntuple for testing.
        /// </summary>
        public class singleIntNtuple
        {
            public double run;
        }

        /// <summary>
        /// Get the generated code for a simple single run query
        /// </summary>
        /// <returns></returns>
        private static GeneratedCode GeneratedCodeFor(Action forQuery)
        {
            forQuery();

            var query1 = DummyQueryExectuor.FinalResult;
            Assert.IsNotNull(query1);
            query1.DumpCodeToConsole();
            return query1;
        }

        /// <summary>
        /// Return the query model for a particular guy
        /// </summary>
        /// <param name="forQuery"></param>
        /// <returns></returns>
        private static QueryModel QueryModelFor (Action forQuery)
        {
            forQuery();
            return DummyQueryExectuor.LastQueryModel;
        }

        /// <summary>
        /// Generate a simple single run query
        /// </summary>
        private static void SimpleSingleRunQuery()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => e.run)
                .AsCSV(new FileInfo("hi.csv"), "firstCol");
        }
        #endregion
    }
}
