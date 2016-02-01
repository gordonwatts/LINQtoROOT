using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LINQToTTreeLib.Files;
using System.Threading.Tasks;

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

            // Check that we have a cout somewhere in the statemnt.
            Assert.IsTrue(query1.DumpCode().Where(l => l.Contains("->Fill()")).Any(), "At least one Fill statement.");
        }

        #region Test Query Generation
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
                .Select(e => new ourCustomObject() { col1 = e.run, col2 = (int)e.run, col3 = e.run })
                .AsTTree(new FileInfo("hi.root"));
        }

        #endregion
    }
}
