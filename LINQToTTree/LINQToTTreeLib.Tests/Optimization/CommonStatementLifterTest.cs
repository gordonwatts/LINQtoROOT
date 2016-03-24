using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Optimization;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTTreeLib.Tests.Optimization
{
    [TestClass]
    [DeploymentItem(@"ConfigData\default.classmethodmappings")]
    public class CommonStatementLifterTest
    {
        /// <summary>
        /// Setup the code
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            TestUtils.ResetLINQLibrary();
        }

        /// <summary>
        /// Make sure MEF is closed out and read.
        /// </summary>
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
            Assert.AreEqual(0, cc.QueryCode().Count());
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
            var assign = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
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
        /// Discovered in the wild: a lift put statements out of order. This happens because one
        /// statement looks like it can be absorbed into another, when buried inside it, there is
        /// some object reference which doesn't work.
        /// </summary>
        [TestMethod]
        public void TestLifingPreservesOrder()
        {
            // We will have two if statements to do the combination with. They basically "hide" the modification.
            var checkVar1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var if1s1 = new StatementFilter(new ValSimple(checkVar1.RawValue, typeof(bool)));
            var if1s2 = new StatementFilter(new ValSimple(checkVar1.RawValue, typeof(bool)));

            var checkVar2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var if2s1 = new StatementFilter(new ValSimple(checkVar2.RawValue, typeof(bool)));
            var if2s2 = new StatementFilter(new ValSimple(checkVar2.RawValue, typeof(bool)));

            var randomVar = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            if1s2.Add(new StatementAssign(randomVar, new ValSimple("1", typeof(int))));
            if2s2.Add(new StatementAssign(randomVar, new ValSimple("1", typeof(int))));
            if1s1.Add(new StatementAssign(randomVar, new ValSimple("1", typeof(int))));
            if2s1.Add(new StatementAssign(randomVar, new ValSimple("1", typeof(int))));

            // We will put the first if's - that we can perhaps combine with - at the top level (though we
            // shouldn't combine with them!).
            var gc = new GeneratedCode();
            gc.Add(checkVar1);
            gc.Add(checkVar2);
            gc.Add(if2s2);
            gc.Pop();
            gc.Add(if1s2);
            gc.Pop();

            // Next, we want an inline block. We will push everything down into it.
            //var blockWithModified = new StatementInlineBlock();
            //gc.Add(blockWithModified);

            //blockWithModified.Add(if1s1);
            //blockWithModified.Add(if2s1);
            gc.Add(if1s1);
            gc.Pop();
            gc.Add(if2s1);
            gc.Pop();

            // Have the modified if statement contain the modification now.

            var varToBeModified = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var statementModifier = new StatementAssign(varToBeModified, new ValSimple("1", typeof(int)));
            //blockWithModified.Add(varToBeModified);
            gc.Add(varToBeModified);
            if1s1.Add(statementModifier);

            // Next, we need to use the variable in the second if statement. Which, since it is like the first, should be pushed back up there.
            var finalVar = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assignment = new StatementAssign(finalVar, varToBeModified);
            //blockWithModified.Add(finalVar);
            gc.Add(finalVar);
            if2s1.Add(assignment);

            // Optimize.

            Console.WriteLine("Before optimization:");
            gc.DumpCodeToConsole();
            Console.WriteLine("");
            CommonStatementLifter.Optimize(gc);
            Console.WriteLine("After optimization:");
            gc.DumpCodeToConsole();

            var firstMention = gc.DumpCode().TakeWhile(l => !l.Contains("aInt32_4=1")).Count();
            var secondMetnion = gc.DumpCode().SkipWhile(l => !l.Contains("aInt32_4=1")).Skip(1).Where(l => l.Contains("aInt32_4")).Count();
            Assert.AreEqual(1, secondMetnion, "Mention of ainte32 4 after the first one");
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
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign1);
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign2);


            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            Console.WriteLine("Before optimization:");
            cc.DumpCodeToConsole();

            CommonStatementLifter.Optimize(cc);

            Console.WriteLine();
            Console.WriteLine("After optimization:");
            cc.DumpCodeToConsole();

            var block1 = cc.QueryCode().First();
            var firstAssignment = block1.Statements.First() as StatementAssign;
            Assert.IsNotNull(firstAssignment, "first assignment");
            var backIfStatement = block1.Statements.Skip(1).First() as StatementFilter;
            Assert.IsNotNull(backIfStatement, "if statement there");
            Assert.AreEqual(0, backIfStatement.Statements.Count(), "# of if statements inside the if");
        }

        /// <summary>
        /// Slightly different statement, but same value, in both places. Lift should occur, and rename
        /// of variables should also happen correctly.
        /// </summary>
        [TestMethod]
        public void TestStatementOutAndInIfWithDifferentTargets()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));

            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign1);

            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);

            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign2 = new StatementAssign(p2, new ValSimple("f", typeof(int)), true);
            gc.Add(assign2);

            var p3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p3, p2, true);
            gc.Add(assign3);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            Console.WriteLine("Before optimization:");
            cc.DumpCodeToConsole();

            CommonStatementLifter.Optimize(cc);

            Console.WriteLine();
            Console.WriteLine("After optimization:");
            cc.DumpCodeToConsole();

            var block1 = cc.QueryCode().First();
            var firstAssignment = block1.Statements.First() as StatementAssign;
            Assert.IsNotNull(firstAssignment, "first assignment");
            var backIfStatement = block1.Statements.Skip(1).First() as StatementFilter;
            Assert.IsNotNull(backIfStatement, "if statement there");
            Assert.AreEqual(1, backIfStatement.Statements.Count(), "# of if statements inside the if");

            var redoneAssign = backIfStatement.Statements.First() as StatementAssign;
            Assert.IsNotNull(redoneAssign, "the if block statement isn't an assign");
            Assert.AreEqual(p3.ParameterName, redoneAssign.ResultVariable.ParameterName);
            Assert.AreEqual(p1.ParameterName, redoneAssign.Expression.ToString());
        }

        /// <summary>
        /// Slightly different statement, but same value, in both places. Lift should occur, and rename
        /// of variables should also happen correctly.
        /// </summary>
        /// <remarks>This was found in the wild, because of a funny way that the "?" statement was coded up.</remarks>
        [TestMethod]
        public void TestStatementOutAndInIfWithSeperateDecl()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));

            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(p1);
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)), false);
            gc.Add(assign1);

            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);

            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(p2);
            var assign2 = new StatementAssign(p2, new ValSimple("f", typeof(int)), false);
            gc.Add(assign2);

            var p3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p3, p2, true);
            gc.Add(assign3);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            Console.WriteLine("Before optimization:");
            cc.DumpCodeToConsole();

            CommonStatementLifter.Optimize(cc);

            Console.WriteLine();
            Console.WriteLine("After optimization:");
            cc.DumpCodeToConsole();

            var block1 = cc.QueryCode().First();
            var firstAssignment = block1.Statements.First() as StatementAssign;
            Assert.IsNotNull(firstAssignment, "first assignment");
            var backIfStatement = block1.Statements.Skip(1).First() as StatementFilter;
            Assert.IsNotNull(backIfStatement, "if statement there");
            Assert.AreEqual(0, backIfStatement.DeclaredVariables.Count(), "lifted variables should no longer be declared here");
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
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign1);
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign11 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)), true);
            gc.Add(assign11);
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            var assign22 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)), true);
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
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign1);
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign2);
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)), true);
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
        /// Identical nested loops should not be lifted! That would be very bad. If they are lifted, then
        /// you will miss a 2D run on something!
        /// </summary>
        [TestMethod]
        public void TestIdenticalNestedLoopsStayPut()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));

            gc.Add(new DummyLoop());

            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign1);

            gc.Add(new DummyLoop());

            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign2 = new StatementAssign(p2, new ValSimple("f", typeof(int)), true);
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
            Assert.AreEqual(1, secondLoop.Statements.Count(), "# of statements in second loop");
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
                statement.Parent = this;
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
                yield return "do {";
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
            public bool Combine(IEnumerable<IStatement> statements, IBookingStatementBlock parent, bool appendIfNoCombine = true, bool moveIfIdentical = false)
            {
                return appendIfNoCombine;
            }


            public IStatement CombineAndMark(IStatement statement, IBookingStatementBlock parent, bool appendIfNoCombine = true)
            {
                foreach (var s in _statements)
                {
                    if (s.ToString() == statement.ToString())
                        return s;
                }
                return null;
            }


            public bool IsBefore(IStatement first, IStatement second)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IDeclaredParameter> LoopIndexVariable
            {
                get { throw new NotImplementedException(); }
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
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign2);

            var ifstatement2 = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement2);

            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)), true);
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
        /// If we lift a statement from inside, make sure it appears in the right "order" - that is, before
        /// the place it is lifted from.
        /// </summary>
        [TestMethod]
        public void TestLiftedStatmentInRightOrder()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign2);
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)), true);
            gc.Add(assign3);
            gc.Pop();
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign1);

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
        /// If we lift the same code from two statements, make sure the code appears at the proper spot (i.e.
        /// before the first statement, not the second one).
        /// </summary>
        [TestMethod]
        public void TestLifted2ndStatmentInRightOrder()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var ifstatement1 = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement1);
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign2);
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)), true);
            gc.Add(assign3);
            gc.Pop();

            var ifstatement2 = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement2);
            var assign4 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign4);
            var p3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign5 = new StatementAssign(p3, new ValSimple("f*5", typeof(int)), true);
            gc.Add(assign5);
            gc.Pop();

            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign1);

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
        /// Make sure the detection code can work properly with a lifted statement that is actually two
        /// if statements deep.
        /// </summary>
        [TestMethod]
        public void TestLift2DeepStatement()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));

            // Add one common assign statement.
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign2);

            // Go down two levels.
            var ifstatement1 = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement1);
            var ifstatement2 = new StatementFilter(new ValSimple("j", typeof(int)));
            gc.Add(ifstatement2);

            // Two levels down add the second common and unique statements.
            var assign4 = new StatementAssign(p1, new ValSimple("f", typeof(int)), true);
            gc.Add(assign4);
            var p3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign5 = new StatementAssign(p3, new ValSimple("f*5", typeof(int)), true);
            gc.Add(assign5);


            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            cc.DumpCodeToConsole();
            Console.WriteLine("Optimized:");
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
        /// When two queries are combined, sometimes you'll get two of the same statements at different
        /// depths. They normally won't move past an "if" statement - but if one is already outside the if
        /// statement there is no need to re-calc the one inside the if statement.
        /// </summary>
        [TestMethod]
        public void TestListCommonStatementOverIfWhenAlreadyThere()
        {
            var q = new QueriableDummy<dummyntup>();

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

            Assert.IsTrue(query1.CheckCodeBlock(), "query 1 is good format");
            Assert.IsTrue(query2.CheckCodeBlock(), "query 2 is good format");

            Console.WriteLine("Query #1:");
            query1.DumpCodeToConsole();
            Console.WriteLine("Query #2:");
            query2.DumpCodeToConsole();

            var query = CombineQueries(query1, query2);
            Console.WriteLine("Unoptimized");
            query.DumpCodeToConsole();
            Assert.IsTrue(query.CheckCodeBlock(), "combined query ok");

            CommonStatementLifter.Optimize(query);
            Assert.IsTrue(query.CheckCodeBlock(), "optimzied combined query ok");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("After optimization...");
            Console.WriteLine();
            query.DumpCodeToConsole();

            // We test for this by making sure the "abs" function is called only twice in
            // the generated code.

            Assert.AreEqual(1, query.DumpCode().Where(l => l.Contains("abs")).Count(), "# of times abs appears in the code");
        }

        /// <summary>
        /// 1. Loop 1 over array a
        /// 2. Loop 2 over array a
        /// 3.  something with iterator from Loop 1 and Loop 2.
        /// 
        /// You can't necessarily pull things out when they are nested identical loops - they may well be
        /// there for a reason!
        /// </summary>
        [TestMethod]
        public void TestNoCommonLifeNestedIdenticalLoops()
        {
            var v = new GeneratedCode();

            var limit = new LINQToTTreeLib.Variables.ValSimple("5", typeof(int));
            var loopP1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop1 = new StatementForLoop(loopP1, limit);
            v.Add(loop1);
            var loopP2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop2 = new StatementForLoop(loopP2, limit);
            v.Add(loop2);
            v.Add(new StatementAssign(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)), new ValSimple("10", typeof(int))));

            Console.WriteLine("Unoptimized:");
            v.DumpCodeToConsole();
            CommonStatementLifter.Optimize(v);
            Console.WriteLine("");
            Console.WriteLine("Optimized:");
            v.DumpCodeToConsole();

            // Make sure it is two if statements, nested.
            var if1 = v.CodeBody.Statements.First() as StatementForLoop;
            Assert.IsNotNull(if1, "if #1");
            var if2 = if1.Statements.First() as StatementForLoop;
            Assert.IsNotNull(if2, "if #2");

            // Make sure the two loop variables are different.
            Assert.AreNotEqual(if1.LoopIndexVariable.First(), if2.LoopIndexVariable.First(), "Loop index vars");
        }

        /// <summary>
        /// 1. Loop 1 over array a
        /// 2. Loop 2 over array a
        /// 3.  if statement
        /// 4.    something with iterator from Loop 1 and Loop 2.
        /// 
        /// You can't necessarily pull things out when they are nested identical loops - they may well be
        /// there for a reason!
        /// </summary>
        [TestMethod]
        public void TestNoCommonLifeNestedIdenticalLoopsIf()
        {
            var v = new GeneratedCode();

            var limit = new LINQToTTreeLib.Variables.ValSimple("5", typeof(int));
            var loopP1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop1 = new StatementForLoop(loopP1, limit);
            v.Add(loop1);
            var loopP2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop2 = new StatementForLoop(loopP2, limit);
            v.Add(loop2);
            v.Add(new StatementFilter(new ValSimple(string.Format("{0}!=0", loopP1.RawValue), typeof(bool))));
            v.Add(new StatementAssign(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)), new ValSimple("10", typeof(int))));

            Console.WriteLine("Unoptimized:");
            v.DumpCodeToConsole();
            CommonStatementLifter.Optimize(v);
            Console.WriteLine("");
            Console.WriteLine("Optimized:");
            v.DumpCodeToConsole();

            // Make sure it is two if statements, nested.
            var if1 = v.CodeBody.Statements.First() as StatementForLoop;
            Assert.IsNotNull(if1, "if #1");
            var if2 = if1.Statements.First() as StatementForLoop;
            Assert.IsNotNull(if2, "if #2");

            // Make sure the two loop variables are different.
            Assert.AreNotEqual(if1.LoopIndexVariable.First(), if2.LoopIndexVariable.First(), "Loop index vars");
        }

        /// <summary>
        /// 1. Loop 1 over array a
        /// 2. Loop 2 over array a
        /// 3.  if statement
        /// 4.    something with iterator from Loop 1 and Loop 2.
        /// 5. More statements.
        /// 
        /// You can't necessarily pull things out when they are nested identical loops - they may well be
        /// there for a reason!
        /// </summary>
        [TestMethod]
        public void TestNoCommonLifeNestedIdenticalLoopsIfStatementAfter()
        {
            var v = new GeneratedCode();

            var limit = new LINQToTTreeLib.Variables.ValSimple("5", typeof(int));
            var loopP1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop1 = new StatementForLoop(loopP1, limit);
            v.Add(loop1);
            var lp1Scope = v.CurrentScope;
            var loopP2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop2 = new StatementForLoop(loopP2, limit);
            v.Add(loop2);
            v.Add(new StatementFilter(new ValSimple(string.Format("{0}!=0", loopP1.RawValue), typeof(bool))));
            v.Add(new StatementAssign(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)), new ValSimple("10", typeof(int))));

            v.CurrentScope = lp1Scope;
            v.Add(new StatementAssign(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)), new ValSimple("10", typeof(int))));

            Console.WriteLine("Unoptimized:");
            v.DumpCodeToConsole();
            CommonStatementLifter.Optimize(v);
            Console.WriteLine("");
            Console.WriteLine("Optimized:");
            v.DumpCodeToConsole();

            // Make sure it is two if statements, nested.
            var if1 = v.CodeBody.Statements.First() as StatementForLoop;
            Assert.IsNotNull(if1, "if #1");
            var if2 = if1.Statements.First() as StatementForLoop;
            Assert.IsNotNull(if2, "if #2");

            // Make sure the two loop variables are different.
            Assert.AreNotEqual(if1.LoopIndexVariable.First(), if2.LoopIndexVariable.First(), "Loop index vars");
        }

        /// <summary>
        /// Say you have an aggregate statement that is in an inner loop that is the "same" as the outer loop one.
        /// It should not be lifted since it will alter the counting!
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestAggregateStatementIndependentOfInnerLoop()
        {
            var q = new QueriableDummy<dummyntup>();

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

            // We can't totally combine because some gets extract into a function.
            Assert.AreEqual(3, query.DumpCode().Where(l => l.Contains("for (")).Count(), "# of times for loop appears in the code");
            // TODO: in the inner loop i see the following:
            //aInt32_12 = aInt32_12 + aInt32_15;
            //aInt32_15 = aInt32_15 + aInt32_17;
            // note how 15 is used and then updated. Is that really right? This seems like a funny combination here.
        }

        /// <summary>
        /// Say you have an aggregate statement that is in an inner loop that is the "same" as the outer loop one.
        /// It should not be lifted since it will alter the counting!
        /// </summary>
        [TestMethod]
        public void AggregateLiftDoubleInnerLoop()
        {
            var q = new QueriableDummy<dummyntup>();

            var res2 = from f in q
                       from r12 in f.valC1D
                       select (from r1 in f.valC1D
                               let rr1 = Math.Abs(LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r12))
                               select rr1).Aggregate(0, (acc, v) => acc + v);
            var resu2 = res2.Aggregate(0, (acc, v) => acc + v);
            var query2 = DummyQueryExectuor.FinalResult;

            Console.WriteLine("Unoptimized");
            query2.DumpCodeToConsole();

            StatementLifter.Optimize(query2);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("After optimization...");
            Console.WriteLine();
            query2.DumpCodeToConsole();

            // We can't totally combine because some gets extract into a function.
            Assert.AreEqual(2, query2.DumpCode().Where(l => l.Contains("for (")).Count(), "# of times for loop appears in the code");
        }

        /// <summary>
        /// Say you have an aggregate statement that is in an inner loop that is the "same" as the outter loop one.
        /// It should not be lifted since it will alter the counting!
        /// </summary>
        [TestMethod]
        public void AggregateStatementLiftNonSideEffect()
        {
            var q = new QueriableDummy<dummyntup>();

            var res1 = from f in q
                       select
                       (from r1 in f.valC1D
                        let rr1 = Math.Abs(LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(r1))
                        select rr1).Aggregate(0, (acc, v) => acc + v);
            var resu1 = res1.Aggregate(0, (acc, v) => acc + v);
            var query1 = DummyQueryExectuor.FinalResult;

            Console.WriteLine("Unoptimized");
            Console.WriteLine();
            query1.DumpCodeToConsole();

            StatementLifter.Optimize(query1);

            Console.WriteLine();
            Console.WriteLine("Optimized");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            query1.DumpCodeToConsole();

            Assert.AreEqual(1, query1.DumpCode().Where(l => l.Contains("for (")).Count(), "# of times for loop appears in the code");
        }

        [TestMethod]
        public void TestIfStatementsFromSkips()
        {
            var q = new QueriableDummy<dummyntup>();

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

            Assert.IsTrue(query.DumpCode().Any(l => l.Contains("aInt32_14++")), "The second if statement was optimized away!");
        }

        /// <summary>
        /// Found in the wild, when combining two statements that have a lift of a CPPCode, the CPPcode ends up
        /// above the declaration in some very funny and odd way. This test captures that error and makes sure it
        /// never shows up again.
        /// </summary>
        [TestMethod]
        public void TestCombinedLoopLiftReordering()
        {
            var q = new QueriableDummy<dummyntup>();

            var r1 = from f in q
                     let l1 = f.valC1D.Where(v => LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(v) > 1).OrderByDescending(v => LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(v))
                     select new
                     {
                         jets = l1,
                         truthjets = f.valC1D
                     };

            var r2 = from f in r1
                     select new
                     {
                         machedJets = (from j in f.jets
                                       select new
                                       {
                                           J = j,
                                           MJ = (from mj in f.truthjets
                                                 let imj = LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(mj)
                                                 let ij = LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(j)
                                                 orderby imj - ij ascending
                                                 select mj).First()
                                       })
                     };

            var res1 = (from f in r2
                        from j in f.machedJets
                        where j.MJ == 5
                        select LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(j.J) * 2).Sum();
            var query1 = DummyQueryExectuor.FinalResult;
            StatementLifter.Optimize(query1);

            var res2 = (from f in r2
                        from j in f.machedJets
                        select LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.Calc(j.J)).Sum();
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
            var lines = query.DumpCode().ToArray();
            lines.DumpToConsole();

            // Find the first mention of aInt32_28. It should be declared.

            var varname = lines.FindVariableIn("int $$=-1;");
            var firstMention = query.DumpCode().Where(l => l.Contains(varname)).Skip(0).First();
            Assert.AreEqual(string.Format("int {0}=-1;", varname), firstMention.Trim(), "aint32_23 decl");
        }

        /// <summary>
        /// Seen in the wild. A lift leaves behind multiple assignment statements that look identical.
        /// </summary>
        [TestMethod]
        public void LiftGeneratesDuplicateStatements()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));

            var toplevel = gc.CurrentScope;

            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);

            AddConditionalExpr(gc);

            gc.Pop();
            AddConditionalExpr(gc);
            
            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            Console.WriteLine("Before optimization:");
            cc.DumpCodeToConsole();

            CommonStatementLifter.Optimize(cc);

            Console.WriteLine();
            Console.WriteLine("After optimization:");
            cc.DumpCodeToConsole();

            var block1 = cc.QueryCode().First().Statements.Skip(2).FirstOrDefault();
            Assert.IsInstanceOfType(block1, typeof(StatementFilter));
            var filter = block1 as StatementFilter;
            Assert.AreEqual(1, filter.Statements.Count());

            var ifStatementI = cc.QueryCode().First().Statements.Where(s => s is StatementFilter && ((s as StatementFilter).TestExpression.RawValue == "i")).Cast<StatementFilter>().FirstOrDefault();
            Assert.IsNotNull(ifStatementI);
            Assert.AreEqual(0, ifStatementI.Statements.Count());
        }

        [TestMethod]
        public void DuplicateIfStatementWithExtraInnerLineAtEnd()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));

            var toplevel = gc.CurrentScope;

            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);

            AddConditionalExpr(gc);
            var p1Extra = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            gc.Add(new StatementAssign(p1Extra, new ValSimple("fExtra", typeof(double)), true));

            gc.Pop();
            AddConditionalExpr(gc);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            Console.WriteLine("Before optimization:");
            cc.DumpCodeToConsole();

            CommonStatementLifter.Optimize(cc);

            Console.WriteLine();
            Console.WriteLine("After optimization:");
            cc.DumpCodeToConsole();

            var block1 = cc.QueryCode().First().Statements.Skip(2).FirstOrDefault();
            Assert.IsInstanceOfType(block1, typeof(StatementFilter));
            var filter = block1 as StatementFilter;
            Assert.AreEqual(1, filter.Statements.Count());

            var ifStatementI = cc.QueryCode().First().Statements.Where(s => s is StatementFilter && ((s as StatementFilter).TestExpression.RawValue == "i")).Cast<StatementFilter>().FirstOrDefault();
            Assert.IsNotNull(ifStatementI);
            Assert.AreEqual(1, ifStatementI.Statements.Count());
        }

        [TestMethod]
        public void DuplicateIfStatementWithExtraInnerLineIfDeepIf()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));

            var toplevel = gc.CurrentScope;

            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);

            var p1Extra = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var toadd = new StatementAssign(p1Extra, new ValSimple("fExtra", typeof(double)), true);
            AddSingleIfExpr(gc, addIn: toadd);

            gc.Pop();

            AddSingleIfExpr(gc);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            Console.WriteLine("Before optimization:");
            cc.DumpCodeToConsole();

            CommonStatementLifter.Optimize(cc);

            Console.WriteLine();
            Console.WriteLine("After optimization:");
            cc.DumpCodeToConsole();

            var block1 = cc.QueryCode().First().Statements.Skip(2).FirstOrDefault();
            Assert.IsInstanceOfType(block1, typeof(StatementFilter));
            var filter = block1 as StatementFilter;
            Assert.AreEqual(1, filter.Statements.Count());

            var ifStatementI = cc.QueryCode().First().Statements.Where(s => s is StatementFilter && ((s as StatementFilter).TestExpression.RawValue == "i")).Cast<StatementFilter>().FirstOrDefault();
            Assert.IsNotNull(ifStatementI);
            Assert.AreEqual(1, ifStatementI.Statements.Count());
        }

        /// <summary>
        /// Helper function to add a conditional statement.
        /// </summary>
        /// <param name="gc"></param>
        private void AddConditionalExpr(GeneratedCode gc, IStatement addInFirst = null, IStatement addInSecond = null)
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            gc.Add(p1);

            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var assignp2 = new StatementAssign(p2, new ValSimple("f", typeof(bool)), true);
            gc.Add(assignp2);
            var ifstatement = new StatementFilter(p2);
            gc.Add(ifstatement);
            if (addInFirst != null)
                gc.Add(addInFirst);
            var assign3 = new StatementAssign(p1, new ValSimple("f1", typeof(double)), false);
            gc.Add(assign3);
            gc.Pop();

            gc.Add(new StatementFilter(new ValSimple("!" + p2.ParameterName, typeof(bool))));
            gc.Add(new StatementAssign(p1, new ValSimple("f2", typeof(double)), false));
            if (addInSecond != null)
            {
                gc.Add(addInSecond);
            }
            gc.Pop();
        }

        /// <summary>
        /// Helper function to add a conditional statement.
        /// </summary>
        /// <param name="gc"></param>
        private void AddSingleIfExpr(GeneratedCode gc, IStatement addIn = null)
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            gc.Add(p1);
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var assignp2 = new StatementAssign(p2, new ValSimple("f", typeof(bool)), true);
            gc.Add(assignp2);
            var ifstatement = new StatementFilter(p2);
            gc.Add(ifstatement);
            if (addIn != null)
                gc.Add(addIn);
            var assign3 = new StatementAssign(p1, new ValSimple("f1", typeof(double)), false);
            gc.Add(assign3);
            gc.Pop();
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
