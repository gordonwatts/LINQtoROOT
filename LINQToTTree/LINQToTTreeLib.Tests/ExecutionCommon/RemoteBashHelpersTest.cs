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
    [DeploymentItem("testmachine.txt")]
    public class RemoteBashHelpersTest
    {
        /// <summary>
        /// This string should be present in the setupATLAS dump
        /// </summary>
        private const string LoginScreenMagicText = "show this menu";

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
        public async Task BashRunBasicCommand()
        {
            string bashCmds = "ls\n";

            List<string> results = new List<string>();
            RemoteBashExecutor.AddLogEndpoint(s => results.Add(s));

            await RemoteBashHelpers.RunBashCommandAsync(File.ReadAllLines("testmachine.txt").First(), 
                "testmeout", bashCmds, s => Console.WriteLine(s), verbose: true);

            Assert.AreNotEqual(0, results.Count);
        }

        [TestMethod]
        public async Task BashRunSimpleROOT()
        {
            var cmds = new StringBuilder();
            cmds.AppendLine("{TH1F *h = new TH1F(\"hi\", \"there\", 10, 0.0, 10.0);");
            cmds.AppendLine("h->Print();}");

            List<string> results = new List<string>();
            RemoteBashExecutor.AddLogEndpoint(s => results.Add(s));

            await RemoteBashHelpers.RunROOTInBashAsync(File.ReadAllLines("testmachine.txt").First(),
                "test", cmds.ToString(), new System.IO.DirectoryInfo(System.IO.Path.GetTempPath()));

            Assert.AreNotEqual(0, results.Count);
        }

        [TestMethod]
        public async Task CheckForNoLoginInfo()
        {
            var cmds = new StringBuilder();
            cmds.AppendLine("{TH1F *h = new TH1F(\"hi\", \"there\", 10, 0.0, 10.0);");
            cmds.AppendLine("h->Print();}");

            var results = new List<string>();
            await RemoteBashHelpers.RunROOTInBashAsync(File.ReadAllLines("testmachine.txt").First(),
                "test", cmds.ToString(), new System.IO.DirectoryInfo(System.IO.Path.GetTempPath()),
                dumpLine: s => results.Add(s));

            Assert.AreNotEqual(0, results.Count);
            Assert.IsFalse(results.Where(l => l.Contains(LoginScreenMagicText)).Any());
        }

        [TestMethod]
        public async Task CheckForLoginInfo()
        {
            var cmds = new StringBuilder();
            cmds.AppendLine("{TH1F *h = new TH1F(\"hi\", \"there\", 10, 0.0, 10.0);");
            cmds.AppendLine("h->Print();}");

            var results = new List<string>();
            await RemoteBashHelpers.RunROOTInBashAsync(File.ReadAllLines("testmachine.txt").First(),
                "test", cmds.ToString(), new System.IO.DirectoryInfo(System.IO.Path.GetTempPath()),
                dumpLine: s => results.Add(s), verbose: true);

            Assert.AreNotEqual(0, results.Count);
            Assert.IsTrue(results.Where(l => l.Contains(LoginScreenMagicText)).Any());
        }

        [TestMethod]
        public async Task BashRunSimpleROOTWithInputFile()
        {
            var f = ROOTNET.NTFile.Open("junk.root", "RECREATE");
            f.Close();

            var cmds = new StringBuilder();
            cmds.AppendLine("{TFile *f = TFile::Open(\"junk.root\", \"READ\"); exit(0);}");
            List<string> results = new List<string>();
            RemoteBashExecutor.AddLogEndpoint(s => results.Add(s));

            await RemoteBashHelpers.RunROOTInBashAsync(File.ReadAllLines("testmachine.txt").First(),
                "test", cmds.ToString(), new System.IO.DirectoryInfo(System.IO.Path.GetTempPath()),
                filesToSend: new[] { new FileInfo("junk.root") });

            // Basically, there should be no crash.
            Assert.IsFalse(results.Where(s => s.Contains("Error")).Any());
        }

        [TestMethod]
        public async Task BashRunSimpleROOTWithRelatuveInputFile()
        {
            var loc = new FileInfo("special/junk.root");
            if (loc.Directory.Exists)
            {
                loc.Directory.Delete(true);
            }
            loc.Directory.Create();
            loc.Directory.Refresh();
            var f = ROOTNET.NTFile.Open(loc.FullName, "RECREATE");
            f.Close();

            var cmds = new StringBuilder();
            cmds.AppendLine("{TFile *f = TFile::Open(\"special/junk.root\", \"READ\"); exit(0);}");
            List<string> results = new List<string>();
            RemoteBashExecutor.AddLogEndpoint(s =>
            {
                results.Add(s);
                Console.WriteLine(s);
            });

            await RemoteBashHelpers.RunROOTInBashAsync(File.ReadAllLines("testmachine.txt").First(),
                "test", cmds.ToString(), new DirectoryInfo("."),
                filesToSend: new[] { loc });

            // Basically, there should be no crash.
            Assert.IsFalse(results.Where(s => s.Contains("Error")).Any());
        }

        [TestMethod]
        public async Task BashRunSimpleROOTWithRelatuveOutputFile()
        {
            var loc = new FileInfo("special/junk.root");
            if (loc.Directory.Exists)
            {
                loc.Directory.Delete(true);
            }
            loc.Directory.Refresh();

            var cmds = new StringBuilder();
            cmds.AppendLine("{gSystem->mkdir(\"special\"); TFile *f = TFile::Open(\"special/junk.root\", \"RECREATE\"); exit(0);}");
            List<string> results = new List<string>();
            RemoteBashExecutor.AddLogEndpoint(s =>
            {
                results.Add(s);
                Console.WriteLine(s);
            });

            await RemoteBashHelpers.RunROOTInBashAsync(File.ReadAllLines("testmachine.txt").First(),
                "test", cmds.ToString(), new DirectoryInfo("."),
                filesToReceive: new[] { loc });

            // Basically, there should be no crash.
            Assert.IsFalse(results.Where(s => s.Contains("Error")).Any());

            // And the file should come back
            loc.Refresh();
            Assert.IsTrue(loc.Exists);
        }
    }
}
