using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTreeDataModel;
using TTreeParserCPPTests;
using System.Diagnostics;

namespace TTreeParser.Tests
{
    /// <summary>
    ///This is a test class for ArrayAnalyzerTest and is intended
    ///to contain all ArrayAnalyzerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ArrayAnalyzerTest
    {
        [TestInitialize]
        public void Setup()
        {
            var a = ROOTNET.NTROOT.gROOT.GetApplication();
        }

        /// <summary>
        ///A test for DetermineAllArrayLengths
        ///</summary>
        [TestMethod]
        public void TestArrayWithNoEntries()
        {
            ArrayAnalyzer target = new ArrayAnalyzer(); // TODO: Initialize to an appropriate value

            var tree = CreateTrees.CreateVectorTree();

            ROOTClassShell sh = new ROOTClassShell();

            target.DetermineAllArrayLengths(sh, tree, 10);
        }

        [TestMethod]
        public void TestWithNoArrays()
        {
            var aa = new ArrayAnalyzer();
            var tree = CreateTrees.CreateWithIntOnly(10);
            ROOTClassShell sh = new ROOTClassShell();
            var results = aa.DetermineAllArrayLengths(sh, tree, 10);
            Assert.AreEqual(1, results.Length, "# of events incorrect");
            Assert.AreEqual(0, results[0].Length, "found an array!?");
        }

        class classitem : IClassItem
        {
            public override string ItemType { get; set; }
            public override string Name { get; set; }
            public override bool NotAPointer { get; set; }
        }

        [TestMethod]
        public void TestWithOneArray()
        {
            // We get memory corruption if we create the tree and use it, rather
            // than writing it out to a file first. Not sure why... GC and other
            // things looked at, but they don't seem to be variables.
            {
                var f = ROOTNET.NTFile.Open("TestWithOneArray.root", "RECREATE");
                var tree = CreateTrees.CreateTreeWithSimpleSingleVector(50);
                f.Write();
                f.Close();
            }

            var f1 = ROOTNET.NTFile.Open("TestWithOneArray.root", "READ");
            var tree1 = f1.Get("dude") as ROOTNET.NTTree;

            var aa = new ArrayAnalyzer();
            ROOTClassShell sh = new ROOTClassShell();
            sh.Add(new classitem() { ItemType = "int[]", Name = "myvectorofint" });
            var result = aa.DetermineAllArrayLengths(sh, tree1, 10);
            Assert.AreEqual(10, result.Length, "# of events");
            Assert.IsTrue(result.All(x => x.Length == 1), "incorrect individual variable list length list");
            Assert.IsTrue(result.All(x => x[0].Item2 == 10), "incorrect individual variable list length list");
            Assert.IsTrue(result.All(x => x[0].Item1 == "myvectorofint"), "incorrect individual variable list length list");
            f1.Close();
        }

        [TestMethod]
        public void TestArrayAndNormalItem()
        {
            var aa = new ArrayAnalyzer();
            // We get memory corruption if we create the tree and use it, rather
            // than writing it out to a file first. Not sure why... GC and other
            // things looked at, but they don't seem to be variables.
            {
                var f = ROOTNET.NTFile.Open("TestArrayAndNormalItem.root", "RECREATE");
                var tree = CreateTrees.CreateTreeWithSimpleSingleVectorAndItem(20);
                f.Write();
                f.Close();
            }

            var f1 = ROOTNET.NTFile.Open("TestArrayAndNormalItem.root", "READ");
            var tree1 = f1.Get("dude") as ROOTNET.NTTree;

            ROOTClassShell sh = new ROOTClassShell();
            sh.Add(new classitem() { ItemType = "int[]", Name = "myvectorofint" });
            var result = aa.DetermineAllArrayLengths(sh, tree1, 10);
            Assert.AreEqual(10, result.Length, "# of events");
            Assert.IsTrue(result.All(x => x.Length == 1), "incorrect individual variable list length list");
            Assert.IsTrue(result.All(x => x[0].Item2 == 10), "incorrect individual variable list length list");
            f1.Close();
        }

        [TestMethod]
        public void TestWithOneIndexedArray()
        {
            var aa = new ArrayAnalyzer();

            string filename = "TestWithOneIndexedArray.root";
            var f = new ROOTNET.NTFile(filename, "RECREATE");
            var tree = CreateTrees.CreateTreeWithIndexedSimpleVector(20);
            f.Write();

            Trace.WriteLine("The size is: " + tree.Entries.ToString());

            ROOTClassShell sh = new ROOTClassShell();
            sh.Add(new classitem() { ItemType = "int[]", Name = "arr" });
            var result = aa.DetermineAllArrayLengths(sh, tree, 10);

            Console.WriteLine("result.length = {0}", result.Length);
            Assert.AreEqual(10, result.Length, "# of events");
            Assert.IsTrue(result.All(x => x.Length == 1), "# of arrays");
            Assert.IsTrue(result.All(x => x[0].Item2 == 10), "Length of array");
            Assert.IsTrue(result.All(x => x[0].Item1 == "arr"), "variable name");

            f.Close();
        }

        [TestMethod]
        public void TestWithOneConstIndexedArray()
        {
            var aa = new ArrayAnalyzer();

            string filename = "TestWithOneConstIndexedArray.root";
            var f = new ROOTNET.NTFile(filename, "RECREATE");
            var tree = CreateTrees.CreateTreeWithConstIndexedSimpleVector(20);
            f.Write();

            ROOTClassShell sh = new ROOTClassShell();
            sh.Add(new classitem() { ItemType = "int[]", Name = "arr" });
            var result = aa.DetermineAllArrayLengths(sh, tree, 10);

            Assert.AreEqual(10, result.Length, "# of events");
            Assert.IsTrue(result.All(x => x.Length == 1), "# of arrays");
            Assert.IsTrue(result.All(x => x[0].Item2 == 20), "Length of array");
            Assert.IsTrue(result.All(x => x[0].Item1 == "arr"), "variable name");

            f.Close();
        }

        [TestMethod]
        public void TestWithTwoArrays()
        {
            // We get memory corruption if we create the tree and use it, rather
            // than writing it out to a file first. Not sure why... GC and other
            // things looked at, but they don't seem to be variables.
            {
                var f = ROOTNET.NTFile.Open("TestWithTwoArrays.root", "RECREATE");
                var tree = CreateTrees.CreateTreeWithSimpleDoubleVector(20);
                f.Write();
                f.Close();
            }

            var f1 = ROOTNET.NTFile.Open("TestWithTwoArrays.root", "READ");
            var tree1 = f1.Get("dude") as ROOTNET.NTTree;

            var aa = new ArrayAnalyzer();
            ROOTClassShell sh = new ROOTClassShell();
            sh.Add(new classitem() { ItemType = "int[]", Name = "myvectorofint" });
            sh.Add(new classitem() { ItemType = "int[]", Name = "myvectorofint1" });
            var result = aa.DetermineAllArrayLengths(sh, tree1, 10);
            Assert.AreEqual(10, result.Length, "# of events");
            Assert.IsTrue(result.All(x => x.Length == 2), "incorrect individual variable list length list");
            foreach (var evt in result)
            {
                foreach (var item in evt)
                {
                    Assert.IsTrue(item.Item1 == "myvectorofint" || item.Item1 == "myvectorofint1", "item name");
                    Assert.IsTrue(item.Item2 == 10 || item.Item2 == 20, "# of items");
                }
            }

            f1.Close();
        }

        [TestMethod]
        public void TestNullGrouping()
        {
            /// No elements at all - like an empty ntuple with no arrays
            var aa = new ArrayAnalyzer();
            var data = new Tuple<string, int>[][]
            {
                new Tuple<string, int>[0]
            };
            var result = aa.DetermineGroups(data);
            Assert.AreEqual(0, result.Count, "# of groups");
        }

        [TestMethod]
        public void TestGroupingOneArray()
        {
            /// A single array. Make sure it goes into ungrouped.

            var aa = new ArrayAnalyzer();
            var data = new Tuple<string, int>[][]
            {
                new Tuple<string, int>[] { Tuple.Create("var1", 5)},
                new Tuple<string, int>[] { Tuple.Create("var1", 10)},
                new Tuple<string, int>[] { Tuple.Create("var1", 4)}
            };
            var result = aa.DetermineGroups(data);
            Assert.AreEqual(1, result.Count, "# of groups");
            Assert.AreEqual("ungrouped", result[0].Name, "ungrouped name");
            Assert.AreEqual(1, result[0].Variables.Length, "var count");
            Assert.AreEqual("var1", result[0].Variables[0].NETName, "var name");
        }

        [TestMethod]
        public void TestGroupingTwoArrays()
        {
            /// Two arrays, that are in lock step
            /// A single array. Make sure it goes into ungrouped.

            var aa = new ArrayAnalyzer();
            var data = new Tuple<string, int>[][]
            {
                new Tuple<string, int>[] { Tuple.Create("var1", 5), Tuple.Create("var2", 5)},
                new Tuple<string, int>[] { Tuple.Create("var1", 10), Tuple.Create("var2", 10)},
                new Tuple<string, int>[] { Tuple.Create("var1", 10), Tuple.Create("var2", 10)}
            };
            var result = aa.DetermineGroups(data);
            Assert.AreEqual(1, result.Count, "# of groups");
            Assert.AreEqual(2, result[0].Variables.Length, "# of vars in group");
            Assert.IsTrue((result[0].Variables[0].NETName == "var1" && result[0].Variables[1].NETName == "var2")
        || (result[0].Variables[0].NETName == "var2" && result[0].Variables[1].NETName == "var1"), "varable names");
        }

        [TestMethod]
        public void TestGroupingTwoArraysNoGroups()
        {
            /// Two arrays, that don't match at all (ungrouped)

            var aa = new ArrayAnalyzer();
            var data = new Tuple<string, int>[][]
            {
                new Tuple<string, int>[] { Tuple.Create("var1", 5), Tuple.Create("var2", 3)},
                new Tuple<string, int>[] { Tuple.Create("var1", 10), Tuple.Create("var2", 9)},
                new Tuple<string, int>[] { Tuple.Create("var1", 10), Tuple.Create("var2", 4)}
            };

            var result = aa.DetermineGroups(data);
            Assert.AreEqual(1, result.Count, "# of groups");
            Assert.AreEqual("ungrouped", result[0].Name, "ungroued name incorrect");
            Assert.AreEqual(2, result[0].Variables.Length, "# of variables in ungrouped");
        }

        [TestMethod]
        public void TestGroupingTwoArraysEventualMismatch()
        {
            /// Start out in lock step, but then fall apart later in the sequence.
            var aa = new ArrayAnalyzer();
            var data = new Tuple<string, int>[][]
            {
                new Tuple<string, int>[] { Tuple.Create("var1", 5), Tuple.Create("var2", 5)},
                new Tuple<string, int>[] { Tuple.Create("var1", 10), Tuple.Create("var2", 10)},
                new Tuple<string, int>[] { Tuple.Create("var1", 10), Tuple.Create("var2", 4)}
            };

            var result = aa.DetermineGroups(data);
            Assert.AreEqual(1, result.Count, "# of groups");
            Assert.AreEqual("ungrouped", result[0].Name, "ungroued name incorrect");
            Assert.AreEqual(2, result[0].Variables.Length, "# of variables in ungrouped");
        }

        [TestMethod]
        public void TestGroupingFourArraysTwoGroups()
        {
            var aa = new ArrayAnalyzer();
            var data = new Tuple<string, int>[][]
            {
                new Tuple<string, int>[] { Tuple.Create("var1", 5), Tuple.Create("var2", 5), Tuple.Create("var3", 2), Tuple.Create("var4", 2)},
                new Tuple<string, int>[] { Tuple.Create("var1", 5), Tuple.Create("var2", 5), Tuple.Create("var3", 8), Tuple.Create("var4", 8)},
                new Tuple<string, int>[] { Tuple.Create("var1", 5), Tuple.Create("var2", 5), Tuple.Create("var3", 7), Tuple.Create("var4", 7)}
            };

            var result = aa.DetermineGroups(data);
            Assert.AreEqual(2, result.Count, "# of groups");
            Assert.AreEqual(2, result[0].Variables.Length, "# of variables in group 1");
            Assert.AreEqual(2, result[1].Variables.Length, "# of variables in group 2");
        }

        [TestMethod]
        public void TestGroupingSomeZeros()
        {
            /// 2 arrays in one group, and two with zeros all the time.
            var aa = new ArrayAnalyzer();
            var data = new Tuple<string, int>[][]
            {
                new Tuple<string, int>[] { Tuple.Create("var1", 5), Tuple.Create("var2", 5), Tuple.Create("var3", 2), Tuple.Create("var4", 0)},
                new Tuple<string, int>[] { Tuple.Create("var1", 5), Tuple.Create("var2", 5), Tuple.Create("var3", 0), Tuple.Create("var4", 0)},
                new Tuple<string, int>[] { Tuple.Create("var1", 5), Tuple.Create("var2", 5), Tuple.Create("var3", 0), Tuple.Create("var4", 0)}
            };

            var result = aa.DetermineGroups(data);
            Assert.AreEqual(2, result.Count, "# of groups");
            Assert.AreEqual(2, result[0].Variables.Length, "# of variables in group 1");
            Assert.AreEqual(2, result[1].Variables.Length, "# of variables in group 2");
            var ungroup = (from g in result where g.Name == "ungrouped" select g).First();
            if (ungroup.Variables[0].NETName == "var3")
                Assert.AreEqual("var4", ungroup.Variables[1].NETName);
            else
                Assert.AreEqual("var3", ungroup.Variables[1]);
        }

        [TestMethod]
        public void TestGroupingInitalZeros()
        {
            /// 2 arrays, both have zeros, but then have something good, should end up
            /// in same group.

            var aa = new ArrayAnalyzer();
            var data = new Tuple<string, int>[][]
            {
                new Tuple<string, int>[] { Tuple.Create("var1", 0), Tuple.Create("var2", 0)},
                new Tuple<string, int>[] { Tuple.Create("var1", 10), Tuple.Create("var2", 10)},
                new Tuple<string, int>[] { Tuple.Create("var1", 10), Tuple.Create("var2", 10)}
            };
            var result = aa.DetermineGroups(data);
            Assert.AreEqual(1, result.Count, "# of groups");
            Assert.AreEqual(2, result[0].Variables.Length, "# of vars in group");
            Assert.AreNotEqual("ungrouped", result[0].Name, "group name");
        }

        [TestMethod]
        public void TestSingleGrouping()
        {
            /// Test the DetermineGroups that takes the simple argument

            var rawData = new Tuple<string, int>[]
            {
            Tuple.Create("var1", 0),
            Tuple.Create("var2", 10),
            Tuple.Create("var3", 10),
            Tuple.Create("var4", 0)
            };
            var aa = new ArrayAnalyzer();
            var gp = aa.DetermineGroups(rawData).ToArray();
            Assert.AreEqual(2, gp.Length, "# of groups");
            var zero = (from g in gp where g.Item1 == 0 select g).First();
            var ten = (from g in gp where g.Item1 == 10 select g).First();

            Assert.AreEqual(2, zero.Item2.Length, "zero items");
            Assert.AreEqual(2, ten.Item2.Length, "ten items");

            Assert.IsTrue(zero.Item2[0] == "var1" || zero.Item2[0] == "var4", "zero item 0 name");
            Assert.IsTrue(zero.Item2[1] == "var1" || zero.Item2[1] == "var4", "zero item 1 name");
            Assert.IsTrue(ten.Item2[0] == "var2" || ten.Item2[0] == "var3", "ten item 0 name");
            Assert.IsTrue(ten.Item2[1] == "var2" || ten.Item2[1] == "var3", "ten item 1 name");
        }
    }
}
