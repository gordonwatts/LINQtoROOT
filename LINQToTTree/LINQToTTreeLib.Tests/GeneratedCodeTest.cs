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
        public void Pop(IStatement s, bool popPastLoop)
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


            public bool Combine(IEnumerable<IStatement> statements, IBookingStatementBlock parent, bool appendIfNoCombine = true, bool moveIfIdentical = false)
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

            public void Remove(IDeclaredParameter var)
            {
                _vlist.Remove(var);
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

        /// <summary>
        /// Seen in the wild... the internal scope stack isn't getting tracked.
        /// </summary>
        [TestMethod]
        public void CheckPopAndCurrentScopeWorkTogether()
        {
            GeneratedCode target = new GeneratedCode();
            IStatement s1 = new StatementInlineBlock();
            target.Add(s1);
            var depth = target.Depth;
            var s2 = new StatementInlineBlock();
            target.Add(s2);
            var firstTwoDown = target.CurrentScope;

            var s3 = new StatementInlineBlock();
            target.Add(s3);

            target.CurrentScope = firstTwoDown;
            // This pop should put us at the s1 level.
            target.Pop();
            Assert.AreEqual(depth, target.Depth);
        }

        [TestMethod]
        public void CheckDepthAndPopWorkTogether()
        {
            var target = new GeneratedCode();
            var s1 = new StatementInlineBlock();
            target.Add(s1);
            var s2 = new StatementInlineBlock();
            target.Add(s2);
            Assert.AreEqual(3, target.Depth);
            target.Pop();
            Assert.AreEqual(2, target.Depth);
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
            Assert.AreEqual(1, gc.CodeBody.DeclaredVariables.Count(), "Expected top level declaration");
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
            gc.Add(new StatementAssign(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)), new Variables.ValSimple("ih", typeof(int))));
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements present now");
        }

        [TestMethod]
        public void TestPopUpOneLevelLoopWithLoopDeeper()
        {
            GeneratedCode gc = new GeneratedCode();
            var loopV = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(new StatementForLoop(loopV, new Variables.ValSimple("5", typeof(int))));
            gc.Add(new StatementInlineBlock());
            gc.Add(new StatementAssign(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)), new Variables.ValSimple("ih", typeof(int))));
            gc.Pop(true);
            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "# of statements present now");
            gc.Add(new StatementAssign(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)), new Variables.ValSimple("ih", typeof(int))));
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

            gc.RememberSubexpression(expr, r);

            Assert.AreEqual(r, gc.LookupSubexpression(expr), "Could not find expression");
        }

        [TestMethod]
        public void TestRememberExprSimpleByVal()
        {
            var gc = new GeneratedCode();
            var expr = Expression.Constant(10);
            var r = new LINQToTTreeLib.Variables.ValSimple("10", typeof(int));

            gc.RememberSubexpression(expr, r);

            Assert.AreEqual(r, gc.LookupSubexpression(Expression.Constant(10)), "Could not find expression");
        }

        [TestMethod]
        public void TestRememberExprPop()
        {
            var gc = new GeneratedCode();
            var initialScope = gc.CurrentScope;
            gc.Add(new StatementInlineBlock());

            var expr = Expression.Constant(10);
            var r = new LINQToTTreeLib.Variables.ValSimple("10", typeof(int));
            gc.RememberSubexpression(expr, r);

            gc.CurrentScope = initialScope;

            Assert.IsNull(gc.LookupSubexpression(Expression.Constant(10)), "Expression after popping");
        }

        [TestMethod]
        public void TestRememberExprHideAndSeek()
        {
            var gc = new GeneratedCode();
            var expr = Expression.Constant(5);
            var r1 = new LINQToTTreeLib.Variables.ValSimple("10", typeof(int));
            gc.RememberSubexpression(expr, r1);

            var initialScope = gc.CurrentScope;
            gc.Add(new StatementInlineBlock());

            var r2 = new LINQToTTreeLib.Variables.ValSimple("11", typeof(int));
            gc.RememberSubexpression(expr, r2);

            Assert.AreEqual(r2, gc.LookupSubexpression(expr), "Is hidden one done right?");
            gc.Pop();
            Assert.AreEqual(r1, gc.LookupSubexpression(expr), "Is revealed one done right?");
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
            gc.RememberSubexpression(expr, r1);

            Assert.AreEqual(r1, gc.LookupSubexpression(expr), "Constant Expressions of arbitrary objects can't be looked up");
        }

        /// <summary>
        /// Perhaps found in the wild - a sub-expression, that includes a manipulation of the constant
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

            gc.RememberSubexpression(n1, r1);
            gc.RememberSubexpression(n2, r2);

            Assert.AreEqual(r1, gc.LookupSubexpression(n1), "lookup n1");
            Assert.AreEqual(r2, gc.LookupSubexpression(n2), "lookup n2");
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
            gc.RememberSubexpression(expr1, r1);
            gc.RememberSubexpression(expr2, r2);

            Assert.AreEqual(r1, gc.LookupSubexpression(expr1), "expr1 failure");
            Assert.AreEqual(r2, gc.LookupSubexpression(expr2), "expr2 failure");
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
