using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib
{
    [TestClass]
    [DeploymentItem(@"Templates\TSelectorTemplate.cxx")]
    public partial class TTreeQueryExecutorTest
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();

            ntuple.Reset();
            ntuple._gCINTLines = null;
            ntuple._gObjectFiles = null;
            ntuple._gProxyFile = null;

        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

#if false
        [PexMethod, PexAllowedException(typeof(FileNotFoundException))]
        public TTreeQueryExecutor Constructor(Uri rootFile, string proxyLocation, string[] extraLocations, string treeName)
        {
#if false
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
#endif
            ntuple._gProxyFile = proxyLocation;
#if false

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
#endif

#if false
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
#endif
            ntuple._gObjectFiles = extraLocations;

            TTreeQueryExecutor target = new TTreeQueryExecutor(new Uri[] { rootFile }, treeName, typeof(ntuple));

            Assert.IsNotNull(rootFile, "rootfile can't be null here");
            Assert.IsTrue(File.Exists(rootFile.LocalPath), "root file must exist");
            Assert.IsFalse(string.IsNullOrWhiteSpace(ntuple._gProxyFile), "proxy must be there");
            Assert.IsTrue(File.Exists(ntuple._gProxyFile), "proxy file must exist");
            if (ntuple._gObjectFiles != null)
            {
                Assert.IsTrue(ntuple._gObjectFiles.All(f => File.Exists(f)), "extra files must all exist");
            }

            return target;
        }
#endif

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
            public static string _gProxyFile;
        }

        /// <summary>
        /// Ntuple with emptys for everything.
        /// </summary>
        public class ntuple
        {
            public static string _gProxyFile = "";
            public static string[] _gObjectFiles = { };
            public static string[] _gCINTLines = { };

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

            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple_nothing));
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

            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple_with_proxy));
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

            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple));
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

            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);

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
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple));
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
            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

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
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestSimpleResultDebugCompile()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

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
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestCreateTupleWithNew()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = from evt in q
                       select new Tuple<int, int>(evt.run, evt.run);
            var r = dude.Where(i => i.Item1 > 0).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestCreateTupleWithCreate()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = from evt in q
                       select Tuple.Create(evt.run, evt.run);
            var r = dude.Where(i => i.Item1 > 0).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }

        [CPPHelperClass]
        public static class CPPHelperFunctions
        {
            [CPPCode(Code = new string[] { "Calc = arg*2;" })]
            public static int Calc(int arg)
            {
                throw new NotImplementedException();
            }

            [CPPCode(Code = new string[] { "CalcLen = strlen(arg);" }, IncludeFiles =new string[] { "stdlib.h" })]
            public static int CalcLen(string arg)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void TestInclusiveCPPInfo()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var listing = from evt in q
                          where CPPHelperFunctions.Calc(evt.run) > 10.0
                          select evt;
            var dude = listing.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestCPPCodeAndStringPassing()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var listing = from evt in q
                          where CPPHelperFunctions.CalcLen("hi there dude my butt") > 10.0
                          select evt;
            var dude = listing.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestNewOfObject()
        {
            /// Make sure the "new" gets translated to C++ correctly and there are no errors!

            var rootFile = TestUtils.CreateFileOfInt(5);
            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var letResult = from evt in q
                            let temp = new ROOTNET.NTLorentzVector(evt.run, evt.run, evt.run, evt.run)
                            where temp.Pt() > 0.0
                            select temp;
            var cnt = letResult.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            var result = exe.ExecuteScalar<int>(query);

            Assert.AreEqual(1, exe.CountExecutionRuns, "exe after exe run");
            Assert.AreEqual(5, result, "count incorrect");
        }

        [TestMethod]
        public void TestCachingOfSimpleHisto()
        {
            /// Do two identical queries. Make sure only one causes an actual run!

            var rootFile = TestUtils.CreateFileOfInt(5);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            Assert.AreEqual(0, exe.CountExecutionRuns, "exe runs initialization");
            Assert.AreEqual(0, exe.CountCacheHits, "cache hits initialization");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Plot("hi", "there", 10, 0.0, 20.0, d => d.run);
            var query = DummyQueryExectuor.LastQueryModel;

            var result = exe.ExecuteScalar<ROOTNET.Interface.NTH1>(query);

            Assert.AreEqual(1, exe.CountExecutionRuns, "exe after exe run");
            Assert.AreEqual(0, exe.CountCacheHits, "cache after exe run");

            ///
            /// Re-run the idential query. We have to remake the query b/c the histogram
            /// stored internally will have now counts in it!! Ops!! :-)
            /// 

            var dude2 = q.Plot("hi", "there", 10, 0.0, 20.0, d => d.run);
            var query2 = DummyQueryExectuor.LastQueryModel;

            var result2 = exe.ExecuteScalar<ROOTNET.Interface.NTH1>(query2);

            Assert.AreEqual(1, exe.CountExecutionRuns, "exe after exe and cache run");
            Assert.AreEqual(1, exe.CountCacheHits, "cache after exe and cache run");
        }

        [TestMethod]
        public void TestCachingOfSimpleHistoWithNameTitleChange()
        {
            /// Do two identical queries. Make sure only one causes an actual run!

            var rootFile = TestUtils.CreateFileOfInt(5);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            Assert.AreEqual(0, exe.CountExecutionRuns, "exe runs initialization");
            Assert.AreEqual(0, exe.CountCacheHits, "cache hits initialization");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Plot("hi", "there", 10, 0.0, 20.0, d => d.run);
            var query = DummyQueryExectuor.LastQueryModel;

            var result = exe.ExecuteScalar<ROOTNET.Interface.NTH1>(query);

            Assert.AreEqual(1, exe.CountExecutionRuns, "after exe run");
            Assert.AreEqual(0, exe.CountCacheHits, "after exe run");

            ///
            /// Re-run a slightly different query. Should still be cached.
            /// 

            var dude2 = q.Plot("there", "high", 10, 0.0, 20.0, d => d.run);
            var query2 = DummyQueryExectuor.LastQueryModel;

            var result2 = exe.ExecuteScalar<ROOTNET.Interface.NTH1>(query2);

            Assert.AreEqual(1, exe.CountExecutionRuns, "after exe and cache run");
            Assert.AreEqual(1, exe.CountCacheHits, "after exe and cache run");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestForZeroInputFiles()
        {
            var rootFile = TestUtils.CreateFileOfInt(5);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

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
            var exe = new TTreeQueryExecutor(new Uri[] { }, "dude", typeof(ntuple));
            int result = exe.ExecuteScalar<int>(query);
        }

        [TestMethod]
        public void TestTempDirectoryLocationAndEmptying()
        {
            var rootFile = TestUtils.CreateFileOfInt(1);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");
            ntuple._gProxyFile = proxyFile.FullName;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);

            var dir = new DirectoryInfo(Path.GetTempPath() + "\\LINQToROOT"); ;
            dir.Refresh();
            Assert.IsTrue(dir.Exists, "Temp directory doesn't exist");
            Assert.AreEqual(0, dir.EnumerateFiles().Count(), "Expected no spare files in there!");
            Assert.AreEqual(2, dir.EnumerateDirectories().Count(), "Incorrect # of subdirectories");
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

        [TestMethod]
        public void TestDualQueries()
        {
            RunSimpleCountResult(10);
            RunSimpleCountResult(20);
        }

        [TestMethod]
        public void TestDualQueryOnSameQueriable()
        {
            var rootFile = TestUtils.CreateFileOfInt(5);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

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
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);

            ///
            /// Run the second one now
            /// 

            var dude1 = q.Where(e => e.run > 10).Count();
            query = DummyQueryExectuor.LastQueryModel;
            result = exe.ExecuteScalar<int>(query);
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

            var rootFile = TestUtils.CreateFileOfInt(1);
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// First, make sure to clear out the common area. We are relying on the fact that we know
            /// where the common area is for this step.
            /// 

            var commonArea = new DirectoryInfo(Path.GetTempPath() + @"\LINQToROOT\CommonFiles");
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

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");
            ntuple._gProxyFile = proxyFile.FullName;
            ntuple._gObjectFiles = new string[] { f.FullName };

            ///
            /// Now run
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);

            Assert.AreEqual(1, result, "The result should have run correctly.");

            ///
            /// Next, see if we can find the files in the common area.
            /// 

            commonArea.Refresh();
            Assert.IsTrue(commonArea.Exists, string.Format("The common build area doesn't exist currently ({0}).", commonArea.FullName));
            var filesFromOurObj = (from fd in commonArea.EnumerateFiles()
                                   where fd.Name.Contains(fnamebase)
                                   select fd).ToArray();
            Assert.IsTrue(filesFromOurObj.Length > 0, "no files from our common object");
        }

        // For ntuples generated with CreateFileOfIndexedInt
        public class TestSingleIndexArray : IExpressionHolder
        {
            public TestSingleIndexArray(Expression holder)
            {
                HeldExpression = holder;
            }
            public System.Linq.Expressions.Expression HeldExpression { get; set; }

#pragma warning disable 0169
            [ArraySizeIndex("n")]
            public int[] arr;
            public int n;
#pragma warning restore 0169

        }

        [TestMethod]
        public void TestIndexArray()
        {
            // Make sure we can process an index array (an array that is specified as arr[n]).

            // Create the ntuple file and the proxy that we will be using.
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfIndexedInt(numberOfIter);
            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            // Do a simple query to make sure the # of items in each array is "10", and that
            // there are 25 such events.

            var q = new QueriableDummy<TestSingleIndexArray>();
            var dudeQ = from evt in q
                        where (evt.arr.Count() == 10)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestSingleIndexArray));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupeArr : IExpressionHolder
        {
            public TestNtupeArr(Expression holder)
            {
                HeldExpression = holder;
            }
#pragma warning disable 0169
            public int[] myvectorofint;
#pragma warning restore 0169

            public System.Linq.Expressions.Expression HeldExpression { get; set; }
        }

        public class TestNtupeArrD : IExpressionHolder
        {
            public TestNtupeArrD(Expression holder)
            {
                HeldExpression = holder;
            }
#pragma warning disable 0169
            public double[] myvectorofdouble;
#pragma warning restore 0169

            public System.Linq.Expressions.Expression HeldExpression { get; set; }
        }

        public class TestNtupeArrJets
        {
            [TTreeVariableGrouping]
            public int myvectorofint;
        }

        [TranslateToClass(typeof(TestNtupeArr))]
        public class TestNtupeArrEvents
        {
            [TTreeVariableGrouping]
            public TestNtupeArrJets[] jets;
        }

        [TestMethod]
        public void TestFirstCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.First() > 0)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestFirstCodeCombine()
        {
            // Run a First(), but do it twice. The reason is to make sure that
            // the code doesn't step on itself with a break statement.

            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ1 = from evt in q
                         where (evt.myvectorofint.First() > 0)
                         select evt;
            var dude1 = dudeQ1.Count();
            var query1 = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            var dudeQ2 = from evt in q
                         where (evt.myvectorofint.Skip(1).First() > 0)
                         select evt;
            var dude2 = dudeQ2.Count();
            var query2 = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result2 = exe.ExecuteScalarAsFuture<int>(query2);
            var result1 = exe.ExecuteScalarAsFuture<int>(query1);
            Assert.AreEqual(0, result1.Value, "result 1");
            Assert.AreEqual(numberOfIter, result2.Value, "result 2");
        }

        [TestMethod]
        public void TestFirstCodeTranslated()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArrEvents>();
            var dudeQ = from evt in q
                        where (evt.jets.First().myvectorofint > 0)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArrEvents));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void TestFirstCodeDefaultTranslated()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArrEvents>();
            var dudeQ = from evt in q
                        where (evt.jets.Take(1).Skip(2).FirstOrDefault() == null)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArrEvents));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestFirstOrDefaultCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Take(2).Skip(3).FirstOrDefault() == 0)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(25, result, "Incorrect number of iterations found");
        }

        [TestMethod]
        [ExpectedException(typeof(System.Runtime.InteropServices.SEHException))]
        public void TestFirstButNothing()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Skip(50).First() == 0)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
        }

        [TestMethod]
        public void TestLastCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Last() > 0)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestMaxCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Max() > 5)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestMinCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Min() > 5)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void TestAllCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.All(r => r > 5))
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestAnyCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Any(r => r > 0))
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestSumCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        let r = evt.myvectorofint.Sum()
                        where r == 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1
                        select r;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);

        }

        [TestMethod]
        public void TestAverageCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        let r = evt.myvectorofint.Average()
                        where r == ((double)(9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1) / 10)
                        select r;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);

        }

        [TestMethod]
        public void TestAggregateCodeForSimpleVariableType()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        let r = evt.myvectorofint.Aggregate(0, (s, v) => s + v)
                        where r == 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1
                        select r;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);

        }

        [TestMethod]
        public void TestAggregateCodeForSimpleDoubleVariableType()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorDouble(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArrD>();
            var evtvalue = 9.5 + 8.5 + 7.5 + 6.5 + 5.5 + 4.5 + 3.5 + 2.5 + 1.5 + 0.5;
            var dudeQ = from evt in q
                        let r = evt.myvectorofdouble.Aggregate(0.0, (s, v) => s + v)
                        where r == 9.5 + 8.5 + 7.5 + 6.5 + 5.5 + 4.5 + 3.5 + 2.5 + 1.5 + 0.5
                        select r;
            var dude = dudeQ.Aggregate(0.0, (acc, val) => acc + val);

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArrD));
            var result = exe.ExecuteScalar<double>(query);
            Assert.AreEqual(result, numberOfIter * evtvalue);

        }

        [TestMethod]
        public void TestNestedCount()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Count() > 5)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestUniqueCombinations()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// We are looking at an array that has 10 entries in it. So if we create a
            /// unique combo, then we will have 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1 items,
            /// or 45 items.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.UniqueCombinations().Count() == 45)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestUniqueCombinationsWithBreak()
        {
            //
            // The way the unique combo works is it builds in a double loop. So if something inside that loop
            // throws a break it has to be transmitted out two levels. This is meant to test for that.
            //

            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, 9);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// We are looking at an array that has 10 entries in it. So if we create a
            /// unique combo, then we will have 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1 items,
            /// or 45 items.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (from cmb in evt.myvectorofint.UniqueCombinations() where (cmb.Item1 + cmb.Item2 < 4) select cmb).Any()
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestPairWiseAll()
        {
            //
            // The way the unique combo works is it builds in a double loop. So if something inside that loop
            // throws a break it has to be transmitted out two levels. This is meant to test for that.
            //

            const int numberOfIter = 25;
            const int vecSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vecSize);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// We are looking at an array that has 10 entries in it. So if we create a
            /// unique combo, then we will have 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1 items,
            /// or 45 items.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (from cmb in evt.myvectorofint.PairWiseAll((i1, i2) => i1 != i2) select cmb).Count() == vecSize
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestPairwiseAllWithExternalRef()
        {
            //
            // Look for a bug that happens when we have an external guy that references
            // something in the inside loop.
            //

            const int numberOfIter = 25;
            const int vecSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vecSize);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// We are looking at an array that has 10 entries in it. So if we create a
            /// unique combo, then we will have 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1 items,
            /// or 45 items.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (from cmb in evt.myvectorofint.PairWiseAll((i1, i2) => CPPHelperFunctions.Calc(i1) != CPPHelperFunctions.Calc(i2)) select cmb).Count() == vecSize
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

#if false
        [TestMethod]
        public void TestProofSimple()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");
            rootFile = new Uri("proof://tev99.phys.washington.edu/LINQToLINQ");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.First() > 0)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result);
        }
#endif

        [TestMethod]
        public void TestSortAscending()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                orderby v ascending
                                select v).Take(2).Sum();

            // The first two elements are 0 and 1, so 0 + 1 == 1.
            var dude = dudeQ.Where(x => x == 1).Count();

            var query = DummyQueryExectuor.LastQueryModel;

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestSortAscendingTranslated()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArrEvents>();

            var dudeQ = from evt in q
                        select (from v in evt.jets
                                orderby v.myvectorofint ascending
                                select v).Take(2).Sum(j => j.myvectorofint);

            // The first two elements are 0 and 1, so 0 + 1 == 1.
            var dude = dudeQ.Sum();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArrEvents));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        /// <summary>
        /// First sometimes uses a break in the code - make sure that we deal with it correctly.
        /// </summary>
        [TestMethod]
        public void TestSortAscendingWithFirst()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                orderby v ascending
                                select v).First();

            // The first element is 0.
            var dude = dudeQ.Where(x => x == 0).Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestSortDescending()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                orderby v descending
                                select v).Take(2).Sum();

            // The last two elements are 9 and 8, so 9 + 8 = 17.
            var dude = dudeQ.Where(x => x == 17).Count();

            var query = DummyQueryExectuor.LastQueryModel;

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestGroupByAccessKey()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         from grp in evt
                         where grp.Key == 2
                         select grp.Key;

            var dudq = dudeQ1.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestGroupByCountItems()
        {
            const int numberOfIter = 25;
            const int vectorSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vectorSize);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         from grp in evt
                         where grp.Count() == 1
                         select grp.Key;

            var dudq = dudeQ1.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter * vectorSize, result);
        }

        [TestMethod]
        public void TestGroupBySortItems()
        {
            const int numberOfIter = 25;
            const int vectorSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vectorSize);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         from grp in evt
                         where grp.OrderBy(v => v).First() == 0
                         select grp.Key;

            var dudq = dudeQ1.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestGroupByAccessItems1()
        {
            const int numberOfIter = 25;
            const int vectorSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vectorSize);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         from grp in evt
                         where grp.Where(v => v == 2).Any()
                         select grp.Key;

            var dudq = dudeQ1.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestGroupByAccessItems2()
        {
            const int numberOfIter = 25;
            const int vectorSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vectorSize);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         from grp in evt
                         where grp.Where(v => v == 22).Any()
                         select grp.Key;

            var dudq = dudeQ1.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestGroupByAndSortKey()
        {
            const int numberOfIter = 25;
            const int vectorSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vectorSize);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         select (from grp in evt
                                 orderby grp.Key descending
                                 select grp).First();

            var dudeQ2 = from evt in dudeQ1
                         where evt.Key == 9 && evt.Count() == 1
                         select evt;

            var dudq = dudeQ2.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestGroupByWithInto()
        {
            const int numberOfIter = 25;
            const int vectorSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vectorSize);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        from v in evt.myvectorofint
                        group v by v into lists
                        from i in lists
                        where i == 5
                        select i;

            var r = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestInitalizerWithROOTVariable()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var holder = new ROOTNET.NTH1F("hi", "title", 2, 0.0, 2.0);
            holder.Directory = null;
            var dude = q.ApplyToObject(holder, (h, n) => h.Fill(n.run));
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            var result = exe.ExecuteScalar<ROOTNET.Interface.NTH1F>(query);
            Assert.AreEqual(result.Entries, numberOfIter);
            Assert.AreEqual("hi", result.Name, "histogram name");
        }

        [TestMethod]
        public void TestInlineIfIntCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        let j = evt.myvectorofint[0] == 0 ? 1 : 0
                        where (j == 1)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestComplexSimpleTypeInlineIfIntCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        let j = evt.myvectorofint[0] == 0 ? evt.myvectorofint.First() : evt.myvectorofint.Skip(1).First()
                        where (j != 1)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestInlineIfObjectCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        let k = evt.myvectorofint[0]
                        let j = k == 0 ? new ROOTNET.NTVector3(k, k, k) : null
                        where (j == null)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestTransferSingleObject()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var mainHist = new ROOTNET.NTH1F("hi", "there", 1000, 0.0, 1000.0);
            mainHist.Directory = null;

            var dude = from evt in q
                       where mainHist.GetBinContent(evt.run) > 0.0
                       select evt;
            var final = dude.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result, "Didn't add correctly");
            Assert.AreEqual("hi", mainHist.Name, "histogram name changed");
        }

        [TestMethod]
        public void TestSameHistoOverTice()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var mainHist = new ROOTNET.NTH1F("hi", "there", 1000, 0.0, 1000.0);
            mainHist.Directory = null;

            var dude = from evt in q
                       where mainHist.GetBinContent(evt.run) * mainHist.GetBinContent(evt.run) > 0.0
                       select evt;
            var final = dude.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result, "Didn't add correctly");
            Assert.AreEqual("hi", mainHist.Name, "histogram name changed");

        }

        [TestMethod]
        public void TestSameHistInCombinedQueries()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get two querries we can play with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var mainHist = new ROOTNET.NTH1F("hi", "there", 1000, 0.0, 1000.0);
            mainHist.Directory = null;

            var dude1 = from evt in q
                        where mainHist.GetBinContent(evt.run) > 0.0
                        select evt;
            var final1 = dude1.Count();
            var query1 = DummyQueryExectuor.LastQueryModel;

            var dude2 = from evt in q
                        where mainHist.GetBinContent(evt.run) > 0.0
                        select evt;
            var final2 = dude2.Count();
            var query2 = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            var r1f = exe.ExecuteScalarAsFuture<int>(query1);
            var r2f = exe.ExecuteScalarAsFuture<int>(query2);

            var r1 = r1f.Value;
            var r2 = r2f.Value;

            Assert.AreEqual(0, r1, "r1 Didn't add correctly");
            Assert.AreEqual(0, r2, "r2 Didn't add correctly");
            Assert.AreEqual("hi", mainHist.Name, "histogram name changed");
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupeCConstArr : IExpressionHolder
        {
            public TestNtupeCConstArr(Expression holder)
            {
                HeldExpression = holder;
            }
#pragma warning disable 0169
            [ArraySizeIndex("5", Index = 0, IsConstantExpression = true)]
            public int[] arr;
#pragma warning restore 0169

            public System.Linq.Expressions.Expression HeldExpression { get; set; }
        }

        [TestMethod]
        public void TestCArrayConst()
        {
            // Test arr[5].
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOf("TestCArrayConst.root", () => TTreeParserCPPTests.CreateTrees.CreateTreeWithIndexedConstSimpleVector(numberOfIter));

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeCConstArr>();
            var dudeQ = from evt in q
                        where (evt.arr.Count() == 5)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeCConstArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(25, result, "Incorrect number of iterations found");
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupeC2DConstArr : IExpressionHolder
        {
            public TestNtupeC2DConstArr(Expression holder)
            {
                HeldExpression = holder;
            }
#pragma warning disable 0169
            [ArraySizeIndex("5", Index = 0, IsConstantExpression = true)]
            [ArraySizeIndex("5", Index = 1, IsConstantExpression = true)]
            public int[][] arr;
#pragma warning restore 0169

            public System.Linq.Expressions.Expression HeldExpression { get; set; }
        }

        [TestMethod]
        public void TestCArray2DConst()
        {
            // Test arr[5].
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOf("TestCArray2DConst.root", () => TTreeParserCPPTests.CreateTrees.CreateTreeWithIndexed2DConstSimpleVector(numberOfIter));

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeC2DConstArr>();
            var dudeQ = from evt in q
                        let c = (from c1 in evt.arr
                                 from c2 in c1
                                 select c2).Count()
                        where c == 25
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeC2DConstArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(25, result, "Incorrect number of iterations found");
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupeCIndexedArr : IExpressionHolder
        {
            public TestNtupeCIndexedArr(Expression holder)
            {
                HeldExpression = holder;
            }
#pragma warning disable 0169
            [ArraySizeIndex("n")]
            public int[] arr;
            public int n;
#pragma warning restore 0169

            public System.Linq.Expressions.Expression HeldExpression { get; set; }
        }

        [TestMethod]
        public void TestCArrayIndexed()
        {
            // Test arr[5].
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOf("TestCArrayIndexed.root", () => TTreeParserCPPTests.CreateTrees.CreateTreeWithIndexedSimpleVector(numberOfIter));

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeCIndexedArr>();
            var dudeQ = from evt in q
                        where (evt.arr.Count() == evt.n)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeCIndexedArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(25, result, "Incorrect number of iterations found");
        }

        [TestMethod]
        public void TestCArrayConstEnumerable()
        {
            // Test arr[5].
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOf("TestCArrayConstEnumerable.root", () => TTreeParserCPPTests.CreateTrees.CreateTreeWithIndexedConstSimpleVector(numberOfIter));

            ///
            /// Generate a proxy .h file that we can use
            /// 

            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeCConstArr>();
            var dudeQ = from evt in q
                        let tmp = (from index in Enumerable.Range(0, 2)
                                   select evt.arr[index]).Count()
                        where tmp == 2
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeCConstArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(25, result, "Incorrect number of iterations found");
        }
    }
}
