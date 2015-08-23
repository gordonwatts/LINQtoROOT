using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.QMFunctions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for GeneratedCode</summary>
    [TestClass]
    public partial class GeneratedCodeTest
    {
#if false
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

        [PexMethod]
        public void Pop([PexAssumeNotNull]IStatement s, bool popPastLoop)
        {
            var gc = new GeneratedCode();
            int depth = gc.Depth;
            gc.Add(s);

            bool good = s is IBookingStatementBlock;
            try
            {
                gc.Pop(popPastLoop);
                Assert.AreEqual(depth, gc.Depth, "Depth isn't set correctly");
                Assert.IsTrue(good, "booking statement");
            }
            catch (InvalidOperationException)
            {
                Assert.IsFalse(good, "a booking statement");
            }
        }

#endif

        [TestMethod]
        public void AddQMFunc()
        {
            var gc = new GeneratedCode();
            Assert.AreEqual(0, gc.QMFunctions.Count(), "# of functions before add");
            gc.Add(new QMFuncSource());
            Assert.AreEqual(1, gc.QMFunctions.Count(), "# of functions before add");
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

            public void RenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }


            public IStatement Parent { get; set; }


            public void Remove(IStatement statement)
            {
                throw new NotImplementedException();
            }

            public void AddBefore(IStatement statement, IStatement beforeThisStatement)
            {
                throw new NotImplementedException();
            }


            public bool Combine(IEnumerable<IStatement> statements, IBookingStatementBlock parent, bool appendIfNoCombine = true)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IStatement> TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
            {
                throw new NotImplementedException();
            }


            public IStatement CombineAndMark(IStatement statement, IBookingStatementBlock parent, bool appendIfNoCombine = true)
            {
                throw new NotImplementedException();
            }


            bool IStatement.TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
            {
                throw new NotImplementedException();
            }


            public bool IsBefore(IStatement first, IStatement second)
            {
                throw new NotImplementedException();
            }
        }

        public class CompoundBookingStatement : CompoundStatement, IBookingStatementBlock
        {
            private List<IDeclaredParameter> _vlist = new List<IDeclaredParameter>();
            public void Add(IDeclaredParameter variableToDeclare, bool failifthere)
            {
                _vlist.Add(variableToDeclare);
            }

            public IEnumerable<IDeclaredParameter> DeclaredVariables
            {
                get { return _vlist; }
            }
            public IEnumerable<IDeclaredParameter> AllDeclaredVariables
            {
                get { return _vlist; }
            }
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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestPopUpOneLevelLoopEmpty()
        {
            GeneratedCode gc = new GeneratedCode();
            gc.Pop(true);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestPopUpOneLevelLoopInline()
        {
            GeneratedCode gc = new GeneratedCode();
            gc.Add(new StatementInlineBlock());
            gc.Pop(true);
        }

        [TestMethod]
        public void TestPopUpOneLevelLoopWithLoop()
        {
            GeneratedCode gc = new GeneratedCode();
            var loopV = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(new StatementForLoop(loopV, new Variables.ValSimple("5", typeof(int))));
            gc.Pop(true);
            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "# of statements present now");
            gc.Add(new StatementAssign(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)), new Variables.ValSimple("ih", typeof(int)), null));
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements present now");
        }

        [TestMethod]
        public void TestPopUpOneLevelLoopWithLoopDeeper()
        {
            GeneratedCode gc = new GeneratedCode();
            var loopV = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(new StatementForLoop(loopV, new Variables.ValSimple("5", typeof(int))));
            gc.Add(new StatementInlineBlock());
            gc.Add(new StatementAssign(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)), new Variables.ValSimple("ih", typeof(int)), null));
            gc.Pop(true);
            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "# of statements present now");
            gc.Add(new StatementAssign(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)), new Variables.ValSimple("ih", typeof(int)), null));
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements present now");
        }

        [TestMethod]
        public void TestCompoundStatemet()
        {
            Pop(new LINQToTTreeLib.Statements.StatementInlineBlock(), false);
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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestResultScopeNull()
        {
            var gc = new GeneratedCode();
            var b = new StatementInlineBlock();
            gc.Add(b);
            gc.AddAtResultScope(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
        }

        [TestMethod]
        public void TestResultScopeSimple()
        {
            var gc = new GeneratedCode();
            var b = new StatementInlineBlock();
            gc.Add(b);
            gc.SetCurrentScopeAsResultScope();
            var b2 = new StatementInlineBlock();
            gc.Add(b2);

            gc.Add(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));

            Assert.AreEqual(0, b.DeclaredVariables.Count(), "variables at outside loop before add");
            Assert.AreEqual(1, b2.DeclaredVariables.Count(), "variables at inner loop after 1 add");

            gc.AddAtResultScope(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));

            Assert.AreEqual(1, b.DeclaredVariables.Count(), "variables at outside loop after 1 add");
            Assert.AreEqual(1, b2.DeclaredVariables.Count(), "variables at inner loop after 2 add");
        }

        [TestMethod]
        public void TestResultScopeCurrentScope()
        {
            var gc = new GeneratedCode();
            var b1 = new StatementInlineBlock();
            gc.Add(b1);
            gc.SetCurrentScopeAsResultScope();
            var outterScope = gc.CurrentScope;

            var b2 = new StatementInlineBlock();
            gc.Add(b2);
            gc.SetCurrentScopeAsResultScope();

            gc.AddAtResultScope(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));

            Assert.AreEqual(0, b1.DeclaredVariables.Count(), "variables at outside loop after 0 add");
            Assert.AreEqual(1, b2.DeclaredVariables.Count(), "variables at inner loop after 1 add");

            gc.CurrentScope = outterScope;

            gc.AddAtResultScope(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));

            Assert.AreEqual(1, b1.DeclaredVariables.Count(), "variables at outside loop after 1 add");
            Assert.AreEqual(1, b2.DeclaredVariables.Count(), "variables at inner loop after 2 add");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestResultScopePop()
        {
            var gc = new GeneratedCode();
            var b1 = new StatementInlineBlock();
            gc.Add(b1);

            var b2 = new StatementInlineBlock();
            gc.Add(b2);
            gc.SetCurrentScopeAsResultScope();

            gc.AddAtResultScope(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));

            // This pop should remove the result scope, and it should be back to "null" now, which
            // should cause an exception.

            gc.Pop();
            gc.AddAtResultScope(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));

        }

        [TestMethod]
        public void TestRememberExprSimple()
        {
            var gc = new GeneratedCode();
            var expr = Expression.Constant(10);
            var r = new LINQToTTreeLib.Variables.ValSimple("10", typeof(int));

            gc.RememberSubExpression(expr, r);

            Assert.AreEqual(r, gc.LookupSubExpression(expr), "Could not find expression");
        }

        [TestMethod]
        public void TestRememberExprSimpleByVal()
        {
            var gc = new GeneratedCode();
            var expr = Expression.Constant(10);
            var r = new LINQToTTreeLib.Variables.ValSimple("10", typeof(int));

            gc.RememberSubExpression(expr, r);

            Assert.AreEqual(r, gc.LookupSubExpression(Expression.Constant(10)), "Could not find expression");
        }

        [TestMethod]
        public void TestRememberExprPop()
        {
            var gc = new GeneratedCode();
            var initialScope = gc.CurrentScope;
            gc.Add(new StatementInlineBlock());

            var expr = Expression.Constant(10);
            var r = new LINQToTTreeLib.Variables.ValSimple("10", typeof(int));
            gc.RememberSubExpression(expr, r);

            gc.CurrentScope = initialScope;

            Assert.IsNull(gc.LookupSubExpression(Expression.Constant(10)), "Expression after popping");
        }

        [TestMethod]
        public void TestRememberExprHideAndSeek()
        {
            var gc = new GeneratedCode();
            var expr = Expression.Constant(5);
            var r1 = new LINQToTTreeLib.Variables.ValSimple("10", typeof(int));
            gc.RememberSubExpression(expr, r1);

            var initialScope = gc.CurrentScope;
            gc.Add(new StatementInlineBlock());

            var r2 = new LINQToTTreeLib.Variables.ValSimple("11", typeof(int));
            gc.RememberSubExpression(expr, r2);

            Assert.AreEqual(r2, gc.LookupSubExpression(expr), "Is hidden one done right?");
            gc.Pop();
            Assert.AreEqual(r1, gc.LookupSubExpression(expr), "Is revealed one done right?");
        }

        /// <summary>
        /// Seen in real code. Make sure this works for a single object
        /// </summary>
        [TestMethod]
        public void TestRememberConstantObject()
        {
            var gc = new GeneratedCode();
            var expr = Expression.Constant(new ROOTNET.NTH1F());
            var r1 = new LINQToTTreeLib.Variables.ValSimple("10", typeof(int));
            gc.RememberSubExpression(expr, r1);

            Assert.AreEqual(r1, gc.LookupSubExpression(expr), "Constant Expressions of arbitrary objects can't be looked up");
        }

        /// <summary>
        /// Perhaps found in the wild - a sub-expression, that includes a manip of the constant
        /// expression.
        /// </summary>
        [TestMethod]
        public void TestRememberEmbededConstExpr()
        {
            var gc = new GeneratedCode();
            var c1 = Expression.Constant(new ROOTNET.NTH1F("hi", "there", 100, 0.0, 10.0));
            var c2 = Expression.Constant(new ROOTNET.NTH1F("no", "way", 100, 0.0, 10.0));

            var n1 = Expression.Call(c1, typeof(ROOTNET.NTH1F).GetMethod("GetNbinsX"));
            var n2 = Expression.Call(c2, typeof(ROOTNET.NTH1F).GetMethod("GetNbinsX"));

            var r1 = new ValSimple("1", typeof(int));
            var r2 = new ValSimple("2", typeof(int));

            gc.RememberSubExpression(n1, r1);
            gc.RememberSubExpression(n2, r2);

            Assert.AreEqual(r1, gc.LookupSubExpression(n1), "lookup n1");
            Assert.AreEqual(r2, gc.LookupSubExpression(n2), "lookup n2");
        }

        /// <summary>
        /// Seen in real code. Make sure two objects that are the same, but are not the same
        /// object actually link properly.
        /// </summary>
        [TestMethod]
        public void TestRememberTwoSameConstantObjects()
        {
            var gc = new GeneratedCode();
            var expr1 = Expression.Constant(new ROOTNET.NTH1F());
            var expr2 = Expression.Constant(new ROOTNET.NTH1F());
            var r1 = new LINQToTTreeLib.Variables.ValSimple("10", typeof(int));
            var r2 = new LINQToTTreeLib.Variables.ValSimple("11", typeof(int));
            gc.RememberSubExpression(expr1, r1);
            gc.RememberSubExpression(expr2, r2);

            Assert.AreEqual(r1, gc.LookupSubExpression(expr1), "expr1 failure");
            Assert.AreEqual(r2, gc.LookupSubExpression(expr2), "expr2 failure");
        }

        [TestMethod]
        public void TestInScopeNoChangeInScope()
        {
            var gc = new GeneratedCode();
            Assert.IsTrue(gc.InScopeNow(gc.CurrentScope), "current scope should be in scope now.");
        }

        [TestMethod]
        public void TestInScopeDownOneOK()
        {
            var gc = new GeneratedCode();
            var sc = gc.CurrentScope;
            gc.Add(new StatementFilter(new ValSimple("true", typeof(bool))));
            Assert.IsTrue(gc.InScopeNow(sc), "current scope should be in scope now.");
        }

        [TestMethod]
        public void TestInScopeUpOneNotOK()
        {
            var gc = new GeneratedCode();
            gc.Add(new StatementFilter(new ValSimple("true", typeof(bool))));
            var sc = gc.CurrentScope;
            gc.Pop();
            Assert.IsFalse(gc.InScopeNow(sc), "current scope should be in scope now.");
        }

        /// <summary>
        /// If a variable is not declared, then we have an error.
        /// </summary>
        [TestMethod]
        public void TestFirstBookingBad()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var gc = new GeneratedCode();
            Assert.IsNull(gc.FirstAllInScopeFromNow(new IDeclaredParameter[] { i }));
        }

        [TestMethod]
        public void TestFirstBookingThisLevel()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var gc = new GeneratedCode();
            gc.Add(i);
            var r = gc.FirstAllInScopeFromNow(new IDeclaredParameter[] { i });
            Assert.AreEqual(r, gc.CodeBody, "Same code body.");
        }

        [TestMethod]
        public void TestFirstBookingNextLevel()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var gc = new GeneratedCode();
            gc.Add(i);
            gc.Add(new StatementFilter(new ValSimple("dude", typeof(bool))));
            var r = gc.FirstAllInScopeFromNow(new IDeclaredParameter[] { i });
            Assert.AreEqual(r, gc.CodeBody, "Same code body.");
        }

        [TestMethod]
        public void TestFirstBookingNextLevelIt()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var gc = new GeneratedCode();
            var ifs = new StatementFilter(new ValSimple("dude", typeof(bool)));
            gc.Add(ifs);
            gc.Add(i);
            var r = gc.FirstAllInScopeFromNow(new IDeclaredParameter[] { i });
            Assert.AreEqual(r, ifs, "Same code body.");
        }

        [TestMethod]
        public void RemoveStatement()
        {
            var fixture = new GeneratedCode();
            fixture.Add(new StatementInlineBlock());
            var s1 = new SimpleStatement();
            fixture.Add(s1);

            Assert.AreEqual(1, (fixture.CodeBody.Statements.First() as IBookingStatementBlock).Statements.Count(), "before removal");
            fixture.Remove(s1);
            Assert.AreEqual(0, (fixture.CodeBody.Statements.First() as IBookingStatementBlock).Statements.Count(), "after removal");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveMissingStatement()
        {
            var fixture = new GeneratedCode();
            fixture.Add(new StatementInlineBlock());
            var s1 = new SimpleStatement();
            fixture.Remove(s1);
        }
    }
}
