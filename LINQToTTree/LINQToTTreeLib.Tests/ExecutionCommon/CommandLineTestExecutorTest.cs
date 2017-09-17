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
            Assert.AreEqual(2000, (int)h.GetBinContent(1), "Answer from query");
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
        public void LocalWinCmdLineSendObjectsToSelector()
        {
            Assert.Inconclusive("Send an input histogram out there");
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
        public void LocalWinCmdLineCheckDebugDumps()
        {
            // When we run in debug mode, make sure the command line dumps are there.
            Assert.Inconclusive();
        }

        /// <summary>
        /// Create a dirt simple query environment so we can see what running is like.
        /// </summary>
        /// <returns></returns>
        private ExecutionEnvironment CreateSimpleEnvironment()
        {
            var result = new ExecutionEnvironment();
            result.RootFiles = new[] { CreateDatasetRef("LINQTest") };

            return result;
        }

        /// <summary>
        /// Creates the query for a simple execution file.
        /// </summary>
        /// <returns></returns>
        private ExecutionEnvironment CreateSimpleQueryEnvironment()
        {
            var result = CreateSimpleEnvironment();

            result.TreeName = "btagd3pd";

            return result;
        }

        /// <summary>
        /// Create a reference to a dataset that uses the local windows cmdline.
        /// </summary>
        /// <param name="dsName"></param>
        /// <returns></returns>
        static Uri CreateDatasetRef(string dsName)
        {
            return new Uri(string.Format("localwin://{0}", dsName));
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
