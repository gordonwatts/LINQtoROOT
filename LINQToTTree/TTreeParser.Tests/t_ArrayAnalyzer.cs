using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTreeDataModel;
using TTreeParserCPPTests;

namespace TTreeParser.Tests
{


    /// <summary>
    ///This is a test class for ArrayAnalyzerTest and is intended
    ///to contain all ArrayAnalyzerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ArrayAnalyzerTest
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

        [TestInitialize]
        public void Setup()
        {
            var a = ROOTNET.NTROOT.gROOT.GetApplication();
        }

        /// <summary>
        ///A test for DetermineAllArrayLengths
        ///</summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
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
        }

        [TestMethod]
        public void TestWithOneArray()
        {
            var aa = new ArrayAnalyzer();
            var tree = CreateTrees.CreateTreeWithSimpleSingleVector(20);

            ROOTClassShell sh = new ROOTClassShell();
            sh.Add(new classitem() { ItemType = "int[]", Name = "myvectorofint" });
            var result = aa.DetermineAllArrayLengths(sh, tree, 10);
            Assert.AreEqual(10, result.Length, "# of events");
            Assert.IsTrue(result.All(x => x.Length == 1), "incorrect individual variable list length list");
            Assert.IsTrue(result.All(x => x[0].Item2 == 10), "incorrect individual variable list length list");
        }

        [TestMethod]
        public void TestArrayAndNormalItem()
        {
            var aa = new ArrayAnalyzer();
            var tree = CreateTrees.CreateTreeWithSimpleSingleVectorAndItem(20);

            ROOTClassShell sh = new ROOTClassShell();
            sh.Add(new classitem() { ItemType = "int[]", Name = "myvectorofint" });
            var result = aa.DetermineAllArrayLengths(sh, tree, 10);
            Assert.AreEqual(10, result.Length, "# of events");
            Assert.IsTrue(result.All(x => x.Length == 1), "incorrect individual variable list length list");
            Assert.IsTrue(result.All(x => x[0].Item2 == 10), "incorrect individual variable list length list");
        }
    }
}
