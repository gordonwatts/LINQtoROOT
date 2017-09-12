using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.QMFunctions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.Tests.QMFunctions;
using LINQToTTreeLib.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static LINQToTTreeLib.QueryVisitorTest;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for CombinedGeneratedCode</summary>
    [TestClass]
    [DeploymentItem(@"ConfigData\default.classmethodmappings")]
    public class CombinedGeneratedCodeTest
    {
        [TestInitialize]
        public void Setup()
        {
            TestUtils.ResetLINQLibrary();
            MEFUtilities.MyClassInit();
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        [TestMethod]
        public void CombinedGeneratedDifferentInitalizationStatements()
        {
            var q1 = new GeneratedCode();
            var q2 = new GeneratedCode();

            var s1 = new Statements.StatementSimpleStatement("dude1");
            var s2 = new Statements.StatementSimpleStatement("dude2");

            q1.AddInitalizationStatement(s1);
            q2.AddInitalizationStatement(s2);

            var target = new CombinedGeneratedCode();
            target.AddGeneratedCode(q1);
            target.AddGeneratedCode(q2);

            var initStatements = target.InitalizationStatements.ToArray();
            Assert.AreEqual(2, initStatements.Length);
        }

        /// <summary>
        /// Explicit test to see if the combining works correctly.
        /// </summary>
        [TestMethod]
        public void TestSimpleCombine()
        {
            var q1 = new GeneratedCode();
            var q2 = new GeneratedCode();

            var s1 = new Statements.StatementSimpleStatement("dude1");
            var s2 = new Statements.StatementSimpleStatement("dude2");

            q1.Add(s1);
            q2.Add(s2);

            var v1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var v2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            q1.SetResult(v1);
            q2.SetResult(v2);

            var target = new CombinedGeneratedCode();
            target.AddGeneratedCode(q1);
            target.AddGeneratedCode(q2);

            Assert.AreEqual(1, target.QueryCode().Count(), "didn't combine blocks correctly");
            var c = target.QueryCode().First();
            Assert.AreEqual(2, c.Statements.Count(), "bad # of statements in combined query");
            var st1 = c.Statements.First();
            var st2 = c.Statements.Skip(1).First();
            Assert.IsInstanceOfType(st1, typeof(Statements.StatementSimpleStatement), "st1");
            Assert.IsInstanceOfType(st2, typeof(Statements.StatementSimpleStatement), "st2");

            var sst1 = st1 as Statements.StatementSimpleStatement;
            var sst2 = st2 as Statements.StatementSimpleStatement;
            Assert.IsTrue("dude1" == sst1.Line || "dude1" == sst2.Line, "sst1");
            Assert.IsTrue("dude2" == sst1.Line || "dude2" == sst2.Line, "sst2");
        }

        [TestMethod]
        public void CombineTwoTopLevelFunctions()
        {
            var q1 = new GeneratedCode();
            var q2 = new GeneratedCode();

            var f1 = QMFuncUtils.GenerateFunction();
            var r1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new Statements.StatementAssign(r1, new Variables.ValSimple(f1.Name + "()", typeof(int)));
            var f2 = QMFuncUtils.GenerateFunction();
            var r2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new Statements.StatementAssign(r2, new Variables.ValSimple(f2.Name + "()", typeof(int)));

            q1.Add(f1);
            q1.Add(s1);
            q1.SetResult(r1);

            q2.Add(f2);
            q2.Add(s2);
            q2.SetResult(r2);

            var target = new CombinedGeneratedCode();
            target.AddGeneratedCode(q1);
            target.AddGeneratedCode(q2);

            target.DumpCodeToConsole();

            Assert.AreEqual(1, target.Functions.Count(), "# of functions should be combined to 1");
            Assert.AreEqual(1, target.QueryCode().Count(), "# of query code blocks");
            Assert.AreEqual(2, target.QueryCode().First().Statements.Count(), "# of statements in the combined block.");
            Assert.IsFalse(target.DumpCode().Where(l => l.Contains(f2.Name)).Any(), "The new function was still in there");
        }

        [TestMethod]
        public void CombineTwoTwoLevelFunctions()
        {
            var q1 = new GeneratedCode();
            var q2 = new GeneratedCode();

            var f1 = GenerateFunction2();
            var r1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new Statements.StatementAssign(r1, new Variables.ValSimple(f1[1].Name + "()", typeof(int)));
            var f2 = GenerateFunction2();
            var r2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new Statements.StatementAssign(r2, new Variables.ValSimple(f2[1].Name + "()", typeof(int)));

            q1.Add(f1[0]);
            q1.Add(f1[1]);
            q1.Add(s1);
            q1.SetResult(r1);

            q2.Add(f2[0]);
            q2.Add(f2[1]);
            q2.Add(s2);
            q2.SetResult(r2);

            var target = new CombinedGeneratedCode();
            target.AddGeneratedCode(q1);
            target.AddGeneratedCode(q2);

            target.DumpCodeToConsole();

            Assert.AreEqual(2, target.Functions.Count(), "# of functions should be combined to 2");
            Assert.AreEqual(1, target.QueryCode().Count(), "# of query code blocks");
            Assert.AreEqual(2, target.QueryCode().First().Statements.Count(), "# of statements in the combined block.");
            Assert.IsFalse(target.DumpCode().Where(l => l.Contains(f2[0].Name)).Any(), "The new function was still in there");
            Assert.IsFalse(target.DumpCode().Where(l => l.Contains(f2[1].Name)).Any(), "The new function was still in there");
        }

        [TestMethod]
        public void CombineOneTwoLevelAndOneOneLevelFunctions1()
        {
            var q1 = new GeneratedCode();
            var q2 = new GeneratedCode();

            var f1 = GenerateFunction2();
            var r1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new Statements.StatementAssign(r1, new Variables.ValSimple(f1[1].Name + "()", typeof(int)));
            var f2 = QMFuncUtils.GenerateFunction();
            var r2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new Statements.StatementAssign(r2, new Variables.ValSimple(f2.Name + "()", typeof(int)));

            q1.Add(f1[0]);
            q1.Add(f1[1]);
            q1.Add(s1);
            q1.SetResult(r1);

            q2.Add(f2);
            q2.Add(s2);
            q2.SetResult(r2);

            var target = new CombinedGeneratedCode();
            target.AddGeneratedCode(q1);
            target.AddGeneratedCode(q2);

            target.DumpCodeToConsole();

            Assert.AreEqual(2, target.Functions.Count(), "# of functions should be combined to 2");
            Assert.AreEqual(1, target.QueryCode().Count(), "# of query code blocks");
            Assert.AreEqual(2, target.QueryCode().First().Statements.Count(), "# of statements in the combined block.");
            Assert.IsFalse(target.DumpCode().Where(l => l.Contains(f2.Name)).Any(), "The new function was still in there");
        }

        [TestMethod]
        public void CombineOneTwoLevelAndOneOneLevelFunctions2()
        {
            var q1 = new GeneratedCode();
            var q2 = new GeneratedCode();

            var f1 = QMFuncUtils.GenerateFunction();
            var r1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new Statements.StatementAssign(r1, new Variables.ValSimple(f1.Name + "()", typeof(int)));
            var f2 = GenerateFunction2();
            var r2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new Statements.StatementAssign(r2, new Variables.ValSimple(f2[1].Name + "()", typeof(int)));

            q1.Add(f1);
            q1.Add(s1);
            q1.SetResult(r1);

            q2.Add(f2[0]);
            q2.Add(f2[1]);
            q2.Add(s2);
            q2.SetResult(r2);

            var target = new CombinedGeneratedCode();
            target.AddGeneratedCode(q1);
            target.AddGeneratedCode(q2);

            target.DumpCodeToConsole();

            Assert.AreEqual(2, target.Functions.Count(), "# of functions should be combined to 2");
            Assert.AreEqual(1, target.QueryCode().Count(), "# of query code blocks");
            Assert.AreEqual(2, target.QueryCode().First().Statements.Count(), "# of statements in the combined block.");
            Assert.IsFalse(target.DumpCode().Where(l => l.Contains(f2[0].Name)).Any(), "The new function was still in there");
        }

        [TestMethod]
        public void CombineSameQueries()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r = from evt in q
                    let j1 = evt.var1.First()
                    let a1 = j1 > 0 ? evt.var2[j1] : 0.0
                    select a1;
            var r1 = r.Sum();
            var query1 = DummyQueryExectuor.FinalResult;
            Console.WriteLine();
            Console.WriteLine("**** First Query");
            query1.DumpCodeToConsole();

            var rr = from evt in q
                    let j1 = evt.var1.First()
                    let a1 = j1 > 0 ? evt.var2[j1] : 0.0
                    select a1;
            var r2 = rr.Sum();
            var query2 = DummyQueryExectuor.FinalResult;
            Console.WriteLine();
            Console.WriteLine("**** Second Query");
            query2.DumpCodeToConsole();

            var query = CombineQueries(query2, query1);

            Console.WriteLine();
            Console.WriteLine("**** Combined Query");
            query.DumpCodeToConsole();

            // Find the if-statement protected code to see what the values are we are looking at.
            var ifStatementBlock = (query.QueryCode()
                .First()
                .Statements
                .Where(s => s is StatementFilter)
                .First()) as StatementFilter;
            Assert.IsNotNull(ifStatementBlock);

            // Since these are identical, we expect only a single assignment statement in here.
            Assert.AreEqual(1, ifStatementBlock.Statements.Count());
        }

        [TestMethod]
        public void CombineSameQueriesWithDifferentPredonditions()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r = from evt in q
                    let j1 = evt.var1.Skip(1).First()
                    let a1 = j1 > 0 ? evt.var2[j1] : 0.0
                    select a1;
            var r1 = r.Sum();
            var query1 = DummyQueryExectuor.FinalResult;
            Console.WriteLine();
            Console.WriteLine("**** First Query");
            query1.DumpCodeToConsole();

            var rr = from evt in q
                     let j1 = evt.var1.First()
                     let a1 = j1 > 0 ? evt.var2[j1] : 0.0
                     select a1;
            var r2 = rr.Sum();
            var query2 = DummyQueryExectuor.FinalResult;
            Console.WriteLine();
            Console.WriteLine("**** Second Query");
            query2.DumpCodeToConsole();

            var query = CombineQueries(query2, query1);

            Console.WriteLine();
            Console.WriteLine("**** Combined Query");
            query.DumpCodeToConsole();

            // Find the if-statement protected code to see what the values are we are looking at.
            var ifStatementCount = query.QueryCode()
                .First()
                .Statements
                .Where(s => s is StatementFilter)
                .Count();
            Assert.AreEqual(4, ifStatementCount);
        }

        [TestMethod]
        public void CombineSlightlyQueries()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r = from evt in q
                    let j1 = evt.var1.First()
                    let a1 = j1 > 0 ? evt.var3[j1] : 0.0
                    select a1;
            var r1 = r.Sum();
            var query1 = DummyQueryExectuor.FinalResult;
            Console.WriteLine();
            Console.WriteLine("**** First Query");
            query1.DumpCodeToConsole();

            var rr = from evt in q
                     let j1 = evt.var1.First()
                     let a1 = j1 > 0 ? evt.var2[j1] : 0.0
                     select a1;
            var r2 = rr.Sum();
            var query2 = DummyQueryExectuor.FinalResult;
            Console.WriteLine();
            Console.WriteLine("**** Second Query");
            query2.DumpCodeToConsole();

            var query = CombineQueries(query2, query1);

            Console.WriteLine();
            Console.WriteLine("**** Combined Query");
            query.DumpCodeToConsole();

            // Find the if-statement protected code to see what the values are we are looking at.
            var ifStatementBlock = (query.QueryCode()
                .First()
                .Statements
                .Where(s => s is StatementFilter)
                .First()) as StatementFilter;
            Assert.IsNotNull(ifStatementBlock);
            Assert.AreEqual(2, ifStatementBlock.Statements.Count());

            // Fetch out the two assignment statements.
            var ass1 = ifStatementBlock.Statements.First() as StatementAssign;
            var ass2 = ifStatementBlock.Statements.Skip(1).First() as StatementAssign;
            Assert.IsNotNull(ass1);
            Assert.IsNotNull(ass2);

            Assert.AreNotEqual(ass1.ResultVariable.RawValue, ass2.ResultVariable.RawValue);
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

        private QMFuncSource[] GenerateFunction2()
        {
            var fsub = QMFuncUtils.GenerateFunction();

            int[] ints = new int[10];
            var h = new QMFuncHeader() { Arguments = new object[] { }, QM = new QueryModel(new MainFromClause("i", typeof(int), Expression.Constant(ints)), new SelectClause(Expression.Constant(10))) };
            h.QMText = h.QM.ToString();
            var f = new QMFuncSource(h);

            var p = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var st = new StatementAssign(p, new ValSimple(fsub.Name + "()", typeof(int)));
            var inlineblock = new StatementInlineBlock();
            inlineblock.Add(st);
            inlineblock.Add(new StatementReturn(p));
            f.SetCodeBody(inlineblock);

            return new QMFuncSource[] { fsub, f };
        }
    }
}
