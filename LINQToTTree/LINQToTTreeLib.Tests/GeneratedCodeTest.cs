// <copyright file="GeneratedCodeTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for GeneratedCode</summary>
    [PexClass(typeof(GeneratedCode))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class GeneratedCodeTest
    {
        /// <summary>Test stub for Add(IStatement)</summary>
        [PexMethod]
        [PexUseType(typeof(StatementIncrementInteger))]
        public void Add([PexAssumeUnderTest]GeneratedCode target, IStatement s)
        {
            Assert.IsNotInstanceOfType(s, typeof(IStatementCompound), "Not testing this!");
            target.Add(s);
            Assert.AreEqual(1, target.CodeBody.Statements.Count(), "Expected a single statement to have been added");
        }

        public class SimpleStatement : IStatement
        {
            public IEnumerable<string> CodeItUp()
            {
                throw new NotImplementedException();
            }
        }

        public class CompoundStatement : IStatementCompound
        {
            private List<IStatement> _list = new List<IStatement>();
            public System.Collections.Generic.IEnumerable<IStatement> Statements
            {
                get { return _list; }
            }

            public void Add(IStatement statement)
            {
                _list.Add(statement);
            }

            public IEnumerable<string> CodeItUp()
            {
                throw new NotImplementedException();
            }
        }

        public class CompoundBookingStatement : CompoundStatement, IBookingStatementBlock
        {
            private List<IVariable> _vlist = new List<IVariable>();
            public void Add(IVariable variableToDeclare)
            {
                _vlist.Add(variableToDeclare);
            }

            public IEnumerable<IVariable> DeclaredVariables
            {
                get { return _vlist; }
            }
        }


        [PexMethod]
        [PexUseType(typeof(CompoundBookingStatement))]
        [PexUseType(typeof(CompoundStatement))]
        [PexUseType(typeof(SimpleStatement))]
        public void AddCompoundStatementTest(IStatement s)
        {
            var target = new GeneratedCode();
            var v = new VarInteger();
            var boringStatement = new StatementIncrementInteger(v);

            target.Add(s);
            target.Add(v);
            target.Add(boringStatement);

            int expectedTopLevelStatements = 2;
            if (s is IStatementCompound)
                expectedTopLevelStatements = 1;

            int expectedTopLevelVariables = 1;
            if (s is IBookingStatementBlock)
                expectedTopLevelVariables = 0;

            Assert.AreEqual(expectedTopLevelStatements, target.CodeBody.Statements.Count(), "Incorrect # of statements");
            Assert.AreEqual(expectedTopLevelVariables, target.CodeBody.DeclaredVariables.Count(), "Incorrect # of variables");
        }

        [PexMethod]
        public void AddCompound()
        {
            var s = new StatementInlineBlock();
            var target = new GeneratedCode();
            target.Add(s);
            Assert.AreEqual(1, target.CodeBody.Statements.Count(), "Expected a single statement to have been added");

            ///
            /// Adding a second statement should not alter the top level statement.
            /// 

            target.Add(new StatementIncrementInteger(new VarInteger()));
            Assert.AreEqual(1, target.CodeBody.Statements.Count(), "Expected a single statement to have been added");
        }

        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public GeneratedCode Constructor()
        {
            GeneratedCode target = new GeneratedCode();
            Assert.IsNull(target.ResultValue, "Expected no result");
            Assert.IsNotNull(target.CodeBody, "Expected a code body object to exist");
            return target;
        }

        /// <summary>Test stub for SetResult(IVariable)</summary>
        [PexMethod]
        internal void SetResult([PexAssumeUnderTest]GeneratedCode target, IVariable r)
        {
            target.SetResult(r);
            Assert.AreEqual(r, target.ResultValue, "The result value wasn't changed");
        }

        [PexMethod]
        [PexUseType(typeof(StatementIncrementInteger))]
        [PexUseType(typeof(StatementInlineBlock))]
        public void TestChangeScope([PexAssumeUnderTest]GeneratedCode target, IStatement s)
        {
            var currentScope = target.CurrentScope;
            var v = new VarInteger();
            target.Add(v);
            target.Add(s);
            target.CurrentScope = currentScope;
            target.Add(v);
            target.Add(s);
            Assert.AreEqual(2, target.CodeBody.Statements.Count(), "Scope reset, should always be two statements here!");
            Assert.AreEqual(2, target.CodeBody.DeclaredVariables.Count(), "Scope reset should have also reset where the variable was pointing");
        }

        [PexMethod]
        public void TestAddTransfer([PexAssumeUnderTest] GeneratedCode target, string name, object val)
        {
            int count = target.VariablesToTransfer.Count();
            target.QueueForTransfer(name, val);
            Assert.IsNotNull(target.VariablesToTransfer.Last());
            Assert.AreEqual(count + 1, target.VariablesToTransfer.Count());
        }
    }
}
