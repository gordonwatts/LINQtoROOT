// <copyright file="GeneratedCodeTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
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
        [PexUseType(typeof(StatementIncrementInteger)), PexAllowedException(typeof(ArgumentException))]
        public void Add([PexAssumeUnderTest]GeneratedCode target, IStatementCompound s)
        {
            int old = CountStatements(target.CodeBody);
            target.Add(s);
            Assert.AreEqual(old + 1, CountStatements(target.CodeBody), "Expected a single statement to have been added");
        }

        [PexMethod]
        public void AddOutsideLoop([PexAssumeUnderTest] GeneratedCode target, IDeclaredParameter var)
        {
            target.AddOutsideLoop(var);
        }

        [PexMethod]
        public void AddOneLevelUp([PexAssumeUnderTest] GeneratedCode target, IDeclaredParameter var)
        {
            target.AddOneLevelUp(var);
        }

        /// <summary>
        /// Recursively add the items in...
        /// </summary>
        /// <param name="iBookingStatementBlock"></param>
        /// <returns></returns>
        private int CountStatements(IStatementCompound s)
        {
            int cnt = 0;
            foreach (var substatement in s.Statements)
            {
                cnt++;
                if (substatement is IStatementCompound)
                {
                    cnt += CountStatements(substatement as IStatementCompound);
                }
            }

            return cnt;
        }

        public class SimpleStatement : IStatement
        {
            public IEnumerable<string> CodeItUp()
            {
                throw new NotImplementedException();
            }


            public void RenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }


            public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
            {
                throw new NotImplementedException();
            }


            public IStatement Parent { get; set; }
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


            public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
            {
                throw new NotImplementedException();
            }

            public void RenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }


            public IStatement Parent { get; set; }
        }

        public class CompoundBookingStatement : CompoundStatement, IBookingStatementBlock
        {
            private List<IDeclaredParameter> _vlist = new List<IDeclaredParameter>();
            public void Add(IDeclaredParameter variableToDeclare)
            {
                _vlist.Add(variableToDeclare);
            }

            public IEnumerable<IDeclaredParameter> DeclaredVariables
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
        [PexUseType(typeof(SimpleStatement)), PexAllowedException(typeof(ArgumentException))]
        public void AddCompoundStatementTest(IStatement s)
        {
            var target = new GeneratedCode();
            var v = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
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

            target.Add(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
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
        internal void SetResult([PexAssumeUnderTest]GeneratedCode target, DeclarableParameter r)
        {
            target.SetResult(r);
            Assert.AreEqual(r, target.ResultValue, "The result value wasn't changed");
        }

        [PexMethod]
        [PexUseType(typeof(StatementInlineBlock))]
        [PexUseType(typeof(StatementIncrementInteger)), PexAllowedException(typeof(ArgumentException))]
        public void TestChangeScope([PexAssumeNotNull]IStatement[] initialStatements, [PexAssumeNotNull] IStatement s1, [PexAssumeNotNull] IStatement s2)
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

            var v1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            target.Add(v1);
            target.Add(s1);
            target.CurrentScope = currentScope;
            var v2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            target.Add(v2);
            target.Add(s2);
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
            var v1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            target.Add(v1);
            target.Add(s);

            target.CurrentScope = currentS;

            var v2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            target.Add(v2);
            s.Parent = null;
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
            var v1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            target.Add(v1);
            target.Add(s);

            target.CurrentScope = currentS;

            var v2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            target.Add(v2);
            s.Parent = null;
            target.Add(s);
            Assert.AreEqual(curStatements + 2, deepestStatementLevel.Statements.Count(), "Scope reset, should always be two extra statements here!");
            Assert.AreEqual(curVars + 2, deepestDeclarLevel.DeclaredVariables.Count(), "Scope reset should have also reset where the variable was pointing");
        }

        [PexMethod]
        public void TestAddTransfer([PexAssumeUnderTest] GeneratedCode target, object val)
        {
            int count = target.VariablesToTransfer.Count();
            HashSet<string> names = new HashSet<string>(target.VariablesToTransfer.Select(v => v.Key));
            var name = target.QueueForTransfer(val);
            names.Add(name);
            Assert.IsNotNull(target.VariablesToTransfer.Last());
            Assert.AreEqual(names.Count, target.VariablesToTransfer.Count());
        }

        [TestMethod]
        public void TestNameCombo()
        {
            GeneratedCode gc = new GeneratedCode();
            var obj = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            var n1 = gc.QueueForTransfer(obj);
            var n2 = gc.QueueForTransfer(obj);

            Assert.AreEqual(n1, n2, "Names should be identical");
            Assert.AreEqual(1, gc.VariablesToTransfer.Count(), "# of variables to move over wire");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddOneLevelUpTopLevel()
        {
            GeneratedCode gc = new GeneratedCode();
            gc.AddOneLevelUp(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
        }

        [TestMethod]
        public void TestAddOneLevelUpSingleLevel()
        {
            GeneratedCode gc = new GeneratedCode();
            gc.Add(new Statements.StatementInlineBlock());
            gc.AddOneLevelUp(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
            Assert.AreEqual(1, gc.CodeBody.DeclaredVariables.Count(), "Expected top level decl");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddOutsideLoopWithNothing()
        {
            var target = new GeneratedCode();
            target.AddOutsideLoop(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddOutsideLoopWithJustInlineBLock()
        {
            var target = new GeneratedCode();
            target.Add(new Statements.StatementInlineBlock());
            target.AddOutsideLoop(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
        }

        [TestMethod]
        public void TestAddOutsideLoopWithJustInLoop()
        {
            var target = new GeneratedCode();
            var blk = new Statements.StatementInlineBlock();
            target.Add(blk);
            target.Add(new LINQToTTreeLib.Tests.TestUtils.SimpleLoop());
            target.Add(new Statements.StatementInlineBlock());
            target.AddOutsideLoop(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
            Assert.AreEqual(1, blk.DeclaredVariables.Count(), "# of loop declared variables");
        }

        [TestMethod]
        public void TestAddOutsideLoopWithJustASingleLoop()
        {
            var target = new GeneratedCode();
            var blk = new Statements.StatementInlineBlock();
            target.Add(blk);
            target.Add(new LINQToTTreeLib.Tests.TestUtils.SimpleLoop());
            target.AddOutsideLoop(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
            Assert.AreEqual(1, blk.DeclaredVariables.Count(), "# of loop declared variables");
        }

        [TestMethod]
        public void TestAddOutsideLoopWithOnStatement()
        {
            var target = new GeneratedCode();
            var blk = new Statements.StatementInlineBlock();
            target.Add(blk);
            target.Add(new LINQToTTreeLib.Tests.TestUtils.SimpleLoop());
            target.AddOutsideLoop(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
            Assert.AreEqual(1, blk.DeclaredVariables.Count(), "# of loop declared variables");
        }

        [TestMethod]
        public void TestAddOutsideLoopWithJustIn2Loops()
        {
            var target = new GeneratedCode();
            target.Add(new Statements.StatementInlineBlock());
            var loop = new LINQToTTreeLib.Tests.TestUtils.SimpleLoop();
            target.Add(loop);
            target.Add(new LINQToTTreeLib.Tests.TestUtils.SimpleLoop());
            target.AddOutsideLoop(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
            Assert.AreEqual(1, loop.DeclaredVariables.Count(), "# of loop declared variables");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestPopUpOneLevel()
        {
            GeneratedCode gc = new GeneratedCode();
            gc.Pop();
        }

        [PexMethod]
        public void Pop([PexAssumeNotNull]IStatement s)
        {
            var gc = new GeneratedCode();
            int depth = gc.Depth;
            gc.Add(s);

            bool good = s is IBookingStatementBlock;
            try
            {
                gc.Pop();
                Assert.AreEqual(depth, gc.Depth, "Depth isn't set correctly");
                Assert.IsTrue(good, "booking statement");
            }
            catch (InvalidOperationException)
            {
                Assert.IsFalse(good, "a booking statement");
            }
        }

        [TestMethod]
        public void TestCompoundStatemet()
        {
            Pop(new LINQToTTreeLib.Statements.StatementInlineBlock());
        }

        [TestMethod]
        public void TestCompoundPostInsert()
        {
            var gc = new GeneratedCode();
            gc.Add(new Statements.StatementSimpleStatement("dir"));
            var block = new Statements.StatementInlineBlock();
            gc.Add(block);
            gc.Add(new Statements.StatementSimpleStatement("dir"));
            gc.Add(new Statements.StatementSimpleStatement("fork"));
            gc.Pop();
            gc.Add(new Statements.StatementSimpleStatement("dir"));

            Assert.AreEqual(3, gc.CodeBody.Statements.Count(), "# of statements");
        }
    }
}
