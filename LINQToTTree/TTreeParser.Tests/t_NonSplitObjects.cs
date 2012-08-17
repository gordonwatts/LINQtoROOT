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
            ROOTNET.NTSystem.gSystem.Load("vector");
            SimpleLogging.ResetLogging();
        }

        /// <summary>
        /// Make it easy to load a file.
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="treename"></param>
        /// <returns></returns>
        Tuple<ROOTNET.NTTree, ROOTNET.NTFile> GetTree(string fname, string treename)
        {
            var f = new ROOTNET.NTFile(fname, "READ");
            Assert.IsTrue(f.IsOpen(), "Test file not found");
            var t = f.Get(treename) as ROOTNET.NTTree;
            Assert.IsNotNull(t, "Tree not found");
            return Tuple.Create(t, f);
        }

        [TestMethod]
        [DeploymentItem("short-nonsplit.root")]
        public void TestSimpleParse()
        {
            var t = GetTree("short-nonsplit.root", "CollectionTree");
            var p = new ParseTTree();
            var result = p.GenerateClasses(t.Item1).ToArray();
            Assert.AreEqual(1, result.Where(c => c.IsTopLevelClass).Count(), "# of top level classes");
            Assert.AreEqual(6, result.Count(), "Total number of classes");
            var classMap = result.ToDictionary(i => i.Name, i => i);
            Assert.IsTrue(classMap.ContainsKey("EventInfo_p3"), "EventInfo_p3");
            Assert.IsTrue(classMap.ContainsKey("McEventCollection_p5"), "EventInfo_p3");
            Assert.IsTrue(classMap.ContainsKey("GenParticle_p5"), "EventInfo_p3");
            Assert.IsTrue(classMap.ContainsKey("GenVertex_p5"), "EventInfo_p3");
            Assert.IsTrue(classMap.ContainsKey("GenEvent_p5"), "EventInfo_p3");
            Assert.IsTrue(classMap.ContainsKey("CollectionTree"), "CollectionTree");

            // Check collection tree has the right top level stuff
            var ct = classMap["CollectionTree"];
            Assert.AreEqual(2, ct.Items.Count, "# of items in collection tree");
            var ctitems = ct.Items.ToDictionary(citem => citem.Name, citem => citem);
            Assert.IsTrue(ctitems.ContainsKey("EventInfo_p3_McEventInfo"), "EventInfo_p3_McEventInfo");
            Assert.IsTrue(ctitems.ContainsKey("McEventCollection_p5_GEN_EVENT"), "McEventCollection_p5_GEN_EVENT");
        }
    }
}
