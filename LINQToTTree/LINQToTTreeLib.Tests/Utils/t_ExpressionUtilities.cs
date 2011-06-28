using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for ExpressionUtilitiesTest and is intended
    ///to contain all ExpressionUtilitiesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExpressionUtilitiesTest
    {
        /// <summary>
        ///A test for ApplyParensIfNeeded
        ///</summary>
        [TestMethod()]
        public void ApplyParensIfNeededTest()
        {
            Assert.AreEqual("a", new ValSimple("a", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("a1", new ValSimple("a1", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("a_1", new ValSimple("a_1", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("(a+b)", new ValSimple("a+b", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("(a/b)", new ValSimple("a/b", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("(-a)", new ValSimple("-a", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("(a)", new ValSimple("(a)", typeof(int)).ApplyParensIfNeeded(), "single term");
        }

        [PexMethod]
        public string TestApply(string value)
        {
            return value.ApplyParensIfNeeded();
        }
    }
}
