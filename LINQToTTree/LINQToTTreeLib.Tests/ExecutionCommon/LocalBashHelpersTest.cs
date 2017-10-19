using LINQToTTreeLib.ExecutionCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Tests.ExecutionCommon
{
    /// <summary>
    /// Check to see if we can do basic ROOT stuff.
    /// </summary>
    [TestClass]
    public class LocalBashHelpersTest
    {
        [TestInitialize]
        public void Setup()
        {
            LocalBashExecutor.ResetCommandLineExecutor();
            LocalBashExecutor.ResetLocalBashExecutor();
        }

        [TestCleanup]
        public void Cleanup()
        {
            LocalBashExecutor.ResetCommandLineExecutor();
            LocalBashExecutor.ResetLocalBashExecutor();
        }

        [TestMethod]
        public void BashRunBasicCommand()
        {
            string bashCmds = "ls\n";

            List<string> results = new List<string>();
            LocalBashExecutor.AddLogEndpoint(s => results.Add(s));

            LocalBashHelpers.RunBashCommand("testmeout", bashCmds, s => Console.WriteLine(s), verbose: true);

            Assert.AreNotEqual(0, results.Count);
        }

        [TestMethod]
        public void BashRunSimpleROOT()
        {
            var cmds = new StringBuilder();
            cmds.AppendLine("TH1F *h = new TH1F(\"hi\", \"there\", 10, 0.0, 10.0);");
            cmds.AppendLine("h->Print();");

            List<string> results = new List<string>();
            LocalBashExecutor.AddLogEndpoint(s => results.Add(s));

            LocalBashHelpers.RunROOTInBash("test", cmds.ToString(), new System.IO.DirectoryInfo(System.IO.Path.GetTempPath()));

            Assert.AreNotEqual(0, results.Count);
        }
    }
}
