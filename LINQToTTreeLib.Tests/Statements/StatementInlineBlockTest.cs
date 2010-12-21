// <copyright file="StatementInlineBlockTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Generated;
using LINQToTTreeLib.Variables;

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
    }
}
