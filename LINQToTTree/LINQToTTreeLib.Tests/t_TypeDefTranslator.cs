using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTreeParser;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for TypeDefTranslatorTest and is intended
    ///to contain all TypeDefTranslatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TypeDefTranslatorTest
    {


        private TestContext testContextInstance;

        [TestInitialize]
        public void Setup()
        {
            var a = ROOTNET.NTROOT.gROOT.GetApplication();
        }

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
        ///A test for ResolveTypedef
        ///</summary>
        [TestMethod()]
        public void ResolveTypedefTest()
        {
            Assert.AreEqual("int", TypeDefTranslator.ResolveTypedef("int"), "No change expected");
            Assert.AreEqual("unsigned int", TypeDefTranslator.ResolveTypedef("size_t"), "Should have found it");
        }

        [TestMethod]
        public void DefineNewTypedef()
        {
            Assert.AreEqual("fork", TypeDefTranslator.ResolveTypedef("fork"), "the fork typdef should not have been defined");

            var output = new FileInfo("DefineNewTypedef.C");
            using (var writer = output.CreateText())
            {
                writer.WriteLine("typedef int fork;");
                writer.WriteLine();
                writer.Close();
            }
            int result = ROOTNET.NTSystem.gSystem.CompileMacro("DefineNewTypedef.C");

            Assert.AreEqual("int", TypeDefTranslator.ResolveTypedef("fork"), "the fork typdef should now be defined");
        }
    }
}
