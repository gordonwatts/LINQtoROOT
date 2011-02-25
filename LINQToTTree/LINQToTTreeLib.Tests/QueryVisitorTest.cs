// <copyright file="QueryVisitorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq;
using System.Linq.Expressions;
using LINQToTTreeLib.ResultOperators;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Data.Linq;
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
            MEFUtilities.AddPart(new ROTakeSkipOperators());
            MEFUtilities.AddPart(new TypeHandlerCache());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc);
            MEFUtilities.Compose(qv);
            qv.MEFContainer = MEFUtilities.MEFContainer;

            qv.VisitQueryModel(model);

            Assert.Inconclusive("test not completed yet");
        }

    }
}
