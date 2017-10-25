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
    public class RemoteBashHelpersTest
    {
        [TestInitialize]
        public void Setup()
        {
            RemoteBashExecutor.ResetCommandLineExecutor();
            RemoteBashExecutor.ResetRemoteBashExecutor();
        }

        [TestCleanup]
        public void Cleanup()
        {
            RemoteBashExecutor.ResetCommandLineExecutor();
            RemoteBashExecutor.ResetRemoteBashExecutor();
        }

        [TestMethod]
        public void BashRunBasicCommand()
        {
            string bashCmds = "ls\n";

            List<string> results = new List<string>();
            RemoteBashExecutor.AddLogEndpoint(s => results.Add(s));

            RemoteBashHelpers.RunBashCommand("testmeout", bashCmds, s => Console.WriteLine(s), verbose: true);

            Assert.AreNotEqual(0, results.Count);
        }

        [TestMethod]
        public void BashRunSimpleROOT()
        {
            var cmds = new StringBuilder();
            cmds.AppendLine("{TH1F *h = new TH1F(\"hi\", \"there\", 10, 0.0, 10.0);");
            cmds.AppendLine("h->Print();}");

            List<string> results = new List<string>();
            RemoteBashExecutor.AddLogEndpoint(s => results.Add(s));

            RemoteBashHelpers.RunROOTInBash("test", cmds.ToString(), new System.IO.DirectoryInfo(System.IO.Path.GetTempPath()));

            Assert.AreNotEqual(0, results.Count);
        }

        [TestMethod]
        public void BashRunSimpleROOTWithInputFile()
        {
            var f = ROOTNET.NTFile.Open("junk.root", "RECREATE");
            f.Close();

            var cmds = new StringBuilder();
            cmds.AppendLine("{TFile *f = TFile::Open(\"junk.root\", \"READ\"); exit(0);}");
            List<string> results = new List<string>();
            RemoteBashExecutor.AddLogEndpoint(s => results.Add(s));

            RemoteBashHelpers.RunROOTInBash("test", cmds.ToString(), new System.IO.DirectoryInfo(System.IO.Path.GetTempPath()),
                filesToSend: new[] { new FileInfo("junk.root") });

            // Basically, there should be no crash.
            Assert.IsFalse(results.Where(s => s.Contains("Error")).Any());
        }
    }
}
