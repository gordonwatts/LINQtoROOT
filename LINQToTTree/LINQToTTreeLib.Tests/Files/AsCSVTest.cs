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
    /// Test various AsCSV extensions to make sure they work OK.
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
            GeneratedCode query1 = GeneratedCodeFor(QuerySimpleSingleRun);

            // Check that we have a cout somewhere in the statement.
            Assert.IsTrue(query1.DumpCode().Where(l => l.Contains("<<") && l.Contains(".run")).Any(), "At least one cout statement.");
        }

        [TestMethod]
        public void TupleDoubleStreamToCSVFileGeneratesOutput()
        {
            GeneratedCode query1 = GeneratedCodeFor(QueryTupleTwoDoubles);

            // Check that we have a cout somewhere in the statement.
            Assert.IsTrue(query1.DumpCode().Where(l => l.Contains("<<") && l.Contains(".run")).Any(), "At least one cout statement.");
        }

        [TestMethod]
        public void CustomObjectStreamToCSVFileGeneratesOutput()
        {
            GeneratedCode query1 = GeneratedCodeFor(QueryTupleOurCustomObject);

            // Check that we have a cout somewhere in the statement.
            Assert.IsTrue(query1.DumpCode().Where(l => l.Contains("<<") && l.Contains(".run")).Any(), "At least one cout statement.");
        }

        [TestMethod]
        public void CustomObjectInsideTuple()
        {
            var query1 = GeneratedCodeFor(QueryTupleOurCustomObjectWithTuple);
            Assert.IsTrue(query1.DumpCode().Where(l => l.Contains("<<") && l.Contains(".run")).Any(), "At least one cout statement.");
        }

        [TestMethod]
        public void AsCSVSetsResultVariable()
        {
            // Check that the QM returns a "good" result.
            var gc = GeneratedCodeFor(QuerySimpleSingleRun);
            Assert.IsNotNull(gc.ResultValue, "Result Value should be a FileInfo");
        }

        [TestMethod]
        public void AsCSVIncludeFiles()
        {
            var gc = GeneratedCodeFor(QuerySimpleSingleRun);
            Assert.IsTrue(gc.IncludeFiles.Contains("<fstream>"), "Output file stream header missing");
        }

        [TestMethod]
        public void AsCSVInitialValueProper()
        {
            // Check that the QM returns a "good" result.
            var gc = GeneratedCodeFor(QuerySimpleSingleRun);
            Assert.IsNotNull(gc.ResultValueAsVaraible);
        }

        [TestMethod]
        public void AsCSVCanDoAnonymousType()
        {
            // If this crashes, then bad things!
            var gc = RunQueryForSingleColumnTTree(QuerySimpleSingleRunWithCustomObject);

            // Look for Col1 and Col2 in the resulting text, and make sure Col2 comes first (so decl order is preserved).
            Assert.AreEqual(1, gc.Length);
            var lines = gc[0].ReadAllLines().ToArray();
            Assert.AreEqual("Col2, Col1", lines[0]);
        }

        [TestMethod]
        public void AsCSVUniqueFileComesBack()
        {
            // Each file has to include a query hash so we can be sure that it is unique. This
            // mostly matters when a Concat is involved.
            var gc = RunQueryForSingleColumnTTree(QuerySimpleSingleRun);
            Assert.AreEqual(1, gc.Length);
            Assert.AreNotEqual("hi.csv", gc[0].Name);
            Assert.IsTrue(gc[0].Name.StartsWith("hi"));
            Assert.IsTrue(gc[0].Name.EndsWith(".csv"));
        }

        [TestMethod]
        public void AlreadyMadeFileShouldNotBeRemade()
        {
            // Remove file if it exists
            CleanUpFile(new FileInfo("hi.csv"));

            var result1 = RunQueryForSingleColumnTTree(QuerySimpleSingleRun);
            Assert.AreEqual(1, result1.Length);
            result1[0].Refresh();
            Assert.IsTrue(result1[0].Exists, "File exists");
            var modTime = result1[0].LastWriteTime;

            // Run it a second time, and see what happens.
            var result2 = RunQueryForSingleColumnTTree(QuerySimpleSingleRun);
            result2[0].Refresh();
            Assert.AreEqual(modTime, result2[0].LastWriteTime);
        }

        [TestMethod]
        public void DeletedFileRegenerated()
        {
            // Remove file if it exists
            CleanUpFile(new FileInfo("hi.csv"));

            var result1 = RunQueryForSingleColumnTTree(QuerySimpleSingleRun);
            result1[0].Refresh();
            result1[0].Delete();

            // Run it a second time, and see what happens.
            var result2 = RunQueryForSingleColumnTTree(QuerySimpleSingleRun);
            result2[0].Refresh();
            Assert.IsTrue(result2[0].Exists);
        }

        [TestMethod]
        public void FileModifiedToKilLCache()
        {
            // Remove file if it exists
            CleanUpFile(new FileInfo("hi.csv"));

            var result1 = RunQueryForSingleColumnTTree(QuerySimpleSingleRun);
            using (var wr = result1[0].CreateText())
            {
                wr.WriteLine("hi there");
            }

            // Run it a second time, and see what happens.
            var result2 = RunQueryForSingleColumnTTree(QuerySimpleSingleRun);
            result2[0].Refresh();
            var lines = result2[0].ReadAllLines();
            Assert.AreEqual(11, lines.ToArray().Length);
        }

        [TestMethod]
        public void TupleStreamCompiled()
        {
            // Remove file if it exists
            CleanUpFile(new FileInfo("hi.csv"));

            var result = RunQueryForSingleColumnTTree(QueryTupleTwoDoubles);

            Assert.IsTrue(result[0].Exists, "File exists");

            // Check the contents of the resulting file. It should have the 10 lines from the root
            // file plus a column header.
            var lines = result[0].ReadAllLines().ToArray();
            Assert.AreEqual(11, lines.Length);
            Assert.AreEqual("firstCol, second Col", lines[0]);
            Assert.AreEqual("10, 10", lines[1]);
        }

        [TestMethod]
        public void TupleStreamCompiled4()
        {
            // Remove file if it exists
            CleanUpFile(new FileInfo("hi.csv"));

            var result = RunQueryForSingleColumnTTree(QueryTupleFourDoubles);

            Assert.IsTrue(result[0].Exists, "File exists");

            // Check the contents of the resulting file. It should have the 10 lines from the root
            // file plus a column header.
            var lines = result[0].ReadAllLines().ToArray();
            Assert.AreEqual(11, lines.Length);
            Assert.AreEqual("firstCol, second Col, col3, col4", lines[0]);
            Assert.AreEqual("10, 10, 10, 10", lines[1]);
        }

        [TestMethod]
        public void TupleStreamCompiled8()
        {
            // Remove file if it exists
            CleanUpFile(new FileInfo("hi.csv"));

            var result = RunQueryForSingleColumnTTree(QueryTupleSevenDoubles);

            Assert.IsTrue(result[0].Exists, "File exists");

            // Check the contents of the resulting file. It should have the 10 lines from the root
            // file plus a column header.
            var lines = result[0].ReadAllLines().ToArray();
            Assert.AreEqual(11, lines.Length);
            Assert.AreEqual("firstCol, second Col, col3, col4, col5, col6, col7", lines[0]);
            Assert.AreEqual("10, 10, 10, 10, 10, 10, 10", lines[1]);
        }

        [TestMethod]
        public void CustomObjectStreamCompiled()
        {
            // Remove file if it exists
            CleanUpFile(new FileInfo("hi.csv"));

            var result = RunQueryForSingleColumnTTree(QueryTupleOurCustomObject);

            Assert.IsTrue(result[0].Exists, "File exists");

            // Check the contents of the resulting file. It should have the 10 lines from the root
            // file plus a column header.
            var lines = result[0].ReadAllLines().ToArray();
            Assert.AreEqual(11, lines.Length);
            Assert.AreEqual("col1, col2, col3", lines[0]);
            Assert.AreEqual("10, 10, 10", lines[1]);
        }

        private static FileInfo[] RunQueryForSingleColumnTTree(Action queryBuilder)
        {
            // Test a full round trip for a really simple CSV dump.
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var query = QueryModelFor(queryBuilder);

            ///
            /// OK, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(singleIntNtuple));
            var result = exe.ExecuteScalar<FileInfo[]>(query);
            return result;
        }

        /// <summary>
        /// Delete file if it exists.
        /// </summary>
        /// <param name="fileInfo"></param>
        private void CleanUpFile(FileInfo fileInfo)
        {
            // This clean up might be for something that has a cache, so...
            var d = fileInfo.Directory;
            var filesToDelete = d.EnumerateFiles().Where(f => f.Name.StartsWith(fileInfo.Name) && f.Extension == fileInfo.Extension);
            foreach (var f in filesToDelete)
            {
                f.Delete();
            }
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
        private static void QuerySimpleSingleRun()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => e.run)
                .AsCSV(new FileInfo("hi.csv"), "firstCol");
        }

        /// <summary>
        /// Create a AsCSV from an anonymous object.
        /// </summary>
        private static void QuerySimpleSingleRunWithCustomObject()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => new { Col2 = e.run, Col1 = e.run * 2 })
                .AsCSV(new FileInfo("hi.csv"));
        }

        /// <summary>
        /// A tuple with two doubles.
        /// </summary>
        private static void QueryTupleTwoDoubles()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => Tuple.Create(e.run, e.run))
                .AsCSV(new FileInfo("hi.csv"), "firstCol", "second Col");
        }

        /// <summary>
        /// A tuple with two doubles.
        /// </summary>
        private static void QueryTupleFourDoubles()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => Tuple.Create(e.run, e.run, e.run, e.run))
                .AsCSV(new FileInfo("hi.csv"), "firstCol", "second Col", "col3", "col4");

        }

        /// <summary>
        /// A tuple with two doubles.
        /// </summary>
        private static void QueryTupleSevenDoubles()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => Tuple.Create(e.run, e.run, e.run, e.run, e.run, e.run, e.run))
                .AsCSV(new FileInfo("hi.csv"), "firstCol", "second Col", "col3", "col4", "col5", "col6", "col7");
        }

        class ourCustomObject
        {
            public double col1;
            public int col2;
            public double col3;
        }

        private static void QueryTupleOurCustomObject()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => new ourCustomObject() { col1 = e.run, col2 = (int) e.run, col3 = e.run})
                .AsCSV(new FileInfo("hi.csv"));
        }

        private static void QueryTupleOurCustomObjectWithTuple()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => new ourCustomObject() { col1 = e.run, col2 = (int)e.run, col3 = e.run })
                .Select(co => Tuple.Create(co, co.col1, co.col2))
                .AsCSV(new FileInfo("hi.csv"));
        }
        #endregion
    }
}
