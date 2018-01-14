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
            var assign = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(assign);
            gc.Add(p1);

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
            checkVar1.InitialValue = new ValSimple("5>10", typeof(bool));
            var if1s1 = new StatementFilter(checkVar1);
            var if1s2 = new StatementFilter(checkVar1);

            var checkVar2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            checkVar2.InitialValue = new ValSimple("5>10", typeof(bool));
            var if2s1 = new StatementFilter(checkVar2);
            var if2s2 = new StatementFilter(checkVar2);

            var randomVar = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            if1s2.Add(new StatementAssign(randomVar, new ValSimple("1", typeof(int))));
            if2s2.Add(new StatementAssign(randomVar, new ValSimple("1", typeof(int))));
            if1s1.Add(new StatementAssign(randomVar, new ValSimple("1", typeof(int))));
            if2s1.Add(new StatementAssign(randomVar, new ValSimple("1", typeof(int))));

            // We will put the first if's - that we can perhaps combine with - at the top level (though we
            // shouldn't combine with them!).
            var gc = new GeneratedCode();
            gc.Add(randomVar);
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

            DoOptimizationAndConsoleDump(gc);

            var firstMention = gc.DumpCode().TakeWhile(l => !l.Contains("aInt32_4=1")).Count();
            var secondMetnion = gc.DumpCode().SkipWhile(l => !l.Contains("aInt32_4=1")).Skip(1).Where(l => l.Contains("aInt32_4")).Count();
            Assert.AreEqual(1, secondMetnion, "Mention of ainte32 4 after the first one");
        }

        /// <summary>
        /// Two statements that are indtical should be combined.
        /// </summary>
        [TestMethod]
        public void RepeatedStatementsCombined()
        {
            var gc = new GeneratedCode();

            var a = AddSimpleAssign(gc, valToAssign: new ValSimple("10", typeof(int)));
            var b = AddSimpleAssign(gc, valToAssign: new ValSimple("10", typeof(int)));

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "should have combined into a single statement.");
            Assert.AreEqual(1, gc.CodeBody.AllDeclaredVariables.Count(), "# of declared variables");
        }

        /// <summary>
        /// Two identical assignment statements that look like they could be combined, but later on in the code
        /// they are used for different things (e.g. modified). Make sure they aren't combined.
        /// </summary>
        [TestMethod]
        public void RepeatedStatementsWithLaterModification()
        {
            var gc = new GeneratedCode();

            var a = AddSimpleAssign(gc, valToAssign: new ValSimple("10", typeof(int)));
            var b = AddSimpleAssign(gc, valToAssign: new ValSimple("10", typeof(int)));

            var c = AddSimpleAssign(gc, useParam: a, valToAssign: new ValSimple($"10+{a.RawValue}", typeof(int), new IDeclaredParameter[] { a }));

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(3, gc.CodeBody.Statements.Count(), "should have combined into a single statement.");
            Assert.AreEqual(2, gc.CodeBody.AllDeclaredVariables.Count(), "# of declared variables");
        }

        [TestMethod]
        public void RepeatedStatementsWithLaterModificationHidingAnOptimization()
        {
            var gc = new GeneratedCode();

            var a = AddSimpleAssign(gc, valToAssign: new ValSimple("10", typeof(int)));
            var b = AddSimpleAssign(gc, valToAssign: new ValSimple("10", typeof(int)));
            var c = AddSimpleAssign(gc, valToAssign: new ValSimple("10", typeof(int)));

            AddSimpleAssign(gc, useParam: a, valToAssign: new ValSimple($"10+{a.RawValue}", typeof(int), new IDeclaredParameter[] { a }));

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(3, gc.CodeBody.Statements.Count(), "should have combined into a single statement.");
            Assert.AreEqual(2, gc.CodeBody.AllDeclaredVariables.Count(), "# of declared variables");
        }

        /// <summary>
        /// 1. a = a + 1
        /// 2. a = a + 1
        /// should not be combined.
        /// </summary>
        [TestMethod]
        public void RepeatedSelfReferenctialStatments()
        {
            var gc = new GeneratedCode();
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            gc.Add(new StatementAssign(p1, new ValSimple($"{p1.RawValue}+1", typeof(int), new IDeclaredParameter[] { p1 })));
            gc.Add(new StatementAssign(p1, new ValSimple($"{p1.RawValue}+1", typeof(int), new IDeclaredParameter[] { p1 })));

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(2, gc.CodeBody.Statements.Count());
        }

        /// <summary>
        /// a = 10
        /// b = a
        /// a = 10
        /// You could combine those two a statements.
        /// </summary>
        [TestMethod]
        public void IdenticalStatementsSeperatedByUserStatement()
        {
            var gc = new GeneratedCode();

            var a = AddSimpleAssign(gc, valToAssign: new ValSimple("10", typeof(int)));
            var c = AddSimpleAssign(gc, valToAssign: new ValSimple($"{a.RawValue}", typeof(int), new IDeclaredParameter[] { a }));
            var b = AddSimpleAssign(gc, useParam: a, valToAssign: new ValSimple("10", typeof(int)));

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "should have combined into a single statement.");
            Assert.AreEqual(2, gc.CodeBody.AllDeclaredVariables.Count(), "# of declared variables");
        }

        [TestMethod]
        public void IdenticalFilters()
        {
            var gc = new GeneratedCode();

            var test = DeclarableParameter.CreateDeclarableParameterExpression("mt", typeof(bool));

            AddConditionalExpr(gc, doElseClause: false, ifStatementTest: test);
            AddConditionalExpr(gc, doElseClause: false, ifStatementTest: test);

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "# of if statements");
            Assert.AreEqual(1, gc.CodeBody.Statements.WhereCast<IStatement, StatementFilter>().First().Statements.Count(), "# of statements in the if statement");
            Assert.AreEqual(1, gc.CodeBody.DeclaredVariables.Count(), "#of declared varaibles");
        }

        /// <summary>
        /// The same thing twice:
        /// double v1, v2;
        /// if (t) {
        ///   var u1 = 0;
        ///   u1 = 10;
        ///   v1 = u1;
        /// }
        /// Only use v2 in the second one, and u2 too (which is locally declared).
        /// </summary>
        [TestMethod]
        public void IdenticalFiltersWithTwoStepAssignments()
        {
            var gc = new GeneratedCode();

            var test = DeclarableParameter.CreateDeclarableParameterExpression("mt", typeof(bool));

            AddConditionalExpr(gc, doElseClause: false, ifStatementTest: test);
            AddLocalSetAndReturn(gc, gc.CodeBody.Statements.Take(1).Cast<StatementFilter>().First());

            AddConditionalExpr(gc, doElseClause: false, ifStatementTest: test);
            AddLocalSetAndReturn(gc, gc.CodeBody.Statements.Skip(1).Take(1).Cast<StatementFilter>().First());

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(2, gc.CodeBody.DeclaredVariables.Count());
            Assert.AreEqual(1, gc.CodeBody.Statements.Count());
        }

        /// <summary>
        /// two of the following:
        /// var v;
        /// if (test) {
        ///   bool u;
        ///   var u2
        ///   u = 10 > 10;
        ///   if (u) {
        ///     u2 = 1;
        ///   }
        ///   v = u2;
        /// }
        /// </summary>
        [TestMethod]
        public void IdenticalFiltersWithNestedIfs()
        {
            var gc = new GeneratedCode();

            var test = DeclarableParameter.CreateDeclarableParameterExpression("mt", typeof(bool));

            AddConditionalExpr(gc, doElseClause: false, ifStatementTest: test);
            AddLocalInteriorIf(gc, gc.CodeBody.Statements.Take(1).Cast<StatementFilter>().First());

            AddConditionalExpr(gc, doElseClause: false, ifStatementTest: test);
            AddLocalInteriorIf(gc, gc.CodeBody.Statements.Skip(1).Take(1).Cast<StatementFilter>().First());

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(2, gc.CodeBody.DeclaredVariables.Count());
            Assert.AreEqual(1, gc.CodeBody.Statements.Count());
        }

        /// <summary>
        /// repeat this twice:
        /// var v
        /// if (mt) {
        ///   bool test
        ///   int u
        ///   test = 10 > 5
        ///   if (test) {
        ///     u = 10
        ///   }
        ///   if (!test) {
        ///     u = 20
        ///   }
        ///   v = u
        /// }
        /// The second time goes for the same v
        /// </summary>
        [TestMethod]
        public void IdenticalFiltersWithNestedIfAndElses()
        {
            var gc = new GeneratedCode();

            var test = DeclarableParameter.CreateDeclarableParameterExpression("mt", typeof(bool));

            AddConditionalExpr(gc, doElseClause: false, ifStatementTest: test);
            AddLocalInteriorIfAndElse(gc, gc.CodeBody.Statements.Take(1).Cast<StatementFilter>().First());

            AddConditionalExpr(gc, doElseClause: false, ifStatementTest: test);
            AddLocalInteriorIfAndElse(gc, gc.CodeBody.Statements.Skip(1).Take(1).Cast<StatementFilter>().First());

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(2, gc.CodeBody.DeclaredVariables.Count());
            Assert.AreEqual(1, gc.CodeBody.Statements.Count());
        }

#if false
        /// <summary>
        /// repeat this twice:
        /// var v
        /// if (mt) {
        ///   bool test
        ///   int u
        ///   test = 10 > 5
        ///   if (test) {
        ///     u = 10
        ///   }
        ///   if (!test) {
        ///     u = 20
        ///   }
        ///   v = u
        /// }
        /// Then go for a second level down from that, enclosed in an if statement
        /// that is unrelated.
        /// </summary>
        [TestMethod]
        public void IdenticalFiltersWithNestedIfAndElsesNested()
        {
            var gc = new GeneratedCode();

            var test = DeclarableParameter.CreateDeclarableParameterExpression("mt", typeof(bool));

            AddConditionalExpr(gc, doElseClause: false, ifStatementTest: test);
            AddLocalInteriorIfAndElse(gc, gc.CodeBody.Statements.Take(1).Cast<StatementFilter>().First());

            var test2 = DeclarableParameter.CreateDeclarableParameterExpression("test2", typeof(bool));
            var hiderif = new StatementFilter(test2);
            gc.Add(hiderif);

            AddConditionalExpr(gc, doElseClause: false, ifStatementTest: test);
            AddLocalInteriorIfAndElse(gc, gc.CodeBody.Statements.Skip(1).Take(1).Cast<StatementFilter>().First());

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(2, gc.CodeBody.DeclaredVariables.Count());
            Assert.AreEqual(1, gc.CodeBody.Statements.Count());
        }
#endif

        /// <summary>
        /// Pretty close to something we are seeing in the wild... that isn't working at all.
        /// </summary>
        [TestMethod]
        public void IdenticalFiltersWithNestedIfAndElseWithInbetweenStatement()
        {
            var gc = new GeneratedCode();

            var test = DeclarableParameter.CreateDeclarableParameterExpression("mt", typeof(bool));
            var result = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            AddConditionalExpr(gc, doElseClause: false, ifStatementTest: test);
            var topLevelVar = AddLocalInteriorIfAndElse(gc, gc.CodeBody.Statements.Take(1).Cast<StatementFilter>().First());

            gc.Add(new StatementAssign(result, topLevelVar));

            AddConditionalExpr(gc, doElseClause: false, ifStatementTest: test);
            AddLocalInteriorIfAndElse(gc, gc.CodeBody.Statements.Skip(2).Take(1).Cast<StatementFilter>().First(), finalSetVar: topLevelVar);

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(2, gc.CodeBody.DeclaredVariables.Count());
            Assert.AreEqual(2, gc.CodeBody.Statements.Count());
        }

        /// <summary>
        /// an internor if statement.
        /// </summary>
        /// <param name="gc"></param>
        /// <param name="statementFilter"></param>
        private void AddLocalInteriorIf(GeneratedCode gc, StatementFilter statementFilter)
        {
            // Add at top level the variable we will set down one.
            var v = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(v);

            var t = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            statementFilter.Add(t);

            var filt = new StatementFilter(t);
            statementFilter.Add(new StatementAssign(t, new ValSimple("10 > 5", typeof(bool))));
            statementFilter.Add(filt);

            var u = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            statementFilter.Add(u);

            var s1 = new StatementAssign(u, new ValSimple("10", typeof(int)));
            var s2 = new StatementAssign(v, new ValSimple($"{u.RawValue}", typeof(int), new IDeclaredParameter[] { u }));
            filt.Add(s1);
            filt.Add(s2);
        }

        /// <summary>
        /// an internor if statement.
        /// </summary>
        /// <param name="gc"></param>
        /// <param name="statementFilter">If statement</param>
        public static IDeclaredParameter AddLocalInteriorIfAndElse(GeneratedCode gc, StatementInlineBlockBase statementFilter, IDeclaredParameter finalSetVar = null)
        {
            // Add at top level the variable we will set down one.
            var v = finalSetVar;
            if (v == null)
            {
                v = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
                gc.Add(v);
            }

            var t = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            statementFilter.Add(t);

            var filt = new StatementFilter(t);
            statementFilter.Add(new StatementAssign(t, new ValSimple("10 > 5", typeof(bool))));
            statementFilter.Add(filt);

            var u = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            statementFilter.Add(u);

            var s1 = new StatementAssign(u, new ValSimple("10", typeof(int)));
            filt.Add(s1);

            var filtE = new StatementFilter(new ValSimple($"!{t.RawValue}", typeof(bool), new IDeclaredParameter[] { t }));
            statementFilter.Add(filtE);
            var s3 = new StatementAssign(u, new ValSimple("20", typeof(int)));
            filtE.Add(s3);

            var s2 = new StatementAssign(v, new ValSimple($"{u.RawValue}", typeof(int), new IDeclaredParameter[] { u }));
            statementFilter.Add(s2);

            return v;
        }

        /// <summary>
        /// Add an external declared param, and then local and a set.
        /// </summary>
        /// <param name="gc"></param>
        /// <param name="statementFilter"></param>
        private void AddLocalSetAndReturn(GeneratedCode gc, IBookingStatementBlock statementFilter)
        {
            // Add at top level the variable we will set down one.
            var v = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(v);

            var u = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementAssign(u, new ValSimple("10", typeof(int)));
            var s2 = new StatementAssign(v, new ValSimple($"{u.RawValue}", typeof(int), new IDeclaredParameter[] { u }));
            statementFilter.Add(u);
            statementFilter.Add(s1);
            statementFilter.Add(s2);
        }

        /// <summary>
        /// 1. a = 10
        /// 2. b = 10
        /// 3. if con1
        /// 4.   a = 20
        /// 5. if con2
        /// 6.   b = 20
        /// No combinations should occur.
        /// </summary>
        [TestMethod]
        public void NoCombineDifferentHistories()
        {
            var gc = new GeneratedCode();

            var a = AddSimpleAssign(gc, valToAssign: new ValSimple("10", typeof(int)));
            var b = AddSimpleAssign(gc, valToAssign: new ValSimple("20", typeof(int)));

            AddConditionalExpr(gc, doElseClause: false, mainSettingParam: a);
            AddConditionalExpr(gc, doElseClause: false, mainSettingParam: b);

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(3, gc.CodeBody.Statements.TakeWhile(s => !(s is StatementFilter)).WhereCast<IStatement, StatementAssign>().Count(), "# of top level assign statements");
            Assert.AreEqual(2, gc.CodeBody.Statements.WhereCast<IStatement, StatementFilter>().Count(), "# of if statements");
            Assert.AreEqual(2, gc.CodeBody.Statements.WhereCast<IStatement, StatementFilter>().Where(ifs => ifs.Statements.Count() == 1).Count(), "# of if statements with a statement inside them");
        }

        /// <summary>
        /// 1. a = 10
        /// 2. b = 10
        /// 3. if con1
        /// 4.   a = 20
        /// 5. if con2
        /// 6.   b = 30
        /// No combinations should occur.
        /// </summary>
        [TestMethod]
        public void NoCombineDifferentFutures()
        {
            var gc = new GeneratedCode();

            var a = AddSimpleAssign(gc, valToAssign: new ValSimple("10", typeof(int)));
            var b = AddSimpleAssign(gc, valToAssign: new ValSimple("10", typeof(int)));

            AddConditionalExpr(gc, doElseClause: false, mainSettingParam: a, mainSettingValue: new ValSimple("20", typeof(int)));
            AddConditionalExpr(gc, doElseClause: false, mainSettingParam: b, mainSettingValue: new ValSimple("30", typeof(int)));

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(3, gc.CodeBody.Statements.TakeWhile(s => !(s is StatementFilter)).WhereCast<IStatement, StatementAssign>().Count(), "# of top level assign statements");
            Assert.AreEqual(2, gc.CodeBody.Statements.WhereCast<IStatement, StatementFilter>().Count(), "# of if statements");
            Assert.AreEqual(2, gc.CodeBody.Statements.WhereCast<IStatement, StatementFilter>().Where(ifs => ifs.Statements.Count() == 1).Count(), "# of if statements with a statement inside them");
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
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(p1);
            gc.Add(assign1);
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
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
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(p1);
            gc.Add(assign1);

            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);

            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign2 = new StatementAssign(p2, new ValSimple("f", typeof(int)));
            gc.Add(p2);
            gc.Add(assign2);

            var p3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p3, p2);
            gc.Add(p3);
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
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(assign1);

            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);

            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(p2);
            var assign2 = new StatementAssign(p2, new ValSimple("f", typeof(int)));
            gc.Add(assign2);

            var p3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p3, p2);
            gc.Add(p3);
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
            Assert.AreEqual(1, backIfStatement.DeclaredVariables.Count(), "lifted variables should no longer be declared here");
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
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(p1);
            gc.Add(assign1);
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign11 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)));
            gc.Add(p2);
            gc.Add(assign11);
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            var assign22 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)));
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
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(p1);
            gc.Add(assign1);
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(assign2);
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)));
            gc.Add(p2);
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
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(p1);
            gc.Add(assign1);

            gc.Add(new DummyLoop());

            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign2 = new StatementAssign(p2, new ValSimple("f", typeof(int)));
            gc.Add(p2);
            gc.Add(assign2);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            DoOptimizationAndConsoleDump(cc);

            var block1 = cc.QueryCode().First();
            var firstLoop = block1.Statements.First() as DummyLoop;
            Assert.IsNotNull(firstLoop, "first loop");
            Assert.AreEqual(2, firstLoop.Statements.Count(), "# of statements in first loop");
            var secondLoop = firstLoop.Statements.Skip(1).First() as DummyLoop;
            Assert.IsNotNull(secondLoop, "second loop");
            Assert.AreEqual(1, secondLoop.Statements.Count(), "# of statements in second loop");
        }

        /// <summary>
        /// 1. loop
        /// 2.   idempotent statement not involving loop variables
        /// That 2 should be lifted, and the loop eliminated.
        /// </summary>
        [TestMethod]
        public void LiftIdempotentStatementInLoop()
        {
            var gc = new GeneratedCode();
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(p1);
            var loop = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(new StatementForLoop(loop, new ValSimple("10", typeof(int))));
            gc.Add(new StatementAssign(p1, new ValSimple("20", typeof(int))));

            DoOptimizationAndConsoleDump(gc);

            var loopFound = gc.CodeBody.Statements.WhereCast<IStatement, StatementForLoop>().First();
            Assert.AreEqual(0, loopFound.Statements.Count(), "# of statements in loop now");
        }

        /// <summary>
        /// 1. if
        /// 2.   idempotent statement not involving loop variables
        /// That 2 should be lifted, and the loop eliminated.
        /// </summary>
        [TestMethod]
        public void NoLiftIdempotentStatementInsideIf()
        {
            var gc = new GeneratedCode();
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(p1);
            var loop = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(new StatementFilter(new ValSimple("10>20", typeof(bool))));
            gc.Add(new StatementAssign(p1, new ValSimple("20", typeof(int))));

            DoOptimizationAndConsoleDump(gc);

            var loopFound = gc.CodeBody.Statements.WhereCast<IStatement, StatementFilter>().First();
            Assert.AreEqual(1, loopFound.Statements.Count(), "# of statements in loop now");
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
                _statements.Remove(statement);
                statement.Parent = null;
            }

            public void AddBefore(IStatement statement, IStatement beforeThisStatement)
            {
                _statements.Insert(_statements.IndexOf(beforeThisStatement), statement);
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
        public void TestIdenticalNestedIfsGetLifted()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));
            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(p1);
            gc.Add(assign2);

            var ifstatement2 = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement2);

            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)));
            gc.Add(p2);
            gc.Add(assign3);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            DoOptimizationAndConsoleDump(cc);

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
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(p1);
            gc.Add(assign2);
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)));
            gc.Add(p2);
            gc.Add(assign3);
            gc.Pop();
            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
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
            gc.Add(p1);

            var ifstatement1 = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement1);
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(assign2);
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign3 = new StatementAssign(p2, new ValSimple("f*5", typeof(int)));
            gc.Add(p2);
            gc.Add(assign3);
            gc.Pop();

            var ifstatement2 = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement2);
            var assign4 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(assign4);
            var p3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign5 = new StatementAssign(p3, new ValSimple("f*5", typeof(int)));
            gc.Add(p3);
            gc.Add(assign5);
            gc.Pop();

            var assign1 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(assign1);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            DoOptimizationAndConsoleDump(cc);

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
            var assign2 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(p1);
            gc.Add(assign2);

            // Go down two levels.
            var ifstatement1 = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement1);
            var ifstatement2 = new StatementFilter(new ValSimple("j", typeof(int)));
            gc.Add(ifstatement2);

            // Two levels down add the second common and unique statements.
            var assign4 = new StatementAssign(p1, new ValSimple("f", typeof(int)));
            gc.Add(assign4);
            var p3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assign5 = new StatementAssign(p3, new ValSimple("f*5", typeof(int)));
            gc.Add(p3);
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

            var query = CombineQueries(query1, query2);
            Assert.IsTrue(query.CheckCodeBlock(), "combined query ok");

            DoOptimizationAndConsoleDump(query);
            Assert.IsTrue(query.CheckCodeBlock(), "optimized combined query ok");

            // We test for this by making sure the "abs" function is called only twice in
            // the generated code.

            Assert.AreEqual(2, query.DumpCode().Where(l => l.Contains("abs")).Count(), "# of times abs appears in the code");
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
        public void TestNoCommonLifeNestedIdenticalLoops()
        {
            var v = new GeneratedCode();
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            v.Add(p1);

            var limit = new LINQToTTreeLib.Variables.ValSimple("5", typeof(int));
            var loopP1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop1 = new StatementForLoop(loopP1, limit);
            v.Add(loop1);
            var loopP2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop2 = new StatementForLoop(loopP2, limit);
            v.Add(loop2);
            v.Add(new StatementAssign(p1, new ValSimple($"{loopP1.RawValue}+{loopP2.RawValue}", typeof(int), new IDeclaredParameter[] { loopP2, loopP1 })));

            Console.WriteLine("Unoptimized:");
            v.DumpCodeToConsole();
            CommonStatementLifter.Optimize(v);
            Console.WriteLine("");
            Console.WriteLine("Optimized:");
            v.DumpCodeToConsole();

            // Make sure it is two if statements, nested.
            var if1 = v.CodeBody.Statements.First() as StatementForLoop;
            Assert.IsNotNull(if1, "for #1");
            var if2 = if1.Statements.First() as StatementForLoop;
            Assert.IsNotNull(if2, "assign");
            var a = if2.Statements.First() as StatementAssign;
            Assert.IsNotNull(a);
        }

        /// <summary>
        /// 1. Loop 1 over array a
        /// 2. Loop 2 over array a
        /// 3.  if statement with iterator 1
        /// 4.    simple statement
        /// The if statement should come up one level.
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
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            v.Add(p1);
            v.Add(loop2);
            v.Add(new StatementFilter(new ValSimple(string.Format("{0}!=0", loopP1.RawValue), typeof(bool), new IDeclaredParameter[] { loopP1 })));
            v.Add(new StatementAssign(p1, new ValSimple("10", typeof(int))));

            Console.WriteLine("Unoptimized:");
            v.DumpCodeToConsole();
            CommonStatementLifter.Optimize(v);
            Console.WriteLine("");
            Console.WriteLine("Optimized:");
            v.DumpCodeToConsole();

            // Make sure it is two if statements, nested.
            var if1 = v.CodeBody.Statements.WhereCast<IStatement, StatementForLoop>().Where(s => s.Statements.Any()).First();
            Assert.IsNotNull(if1, "for #1");
            var if2 = if1.Statements.First() as StatementFilter;
            Assert.IsNotNull(if2, "if #2");
            var a2 = if2.Statements.First() as StatementAssign;
            Assert.IsNotNull(a2);
        }

        /// <summary>
        /// 1. Loop 1 over array a
        /// 2.   Loop 2 over array a
        /// 3.    if statement
        /// 4.      Something independent of 1 and 2
        /// 5.   Same statement independent of 1 and 2
        /// Inner statement should be pulled out to the interior of loop 1.
        /// And that should be pulled all the way out to the top since it is a loop invarient.
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
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            v.Add(p1);
            v.Add(new StatementFilter(new ValSimple(string.Format("{0}!=0", loopP1.RawValue), typeof(bool))));
            v.Add(new StatementAssign(p1, new ValSimple("10", typeof(int))));

            v.CurrentScope = lp1Scope;
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            v.Add(p2);
            v.Add(new StatementAssign(p2, new ValSimple("10", typeof(int))));

            Console.WriteLine("Unoptimized:");
            v.DumpCodeToConsole();
            CommonStatementLifter.Optimize(v);
            Console.WriteLine("");
            Console.WriteLine("Optimized:");
            v.DumpCodeToConsole();

            // Make sure it is two if statements, nested.
            Assert.AreEqual(1, v.CodeBody.Statements.WhereCast<IStatement, StatementAssign>().Count());
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
        /// We don't normally want to lift things past an if statement b.c. it is an efficiency
        /// protector
        /// </summary>
        [TestMethod]
        public void LiftIdenticalLoopOutOfIfStatement()
        {
            var gc = new GeneratedCode();
            var c1 = StatementLifterTest.AddLoop(gc);
            gc.Pop();
            StatementLifterTest.AddIf(gc);
            var c2 = StatementLifterTest.AddLoop(gc);
            gc.Pop();
            StatementLifterTest.AddSum(gc, c1, c2);

            Console.WriteLine("Before lifting and optimization: ");
            gc.DumpCodeToConsole();

            CommonStatementLifter.Optimize(gc);

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
            Assert.AreEqual($"{c1.RawValue}+{c1.RawValue}", ass.Expression.RawValue);
            Assert.AreEqual(1, gc.CodeBody.DeclaredVariables.Count(), "# of variables declared");
            Assert.AreEqual(c1.RawValue, gc.CodeBody.DeclaredVariables.First().RawValue, "Counter is declared.");
        }

        /// <summary>
        /// Make sure lift occurs when identical loops are present, but in reverse order.
        /// 1. if statement
        /// 2.   loop A
        /// 4. loop A
        /// In this case loop A can be removed.
        /// Normally, this can't be lifted as we don't want to lift things out of an
        /// if statement. However, in this case, they are identical, so it is OK.
        /// We don't normally want to lift things past an if statement b.c. it is an efficiency
        /// protector
        /// </summary>
        [TestMethod]
        public void LiftIdenticalLoopOutOfIfStatementReverse()
        {
            var gc = new GeneratedCode();
            StatementLifterTest.AddIf(gc);
            var c2 = StatementLifterTest.AddLoop(gc);
            gc.Pop();
            gc.Pop();
            var c1 = StatementLifterTest.AddLoop(gc);

            DoOptimizationAndConsoleDump(gc);

            // Now check that things happened as we would expect them to happen.
            Assert.AreEqual(1, gc.CodeBody.Statements.Where(s => s is StatementForLoop).Count(), "# of for loops at outer level");
            Assert.AreEqual(1, gc.CodeBody.Statements.Where(s => s is StatementForLoop).Cast<StatementForLoop>().Where(s => s.Statements.Count() == 1).Count(), "# of statements inside first for loop");

            var ifStatement = gc.CodeBody.Statements.Where(s => s is StatementFilter).Cast<StatementFilter>().First();
            Assert.IsNotNull(ifStatement, "Finding if statement");
            Assert.AreEqual(0, ifStatement.Statements.Count(), "# of statements inside the if statement");
            Assert.AreEqual(1, gc.CodeBody.DeclaredVariables.Count(), "# of variables declared");
            Assert.AreEqual(c1.RawValue, gc.CodeBody.DeclaredVariables.First().RawValue, "Counter is declared.");
        }

        /// <summary>
        /// 1. statement A
        /// 2. if
        /// 3.   Statement A
        /// Where A is idempotent. This lift should occur.
        /// </summary>
        [TestMethod]
        public void LiftSimpleStatementInIfBefore()
        {
            var gc = new GeneratedCode();
            var p = AddSimpleAssign(gc, valToAssign: new ValSimple("f1", typeof(int)));
            AddConditionalExpr(gc, doElseClause: false, mainSettingParam: p);

            Console.WriteLine("Before Optimization");
            Console.WriteLine("");
            gc.DumpCodeToConsole();

            CommonStatementLifter.Optimize(gc);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("After Optimization");
            Console.WriteLine("");
            gc.DumpCodeToConsole();

            Assert.AreEqual(0, gc.CodeBody.Statements.WhereCast<IStatement, StatementFilter>().Select(s => s.Statements.Count()).First(), "# of statements under if statement");
        }

        /// <summary>
        /// 1. if
        /// 2.   Statement A
        /// 3. statement A
        /// Where A is idempotent. This lift should occur, and statement A should be moved in front of the if statement.
        /// </summary>
        [TestMethod]
        public void LiftSimpleStatementInIfAfter()
        {
            var gc = new GeneratedCode();
            var p = AddConditionalExpr(gc, doElseClause: false);
            AddSimpleAssign(gc, useParam: p, valToAssign: new ValSimple("f1", typeof(int)));

            Console.WriteLine("Before Optimization");
            Console.WriteLine("");
            gc.DumpCodeToConsole();

            CommonStatementLifter.Optimize(gc);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("After Optimization");
            Console.WriteLine("");
            gc.DumpCodeToConsole();

            Assert.AreEqual(0, gc.CodeBody.Statements.WhereCast<IStatement, StatementFilter>().Select(s => s.Statements.Count()).First(), "# of statements under if statement");
        }

        /// <summary>
        /// 1. if B
        /// 2.   statement A
        /// 3.   dependent on A
        /// 4. statement A
        /// If A can commute with if, it should be put on top of if.
        /// </summary>
        [TestMethod]
        public void LiftStatementInAfterWithDependent()
        {
            var gc = new GeneratedCode();

            var pfinal = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var p = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var secondAssign = new StatementAssign(pfinal, new ValSimple($"{p.RawValue}*2", typeof(int), new IDeclaredParameter[] { p }));
            gc.Add(p);
            gc.Add(pfinal);

            AddConditionalExpr(gc, doElseClause: false, mainSettingParam: p, addInFirstAfter: secondAssign);
            AddSimpleAssign(gc, useParam: p, valToAssign: new ValSimple("f1", typeof(int)));

            DoOptimizationAndConsoleDump(gc);

            Assert.AreEqual(1, gc.CodeBody.Statements.WhereCast<IStatement, StatementFilter>().Select(s => s.Statements.Count()).First(), "# of statements under if statement");
            Assert.AreEqual(2, gc.CodeBody.Statements.TakeWhile(s => !(s is StatementFilter)).Where(s => s is StatementAssign).Count(), "# of statements before if statement (must be assign)");
        }

        /// <summary>
        /// Make the output and test a little more uniform.
        /// </summary>
        /// <param name="gc"></param>
        private static CombinedGeneratedCode DoOptimizationAndConsoleDump(params IExecutableCode[] gc)
        {
            Console.WriteLine("Before Optimization");
            Console.WriteLine("");

            var cc = new CombinedGeneratedCode();
            foreach (var block in gc)
            {
                Console.WriteLine("Code Block:");
                Console.WriteLine("===========");
                block.DumpCodeToConsole();
                cc.AddGeneratedCode(block);
                Console.WriteLine();
            }

            if (gc.Length > 1)
            {
                Console.WriteLine("Combined Code Block:");
                Console.WriteLine("===========");
                cc.DumpCodeToConsole();
            }

            CommonStatementLifter.Optimize(cc);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("After Optimization");
            Console.WriteLine("");
            cc.DumpCodeToConsole();

            return cc;
        }

        /// <summary>
        /// 1. if cond1
        /// 2.   Statement A
        /// 3. if cond2
        /// 4.   if cond 1
        /// 5.     statement A
        /// 6.   statement involving result of A
        /// 7. Second statement involving A
        /// Should lift that inner if statement.
        /// </summary>
        [TestMethod]
        public void LiftIfStatement()
        {
            var gc = new GeneratedCode();

            var testExpr = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            gc.Add(testExpr);

            var p = AddConditionalExpr(gc, doElseClause: false, ifStatementTest: testExpr);

            var secondIf = new StatementFilter(new ValSimple("5>10", typeof(bool), null));
            gc.Add(secondIf);
            AddConditionalExpr(gc, doElseClause: false, mainSettingParam: p, ifStatementTest: testExpr);
            AddSimpleAssign(gc, valToAssign: new ValSimple($"{p.RawValue}*2.0", p.Type, new IDeclaredParameter[] { p }));

            DoOptimizationAndConsoleDump(gc);

            var secondIfStatement = gc.CodeBody.Statements.WhereCast<IStatement, StatementFilter>().Skip(1).First();
            Assert.AreEqual(1, secondIfStatement.Statements.Count(), "only the assign should remain");
            Assert.IsTrue(secondIfStatement.Statements.Where(s => s is StatementAssign).Any(), "Is an assignmnt statement");
        }

        /// <summary>
        /// Say you have an aggregate statement that is in an inner loop that is the "same" as the outer loop one.
        /// It should not be lifted since it will alter the counting!
        /// </summary>
        [TestMethod]
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
        /// Found in the wild in another test (actually), and isolating it here to help debug it.
        /// </summary>
        [TestMethod]
        public void DontLiftEmbededIfWithDifferentOutcome()
        {
            // Generate the two close, but not identical calls.
            var q = new QueriableDummy<TestNtupeArr>
            {
                DOQueryFunctions = false
            };

            var dudeQ1 = from evt in q
                         where (evt.myvectorofint.First() > 0)
                         select evt;
            var dude1 = dudeQ1.Count();
            var query1 = DummyQueryExectuor.FinalResult;
            Optimizer.Optimize(query1);

            var dudeQ2 = from evt in q
                         where (evt.myvectorofint.Skip(1).First() > 0)
                         select evt;
            var dude2 = dudeQ2.Count();
            var query2 = DummyQueryExectuor.FinalResult;
            Optimizer.Optimize(query2);

            // SHove them into a combined guy.
            var cc = DoOptimizationAndConsoleDump(query1, query2);

            // Now, take a look at this and look for there to be both the Take and the Skip in there.
            Assert.IsTrue(cc.DumpCode().Where(l => l.Contains("++;")).Any(), "Some evidence of auto-increment");
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

            var varname = lines.FindVariableIn("int $$ = -1;");
            var firstMention = query.DumpCode().Where(l => l.Contains(varname)).Skip(0).First();
            Assert.AreEqual(string.Format("int {0} = -1;", varname), firstMention.Trim(), "aint32_23 decl");
        }

#if false
        Here is the code this generates:
    {
      double aDouble_4=0;
      int aInt32_5=0;
      if (i)
      {
        double aDouble_2=0;
        int aInt32_3=0;
        aInt32_3=f;
        if (aInt32_3)
        {
          aDouble_2=f1;
        }
        if (!aInt32_3)
        {
          aDouble_2=f2;
        }
      }
      aInt32_5=f;
      if (aInt32_5)
      {
        aDouble_4=f1;
      }
      if (!aInt32_5)
      {
        aDouble_4=f2;
      }
    }

        Note that the two sets of if statements have to both be identical. The code doesn't recognize pairs yet, so
        this is impossible (e.g. think about what if the second aDouble_4 was f3 instead of f2).

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

            DoOptimizationAndConsoleDump(gc);

            var block1 = cc.QueryCode().First().Statements.Skip(2).FirstOrDefault();
            Assert.IsInstanceOfType(block1, typeof(StatementFilter));
            var filter = block1 as StatementFilter;
            Assert.AreEqual(1, filter.Statements.Count());

            var ifStatementI = cc.QueryCode().First().Statements.Where(s => s is StatementFilter && ((s as StatementFilter).TestExpression.RawValue == "i")).Cast<StatementFilter>().FirstOrDefault();
            Assert.IsNotNull(ifStatementI);
            Assert.AreEqual(0, ifStatementI.Statements.Count());
        }
#endif

#if false
        Here is the code this generates:
    {
      double aDouble_4=0;
      int aInt32_5=0;
      if (i)
      {
        double aDouble_2=0;
        int aInt32_3=0;
        aInt32_3=f;
        if (aInt32_3)
        {
          aDouble_2=f1;
        }
        if (!aInt32_3)
        {
          aDouble_2=f2;
        }
      }
      aInt32_5=f;
      if (aInt32_5)
      {
        aDouble_4=f1;
      }
      if (!aInt32_5)
      {
        aDouble_4=f2;
      }
    }

        The optimization only works if *both* if statements are the same. If the second one had aDouble_4=f3 instead of f2, then
        this wouldn't work. If these two if statements could be gathered into a single statement (if/else), then this combination
        could be done. Since it is more complex than the code is currently intended to solve, we will remove this test.

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
            gc.Add(new StatementAssign(p1Extra, new ValSimple("fExtra", typeof(double))));
            gc.Add(p1Extra);

            gc.Pop();
            AddConditionalExpr(gc);

            var cc = new CombinedGeneratedCode();
            cc.AddGeneratedCode(gc);

            DoOptimizationAndConsoleDump(cc);

            var block1 = cc.QueryCode().First().Statements.Skip(2).FirstOrDefault();
            Assert.IsInstanceOfType(block1, typeof(StatementFilter));
            var filter = block1 as StatementFilter;
            Assert.AreEqual(1, filter.Statements.Count());

            var ifStatementI = cc.QueryCode().First().Statements.Where(s => s is StatementFilter && ((s as StatementFilter).TestExpression.RawValue == "i")).Cast<StatementFilter>().FirstOrDefault();
            Assert.IsNotNull(ifStatementI);
            Assert.AreEqual(1, ifStatementI.Statements.Count());
        }
#endif

        [TestMethod]
        public void DuplicateIfStatementWithExtraInnerLineIfDeepIf()
        {
            var gc = new GeneratedCode();
            gc.SetResult(DeclarableParameter.CreateDeclarableParameterExpression(typeof(double)));

            var toplevel = gc.CurrentScope;

            var ifstatement = new StatementFilter(new ValSimple("i", typeof(int)));
            gc.Add(ifstatement);

            var p1Extra = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var toadd = new StatementAssign(p1Extra, new ValSimple("fExtra", typeof(double)));
            AddSingleIfExpr(gc, addIn: toadd);
            gc.Add(p1Extra);

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
        public static IDeclaredParameter AddConditionalExpr(GeneratedCode gc, IValue mainSettingValue = null, IStatement addInFirst = null, IStatement addInFirstAfter = null, IDeclaredParameter ifStatementTest = null, IStatement addInSecond = null, bool doElseClause = true, IDeclaredParameter mainSettingParam = null)
        {
            if (mainSettingParam == null)
            {
                mainSettingParam = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
                gc.Add(mainSettingParam);
            }
            if (mainSettingValue == null)
            {
                mainSettingValue = new ValSimple("f1", typeof(double));
            }

            if (ifStatementTest == null)
            {
                ifStatementTest = AddSimpleAssign(gc, valToAssign: new ValSimple("f", typeof(bool)));
            }

            var ifstatement = new StatementFilter(ifStatementTest);
            gc.Add(ifstatement);
            if (addInFirst != null)
                gc.Add(addInFirst);
            AddSimpleAssign(gc, valToAssign: mainSettingValue, useParam: mainSettingParam);
            if (addInFirstAfter != null)
                gc.Add(addInFirstAfter);
            gc.Pop();

            if (doElseClause)
            {
                gc.Add(new StatementFilter(new ValSimple("!" + ifStatementTest.ParameterName, typeof(bool))));
                AddSimpleAssign(gc, useParam: mainSettingParam, valToAssign: new ValSimple("f2", typeof(double)));
                if (addInSecond != null)
                {
                    gc.Add(addInSecond);
                }
                gc.Pop();
            }

            return mainSettingParam;
        }

        /// <summary>
        /// Add a simple assign statement
        /// </summary>
        /// <param name="gc"></param>
        /// <param name="useParam"></param>
        /// <param name="valToAssign"></param>
        /// <returns></returns>
        public static IDeclaredParameter AddSimpleAssign(GeneratedCode gc, IDeclaredParameter useParam = null, IValue valToAssign = null, Type t = null)
        {
            if (useParam == null)
            {
                useParam = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
                gc.Add(useParam);
            }
            if (t == null)
            {
                t = typeof(int);
            }
            if (valToAssign == null)
            {
                valToAssign = new ValSimple("f1", t, null);
            }
            gc.Add(new StatementAssign(useParam, valToAssign));
            return useParam;
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
            var assignp2 = new StatementAssign(p2, new ValSimple("f", typeof(bool)));
            gc.Add(p2);
            gc.Add(assignp2);
            var ifstatement = new StatementFilter(p2);
            gc.Add(ifstatement);
            if (addIn != null)
                gc.Add(addIn);
            var assign3 = new StatementAssign(p1, new ValSimple("f1", typeof(double)));
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
