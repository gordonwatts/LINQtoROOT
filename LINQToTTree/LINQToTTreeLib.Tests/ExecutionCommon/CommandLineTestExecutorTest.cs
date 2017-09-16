using LINQToTTreeLib.ExecutionCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
            TestUtils.ResetLINQLibrary();
        }

        /// <summary>
        /// Simple run a file and see what happens
        /// </summary>
        [TestMethod]
        [DeploymentItem("ExecutionCommon\\queryTestSimpleQuery.cxx")]
        [DeploymentItem("ExecutionCommon\\junk_macro_parsettree_CollectionTree.C")]
        [DeploymentItem("ExecutionCommon\\ntuple_CollectionTree.h")]
        public void TestSimpleQuery()
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
}
