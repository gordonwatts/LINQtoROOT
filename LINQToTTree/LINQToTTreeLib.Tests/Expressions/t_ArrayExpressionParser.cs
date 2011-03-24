using System;
using System.Linq;
using System.Linq.Expressions;
using LINQToTTreeLib.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.Clauses.Expressions;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    ///This is a test class for TestArrayExpressionParser and is intended
    ///to contain all TestArrayExpressionParser Unit Tests
    ///</summary>
    [TestClass()]
    public class TestArrayExpressionParser
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestForNonArray()
        {
            ArrayExpressionParser.ParseArrayExpression(Expression.Variable(typeof(int), "d"));
        }

        [TestMethod]
        public void TestRunForNormalArray()
        {
            ArrayExpressionParser.ParseArrayExpression(Expression.Variable(typeof(int[]), "d"));
        }

        private QueryModel GetModel<T>(Expression<Func<T>> expr)
        {
            var parser = new QueryParser();
            return parser.GetParsedQuery(expr.Body);
        }

        class dummyntup
        {
            public int run;
            public int[] vals;
        }

        [TestMethod]
        public void TestSubQueryExpression()
        {
            //var model = GetModel(() => (from q in new QueriableDummy<dummyntup>() from j in q.vals.Take(1) select j).Aggregate(0, (acc, va) => acc + 1));

            var q = new dummyntup();
            q.vals = new int[] { 1, 2, 3, 4, 5 };
            var model = GetModel(() => (from j in q.vals select j).Take(1));
            var sq = new SubQueryExpression(model);

            var result = ArrayExpressionParser.ParseArrayExpression(sq);
            Assert.IsNotNull(result);

            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();

            var indexVar = result.AddLoop(gc, cc);
            Assert.IsNotNull(indexVar);
            Assert.AreEqual(0, gc.CodeBody.Statements.Count(), "No statement should have been added");
        }
    }
}
