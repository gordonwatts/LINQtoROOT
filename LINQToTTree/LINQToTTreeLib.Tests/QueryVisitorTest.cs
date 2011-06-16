// <copyright file="QueryVisitorTest.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
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

    }
}
