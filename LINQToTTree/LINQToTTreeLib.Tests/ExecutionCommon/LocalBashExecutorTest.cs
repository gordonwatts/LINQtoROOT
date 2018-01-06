using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.ExecutionCommon;
using LINQToTTreeLib.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LINQToTTreeLib.ExecutionCommon.CommandLineCommonExecutor;
using static LINQToTTreeLib.FutureResultOperatorsTest;

namespace LINQToTTreeLib.Tests.ExecutionCommon
{
    /// <summary>
    /// Test out a local run on bash
    /// </summary>
    [TestClass]
    public class LocalBashExecutorTest
    {
        public string tempDir = Path.GetTempPath() + "\\LocalBashExecutorTestDir";

        [TestInitialize]
        public void TestSetup()
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
            Directory.CreateDirectory(tempDir);
            TestUtils.ResetLINQLibrary();
            CommandLineCommonExecutor.ResetCommandLineExecutor();
            LocalBashExecutor.ResetLocalBashExecutor();
            ntuple.Reset();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestUtils.ResetLINQLibrary();
            MEFUtilities.MyClassDone();
            LocalBashExecutor.ResetLocalBashExecutor();
        }

        [TestMethod]
        public void LocalBashCmdLineCountOperator()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void LocalBashMultiThreaded()
        {
            // Create one file of int, but use it 20 times.
            var rootFile = TestUtils.CreateFileOfInt(20);
            var files = Enumerable.Range(0, 20).Select(c => rootFile.AsLocalBashUri()).ToArray();
            const int expectedSize = 20 * 20;

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(files, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(expectedSize, result);

            // We should have split this many files into the number of CPU's we have.
            Assert.AreEqual(Environment.ProcessorCount, exe.CountExecutionRuns);
        }

        [TestMethod]
        public void LocalBashCSVFileResult()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.AsCSV(new FileInfo("bogus.csv"), "run");
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            exe.CleanupQuery = false;
            var result = exe.ExecuteScalar<FileInfo[]>(query);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            result[0].Refresh();
            Assert.IsTrue(result[0].Exists);
        }

        [TestMethod]
        public void LocalBashROOTFileResult()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.AsTTree("mytree");
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            exe.CleanupQuery = false;
            var result = exe.ExecuteScalar<FileInfo[]>(query);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            result[0].Refresh();
            Assert.IsTrue(result[0].Exists);
        }

        [TestMethod]
        [ExpectedException(typeof(LocalBashExecutor.FailedToInstallROOTException))]
        public void LocalBashNoROOTFound()
        {
            try
            {
                // This should cause a hard fail.
                LocalBashExecutor.ROOTVersionNumber = "22322";

                // Set it up to look for something else.

                var rootFile = TestUtils.CreateFileOfInt(20);

                // Get a simple query we can "play" with
                var q = new QueriableDummy<TestNtupe>();
                var dude = q.Count();
                var query = DummyQueryExectuor.LastQueryModel;

                // Ok, now we can actually see if we can make it "go".
                var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
                int result = exe.ExecuteScalar<int>(query);
            } catch (AggregateException ag)
            {
                throw ag.UnrollAggregateExceptions();
            }
        }

        [CPPHelperClass]
        static class LocalBashCmdLineLoadExtraClassFilesHelpers
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
        public void LocalBashCmdLineLoadExtraClassFiles()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Setup an extra file to be loaded.
            string fnamebase = "TestLoadingCommonFilesObj";
            var f = TTreeQueryExecutorTest.CreateCommonObject(fnamebase, new DirectoryInfo("."));
            ntuple._gObjectFiles = new[] { f.FullName };

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Where(r => LocalBashCmdLineLoadExtraClassFilesHelpers.DoObjectLookup() > 0).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);
        }

        [CPPHelperClass]
        static class LocalBashCmdLineDictifyClassesHelpers
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

        /// <summary>
        /// In root 6 this dictifying doesn't seem to have an effect. Need to ask about it.
        /// </summary>
        [Ignore]
        [TestMethod]
        public void LocalBashCmdLineDictifyClasses()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Load up an extra dictionary
            ntuple._gClassesToDeclare = new[] { "vector<vector<vector<float>>>" };
            ntuple._gClassesToDeclareIncludes = new[] { "vector" };

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Where(r => LocalBashCmdLineDictifyClassesHelpers.DoDictDump() > 0).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CleanupQuery = false;

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
        public void LocalBashCmdLineSendObjectsToSelector()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query that includes packing over a histogram.
            // The funny way we do the bin content is to make sure the histogram is accessed
            // in a query data dependent way - otherwise the system optimizes the histogram access
            // to the host!
            var q = new QueriableDummy<TestNtupe>();
            var h = new ROOTNET.NTH1F("hi", "there", 1, 0.0, 10.0);
            h.Directory = null;
            h.Fill(5, 3.0);
            GC.WaitForPendingFinalizers();
            var dude = q.Select(i => h.GetBinContent(i.run != 1 ? 1 : i.run)).Where(i => i > 2.5).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void LocalBashCmdLineCheckWarningsAndErrors()
        {
            RunSimpleTestForErrorsAndWarnings(exe => { });
        }

        [TestMethod]
        public void LocalBashCmdLineCheckWarningsAndErrorsDebug()
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
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
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
        public void LocalBashCmdLineCheckDebugDumps()
        {
            // When we run in debug mode, make sure the command line dumps are there.
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));

            // Capture the lines
            bool seenCacheInfo = false;
            CommandLineExecutor.AddLogEndpoint(s => seenCacheInfo |= s.ToLower().Contains("cache"));
            exe.CompileDebug = true;
            int result = exe.ExecuteScalar<int>(query);

            Assert.IsTrue(seenCacheInfo);
        }

        [TestMethod]
        public void LocalBashCmdLocalIncludeFile()
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
                          where TTreeQueryExecutorTest.CPPHelperFunctions.ReturnCustomFuncValue() > 10.0
                          select evt;
            var dude = listing.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Run the execution environment.
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CleanupQuery = false;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
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

    public static class HelpersBash
    {
        /// <summary>
        /// Return a new uri that is the same as the old uri, but using the localwin guy specifier
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Uri AsLocalBashUri(this Uri source)
        {
            return new Uri($"localbash://{source.LocalPath}");
        }
    }
}
