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
            TestUtils.ResetLINQLibrary();
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
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

        /// <summary>
        ///A test for ResolveTypedef
        ///</summary>
        [TestMethod()]
        public void ResolveTypedefTest()
        {
            Assert.AreEqual("int", TypeDefTranslator.ResolveTypedef("int"), "No change expected");
            Assert.AreEqual("unsigned int", TypeDefTranslator.ResolveTypedef("size_t"), "Should have found it");
        }

        /// <summary>
        /// We have to do a test here, as if the dll's are still loaded from last time we ran, then this guy
        /// won't work correctly. A shame, but...
        /// </summary>
        [TestMethod]
        public void DefineNewTypedef()
        {
            var original = TypeDefTranslator.ResolveTypedef("myfork");
            if (original != "myfork")
            {
                ROOTNET.NTROOT.gROOT.ProcessLine("typedef int myfork;");
                /// Force a re-load - normally someone else behind our backs does this - like after a copmile or similar.
                ROOTNET.NTROOT.gROOT.GetListOfTypes(true);

                Assert.AreEqual("int", TypeDefTranslator.ResolveTypedef("myfork"), "the myfork typdef should now be defined");
            }
        }
    }
}
