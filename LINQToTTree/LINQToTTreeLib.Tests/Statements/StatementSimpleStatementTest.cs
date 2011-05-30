// <copyright file="StatementSimpleStatementTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Statements
{
    /// <summary>This class contains parameterized unit tests for StatementSimpleStatement</summary>
    [PexClass(typeof(StatementSimpleStatement))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class StatementSimpleStatementTest
    {
        /// <summary>Test stub for CodeItUp()</summary>
        [PexMethod]
        public IEnumerable<string> CodeItUp([PexAssumeUnderTest]StatementSimpleStatement target)
        {
            IEnumerable<string> result = target.CodeItUp();
            var lines = result.ToArray();

            Assert.AreEqual(1, lines.Length, "bad # of lines");
            Assert.AreEqual(target.Line + ";", lines[0], "bad line");

            return result;
            // TODO: add assertions to method StatementSimpleStatementTest.CodeItUp(StatementSimpleStatement)
        }

        /// <summary>Test stub for .ctor(String)</summary>
        [PexMethod]
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
    }
}
