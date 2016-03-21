﻿using LinqToTTreeInterfacesLib;
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
        /// You can't necessarily pull things out when they are nested identical loops - they may well be
        /// there for a reason!
        /// </summary>
        [TestMethod]
        public void TestNoLifeNestedIdenticalLoops()
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

            // Make sure it is two if statements, nested.
            var if1 = v.CodeBody.Statements.Skip(1).First() as StatementForLoop;
            Assert.IsNotNull(if1, "if #1");
            var if2 = if1.Statements.First() as StatementForLoop;
            Assert.IsNotNull(if2, "if #2");

            // Make sure the two loop variables are different.
            Assert.AreNotEqual(if1.LoopIndexVariable != if2.LoopIndexVariable, "Loop index vars");
        }

        /// <summary>
        /// Distilled from something we found in the wild.
        /// 1. Statement
        /// 2. Non ICM Loop/compound statement (like any inline block, I guess)
        /// 3.   Loop
        /// 4.     Statement with no side effects
        /// 
        /// 3 can get lifted past 2, but not above...
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

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementNonOptimizing), "first statement");
            var secondStatement = v.CodeBody.Statements.Skip(1).First();
            Assert.IsInstanceOfType(secondStatement, typeof(StatementInlineBlock), "Second statement");
        }

        /// <summary>
        /// Distilled from something we found in the wild.
        /// 1. Statement
        /// 2. Non ICM Loop/compound statement (like any inline block, I guess)
        /// 3.   Loop
        /// 4.     Statement with no side effects
        /// 
        /// 3 can get lifted past 2, but not above...
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

            StatementLifter.Optimize(v);

            var firstStatement = v.CodeBody.Statements.First();
            Assert.IsInstanceOfType(firstStatement, typeof(StatementNonOptimizing), "first statement");
            var secondStatement = v.CodeBody.Statements.Skip(1).First();
            Assert.IsInstanceOfType(secondStatement, typeof(StatementInlineBlock), "Second statement");
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
        /// Make sure lift occurs when identical loops are present
        /// 1. loop A
        /// 2. if statement
        /// 3.   loop A
        /// 4.   statement
        /// In this case loop A can be removed.
        /// Normally, this can't be lifted as we don't want to lift things out of an
        /// if statement. However, in this case, they are identical, so it is OK.
        /// </summary>
        [TestMethod]
        public void LiftIdenticalLoopOutOfIfStatement()
        {
            var gc = new GeneratedCode();
            var c1 = AddLoop(gc);
            gc.Pop();
            AddIf(gc);
            var c2 = AddLoop(gc);
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
            Assert.AreEqual(1, ifStatement.Statements.Count(), "# of statements inside the if statement");
            Assert.IsInstanceOfType(ifStatement.Statements.First(), typeof(StatementAssign));
            var ass = ifStatement.Statements.First() as StatementAssign;
            Assert.AreEqual("aInt_3+aInt_3", ass.Expression.RawValue);
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
            Assert.AreEqual("aInt32_3+aInt32_7", ass.Expression.RawValue);
            Assert.AreEqual(1, gc.CodeBody.Statements.Where(s => s is StatementForLoop).Count(), "# of for loops");
        }

        /// <summary>
        /// A loop repeated can be combined
        /// 1. loop A
        /// 2. loop A'
        /// 3. statement
        /// Where loop A' includes the statements in A plus extra. Until we can look
        /// into individual statements, we can't lift the common portion of A'.
        /// </summary>
        [TestMethod]
        public void CombineRepeatedAlmostIdenticalLoops()
        {
            var gc = new GeneratedCode();
            var c1 = AddLoop(gc);
            gc.Pop();
            var c2 = AddLoop(gc, true);
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
            Assert.AreEqual("aInt32_3+aInt32_7", ass.Expression.RawValue);
            Assert.AreEqual(1, gc.CodeBody.Statements.Where(s => s is StatementForLoop).Count(), "# of for loops");
            Assert.AreEqual(3, gc.CodeBody.Statements.Where(s => s is StatementForLoop).Cast<StatementForLoop>().Where(sf => sf.Statements.Count() == 2).Count(), "# of statement sin the for loop");
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

        /// <summary>
        /// Create a new declarable parameter that will sum these two things.
        /// </summary>
        /// <param name="gc"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        private IDeclaredParameter AddSum(GeneratedCode gc, IDeclaredParameter c1, IDeclaredParameter c2)
        {
            var r = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            gc.Add(new StatementAssign(r, new ValSimple($"{c1.RawValue}+{c2.RawValue}", c1.Type), new IDeclaredParameter[] { c1, c2 }));

            return r;
        }

        /// <summary>
        /// Add a simple if statement.
        /// </summary>
        /// <param name="gc"></param>
        private void AddIf(GeneratedCode gc)
        {
            gc.Add(new StatementFilter(new ValSimple("5>10", typeof(bool))));
        }

        enum MainStatementType
        {
            IsCounter,
            IsConstant
        }

        /// <summary>
        /// Add a simple loop to the current scope. It will have one statement in it, declared at the outer level.
        /// </summary>
        /// <param name="gc"></param>
        private IDeclaredParameter AddLoop(GeneratedCode gc,
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
                    gc.Add(new StatementAssign(counter, new ValSimple($"{counter.RawValue} + 1", typeof(int)), new IDeclaredParameter[] { counter }));
                }
                else
                {
                    gc.Add(new StatementAssign(counter, new ValSimple($"{counter.RawValue}+{useCounter.RawValue}", typeof(int)), new IDeclaredParameter[] { counter, useCounter }));
                }
            } else
            {
                gc.Add(new StatementAssign(counter, new ValSimple("1", typeof(int)), new IDeclaredParameter[] { }));
            }

            if (addDependentStatement)
            {
                gc.Add(new StatementAssign(counterExtra, new ValSimple($"{counterExtra.RawValue}+{counter.RawValue}", typeof(int)), new IDeclaredParameter[] { counterExtra, counter }));
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

            public IStatement Parent { get; set; }

            public ISet<string> DependentVariables
            {
                get { throw new NotImplementedException(); }
            }

            public ISet<string> ResultVariables
            {
                get { throw new NotImplementedException(); }
            }

            public bool NeverLift
            {
                get { return true; }
            }
        }

        [TestMethod]
        public void TestCodeWithDoubleIndex()
        {
            // Looking for two loops, and the Calc function should be moved outside
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
            // Looking for two loops, and the Calc function should be moved outside
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
            Assert.AreEqual(openBrackets, closeBrackets, "#of of nesting levesl for the Phi call");
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
                return false;
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
