// <copyright file="StatementInlineBlockTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Statements
{
    /// <summary>This class contains parameterized unit tests for StatementInlineBlock</summary>
    [PexClass(typeof(StatementInlineBlock))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class StatementInlineBlockTest
    {
        /// <summary>Test stub for Add(IStatement)</summary>
        [PexMethod]
        [PexUseType(typeof(StatementIncrementInteger))]
        public void Add(
            [PexAssumeUnderTest]StatementInlineBlock target,
            IStatement statement
        )
        {
            target.Add(statement);
            Assert.AreEqual(1, target.Statements.Count(), "Expected a statement to have been added");
            Assert.IsFalse(target.Statements.Any(s => s == null), "Should never add a null statement");
        }

        [PexMethod]
        [PexUseType(typeof(StatementIncrementInteger))]
        public void Add(
            [PexAssumeUnderTest]StatementInlineBlock target,
            IVariable var
        )
        {
            target.Add(var);
            Assert.AreEqual(1, target.DeclaredVariables.Count(), "Expected a statement to have been added");
            Assert.IsFalse(target.DeclaredVariables.Any(s => s == null), "Should never add a null statement");
        }

        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public StatementInlineBlock Constructor()
        {
            StatementInlineBlock target = new StatementInlineBlock();
            Assert.AreEqual(0, target.Statements.Count(), "Expected no statements after creation");
            return target;
        }

        /// <summary>Test stub for get_Statements()</summary>
        [PexMethod]
        public IEnumerable<IStatement> StatementsGet([PexAssumeUnderTest]StatementInlineBlock target)
        {
            IEnumerable<IStatement> result = target.Statements;
            return result;
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddThrowsArgumentNullException586()
        {
            StatementInlineBlock statementInlineBlock;
            statementInlineBlock = new StatementInlineBlock();
            this.Add(statementInlineBlock, (IStatement)null);
        }
        [TestMethod]
        public void Add472()
        {
            StatementInlineBlock statementInlineBlock;
            statementInlineBlock = new StatementInlineBlock();
            this.Add(statementInlineBlock, (IStatement)statementInlineBlock);
            Assert.IsNotNull((object)statementInlineBlock);
            Assert.IsNotNull(statementInlineBlock.Statements);
        }
        [TestMethod]
        public void Constructor545()
        {
            StatementInlineBlock statementInlineBlock;
            statementInlineBlock = this.Constructor();
            Assert.IsNotNull((object)statementInlineBlock);
            Assert.IsNotNull(statementInlineBlock.Statements);
        }
        [TestMethod]
        public void Add47201()
        {
            StatementInlineBlock statementInlineBlock;
            statementInlineBlock = StatementInlineBlockFactory.Create();
            this.Add(statementInlineBlock, (IStatement)statementInlineBlock);
            Assert.IsNotNull((object)statementInlineBlock);
            Assert.IsNotNull(statementInlineBlock.Statements);
            Assert.IsNotNull(statementInlineBlock.DeclaredVariables);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddThrowsArgumentNullException469()
        {
            StatementInlineBlock statementInlineBlock;
            statementInlineBlock = StatementInlineBlockFactory.Create();
            this.Add(statementInlineBlock, (IStatement)null);
        }
        [TestMethod]
        public void Add803()
        {
            StatementInlineBlock statementInlineBlock;
            statementInlineBlock = StatementInlineBlockFactory.Create();
            VarInteger s0 = new VarInteger();
            this.Add(statementInlineBlock, (IVariable)s0);
            Assert.IsNotNull((object)statementInlineBlock);
            Assert.IsNotNull(statementInlineBlock.Statements);
            Assert.IsNotNull(statementInlineBlock.DeclaredVariables);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddThrowsArgumentNullException78()
        {
            StatementInlineBlock statementInlineBlock;
            statementInlineBlock = StatementInlineBlockFactory.Create();
            this.Add(statementInlineBlock, (IVariable)null);
        }
        [TestMethod]
        public void Add47202()
        {
            StatementInlineBlock statementInlineBlock;
            statementInlineBlock = StatementInlineBlockFactory.Create();
            this.Add(statementInlineBlock, (IStatement)statementInlineBlock);
            Assert.IsNotNull((object)statementInlineBlock);
            Assert.IsNotNull(statementInlineBlock.Statements);
            Assert.IsNotNull(statementInlineBlock.DeclaredVariables);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddThrowsArgumentNullException313()
        {
            StatementInlineBlock statementInlineBlock;
            statementInlineBlock = StatementInlineBlockFactory.Create();
            this.Add(statementInlineBlock, (IStatement)null);
        }

        [TestMethod]
        public void TestCodeItUp()
        {
            StatementInlineBlock b = new StatementInlineBlock();
            Assert.AreEqual(0, b.CodeItUp().Count(), "Expect nothing for an empty inline block");
        }

        [TestMethod]
        public void TestSimpleCodeing()
        {
            StatementInlineBlock b = new StatementInlineBlock();
            b.Add(new StatementSimpleStatement("junk;"));
            var r = b.CodeItUp().ToArray();
            Assert.AreEqual(3, r.Length, "incorrect number of lines");
            Assert.AreEqual("{", r[0], "open bracket");
            Assert.AreEqual("}", r[2], "close bracket");
            Assert.AreEqual("  junk;", r[1], "statement");
        }

        [TestMethod]
        public void TestSimpleVariableCoding()
        {
            StatementInlineBlock b = new StatementInlineBlock();
            b.Add(new VarInteger());
            var r = b.CodeItUp().ToArray();
            Assert.AreEqual(3, r.Length, "incorrect number of lines");
            Assert.AreEqual("{", r[0], "open bracket");
            Assert.AreEqual("}", r[2], "close bracket");
            Assert.IsTrue(r[1].EndsWith("=0;"));
        }

        [TestMethod]
        public void TestSimpleVariableCodingNoDecl()
        {
            StatementInlineBlock b = new StatementInlineBlock();
            b.Add(new VarInteger() { Declare = false });
            var r = b.CodeItUp().ToArray();
            Assert.AreEqual(0, r.Length, "# of statements");
        }

        [TestMethod]
        public void TestSimpleVariableCodingNoDeclAndDecl()
        {
            StatementInlineBlock b = new StatementInlineBlock();
            b.Add(new VarInteger() { Declare = false });
            b.Add(new VarInteger());
            var r = b.CodeItUp().ToArray();
            Assert.AreEqual(3, r.Length, "# of statements");
        }
    }
}
