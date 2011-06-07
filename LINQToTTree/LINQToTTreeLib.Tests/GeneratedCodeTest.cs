// <copyright file="GeneratedCodeTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
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


            public bool TryCombineStatement(IStatement statement)
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

        /// <summary>
        /// No matter what, there should always be one statement in there!
        /// </summary>
        /// <param name="s"></param>
        [PexMethod]
        public void LookAtStatements([PexAssumeUnderTest] GeneratedCode target)
        {
            Assert.IsNotNull(target.QueryCode(), "always a good statement for jokes!");
            Assert.AreEqual(1, target.QueryCode().Count(), "Single query should have a single statement at all times");
            Assert.AreEqual(target.CodeBody, target.QueryCode().First(), "The code body is what we should be seeing here!");
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
        [PexUseType(typeof(StatementInlineBlock))]
        [PexUseType(typeof(StatementIncrementInteger))]
        public void TestChangeScope([PexAssumeNotNull]IStatement[] initialStatements, [PexAssumeNotNull] IStatement s)
        {
            ///
            /// Check that scoping correctly moves back tot he right place and inserts the stements and
            /// the variables.
            /// 


            ///
            /// Initial setup - we need to do this to prevent some sort of funny insertion that Pex finds that
            /// invalidates this test.
            /// 

            GeneratedCode target = new GeneratedCode();
            foreach (var stat in initialStatements)
            {
                target.Add(stat);
            }

            ///
            /// Now, mark where we are and count initial conditions
            /// 

            var currentScope = target.CurrentScope;

            ///
            /// Now insert the statement twice and the variables too.
            /// 

            var v1 = new VarInteger();
            target.Add(v1);
            target.Add(s);
            target.CurrentScope = currentScope;
            var v2 = new VarInteger();
            target.Add(v2);
            target.Add(s);
        }

        [TestMethod]
        public void TestChangeScopeSpecificTopLevel()
        {
            GeneratedCode target = new GeneratedCode();
            IStatement s = new StatementInlineBlock();

            var currentS = target.CurrentScope;

            var deepestStatementLevel = TestUtils.GetDeepestStatementLevel(target);
            var deepestDeclarLevel = TestUtils.GetDeepestBookingLevel(target);

            var curVars = deepestDeclarLevel.DeclaredVariables.Count();
            var curStatements = deepestStatementLevel.Statements.Count();
            var v1 = new VarInteger();
            target.Add(v1);
            target.Add(s);

            target.CurrentScope = currentS;

            var v2 = new VarInteger();
            target.Add(v2);
            target.Add(s);
            Assert.AreEqual(curStatements + 2, deepestStatementLevel.Statements.Count(), "Scope reset, should always be two extra statements here!");
            Assert.AreEqual(curVars + 2, deepestDeclarLevel.DeclaredVariables.Count(), "Scope reset should have also reset where the variable was pointing");
        }

        [TestMethod]
        public void TestChangeScopeSpecificNextLevel()
        {
            GeneratedCode target = new GeneratedCode();
            target.Add(new StatementInlineBlock());
            IStatement s = new StatementInlineBlock();

            var deepestStatementLevel = TestUtils.GetDeepestStatementLevel(target);
            var deepestDeclarLevel = TestUtils.GetDeepestBookingLevel(target);

            var currentS = target.CurrentScope;

            var curVars = deepestDeclarLevel.DeclaredVariables.Count();
            var curStatements = deepestStatementLevel.Statements.Count();
            var v1 = new VarInteger();
            target.Add(v1);
            target.Add(s);

            target.CurrentScope = currentS;

            var v2 = new VarInteger();
            target.Add(v2);
            target.Add(s);
            Assert.AreEqual(curStatements + 2, deepestStatementLevel.Statements.Count(), "Scope reset, should always be two extra statements here!");
            Assert.AreEqual(curVars + 2, deepestDeclarLevel.DeclaredVariables.Count(), "Scope reset should have also reset where the variable was pointing");
        }

        [PexMethod]
        public void TestAddTransfer([PexAssumeUnderTest] GeneratedCode target, string name, object val)
        {
            int count = target.VariablesToTransfer.Count();
            HashSet<string> names = new HashSet<string>(target.VariablesToTransfer.Select(v => v.Key));
            target.QueueForTransfer(name, val);
            names.Add(name);
            Assert.IsNotNull(target.VariablesToTransfer.Last());
            Assert.AreEqual(names.Count, target.VariablesToTransfer.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddOneLevelUpTopLevel()
        {
            GeneratedCode gc = new GeneratedCode();
            gc.AddOneLevelUp(new Variables.VarSimple(typeof(int)));
        }

        [TestMethod]
        public void TestAddOneLevelUpSingleLevel()
        {
            GeneratedCode gc = new GeneratedCode();
            gc.Add(new Statements.StatementInlineBlock());
            gc.AddOneLevelUp(new Variables.VarSimple(typeof(int)));
            Assert.AreEqual(1, gc.CodeBody.DeclaredVariables.Count(), "Expected top level decl");
        }
    }
}
