using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LINQToTTreeLib.Files;
using System.Threading.Tasks;
using Remotion.Linq;
using ROOTNET;

using static System.Console;

namespace LINQToTTreeLib.Tests.Files
{
    /// <summary>
    /// Tests for writing out a ROOT file in sequence.
    /// Since the code is shared between AsTTree and AsCVS we don't actually
    /// have that much to test here!
    /// </summary>
    [TestClass]
    public class AsTTreeTest
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
        public void QueryCustomObjectToTTree()
        {
            GeneratedCode query1 = GeneratedCodeFor(QueryTupleOurCustomObject);

            // Check that we have a Fill somewhere in the statement.
            Assert.IsTrue(query1.DumpCode().Where(l => l.Contains("->Fill()")).Any(), "At least one Fill statement.");
        }

        [TestMethod]
        public void TupleStreamCompiled()
        {
            // Remove file if it exists
            CleanUpFile(new FileInfo("hi.csv"));

            FileInfo result = RunQueryForSingleColumnTTree(QueryTupleOurCustomObject);

            Console.WriteLine(result.FullName);
            Assert.IsTrue(result.Exists, "File exists");
            Assert.AreNotEqual("hi.root", result.Name);
            Assert.AreEqual(".root", result.Extension);
            Assert.IsTrue(result.Name.StartsWith("hi"));
            Assert.IsFalse(result.FullName.Contains(".."));

            // Check the contents of the file. It should have a Tree in it with 10 entries.
            var f = NTFile.Open(result.FullName);
            try {
                var t = f.Get("mytree") as ROOTNET.Interface.NTTree;
                Assert.IsNotNull(t, "Getting a tree from the file");
                Assert.AreEqual(10, t.Entries);

                var leaves = t.ListOfLeaves;
                foreach (var leaf in leaves.Select(l => l.Name))
                {
                    WriteLine($"Leave name: {leaf}");
                }
                Assert.AreEqual(3, leaves.Entries, "Number of leaves");

                foreach (dynamic entry in t)
                {
                    WriteLine($"Entry value: col1: {entry.col1} col2: {entry.col2} col3: {entry.col3}");
                    Assert.AreEqual(10, entry.col1);
                    Assert.AreEqual(11, entry.col2);
                    Assert.AreEqual(12, entry.col3);
                }
            } finally
            {
                f.Close();
            }
        }

        #region Test Query Generation
        private static FileInfo RunQueryForSingleColumnTTree(Action queryBuilder)
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
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(singleIntNtuple));
            var result = exe.ExecuteScalar<FileInfo>(query);
            return result;
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
        private static QueryModel QueryModelFor(Action forQuery)
        {
            forQuery();
            return DummyQueryExectuor.LastQueryModel;
        }

        /// <summary>
        /// Very simple ntuple for testing.
        /// </summary>
        public class singleIntNtuple
        {
            public double run;
        }

        /// <summary>
        /// Custom object for writing everything out
        /// </summary>
        class ourCustomObject
        {
            public double col1;
            public int col2;
            public double col3;
        }

        /// <summary>
        /// Simple query for a customized object.
        /// </summary>
        private static void QueryTupleOurCustomObject()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => new ourCustomObject() { col1 = e.run, col2 = (int)e.run + 1, col3 = e.run + 2 })
                .AsTTree(new FileInfo("hi.root"));
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


        #endregion
    }
}
