using System;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for ExpressionUtilitiesTest and is intended
    ///to contain all ExpressionUtilitiesTest Unit Tests
    ///</summary>
    [TestClass()]
    public partial class ExpressionUtilitiesTest
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
            Assert.AreEqual("((a)-(b))", new ValSimple("(a)-(b)", typeof(int)).ApplyParensIfNeeded(), "single term");
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public string TestApply(string value)
        {
            return value.ApplyParensIfNeeded();
        }

        [TestMethod]
        public void TestArrayRemovalNull()
        {
            Expression<Func<int[], int>> t1 = a => a[0];
            var r = t1.RemoveArrayReferences();
            Assert.AreEqual(t1, r, "Didn't return the same thing");
        }

        [TestMethod]
        public void TestArrayRemoval1D()
        {
            Expression<Func<int[], int>> t1 = a => a[0];
            var r = t1.Body.RemoveArrayReferences();
            Assert.AreEqual("a", r.ToString(), "Didn't return the same thing");
        }

        [TestMethod]
        public void TestArrayRemoval2D()
        {
            Expression<Func<int[][], int>> t1 = a => a[0][5];
            var r = t1.Body.RemoveArrayReferences();
            Assert.AreEqual("a", r.ToString(), "Didn't return the same thing");
        }
    }
}
