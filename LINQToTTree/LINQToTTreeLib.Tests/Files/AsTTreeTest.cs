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
using System.Reflection;
using LINQToTTreeLib.Expressions;

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
        public void QueryAnonymousObjectToTTree()
        {
            GeneratedCode query1 = GeneratedCodeFor(QueryTupleAnonyoumsObject);

            // Check that we have a Fill somewhere in the statement.
            Assert.IsTrue(query1.DumpCode().Where(l => l.Contains("->Fill()")).Any(), "At least one Fill statement.");
        }
        
        [TestMethod]
        public void QuerySimple()
        {
            var q1 = GeneratedCodeFor(ASimpleQuery);

        }

        [TestMethod]
        public void TupleTitleAndItemsForAnonymousObject()
        {
            var f = NTFile.Open(RunQueryForSingleColumnTTree(QueryTupleOurCustomObject).FullName, "READ");
            try
            {
                var t = f.Get("DataTree") as ROOTNET.Interface.NTTree;
                var blist = t.ListOfBranches.Select(b => b.Name).ToArray();
                Assert.AreEqual(3, blist.Length);
                Assert.IsTrue(blist.Contains("col1"));
                Assert.IsTrue(blist.Contains("col2"));
                Assert.IsTrue(blist.Contains("col3"));
            }
            finally
            {
                f.Close();
            }

        }

        [TestMethod]
        public void TupleSetTitleAndItems()
        {
            FileInfo result = RunQueryForSingleColumnTTree(QueryTupleOurCustomObjectTitleAndNameDefaultFile);

            Assert.IsTrue(result.Exists, "resulting file does not exist");
            var f = NTFile.Open(result.FullName);
            try
            {
                var t = f.Get("PhysicsTree") as ROOTNET.Interface.NTTree;
                Assert.IsNotNull(t, "Getting a tree from the file");

                Assert.AreEqual("PhysicsTree", t.Name);
                Assert.AreEqual("Data that we've written for a physics tree", t.Title);

            }
            finally
            {
                f.Close();
            }
        }

        [TestMethod]
        public void TupleCustomLeafNames()
        {
            FileInfo result = RunQueryForSingleColumnTTree(QueryTupleOurCustomObjectCustomLeafNames);

            Assert.IsTrue(result.Exists, "File exists");

            // Check the contents of the file.
            var f = NTFile.Open(result.FullName);
            try
            {
                var t = f.Get("DataTree") as ROOTNET.Interface.NTTree;
                Assert.IsNotNull(t, "Getting a tree from the file");

                var leaves = t.ListOfLeaves;
                Assert.AreEqual(3, leaves.Entries, "Number of leaves");
                Assert.AreEqual("c1", leaves[0].Name);
                Assert.AreEqual("c2", leaves[1].Name);
                Assert.AreEqual("col3", leaves[2].Name);
            }
            finally
            {
                f.Close();
            }
        }

        [TestMethod]
        public void BadTypeInNtuple()
        {
            try
            {
                FileInfo result = RunQueryForSingleColumnTTree(QueryTupleWithBadType);
                Assert.Fail("Shoudl have thrown");
            } catch (TargetInvocationException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(BadTypeForFileOrTreeTranslationException));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BadPropertyReferenceException))]
        public void MissingVariableInCustomObject()
        {
            var result = RunQueryForSingleColumnTTree(QueryTupleOurCustomObjectMissingVar);
        }

        [TestMethod]
        public void AllTypesPossible()
        {
            var result = RunQueryForSingleColumnTTree(QueryTupleAllTypes);
        }

        [TestMethod]
        public void TupleStreamCompiled()
        {
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
                var t = f.Get("DataTree") as ROOTNET.Interface.NTTree;
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

        [TestMethod]
        public void TTreeWithZeroEntries()
        {
            // Test a full round trip for a really simple CSV dump.
            var rootFile = TestUtils.CreateFileOfInt(10);

            // Simple query that will return nothing.
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => new customObjectAllValidTypes() { vDouble = (double)e.run, vBool = e.run == 10, vInt = (int)e.run })
                .Where(e => false)
                .AsTTree(outputROOTFile: new FileInfo("allguys.root"));
            var query = DummyQueryExectuor.LastQueryModel;

            // Run it.
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(singleIntNtuple));
            exe.CleanupQuery = false;
            var result = exe.ExecuteScalar<FileInfo[]>(query);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);

            var fout = ROOTNET.NTFile.Open(result[0].FullName, "READ");
            var t = fout.Get("DataTree") as NTTree;
            Assert.IsNotNull(t);
            Assert.AreEqual(0, t.GetEntries());
        }

        #region Test Query Generation
        private static FileInfo RunQueryForSingleColumnTTree(Action queryBuilder)
        {
            // Test a full round trip for a really simple CSV dump.
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Get a simple query we can "play" with
            /// 

            var query = QueryModelFor(queryBuilder);

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(singleIntNtuple));
            exe.CleanupQuery = false;
            var result = exe.ExecuteScalar<FileInfo[]>(query);
            Assert.AreEqual(1, result.Length);
            return result[0];
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
                .AsTTree(outputROOTFile: new FileInfo("hi.root"));
        }

        /// <summary>
        /// We forget to initalize a varaible. Use to make sure the system gives a good
        /// error so the user can figure out what is going on.
        /// </summary>
        private static void QueryTupleOurCustomObjectMissingVar()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => new ourCustomObject() { col2 = (int)e.run + 1, col3 = e.run + 2 })
                .AsTTree(outputROOTFile: new FileInfo("hi.root"));
        }

        /// <summary>
        /// Create a query with an anonymous object.
        /// </summary>
        private static void QueryTupleAnonyoumsObject()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => new { col2 = (int)e.run + 1, col3 = e.run + 2, col1 = e.run})
                .AsTTree(outputROOTFile: new FileInfo("hi.root"));
        }

        /// <summary>
        /// Add each type here that we should be able to put into a csv or a TTree.
        /// </summary>
        class customObjectAllValidTypes
        {
            public double vDouble;
            public int vInt;
            public bool vBool;
        }

        /// <summary>
        /// Generage a query that fills all variable type.
        /// </summary>
        private static void QueryTupleAllTypes()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => new customObjectAllValidTypes() { vDouble = (double)e.run, vBool = e.run == 10, vInt = (int)e.run })
                .AsTTree(outputROOTFile: new FileInfo("allguys.root"));
        }

        /// <summary>
        /// Simple pass-through request
        /// </summary>
        private static void ASimpleQuery()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .AsTTree(outputROOTFile: new FileInfo("hi.root"));
        }

        /// <summary>
        /// Reset the leaf names
        /// </summary>
        private static void QueryTupleOurCustomObjectCustomLeafNames()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => new ourCustomObject() { col1 = e.run, col2 = (int)e.run + 1, col3 = e.run + 2 })
                .AsTTree("DataTree", "this is a test", null, "c1", "c2");
        }

        class customObjectWithByte
        {
            public byte Var1;
        }
        /// <summary>
        /// Generate a tree against a byte, a type that we don't currently support.
        /// </summary>
        private static void QueryTupleWithBadType()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => new customObjectWithByte() { Var1 = (byte)(e.run + 1) })
                .AsTTree("DataTree", "not going to happen", null, "c1", "c2");
        }

        /// <summary>
        /// Simple query for a customized object.
        /// </summary>
        private static void QueryTupleOurCustomObjectTitleAndNameDefaultFile()
        {
            var q = new QueriableDummy<singleIntNtuple>();
            q
                .Select(e => new ourCustomObject() { col1 = e.run, col2 = (int)e.run + 1, col3 = e.run + 2 })
                .AsTTree("PhysicsTree", "Data that we've written for a physics tree");
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
