// <copyright file="StatementSimpleStatementTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LINQToTTreeLib.Tests;

namespace LINQToTTreeLib.Statements
{
    /// <summary>This class contains parameterized unit tests for StatementSimpleStatement</summary>
    [TestClass]
    public partial class StatementSimpleStatementTest
    {
        [TestInitialize]
        public void TestSTart()
        {
            TestUtils.ResetLINQLibrary();
        }

        /// <summary>Test stub for CodeItUp()</summary>
        public IEnumerable<string> CodeItUp(StatementSimpleStatement target)
        {
            IEnumerable<string> result = target.CodeItUp();
            var lines = result.ToArray();

            Assert.AreEqual(1, lines.Length, "bad # of lines");
            return result;
            // TODO: add assertions to method StatementSimpleStatementTest.CodeItUp(StatementSimpleStatement)
        }

        /// <summary>Test stub for .ctor(String)</summary>
        public StatementSimpleStatement Constructor(string line)
        {
            StatementSimpleStatement target = new StatementSimpleStatement(line);
            Assert.IsFalse(target.Line.EndsWith(";"), "semicolon should have been stripped off ('" + target.Line + "')");
            Assert.AreNotEqual(0, target.Line, "empty line is not allowed");
            line = line.Trim();
            while (line.EndsWith(";"))
            {
                line = line.Substring(0, line.Length - 1);
                line = line.Trim();
            }
            Assert.AreEqual(line, target.Line, "bad line set");
            return target;
        }

        [TestMethod]
        public void CTorTest()
        {
            Constructor("int j");
        }

        [TestMethod]
        public void CTorTestSmi()
        {
            Constructor("int j;");
        }

        [TestMethod]
        public void TestLine()
        {
            CodeItUp(new StatementSimpleStatement("int j"));
        }

        [TestMethod]
        public void TestTwoSemicolons()
        {
            CodeItUp(new StatementSimpleStatement("int j;"));
        }

        [TestMethod]
        public void TestCombineSame()
        {
            var st1 = new StatementSimpleStatement("int");
            var st2 = new StatementSimpleStatement("int");
            Assert.IsTrue(st1.TryCombineStatement(st2, null), "same statements should combine");

            var st3 = new StatementSimpleStatement("float");
            Assert.IsFalse(st1.TryCombineStatement(st3, null), "diff statements should not combine");
        }

        [TestMethod]
        public void SimpleStatementNoCallBackTillCodeUp()
        {
            bool called = false;
            var st = new StatementSimpleStatement(() => { called = true; return "int j"; });
            Assert.IsFalse(called, "Call back called too early");
            var lines = st.CodeItUp().ToArray();
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void SimpleStatementNoCallBackTillCodeUp2()
        {
            int count = 0;
            var st = new StatementSimpleStatement(() => { count++; return "int j"; });
            var lines = st.CodeItUp().ToArray();
            Assert.AreEqual(1, count);
            var lines2 = st.CodeItUp().ToArray();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void SimpleStatementCallbackLineWithSemicolon()
        {
            var st = new StatementSimpleStatement(() => "int j");
            var lines = st.CodeItUp().ToArray();
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("int j;", lines[0]);
        }

        [TestMethod]
        public void SimpleStatementCallbackAndTransform()
        {
            var st = new StatementSimpleStatement(() => "int j");
            st.RenameVariable("j", "k");
            var lines = st.CodeItUp().ToArray();
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("int k;", lines[0]);
        }

        [TestMethod]
        public void SimpleStatementCallbackAndTransformAfterRendering()
        {
            var st = new StatementSimpleStatement(() => "int j");
            var lines = st.CodeItUp().ToArray();
            st.RenameVariable("j", "k");
            lines = st.CodeItUp().ToArray();
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("int k;", lines[0]);
        }

        [TestMethod]
        public void SimpleStatementCallbackLineWithNoSemicolon()
        {
            var st = new StatementSimpleStatement(() => "}", addSemicolon: false);
            var lines = st.CodeItUp().ToArray();
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("}", lines[0]);
        }

#if false
        [PexMethod]
        public bool TestTryCombine([PexAssumeUnderTest] StatementSimpleStatement target, IStatement st)
        {
            return target.TryCombineStatement(st, null);
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException)), PexAllowedException(typeof(ArgumentException)), PexAllowedException(typeof(AssertFailedException))]
        public StatementSimpleStatement TestRename([PexAssumeUnderTest] StatementSimpleStatement target, string oldvar, string newvar)
        {
            //
            // Make sure that Pex is using a legal variable name
            //

            var goodVar = new Regex(string.Format(@"w+"));
            if (!goodVar.Match(oldvar).Success)
                throw new ArgumentException("The old variable is not a proper variable name");
            if (!goodVar.Match(newvar).Success)
                throw new ArgumentException("THe new variable is not a proper variable name");

            target.RenameVariable(oldvar, newvar);

            if (oldvar != null && oldvar != newvar)
                Assert.IsFalse(Regex.Match(target.Line, string.Format(@"\b{0}\b", oldvar)).Success, "old guy should not be in there!");
            return target;
        }
#endif
    }
}
