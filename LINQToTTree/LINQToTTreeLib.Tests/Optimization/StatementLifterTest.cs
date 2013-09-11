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
            var v = new GeneratedCode();
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);
            v.Add(new StatementWithSideEffects(loopP));
            v.Add(new StatementWithNoSideEffects());
            v.Add(new StatementWithNoSideEffects());

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementWithNoSideEffects), "first statement");
            var secondStatement = v.CodeBody.Statements.Skip(1).First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementWithNoSideEffects), "second statement");
            var thirdstatement = v.CodeBody.Statements.Skip(2).First();
            Assert.IsInstanceOfType(thirdstatement, typeof(StatementForLoop), "third statement");
        }

        [TestMethod]
        public void TestLiftTwoDependentStatements()
        {
            var v = new GeneratedCode();
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);

            var var1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var var2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            v.Add(new StatementWithSideEffects(var1, var2));
            v.Add(new StatementWithSideEffects(var2));

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementWithSideEffects), "first statement");
            var secondStatement = v.CodeBody.Statements.Skip(1).First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementWithSideEffects), "second statement");
            var thirdstatement = v.CodeBody.Statements.Skip(2).First();
            Assert.IsInstanceOfType(thirdstatement, typeof(StatementForLoop), "third statement");
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

        [TestMethod]
        public void TestCodeWithDoubleIndexAndFunction()
        {
            // Looking for two loops, and the Calc function should be moved outside
            // the first loop for efficiency reasons (as it doesn't use anything in that
            // first loop.

            var q = new QueriableDummy<LINQToTTreeLib.QueryVisitorTest.dummyntup>();

            var res = from f in q
                      from r1 in f.valC1D
                      from r2 in f.vals
                      let rr1 = Math.Abs(LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r1))
                      let rr2 = Math.Abs(LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r2))
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
            var assCode = outterLoop.Statements.Skip(1).First();
            Assert.AreEqual("StatementAssign", assCode.GetType().Name, "Lifted assignment statement.");
        }

        /// <summary>
        /// A loop contains an if statement that exists above - so they could be combined
        /// if the if statement an the loop were reversed. This optimization is tested by
        /// this code.
        /// </summary>
        [TestMethod]
        public void TestLoopBuriesCommonIfStatement()
        {
            var q = new QueriableDummy<LINQToTTreeLib.QueryVisitorTest.dummyntup>();

            var res1 = from f in q
                       from r1 in f.valC1D
                       let rr1 = Math.Abs(LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r1))
                       where r1 > 2
                       select rr1;
            var resu1 = res1.Aggregate(0, (acc, v) => acc + v);
            var query1 = DummyQueryExectuor.FinalResult;

            var res2 = from f in q
                       from r1 in f.valC1D
                       from r2 in f.valC1D
                       where r1 > 2
                       let rr1 = Math.Abs(LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r1))
                       let rr2 = Math.Abs(LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r2))
                       select rr1 + rr2;
            var resu2 = res2.Aggregate(0, (acc, v) => acc + v);
            var query2 = DummyQueryExectuor.FinalResult;

            // Combine the queries

            var query = CombineQueries(query1, query2);
            Console.WriteLine("Unoptimized");
            query.DumpCodeToConsole();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("After optimization...");
            Console.WriteLine();
            StatementLifter.Optimize(query as IGeneratedQueryCode);
            query.DumpCodeToConsole();

            Assert.Inconclusive("Not coded yet");
        }

        /// <summary>
        /// Do the code combination we require!
        /// </summary>
        /// <param name="gcs"></param>
        /// <returns></returns>
        private IExecutableCode CombineQueries(params IExecutableCode[] gcs)
        {
            var combinedInfo = new CombinedGeneratedCode();
            foreach (var cq in gcs)
            {
                combinedInfo.AddGeneratedCode(cq);
            }

            return combinedInfo;
        }

        /// <summary>
        /// A simple statement that tracks a single dependent variable.
        /// </summary>
        class StatementWithSideEffects : IStatement, ICMStatementInfo
        {
            private DeclarableParameter _trackedVar;
            private DeclarableParameter _resultVar;

            public StatementWithSideEffects(DeclarableParameter loopP, DeclarableParameter result = null)
            {
                this._trackedVar = loopP;
                this._resultVar = result;
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
                get
                {
                    var r = new HashSet<string>();
                    if (_resultVar != null)
                        r.Add(_resultVar.RawValue);
                    return r;
                }
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
