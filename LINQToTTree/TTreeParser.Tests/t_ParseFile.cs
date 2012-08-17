using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace TTreeParser.Tests
{
    /// <summary>
    /// Some complete file tests
    /// </summary>
    [TestClass]
    public class t_ParseFile
    {
        [TestInitialize]
        public void LoadItUp()
        {
            ROOTNET.NTApplication.GetApplications();
            ROOTNET.NTSystem.gSystem.Load("libRIO");
            SimpleLogging.ResetLogging();
        }

        [TestMethod]
        [DeploymentItem("full-mcfile.root")]
        public void TestPythiaFullFile()
        {
            var f = new FileInfo("full-mcfile.root");
            Assert.IsTrue(f.Exists, "input file is there");
            var testit = new ParseTFile();
            var r = testit.ParseFile(f).ToArray();

            // This next line will throw if the classes have the same name.
            var classMap = r.ToDictionary(c => c.Name, c => c);
        }
    }
}
