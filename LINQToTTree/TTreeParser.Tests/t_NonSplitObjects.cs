using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TTreeParser.Tests
{
    /// <summary>
    /// Test out objects in TTree's that aren't split, but for which there is streamer
    /// information
    /// </summary>
    [TestClass]
    public class t_NonSplitObjects
    {
        [TestInitialize]
        public void LoadItUp()
        {
            ROOTNET.NTApplication.GetApplications();
            ROOTNET.NTSystem.gSystem.Load("libRIO");
            SimpleLogging.ResetLogging();
        }

        ROOTNET.NTTree GetTree(string fname, string treename)
        {
            var f = new ROOTNET.NTFile(fname, "READ");
            Assert.IsTrue(f.IsOpen(), "Test file not found");
            var t = f.Get(treename) as ROOTNET.NTTree;
            Assert.IsNotNull(t, "Tree not found");
            return t;
        }

        [TestMethod]
        [DeploymentItem("short-nonsplit.root")]
        public void TestSimpleParse()
        {
            var t = GetTree("short-nonsplit.root", "CollectionTree");
            //var t = f.Get("btag") as ROOTNET.Interface.NTTree;
            //var p = new ParseTTree();
            //var result = p.GenerateClasses(t).ToArray();
        }
    }
}
