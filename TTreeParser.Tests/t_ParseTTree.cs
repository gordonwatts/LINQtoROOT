using TTreeParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ROOTNET.Interface;
using System.Collections.Generic;
using System.Linq;
using TTreeDataModel;

namespace LINQToTTreeLib.Tests
{
    
    
    /// <summary>
    ///This is a test class for ParseTTreeTest and is intended
    ///to contain all ParseTTreeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ParseTTreeTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        /// Make sure if the Tree is stupid that we get back something stupid! :-)
        ///</summary>
        [TestMethod()]
        public void GenerateClassesEmptyTree()
        {
            var emptyT = new ROOTNET.NTTree("dude", "empty");
            var p = new ParseTTree();
            var result = p.GenerateClasses(emptyT).ToArray();

            Assert.AreEqual(1, result.Length, "Expected only the top level class");
            Assert.AreEqual("dude", result[0].Name, "class name incorrect");
            Assert.AreEqual(0, result[0].Items.Count(), "empty tree...");
        }

        [TestMethod]
        public void GenerateClassesTestSingleBasicType()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateWithIntOnly("GenerateClassesTestSingleBasicType.root", 5);
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "should only be top level class");
            var item = result[0];
            Assert.AreEqual(5, item.Items.Count(), "Expected 5 items in there");
            Assert.IsTrue(item.Items.All(i => i.ItemType == "int"), "Not everything is an int!");
        }

        [TestMethod]
        public void GenerateClassesTestTLorentzVector()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateWithTLZOnly("GenerateClassesTestTLorentzVector.root", 5);
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "should only be top level class");
            var item = result[0];
            Assert.AreEqual(5, item.Items.Count(), "Expected 5 items in there");
            Assert.IsTrue(item.Items.All(i => i.ItemType == "ROOTNET.Interface.NTLorentzVector"), "Not everything is a TLZ!");
        }

        [TestMethod]
        public void GenerateClassesTestComplexUnknownObjects()
        {
            var f = new ROOTNET.NTFile("../../../TTreeParser.Tests/ComplexNtupleTestInput.root", "READ");
            Assert.IsTrue(f.IsOpen(), "File wasn't found");
            var t = f.Get("btag") as ROOTNET.Interface.NTTree;
            Assert.IsNotNull(t, "couldn't find the tree");

            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            foreach (var s in result)
            {
                Assert.IsInstanceOfType(s, typeof(ROOTClassShell), "result incorrect type");
                var rc = s as ROOTClassShell;
                if (rc.Name == "BTagJet")
                {
                    Assert.AreEqual(rc.SubClassName, "TLorentzVector");
                    Assert.AreEqual(5, rc.Items.Count());
                }
                else if (rc.Name == "MuonInBJet")
                {
                    Assert.AreEqual(rc.SubClassName, "TLorentzVector");
                    Assert.AreEqual(4, rc.Items.Count(), "# items incorrect for muon in bjet");
                }
                else if (rc.Name == "btag")
                {
                    Assert.IsNull(rc.SubClassName, "Expecting no sub class!");
                    Assert.AreEqual(3, rc.Items.Count(), "# items is incorrect for btag object");
                }
                else
                {
                    Assert.Fail("Invalid class came back: '" + rc.Name + "'.");
                }
            }

            /// There should be 3 classes in there!
            Assert.AreEqual(3, result.Length, "incorrect # of classes parsed");
        }

        [TestMethod]
        public void GenerateClassesTestVectorIntAndDoubleAndShort()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GenerateClassesTestVectorVector()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GenerateClassesTestListOfLeaves()
        {
            Assert.Inconclusive();
        }
    }
}
