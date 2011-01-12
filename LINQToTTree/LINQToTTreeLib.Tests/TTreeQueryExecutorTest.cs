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

        [TestMethod]
        public void TestLoadingCommonFiles()
        {
            /// Create a common C++ object that can be loaded and checked for.
            var d = PrepNonSpaceDir("TestLoadingCommonFiles");
            var f = CreateCommonObject("TestLoadingCommonFilesObj", d);

            ///
            /// Run a simple query - but fool it against another ntuple
            /// 

            var rootFile = CreateFileOfInt(1);
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            Assert.Inconclusive();
        }

        /// <summary>
        /// Prepare/create a common directory that we can use that has no spaces in the name!
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        private DirectoryInfo PrepNonSpaceDir(string dirName)
        {
            string currentDir = System.Environment.CurrentDirectory;
            while (currentDir.Contains(' '))
            {
                currentDir = Path.GetDirectoryName(currentDir);
            }

            ///
            /// If this went all the way back to their user name - that blows. Just bomb and
            /// we will address this when it is time.
            /// 

            if (currentDir.Length < 10)
                Assert.Fail("Something is wrong with the directory - please fix the test harness!");

            var d = new DirectoryInfo(currentDir + "\\" + dirName);
            if (d.Exists)
            {
                d.Delete(true);
            }
            d.Create();
            d.Refresh();

            return d;
        }

        /// <summary>
        /// Create a file that can be loaded by ACLIC
        /// </summary>
        /// <param name="objName"></param>
        /// <returns></returns>
        private FileInfo CreateCommonObject(string objName, DirectoryInfo baseDir)
        {
            FileInfo result = new FileInfo(baseDir + "\\" + objName + ".cpp");
            using (var writer = result.CreateText())
            {
                writer.WriteLine("#include \"{0}.hpp\"", objName);
                writer.WriteLine("ClassImp({0});", objName);
                writer.Close();
            }

            FileInfo resulth = new FileInfo(baseDir + "\\" + objName + ".hpp");
            using (var writer = resulth.CreateText())
            {
                writer.WriteLine("#ifndef __{0}__", objName);
                writer.WriteLine("#define __{0}__", objName);
                writer.WriteLine("#include <TObject.h>");
                writer.WriteLine("class {0} : public TObject {{", objName);
                writer.WriteLine("public:");
                writer.WriteLine("  int j;");
                writer.WriteLine("  ClassDef({0},2);", objName);
                writer.WriteLine("};");
                writer.WriteLine("#endif");
                writer.WriteLine();

                writer.Close();
            }

            return result;
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
