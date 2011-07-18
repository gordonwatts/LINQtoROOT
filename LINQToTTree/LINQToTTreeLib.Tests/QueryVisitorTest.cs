// <copyright file="QueryVisitorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
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
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;
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
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
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
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
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
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(model);

            Assert.IsNotNull(cc.LoopVariable, "Loop variable is null!");
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

            /// <summary>
            /// Dummy return for a variable and sequencer accessor.
            /// </summary>
            class dummyvar : IVariable
            {
                public string VariableName
                {
                    get { return "anint_1234"; }
                }

                public IValue InitialValue
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                    set
                    {
                        throw new NotImplementedException();
                    }
                }

                public bool Declare
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                    set
                    {
                        throw new NotImplementedException();
                    }
                }

                public string RawValue
                {
                    get { return "dude[i]"; }
                }

                public Type Type
                {
                    get { return typeof(int); }
                }
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
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
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
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            /// SelectMany is something that is auto-parsed by re-linq if we are using a recent
            /// enough query.

            qv.VisitQueryModel(model);
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
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(model);

            gc.DumpCodeToConsole();

            /// At the top level we assume there will be a loop over the vals.

            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "Expecting only for loop at the top level");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.Skip(1).First(), typeof(IBookingStatementBlock), "vector loop not compound");
            var outterfloop = gc.CodeBody.Statements.Skip(1).First() as IBookingStatementBlock;

            Assert.AreEqual(1, outterfloop.Statements.Count(), "inner loop statements not set correctly");
            Assert.AreEqual(0, outterfloop.DeclaredVariables.Count(), "no variables should have been declared in the for loop!");
            Assert.IsInstanceOfType(outterfloop.Statements.First(), typeof(Statements.StatementAssign), "aggregate statement type");
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
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(model);

            gc.DumpCodeToConsole();

            /// At the top level we assume there will be a loop over the vals.

            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "Expecting only for loop at the top level");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.Skip(1).First(), typeof(IBookingStatementBlock), "vector loop not compound");
            var outterfloop = gc.CodeBody.Statements.Skip(1).First() as IBookingStatementBlock;

            Assert.AreEqual(1, outterfloop.Statements.Count(), "inner loop statements not set correctly");
            Assert.AreEqual(0, outterfloop.DeclaredVariables.Count(), "no variables should have been declared in the for loop!");
            Assert.IsInstanceOfType(outterfloop.Statements.First(), typeof(Statements.StatementAssign), "aggregate statement type");
            var ass = outterfloop.Statements.First() as Statements.StatementAssign;
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
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(model);

            /// At the top level we assume there will be a loop over the vals.

            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "Unexpected # of statements");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.Skip(1).First(), typeof(IBookingStatementBlock), "vector loop not right");
            var outterfloop = gc.CodeBody.Statements.Skip(1).First() as IBookingStatementBlock;

            Assert.AreEqual(2, outterfloop.Statements.Count(), "inner loop statements not set correctly");
            Assert.AreEqual(0, outterfloop.DeclaredVariables.Count(), "no variables should have been declared in the for loop!");
            Assert.IsInstanceOfType(outterfloop.Statements.First(), typeof(Statements.StatementIncrementInteger), "first loop statemen tis funny");
            Assert.IsInstanceOfType(outterfloop.Statements.Skip(1).First(), typeof(Statements.StatementIfOnCount), "if on count incorrect");

            var incStatement = outterfloop.Statements.First() as Statements.StatementIncrementInteger;
            var ifcountStatement = outterfloop.Statements.Skip(1).First() as Statements.StatementIfOnCount;

            Assert.AreEqual(1, ifcountStatement.Statements.Count(), "expected the fill statement");
            Assert.IsInstanceOfType(ifcountStatement.Statements.First(), typeof(Statements.StatementAssign), "Assign statement not there");
        }

        public class subNtupleObjects
        {
            [TTreeVariableGrouping]
            public int var1;
            [TTreeVariableGrouping]
            public double var2;
        }

        [TranslateToClass(typeof(ntupWithObjectsDest))]
        public class ntupWithObjects
        {
            [TTreeVariableGrouping]
            public subNtupleObjects[] jets;
        }

        public class ntupWithObjectsDest : IExpressionHolder
        {
            public ntupWithObjectsDest(Expression expr)
            {
                HeldExpression = expr;
            }
            public int[] var1;

            public Expression HeldExpression { get; private set; }
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
            var myth = new TypeHandlerCache();
            MEFUtilities.AddPart(myth);
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            /// Note that the Assert takes place above, in the TakeOperatortestLoopVar test!

            qv.VisitQueryModel(model);

            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "Expecting only for loop at the top level");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.Skip(1).First(), typeof(IBookingStatementBlock), "vector loop not right");
            var outterfloop = gc.CodeBody.Statements.Skip(1).First() as IBookingStatementBlock;


            Assert.AreEqual(1, gc.CodeBody.DeclaredVariables.Count(), "Declared variables at the outside loop (the agragate var)");

            Assert.AreEqual(1, outterfloop.Statements.Count(), "inner loop statements not set correctly");
            Assert.AreEqual(0, outterfloop.DeclaredVariables.Count(), "no variables should have been declared in the for loop!");
            Assert.IsInstanceOfType(outterfloop.Statements.First(), typeof(Statements.StatementAssign), "assignment statement missing");

            var ass = outterfloop.Statements.First() as Statements.StatementAssign;
            Assert.IsFalse(ass.Expression.RawValue.Contains("jets"), "jets should be missing from the expression - " + ass.Expression.RawValue);
        }

        [TestMethod]
        public void TestLambdaExpressionLookup()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            Expression<Func<subNtupleObjects, bool>> checker = s => s.var1 == 0;

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

        [CPPHelperClass]
        public static class CPPHelperFunctions
        {
            [CPPCode(Code = new string[] { "Calc = arg*2;" })]
            public static int Calc(int arg)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void TestSubQueryWithTranslationOutside()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            Expression<Func<subNtupleObjects, bool>> checker = j => CPPHelperFunctions.Calc(j.var1) > 1;

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
        public void TestSubQueryWithTranslationOutsideRenameBug()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            Expression<Func<subNtupleObjects, bool>> checker = j => CPPHelperFunctions.Calc(j.var1) > 1;

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
        public void TestGroupingWithAnonymousObjectOneLevelDown()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            Expression<Func<subNtupleObjects, bool>> checker = j => CPPHelperFunctions.Calc(j.var1) > 1;

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
        /// Check the code contains no reference to a variable by name!
        /// </summary>
        /// <param name="iBookingStatementBlock"></param>
        /// <param name="p"></param>
        private void MakeSureNoVariable(IBookingStatementBlock statements, string vname)
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
        public void TestAsQueriable()
        {
            // Reset variable counter
            TypeUtils._variableNameCounter = 0;

            var q = new QueriableDummy<dummyntup>();
            var r1 = from evt in q
                     from my in evt.vals.AsQueryable().Where(n => n > 5)
                     select my;
            var r = r1.Count();
            var asQ = DummyQueryExectuor.FinalResult;
            asQ.DumpCodeToConsole();

            // Reset variable counter
            TypeUtils._variableNameCounter = 0;

            var r2 = from evt in q
                     from my in evt.vals.Where(n => n > 5)
                     select my;
            var rI = r2.Count();
            var asNQ = DummyQueryExectuor.FinalResult;

            foreach (var l in asQ.CodeBody.CodeItUp().Zip(asNQ.CodeBody.CodeItUp(), (l1, l2) => Tuple.Create(l1, l2)))
            {
                Assert.AreEqual(l.Item1, l.Item2, "Line mis-match");
            }

        }

        [TestMethod]
        public void TestNewAnoymousObject()
        {
            var q = new QueriableDummy<dummyntup>();
            var firstR = from evt in q
                         select (from my in evt.vals.AsQueryable().Where(n => n > 5)
                                 select new
                                 {
                                     EVT = evt,
                                     MY = my
                                 });
            var r1 = from evt in firstR
                     select (from r in evt where r.MY > 6 select r.MY);

            var res = r1.Aggregate(0, (s, r) => s + r.Count());

            var result = DummyQueryExectuor.FinalResult;
            result.DumpCodeToConsole();

            Regex tfinder = new Regex(@"\bMY\b");
            var foundT = from l in result.CodeBody.CodeItUp()
                         where tfinder.Match(l).Success
                         select l;
            Assert.AreEqual(0, foundT.Count(), "No lines should have contained any expression involving MY!");
        }

        [TestMethod]
        public void TestAnonymouseArrayLoop()
        {
            var q = new QueriableDummy<dummyntup>();
            var firstR = from evt in q
                         select (from my in evt.vals.AsQueryable().Where(n => n > 5)
                                 select new
                                 {
                                     EVT = evt,
                                     MY = evt.vals.Where(j => j > 0)
                                 });
            var r1 = from evt in firstR
                     select (from r in evt where r.MY.Count() > 6 select r.MY);

            var res = r1.Aggregate(0, (s, r) => s + r.Count());

            var result = DummyQueryExectuor.FinalResult;
            result.DumpCodeToConsole();

            MakeSureNoVariable(result.CodeBody, "evt");
            MakeSureNoVariable(result.CodeBody, "q");
            MakeSureNoVariable(result.CodeBody, "my");
            MakeSureNoVariable(result.CodeBody, "n");
            MakeSureNoVariable(result.CodeBody, "EVT");
            MakeSureNoVariable(result.CodeBody, "MY");
            MakeSureNoVariable(result.CodeBody, "j");
            MakeSureNoVariable(result.CodeBody, "r");
        }

        [TestMethod]
        public void TestAnonymouseArrayLoopTwoDeep()
        {
            var q = new QueriableDummy<dummyntup>();
            var firstR = from evt in q
                         select (from my in evt.vals.AsQueryable().Where(n => n > 5)
                                 select new
                                 {
                                     EVT = evt,
                                     MY = evt.vals.Where(j => j > 0)
                                 });
            var r1 = from evt in firstR
                     select (from r in evt where r.MY.Count() > 6 select r);

            var res = r1.Aggregate(0, (s, r) => s + r.SelectMany(z => z.MY).Count());

            var result = DummyQueryExectuor.FinalResult;
            result.DumpCodeToConsole();

            MakeSureNoVariable(result.CodeBody, "evt");
            MakeSureNoVariable(result.CodeBody, "q");
            MakeSureNoVariable(result.CodeBody, "my");
            MakeSureNoVariable(result.CodeBody, "n");
            MakeSureNoVariable(result.CodeBody, "EVT");
            MakeSureNoVariable(result.CodeBody, "MY");
            MakeSureNoVariable(result.CodeBody, "j");
            MakeSureNoVariable(result.CodeBody, "r");
            MakeSureNoVariable(result.CodeBody, "s");
            MakeSureNoVariable(result.CodeBody, "z");
        }

        [TestMethod]
        public void TestSelectObject()
        {
            var q = new QueriableDummy<dummyntup>();
            var stup = from evt in q
                       select (from t in evt.vals.AsQueryable()
                               select new Tuple<int, int>(t, t));

            var r1 = from evt in stup
                     select (from r in evt where r.Item1 > 5 select r.Item1);

            var res = r1.Aggregate(0, (s, r) => s + r.Count());

            var result = DummyQueryExectuor.FinalResult;
            result.DumpCodeToConsole();

            // Make sure nothing silly happened, like "t" got in there somehow.

            Regex tfinder = new Regex(@"\bt\b");
            var foundT = from l in result.CodeBody.CodeItUp()
                         where tfinder.Match(l).Success
                         select l;
            Assert.AreEqual(0, foundT.Count(), "No lines should have contained any expression involving t!");
        }

        [TestMethod]
        public void TestQueryReferenceInSubQuery()
        {
            var q = new QueriableDummy<dummyntup>();
            var slist = from evt in q
                        select (from t in evt.vals select 2 * t);

            var r1 = from evt in slist
                     select (from r in evt where r > 5 select r);

            var res = r1.Aggregate(0, (s, r) => s + r.Count());

            var result = DummyQueryExectuor.FinalResult;
            result.DumpCodeToConsole();

            Regex tfinder = new Regex(@"\bt\b");
            var foundT = from l in result.CodeBody.CodeItUp()
                         where tfinder.Match(l).Success
                         select l;

            Assert.AreEqual(0, foundT.Count(), "No lines referencing t should be in there! Ack!");
        }

    }
}
