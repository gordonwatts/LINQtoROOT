using AtlasSSH;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.ExecutionCommon;
using LINQToTTreeLib.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static LINQToTTreeLib.TTreeQueryExecutorTest;

namespace LINQToTTreeLib.Tests.ExecutionCommon
{
    /// <summary>
    /// Test out remote bash execution. This will require a valid ssh connection and config files to work
    /// (which aren't part of the git repo).
    /// </summary>
    [TestClass]
    public class RemoteBashExecutorTest
    {
        [TestInitialize]
        public void TestSetup()
        {
            TestUtils.ResetLINQLibrary();
            CommandLineCommonExecutor.ResetCommandLineExecutor();
            RemoteBashExecutor.ResetRemoteBashExecutor();
            ntuple.Reset();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestUtils.ResetLINQLibrary();
            MEFUtilities.MyClassDone();
            RemoteBashExecutor.ResetRemoteBashExecutor();
        }

        [TestMethod]
        public void ROOTNotInstalledTestMethod()
        {
            Assert.Inconclusive("Fail to install ROOT and catch the right exception");
        }

        [TestMethod]
        public void RemoteBashCmdLineCountOperator()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsRemoteBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        [DeploymentItem("testmachine.txt")]
        public void RemoteBashCmdLineCountOperatorWithRemoteFile()
        {
            // Put a file over on the remote machine, and then remove it locally, so it can't be sent over.
            var rootFileLocal = TestUtils.CreateFileOfInt(20);
            var rootFileRemote = MoveToBash(rootFileLocal, File.ReadLines("testmachine.txt").First(), "/tmp");
            var localFile = new FileInfo(rootFileLocal.LocalPath);
            localFile.Delete();

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Watch to see if the file is copied over.
            var fname = Path.GetFileName(rootFileLocal.LocalPath);
            var listOfBadLines = new List<string>();
            RemoteBashExecutor.AddLogEndpoint(l =>
            {
                if (l.Contains(fname))
                {
                    listOfBadLines.Add(l);
                }
            });

            // Next, for this machine, create a pattern that says this file doesn't need to be copied, and where it is located.
            var m = RemoteBashExecutor.GetMachineInfo("tev.machines");
            m.AddFileNoCopyPattern(rootFileLocal.LocalPath, $"/tmp/{fname}");

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFileRemote }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);

            // Check that there are no or few bad lines.
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Lines with the root file we saw:");
            foreach (var l in listOfBadLines)
            {
                Console.WriteLine(l);
            }
            Assert.AreEqual(0, listOfBadLines.Count);
        }

        /// <summary>
        /// Move a file to the remote machine
        /// </summary>
        /// <param name="rootFile"></param>
        /// <returns></returns>
        private Uri MoveToBash(Uri rootFile, string remoteInfo, string remoteDirectory)
        {
            using (var t = new SSHConnectionTunnel(remoteInfo))
            {
                var f = new FileInfo(rootFile.LocalPath);
                t.Connection.CopyLocalFileRemotely(f, $"{remoteDirectory}/{f.Name}");
                return new Uri($"remotebash://tev.machines/{remoteDirectory}/{f.Name}");
            }
        }

        [TestMethod]
        public void RemoteBashCSVFileResult()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.AsCSV(new FileInfo("bogus.csv"), "run");
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsRemoteBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            exe.CleanupQuery = false;
            var result = exe.ExecuteScalar<FileInfo[]>(query);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            result[0].Refresh();
            Assert.IsTrue(result[0].Exists);
        }

        [TestMethod]
        public void RemoteBashROOTFileResult()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.AsTTree("mytree");
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsRemoteBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            exe.CleanupQuery = false;
            var result = exe.ExecuteScalar<FileInfo[]>(query);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
            result[0].Refresh();
            Assert.IsTrue(result[0].Exists);
        }

        [TestMethod]
        [ExpectedException(typeof(RemoteBashExecutor.ROOTCantBeInstalledRemotelyException))]
        public void RemoteBashNoROOTFound()
        {
            // This should cause a hard fail.
            RemoteBashExecutor.ROOTVersionNumber = "22322";

            // Set it up to look for something else.

            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsRemoteBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
        }

        [CPPHelperClass]
        static class RemoteBashCmdLineLoadExtraClassFilesHelpers
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
        public void RemoteBashCmdLineLoadExtraClassFiles()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Setup an extra file to be loaded.
            string fnamebase = "TestLoadingCommonFilesObj";
            var f = TTreeQueryExecutorTest.CreateCommonObject(fnamebase, new DirectoryInfo("."));
            ntuple._gObjectFiles = new[] { f.FullName };

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Where(r => RemoteBashCmdLineLoadExtraClassFilesHelpers.DoObjectLookup() > 0).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsRemoteBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);
        }

        [CPPHelperClass]
        static class RemoteBashCmdLineDictifyClassesHelpers
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
        public void RemoteBashCmdLineSendObjectsToSelector()
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
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsRemoteBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void RemoteBashCmdLineCheckWarningsAndErrors()
        {
            RunSimpleTestForErrorsAndWarnings(exe => { });
        }

        [TestMethod]
        public void RemoteBashCmdLineCheckWarningsAndErrorsDebug()
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
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsRemoteBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
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
        [Ignore]
        public void RemoteBashCmdLineCheckDebugDumps()
        {
            // When we run in debug mode, make sure the command line dumps are there.
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsRemoteBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));

            // Capture the lines
            bool seenCacheInfo = false;
            CommandLineExecutor.AddLogEndpoint(s => seenCacheInfo |= s.ToLower().Contains("cache"));
            exe.CompileDebug = true;
            int result = exe.ExecuteScalar<int>(query);

            Assert.IsTrue(seenCacheInfo);
        }

        [TestMethod]
        public void RemoteBashCmdLocalIncludeFile()
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
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsRemoteBashUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CleanupQuery = false;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }
    }

    static class RemoteBashExecutorHelpers
    {
        /// <summary>
        /// Convert the file path from Windows to bash
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertToBash(this FileInfo path)
        {
            var splt = path.FullName.Split(':');
            var rootPath = splt[1].Replace('\\', '/');
            return $"/mnt/{splt[0].ToLower()}{rootPath}";
        }

        public static string ConvertToBash(this DirectoryInfo path)
        {
            var splt = path.FullName.Split(':');
            var rootPath = splt[1].Replace('\\', '/');
            return $"/mnt/{splt[0].ToLower()}{rootPath}";
        }

        /// <summary>
        /// Return a new uri that is the same as the old uri, but using the localwin guy specifier
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Uri AsRemoteBashUri(this Uri source)
        {
            return new Uri($"remotebash://tev.machines/{source.LocalPath}");
        }

    }
}
