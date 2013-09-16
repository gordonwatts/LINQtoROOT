﻿using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Optimization;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.Optimization
{
    [TestClass]
    public class CommonStatementLifterTest
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
        public void TestEmptyCode()
        {
            var cc = new CombinedGeneratedCode();
            CommonStatementLifter.Optimize(cc);
        }

        [TestMethod]
        public void TestSingleStatement()
        {
            var cc = new CombinedGeneratedCode();
            var gc = new GeneratedCode();
            gc.Add(new StatementSimpleStatement("ls"));
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));

            cc.AddGeneratedCode(gc);
            CommonStatementLifter.Optimize(cc);
        }

        /// <summary>
        /// A line of code that sits inside an if statement. We should not lift it, as it is
        /// not outside the if statement. Optimization should do nothing.
        /// </summary>
        [TestMethod]
        public void TestStatementInIf()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign = new StatementAssign(p1, new ValSimple("f", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            CommonStatementLifter.Optimize(cc);
            cc.DumpCodeToConsole();

            var block1 = cc.QueryCode().First();
            var backIfStatement = block1.Statements.First() as StatementFilter;
            Assert.IsNotNull(backIfStatement, "if statement there");
            var backAssignStatement = backIfStatement.Statements.First() as StatementAssign;
            Assert.IsNotNull(backAssignStatement, "assign statement there");
        }

        /// <summary>
        /// The same statement, in both places. The lift should occur.
        /// </summary>
        [TestMethod]
        public void TestStatementOutAndInIf()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign1);
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign2);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            CommonStatementLifter.Optimize(cc);
            cc.DumpCodeToConsole();

            var block1 = cc.QueryCode().First();
            var firstAssignment = block1.Statements.First() as StatementAssign;
            Assert.IsNotNull(firstAssignment, "first assignment");
            var backIfStatement = block1.Statements.Skip(1).First() as StatementFilter;
            Assert.IsNotNull(backIfStatement, "if statement there");
            Assert.AreEqual(0, backIfStatement.Statements.Count(), "# of if statements inside the if");
        }

        /// <summary>
        /// A pair of the same statements, in both places. The lift should occur.
        /// </summary>
        [TestMethod]
        public void TestTwoStatementOutAndInIf()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign1);
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign11 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign11);
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)), new IDeclaredParameter[] { }, true);
            var assign22 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign22);
            gc.Add(assign2);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            CommonStatementLifter.Optimize(cc);
            cc.DumpCodeToConsole();

            var block1 = cc.QueryCode().First();
            Assert.AreEqual(3, block1.Statements.Count(), "# of statements lifted plus the loop");
            var firstAssignment = block1.Statements.First() as StatementAssign;
            Assert.IsNotNull(firstAssignment, "first assignment");
            var backIfStatement = block1.Statements.Skip(2).First() as StatementFilter;
            Assert.IsNotNull(backIfStatement, "if statement there");
            Assert.AreEqual(0, backIfStatement.Statements.Count(), "# of if statements inside the if");
        }

        /// <summary>
        /// The same statement, in both places. The lift should occur.
        /// </summary>
        [TestMethod]
        public void TestStatementOutAndInIfWithOtherStatements()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign1);
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign2);
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign3);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            CommonStatementLifter.Optimize(cc);
            cc.DumpCodeToConsole();

            var block1 = cc.QueryCode().First();
            var firstAssignment = block1.Statements.First() as StatementAssign;
            Assert.IsNotNull(firstAssignment, "first assignment");
            var backIfStatement = block1.Statements.Skip(1).First() as StatementFilter;
            Assert.IsNotNull(backIfStatement, "if statement there");
            Assert.AreEqual(1, backIfStatement.Statements.Count(), "# of if statements inside the if");
        }

        /// <summary>
        /// Identical loops should not be lifted! That would be very bad. If they are lifted, then
        /// you will miss a 2D run on something!
        /// </summary>
        [TestMethod]
        public void TestIdenticalLoopsStayPut()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));

            gc.Add(new DummyLoop());

            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign1);

            gc.Add(new DummyLoop());

            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign2 = new StatementAssign(p2, new ValSimple("f", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign2);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            CommonStatementLifter.Optimize(cc);
            cc.DumpCodeToConsole();

            var block1 = cc.QueryCode().First();
            var firstLoop = block1.Statements.First() as DummyLoop;
            Assert.IsNotNull(firstLoop, "first loop");
            Assert.AreEqual(2, firstLoop.Statements.Count(), "# of statements in first loop");
            var secondLoop = firstLoop.Statements.Skip(1).First() as DummyLoop;
            Assert.IsNotNull(secondLoop, "second loop");
            Assert.AreEqual(1, secondLoop.Statements.Count(), "# of satements in second loop");
        }

        /// <summary>
        /// A dummy loop.
        /// </summary>
        class DummyLoop : IStatementLoop, IStatementCompound
        {
            private List<IStatement> _statements = new List<IStatement>();
            public System.Collections.Generic.IEnumerable<IStatement> Statements
            {
                get { return _statements; }
            }

            public void Add(IStatement statement)
            {
                _statements.Add(statement);
            }

            public void Remove(IStatement statement)
            {
                throw new NotImplementedException();
            }

            public void AddBefore(IStatement statement, IStatement beforeThisStatement)
            {
                throw new NotImplementedException();
            }

            public System.Collections.Generic.IEnumerable<string> CodeItUp()
            {
                yield return "{";
                foreach (var s in _statements)
                {
                    foreach (var l in s.CodeItUp())
                    {
                        yield return "  " + l;
                    }
                }
                yield return "}";
            }

            public void RenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Allow combinations that are dummy loops. Otherwise, fail.
            /// </summary>
            /// <param name="statement"></param>
            /// <param name="optimize"></param>
            /// <returns></returns>
            public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
            {
                if (statement is DummyLoop)
                    return true;
                return false;
            }

            public IStatement Parent { get; set; }

            /// <summary>
            /// We need to do the combine.
            /// </summary>
            /// <param name="statements"></param>
            /// <param name="parent"></param>
            /// <param name="appendIfNoCombine"></param>
            /// <returns></returns>
            public bool Combine(IEnumerable<IStatement> statements, IBookingStatementBlock parent, bool appendIfNoCombine = true)
            {
                return appendIfNoCombine;
            }
        }

        /// <summary>
        /// Identical if statements - the second if statement is meaningless, so it
        /// is ok to lift it. I wonder if the normal compiler would do that?
        /// </summary>
        [TestMethod]
        public void TestIdenticalIfsGetLifted()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign2);

            var ifstatement2 = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement2);

            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)), new IDeclaredParameter[] { }, true);
            gc.Add(assign3);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            CommonStatementLifter.Optimize(cc);
            cc.DumpCodeToConsole();

            var block1 = cc.QueryCode().First();
            var firstFilter = block1.Statements.First() as StatementFilter;
            Assert.IsNotNull(firstFilter, "first filter");
            Assert.AreEqual(2, firstFilter.Statements.Count(), "Number of statements inside the filter");
            Assert.IsTrue(firstFilter.Statements.All(s => s is StatementAssign), "All statements inside filter are assign");
        }

        /// <summary>
        /// When two queries are combined, sometimes you'll get two of the same statements at different
        /// depths. They normally won't move past an "if" statement - but if one is already outside the if
        /// statement there is no need to re-calc the one inside the if statement.
        /// </summary>
        [TestMethod]
        public void TestListCommonStatementOverIfWhenAlreadyThere()
        {
            var q = new QueriableDummy<LINQToTTreeLib.QueryVisitorTest.dummyntup>();

            var res1 = from f in q
                       from r1 in f.valC1D
                       let rr1 = Math.Abs(LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r1))
                       select rr1;
            var resu1 = res1.Aggregate(0, (acc, v) => acc + v);
            var query1 = DummyQueryExectuor.FinalResult;
            StatementLifter.Optimize(query1);

            var res2 = from f in q
                       from r1 in f.valC1D
                       from r2 in f.valC1D
                       where r1 > 2
                       let rr1 = Math.Abs(LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r1))
                       let rr2 = Math.Abs(LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r2))
                       select rr1 + rr2;
            var resu2 = res2.Aggregate(0, (acc, v) => acc + v);
            var query2 = DummyQueryExectuor.FinalResult;
            StatementLifter.Optimize(query2);

            // Combine the queries

            var query = CombineQueries(query1, query2);
            Console.WriteLine("Unoptimized");
            query.DumpCodeToConsole();

            CommonStatementLifter.Optimize(query);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("After optimization...");
            Console.WriteLine();
            query.DumpCodeToConsole();

            // We test for this by making sure the "abs" function is called only twice in
            // the generated code.

            Assert.AreEqual(2, query.DumpCode().Where(l => l.Contains("abs")).Count(), "# of times abs appears in the code");
        }

        /// <summary>
        /// Say you have an aggregate statement that is in an inner loop that is the "same" as the outter loop one.
        /// It should not be lifted since it will alter the counting!
        /// </summary>
        [TestMethod]
        public void TestAggregateStatementIndependentOfInnerLoop()
        {
            var q = new QueriableDummy<LINQToTTreeLib.QueryVisitorTest.dummyntup>();

            var res1 = from f in q
                       select
                       (from r1 in f.valC1D
                        let rr1 = Math.Abs(LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r1))
                        select rr1).Aggregate(0, (acc, v) => acc + v);
            var resu1 = res1.Aggregate(0, (acc, v) => acc + v);
            var query1 = DummyQueryExectuor.FinalResult;
            StatementLifter.Optimize(query1);

            var res2 = from f in q
                       from r12 in f.valC1D
                       select (from r1 in f.valC1D
                               let rr1 = Math.Abs(LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r12))
                               select rr1).Aggregate(0, (acc, v) => acc + v);
            var resu2 = res2.Aggregate(0, (acc, v) => acc + v);
            var query2 = DummyQueryExectuor.FinalResult;
            StatementLifter.Optimize(query2);

            // Combine the queries

            var query = CombineQueries(query1, query2);
            Console.WriteLine("Unoptimized");
            query.DumpCodeToConsole();

            CommonStatementLifter.Optimize(query);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("After optimization...");
            Console.WriteLine();
            query.DumpCodeToConsole();

            Assert.AreEqual(2, query.DumpCode().Where(l => l.Contains("for (")).Count(), "# of times for loop appears in the code");
        }

        [TestMethod]
        public void TestIfStatementsFromSkips()
        {
            var q = new QueriableDummy<LINQToTTreeLib.QueryVisitorTest.dummyntup>();

            var res1 = from f in q
                       where f.valC1D.First() > 0
                       select f;
            var resu1 = res1.Count();
            var query1 = DummyQueryExectuor.FinalResult;
            StatementLifter.Optimize(query1);

            var res2 = from f in q
                       where f.valC1D.Skip(1).First() > 0
                       select f;
            var resu2 = res2.Count();
            var query2 = DummyQueryExectuor.FinalResult;
            StatementLifter.Optimize(query2);

            // Combine the queries

            var query = CombineQueries(query1, query2);
            Console.WriteLine("Unoptimized");
            query.DumpCodeToConsole();

            CommonStatementLifter.Optimize(query);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("After optimization...");
            Console.WriteLine();
            query.DumpCodeToConsole();

            // We test for this by making sure the "abs" function is called only twice in
            // the generated code.

            Assert.IsTrue(query.DumpCode().Any(l => l.Contains("aInt32_8++")), "The second if statement was optimized away!");
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
    }
}