// <copyright file="ClassGeneratorTest.cs" company="Microsoft">Copyright © Microsoft 2011</copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTreeDataModel;

namespace TTreeClassGenerator
{
    /// <summary>This class contains parameterized unit tests for ClassGenerator</summary>
    [TestClass]
    public partial class ClassGeneratorTest
    {
#if false
        /// <summary>Test stub for GenerateClasss(FileInfo, FileInfo, String)</summary>
        [PexMethod(MaxBranches = 20000)]
        public void GenerateClasss(
            [PexAssumeUnderTest]ClassGenerator target,
            int inputChoice,
            int outputChoice,
            string namespaceName
        )
        {
            FileInfo inputXMLFile, outputCSFile;

            if (inputChoice == 1)
            {
                inputXMLFile = new FileInfo(@"..\..\..\TTreeClassGenerator.Tests");
            }
            else
            {
                inputXMLFile = null;
            }

            if (outputChoice == 1)
            {
                outputCSFile = new FileInfo("output.cs");
            }
            else
            {
                outputCSFile = null;
            }

            target.GenerateClasss(inputXMLFile, outputCSFile, namespaceName);

            Assert.IsTrue(outputCSFile.Exists, "Output file should exist if we ran GenerateClass ok!");
        }
#endif
        public void GenerateClassFromClasses(
            ClassGenerator target,
            int outputChoice,
            int numExtraFiles,
            int numExtraFilesToCreate,
            int extraFileIndexNull,
            int proxyPathChoice,
            string nameSName,
            int NumObjectCollection)
        {
            if (numExtraFiles < 0
                || numExtraFilesToCreate < 0
                || extraFileIndexNull < 0
                || outputChoice < 0
                || proxyPathChoice < 0
                || NumObjectCollection < 0)
                return;

            ///
            /// Kill off the directory we might have left behind from a previous run, and create a new one.
            /// 

            DirectoryInfo testDir = new DirectoryInfo(".\\GenerateClassFromClasses");
            if (testDir.Exists)
            {
                testDir.Delete(true);
            }
            testDir.Create();

            ///
            /// Setup the input stuff so Pex can play
            /// 

            FileInfo outputCSFile;
            if (outputChoice == 1)
            {
                outputCSFile = new FileInfo(testDir.FullName + "\\output.cs");
            }
            else
            {
                outputCSFile = null;
            }

            ROOTClassShell[] objCollect = null;
            if (NumObjectCollection > 0)
            {
                List<ROOTClassShell> objs = new List<ROOTClassShell>();

                for (int i = 0; i < NumObjectCollection; i++)
                {
                    ROOTClassShell rcs = new ROOTClassShell();
                    rcs.Name = "dude_" + i.ToString();

                    for (int j = 0; j < i; j++)
                    {
                        IClassItem item = null;
                        switch (NumObjectCollection % 4)
                        {
                            case 0:
                                item = null;
                                break;

                            case 1:
                                var itm = new ItemSimpleType() { ItemType = "int" };
                                item = itm;
                                break;

                            case 2:
                                var itmv = new ItemVector() { ItemType = "int[]" };
                                item = itmv;
                                break;

                            case 3:
                                var itmr = new ItemROOTClass() { ItemType = "TLorentzVector" };
                                item = itmr;
                                break;
                        }
                        if (item != null)
                            item.Name = "item_" + j.ToString();
                        rcs.Items.Add(item);
                    }
                    objs.Add(rcs);
                }

                if (NumObjectCollection > 0)
                {
                    if (proxyPathChoice == 1)
                    {
                        var proxyFile = new FileInfo(testDir.FullName + "\\GenerateClassFromClasses_" + proxyPathChoice.ToString() + ".h");
                        using (var w = proxyFile.CreateText())
                        {
                            w.WriteLine("hi");
                            w.Close();
                        }
                        objs[0].NtupleProxyPath = proxyFile.FullName;
                    }

                    if (proxyPathChoice == 2)
                    {
                        var proxyFile = new FileInfo(testDir.FullName + "\\GenerateClassFromClasses_" + proxyPathChoice.ToString() + ".h");
                        objs[0].NtupleProxyPath = proxyFile.FullName;
                    }

                    if (proxyPathChoice == 3)
                    {
                        var proxyFile = new FileInfo(testDir.FullName + "\\GenerateClassFromClasses_" + proxyPathChoice.ToString() + ".h");
                        using (var w = proxyFile.CreateText())
                        {
                            w.WriteLine("hi");
                            w.Close();
                        }
                        foreach (var item in objs)
                        {
                            item.NtupleProxyPath = proxyFile.FullName;
                        }
                    }
                }
                objCollect = objs.ToArray();
            }

            ///
            /// Create the final object, and any extra files needed!
            /// 

            NtupleTreeInfo info = new NtupleTreeInfo() { Classes = objCollect };

            info.ClassImplimintationFiles = (from c in Enumerable.Range(0, numExtraFiles)
                                             let f = new FileInfo(testDir.FullName + "\\GenerateClassFromClasses_extra_" + c.ToString() + ".cpp")
                                             select f.FullName
                                                 ).ToArray();

            int maxFilesToCreate = numExtraFilesToCreate > numExtraFiles ? numExtraFiles : numExtraFilesToCreate;
            for (int i = 0; i < maxFilesToCreate; i++)
            {
                using (var w = File.CreateText(info.ClassImplimintationFiles[i]))
                {
                    w.WriteLine();
                    w.Close();
                }
            }

            if (extraFileIndexNull < numExtraFiles)
            {
                info.ClassImplimintationFiles[extraFileIndexNull] = null;
            }

            ///
            /// Ok, do the investigation
            /// 

            target.GenerateClasss(info, outputCSFile, nameSName);

            Assert.IsFalse(info.Classes.Any(c => c.NtupleProxyPath == null), "No null proxy paths allowed");
            Assert.IsFalse(info.Classes.Any(c => !File.Exists(c.NtupleProxyPath)), "proxy files must exist");

            Assert.IsFalse(info.ClassImplimintationFiles.Any(c => c == null), "no null implementation files allowed");
            Assert.IsFalse(info.ClassImplimintationFiles.Any(c => !File.Exists(c)), "all implimntation files must exist");

            /// Check that all the ntuple proxy guys and the temp file guys appear in the file

            foreach (var item in info.Classes.Where(c => c.IsTopLevelClass))
            {
                Assert.IsTrue(FindInFile(outputCSFile, item.NtupleProxyPath), "Could not find the proxy path '" + item.NtupleProxyPath + "'");
            }
            foreach (var item in info.ClassImplimintationFiles)
            {
                Assert.IsTrue(FindInFile(outputCSFile, item), "coul dnot find impl file '" + item + "'");
            }
        }

        [TestMethod]
        public void TestNoGroups()
        {
            /// Create simple user info - but don't do anything with it!
            ItemSimpleType simple = new ItemSimpleType("var1", "int[]");
            Assert.IsFalse(simple.NotAPointer, "not a pointer");
            FileInfo proxyFile = new FileInfo("TestNoGroupsProxy.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }
            ROOTClassShell mainClass = new ROOTClassShell("TestSimpleRename") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(simple);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo() { Groups = new ArrayGroup[] { new ArrayGroup() { Name = "ungrouped", Variables = new VariableInfo[] { new VariableInfo() { NETName = "var1", TTreeName = "var1" } } } } };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestNoGroups.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestSimpleRename", userinfo } });

            /// Look through this to see if we can make sure there are no renames!
            Assert.IsFalse(FindInFile(outputFile, "RenameVariable"), "We saw a rename!");
            Assert.IsFalse(FindInFile(outputFile, "ungrouped"), "group found");
        }

        [TestMethod]
        public void TestSimpleRename()
        {
            /// Create simple user info - but don't do anything with it!
            ItemSimpleType simple = new ItemSimpleType("var1", "int[]");
            FileInfo proxyFile = new FileInfo("TestSimpleRename.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }
            ROOTClassShell mainClass = new ROOTClassShell("TestSimpleRename") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(simple);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo() { Groups = new ArrayGroup[] { new ArrayGroup() { Name = "ungrouped", Variables = new VariableInfo[] { new VariableInfo() { NETName = "myvar", TTreeName = "var1" } } } } };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestSimpleRename.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestSimpleRename", userinfo } });

            DumpOutputFile(outputFile);

            /// Look through this to see if we can make sure there are no renames!
            Assert.IsTrue(FindInFile(outputFile, "RenameVariable(\"var1\")"), "Rename missing!");
            Assert.IsTrue(FindInFile(outputFile, "int[] myvar"), "myvar missing");
            Assert.IsTrue(FindInFile(outputFile, "int[] var1"), "val1 missing");
            Assert.IsFalse(FindInFile(outputFile, "ungrouped"), "group found");
        }

        [TestMethod]
        public void TestColonsInVarNameWRename()
        {
            ItemSimpleType simple = new ItemSimpleType("dude::fork", "int[]");
            FileInfo proxyFile = new FileInfo("TestColonsInVarNameWRename.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }

            ROOTClassShell mainClass = new ROOTClassShell("TestSimpleGroupAndRename") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(simple);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo() { Groups = new ArrayGroup[] { new ArrayGroup() { Name = "jets", Variables = new VariableInfo[] { new VariableInfo() { NETName = "myvar", TTreeName = "dude::fork" } } } } };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestSimpleGroupAndRename.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestSimpleGroupAndRename", userinfo } });

            DumpOutputFile(outputFile);

            /// Look through this to see if we can make sure there are no renames!
            Assert.IsFalse(FindInFile(outputFile, "dude::fork"), "Saw the double colon!!");
            Assert.IsTrue(FindInFile(outputFile, "dude__fork"), "Missing the variable!!");
        }

        [TestMethod]
        public void TestColonsInVarName()
        {
            ItemSimpleType simple = new ItemSimpleType("dude::fork", "int[]");
            FileInfo proxyFile = new FileInfo("TestColonsInVarName.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }

            ROOTClassShell mainClass = new ROOTClassShell("TestSimpleGroupAndRename") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(simple);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo() { Groups = new ArrayGroup[] { new ArrayGroup() { Name = "jets", Variables = new VariableInfo[] { new VariableInfo() { NETName = "dude::fork", TTreeName = "dude::fork" } } } } };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestSimpleGroupAndRename.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestSimpleGroupAndRename", userinfo } });

            DumpOutputFile(outputFile);

            /// Look through this to see if we can make sure there are no renames!
            Assert.IsFalse(FindInFile(outputFile, "dude::fork"), "Saw the double colon!!");
            Assert.IsTrue(FindInFile(outputFile, "dude__fork"), "Missing the variable!!");
        }

        [TestMethod]
        public void TestCharactersInClassName()
        {
            ItemSimpleType simple = new ItemSimpleType("fork", "int");
            FileInfo proxyFile = new FileInfo("TestColonsInVarName.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }

            ROOTClassShell mainClass = new ROOTClassShell("##Shapes") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(simple);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo() { Groups = new ArrayGroup[] { new ArrayGroup() { Name = "jets", Variables = new VariableInfo[] { new VariableInfo() { NETName = "fork", TTreeName = "fork" } } } } };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestCharactersInClassName.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestSimpleGroupAndRename", userinfo } });

            DumpOutputFile(outputFile);

            Assert.AreEqual(3, CountInFile(outputFile, "##Shapes"), "Missing reference ot the shapes object");
        }

        [TestMethod]
        public void TestSimpleGroupAndRename()
        {
            /// Create simple user info - but don't do anything with it!
            ItemSimpleType simple = new ItemSimpleType("var1", "int[]");
            FileInfo proxyFile = new FileInfo("TestSimpleGroupAndRename.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }
            ROOTClassShell mainClass = new ROOTClassShell("TestSimpleGroupAndRename") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(simple);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo() { Groups = new ArrayGroup[] { new ArrayGroup() { Name = "jets", Variables = new VariableInfo[] { new VariableInfo() { NETName = "myvar", TTreeName = "var1" } } } } };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestSimpleGroupAndRename.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestSimpleGroupAndRename", userinfo } });

            DumpOutputFile(outputFile);

            /// Look through this to see if we can make sure there are no renames!
            Assert.IsTrue(FindInFile(outputFile, "RenameVariable(\"var1\")"), "Rename missing!");
            Assert.IsTrue(FindInFile(outputFile, "TTreeVariableGrouping"), "Missing TTreeVariableGrouping");
            Assert.IsTrue(FindInFile(outputFile, "jets"), "missing a reference to jets");
            Assert.IsTrue(FindInFile(outputFile, "int myvar"), "myvar missing");
            Assert.IsTrue(FindInFile(outputFile, "int[] var1"), "val1 missing");
            Assert.IsFalse(FindInFile(outputFile, "ungrouped"), "group found");
            Assert.IsTrue(FindInFile(outputFile, "TestSimpleGroupAndRenamejets"), "Missing the group definition");
        }

        [TestMethod]
        public void TestSimpleGroupWithCustomClassName()
        {
            /// Create simple user info - but don't do anything with it!
            ItemSimpleType simple = new ItemSimpleType("var1", "int[]");
            FileInfo proxyFile = new FileInfo("TestSimpleGroupAndRename.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }
            ROOTClassShell mainClass = new ROOTClassShell("TestSimpleGroupAndRename") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(simple);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo() { Groups = new ArrayGroup[] { new ArrayGroup() { Name = "jets", ClassName = "Jet", Variables = new VariableInfo[] { new VariableInfo() { NETName = "myvar", TTreeName = "var1" } } } } };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestSimpleGroupAndRename.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestSimpleGroupAndRename", userinfo } });

            DumpOutputFile(outputFile);

            /// Look through this to see if we can make sure there are no renames!
            Assert.IsTrue(FindInFile(outputFile, "RenameVariable(\"var1\")"), "Rename missing!");
            Assert.IsTrue(FindInFile(outputFile, "TTreeVariableGrouping"), "Missing TTreeVariableGrouping");
            Assert.IsTrue(FindInFile(outputFile, "jets"), "missing a reference to jets");
            Assert.IsTrue(FindInFile(outputFile, "int myvar"), "myvar missing");
            Assert.IsTrue(FindInFile(outputFile, "int[] var1"), "val1 missing");
            Assert.IsFalse(FindInFile(outputFile, "ungrouped"), "group found");
            Assert.IsFalse(FindInFile(outputFile, "TestSimpleGroupAndRenamejets"), "Found the non-class name default class name");
            Assert.IsTrue(FindInFile(outputFile, "Jet"), "Did not find the Jet custom class name");
        }

        [TestMethod]
        public void TestSimpleIndexing()
        {
            /// Create simple user info - but don't do anything with it!
            ItemSimpleType simpleIndex = new ItemSimpleType("index", "int[]");
            ItemSimpleType simpleVal = new ItemSimpleType("var1", "float[]");
            FileInfo proxyFile = new FileInfo("TestSimpleIndexing.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }
            ROOTClassShell mainClass = new ROOTClassShell("TestSimpleIndexing") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(simpleIndex);
            mainClass.Add(simpleVal);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo()
            {
                Groups = new ArrayGroup[] {
                    new ArrayGroup()
                    {
                        Name = "jets", Variables = new VariableInfo[]
                        {
                            new VariableInfo() { NETName = "index", TTreeName = "index", IndexToGroup="muons" }
                        }
                    },
                    new ArrayGroup()
                    {
                        Name = "muons", Variables = new VariableInfo[]
                        {
                            new VariableInfo() { NETName = "var1", TTreeName = "var1"}
                        }
                    }
                }
            };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestSimpleIndexing.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestSimpleIndexing", userinfo } });

            DumpOutputFile(outputFile);

            /// Look through this to see if we can make sure there are no renames!
            Assert.IsTrue(FindInFile(outputFile, "TTreeVariableGrouping"), "Missing TTreeVariableGrouping");
            Assert.IsTrue(FindInFile(outputFile, "jets"), "missing a reference to jets");
            Assert.IsTrue(FindInFile(outputFile, "muons"), "missing a reference to jets");
            Assert.IsTrue(FindInFile(outputFile, "IndexToOtherObjectArray(typeof("), "Missing IndexToOtherObject");
            Assert.IsTrue(FindInFile(outputFile, "\"muons\")"), "Missing muons reference");
            Assert.IsTrue(FindInFile(outputFile, "float var1"), "var1 missing");
            Assert.IsFalse(FindInFile(outputFile, "ungrouped"), "group found");
            Assert.IsTrue(FindInFile(outputFile, "TestSimpleIndexingmuons index"), "index should be of the muon type");
        }

        [TestMethod]
        public void TestRenameIndexArray()
        {
            /// Create simple user info - but don't do anything with it!
            ItemSimpleType simpleIndex = new ItemSimpleType("index", "int[][]");
            ItemSimpleType simpleVal = new ItemSimpleType("var1", "float[]");
            FileInfo proxyFile = new FileInfo("TestRenameIndexArray.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }
            ROOTClassShell mainClass = new ROOTClassShell("TestRenameIndexArray") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(simpleIndex);
            mainClass.Add(simpleVal);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo()
            {
                Groups = new ArrayGroup[] {
                    new ArrayGroup()
                    {
                        Name = "jets", Variables = new VariableInfo[]
                        {
                            new VariableInfo() { NETName = "muons", TTreeName = "index", IndexToGroup="muons" }
                        }
                    },
                    new ArrayGroup()
                    {
                        Name = "muons", Variables = new VariableInfo[]
                        {
                            new VariableInfo() { NETName = "var1", TTreeName = "var1"}
                        }
                    }
                }
            };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestRenameIndexArray.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestRenameIndexArray", userinfo } });

            DumpOutputFile(outputFile);

            /// Look through this to see if we can make sure there are no renames!
            Assert.IsTrue(FindInFile(outputFile, "TTreeVariableGrouping"), "Missing TTreeVariableGrouping");
            Assert.IsTrue(FindInFile(outputFile, "jets"), "missing a reference to jets");
            Assert.IsTrue(FindInFile(outputFile, "muons"), "missing a reference to jets");
            Assert.IsTrue(FindInFile(outputFile, "IndexToOtherObjectArray(typeof("), "Missing IndexToOtherObject");
            Assert.IsTrue(FindInFile(outputFile, "float var1"), "var1 missing");
            Assert.IsFalse(FindInFile(outputFile, "ungrouped"), "group found");
            Assert.IsTrue(FindInFile(outputFile, "TestRenameIndexArraymuons[] muons"), "Muon reference is imporper");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestNonIntIndex()
        {
            /// Create simple user info - but don't do anything with it!
            ItemSimpleType simpleIndex = new ItemSimpleType("index", "float[]");
            ItemSimpleType simpleVal = new ItemSimpleType("var1", "float[]");
            FileInfo proxyFile = new FileInfo("TestNonIntIndex.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }
            ROOTClassShell mainClass = new ROOTClassShell("TestNonIntIndex") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(simpleIndex);
            mainClass.Add(simpleVal);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo()
            {
                Groups = new ArrayGroup[] {
                    new ArrayGroup()
                    {
                        Name = "jets", Variables = new VariableInfo[]
                        {
                            new VariableInfo() { NETName = "index", TTreeName = "index", IndexToGroup="muons" }
                        }
                    },
                    new ArrayGroup()
                    {
                        Name = "muons", Variables = new VariableInfo[]
                        {
                            new VariableInfo() { NETName = "var1", TTreeName = "var1"}
                        }
                    }
                }
            };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestNonIntIndex.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestNonIntIndex", userinfo } });
        }

        [TestMethod]
        public void TestRenamedIndex()
        {
            /// Create simple user info - but don't do anything with it!
            ItemSimpleType simpleIndex = new ItemSimpleType("index", "int[]");
            ItemSimpleType simpleVal = new ItemSimpleType("var1", "float[]");
            FileInfo proxyFile = new FileInfo("TestRenamedIndex.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }
            ROOTClassShell mainClass = new ROOTClassShell("TestRenamedIndex") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(simpleIndex);
            mainClass.Add(simpleVal);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo()
            {
                Groups = new ArrayGroup[] {
                    new ArrayGroup()
                    {
                        Name = "jets", Variables = new VariableInfo[]
                        {
                            new VariableInfo() { NETName = "muons", TTreeName = "index", IndexToGroup="muons" }
                        }
                    },
                    new ArrayGroup()
                    {
                        Name = "muons", Variables = new VariableInfo[]
                        {
                            new VariableInfo() { NETName = "var1", TTreeName = "var1"}
                        }
                    }
                }
            };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestRenamedIndex.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestRenamedIndex", userinfo } });

            DumpOutputFile(outputFile);

            /// Look through this to see if we can make sure there are no renames!
            Assert.IsTrue(FindInFile(outputFile, "TTreeVariableGrouping"), "Missing TTreeVariableGrouping");
            Assert.IsTrue(FindInFile(outputFile, "jets"), "missing a reference to jets");
            Assert.IsTrue(FindInFile(outputFile, "muons"), "missing a reference to jets");
            Assert.IsTrue(FindInFile(outputFile, "IndexToOtherObjectArray(typeof("), "Missing IndexToOtherObject");
            Assert.IsTrue(FindInFile(outputFile, "TestRenamedIndexmuons muons"), "Muon reference is imporper");
            Assert.IsTrue(FindInFile(outputFile, "float var1"), "var1 missing");
            Assert.IsFalse(FindInFile(outputFile, "ungrouped"), "group found");
        }

        [TestMethod]
        public void TestCStyleArray()
        {
            // Simple set of types for an index array
            var vArray = new ItemCStyleArray("int[]", new ItemSimpleType("arr", "int"));
            vArray.Add(0, "n", false);
            var vIndex = new ItemSimpleType("n", "int");
            FileInfo proxyFile = new FileInfo("TestCStyleArray.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }
            ROOTClassShell mainClass = new ROOTClassShell("TestSimpleRename") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(vIndex);
            mainClass.Add(vArray);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo()
            {
                Groups = new ArrayGroup[] { new ArrayGroup() {
                Name = "ungrouped",
                Variables = new VariableInfo[] {
                    new VariableInfo() { NETName = "n", TTreeName = "n" },
                    new VariableInfo() { NETName = "arr", TTreeName = "arr"}
                } } }
            };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestCStyleArray.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestSimpleRename", userinfo } });

            CopyToOutput(outputFile);

            /// Look through this to see if we can make sure there are no renames!
            Assert.IsTrue(FindInFile(outputFile, "int[] arr"), "Array Decl missing");
            Assert.IsTrue(FindInFile(outputFile, "int n"), "Index decl missing");
            Assert.IsTrue(FindInFile(outputFile, "[ArraySizeIndex(\"n\", Index = 0)]"), "Missing array size index attribute");
        }

        /// <summary>
        /// Copy a file to the console. This is to just make dealing with tests simpler.
        /// </summary>
        /// <param name="outputFile"></param>
        private void CopyToOutput(FileInfo outputFile)
        {
            Console.WriteLine("Output of {0} ({1})", outputFile.Name, outputFile.FullName);
            using (var f = outputFile.OpenText())
            {
                while (!f.EndOfStream)
                {
                    var line = f.ReadLine();
                    Console.WriteLine(line);
                }
            }
        }

        [TestMethod]
        public void TestConstCStyleArray()
        {
            // Simple set of types for an index array
            var vArray = new ItemCStyleArray("int[]", new ItemSimpleType("arr", "int"));
            vArray.Add(0, "10", true);
            var vIndex = new ItemSimpleType("n", "int");
            FileInfo proxyFile = new FileInfo("TestConstCStyleArray.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }

            ROOTClassShell mainClass = new ROOTClassShell("TestSimpleRename") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(vIndex);
            mainClass.Add(vArray);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo()
            {
                Groups = new ArrayGroup[] { new ArrayGroup() {
                Name = "ungrouped",
                Variables = new VariableInfo[] {
                    new VariableInfo() { NETName = "n", TTreeName = "n" },
                    new VariableInfo() { NETName = "arr", TTreeName = "arr"}
                } } }
            };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestConstCStyleArray.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestSimpleRename", userinfo } });

            CopyToOutput(outputFile);

            /// Look through this to see if we can make sure there are no renames!
            Assert.IsTrue(FindInFile(outputFile, "int[] arr"), "Array Decl missing");
            Assert.IsTrue(FindInFile(outputFile, "int n"), "Index decl missing");
            Assert.IsTrue(FindInFile(outputFile, "[ArraySizeIndex(\"10\", IsConstantExpression = true, Index = 0)]"), "Missing array size index attribute");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCSyleArrayNonIntIndex()
        {
            // Simple set of types for an index array
            var vArray = new ItemCStyleArray("int[]", new ItemSimpleType("arr", "int"));
            vArray.Add(0, "n", false);
            var vIndex = new ItemSimpleType("n", "float");
            FileInfo proxyFile = new FileInfo("TestCStyleArray.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }
            ROOTClassShell mainClass = new ROOTClassShell("TestSimpleRename") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(vIndex);
            mainClass.Add(vArray);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo()
            {
                Groups = new ArrayGroup[] { new ArrayGroup() {
                Name = "ungrouped",
                Variables = new VariableInfo[] {
                    new VariableInfo() { NETName = "n", TTreeName = "n" },
                    new VariableInfo() { NETName = "arr", TTreeName = "arr"}
                } } }
            };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestCStyleArray.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestSimpleRename", userinfo } });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCStyleArrayBadIndexName()
        {
            // Simple set of types for an index array
            var vArray = new ItemCStyleArray("int[]", new ItemSimpleType("arr", "int"));
            vArray.Add(0, "i", false);
            var vIndex = new ItemSimpleType("n", "int");
            FileInfo proxyFile = new FileInfo("TestCStyleArray.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }
            ROOTClassShell mainClass = new ROOTClassShell("TestSimpleRename") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(vIndex);
            mainClass.Add(vArray);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo()
            {
                Groups = new ArrayGroup[] { new ArrayGroup() {
                Name = "ungrouped",
                Variables = new VariableInfo[] {
                    new VariableInfo() { NETName = "n", TTreeName = "n" },
                    new VariableInfo() { NETName = "arr", TTreeName = "arr"}
                } } }
            };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestCStyleArray.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestSimpleRename", userinfo } });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void TestDuplicateClassNames()
        {
            var vIndex = new ItemSimpleType("n", "int");
            FileInfo proxyFile = new FileInfo("TestCStyleArray.cpp");
            using (var writer = proxyFile.CreateText())
            {
                writer.WriteLine();
                writer.Close();
            }
            ROOTClassShell mainClass = new ROOTClassShell("TestSimpleRename") { NtupleProxyPath = proxyFile.FullName };
            mainClass.Add(vIndex);

            ROOTClassShell mainClass1 = new ROOTClassShell("TestSimpleRename") { NtupleProxyPath = proxyFile.FullName };
            mainClass1.Add(vIndex);
            var ntup = new NtupleTreeInfo() { Classes = new ROOTClassShell[] { mainClass, mainClass1 }, ClassImplimintationFiles = new string[0] };

            var userinfo = new TTreeUserInfo()
            {
                Groups = new ArrayGroup[] { new ArrayGroup() {
                Name = "ungrouped",
                Variables = new VariableInfo[] {
                    new VariableInfo() { NETName = "n", TTreeName = "n" },
                } } }
            };

            var cg = new ClassGenerator();
            var outputFile = new FileInfo("TestDuplicateClassNames.cs");
            cg.GenerateClasss(ntup, outputFile, "junk", new Dictionary<string, TTreeUserInfo>() { { "TestDuplicateClassNames", userinfo } });
        }

        private void DumpOutputFile(FileInfo outputFile)
        {
            foreach (var l in LinesInFile(outputFile))
            {
                Console.WriteLine(l);
            }
        }

        [TestMethod]
        public void TestOutputTestFiles()
        {
            GenerateClassFromClasses(new ClassGenerator(),
                1, // Output choice
                2, // Number of extra files
                2, // number of extra files to create
                10, // Null file index
                3, // Proxy path choice
                "junk", // Namespace
                2 // # of output objects to create
                );
        }

        /// <summary>
        /// Search through the file for some text line.
        /// </summary>
        /// <param name="outputCSFile"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool FindInFile(FileInfo outputCSFile, string p)
        {
            return (from l in LinesInFile(outputCSFile)
                    where l.Contains(p)
                    select l).Any();
        }

        /// <summary>
        /// Count number of times some string appears in the text file.
        /// </summary>
        /// <param name="outputCSFile"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private int CountInFile(FileInfo outputCSFile, string p)
        {
            return (from l in LinesInFile(outputCSFile)
                    where l.Contains(p)
                    select l).Count();
        }

        private IEnumerable<string> LinesInFile(FileInfo f)
        {
            using (var reader = f.OpenText())
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null)
                        yield return line;
                }
                reader.Close();
            }
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullInputFile()
        {
            ClassGenerator target = new ClassGenerator();
            target.GenerateClasss((FileInfo)null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNullOutputFile()
        {
            FileInfo f = new FileInfo("junk.xml");
            ClassGenerator target = new ClassGenerator();
            target.GenerateClasss(f, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNullNamespace()
        {
            FileInfo f = new FileInfo("junk.xml");
            ClassGenerator target = new ClassGenerator();
            target.GenerateClasss(f, f, null);
        }

        [TestMethod]
        [DeploymentItem("CollectionTreeConfig-000.ntup")]
        [DeploymentItem("EVNT-short.ntupom")]
        public void TestParseXMLOfTTreeWithInterior()
        {
            var basexml = new FileInfo("EVNT-short.ntupom");
            Assert.IsTrue(basexml.Exists, "can't find input xml file");

            using (var h = new FileInfo("ntuple_CollectionTree.h").CreateText())
            {
                h.WriteLine("hi");
                h.Close();
            }

            var t = new ClassGenerator();
            var output = new FileInfo("TestXMLParsing.cs");
            t.GenerateClasss(basexml, output, "Bogus");
            Assert.IsTrue(output.Exists, "Output file existance");
            Assert.AreEqual(3, CountInFile(output, "TClonesArrayImpliedClass"), "# of TClonesArrayImpliedClass attributes");
            Assert.AreEqual(1, CountInFile(output, "class Queryable"), "# of Queryable classes");
            Assert.AreEqual(1, CountInFile(output, ": IExpressionHolder"), "# of Expression holder classeS");
            Assert.AreEqual(0, CountInFile(output, "ArraySizeIndex"), "# of times the ArraySizeIndex method appears"); // implied for tclones array guys...
            Assert.AreEqual(40, CountInFile(output, "[NotAPointer]"), "# of NotAPointer attributes");
        }
    }

}
