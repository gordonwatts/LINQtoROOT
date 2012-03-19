using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTreeDataModel;

namespace TTreeParser.Tests
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
            SimpleLogging.ResetLogging();
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
            p.ProxyGenerationLocation = new DirectoryInfo(".");
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "should only be top level class");
            var item = result[0];
            Assert.AreEqual(5, item.Items.Count(), "Expected 5 items in there");
            Assert.IsTrue(item.Items.All(i => i.ItemType == "int"), "Not everything is an int!");

            CheckSerialization(result, "GenerateClassesTestSingleBasicType");
        }

        [TestMethod]
        public void GenerateClassesTestCPPName()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateTreeWithIndexedSimpleVector(20);
            var p = new ParseTTree();
            p.ProxyGenerationLocation = new DirectoryInfo(".");
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "should only be top level class");
            var item = result[0];
            Assert.AreEqual(2, item.Items.Count(), "# of items found");
            Assert.AreEqual(1, item.Items.Where(i => i.ItemType == "int").Count(), "# found variables");
            var i1 = item.Items.Where(i => i.ItemType == "int").First();
            Assert.AreEqual("n", i1.Name, "index name");
            Assert.AreEqual(1, item.Items.Where(i => i.ItemType == "int[]").Count(), "# of int[] variables");
            var i2 = item.Items.Where(i => i.ItemType == "int[]").First();
            Assert.AreEqual("arr", i2.Name, "arr name");
            var i2asA = i2 as ItemCStyleArray;
            Assert.AreEqual(1, i2asA.Indicies.Count, "# of indicies");
            Assert.AreEqual("n", i2asA.Indicies[0].indexBoundName, "index name");
            Assert.AreEqual(0, i2asA.Indicies[0].indexPosition, "index name");
            Assert.IsFalse(i2asA.Indicies[0].indexConst, "index const");

            CheckSerialization(result, "GenerateClassesTestCPPName");
        }

        [TestMethod]
        public void GenerateClassesTestCPPName2D()
        {
            // Test an array that is 2D C style

            var t = TTreeParserCPPTests.CreateTrees.CreateTreeWithIndexedSimpleVector2D(20);
            var p = new ParseTTree();
            p.ProxyGenerationLocation = new DirectoryInfo(".");
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "should only be top level class");
            var item = result[0];
            Assert.AreEqual(1, item.Items.Count(), "# of items found");
            var item1 = item.Items[0];
            Assert.AreEqual("int[][]", item1.ItemType, "2d array type");
            Assert.AreEqual("arr", item1.Name, "arr name");

            var i2asA = item1 as ItemCStyleArray;
            Assert.AreEqual(2, i2asA.Indicies.Count, "# of indicies");
            Assert.AreEqual("5", i2asA.Indicies[0].indexBoundName, "index name");
            Assert.AreEqual(0, i2asA.Indicies[0].indexPosition, "index name");
            Assert.IsTrue(i2asA.Indicies[0].indexConst, "index const");
            Assert.AreEqual("4", i2asA.Indicies[1].indexBoundName, "index name");
            Assert.AreEqual(0, i2asA.Indicies[1].indexPosition, "index name");
            Assert.IsTrue(i2asA.Indicies[1].indexConst, "index const");

            CheckSerialization(result, "GenerateClassesTestCPPName");
        }

        [TestMethod]
        public void GenerateClassesTestConstCPPName()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateTreeWithConstIndexedSimpleVector(20);
            var p = new ParseTTree();
            p.ProxyGenerationLocation = new DirectoryInfo(".");
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "should only be top level class");
            var item = result[0];
            Assert.AreEqual(1, item.Items.Count(), "# of items found");
            Assert.AreEqual(1, item.Items.Where(i => i.ItemType == "int[]").Count(), "# of int[] variables");
            var i2 = item.Items.Where(i => i.ItemType == "int[]").First();
            Assert.AreEqual("arr", i2.Name, "arr name");
            var i2asA = i2 as ItemCStyleArray;
            Assert.AreEqual(1, i2asA.Indicies.Count, "# of indicies");
            Assert.AreEqual("20", i2asA.Indicies[0].indexBoundName, "index name");
            Assert.AreEqual(0, i2asA.Indicies[0].indexPosition, "index name");
            Assert.IsTrue(i2asA.Indicies[0].indexConst, "index const");

            CheckSerialization(result, "GenerateClassesTestCPPName");
        }

#if false
        /// Test case doesn't work b/c I don't know how to create a tree with a leaf name that contains a "::".
        [TestMethod]
        public void TestClassWithDoubleColonName()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateTreeWithDoubleColorName();
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            var cls = result[0];
            var item = cls.Items[0];
            Assert.AreEqual("dude__fork", item.Name, "Name not translated correctly");
        }
#endif

        [TestMethod]
        public void TestSingleBasicTypeIsUngrouped()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateWithIntOnly(5);
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            ///
            /// See if the ntuple output file that groups things exists
            /// 

            using (var reader = File.OpenText(result[0].UserInfoPath))
            {
                var parser = new XmlSerializer(typeof(TTreeUserInfo));
                var classInfo = parser.Deserialize(reader) as TTreeUserInfo;
                Assert.IsNotNull(classInfo, "Unable to read back the user info");
                Assert.AreEqual(1, classInfo.Groups.Length, "# of groups");
                var grp = classInfo.Groups[0];
                Assert.AreEqual("ungrouped", grp.Name, "group name");
                Assert.AreEqual(5, grp.Variables.Length, "# of group variables");
                Assert.AreEqual("item_", grp.Variables[0].NETName, "first variable name incorrect");
            }
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

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        [DeploymentItem("ComplexNtupleTestInput.root")]
        public void GenerateClassesTestNoClassInfo()
        {
            /// There are some classes in here that ROOT dosen't know about - so we
            /// need to detect that and "bomb".
            var f = new ROOTNET.NTFile("ComplexNtupleTestInput.root", "READ");
            Assert.IsTrue(f.IsOpen(), "Test file not found");
            var t = f.Get("btag") as ROOTNET.Interface.NTTree;
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();
        }

        [TestMethod]
        [DeploymentItem("atest.root")]
        public void TestTreesWithPounds()
        {
            var f = new ROOTNET.NTFile("atest.root", "READ");
            var t = f.Get("##Shapes") as ROOTNET.Interface.NTTree;
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "# of classes");
            var obj = result[0];
            Assert.IsFalse(obj.NtupleProxyPath.Contains("#"), "proxy path: " + obj.NtupleProxyPath);
            Assert.IsFalse(obj.UserInfoPath.Contains("#"), "user path: " + obj.UserInfoPath);
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
            p.ProxyGenerationLocation = new DirectoryInfo(".");
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

#if false
        ///
        /// The problem with this test is that the vector<>'s are never written out as #pragma's.
        /// So this can never be tested! :(
        ///
        [TestMethod]
        public void TestForSTLVectorPickup()
        {
            ///
            /// If a tree contains a vector<float> or similar, then that will generate
            /// a #pragma link and link6 line. We need to make sure that our code correctly
            /// removes that from the .h file and also adds them to the list of variables
            /// that should be added later. This is due to a bug in the ROOT on windows that
            /// means unloading the query after using one of these becomes unsafe (and unstable!).
            ///

            var t = TTreeParserCPPTests.CreateTrees.CreateVectorVectorTree();
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.IsNotNull(result[0].ClassesToGenerate, "Expected some classes to generate");
            Assert.AreEqual(3, result[0].ClassesToGenerate.Length, "expected 3 different vector class references");
            Assert.IsTrue(result[0].ClassesToGenerate.All(c => c.includeFiles == "vector"), "vector needed to be the include file");

            var names = new List<string>()
            {
                "vector<int>", "vector<short>", "vector<bool>"
            };
            Assert.IsTrue(result[0].ClassesToGenerate.All(c => names.Contains(c.classSpec)), "Some bad class spec names lifted from C++");

            var fi = new FileInfo(result[0].NtupleProxyPath);
            using (var reader = fi.OpenText())
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    Assert.IsFalse(line.Contains("#pragma") && line.Contains("vector"), "line '" + line + "' seems to have a bad pragma");
                }
            }
        }
#endif

        [TestMethod]
        public void TestGenerateSimpleItems()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateSingleItemTree();
            var p = new ParseTTree();
            p.ProxyGenerationLocation = new DirectoryInfo(".");
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
        public void TestGenerateWithStringVectorTypes()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateWithStringTypes();
            var p = new ParseTTree();
            var result = p.GenerateClasses(t).ToArray();

            Assert.AreEqual(1, result.Length, "Empty ntuple should have fired");
            Assert.AreEqual(0, result[0].Items.Count, "Expected nothing to be pasred");
        }

        [TestMethod]
        public void GenerateClassesTestVectorVector()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateVectorVectorTree();
            var p = new ParseTTree();
            p.ProxyGenerationLocation = new DirectoryInfo(".");
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
            p.ProxyGenerationLocation = new DirectoryInfo(".");
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
        public void TestBasicProxyAndUserInfoGeneration()
        {
            var t = TTreeParserCPPTests.CreateTrees.CreateWithIntOnly(5);
            var p = new ParseTTree();
            p.ProxyGenerationLocation = new DirectoryInfo(".");
            var result = p.GenerateClasses(t).ToArray();

            FileInfo fhpp = new FileInfo("ntuple_dude.h");

            Assert.IsTrue(fhpp.Exists, "check for hpp file existance");

            Assert.AreEqual(fhpp.FullName, result[0].NtupleProxyPath, "ntuple proxy path incorrect");
            Assert.IsTrue(File.Exists(result[0].UserInfoPath), "user info file missing");
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

        [TestMethod]
        [DeploymentItem("EVNT-short.root")]
        public void TestComplexObjectATLASMCFile()
        {
            // Do we correctly parse an ATLAS MC file?
            var f = ROOTNET.NTFile.Open("EVNT-short.root", "READ");
            var t = f.Get("CollectionTree") as ROOTNET.Interface.NTTree;

            var p = new ParseTTree();
            var r = p.GenerateClasses(t).ToArray();
            f.Close();

            // Get the root class first.
            var mainClass = r.FindClass("CollectionTree");
            Assert.IsNotNull(mainClass, "CollectionTree class not found");

            // Check that the top level classes are present.
            var eventInfo = mainClass.FindItem("EventInfo_p3_McEventInfo");
            Assert.IsNotNull(eventInfo, "EventInfo_p3_McEventInfo");
            var eventInfoClass = r.FindClass(eventInfo.ItemType);
            Assert.IsNotNull(eventInfoClass, string.Format("Event info class {0} wasn't found in the list", eventInfo.ItemType));
            var mcCollection = mainClass.FindItem("McEventCollection_p4_GEN_EVENT");
            Assert.IsNotNull(mcCollection, "McEventCollection");
            var mcCollectionClass = r.FindClass(mcCollection.ItemType);
            Assert.IsNotNull(mcCollectionClass, string.Format("Mc Collection class {0} wasn't foudn in the list", mcCollection.ItemType));

            // The McEventInfo guy should have one item in it.
            Assert.AreEqual(1, eventInfoClass.Items.Count, "# items in the event info class");
            Assert.AreEqual("m_AllTheData", eventInfoClass.Items[0].Name, "m_AllTheData name");
            Assert.AreEqual("uint[]", eventInfoClass.Items[0].ItemType, "m_AllTheData type");

            // The McCollection has a bunch more stuff in it
            Assert.AreEqual(13, mcCollectionClass.Items.Count, "# items in the McCollection class");
            var rsignalProcessId = mcCollectionClass.FindItem("m_signalProcessId");
            Assert.IsNotNull(rsignalProcessId, "m_signalProcessId item");
            Assert.AreEqual("int[]", rsignalProcessId.ItemType, "m_signalProcessId type");
            var rWeights = mcCollectionClass.FindItem("m_weights");
            Assert.IsNotNull(rWeights, "m_weights item");
            Assert.AreEqual("double[]", rWeights.ItemType, "m_weights type");
        }

#if false
        // We don't know how to do this yet.
        [TestMethod]
        [DeploymentItem("btagobjs.root")]
        public void TestComplexObjectNiceObjectFile()
        {
            // Do we correctly parse an ATLAS MC file?
            var f = ROOTNET.NTFile.Open("btagobjs.root", "READ");
            var t = f.Get("btagging") as ROOTNET.Interface.NTTree;

            var p = new ParseTTree();
            var r = p.GenerateClasses(t).ToArray();

            f.Close();
            Assert.Inconclusive();
        }
#endif

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
