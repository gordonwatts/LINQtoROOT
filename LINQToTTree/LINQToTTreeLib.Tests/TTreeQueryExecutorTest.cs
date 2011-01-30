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
            ntuple.Reset();

            /// Get the path for the other nutple guy correct!
            DirectoryInfo projectDir = new DirectoryInfo(Environment.CurrentDirectory + @"\..\..\..\..");
            ntuple_with_proxy._gProxyFile = projectDir.FullName + @"\DemosAndTests\GenerateNtupleXMLSpec\ntuple_btag.h";
            Assert.IsTrue(File.Exists(ntuple_with_proxy._gProxyFile), "Proxy file we are using for testing isn't around!");
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        [PexMethod, PexAllowedException(typeof(FileNotFoundException))]
        public TTreeQueryExecutor Constructor(int rootFileIndex, int ntupleProxyIndex, int ntupleExtraIndex, string treeName)
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

            FileInfo proxyFile = new FileInfo("Constructor_Test\\bogus.cpp");
            if (proxyFile.Directory.Exists)
                proxyFile.Directory.Delete(true);
            proxyFile.Directory.Create();
            switch (ntupleProxyIndex)
            {
                case 0:
                    ntuple._gProxyFile = "";
                    break;

                case 1:
                    ntuple._gProxyFile = proxyFile.FullName;
                    break;

                case 2:
                    using (var w = proxyFile.CreateText())
                    {
                        w.Close();
                    }
                    ntuple._gProxyFile = proxyFile.FullName;
                    break;

                default:
                    ntuple._gProxyFile = "";
                    break;
            }

            FileInfo extraFile = new FileInfo("Constructor_Test\\extra.cpp");
            switch (ntupleExtraIndex)
            {
                case 0:
                    ntuple._gObjectFiles = new string[0];
                    break;

                case 1:
                    ntuple._gObjectFiles = new string[] { extraFile.FullName };
                    break;

                case 2:
                    using (var w = extraFile.CreateText())
                    {
                        w.Close();
                    }
                    ntuple._gObjectFiles = new string[] { extraFile.FullName };
                    break;

                default:
                    ntuple._gObjectFiles = new string[0];
                    break;
            }


            TTreeQueryExecutor target = new TTreeQueryExecutor(new FileInfo[] { rootFile }, treeName, typeof(ntuple));

            Assert.IsNotNull(rootFile, "rootfile can't be null here");
            rootFile.Refresh();
            Assert.IsTrue(rootFile.Exists, "root file must exist");
            Assert.IsFalse(string.IsNullOrWhiteSpace(ntuple._gProxyFile), "proxy must be there");
            Assert.IsTrue(File.Exists(ntuple._gProxyFile), "proxy file must exist");
            if (ntuple._gObjectFiles != null)
            {
                Assert.IsTrue(ntuple._gObjectFiles.All(f => File.Exists(f)), "extra files must all exist");
            }

            ///
            /// Some post-tests to make sure things are "good".
            /// 



            return target;
            // TODO: add assertions to method TTreeQueryExecutorTest.Constructor(FileInfo, String)
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupe
        {
#pragma warning disable 0169
            public int run;
#pragma warning restore 0169
        }

        /// <summary>
        /// Ntuple with nothing in it - proxy and obj files missing.
        /// </summary>
        public class ntuple_nothing
        {

        }

        /// <summary>
        /// Ntuple with a proxy guy
        /// </summary>
        public class ntuple_with_proxy
        {
            public static string _gProxyFile = @"C:\Users\gwatts\Documents\ATLAS\Code\LINQtoROOT\DemosAndTests\GenerateNtupleXMLSpec\ntuple_btag.h";
        }

        /// <summary>
        /// Ntuple with emptys for everything.
        /// </summary>
        public class ntuple
        {
            public static string _gProxyFile = "";
            public static string[] _gObjectFiles = { };

            internal static void Reset()
            {
                _gProxyFile = "";
                _gObjectFiles = new string[0];
            }
        }

        /// <summary>
        /// Test out a simple result base ntuple object base.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSimpleReultOperatorNtupleEmptyObj()
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

            var exe = new TTreeQueryExecutor(new FileInfo[] { rootFile }, "dude", typeof(ntuple_nothing));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        /// <summary>
        /// Test out a simple result base ntuple object base and null proxy file base.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSimpleReultOperatorNtupleWithProxy()
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

            var exe = new TTreeQueryExecutor(new FileInfo[] { rootFile }, "dude", typeof(ntuple_with_proxy));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        /// <summary>
        /// Test out a simple result base ntuple object base.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSimpleReultOperatorNtupleOkButNothingInit()
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

            var exe = new TTreeQueryExecutor(new FileInfo[] { rootFile }, "dude", typeof(ntuple));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        /// <summary>
        /// Test out a simple result base ntuple object base.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestSimpleReultOperatorNtupleOkButBogusProxyPath()
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

            ntuple._gProxyFile = "junk.cppxdude";
            var exe = new TTreeQueryExecutor(new FileInfo[] { rootFile }, "dude", typeof(ntuple));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        /// <summary>
        /// Test out a simple result operator.
        /// </summary>
        [TestMethod]
        public void TestSimpleResultOperator()
        {
            RunSimpleCountResult(10);
        }

        private void RunSimpleCountResult(int numberOfIter)
        {
            var rootFile = CreateFileOfInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new FileInfo[] { rootFile }, "dude", typeof(ntuple));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        /// <summary>
        /// Given the root file and the root-tuple name, generate a proxy file 
        /// </summary>
        /// <param name="rootFile"></param>
        /// <returns></returns>
        private FileInfo GenerateROOTProxy(FileInfo rootFile, string rootTupleName)
        {
            ///
            /// First, load up the TTree
            /// 

            var tfile = new ROOTNET.NTFile(rootFile.FullName, "READ");
            var tree = tfile.Get(rootTupleName) as ROOTNET.Interface.NTTree;
            Assert.IsNotNull(tree, "Tree couldn't be found");

            ///
            /// Create the proxy sub-dir if not there already, and put the dummy macro in there
            /// 

            using (var w = File.CreateText("junk.C"))
            {
                w.Write("int junk() {return 10.0;}");
                w.Close();
            }

            ///
            /// Create the macro proxy now
            /// 

            tree.MakeProxy("scanner", "junk.C", null, "nohist");
            return new FileInfo("scanner.h");
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
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = GenerateROOTProxy(rootFile, "dude");
            ntuple._gProxyFile = proxyFile.FullName;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new FileInfo[] { rootFile }, "dude", typeof(ntuple));
            int result = exe.ExecuteScalar<int>(query);

            DirectoryInfo dir = TTreeQueryExecutor.TempDirectory;
            dir.Refresh();
            Assert.IsTrue(dir.Exists, "Temp directory doesn't exist");
            Assert.AreEqual(0, dir.EnumerateFiles().Count(), "Expected no spare files in there!");
            Assert.AreEqual(1, dir.EnumerateDirectories().Count(), "Incorrect # of subdirectories");
            Assert.AreEqual("CommonFiles", dir.GetDirectories()[0].Name, "incorrect name of single existing directory");
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

        [TestMethod]
        public void TestQueryCleanup()
        {
            int numberOfIter = 10;

            var rootFile = CreateFileOfInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Get the info out of the query executor
            /// 

            var d = TTreeQueryExecutor.QueryCreationDirectory;
            int currentDirs = d.EnumerateDirectories().Count();
            int currentFiles = d.EnumerateFiles().Count();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new FileInfo[] { rootFile }, "dude", typeof(ntuple));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);

            ///
            /// Ok - all went well. Now lets check again
            /// 

            d.Refresh();
            Assert.AreEqual(currentDirs, d.EnumerateDirectories().Count(), "directories aren't stable");
            Assert.AreEqual(currentFiles, d.EnumerateFiles().Count(), "files aren't stable");
        }

        [TestMethod]
        public void TestDualQueries()
        {
            RunSimpleCountResult(10);
            RunSimpleCountResult(20);
        }

        [TestMethod]
        public void TestLoadingCommonFiles()
        {
            /// Create a common C++ object that can be loaded and checked for.
            var d = PrepNonSpaceDir("TestLoadingCommonFiles");
            string fnamebase = "TestLoadingCommonFilesObj";
            var f = CreateCommonObject(fnamebase, d);

            ///
            /// Run a simple query - but fool it against another ntuple
            /// 

            var rootFile = CreateFileOfInt(1);
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// First, make sure to clear out the common area. We are relying on the fact that we know
            /// where the common area is for this step.
            /// 

            var commonArea = new DirectoryInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\LINQToROOT\CommonFiles");
            if (commonArea.Exists)
            {
                var filesToKill = (from fd in commonArea.EnumerateFiles()
                                   where fd.Name.Contains(fnamebase)
                                   select fd).ToArray();
                foreach (var theFile in filesToKill)
                {
                    theFile.Delete();
                }
            }

            ///
            /// Setup all the files we will have to have!
            /// 

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = GenerateROOTProxy(rootFile, "dude");
            ntuple._gProxyFile = proxyFile.FullName;
            ntuple._gObjectFiles = new string[] { f.FullName };

            ///
            /// Now run
            /// 

            var exe = new TTreeQueryExecutor(new FileInfo[] { rootFile }, "dude", typeof(ntuple));
            int result = exe.ExecuteScalar<int>(query);

            Assert.AreEqual(1, result, "The result should have run correctly.");

            ///
            /// Next, see if we can find the files in the common area.
            /// 

            commonArea.Refresh();
            Assert.IsTrue(commonArea.Exists, "The common build area doesn't exist currently");
            var filesFromOurObj = (from fd in commonArea.EnumerateFiles()
                                   where fd.Name.Contains(fnamebase)
                                   select fd).ToArray();
            Assert.IsTrue(filesFromOurObj.Length > 0, "no files from our common object");
        }

        [TestMethod]
        public void TestInitalizerWithROOTVariable()
        {
            const int numberOfIter = 25;
            var rootFile = CreateFileOfInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.AggregateNoReturn(new ROOTNET.NTH1F("hi", "title", 2, 0.0, 2.0), (h, n) => h.Fill(n.run));
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new FileInfo[] { rootFile }, "dude", typeof(ntuple));
            var result = exe.ExecuteScalar<ROOTNET.Interface.NTH1F>(query);
            Assert.AreEqual(result.Entries, numberOfIter);
        }
    }
}
