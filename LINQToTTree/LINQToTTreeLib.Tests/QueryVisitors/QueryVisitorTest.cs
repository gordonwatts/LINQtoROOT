using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.ResultOperators;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.TypeHandlers.ROOT;
using LINQToTTreeLib.TypeHandlers.TranslationTypes;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing.Structure;
// <copyright file="QueryVisitorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for QueryVisitor</summary>
    [PexClass(typeof(QueryVisitor))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class QueryVisitorTest
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

        private QueryModel GetModel<T>(Expression<Func<T>> expr)
        {
            var parser = QueryParser.CreateDefault();
            return parser.GetParsedQuery(expr.Body);
        }

        public class dummyntup
        {
            public int run;
            public int[] vals;
            public int[][] val2D;

            [ArraySizeIndex("run")]
            public int[] valC1D;

            [ArraySizeIndex("20", IsConstantExpression = true)]
            public int[] valC1DConst;

            [ArraySizeIndex("20", IsConstantExpression = true, Index = 0)]
            [ArraySizeIndex("run", Index = 1)]
            public int[][] valC2D;
        }

        [TestMethod]
        public void TestMEFQueryPassAlong()
        {
            var model = GetModel(() => (
                from q in new QueriableDummy<dummyntup>()
                select q.vals.Count()).Aggregate(0, (acc, va) => acc + va));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(dummyntup) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(model);
        }

#if false
        /// Don't know how to count the # of parts in the catalog.
        [TestMethod]
        public void TestMEFContainerIsQuiet()
        {
            var model = GetModel(() => (
                from q in new QueriableDummy<dummyntup>()
                select q.vals.Count()).Aggregate(0, (acc, va) => acc + va));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new TypeHandlerCache());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            int originalCount = MEFUtilities.CountParts();

            qv.VisitQueryModel(model);

            Assert.AreEqual(originalCount, MEFUtilities.CountParts(), "# of parts in MEF container");
            Assert.Inconclusive("Can't figure out how to count the # of parts");
        }
#endif

        [TestMethod]
        public void TestLetOperator()
        {
            var model = GetModel(() => (
                from q in new QueriableDummy<dummyntup>()
                let qtest = new ROOTNET.NTLorentzVector(q.run, q.run, q.run)
                from qvlist in q.vals
                select qvlist + qtest.Pt()).Aggregate(0, (acc, va) => acc + (int)va));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new TypeHandlerROOT());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(dummyntup) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(model);
        }

        [TestMethod]
        public void TestSetMainLoopVariable()
        {
            var model = GetModel(() => (new QueriableDummy<dummyntup>().Count()));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(dummyntup) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(model);

            Assert.IsNotNull(cc.LoopVariable, "Loop variable is null!");
        }

        [TestMethod]
        public void TestCount()
        {
            var q = new QueriableDummy<ntup>();

            var r1 = q.Count();
            var query1 = DummyQueryExectuor.FinalResult;

            query1.DumpCodeToConsole();

            Assert.AreEqual(3, query1.CodeBody.CodeItUp().Count(), "# of lines of code"); // the {, "the addition", and the "}".
        }

        [TestMethod]
        public void TestCountOnArray()
        {
            var q = new QueriableDummy<ntupArray>();

            var r1 = from evt in q
                     select evt.run.Count();
            var r2 = r1.Sum();
            var query1 = DummyQueryExectuor.FinalResult;

            query1.DumpCodeToConsole();

            // With the .Count() on an identity query, this is optimized to just 3 lines of code.
            Assert.AreEqual(3, query1.CodeBody.CodeItUp().Count(), "# of lines of code"); // the {, "the addition", and the "}".
        }

        [TestMethod]
        public void TestCountOnArrayWithIf()
        {
            var q = new QueriableDummy<ntupArray>();

            var r1 = from evt in q
                     select evt.run.Count(i => i > 1);
            var r2 = r1.Sum();
            var query1 = DummyQueryExectuor.FinalResult;

            query1.DumpCodeToConsole();

            // With the .Count() on an identity query, this is optimized to just 3 lines of code.
            Assert.AreEqual(12, query1.CodeBody.CodeItUp().Count(), "# of lines of code"); // the {, "the addition", and the "}".
        }

        /// <summary>
        /// Dummy to test that the loop variable when we get here is actually pointing to the right thing!
        /// </summary>
        [Export(typeof(IQVCollectionResultOperator))]
        class TakeOperatorTestLoopVar : IQVCollectionResultOperator
        {

            public bool CanHandle(Type resultOperatorType)
            {
                return resultOperatorType == typeof(TakeResultOperator)
                    || resultOperatorType == typeof(SkipResultOperator);
            }

            public void ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                ///
                /// Look at the loop variable. It should be pointing to something that is going to loop
                /// over all the "vals"
                /// 

                Assert.AreEqual(typeof(int), _codeContext.LoopVariable.Type, "Loopvariable type");
            }


        }

        [TestMethod]
        public void TestTakeInSubQuery()
        {
            /// Make sure the non-var return Take works when in a sub-query expression.
            /// The take operator is funny b/c it is a result, but it returns nothing.
            /// So, for all operators like that the QV has to deal with this correctly.

            var model = GetModel(() => (
                from q in new QueriableDummy<dummyntup>()
                from j in q.vals.Take(1)
                select j).Aggregate(0, (acc, va) => acc + 1));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new TakeOperatorTestLoopVar());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(dummyntup) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            /// Note that the Assert takes place above, in the TakeOperatortestLoopVar test!

            qv.VisitQueryModel(model);
        }

        [TestMethod]
        public void TestSelectMany()
        {
            ///
            /// Make sure we can also use SelectMany directly, rather than always having to do the
            /// for loop.
            /// 

            var model = GetModel(() => (
                from q in new QueriableDummy<dummyntup>()
                select q.val2D.SelectMany(vs => vs).Count()
                ).Aggregate(0, (acc, va) => acc + 1));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new TypeHandlerCache());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(dummyntup) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            /// SelectMany is something that is auto-parsed by re-linq if we are using a recent
            /// enough query.

            qv.VisitQueryModel(model);
        }

        [TestMethod]
        public void TestSelectWithNewTuple()
        {
            ///
            /// Make sure we can also use SelectMany directly, rather than always having to do the
            /// for loop.
            /// 

            var model = GetModel(() => (
                from q in new QueriableDummy<dummyntup>()
                select new Tuple<int, int>(q.run, q.run)
                ).Aggregate(0, (acc, va) => acc + va.Item1));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new TypeHandlerCache());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(dummyntup) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            /// SelectMany is something that is auto-parsed by re-linq if we are using a recent
            /// enough query.

            qv.VisitQueryModel(model);
            gc.DumpCodeToConsole();

            var goodlines = gc.CodeBody.CodeItUp().Where(s => s.Contains("+((*this).run)")).Count();
            Assert.AreEqual(1, goodlines, "# of times the .run appears");
        }

        [TestMethod]
        public void TestSubQueryForStatements()
        {
            /// Make sure a sub query works correctly...

            var model = GetModel(() => (
                from q in new QueriableDummy<dummyntup>()
                from j in q.vals
                select j).Aggregate(0, (acc, va) => acc + 1));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new ROTakeSkipOperators());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(dummyntup) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(model);

            gc.DumpCodeToConsole();

            /// At the top level we assume there will be a loop over the vals.

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "Expecting only for loop at the top level");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(IBookingStatementBlock), "vector loop not compound");
            var outterfloop = gc.CodeBody.Statements.First() as IBookingStatementBlock;

            Assert.AreEqual(1, outterfloop.Statements.Count(), "inner loop statements not set correctly");
            Assert.AreEqual(0, outterfloop.DeclaredVariables.Count(), "no variables should have been declared in the for loop!");
            Assert.IsInstanceOfType(outterfloop.Statements.First(), typeof(Statements.StatementAggregate), "aggregate statement type");
        }

        [TestMethod]
        public void TestSubQueryForStatementsWithPlaneAdd()
        {
            /// Make sure a sub query works correctly...

            var model = GetModel(() => (
                from q in new QueriableDummy<dummyntup>()
                from j in q.vals
                select j).Aggregate(0, (acc, va) => acc + va));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new ROTakeSkipOperators());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(dummyntup) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(model);

            gc.DumpCodeToConsole();

            /// At the top level we assume there will be a loop over the vals.

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "Expecting only for loop at the top level");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(IBookingStatementBlock), "vector loop not compound");
            var outterfloop = gc.CodeBody.Statements.First() as IBookingStatementBlock;

            Assert.AreEqual(1, outterfloop.Statements.Count(), "inner loop statements not set correctly");
            Assert.AreEqual(0, outterfloop.DeclaredVariables.Count(), "no variables should have been declared in the for loop!");
            Assert.IsInstanceOfType(outterfloop.Statements.First(), typeof(Statements.StatementAggregate), "aggregate statement type");
            var ass = outterfloop.Statements.First() as Statements.StatementAggregate;
            Assert.IsFalse(ass.Expression.RawValue.Contains("(int)j"), "Expression seems to reference the linq variable name j: '" + ass.Expression.RawValue + "'");
        }

        [TestMethod]
        public void TestTakeInSubQueryForStatements()
        {
            var model = GetModel(() => (
                from q in new QueriableDummy<dummyntup>()
                from j in q.vals.Take(1)
                select j).Aggregate(0, (acc, va) => acc + 1));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new ROTakeSkipOperators());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(dummyntup) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(model);

            /// At the top level we assume there will be a loop over the vals.

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "Unexpected # of statements");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(IBookingStatementBlock), "vector loop not right");
            var outterfloop = gc.CodeBody.Statements.First() as IBookingStatementBlock;

            Assert.AreEqual(1, outterfloop.Statements.Count(), "inner loop statements not set correctly");
            Assert.AreEqual(0, outterfloop.DeclaredVariables.Count(), "no variables should have been declared in the for loop!");
            Assert.IsInstanceOfType(outterfloop.Statements.First(), typeof(Statements.StatementIfOnCount), "if on count incorrect");

            var ifcountStatement = outterfloop.Statements.First() as Statements.StatementIfOnCount;

            Assert.AreEqual(1, ifcountStatement.Statements.Count(), "expected the fill statement");
            Assert.IsInstanceOfType(ifcountStatement.Statements.First(), typeof(Statements.StatementAggregate), "Assign statement not there");
        }

        public class subNtupleObjects1
        {
            [TTreeVariableGrouping]
            public int var1;
            [TTreeVariableGrouping]
            public double var2;
            [TTreeVariableGrouping]
            [RenameVariable("var3")]
            public double v3;
        }

        public class subNtupleObjects2
        {
            [TTreeVariableGrouping]
            public int var4;
            [TTreeVariableGrouping]
            public double var5;
            [TTreeVariableGrouping]
            [RenameVariable("var6")]
            public double v6;
        }

        [TranslateToClass(typeof(ntupWithObjectsDest))]
        public class ntupWithObjects
        {
            [TTreeVariableGrouping]
            public subNtupleObjects1[] jets;
            [TTreeVariableGrouping]
            public subNtupleObjects2[] tracks;
        }

        public class ntupWithObjectsDest : IExpressionHolder
        {
            public ntupWithObjectsDest(Expression expr)
            {
                HeldExpression = expr;
            }
            public int[] var1;
            public double[] var2;
            public double[] var3;

            public int[] var4;
            public double[] var5;
            public double[] var6;

            public Expression HeldExpression { get; private set; }
        }

        [TestMethod]
        public void TestSortSimple()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r = from evt in q
                    select (from v in evt.var1
                            orderby v
                            select v).Take(2).Sum();
            var r1 = from evt in r
                     where evt > 10
                     select evt;
            var r2 = r1.Count();
            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            // Look for the sorting somewhere in here...
            bool sortThere = query1.CodeBody.CodeItUp().Where(s => s.Contains("sort")).Any();
            Assert.IsTrue(sortThere, "No sort call in the code");
        }

        [TestMethod]
        public void TestNestedFilterSort()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r = from evt in q
                    let ns = evt.var1.Where(x => x > 1).OrderBy(x => x)
                    select ns;
            var r1 = from lst in r
                     from x in lst
                     select x;
            var res = r1.Count();
            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            var initalLines = query1.DumpCode().TakeWhile(l => !l.Contains("vector<int> aInt"));
            var openCount = initalLines.Where(l => l.Contains("{")).Count();
            var closeCount = initalLines.Where(l => l.Contains("}")).Count();

            Assert.AreEqual(openCount - 1, closeCount, "# of close brackets before vector<int> decl");
        }

        [TestMethod]
        public void TestNestedSorts()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r = from evt in q
                    let ns = evt.var1.Where(x => x > 1).OrderBy(x => x)
                    select new
                    {
                        NS = ns,
                        V = evt.var2
                    };

            var r0 = from lst in r
                     select new
                     {
                         NS = lst.NS,
                         V = lst.V.Where(v => v > 1.0).OrderBy(x => x)
                     };

            var r1 = from lst in r0
                     from n in lst.NS
                     from v in lst.V
                     select n + v;
            var res = r1.Sum();
            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            var initalLines = query1.DumpCode().TakeWhile(l => !l.Contains("vector<double> aDouble"));
            var openCount = initalLines.Where(l => l.Contains("{")).Count();
            var closeCount = initalLines.Where(l => l.Contains("}")).Count();

            Assert.AreEqual(openCount - 4, closeCount, "# of close brackets before vector<int> decl");
        }

        [TestMethod]
        public void TestSortSimpleCombine()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r = from evt in q
                    select (from v in evt.var1
                            orderby v
                            select v).Take(2).Sum();
            var r1 = from evt in r
                     where evt > 10
                     select evt;
            var r2 = r1.Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var rr = from evt in q
                     select (from v in evt.var1
                             orderby v
                             select v).Take(2).Sum();
            var rr1 = from evt in rr
                      where evt > 10
                      select evt;
            var rr2 = rr1.Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query2, query1);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            var st = query.QueryCode().First();
            Assert.AreEqual(query1.CodeBody.Statements.Count(), st.Statements.Count(), "# of statements");
            CompareNumbersOfStatements(query1.CodeBody.Statements, st.Statements, 2);
        }

        /// <summary>
        /// Look at the statements, make sure they are all the same size.
        /// </summary>
        /// <param name="iEnumerable"></param>
        /// <param name="iEnumerable_2"></param>
        private void CompareNumbersOfStatements(System.Collections.Generic.IEnumerable<IStatement> sExpected, System.Collections.Generic.IEnumerable<IStatement> sActual, int statementsToCheck)
        {
            Assert.AreEqual(sExpected.Count(), sActual.Count(), "# of statements incorrect");
            int count = 0;
            foreach (var sPair in sExpected.Zip(sActual, (ae, aa) => Tuple.Create(ae, aa)))
            {
                count += 1;
                Assert.AreEqual(sPair.Item1.GetType(), sPair.Item2.GetType(), "Statement type mis-match");
                if (sPair.Item1 is IStatementCompound)
                {
                    if (statementsToCheck >= count)
                        CompareNumbersOfStatements((sPair.Item1 as IStatementCompound).Statements, (sPair.Item2 as IStatementCompound).Statements, 1000);
                }
            }
        }

        [TestMethod]
        public void TestSortTranslatedObjects()
        {
            var q = new QueriableDummy<ntupWithObjects>();
            var r = from evt in q
                    select (from j in evt.jets
                            orderby j.var1
                            select j).Take(2).Sum(js => js.var1);

            var r2 = r.Sum();
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestGroupSimple()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r = from evt in q
                    select from v in evt.var1 group v by v;

            var cnt = from evt in r
                      from grp in evt
                      where grp.Key == 2
                      select grp.Key;

            var final = cnt.Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestGroupTranslatedGroup()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var r = from evt in q
                    select from v in evt.jets group v by v.var1;

            var cnt = from evt in r
                      from grp in evt
                      where grp.Key == 2 && grp.Count() == 1
                      select grp.Key;
            var final = cnt.Count();
            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestGroupSimpleCombine()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r1 = from evt in q
                     select from v in evt.var1 group v by v;

            var cnt1 = from evt in r1
                       from grp in evt
                       where grp.Key == 2
                       select grp.Key;

            var f1 = cnt1.Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var r2 = from evt in q
                     select from v in evt.var1 group v by v;

            var cnt2 = from evt in r2
                       from grp in evt
                       where grp.Key == 2
                       select grp.Key;

            var f2 = cnt2.Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query2, query1);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            var st = query.QueryCode().First();
            Assert.AreEqual(query1.CodeBody.Statements.Count(), st.Statements.Count(), "# of statements");
            CompareNumbersOfStatements(query1.CodeBody.Statements, st.Statements, 1);
        }

        [TestMethod]
        public void TestGroupLongRangeCombine()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();
            var dudeQ1 = from evt in q
                         from v in evt.var1
                         group v by v into lists
                         from i in lists
                         where i == 5
                         select i;
            var r1 = dudeQ1.Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var dudeQ2 = from evt in q
                         from v in evt.var1
                         group v by v into lists
                         from i in lists
                         where i == 5
                         select i;

            var r2 = dudeQ2.Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query2, query1);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            var st = query.QueryCode().First();
            Assert.AreEqual(query1.CodeBody.Statements.Count(), st.Statements.Count(), "# of statements");
            CompareNumbersOfStatements(query1.CodeBody.Statements, st.Statements, 1);
        }

        [TestMethod]
        public void TestGroupLongRange()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();
            var dudeQ = from evt in q
                        from v in evt.var1
                        group v by v into lists
                        from i in lists
                        where i == 5
                        select i;

            var r = dudeQ.Count();
            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestGroupAndCount()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r = from evt in q
                    select from v in evt.var1 group v by v;

            var cnt = from evt in r
                      from grp in evt
                      where grp.Key == 2 && grp.Count() > 5
                      select grp.Key;

            var final = cnt.Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestGroupAndCutAndCount()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r = from evt in q
                    select from v in evt.var1 group v by v;

            var cnt = from evt in r
                      from grp in evt
                      where grp.Key == 2 && grp.Where(v => v > 1).Count() > 5
                      select grp.Key;

            var final = cnt.Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestSortGroupByKey()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();
            var dudeQ = from evt in q
                        select (from v in evt.var1
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         select (from grp in evt
                                 orderby grp.Key descending
                                 select grp).First();

            var dudeQ2 = from evt in dudeQ1
                         where evt.Key == 10 && evt.Count() == 1
                         select evt;

            var dudq = dudeQ2.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestSortGroupByItems()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();
            var dudeQ = from evt in q
                        select (from v in evt.var1
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         select (from grp in evt
                                 where grp.Count() >= 5
                                 where grp.OrderBy(s => s).First() == 12
                                 select grp);

            var dudeQ2 = from evt in dudeQ1
                         where evt.Count() > 1
                         select evt;
            var r = dudeQ2.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestSortGroupByItemsWithFilter()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();
            var dudeQ = from evt in q
                        select (from v in evt.var1.Where(l => l > 2)
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         select (from grp in evt
                                 where grp.Count() >= 5
                                 where grp.OrderBy(s => s).First() == 12
                                 select grp);

            var dudeQ2 = from evt in dudeQ1
                         where evt.Count() > 1
                         select evt;
            var r = dudeQ2.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            var lines = DummyQueryExectuor.FinalResult.DumpCode().TakeWhile(l => !l.Contains("for (map"));
            var openB = lines.Count(l => l.Contains("{"));
            var closeB = lines.Count(l => l.Contains("}"));

            Assert.AreEqual(openB - 1, closeB, "# of }");
        }

        [TestMethod]
        public void TestSortReverseSimple()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r = from evt in q
                    select (from v in evt.var1
                            orderby v descending
                            select v).Take(2).Sum();
            var r1 = from evt in r
                     where evt > 10
                     select evt;
            var r2 = r1.Count();
            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            // Look for the sorting somewhere in here...
            bool sortThere = query1.CodeBody.CodeItUp().Where(s => s.Contains("sort")).Any();
            Assert.IsTrue(sortThere, "No sort call in the code");
        }

        [TestMethod]
        public void TestTranslatedAggregate()
        {
            var model = GetModel(() => (
                from q in new QueriableDummy<ntupWithObjects>()
                select q).Aggregate(0, (acc, va) => acc + va.jets.Sum(j => j.var1)));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROSum());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            var myth = new TypeHandlerCache();
            MEFUtilities.AddPart(myth);
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(ntupWithObjects) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);
            qv.VisitQueryModel(model);

            gc.DumpCodeToConsole();

            Assert.IsFalse(gc.CodeBody.CodeItUp().Where(s => s.Contains("jets")).Any(), "A line contains the word jets");
        }

        [TestMethod]
        public void TestTranslatedUsesRightSizeIndex()
        {
            var model = GetModel(() => (
                from q in new QueriableDummy<ntupWithObjects>()
                select q).Aggregate(0, (acc, va) => acc + va.jets.Sum(j => j.var1)));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROSum());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            var myth = new TypeHandlerCache();
            MEFUtilities.AddPart(myth);
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(ntupWithObjects) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);
            qv.VisitQueryModel(model);

            gc.DumpCodeToConsole();

            Assert.IsTrue(gc.CodeBody.CodeItUp().Where(s => s.Contains("var3).size()")).Any(), "A line contains the word jets");
        }

        [TestMethod]
        public void TestTranslatedUsesRightSizeIndex2()
        {
            var model = GetModel(() => (
                from q in new QueriableDummy<ntupWithObjects>()
                select q).Aggregate(0.0, (acc, va) => acc + va.jets.Sum(j => j.var2)));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROSum());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            var myth = new TypeHandlerCache();
            MEFUtilities.AddPart(myth);
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(ntupWithObjects) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);
            qv.VisitQueryModel(model);

            gc.DumpCodeToConsole();

            Assert.IsTrue(gc.CodeBody.CodeItUp().Where(s => s.Contains("var3).size()")).Any(), "A line contains the word jets");
        }

        [TestMethod]
        public void TestTranslatedUsesRightSizeIndex3()
        {
            var model = GetModel(() => (
                from q in new QueriableDummy<ntupWithObjects>()
                select q).Aggregate(0.0, (acc, va) => acc + va.jets.Sum(j => j.v3)));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROSum());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            var myth = new TypeHandlerCache();
            MEFUtilities.AddPart(myth);
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(ntupWithObjects) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);
            qv.VisitQueryModel(model);

            gc.DumpCodeToConsole();

            Assert.IsTrue(gc.CodeBody.CodeItUp().Where(s => s.Contains("var3).size()")).Any(), "A line contains the word jets");
        }

        [TestMethod]
        public void TestTranslatedAggregateWhereSingle()
        {
            var model = GetModel(() => (new QueriableDummy<ntupWithObjects>()).Where(evt => evt.jets.Aggregate(0, (acc, va) => acc + va.var1) > 5).Count());

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROFirstLast());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            var myth = new TypeHandlerCache();
            MEFUtilities.AddPart(myth);
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(ntupWithObjects) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);
            qv.VisitQueryModel(model);

            gc.DumpCodeToConsole();

            Assert.IsFalse(gc.CodeBody.CodeItUp().Where(s => s.Contains("jets")).Any(), "A line contains the word jets");
        }

        [TestMethod]
        public void TestTranslatedNestedLoop()
        {
            var model = GetModel(() => (
                from q in new QueriableDummy<ntupWithObjects>()
                from j in q.jets
                select j.var1).Aggregate(0, (acc, va) => acc + va));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new ROTakeSkipOperators());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            var myth = new TypeHandlerCache();
            MEFUtilities.AddPart(myth);
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(ntupWithObjects) };
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            /// Note that the Assert takes place above, in the TakeOperatortestLoopVar test!

            qv.VisitQueryModel(model);

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "Expecting only for loop at the top level");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(IBookingStatementBlock), "vector loop not right");
            var outterfloop = gc.CodeBody.Statements.First() as IBookingStatementBlock;


            Assert.AreEqual(0, gc.CodeBody.DeclaredVariables.Count(), "Declared variables at the outside loop (the agragate var)");

            Assert.AreEqual(1, outterfloop.Statements.Count(), "inner loop statements not set correctly");
            Assert.AreEqual(0, outterfloop.DeclaredVariables.Count(), "no variables should have been declared in the for loop!");
            Assert.IsInstanceOfType(outterfloop.Statements.First(), typeof(Statements.StatementAggregate), "assignment statement missing");

            var ass = outterfloop.Statements.First() as Statements.StatementAggregate;
            Assert.IsFalse(ass.Expression.RawValue.Contains("jets"), "jets should be missing from the expression - " + ass.Expression.RawValue);
        }

        class TestTranslatedNestedCompareAndSortHolder
        {
            public subNtupleObjects1 jet { get; set; }
            public subNtupleObjects2 track { get; set; }
        }

        class TestTranslatedNestedCompareAndSortHolderEvent
        {
            public IEnumerable<TestTranslatedNestedCompareAndSortHolder> matches;
        }


        /// <summary>
        /// THis comes from a bug in the wild. Two objects that were "close" to each other, look for the second one to do something with it,
        /// and it produced some bad code.
        /// </summary>
        [TestMethod]
        public void TestTranslatedNestedCompareAndSort()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            // Create a dual object. Avoid anonymous objects just for the sake of it.
            var matched = from evt in q
                          select new TestTranslatedNestedCompareAndSortHolderEvent()
                          {
                              matches = from j in evt.jets
                                        orderby j.v3 ascending
                                        select new TestTranslatedNestedCompareAndSortHolder()
                                        {
                                            jet = j,
                                            track = (from t in evt.tracks
                                                     orderby Math.Abs(t.v6 - j.v3) ascending
                                                     select t).First()
                                        }
                          };

            // Filter on the first jet in the sequence.
            var goodmatched = from evt in matched
                              where evt.matches.First().jet.v3 > 0
                              select evt;

            // Do something with the second one now
            var otherTrack = from evt in goodmatched
                             where evt.matches.Skip(1).First().track.v6 > 0
                             select evt.matches.Skip(1).First().jet.v3;

            //var r = matched.Where(evt => evt.matches.Where(m => m.track.v6 > 2.0).Count() > 5).Count();
            var r = otherTrack.Sum();

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            // This was crashing, but does need to be fixed up.
        }

        [TestMethod]
        public void TestLambdaExpressionLookup()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            Expression<Func<subNtupleObjects1, bool>> checker = s => s.var1 == 0;

            var result = q.SelectMany(evt => evt.jets).Where(checker).Count();

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            MakeSureNoVariable(code.CodeBody, "evt");
            MakeSureNoVariable(code.CodeBody, "s");
        }

        [TestMethod]
        public void TestTranslatedObjectCompareNE()
        {
            var q = new QueriableDummy<ntupWithObjects>();
            var together = from evt in q
                           from j1 in evt.jets
                           from j2 in evt.jets
                           where j1 != j2
                           select j1.var1 + j2.var1;
            var result = together.Sum();

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            MakeSureNoVariable(code.CodeBody, "jets");
            MakeSureNoVariable(code.CodeBody, "j1");
            MakeSureNoVariable(code.CodeBody, "j2");
            MakeSureNoVariable(code.CodeBody, "evt");
        }

        [TestMethod]
        public void TestTranslatedObjectCompareEQ()
        {
            var q = new QueriableDummy<ntupWithObjects>();
            var together = from evt in q
                           from j1 in evt.jets
                           from j2 in evt.jets
                           where j1 == j2
                           select j1.var1 + j2.var1;
            var result = together.Sum();

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            MakeSureNoVariable(code.CodeBody, "jets");
            MakeSureNoVariable(code.CodeBody, "j1");
            MakeSureNoVariable(code.CodeBody, "j2");
            MakeSureNoVariable(code.CodeBody, "evt");
        }

        [TestMethod]
        public void TestAverageSimple()
        {
            // Make sure we can process it!
            var q = new QueriableDummy<ntupWithObjects>();
            var together = from evt in q
                           select evt.jets.Average(j => j.var1);
            var result = together.Sum();

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            var s1 = code.CodeBody.Statements.First() as IStatementLoop;
            var s2 = code.CodeBody.Statements.Skip(1).First();

            Assert.IsNotNull(s1, "loop");
            Assert.IsInstanceOfType(s2, typeof(Statements.StatementThrowIfTrue), "check for average not zero");
        }

        /// <summary>
        /// When the average is used with more than one level deep, it can generate x-check code that
        /// is invalid.
        /// </summary>
        [TestMethod]
        public void TestAverageWithFilter()
        {
            // Make sure we can process it!
            var q = new QueriableDummy<ntupWithObjects>();
            var together = from evt in q
                           select evt.jets.Where(j => j.var1 > 1).Average(j => j.var1);
            var result = together.Sum();

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            var linesbetween = code.DumpCode().SkipWhile(l => !l.Contains(">1")).TakeWhile(l => !l.Contains("==0"));
            Assert.AreEqual(2, linesbetween.Where(l => l.Contains("}")).Count(), "# of lines with a closing bracket");
        }

        /// <summary>
        /// When the average is used with more than one level deep, it can generate x-check code that
        /// is invalid.
        /// </summary>
        [TestMethod]
        public void TestAverageWithLoopFilter()
        {
            // Make sure we can process it!
            var q = new QueriableDummy<ntupWithObjects>();
            var together = from evt in q
                           select evt.jets.Where(j => j.var1 > evt.jets.Sum(p => p.var1)).Average(j => j.var1);
            var result = together.Sum();

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            var linesbefore = code.DumpCode().TakeWhile(l => !l.Contains("==0"));
            var closeBrace = linesbefore.Where(l => l.Contains("}")).Count();
            var openBrace = linesbefore.Where(l => l.Contains("{")).Count();

            Assert.AreEqual(openBrace - 1, closeBrace, "# of close braces");
        }

        [CPPHelperClass]
        public static class CPPHelperFunctions
        {
            [CPPCode(Code = new string[] { "Calc = arg*2;" })]
            public static int Calc(int arg)
            {
                throw new NotImplementedException();
            }

            [CPPCode(Code = new[] { "int dUnique = arg*2; Calc2 = dUnique;" })]
            public static int Calc2(int arg)
            {
                throw new NotImplementedException();
            }

            [CPPCode(Code = new[] { " Inc = a" })]
            public static ROOTNET.Interface.NTLorentzVector Inc(ROOTNET.Interface.NTLorentzVector a, int value)
            {
                throw new NotImplementedException();
            }
            [CPPCode(IncludeFiles = new string[] { "TLorentzVector.h" },
                Code = new string[]{
                "TLorentzVector tlzUnique;",
                "tlzUnique.SetPtEtaPhiE(pt, eta, phi, E);",
                "CreateTLZ = &tlzUnique;"
            })]
            public static ROOTNET.NTLorentzVector CreateTLZ(double pt, double eta, double phi, double E)
            {
                throw new NotImplementedException("This should never get called!");
#pragma warning disable 0162
                var tlz = new ROOTNET.NTLorentzVector();
                tlz.SetPtEtaPhiE(pt, eta, phi, E);
                return tlz;
#pragma warning disable 0162
            }
        }

        /// <summary>
        /// Caught in the wild. When you have an object that has to be calculated
        /// (i.e. TLZ), and you use two different methods, the QV and expression eval
        /// was missing that it was the same object - and thus creating two
        /// versions of it in the code where only one was needed. This fails if
        /// that happens.
        /// </summary>
        [TestMethod]
        public void TestCPPCodeOptimization()
        {
            var q = new QueriableDummy<dummyntup>();

            var resultA = from evt in q
                          select new
                          {
                              Jets = from r in evt.valC1D
                                     let s = CPPHelperFunctions.CreateTLZ(r, r, r, r)
                                     where s.M() > 5 && s.Pt() < 10
                                     select s
                          };
            var resultB = from evt in resultA
                          where (evt.Jets.Count() == 2)
                          select evt;
            var resultC = resultB.SelectMany(evt => evt.Jets).Where(j => j.M() > 7);
            var c = resultC.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            var lm = query.DumpCode().Where(l => l.Contains("TLorentzVector* ")).Count();
            Assert.AreEqual(2, lm, "Number of times TLorentzVector appears in the source");
        }

        /// <summary>
        /// When we make a method call, we cache the result. Make sure that when two guys cache the result
        /// they get combined properly, so the call is made only once.
        /// </summary>
        [TestMethod]
        public void TestMethodCallCacheCombine()
        {
            var q = new QueriableDummy<dummyntup>();

            var resultA = from evt in q
                          from r in evt.valC1D
                          let s = CPPHelperFunctions.CreateTLZ(r, r, r, r)
                          where s.M() > 5
                          select s;
            var r1 = resultA.Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var r2 = resultA.Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.DumpCode().Where(l => l.Contains("M()")).Count(), "# of times M() is called");
        }

        /// <summary>
        // This optimization came from looking at stack traces and heat maps... Found that
        // a large amount of time was wasted calling Phi() repeatedly - often on the same object.
        // ROOT does not cache the value, so since this involves a atan, this is quite expensive.
        // So, what we wnat to make sure is if we need Phi() twice, we only calculate it once.
        /// </summary>
        [TestMethod]
        public void TestMemberFunctionCalledTwiceOptimizedAway()
        {
            var q = new QueriableDummy<dummyntup>();

            var resultA = from evt in q
                          select new
                          {
                              Jets = from r in evt.valC1D
                                     let s = CPPHelperFunctions.CreateTLZ(r, r, r, r)
                                     where s.Phi() * s.Phi() > 5.0
                                     select s
                          };
            var resultC = resultA.SelectMany(evt => evt.Jets).Count();

            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            var lines = query.DumpCode().SelectMany(l => l.Split('(', ')', '.')).Where(s => s == "Phi").Count();
            Assert.AreEqual(1, lines, "# of Phi occurances");

            var phiLine = query.DumpCode().Where(l => l.Contains("Phi()")).FirstOrDefault();
            Assert.IsNotNull(phiLine, "no phi call line");
            Assert.IsTrue(phiLine.Trim().StartsWith("double"), "does not start with double decl: " + phiLine.Trim());
        }

        [TestMethod]
        public void TestSubQueryWithTranslationOutside()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            Expression<Func<subNtupleObjects1, bool>> checker = j => CPPHelperFunctions.Calc(j.var1) > 1;

            var tracksNearJetPerEvent = from evt in q
                                        select from j in evt.jets.AsQueryable().Where(checker)
                                               let jtlz = CPPHelperFunctions.Calc(j.var1)
                                               select new
                                               {
                                                   Jet = j,
                                                   Tracks = from t in evt.jets
                                                            let ttlz = CPPHelperFunctions.Calc(t.var1)
                                                            where ttlz > jtlz
                                                            select t
                                               };

            var tracksNearJet = from evt in tracksNearJetPerEvent
                                from j in evt
                                select j;

            var r = tracksNearJet.Aggregate(0, (s, evt) => s + evt.Tracks.Count());

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            MakeSureNoVariable(code.CodeBody, "evt");
            MakeSureNoVariable(code.CodeBody, "j");
            MakeSureNoVariable(code.CodeBody, "Jet");
            MakeSureNoVariable(code.CodeBody, "Tracks");
        }

        [TestMethod]
        public void TestSubQueryWithTranslationOutsideSimpler()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var tracksNearJetPerEvent = from evt in q
                                        select from j in evt.jets
                                               select new
                                               {
                                                   Tracks = from t in evt.jets
                                                            where t.var1 > j.var1
                                                            select t
                                               };

            var tracksNearJet = tracksNearJetPerEvent.SelectMany(e => e).SelectMany(e1 => e1.Tracks).Count();

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            MakeSureNoVariable(code.CodeBody, "evt");
            MakeSureNoVariable(code.CodeBody, "j");
            MakeSureNoVariable(code.CodeBody, "Jet");
            MakeSureNoVariable(code.CodeBody, "Tracks");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAnonObjectCompare()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var tracksNearJetPerEvent = from evt in q
                                        select from j in evt.jets
                                               select new
                                               {
                                                   Tracks = from t in evt.jets
                                                            where t.var1 > j.var1
                                                            select t
                                               };

            var allcombos = from evt in tracksNearJetPerEvent
                            select from j1 in evt
                                   from j2 in evt
                                   where j1 != j2
                                   select j1;

            var cnt = allcombos.Where(evt => evt.Count() > 5).Count();
            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            Assert.Inconclusive();
        }

        [TestMethod]
        public void TestSubQueryWithTranslationOutsideRenameBug()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            Expression<Func<subNtupleObjects1, bool>> checker = jr => CPPHelperFunctions.Calc(jr.var1) > 1;

            var tracksNearJetPerEvent = from evt in q
                                        select from j in evt.jets
                                               let jtlz = CPPHelperFunctions.Calc(j.var1)
                                               where jtlz > 1
                                               select new
                                               {
                                                   Jet = j,
                                                   Tracks = from t in evt.jets
                                                            let ttlz = CPPHelperFunctions.Calc(t.var1)
                                                            where ttlz > jtlz
                                                            select t
                                               };

            var tracksNearJet = from evt in tracksNearJetPerEvent
                                from jl in evt
                                select jl;

            var r = tracksNearJet.Aggregate(0, (s, evt) => s + evt.Tracks.Count());

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            MakeSureNoVariable(code.CodeBody, "evt");
            MakeSureNoVariable(code.CodeBody, "j");
            MakeSureNoVariable(code.CodeBody, "Jet");
            MakeSureNoVariable(code.CodeBody, "Tracks");
            MakeSureNoVariable(code.CodeBody, "jtlz");
            MakeSureNoVariable(code.CodeBody, "ttlz");
            MakeSureNoVariable(code.CodeBody, "jl");
            MakeSureNoVariable(code.CodeBody, "jr");
        }

        [TestMethod]
        public void TestAnonymousObjectOneLevelDown()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var tracksNearJetPerEvent = from evt in q
                                        select from j in evt.jets
                                               let jtlz = CPPHelperFunctions.Calc(j.var1)
                                               where jtlz > 1
                                               select new
                                               {
                                                   Jet = j,
                                                   Tracks = from t in evt.jets
                                                            let ttlz = CPPHelperFunctions.Calc(t.var1)
                                                            where ttlz > jtlz
                                                            select t
                                               };

            var tracksNearJet = from evt in tracksNearJetPerEvent
                                from jfin in evt
                                select jfin.Jet;

            var result = tracksNearJet.Where(js => js.var1 > 0).Count();


            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            MakeSureNoVariable(code.CodeBody, "evt");
            MakeSureNoVariable(code.CodeBody, "j");
            MakeSureNoVariable(code.CodeBody, "Stuff");

        }

        /// <summary>
        /// In code in the while we saw a crash when we tried to generate code from a 2-level
        /// deep anonymous object. This tests that situation.
        /// </summary>
        [TestMethod]
        public void TestAnonymousObjectTwoLevelsDown()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var all = from evt in q
                      select new
                      {
                          Jets = from v in evt.var1
                                 select new
                                 {
                                     V1 = Enumerable.Range(0, 10).Select(s => s * v),
                                     V2 = 2 * v
                                 },
                          Tracks = from v in evt.var2 select new { T1 = v, T2 = 2 * v }
                      };

            var combined = from evt in all
                           from j in evt.Jets
                           select j.V1.Where(l => l > 2).Count();

            var result = combined.Where(v => v > 1.0).Count();
            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestAnonymousObjectWithSameIndexNames()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var tracksNearJetPerEvent = from evt in q
                                        select from j in evt.jets
                                               let jtlz = CPPHelperFunctions.Calc(j.var1)
                                               where jtlz > 1
                                               select new
                                               {
                                                   Jet = j,
                                                   Tracks = from t in evt.jets
                                                            let ttlz = CPPHelperFunctions.Calc(t.var1)
                                                            where ttlz > jtlz
                                                            select t
                                               };

            var tracksNearJet = from evt in tracksNearJetPerEvent
                                from j in evt
                                select new
                                {
                                    Jet = j.Jet,
                                    Tracks = from t in j.Tracks where t.var2 < 2.2 select t
                                };


            var tracks = from evt in tracksNearJet from t in evt.Tracks select t;

            var count = tracks.Count();

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            MakeSureNoVariable(code.CodeBody, "evt");
            MakeSureNoVariable(code.CodeBody, "j");
            MakeSureNoVariable(code.CodeBody, "Stuff");

        }

        /// <summary>
        /// Check the code contains no reference to a variable by name!
        /// </summary>
        /// <param name="iBookingStatementBlock"></param>
        /// <param name="p"></param>
        private void MakeSureNoVariable(IStatement statements, string vname)
        {
            Regex finder = new Regex(string.Format(@"\b{0}\b", vname));
            var hasit = from l in statements.CodeItUp()
                        where finder.Match(l).Success
                        select l;

            Assert.AreEqual(0, hasit.Count(), string.Format("Some lines have '{0}' referenced", vname));
        }

        [TestMethod]
        public void TestQueryWithTwoRangeVariablesNamedSameThingTranslating()
        {
            var q = new QueriableDummy<ntupWithObjects>();
            var result1 = from evt in q
                          from jet in evt.jets
                          select jet;
            var result2 = from jet in result1
                          where jet.var1 > 0.0
                          select jet;
            var result3 = from jet in result2
                          where jet.var1 > 1.0
                          select jet;
            var c = result3.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            /// Looking for an infinite loop!
        }

        [TestMethod]
        public void TestQueryWithTwoRangeVariablesNamedSameThingTranslatingMainVar()
        {
            var q = new QueriableDummy<ntupWithObjects>();
            var result1 = from evt in q
                          where (from jet in evt.jets where jet.var1 > 1.0 select jet).Count() > 1
                          select evt;
            var result2 = from evt in result1
                          from jet in evt.jets
                          select jet;
            var result3 = from jet in result2
                          where jet.var1 > 1.0
                          select jet;
            var c = result3.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            /// Looking for an infinite loop!
        }

        [TestMethod]
        public void TestDoubleFunctionCall()
        {
            var q = new QueriableDummy<ntupWithObjects>();
            var r1 = from evt in q
                     select from j in evt.jets
                            select Math.Abs(ROOTNET.NTVector2.Phi_0_2pi(j.var1) - 3.0);
            var r2 = from evt in r1
                     where evt.Where(i => i > 2.0).Count() > 0
                     select evt;
            var c = r2.Count();

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            var theline = from l in DummyQueryExectuor.FinalResult.CodeBody.CodeItUp()
                          where l.Contains("Phi_0_2pi")
                          select l;
            var arr = theline.ToArray();
            Assert.AreEqual(1, arr.Length, "too many lines with function reference!");
            Assert.AreEqual(1, DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("std::abs")).Count(), "second function call not found");
        }

        [TestMethod]
        public void TestATan2Call()
        {
            var q = new QueriableDummy<ntupWithObjects>();
            var r1 = from evt in q
                     select from j in evt.jets
                            select Math.Atan2((double)j.var1, (double)j.var2);
            var r2 = r1.SelectMany(evt => evt).Where(c => c > 0.1).Count();
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            var theline = from l in DummyQueryExectuor.FinalResult.CodeBody.CodeItUp()
                          where l.Contains("std::atan2")
                          select l;
            var arr = theline.ToArray();
            Assert.AreEqual(1, arr.Length, "Expecting one reference to atan2!");
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

        [TestMethod]
        public void TestAggregateAsResultCombine()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var r1 = q.SelectMany(r => r.jets).Aggregate(0, (f, s) => s.var1 + f);
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.SelectMany(r => r.jets).Aggregate(0, (f, s) => s.var1 + f);
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "Number of query blocks");
            Assert.AreEqual(1, query.QueryCode().First().Statements.Count(), "# of statements");
            var statement = query.QueryCode().First().Statements.First() as IStatementCompound;
            Assert.IsNotNull(statement, "statement isn't a compound");
            Assert.AreEqual(2, statement.Statements.Count(), "# of inner statements");
        }

        [TestMethod]
        public void TestAggregateInternalCombine()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var r1 = q.Select(v => v.jets.Aggregate(0, (s, f) => s + f.var1)).Where(j => j > 5).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Select(v => v.jets.Aggregate(0, (s, f) => s + f.var1)).Where(j => j > 5).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "Number of query blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");
            var statement = query.QueryCode().First().Statements.First() as IStatementCompound;
            Assert.IsNotNull(statement, "statement isn't a compound");
            Assert.AreEqual(1, statement.Statements.Count(), "# of inner statements");
            var ifstatement = query.QueryCode().First().Statements.Skip(1).First() as IStatementCompound;
            Assert.IsNotNull(ifstatement, "if statement not right");
            Assert.AreEqual(2, ifstatement.Statements.Count(), "# of counts inside the if statement");
        }

        [TestMethod]
        public void TestAggregateInternalResultCombine()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var r1 = q.Where(evt => evt.jets.Aggregate(0, (s, f) => s + f.var1) > 5).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(evt => evt.jets.Aggregate(0, (s, f) => s + f.var1) > 5).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "Number of query blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        /// <summary>
        /// This test was generated to find a bug found in teh while. A multi-level deep Any/All statement used on a select anonymous
        /// object declared its index at a scope that wasn't useful for where the result was getting used.
        /// </summary>
        [TestMethod]
        public void TestAnyAllDeepInSelectSubQuery()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();

            var r1 = from e in q
                     select new
                     {
                         var1 = e.var1.Where(i => i > 1)
                     };

            var r2 = from e in r1
                     where e.var1.Count() == 2
                     select e;

            var r3 = from e in r2
                     select new
                     {
                         MatchedJets = from j in e.var1
                                       select new
                                       {
                                           N1 = j,
                                           N2 = (from j2 in e.var1
                                                 orderby j2 - j
                                                 select j2
                                                 ).First()
                                       }
                     };
            var r4 = from e in r3
                     select new
                     {
                         hasGluon = e.MatchedJets.Where(n => n.N2 > 2).Any(),
                         hasQuark = e.MatchedJets.All(j => j.N2 < 2)
                     };
            var res = r4.Where(e => e.hasGluon && e.hasQuark).Count();

            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();
            Assert.IsFalse(query.DumpCode().Where(l => l.Contains("!(((*(*this).var1).at(aInt32_13))<2)")).Any(), "Bad code in there");
        }

        [TestMethod]
        public void TestAnyCombine()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var r1 = q.Where(evt => evt.jets.Any(j => j.var1 > 5)).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(evt => evt.jets.Any(j => j.var1 > 5)).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "Number of query blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestAllCombine()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var r1 = q.Where(evt => evt.jets.All(j => j.var1 > 5)).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(evt => evt.jets.All(j => j.var1 > 5)).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "Number of query blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestInternalCountCombine()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var r1 = q.Where(evt => evt.jets.Count() > 3).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(evt => evt.jets.Count() > 3).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "Number of query blocks");
            Assert.AreEqual(1, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestFirstCombine()
        {
            var q = new QueriableDummy<dummyntup>();

            var r1 = q.Where(evt => evt.vals.First() > 5).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(evt => evt.vals.First() > 5).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            Assert.AreEqual(3, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestLastDefaultCombine()
        {
            var q = new QueriableDummy<dummyntup>();

            var r1 = q.Where(evt => evt.vals.LastOrDefault() > 5).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(evt => evt.vals.LastOrDefault() > 5).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            Assert.AreEqual(3, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestFirstDefaultCombine()
        {
            var q = new QueriableDummy<dummyntup>();

            var r1 = q.Where(evt => evt.vals.FirstOrDefault() > 5).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(evt => evt.vals.FirstOrDefault() > 5).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            Assert.AreEqual(3, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestMinMaxStatement()
        {
            var q = new QueriableDummy<ntupWithObjects>();
            var dudeQ1 = from evt in q
                         where (evt.jets.Max(j => j.var1) > 5)
                         select evt;
            var dude1 = dudeQ1.Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var dudeQ2 = from evt in q
                         where (evt.jets.Max(j => j.var1) > 5)
                         select evt;
            var dude2 = dudeQ2.Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements incorrect");
        }

        [TestMethod]
        public void TestPairWiseCombine()
        {
            var q = new QueriableDummy<ntupWithObjects>();
            var r1p = from evt in q
                      select evt.jets.PairWiseAll((j1, j2) => j1.var1 != j2.var1).Count();
            var r1 = r1p.Where(c => c > 2).Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var r2p = from evt in q
                      select evt.jets.PairWiseAll((j1, j2) => j1.var1 != j2.var1).Count();
            var r2 = r2p.Where(c => c > 2).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            var st = query.QueryCode().First();
            Assert.AreEqual(4, st.Statements.Count(), "# of statements");
        }

        /// <summary>
        /// Make sure we pop out to the right level here!
        /// </summary>
        [TestMethod]
        public void TestSumWithFilter()
        {
            var q = new QueriableDummy<ntupWithObjectsDest>();
            var r = from evt in q
                    let ns = evt.var1.Where(j => j > 1)
                    select ns.Sum();
            var r2 = r.Where(z => z > 10).Sum();
            var query2 = DummyQueryExectuor.FinalResult;
            query2.DumpCodeToConsole();

            var lines = query2.DumpCode().TakeWhile(l => !l.Contains("if (aInt"));
            var openCount = lines.Where(l => l.Contains("{")).Count();
            var closeCount = lines.Where(l => l.Contains("}")).Count();

            Assert.AreEqual(openCount - 1, closeCount, "# of close }");
        }

        [TestMethod]
        public void TestSumCombine()
        {
            var q = new QueriableDummy<ntupWithObjects>();
            var r1 = q.Where(evt => evt.jets.Sum(j => j.var1) > 10).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(evt => evt.jets.Sum(j => j.var1) > 10).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query Blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestAverageCombine()
        {
            // Make sure we can process it!
            var q = new QueriableDummy<ntupWithObjects>();
            var r1 = q.Where(evt => evt.jets.Average(j => j.var1) > 10).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(evt => evt.jets.Average(j => j.var1) > 10).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            Assert.AreEqual(3, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestTakeSkipCombine()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var r1p = from evt in q
                      let v = evt.jets.Skip(1).Count()
                      where v > 1
                      select v;
            var r1 = r1p.Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var r2p = from evt in q
                      let v = evt.jets.Skip(1).Count()
                      where v > 1
                      select v;
            var r2 = r2p.Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of guys");
        }

        [TestMethod]
        public void TestUnqiueCombineStatements()
        {
            var q = new QueriableDummy<ntupArray>();

            // Query #1

            var results1 = from evt in q
                           select evt.run.UniqueCombinations().Count();
            var total1 = results1.Aggregate(0, (seed, val) => seed + val);
            var query1 = DummyQueryExectuor.FinalResult;

            var results2 = from evt in q
                           select evt.run.UniqueCombinations().Count();
            var total2 = results2.Aggregate(0, (seed, val) => seed + val);
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            // Check that the combine actually worked well!!
            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            // First for loop to crord, 2 to test, and then the two aggregates
            Assert.AreEqual(4, query.QueryCode().First().Statements.Count(), "# of statements incorrect");
        }

        [TestMethod]
        public void TestSimpleLoopSTLVector()
        {
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select evt.vals.Where(s => s > 5).Count();
            var r = r1.Where(v => v > 5).Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains(".size()")).Any(), "missing size() call");
        }

        [TestMethod]
        public void TestSimpleLoopCVector()
        {
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select evt.valC1D.Where(s => s > 5).Count();
            var r = r1.Where(v => v > 5).Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains(".run")).Any(), "missing run reference");
        }

        [TestMethod]
        public void TestSimpleLoopCConstVector()
        {
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select evt.valC1DConst.Where(s => s > 5).Count();
            var r = r1.Where(v => v > 5).Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains("= 20")).Any(), "missing run reference");
        }

        [TestMethod]
        public void TestSimpleLoopC2DConstVector()
        {
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select evt.valC2D.Where(s => s.Count() > 5).Count();
            var r = r1.Where(v => v > 5).Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains("= 20")).Any(), "missing run reference");
            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains(".run")).Any(), "missing run reference");
        }

        [TestMethod]
        public void TestSimpleLoopEnumerableRange()
        {
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select (from i in Enumerable.Range(0, 20)
                             where evt.valC1DConst[i] > 5
                             select evt.valC1DConst[i]).Count();
            var r = r1.Where(v => v > 5).Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains("= 20")).Any(), "missing run reference");
        }

        [TestMethod]
        public void TestSimpleLoopEnumerableRangeCombine()
        {
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select (from i in Enumerable.Range(0, 20)
                             where evt.valC1DConst[i] > 5
                             select evt.valC1DConst[i]).Count();
            var r = r1.Where(v => v > 5).Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var r2 = r1.Where(v => v > 5).Count();
            var query2 = DummyQueryExectuor.FinalResult;
            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "Number of query blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");

            var forstatement = query.QueryCode().First().Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(forstatement, "for statement isn't a block!");
            Assert.AreEqual(1, forstatement.Statements.Count(), "# of statements in the for loop");

            var ifstatement = query.QueryCode().First().Statements.Skip(1).First() as IBookingStatementBlock;
            Assert.IsNotNull(ifstatement, "if statement pointer");
            Assert.AreEqual(2, ifstatement.Statements.Count(), "# of statements inside the if statememt"); // One for each fo the query results!
        }

        [TestMethod]
        public void TestSimpleLoopEnumerabelRangeWithVar()
        {
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select (from i in Enumerable.Range(0, evt.run)
                             where evt.valC1DConst[i] > 5
                             select evt.valC1DConst[i]).Count();
            var r = r1.Where(v => v > 5).Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains("= (*this).run")).Any(), "missing run reference");
        }

        [TestMethod]
        public void TestSimpleLoopEnumerabelRangeWithVarCombine()
        {
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select (from i in Enumerable.Range(0, evt.run)
                             where evt.valC1DConst[i] > 5
                             select evt.valC1DConst[i]).Count();
            var r = r1.Where(v => v > 5).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = r1.Where(v => v > 5).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "Number of query blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");

            var forstatement = query.QueryCode().First().Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(forstatement, "for statement isn't a block!");
            Assert.AreEqual(1, forstatement.Statements.Count(), "# of statements in the for loop");

            var ifstatement = query.QueryCode().First().Statements.Skip(1).First() as IBookingStatementBlock;
            Assert.IsNotNull(ifstatement, "if statement pointer");
            Assert.AreEqual(2, ifstatement.Statements.Count(), "# of statements inside the if statememt"); // One for each fo the query results!
        }

        [TestMethod]
        public void TestSimpleLoopEnumerabelRangeWithVarNZStart()
        {
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select (from i in Enumerable.Range(2, evt.run)
                             where evt.valC1DConst[i] > 5
                             select evt.valC1DConst[i]).Count();
            var r = r1.Where(v => v > 5).Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains("= (*this).run")).Any(), "missing run reference");
            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains("=2; a")).Any(), "missing lower limit reference");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestComplexEnumerableRangeLoop()
        {
            int[] seq = { 0, 2, 4, 6 };
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select (from i in seq
                             where evt.valC1DConst[i] > 5
                             select evt.valC1DConst[i]).Count();
            var r = r1.Where(v => v > 5).Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestSeqEnumerableRangeLoop()
        {
            int[] seq = { 0, 1, 2, 3, 4, 5 };
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select (from i in seq
                             where evt.valC1DConst[i] > 5
                             select evt.valC1DConst[i]).Count();
            var r = r1.Where(v => v > 5).Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains("= 6")).Any(), "missing upper limit reference");
            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains("=0; a")).Any(), "missing lower limit reference");
        }

        [TestMethod]
        public void TestSeqEnumerableRangeLoopWithNonZeroStart()
        {
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select (from i in Enumerable.Range(2, 6)
                             where evt.valC1DConst[i] > 5
                             select evt.valC1DConst[i]).Count();
            var r = r1.Where(v => v > 5).Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains("= 8")).Any(), "missing upper limit reference");
            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains("=2; a")).Any(), "missing lower limit reference");
        }

        [TestMethod]
        public void TestSimpleLoopEnumerableRangeNonZeroStart()
        {
            int[] seq = { 2, 3, 4, 5 };
            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     select (from i in seq
                             where evt.valC1DConst[i] > 5
                             select evt.valC1DConst[i]).Count();
            var r = r1.Where(v => v > 5).Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains("= 6")).Any(), "missing upper limit reference");
            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains("=2; a")).Any(), "missing lower limit reference");
        }

        /// <summary>
        /// A C++ array test.
        /// </summary>
        public class dummyntupCPP
        {
            public int nSize;

            [ArraySizeIndex("nSize")]
            public int[] vals;
        }

        [TestMethod]
        public void TestSimpleLoopCPPArray()
        {
            var q = new QueriableDummy<dummyntupCPP>();
            var r1 = from evt in q
                     select evt.vals.Where(s => s > 5).Count();
            var r = r1.Where(v => v > 5).Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.IsFalse(query1.CodeBody.CodeItUp().Where(s => s.Contains(".size()")).Any(), "size() should not be used");
            Assert.IsTrue(query1.CodeBody.CodeItUp().Where(s => s.Contains(".nSize")).Any(), "missing reference to the variale with the size");
        }

        [TestMethod]
        public void TestVectorLoopAnyCombine()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var r1 = q.SelectMany(t => t.jets).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.SelectMany(t => t.jets).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "Number of query blocks");
            Assert.AreEqual(1, query.QueryCode().First().Statements.Count(), "# of statements");
            var statement = query.QueryCode().First().Statements.First() as IStatementCompound;
            Assert.IsNotNull(statement, "statement isn't a compound");
            Assert.AreEqual(2, statement.Statements.Count(), "# of inner statements");
        }

        /// <summary>
        /// Check what happens when a Combine is called on several different simple loops.
        /// </summary>
        [TestMethod]
        public void TestSimpleOutterLoopCombine()
        {
            var q = new QueriableDummy<ntup>();

            var r1 = q.Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var r2 = q.Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);

            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "Number of query blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestSimpleIfCombine()
        {
            var q = new QueriableDummy<ntup>();

            var r1 = q.Where(f => f.run > 5).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(f => f.run > 5).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "Number of query blocks");
            Assert.AreEqual(1, query.QueryCode().First().Statements.Count(), "# of statements");
            var ifstatement = query.QueryCode().First().Statements.First() as Statements.StatementFilter;
            Assert.IsNotNull(ifstatement, "if statement");
            Assert.AreEqual(2, ifstatement.Statements.Count(), "# of statements inside if block");
        }

        [TestMethod]
        public void TestSimpleIfNotCombine()
        {
            var q = new QueriableDummy<ntup>();

            var r1 = q.Where(f => f.run > 5).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(f => f.run > 6).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "Number of query blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestCodeCombine()
        {
            var q = new QueriableDummy<ntup>();

            var r1 = q.Where(f => CPPHelperFunctions.Calc(f.run) > 5).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(f => CPPHelperFunctions.Calc(f.run) > 5).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestCodeCombineNested()
        {
            var q = new QueriableDummy<ntup>();

            var r1 = from f in q
                     let rs = CPPHelperFunctions.Calc(f.run)
                     where rs > 5 && rs < 10
                     select f;
            var r1c = r1.Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var r2 = from f in q
                     let rs = CPPHelperFunctions.Calc(f.run)
                     where rs > 5 && rs < 10
                     select f;
            var r2c = r2.Count();
            var query2 = DummyQueryExectuor.FinalResult;


            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestCodeCombineDoubleNested()
        {
            var q = new QueriableDummy<dummyntup>();

            var r1 = from f in q
                     select new
                     {
                         LS = from v in f.vals
                              let rs = CPPHelperFunctions.Calc(v)
                              where rs > 5 && rs < 10
                              select v
                     };
            var r1c = r1.Where(f => f.LS.Count() > 2).Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var r2 = from f in q
                     select new
                     {
                         LS = from v in f.vals
                              let rs = CPPHelperFunctions.Calc(v)
                              where rs > 5 && rs < 10
                              select v
                     };
            var r2c = r2.Where(f => f.LS.Count() > 2).Count();
            var query2 = DummyQueryExectuor.FinalResult;


            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        [TestMethod]
        public void TestCodeWithUniqueCombine()
        {
            var q = new QueriableDummy<ntup>();

            var r1 = q.Where(f => CPPHelperFunctions.Calc2(f.run) > 5).Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = q.Where(f => CPPHelperFunctions.Calc2(f.run) > 5).Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            Assert.AreEqual(2, query.QueryCode().First().Statements.Count(), "# of statements");
        }

        class ntupArray
        {
#pragma warning disable 0649
            public int[] run;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestCountTranslated()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            var r = q.Where(evt => evt.jets.Count() > 0).Count();
            var query = DummyQueryExectuor.FinalResult;

            query.DumpCodeToConsole();
            var code = query.QueryCode().First();

            MakeSureNoVariable(code, "jets");
        }

        class SelectionObject
        {
            public Expression<Func<ntupWithObjects, bool>> Selection
            {
                get
                {
                    return evt => evt.jets.Count() > 0;
                }
            }
            public Expression<Func<subNtupleObjects1, bool>> SelectionJet
            {
                get
                {
                    return jet => jet.var1 > 0;
                }
            }
        }

        [TestMethod]
        public void TestQueryUsesPropertyFunction()
        {
            // In the wild there is a case where a property which returns a function can't be used
            // in a Where call. Looking for a crash.

            var q = new QueriableDummy<ntupWithObjects>();

            var obj = new SelectionObject();
            var r = q.Where(obj.Selection).Count();
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestQueryUsesPropertyFunction2()
        {
            // In the wild there is a case where a property which returns a function can't be used
            // in a Where call. Looking for a crash.

            var q = new QueriableDummy<ntupWithObjects>();

            var obj = new SelectionObject();
            Expression<Func<ntupWithObjects, subNtupleObjects1>> test = evt => evt.jets.AsQueryable().Where(obj.SelectionJet).First();
            var r = q.Where(evt => test.Invoke(evt).var1 > 0).Count();
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();
        }

#if notyet
        [TestMethod]
        public void TestAggragateTypeSaftey()
        {
            // Make sure that we can infer things from interface to object.

            var q = new QueriableDummy<ntupWithObjects>();
            var value = q.Aggregate(new ROOTNET.NTLorentzVector(0.0, 0.0, 0.0, 0.0) as ROOTNET.Interface.NTLorentzVector,
                (a, v) => CPPHelperFunctions.Inc(a, v.jets[0].var1));
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();
        }
#endif
    }
}
