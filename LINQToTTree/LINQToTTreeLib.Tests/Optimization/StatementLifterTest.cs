using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Optimization;
using LINQToTTreeLib.Statements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.Optimization
{
    [TestClass]
    public class StatementLifterTest
    {
        [TestInitialize]
        public void Setup()
        {
            TestUtils.ResetLINQLibrary();
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        [TestMethod]
        public void TestEmptyGC()
        {
            var v = new GeneratedCode();
            StatementLifter.Optimize(v);
        }

        [TestMethod]
        public void TestLiftSimpleStatement()
        {
            var v = new GeneratedCode();
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);
            v.Add(new StatementWithNoSideEffects());

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementWithNoSideEffects), "first statement");
        }

        [TestMethod]
        public void TestLiftTwoStatements()
        {
            Assert.Inconclusive("Two stements with no side effects");
        }

        [TestMethod]
        public void TestLiftTwoDependentStatements()
        {
            Assert.Inconclusive("second depends on first, which doesn't depend on anything, both should be lifted and order preserved");
        }

        [TestMethod]
        public void TestLiftSecondStatement()
        {
            var v = new GeneratedCode();
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);
            v.Add(new StatementWithSideEffects(loopP));
            v.Add(new StatementWithNoSideEffects());

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementWithNoSideEffects), "first statement");
            var secondstatement = v.CodeBody.Statements.Skip(1).First();
            Assert.IsInstanceOfType(secondstatement, typeof(StatementForLoop), "second statement");
        }

        [TestMethod]
        public void TestLiftSimpleStatementWithNonOptAfter()
        {
            var v = new GeneratedCode();
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);
            v.Add(new StatementWithNoSideEffects());
            v.Add(new StatementNonOptimizing());

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementWithNoSideEffects), "first statement");
        }

        [TestMethod]
        public void TestLiftSimpleStatementAfterDependentStatement()
        {
            var v = new GeneratedCode();
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);
            v.Add(new StatementWithSideEffects(loopP));
            v.Add(new StatementWithNoSideEffects());

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementWithNoSideEffects), "first statement");
        }

        [TestMethod]
        public void TestNoLiftNonOptStatement()
        {
            var v = new GeneratedCode();
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);
            v.Add(new StatementNonOptimizing());

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementForLoop), "first statement");
        }

        [TestMethod]
        public void TestNoLiftSimpleStatementBlocked()
        {
            var v = new GeneratedCode();
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);
            v.Add(new StatementNonOptimizing());
            v.Add(new StatementWithNoSideEffects());

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementForLoop), "first statement");
        }

        [TestMethod]
        public void TestNoLiftDependentStatement()
        {
            var v = new GeneratedCode();
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);
            v.Add(new StatementWithSideEffects(loopP));

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementForLoop), "first statement");
        }

        [TestMethod]
        public void TestLiftWithDependentStatment()
        {
            var v = new GeneratedCode();
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);
            var loopP2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            v.Add(new StatementWithSideEffects(loopP2));

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementWithSideEffects), "first statement");
        }

        [TestMethod]
        public void TestCodeWithDoubleIndex()
        {
            // Looking for two loops, and the Calc function should be moved outside
            // the first loop for efficiency reasons (as it doesn't use anything in that
            // first loop.

            var q = new QueriableDummy<LINQToTTreeLib.QueryVisitorTest.dummyntup>();

            var res = from f in q
                      from r1 in f.valC1D
                      from r2 in f.vals
                      let rr1 = LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r1)
                      let rr2 = LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r2)
                      where Math.Abs(rr1 - rr2) < 2
                      select f;
            var r = res.Count();

            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("After optimization...");
            Console.WriteLine();
            StatementLifter.Optimize(query);
            query.DumpCodeToConsole();

            var outterBlock = query.QueryCode().First() as IStatementCompound;
            Assert.IsNotNull(outterBlock);
            var outterLoop = outterBlock.Statements.First() as IStatementCompound;
            Assert.IsNotNull(outterLoop);

            // Here we should see one of the cpp code statements.

            var ccpCode = outterLoop.Statements.First();
            Assert.AreEqual("CPPCodeStatement", ccpCode.GetType().Name, "Expected cpp code statement");
        }

        /// <summary>
        /// A simple statement that tracks a single dependent variable.
        /// </summary>
        class StatementWithSideEffects : IStatement, ICMStatementInfo
        {
            private DeclarableParameter _trackedVar;

            public StatementWithSideEffects(DeclarableParameter loopP)
            {
                this._trackedVar = loopP;
            }

            public System.Collections.Generic.IEnumerable<string> CodeItUp()
            {
                throw new NotImplementedException();
            }

            public void RenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }

            public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
            {
                throw new NotImplementedException();
            }

            public IStatement Parent { get; set; }

            public System.Collections.Generic.ISet<string> DependentVariables
            {
                get
                {
                    var r = new HashSet<string>();
                    r.Add(_trackedVar.RawValue);
                    return r;
                }
            }

            public System.Collections.Generic.ISet<string> ResultVariables
            {
                get { return new HashSet<string>(); }
            }
        }

        /// <summary>
        /// A simple statement, but that doesn't know how to participate in optimization.
        /// </summary>
        class StatementNonOptimizing : IStatement
        {
            public System.Collections.Generic.IEnumerable<string> CodeItUp()
            {
                throw new NotImplementedException();
            }

            public void RenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }

            public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
            {
                throw new NotImplementedException();
            }

            public IStatement Parent { get; set; }
        }


        /// <summary>
        /// A very simple statement with no side effects.
        /// </summary>
        class StatementWithNoSideEffects : IStatement, ICMStatementInfo
        {
            public System.Collections.Generic.IEnumerable<string> CodeItUp()
            {
                throw new NotImplementedException();
            }

            public void RenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }

            public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
            {
                throw new NotImplementedException();
            }

            public IStatement Parent { get; set; }

            public System.Collections.Generic.ISet<string> DependentVariables
            {
                get { return new HashSet<string>(); }
            }

            public System.Collections.Generic.ISet<string> ResultVariables
            {
                get { return new HashSet<string>(); }
            }
        }
    }
}
