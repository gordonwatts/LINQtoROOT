// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
// 
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
// 
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>
using System;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{
    public partial class ExpressionUtilitiesTest
    {
        [TestMethod]
        [PexGeneratedBy(typeof(ExpressionUtilitiesTest))]
        public void TestApply40()
        {
            string s;
            s = this.TestApply("\0");
            Assert.AreEqual<string>("(\0)", s);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(ExpressionUtilitiesTest))]
        public void TestApply438()
        {
            string s;
            s = this.TestApply("\u00aa");
            Assert.AreEqual<string>("\u00aa", s);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(ExpressionUtilitiesTest))]
        public void TestApply454()
        {
            string s;
            s = this.TestApply("()");
            Assert.AreEqual<string>("()", s);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(ExpressionUtilitiesTest))]
        public void TestApply718()
        {
            string s;
            s = this.TestApply("\u00aa\0");
            Assert.AreEqual<string>("(\u00aa\0)", s);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(ExpressionUtilitiesTest))]
        public void TestApply955()
        {
            string s;
            s = this.TestApply("\u00aa\u0100");
            Assert.AreEqual<string>("\u00aa\u0100", s);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(ExpressionUtilitiesTest))]
        public void TestApply390()
        {
            string s;
            s = this.TestApply("\u0100\u0100\0");
            Assert.AreEqual<string>("(\u0100\u0100\0)", s);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(ExpressionUtilitiesTest))]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestApplyThrowsArgumentNullException284()
        {
            string s;
            s = this.TestApply((string)null);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(ExpressionUtilitiesTest))]
        public void TestApply270()
        {
            string s;
            s = this.TestApply("(\0");
            Assert.AreEqual<string>("(\0", s);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(ExpressionUtilitiesTest))]
        public void TestApply662()
        {
            string s;
            s = this.TestApply("((");
            Assert.AreEqual<string>("((", s);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(ExpressionUtilitiesTest))]
        public void TestApply43802()
        {
            string s;
            s = this.TestApply("()\0");
            Assert.AreEqual<string>("(()\0)", s);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(ExpressionUtilitiesTest))]
        public void TestApply348()
        {
            string s;
            s = this.TestApply("(\0\u0100");
            Assert.AreEqual<string>("(\0\u0100", s);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(ExpressionUtilitiesTest))]
        public void TestApply731()
        {
            string s;
            s = this.TestApply("(((");
            Assert.AreEqual<string>("(((", s);
        }
    }
}