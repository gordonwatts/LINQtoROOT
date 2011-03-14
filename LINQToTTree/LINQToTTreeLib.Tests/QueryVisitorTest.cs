// <copyright file="QueryVisitorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.ResultOperators;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Parsing.Structure;

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
            var parser = new QueryParser();
            return parser.GetParsedQuery(expr.Body);
        }

        public class dummyntup
        {
            public int run;
            public int[] vals;
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
            var qv = new QueryVisitor(gc, cc);
            MEFUtilities.Compose(qv);
            qv.MEFContainer = MEFUtilities.MEFContainer;

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
            var qv = new QueryVisitor(gc, cc);
            MEFUtilities.Compose(qv);
            qv.MEFContainer = MEFUtilities.MEFContainer;

            qv.VisitQueryModel(model);

            Assert.IsNotNull(cc.LoopVariable, "Loop variable is null!");
        }

#if false
        /// This test doesn't work b/c the Take is at teh top level.
        [TestMethod]
        public void TestBasicTakeOperator()
        {
            /// The take operator is funny b/c it is a result, but it returns nothing.
            /// So, for all operators like that the QV has to deal with this correctly.

            var model = GetModel(() => (new QueriableDummy<dummyntup>().Take(10).Count()));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROTakeSkipOperators());
            MEFUtilities.AddPart(new TypeHandlerCache());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc);
            MEFUtilities.Compose(qv);
            qv.MEFContainer = MEFUtilities.MEFContainer;

            qv.VisitQueryModel(model);

            /// Look for the if statement that is the test for the take.

            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "Incorrect # of statementes");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.Skip(1).First(), typeof(Statements.StatementIfOnCount), "take not implemented correctly");
            var takestatement = gc.CodeBody.Statements.Skip(1).First() as Statements.StatementIfOnCount;

            /// Make sure the count is inside the loop

            Assert.AreEqual(1, takestatement.Statements.Count(), "Expected the inc statement");
            Assert.IsInstanceOfType(takestatement.Statements.First(), typeof(Statements.StatementIncrementInteger), "inc statement not there");
        }
#endif

        /// <summary>
        /// Dummy to test that the loop variable when we get here is actually pointing to the right thing!
        /// </summary>
        [Export(typeof(IQVResultOperator))]
        class TakeOperatorTestLoopVar : IQVResultOperator
        {

            public bool CanHandle(Type resultOperatorType)
            {
                return resultOperatorType == typeof(TakeResultOperator)
                    || resultOperatorType == typeof(SkipResultOperator);
            }

            public IVariable ProcessResultOperator(Remotion.Data.Linq.Clauses.ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedCode _codeEnv, ICodeContext _codeContext, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                ///
                /// Look at the loop variable. It should be pointing to something that is going to loop
                /// over all the "vals"
                /// 

                Assert.AreEqual(typeof(int), _codeContext.LoopVariable.Type, "Loopvariable type");
                return new dummyvar();
            }

            /// <summary>
            /// Dummy return for a variable and sequencer accessor.
            /// </summary>
            class dummyvar : IVariable, ISequenceAccessor
            {
                public IVariable AddLoop(IGeneratedCode env, ICodeContext context, string indexName, Action<IVariableScopeHolder> popVariableContext)
                {
                    return new Variables.VarSimple(typeof(int));
                }

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
            var qv = new QueryVisitor(gc, cc);
            MEFUtilities.Compose(qv);
            qv.MEFContainer = MEFUtilities.MEFContainer;

            /// Note that the Assert takes place above, in the TakeOperatortestLoopVar test!

            qv.VisitQueryModel(model);
        }

        [TestMethod]
        public void TestTakeInSubQueryForStatements()
        {
            var model = GetModel(() => (
                from q in new QueriableDummy<dummyntup>()
                from j in q.vals.Take(1)
                select j).Aggregate(0, (acc, va) => acc + 1));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new ROTakeSkipOperators());
            MEFUtilities.AddPart(new TypeHandlerCache());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc);
            MEFUtilities.Compose(qv);
            qv.MEFContainer = MEFUtilities.MEFContainer;

            /// Note that the Assert takes place above, in the TakeOperatortestLoopVar test!

            qv.VisitQueryModel(model);

            /// At the top level we assume there will be a loop over the vals.

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "Expecting only for loop at the top level");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(Statements.StatementLoopOnVector), "vector loop not right");
            var outterfloop = gc.CodeBody.Statements.First() as Statements.StatementLoopOnVector;

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
            public int var1;
            public double var2;
        }

        public class ntupWithObjects
        {
            [TTreeVariableGrouping]
            public subNtupleObjects[] jets;
        }

        [TestMethod]
        public void TestObjectStacked()
        {
            var model = GetModel(() => (
                from q in new QueriableDummy<ntupWithObjects>()
                from j in q.jets
                select j.var1).Aggregate(0, (acc, va) => acc + va));

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new ROAggregate());
            MEFUtilities.AddPart(new ROTakeSkipOperators());
            MEFUtilities.AddPart(new TypeHandlerCache());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc);
            MEFUtilities.Compose(qv);
            qv.MEFContainer = MEFUtilities.MEFContainer;

            /// Note that the Assert takes place above, in the TakeOperatortestLoopVar test!

            qv.VisitQueryModel(model);

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "Expecting only for loop at the top level");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(Statements.StatementLoopOnVector), "vector loop not right");
            var outterfloop = gc.CodeBody.Statements.First() as Statements.StatementLoopOnVector;


            Assert.AreEqual(1, gc.CodeBody.DeclaredVariables.Count(), "Declared variables at the outside loop (the agragate var)");

            Assert.AreEqual(1, outterfloop.Statements.Count(), "inner loop statements not set correctly");
            Assert.AreEqual(0, outterfloop.DeclaredVariables.Count(), "no variables should have been declared in the for loop!");
            Assert.IsInstanceOfType(outterfloop.Statements.First(), typeof(Statements.StatementAssign), "assignment statement missing");

            var ass = outterfloop.Statements.First() as Statements.StatementAssign;
            Assert.IsFalse(ass.Expression.RawValue.Contains("jets"), "jets should be missing from the expression - " + ass.Expression.RawValue);
        }
    }
}
