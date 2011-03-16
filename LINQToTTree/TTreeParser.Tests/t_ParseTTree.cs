using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTreeDataModel;
using TTreeParser;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    ///This is a test class for ParseTTreeTest and is intended
    ///to contain all ParseTTreeTest Unit Tests
    ///</summary>
    [TestClass]
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

        [TestInitialize]
        public void LoadItUp()
        {
            ROOTNET.NTApplication.GetApplications();
            ROOTNET.NTSystem.gSystem.Load("libRIO");
        }

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

            CheckSerialization(result, "GenerateClassesEmptyTree");
        }

        /// <summary>
        /// Check to make sure that we can serialize everything.
        /// </summary>
        /// <param name="result"></param>
        private void CheckSerialization(ROOTClassShell[] result, string testName)
        {
            FileInfo outputFile = new FileInfo(testName + ".xml");
            XmlSerializer xmlTrans = new XmlSerializer(typeof(ROOTClassShell[]));

            using (var writer = outputFile.CreateText())
            {
                xmlTrans.Serialize(writer, result);
                writer.Close();
            }
        }

        [TestMethod]
        public void GenerateClassesTestSingleBasicType()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateWithIntOnly(5);
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "should only be top level class");
            var item = result[0];
            Assert.AreEqual(5, item.Items.Count(), "Expected 5 items in there");
            Assert.IsTrue(item.Items.All(i => i.ItemType == "int"), "Not everything is an int!");

            CheckSerialization(result, "GenerateClassesTestSingleBasicType");
        }

        [TestMethod]
        public void GenerateClassesTestBadNameEvent()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateWithIntName("event");
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            var item = result[0];
            var first = item.Items[0];
            var st = first as ItemSimpleType;
            Assert.AreEqual("event_", st.Name, "Specialized name not correct");
        }

        [TestMethod]
        public void GenerateClassesTestTLorentzVector()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateWithTLZOnly(5);
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "should only be top level class");
            var item = result[0];
            Assert.AreEqual(5, item.Items.Count(), "Expected 5 items in there");
            Assert.IsTrue(item.Items.All(i => i.ItemType == "ROOTNET.Interface.NTLorentzVector"), "Not everything is a TLZ!");

            CheckSerialization(result, "GenerateClassesTestTLorentzVector");
        }

#if false
        /// Commented out b/c it causes a DLL to be kept in.
        [TestMethod]
        public void GenerateClassWithCustomROOTClass()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateCustomROOTClassTree();
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "should only be top level class");
            var item = result[0];
            Assert.AreEqual(1, item.Items.Count, "Incorrect number of leaves detected");
            var leaf = item.Items[0];
            Assert.IsInstanceOfType(leaf, typeof(ItemROOTClass), "leaf should be a root class!");
            Assert.AreEqual("ROOTNET.Interface.NATestRootClass", leaf.ItemType, "bad item type");
        }
#endif

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void GenerateClassesTestNoClassInfo()
        {
            /// There are some classes in here that ROOT dosen't know about - so we
            /// need to detect that and "bomb".
            var f = new ROOTNET.NTFile("../../../TTreeParser.Tests/ComplexNtupleTestInput.root", "READ");
            var t = f.Get("btag") as ROOTNET.Interface.NTTree;
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();
        }

#if false
        /// This test relyies on having the ntuple stuff translated, something we are doing in the
        /// demos now, not here in the actual test cases - where we do everything ad-hoc.
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

            CheckSerialization(result, "GenerateClassesTestComplexUnknownObjects");
            /// There should be 3 classes in there!
            Assert.AreEqual(3, result.Length, "incorrect # of classes parsed");

        }
#endif

        [TestMethod]
        public void GenerateClassesTestVectorIntAndDoubleAndShort()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateVectorTree();
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "expected only the top level class to come back");

            string[] possibleTypes = new string[] {
                "int[]",
                "double[]",
                // "short[]", // Not known to root dictionary by default.
                "bool[]"
                //"float[]"  // Weird - it doesn't work either.
            };

            Assert.IsTrue(result[0].Items.All(i => possibleTypes.Contains(i.ItemType)), "Some responses not correct!");
            Assert.AreEqual(possibleTypes.Length, result[0].Items.Select(i => i.ItemType).Distinct().Count(), "Incorrect number of branches found!");

            CheckSerialization(result, "GenerateClassesTestVectorIntAndDoubleAndShort");
        }

        [TestMethod]
        public void TestGenerateSimpleItems()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateSingleItemTree();
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "expected only the top level class to come back");

            string[] possibleTypes = new string[] {
                "int",
                "uint",
                "short",
                "ushort",
                "long",
                "ulong",
                "bool"
            };

            Assert.IsTrue(result[0].Items.All(i => possibleTypes.Contains(i.ItemType)), "Some responses not correct!");
            Assert.AreEqual(possibleTypes.Length, result[0].Items.Select(i => i.ItemType).Distinct().Count(), "Incorrect number of branches found!");

            CheckSerialization(result, "GenerateClassesTestVectorIntAndDoubleAndShort");
        }

        [TestMethod]
        public void TestGenerateTemplateTypedef()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateVectorTreeWithTypedef();
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "expected only the top level class to come back");

            string[] possibleTypes = new string[] {
                "int[]"
            };

            Assert.IsTrue(result[0].Items.All(i => possibleTypes.Contains(i.ItemType)), "Some responses not correct!");
            Assert.AreEqual(possibleTypes.Length, result[0].Items.Select(i => i.ItemType).Distinct().Count(), "Incorrect number of branches found!");

            CheckSerialization(result, "GenerateClassesTestVectorIntAndDoubleAndShort");
        }

        [TestMethod]
        public void TestGenerateWithOddVectorTypes()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateWithOddTypes();
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "expected only the top level class to come back");

            string[] possibleTypes = new string[] {
                "uint[]",
                "int[]"
            };

            Assert.IsTrue(result[0].Items.All(i => possibleTypes.Contains(i.ItemType)), "Some responses not correct!");
            Assert.AreEqual(possibleTypes.Length, result[0].Items.Select(i => i.ItemType).Distinct().Count(), "Incorrect number of branches found!");

            CheckSerialization(result, "GenerateClassesTestVectorIntAndDoubleAndShort");
        }

        [TestMethod]
        public void GenerateClassesTestVectorVector()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateVectorVectorTree();
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "expected only the top level class to come back");
            Assert.AreEqual(1, result[0].Items.Count(), "incorrect # of items found for this guy");

            Assert.AreEqual("double[][]", result[0].Items.First().ItemType, "improper double vector type!");

            CheckSerialization(result, "GenerateClassesTestVectorVector");
        }

        [TestMethod]
        public void GenerateClassesTestListOfLeaves()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateListOfLeavesTree();
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "expected only the top level class to come back");
            Assert.AreEqual(3, result[0].Items.Count(), "incorrect # of items found for this guy");

            string[] leafTypes = new string[] { "int", "float", "double" };

            Assert.IsTrue(result[0].Items.All(i => leafTypes.Contains(i.ItemType)), "bad item type in the list!");
            Assert.AreEqual(leafTypes.Length, result[0].Items.Select(l => l.ItemType).Distinct().Count(), "Bad # of differen tytpes");

            CheckSerialization(result, "GenerateClassesTestListOfLeaves");
        }

#if false
        /// This test - the vector<TLZ> doesn't seem to be a known dictionary!
        [TestMethod]
        public void GenerateClassesTestListOfTLZ()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateWithTLZVector();
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "Only the top level class was expected");
            Assert.AreEqual(1, result[0].Items.Count(), "only one TLZ should have been in there");
            var i = result[0].Items.First();
            Assert.AreEqual("TLorentzVector[]", i.ItemType, "incorrect item type for vector of tlz");
        }
#endif
        [TestMethod]
        public void TestBasicProxyGeneration()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateWithIntOnly(5);
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            FileInfo fhpp = new FileInfo("ntuple_dude.h");

            Assert.IsTrue(fhpp.Exists, "check for hpp file existance");

            Assert.AreEqual(fhpp.FullName, result[0].NtupleProxyPath, "ntuple proxy path incorrect");
        }

        [TestMethod]
        public void TestBasicProxyGenerationSpecial()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateWithIntOnly(5);

            var p = new ParseTTree();
            DirectoryInfo newdir = new DirectoryInfo("./TestBasicProxyGenerationSpecial");
            if (!newdir.Exists)
                newdir.Create();

            p.ProxyGenerationLocation = newdir;

            var result = p.GenerateClasses(t).ToArray();

            FileInfo fhpp = new FileInfo(newdir.FullName + "\\ntuple_dude.h");

            Assert.IsTrue(fhpp.Exists, "check for hpp file existance");

            Assert.AreEqual(fhpp.FullName, result[0].NtupleProxyPath, "ntuple proxy path incorrect");
        }

        /// <summary>
        /// WARNING - you must start devenv in a vs 2010 command line window so that "cl" is availible.
        /// </summary>
        [TestMethod]
        public void TestProxyBuild()
        {
            /// Make sure the header file that gets generated is basically "ok" for use. These are simple tests.
            var t = TTreeParserCPPTests.CreateTrees.CreateWithIntOnly(5);
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();
            FileInfo fhpp = new FileInfo("ntuple_dude.h");

            FileInfo cfile = new FileInfo("TestProxyBuild.C");
            using (var writer = cfile.CreateText())
            {
                writer.WriteLine("#include \"ntuple_dude.h\"");
                writer.Close();
            }

            /// Make sure that ROOT has been correctly initalized!

            ROOTNET.NTApplication.CreateApplication();

            /// Now run the compile

            var compile = ROOTNET.NTSystem.gSystem.CompileMacro("TestProxyBuild.C");
            Assert.AreEqual(1, compile, "compile error for built macro - make sure that cl is a good command by starting devenv with vs command line!!");
        }

        [TestMethod]
        public void TestProxyGenerationContents()
        {
            /// Make sure the header file that gets generated is basically "ok" for use. These are simple tests.
            var t = TTreeParserCPPTests.CreateTrees.CreateWithIntOnly(5);
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();
            FileInfo fhpp = new FileInfo("ntuple_dude.h");

            //Assert.IsFalse(CheckForLineContaining(fhpp, "junk_macro_parsettree.C"), "Checking for junk_macro in the file");
        }

        /// <summary>
        /// Check to see if this file contains a certian string in any of its lines.
        /// </summary>
        /// <param name="fhpp"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool CheckForLineContaining(FileInfo fhpp, string lineContents)
        {
            var found = from l in ReadFileLines(fhpp)
                        where l.Contains(lineContents)
                        select l;

            Trace.WriteLine("Looking for string '" + lineContents + "'");
            bool foundIt = false;
            foreach (var item in found)
            {
                foundIt = true;
                Trace.WriteLine("  " + item);
            }
            return foundIt;
        }

        /// <summary>
        /// IEnumerable line reader
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private IEnumerable<string> ReadFileLines(FileInfo f)
        {
            using (var reader = f.OpenText())
            {
                while (!reader.EndOfStream)
                {
                    var l = reader.ReadLine();
                    if (l != null)
                        yield return l;
                }
            }
        }
    }
}
