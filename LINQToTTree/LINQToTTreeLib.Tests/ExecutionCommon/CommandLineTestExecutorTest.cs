using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.ExecutionCommon;
using LINQToTTreeLib.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static LINQToTTreeLib.ExecutionCommon.CommandLineCommonExecutor;
using static LINQToTTreeLib.TTreeQueryExecutorTest;

namespace LINQToTTreeLib.Tests.ExecutionCommon
{
    [TestClass]
    [DeploymentItem(@"Templates\TSelectorTemplate.cxx")]
    public class CommandLineTestExecutorTest
    {
        public string tempDir = Path.GetTempPath() + "\\TestLINQToROOTDummyDir";

        [TestInitialize]
        public void TestSetup()
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
            Directory.CreateDirectory(tempDir);
            TestUtils.ResetLINQLibrary();
            ntuple.Reset();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestUtils.ResetLINQLibrary();
            MEFUtilities.MyClassDone();
            CommandLineExecutor.ResetCommandLineExecutor();
        }

        [CPPHelperClass]
        static class LocalWinCmdLineLoadExtraClassFilesHelpers
        {
            [CPPCode(
                Code = new string[] {
                    "auto h = new TestLoadingCommonFilesObj();" +
                    "DoObjectLookup = 1;"
                },
                IncludeFiles = new[] {
                    "TestLoadingCommonFilesObj.hpp"
                })]
            public static int DoObjectLookup()
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void LocalWinCmdLineLoadExtraClassFiles()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Setup an extra file to be loaded.
            string fnamebase = "TestLoadingCommonFilesObj";
            var f = TTreeQueryExecutorTest.CreateCommonObject(fnamebase, new DirectoryInfo("."));
            ntuple._gObjectFiles = new[] { f.FullName };

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Where(r => LocalWinCmdLineLoadExtraClassFilesHelpers.DoObjectLookup() > 0).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);
        }

        [CPPHelperClass]
        static class LocalWinCmdLineDictifyClassesHelpers
        {
            [CPPCode(
                Code = new string[] {
                    "auto d = TDictionary::GetDictionary(\"vector<vector<vector<float>>>\");",
                    "d->Dump();",
                    "DoDictDump = 1;"
                },
                IncludeFiles = new[] {
                    "TDictionary.h"
                })]
            public static int DoDictDump()
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void LocalWinCmdLineDictifyClasses()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Load up an extra dictionary
            ntuple._gClassesToDeclare = new[] { "vector<vector<vector<float>>>" };
            ntuple._gClassesToDeclareIncludes = new[] { "vector" };

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Where(r => LocalWinCmdLineDictifyClassesHelpers.DoDictDump() > 0).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));

            bool seenTDataType = false;
            CommandLineExecutor.AddLogEndpoint(s =>
            {
                seenTDataType |= s.Contains("TDataType");
                Console.WriteLine(s);
            });

            int result = exe.ExecuteScalar<int>(query);
            Assert.IsTrue(seenTDataType);
        }

        [TestMethod]
        public void LocalWinCmdLineSendObjectsToSelector()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query that includes packing over a histogram.
            // The funny way we do the bin content is to make sure the histogram is accessed
            // in a query data dependent way - otherwise the system optimizes the histogram access
            // to the host!
            var q = new QueriableDummy<TestNtupe>();
            var h = new ROOTNET.NTH1F("hi", "there", 1, 0.0, 10.0);
            h.Directory = null;
            h.Fill(5,3.0);
            GC.WaitForPendingFinalizers();
            var dude = q.Select(i => h.GetBinContent(i.run != 1 ? 1 : i.run)).Where(i => i > 2.5).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CleanupQuery = false;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void LocalWinCmdLineCountOperator()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        [DeploymentItem("unc_location.txt")]
        public void LocalWinCmdLineCountOperatorOnUNCPath()
        {
            var rootFileSource = TestUtils.CreateFileOfInt(20);
            var rootFile = MoveToUNC(rootFileSource);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);
        }

        /// <summary>
        /// Move this to some UNC path or similar.
        /// </summary>
        /// <param name="rootFileSource"></param>
        /// <returns></returns>
        private Uri MoveToUNC(Uri rootFileSource)
        {
            var f = new FileInfo(rootFileSource.LocalPath);
            var uncPath = new DirectoryInfo(File.ReadLines("unc_location.txt").First());
            var fDest = new FileInfo($"{uncPath.FullName}\\{f.Name}");
            f.CopyTo(fDest.FullName, overwrite: true);
            fDest.Refresh();
            return new Uri(fDest.FullName);
        }

        [TestMethod]
        public void LocalWinCmdLineCheckWarningsAndErrors()
        {
            RunSimpleTestForErrorsAndWarnings(exe => { });
        }

        [TestMethod]
        public void LocalWinCmdLineCheckWarningsAndErrorsDebug()
        {
            RunSimpleTestForErrorsAndWarnings(exe => { exe.CompileDebug = true; });
        }

        /// <summary>
        /// Run a simple test that looks for errors and warnings
        /// </summary>
        /// <param name="configureMe"></param>
        private static void RunSimpleTestForErrorsAndWarnings(Action<TTreeQueryExecutor> configureMe)
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            configureMe(exe);

            // Look for warning or error
            var errorLines = new List<string>();
            CommandLineExecutor.AddLogEndpoint(s =>
            {
                if (s.ToLower().Contains("warning") || s.ToLower().Contains("error"))
                {
                    errorLines.Add(s);
                }
            });
            int result = exe.ExecuteScalar<int>(query);

            foreach (var l in errorLines)
            {
                Console.WriteLine("************ Contains error or warning: " + l);
            }

            Assert.AreEqual(0, errorLines.Count, "See std out for list of error liens");
        }

        [TestMethod]
        public void LocalWinCmdLineCheckDebugDumps()
        {
            // When we run in debug mode, make sure the command line dumps are there.
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));

            // Capture the lines
            bool seenCacheInfo = false;
            CommandLineExecutor.AddLogEndpoint(s => seenCacheInfo |= s.ToLower().Contains("cache"));
            exe.CompileDebug = true;
            int result = exe.ExecuteScalar<int>(query);

            Assert.IsTrue(seenCacheInfo);
        }

        [TestMethod]
        public void LocalWinCmdLineLocalIncludeFile()
        {
            // Write out the local include file. The system should pick it up from here.
            using (var writer = File.CreateText("bogus_function.h"))
            {
                writer.WriteLine("int bogus() { return 15; }");
                writer.WriteLine();
                writer.Close();
            }

            // Run on ints, though for this test it won't matter.
            var rootFile = TestUtils.CreateFileOfInt(10);

            // Run the special function.
            var q = new QueriableDummy<TestNtupe>();
            var listing = from evt in q
                          where CPPHelperFunctions.ReturnCustomFuncValue() > 10.0
                          select evt;
            var dude = listing.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Run the execution environment.
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CleanupQuery = false;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void LocalWinCSVFileResult()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.AsCSV(new FileInfo("bogus.csv"), "run");
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            exe.CleanupQuery = false;
            var result = exe.ExecuteScalar<FileInfo[]>(query);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            result[0].Refresh();
            Assert.IsTrue(result[0].Exists);
        }

        [TestMethod]
        public void LocalWinROOTFileResult()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.AsTTree("mytree");
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            exe.CleanupQuery = false;
            var result = exe.ExecuteScalar<FileInfo[]>(query);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            result[0].Refresh();
            Assert.IsTrue(result[0].Exists);
        }

        /// <summary>
        /// Create a dirt simple query environment so we can see what running is like.
        /// </summary>
        /// <returns></returns>
        private ExecutionEnvironment CreateSimpleEnvironment()
        {
            var result = new ExecutionEnvironment();

            return result;
        }

        /// <summary>
        /// Creates the query for a simple execution file.
        /// </summary>
        /// <returns></returns>
        private ExecutionEnvironment CreateSimpleQueryEnvironment()
        {
            var result = CreateSimpleEnvironment();

            result.TreeName = "dude";

            return result;
        }

        /// <summary>
        /// Move everything to a temp dir... mostly to make sure that we are in a directory
        /// with no spaces int he path b/c ROOT doesn't know how to deal with them!
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private FileInfo CopyToTempDir(string p)
        {
            var dest = new FileInfo(string.Format("{0}\\{1}", tempDir, p));
            File.Copy(p, dest.FullName);
            dest.Refresh();
            return dest;
        }
    }

    public static class Helpers
    {
        /// <summary>
        /// Return a new uri that is the same as the old uri, but using the localwin guy specifier
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Uri AsLocalWinUri(this Uri source)
        {
            return new UriBuilder(source) { Scheme = "localwin" }.Uri;
        }
    }
}
