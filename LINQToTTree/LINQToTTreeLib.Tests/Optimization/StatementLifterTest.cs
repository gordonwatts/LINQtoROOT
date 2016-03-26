using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Optimization;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static LINQToTTreeLib.Tests.TestUtils;

namespace LINQToTTreeLib.Tests.Optimization
{
    [TestClass]
    [DeploymentItem(@"ConfigData\default.classmethodmappings")]
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
        public void TestLiftSimpleStatementInFunction()
        {
            var v = new StatementInlineBlock();
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);
            loop.Add(new StatementWithNoSideEffects());

            var f = QMFunctions.QMFuncUtils.GenerateFunction();
            f.SetCodeBody(v);

            var gc = new GeneratedCode();
            gc.Add(new StatementSimpleStatement("int i = 10;"));
            gc.Add(f);
            StatementLifter.Optimize(gc);

            Assert.AreEqual(1, gc.Functions.Count(), "# of functions after lifting");
            var firstStatement = gc.Functions.First().StatementBlock.Statements.First();
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

            Console.WriteLine("Before optimization");
            v.DumpCodeToConsole();

            StatementLifter.Optimize(v);
            Console.WriteLine("After optimization");
            v.DumpCodeToConsole();

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementWithNoSideEffects), "first statement");
            var thirdstatement = v.CodeBody.Statements.Skip(1).First();
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

        /// <summary>
        /// Distilled from something we found in the wild.
        /// 1. Non ICM statement (like an if statement)
        /// 2. Loop
        /// 3.   Statement that is independent of loop 1
        /// 
        /// It is OK to lift statement 3 to be above Loop, but we can't
        /// move it past #1.
        /// </summary>
        [TestMethod]
        public void TestLiftNestedInLoopStatementWithBlockAtTopLevel()
        {
            var v = new GeneratedCode();

            v.Add(new StatementNonOptimizing());

            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);
            v.Add(new StatementWithNoSideEffects());

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementNonOptimizing), "first statement");
            var secondStatement = v.CodeBody.Statements.Skip(1).First();
            Assert.IsInstanceOfType(secondStatement, typeof(StatementWithNoSideEffects), "Second statement");
        }

        /// <summary>
        /// 1. Loop 1 over array a
        /// 2.   Loop 2 over array a
        /// 3.     something with iterator from Loop 1 and Loop 2.
        /// 
        /// Since the statement explicitly has no side effects, it should be
        /// popped up to the top.
        /// </summary>
        [TestMethod]
        public void LiftNoSideEffectFromNestedIdenticalLoops()
        {
            var v = new GeneratedCode();

            var limit = new LINQToTTreeLib.Variables.ValSimple("10", typeof(int));
            var loopP1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop1 = new StatementForLoop(loopP1, limit);
            v.Add(loop1);
            var loopP2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop2 = new StatementForLoop(loopP2, limit);
            v.Add(loop2);
            v.Add(new StatementWithNoSideEffects());

            Console.WriteLine("Unoptimized:");
            v.DumpCodeToConsole();
            StatementLifter.Optimize(v);
            Console.WriteLine("");
            Console.WriteLine("Optimized:");
            v.DumpCodeToConsole();

            // Check to see if it got lifted.
            Assert.AreEqual(1, v.CodeBody.Statements.WhereCast<IStatement, StatementWithNoSideEffects>().Count(), "#of no side effect statements");
        }

        /// <summary>
        /// Distilled from something we found in the wild.
        /// 1. Statement
        /// 2. ICM Loop/compound statement (like any inline block, I guess)
        /// 3.   Loop
        /// 4.     Statement with no side effects
        /// 
        /// 4 can get lifted past 2, but not across #1 if we don't know about it.
        /// </summary>
        [TestMethod]
        public void TestLoopBelowNonLoopBlockNoSideEffects()
        {
            var v = new GeneratedCode();

            v.Add(new StatementNonOptimizing());
            v.Add(new StatementInlineBlock());
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);

            v.Add(new StatementWithNoSideEffects());

            DoOptimizeTest(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementNonOptimizing), "first statement");
            var secondStatement = v.CodeBody.Statements.Skip(1).First();
            Assert.IsInstanceOfType(secondStatement, typeof(StatementWithNoSideEffects), "Second statement");
        }

        private static void DoOptimizeTest(GeneratedCode v)
        {
            Console.WriteLine("Before Optimization:");
            Console.WriteLine("");
            v.DumpCodeToConsole();

            StatementLifter.Optimize(v);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("After Optimization:");
            Console.WriteLine("");
            v.DumpCodeToConsole();
        }

        /// <summary>
        /// Distilled from something we found in the wild.
        /// 1. Statement
        /// 2. Non ICM Loop/compound statement (like any inline block, I guess)
        /// 3.   Loop
        /// 4.     Idempotent Statement with no side effects
        /// 
        /// 4 can get lifted past 2, but not above...
        /// </summary>
        [TestMethod]
        public void TestLoopBelowNonLoopBlockSideEffectsNotImportant()
        {
            var v = new GeneratedCode();

            v.Add(new StatementNonOptimizing());
            v.Add(new StatementInlineBlock());
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);

            var lnp = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            v.Add(new StatementWithSideEffects(lnp));

            DoOptimizeTest(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementNonOptimizing), "first statement");
            var secondStatement = v.CodeBody.Statements.Skip(1).First();
            Assert.IsInstanceOfType(secondStatement, typeof(StatementWithSideEffects), "Second statement");
        }

        /// <summary>
        /// Distilled from something we found in the wild.
        /// 1. Statement
        /// 2. Sorted Loop statement (like any inline block, I guess)
        /// 3.   Loop
        /// 4.     Statement with no side effects
        /// 
        /// 3 can get lifted past 2, but not above...
        /// </summary>
        [TestMethod]
        public void TestLoopBelowSortedLoopBlockSideEffectsNotImportant()
        {
            var v = new GeneratedCode();

            v.Add(new StatementNonOptimizing());
            var dict = DeclarableParameter.CreateDeclarableParameterMapExpression(typeof(int), typeof(int[]));
            v.Add(new StatementLoopOverSortedPairValue(dict, true));
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);

            var lnp = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            v.Add(new StatementWithSideEffects(lnp));

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementNonOptimizing), "first statement");
            var secondStatement = v.CodeBody.Statements.Skip(1).First();
            Assert.IsInstanceOfType(secondStatement, typeof(StatementLoopOverSortedPairValue), "Second statement");
        }

        /// <summary>
        /// Make sure no lifting occurs here:
        /// 1. no side effect statement
        /// 2. non ICM statement that is a compound statement
        /// 3.   no side effect statement
        /// Nothing should be lifted in this case.
        /// </summary>
        [TestMethod]
        public void TestNoLiftPastNoMoveCompound()
        {
            var v = new GeneratedCode();

            v.Add(new StatementWithNoSideEffects());

            v.Add(new DummyStatementCompoundNoCMInfo());
            v.Add(new StatementWithNoSideEffects());

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementWithNoSideEffects), "first statement");
            var secondStatement = v.CodeBody.Statements.Skip(1).First();
            Assert.IsInstanceOfType(secondStatement, typeof(DummyStatementCompoundNoCMInfo), "Second statement");
        }

        /// <summary>
        /// loop A
        /// Inside A is a simple counter statement. Make sure it doesn't get lifted.
        /// </summary>
        [TestMethod]
        public void NoLiftSimpleStatement()
        {
            var gc = new GeneratedCode();
            AddLoop(gc);

            Console.WriteLine("Before optimization");
            gc.DumpCodeToConsole();

            StatementLifter.Optimize(gc);

            Console.WriteLine("After optimization");
            gc.DumpCodeToConsole();

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "# of statements");
            Assert.AreEqual(1, gc.CodeBody.Statements.Where(s => s is StatementForLoop).Count(), "# of for loops");
        }

        /// <summary>
        /// loop A
        /// Inside A is a very simple constant statement. Make sure it does get lifted.
        /// </summary>
        [TestMethod]
        public void LiftConstantStatementInLoop()
        {
            var gc = new GeneratedCode();
            var counter = AddLoop(gc, mainStatementType: MainStatementType.IsConstant);
            gc.Pop();

            Console.WriteLine("Before optimization");
            gc.DumpCodeToConsole();

            StatementLifter.Optimize(gc);

            Console.WriteLine("After optimization");
            gc.DumpCodeToConsole();

            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements");
            Assert.AreEqual(1, gc.CodeBody.Statements.Where(s => s is StatementAssign).Count(), "# of assign statements");
            Assert.AreEqual(1, gc.CodeBody.Statements.Where(s => s is StatementForLoop).Cast<StatementForLoop>().Where(s => s.Statements.Count() == 0).Count(), "# of if statements with zero statements in it");
        }

        /// <summary>
        /// loop A
        /// Inside A is a very simple constant statement. Make sure it does get lifted.
        /// </summary>
        [TestMethod]
        public void LiftConstantDefinedInLoop()
        {
            var gc = new GeneratedCode();
            var counter = AddLoop(gc, mainStatementType: MainStatementType.IsConstant, defineCounterInsideBlock: true);

            Console.WriteLine("Before optimization");
            gc.DumpCodeToConsole();

            StatementLifter.Optimize(gc);

            Console.WriteLine("After optimization");
            gc.DumpCodeToConsole();

            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements");
            Assert.AreEqual(1, gc.CodeBody.Statements.WhereCast<IStatement,StatementAssign>().Count(), "# of assign statements");
            Assert.AreEqual(1, gc.CodeBody.Statements.WhereCast<IStatement, StatementForLoop>().Where(s => s.Statements.Count() == 0).Count(), "# of if statements with zero statements in it");
            Assert.AreEqual(1, gc.CodeBody.DeclaredVariables.Count(), "# of declarations");
        }

        /// <summary>
        /// If loops aren't identical we can't really do the lift, even if it is close.
        /// 1. loop A
        /// 2. if statement
        /// 3.   loop A'
        /// 4.   statement
        /// Here A' contains the statements in A plus an extra statement.
        /// Eventually, perhaps we can remove the common portion in A as long as it isn't touched in A'. But not for now.
        /// </summary>
        [TestMethod]
        public void NoLiftAlmostIdenticalLoopOutOfIfStatement()
        {
            var gc = new GeneratedCode();
            var c1 = AddLoop(gc);
            gc.Pop();
            AddIf(gc);
            var c2 = AddLoop(gc, addDependentStatement: true);
            gc.Pop();
            AddSum(gc, c1, c2);

            Console.WriteLine("Before lifting and optimization: ");
            gc.DumpCodeToConsole();

            StatementLifter.Optimize(gc);

            Console.WriteLine("After lifting and optimization: ");
            gc.DumpCodeToConsole();

            // Now check that things happened as we would expect them to happen.
            Assert.AreEqual(1, gc.CodeBody.Statements.Where(s => s is StatementForLoop).Count(), "# of for loops at outer level");
            Assert.AreEqual(1, gc.CodeBody.Statements.Where(s => s is StatementForLoop).Cast<StatementForLoop>().Where(s => s.Statements.Count() == 1).Count(), "# of statements inside first for loop");

            var ifStatement = gc.CodeBody.Statements.Where(s => s is StatementFilter).Cast<StatementFilter>().First();
            Assert.IsNotNull(ifStatement, "Finding if statement");
            Assert.AreEqual(2, ifStatement.Statements.Count(), "# of statements inside the if statement");
        }

        /// <summary>
        /// A loop repeated can be combined
        /// 1. loop A
        /// 2. loop A
        /// 3. statement
        /// Second loop A can be removed.
        /// </summary>
        [TestMethod]
        public void CombineRepeatedLoops()
        {
            var gc = new GeneratedCode();
            var c1 = AddLoop(gc);
            gc.Pop();
            var c2 = AddLoop(gc);
            gc.Pop();
            AddSum(gc, c1, c2);

            Console.WriteLine("Before lifting and optimization: ");
            gc.DumpCodeToConsole();

            StatementLifter.Optimize(gc);

            Console.WriteLine("After lifting and optimization: ");
            gc.DumpCodeToConsole();

            // Now check that things happened as we would expect them to happen.
            var ass = gc.CodeBody.Statements.Where(s => s is StatementAssign).Cast<StatementAssign>().First();
            Assert.IsNotNull(ass, "Finding the assignment statement");
            Assert.AreEqual($"{c1.RawValue}+{c1.RawValue}", ass.Expression.RawValue);
            Assert.AreEqual(1, gc.CodeBody.DeclaredVariables.Where(dv => dv.RawValue == c1.RawValue).Count(), "# of declared counters");
            Assert.AreEqual(1, gc.CodeBody.Statements.Where(s => s is StatementForLoop).Count(), "# of for loops");
            Assert.AreEqual(1, gc.CodeBody.Statements.WhereCast<IStatement, StatementForLoop>().Where(s => s.Statements.Count() == 1).Count(), "# of it statements with 2 sub-statements");
        }

        /// <summary>
        /// A loop that loops over the same object, but it uses the results of the first loop in the second loop
        /// should not allow for a combining.
        /// </summary>
        [TestMethod]
        public void DontCombineDependentLoops()
        {
            var gc = new GeneratedCode();
            var c1 = AddLoop(gc);
            gc.Pop();
            var c2 = AddLoop(gc, useCounter: c1);
            gc.Pop();

            Console.WriteLine("Before lifting");
            gc.DumpCodeToConsole();

            StatementLifter.Optimize(gc);

            Console.WriteLine("After lifting");
            gc.DumpCodeToConsole();

            Assert.AreEqual(2, gc.CodeBody.Statements.Where(s => s is StatementForLoop).Count(), "# of for loops");
        }

        [TestMethod]
        public void LiftConstantInPairWise()
        {
            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (from cmb in evt.myvectorofint.PairWiseAll((i1, i2) => QueryVisitorTest.CPPHelperFunctions.Calc(i1) != QueryVisitorTest.CPPHelperFunctions.Calc(i2)) select cmb).Count() == 10
                        select evt;
            var dude = dudeQ.Count();

            var gc = DummyQueryExectuor.FinalResult;
            Console.WriteLine("Unoptimized");
            gc.DumpCodeToConsole();

            StatementLifter.Optimize(gc);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Optimized");
            Console.WriteLine("");

            gc.DumpCodeToConsole();

            var index1Ref = gc.DumpCode().Where(l => l.Contains("[index1]") && l.Contains("int")).Select(l => l.Substring(0, l.IndexOf("=")).Trim()).ToArray();
            var index2Ref = gc.DumpCode().Where(l => l.Contains("[index2]") && l.Contains("int")).Select(l => l.Substring(0, l.IndexOf("=")).Trim()).ToArray();

            Assert.AreNotEqual(index1Ref[0], index2Ref[0]);
        }

        /// <summary>
        /// for c1
        ///  int counter;
        ///  for c2
        ///    value that depends on c1
        ///    sum of value that depends on c1 but not c2
        ///  sum of sum of value that depends on c1 but not c2
        ///  
        /// The inner statement is lifted to the outer statement and left to hang.
        /// </summary>
        [TestMethod]
        public void DontLiftThroughTwoForStatements()
        {
            var gc = new GeneratedCode();

            var counter = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            counter.InitialValue = new ValSimple("0", typeof(int));
            gc.Add(counter);

            // The two for loops
            var fc1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var for1 = new StatementForLoop(fc1, new ValSimple("5", typeof(int)));
            gc.Add(for1);

            var innerCounter = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            innerCounter.InitialValue = new ValSimple("0", typeof(int));
            gc.Add(innerCounter);

            var fc2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var for2 = new StatementForLoop(fc2, new ValSimple("5", typeof(int)));
            gc.Add(for2);

            // Now, calculation based only on fc1
            var a1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var ass1 = new StatementAssign(a1, new ValSimple($"{fc1}*2", typeof(int), new IDeclaredParameter[] { fc1 }));
            gc.Add(ass1);
            var agg1 = new StatementAggregate(innerCounter, new ValSimple($"{innerCounter.RawValue}+{a1.RawValue}", typeof(int), new IDeclaredParameter[] { innerCounter, a1 }));
            gc.Add(agg1);

            // and the outer sum.
            gc.Pop();
            var agg2 = new StatementAggregate(counter, new ValSimple($"{counter.RawValue}+{innerCounter.RawValue}", typeof(int), new IDeclaredParameter[] { counter, innerCounter }));
            gc.Add(agg2);

            // Great!
            Console.WriteLine("Unoptimized");
            gc.DumpCodeToConsole();

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");

            StatementLifter.Optimize(gc);

            Console.WriteLine("Optimized");
            Console.WriteLine("");
            gc.DumpCodeToConsole();

            // Make sure the inner aggregate got lifted out properly.
            Assert.AreEqual(1, for2.Statements.Count(), "# of statements in the inner for loop");
        }

        /// <summary>
        /// Create a new declarable parameter that will sum these two things.
        /// </summary>
        /// <param name="gc"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        public static IDeclaredParameter AddSum(GeneratedCode gc, IDeclaredParameter c1, IDeclaredParameter c2)
        {
            var r = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            gc.Add(new StatementAssign(r, new ValSimple($"{c1.RawValue}+{c2.RawValue}", c1.Type, new IDeclaredParameter[] { c1, c2 })));

            return r;
        }

        /// <summary>
        /// Add a simple if statement.
        /// </summary>
        /// <param name="gc"></param>
        public static void AddIf(GeneratedCode gc)
        {
            gc.Add(new StatementFilter(new ValSimple("5>10", typeof(bool))));
        }

        public enum MainStatementType
        {
            IsCounter,
            IsConstant
        }

        /// <summary>
        /// Add a simple loop to the current scope. It will have one statement in it, declared at the outer level.
        /// </summary>
        /// <param name="gc"></param>
        public static IDeclaredParameter AddLoop(GeneratedCode gc,
            bool addDependentStatement = false,
            IDeclaredParameter useCounter = null,
            MainStatementType mainStatementType = MainStatementType.IsCounter,
            bool defineCounterInsideBlock = false)
        {
            if (mainStatementType != MainStatementType.IsCounter && useCounter != null)
            {
                throw new ArgumentException("Can't do a counter when main statement type doesn't use a counter");
            }

            // Add a counter that gets... counted.
            var counter = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            if (!defineCounterInsideBlock)
            {
                gc.Add(counter);
            }

            // And an extra variable that is defined outside the loop
            var counterExtra = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            if (addDependentStatement)
            {
                gc.Add(counterExtra);
            }

            // Now do the loop.
            var loopVar = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopVar, new ValSimple("5", typeof(int)));
            gc.Add(loop);
            if (defineCounterInsideBlock)
            {
                gc.Add(counter);
            }

            // Now add statements to the loop
            if (mainStatementType == MainStatementType.IsCounter)
            {
                if (useCounter == null)
                {
                    gc.Add(new StatementAssign(counter, new ValSimple($"{counter.RawValue} + 1", typeof(int), new IDeclaredParameter[] { counter })));
                }
                else
                {
                    gc.Add(new StatementAssign(counter, new ValSimple($"{counter.RawValue}+{useCounter.RawValue}", typeof(int), new IDeclaredParameter[] { counter, useCounter })));
                }
            } else
            {
                gc.Add(new StatementAssign(counter, new ValSimple("1", typeof(int))));
            }

            if (addDependentStatement)
            {
                gc.Add(new StatementAssign(counterExtra, new ValSimple($"{counterExtra.RawValue}+{counter.RawValue}", typeof(int), new IDeclaredParameter[] { counterExtra, counter })));
            }

            return counter;
        }

        /// <summary>
        /// Dummy statement to help with testing.
        /// </summary>
        class DummyStatementCompoundNoCMInfo : IStatementCompound
        {
            private List<IStatement> _statements = new List<IStatement>();
            public IEnumerable<IStatement> Statements
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

            public bool Combine(IEnumerable<IStatement> statements, IBookingStatementBlock parent, bool appendIfNoCombine = true, bool moveIfIdentical = false)
            {
                throw new NotImplementedException();
            }

            public IStatement CombineAndMark(IStatement statement, IBookingStatementBlock parent, bool appendIfNoCombine = true)
            {
                throw new NotImplementedException();
            }

            public bool IsBefore(IStatement first, IStatement second)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<string> CodeItUp()
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
        public void TestNoLiftStatement()
        {
            var v = new GeneratedCode();
            var loopP = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop = new StatementForLoop(loopP, new LINQToTTreeLib.Variables.ValSimple("10", typeof(int)));
            v.Add(loop);

            v.Add(new statementNoAllowMove());

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementForLoop), "first statement");

        }

        /// <summary>
        /// A statement that isn't allowed to move.
        /// </summary>
        class statementNoAllowMove : ICMStatementInfo, IStatement
        {
            public IEnumerable<string> CodeItUp()
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

            public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst)
            {
                throw new NotImplementedException();
            }

            public IStatement Parent { get; set; }

            public ISet<string> DependentVariables
            {
                get { return new HashSet<string>(); }
            }

            public ISet<string> ResultVariables
            {
                get { return new HashSet<string>(); }
            }

            public bool NeverLift
            {
                get { return true; }
            }
        }

        [TestMethod]
        public void TestCodeWithDoubleIndex()
        {
            // Looking for two loops, and the calculation function should be moved outside
            // the first loop for efficiency reasons (as it doesn't use anything in that
            // first loop.

            var q = new QueriableDummy<dummyntup>();

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
            // Looking for two loops, and the calculation function should be moved outside
            // the first loop for efficiency reasons (as it doesn't use anything in that
            // first loop.

            var q = new QueriableDummy<dummyntup>();

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

        // We don't detect loop invariants correctly yet - so these will remain inside a loop for now.
        [TestMethod]
        public void LiftLoopInvarient()
        {
            var v = new GeneratedCode();

            var limit = new LINQToTTreeLib.Variables.ValSimple("5", typeof(int));
            var loopP1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop1 = new StatementForLoop(loopP1, limit);
            v.Add(loop1);

            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign1 = new StatementAssign(p2, new ValSimple("f", typeof(int)));
            loop1.Add(p2);
            loop1.Add(assign1);

            Console.WriteLine("Unoptimized:");
            v.DumpCodeToConsole();

            StatementLifter.Optimize(v);

            Console.WriteLine("");
            Console.WriteLine("Optimized:");
            v.DumpCodeToConsole();

            Assert.AreEqual(0, loop1.Statements.Count());
        }

#if false
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
#endif
        /// <summary>
        /// Found in the wild, when an expression contains multiple steps, and one of the steps can be lifted,
        /// it isn't.
        /// </summary>
        [TestMethod]
        public void TestLiftingHalfOfExpression()
        {
            var q = new QueriableDummy<dummyntup>();

            var res = from f in q
                      select new
                      {
                          Jets = f.valC1D.Select(l => LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.CreateTLZ(l, l, l, l)),
                          Tracks = f.vals.Select(l => LINQToTTreeLib.QueryVisitorTest.CPPHelperFunctions.CreateTLZ(l, l, l, l)),
                      };

            var resAll = from f in res
                         select new
                         {
                             Jets = f.Jets.Where(j => j.Pt() > 40.0 && Math.Abs(j.Eta()) < 2.5),
                             Tracks = f.Tracks
                         };

            var resGood = from f in resAll
                          where f.Jets.Count() == 2
                          select f;

            var resMatched = from f in resGood
                             select new
                             {
                                 matchedJets = from j in f.Jets
                                               select new
                                               {
                                                   J = j,
                                                   T = f.Tracks.Where(t => Math.Abs(t.Phi() - j.Phi()) < 0.04)
                                               }
                             };

            var resf = from f in resMatched
                       from j in f.matchedJets
                       select j.T.Count();

            var r = resf.Sum();

            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("After optimization...");
            Console.WriteLine();
            StatementLifter.Optimize(query);
            query.DumpCodeToConsole();

            var linesOfCode = query.DumpCode().TakeWhile(l => !l.Contains("aNTLorentzVector_11).Phi"));
            var openBrackets = linesOfCode.Where(l => l.Contains("{")).Count();
            var closeBrackets = linesOfCode.Where(l => l.Contains("}")).Count();
            Assert.AreEqual(openBrackets, closeBrackets, "#of nesting levels for the Phi call");
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
                var resultString = _resultVar == null ? "null" : _resultVar.RawValue;
                yield return $"StatementWithSideEffects(tracked = {_trackedVar.RawValue}, result = {resultString})";
            }

            public void RenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }

            public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
            {
                return false;
            }

            public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst)
            {
                if (other is StatementWithSideEffects)
                {
                    var s2 = other as StatementWithSideEffects;
                    var renames = new List<Tuple<string, string>>();
                    if (_trackedVar.RawValue != s2._trackedVar.RawValue)
                    {
                        renames.Add(new Tuple<string, string>(s2._trackedVar.RawValue, _trackedVar.RawValue));
                    }
                    if (_resultVar != null && s2._resultVar != null)
                    {
                        if (_resultVar.RawValue != s2._resultVar.RawValue)
                        {
                            renames.Add(new Tuple<string, string>(s2._resultVar.RawValue, _resultVar.RawValue));
                        }
                    }
                    if ((_resultVar == null || s2._resultVar == null) && _resultVar != s2.ResultVariables)
                    {
                        return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
                    }
                    return Tuple.Create(true, renames as IEnumerable<Tuple<string,string>>);
                }
                else
                {
                    return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
                }
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

            public bool NeverLift
            {
                get { return false; }
            }
        }

        /// <summary>
        /// A simple statement, but that doesn't know how to participate in optimization.
        /// </summary>
        class StatementNonOptimizing : IStatement
        {
            public System.Collections.Generic.IEnumerable<string> CodeItUp()
            {
                yield return "StatementNonOptimiing";
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
                yield return "StatementWithNoSideEffects";
            }

            public void RenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }

            public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
            {
                return false;
            }

            public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst)
            {
                if (other is StatementWithNoSideEffects)
                {
                    return Tuple.Create(true, Enumerable.Empty<Tuple<string, string>>());
                } else
                {
                    return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
                }
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

            public bool NeverLift
            {
                get { return false; }
            }
        }
    }
}
