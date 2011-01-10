// <copyright file="TTreeQueryExecutorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>

using System;
using System.IO;
using System.Linq;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib
{
    [TestClass]
    [PexClass(typeof(TTreeQueryExecutor))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class TTreeQueryExecutorTest
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        [PexMethod]
        public TTreeQueryExecutor Constructor(int rootFileIndex, string treeName)
        {
            FileInfo rootFile;
            switch (rootFileIndex)
            {
                case 0:
                    rootFile = null;
                    break;

                case 1:
                    rootFile = new FileInfo("stupid.root");
                    break;

                case 2:
                    rootFile = new FileInfo(@"..\..\..\..\DemosAndTests\output.root");
                    break;

                default:
                    rootFile = null;
                    break;
            }

            TTreeQueryExecutor target = new TTreeQueryExecutor(rootFile, treeName);
            return target;
            // TODO: add assertions to method TTreeQueryExecutorTest.Constructor(FileInfo, String)
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupe
        {
            int run;
        }

        /// <summary>
        /// Test out a simple result operator.
        /// </summary>
        [TestMethod]
        public void TestSimpleReultOperator()
        {
            int numberOfIter = 10;

            var rootFile = CreateFileOfInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(rootFile, "dude");
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestTempDirectoryLocationAndEmptying()
        {
            var rootFile = CreateFileOfInt(1);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(rootFile, "dude");
            int result = exe.ExecuteScalar<int>(query);

            DirectoryInfo dir = TTreeQueryExecutor.TempDirectory;
            dir.Refresh();
            Assert.IsTrue(dir.Exists, "Temp directory doesn't exist");
            Assert.AreEqual(0, dir.EnumerateFiles().Count(), "Expected no spare files in there!");
            Assert.AreEqual(1, dir.EnumerateDirectories().Count(), "Incorrect # of subdirectories");
            Assert.AreEqual("CommonFiles", dir.GetDirectories()[0].Name, "incorrect name of single existing directory");
        }

        /// <summary>
        /// Create an output int file... unique so we don't have to regenerate...
        /// </summary>
        /// <param name="numberOfIter"></param>
        /// <returns></returns>
        private FileInfo CreateFileOfInt(int numberOfIter)
        {
            string filename = "intonly_" + numberOfIter.ToString() + ".root";
            FileInfo result = new FileInfo(filename);
            if (result.Exists)
                return result;

            var f = new ROOTNET.NTFile(filename, "RECREATE");
            var tree = TTreeParserCPPTests.CreateTrees.CreateOneIntTree(numberOfIter);
            f.Write();
            f.Close();
            result.Refresh();
            return result;
        }
    }
}
