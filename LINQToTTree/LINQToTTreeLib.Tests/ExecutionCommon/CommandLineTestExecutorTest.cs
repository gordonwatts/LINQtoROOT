using LINQToTTreeLib.ExecutionCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LINQToTTreeLib.ExecutionCommon.CommandLineExecutor;
using static LINQToTTreeLib.TTreeQueryExecutorTest;

namespace LINQToTTreeLib.Tests.ExecutionCommon
{
    [TestClass]
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
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestUtils.ResetLINQLibrary();
            MEFUtilities.MyClassDone();
            CommandLineExecutor.ResetCommandLineExecutor();
        }

        /// <summary>
        /// Simple run a file and see what happens
        /// </summary>
        [TestMethod]
        [DeploymentItem("ExecutionCommon\\queryTestSimpleQuery.cxx")]
        [DeploymentItem("ExecutionCommon\\junk_macro_parsettree_CollectionTree.C")]
        [DeploymentItem("ExecutionCommon\\ntuple_CollectionTree.h")]
        public void LocalWinCmdLineSimpleQuery()
        {
            FileInfo runner = CopyToTempDir("queryTestSimpleQuery.cxx");
            CopyToTempDir("junk_macro_parsettree_CollectionTree.C");
            CopyToTempDir("ntuple_CollectionTree.h");
            Assert.IsTrue(runner.Exists, "Main C++ file missing");
            var targetr = new CommandLineExecutor();
            var env = CreateSimpleQueryEnvironment();

            targetr.Environment = env;
            var r = targetr.Execute(runner, new DirectoryInfo(tempDir), null);

            Assert.IsNotNull(r, "nothing came back!");
            Assert.AreEqual(1, r.Count, "# of returned values from query");
            Assert.AreEqual("aInt32_1", r.Keys.First(), "Key name incorrect");
            var o = r["aInt32_1"];
            Assert.IsInstanceOfType(o, typeof(ROOTNET.NTH1I), "return histo type");
            var h = o as ROOTNET.NTH1I;
            Assert.AreEqual(20, (int)h.GetBinContent(1), "Answer from query");
        }

        [TestMethod]
        [DeploymentItem("ExecutionCommon\\queryTestSimpleQuery.cxx")]
        [DeploymentItem("ExecutionCommon\\junk_macro_parsettree_CollectionTree.C")]
        [DeploymentItem("ExecutionCommon\\ntuple_CollectionTree.h")]
        public void LocalWinCmdLineBadCPPGeneration()
        {
            // Make sure a C++ error makes its way back up the line so we can see the error!
            FileInfo runner = CopyToTempDir("queryTestSimpleQuery.cxx");
            using (var app = runner.AppendText())
            {
                app.WriteLine("blah blah blah;");
                app.Close();
            }

            // Get the rest of the stuff in there.
            CopyToTempDir("junk_macro_parsettree_CollectionTree.C");
            CopyToTempDir("ntuple_CollectionTree.h");
            Assert.IsTrue(runner.Exists, "Main C++ file missing");
            var targetr = new CommandLineExecutor();
            var env = CreateSimpleQueryEnvironment();

            targetr.Environment = env;
            Exception err = null;
            try
            {
                var r = targetr.Execute(runner, new DirectoryInfo(tempDir), null);
            } catch (Exception e)
            {
                err = e;
            }

            Assert.IsNotNull(err);
            Assert.IsInstanceOfType(err, typeof(CommandLineExecutionException));
            var cl = (CommandLineExecutionException)err;
            Assert.IsTrue(cl.Message.Contains("blah"), $"word Blah is missing from error message {cl.Message}.");
        }

        [TestMethod]
        public void LocalWinCmdLineDictifyClasses()
        {
            Assert.Inconclusive("Make sure we dictify classes necessary");
        }

        [TestMethod]
        [DeploymentItem(@"Templates\TSelectorTemplate.cxx")]
        public void LocalWinCmdLineSendObjectsToSelector()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Generate a proxy .h file that we can use
            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

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
            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CleanupQuery = false;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        [DeploymentItem(@"Templates\TSelectorTemplate.cxx")]
        public void LocalWinCmdLineCountOperator()
        {
            var rootFile = TestUtils.CreateFileOfInt(20);

            // Generate a proxy .h file that we can use
            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        [DeploymentItem(@"Templates\TSelectorTemplate.cxx")]
        public void LocalWinCmdLineCheckWarningsAndErrors()
        {
            RunSimpleTestForErrorsAndWarnings(exe => { });
        }

        [TestMethod]
        [DeploymentItem(@"Templates\TSelectorTemplate.cxx")]
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

            // Generate a proxy .h file that we can use
            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            ntuple._gProxyFile = proxyFile.FullName;
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

            // Generate a proxy .h file that we can use
            var proxyFile = TestUtils.GenerateROOTProxy(rootFile, "dude");

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            ntuple._gProxyFile = proxyFile.FullName;
            var exe = new TTreeQueryExecutor(new[] { rootFile.AsLocalWinUri() }, "dude", typeof(ntuple), typeof(TestNtupe));

            // Capture the lines
            bool seenCacheInfo = false;
            CommandLineExecutor.AddLogEndpoint(s => seenCacheInfo |= s.ToLower().Contains("cache"));
            exe.CompileDebug = true;
            int result = exe.ExecuteScalar<int>(query);

            Assert.IsTrue(seenCacheInfo);
        }

        [TestMethod]
        public void LocalWinCmdLineOtherTypes()
        {
            Assert.Inconclusive("Test other types of data that go back and forth than histograms - like dictionaries, etc. ");
        }

        /// <summary>
        /// Create a dirt simple query environment so we can see what running is like.
        /// </summary>
        /// <returns></returns>
        private ExecutionEnvironment CreateSimpleEnvironment()
        {
            var result = new ExecutionEnvironment();
            var rootFile = TestUtils.CreateFileOfInt(20);
            result.RootFiles = new[] { rootFile.AsLocalWinUri()};

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
            return new Uri($"localwin://{source.LocalPath}");
        }
    }
}
