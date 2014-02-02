using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.QMFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System.Linq.Expressions;
using System.Linq;

namespace LINQToTTreeLib.Tests.QMFunctions
{
    [TestClass]
    public class QMFuncSourceTest
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
        public void TypeOfNonSequence()
        {
            var qmb = new QueryModelBuilder();
            qmb.AddClause(new SelectClause(Expression.Constant(1.0)));
            qmb.AddResultOperator(new Remotion.Linq.Clauses.ResultOperators.FirstResultOperator(true));
            int[] list = new int[10];
            qmb.AddClause(new MainFromClause("r1", typeof(int), Expression.Constant(list)));

            var h = new QMFuncHeader() { Arguments = new object[] { }, IsSequence = false, QM = qmb.Build(), QMText = "hi" };
            var src = new QMFuncSource(h);
            Assert.AreEqual(typeof(double), src.ResultType, "result type");

            Assert.Inconclusive();
            //Assert.AreEqual(typeof(double), src.CacheVariable.Type, "Type of cache");
            Assert.AreEqual(typeof(bool), src.CacheVariableGood.Type, "Type of the cache good flag");
        }

        [TestMethod]
        public void TypeOfSequence()
        {
            var qmb = new QueryModelBuilder();
            qmb.AddClause(new SelectClause(Expression.Constant(1.0)));
            int[] list = new int[10];
            qmb.AddClause(new MainFromClause("r1", typeof(int), Expression.Constant(list)));

            var h = new QMFuncHeader() { Arguments = new object[] { }, IsSequence = true, QM = qmb.Build(), QMText = "hi" };
            var src = new QMFuncSource(h);
            Assert.AreEqual(typeof(double[]), src.ResultType, "result type");

            Assert.Inconclusive();
            //Assert.AreEqual(typeof(double[]), src.CacheVariable.Type, "Type of cache");
            Assert.AreEqual(typeof(bool), src.CacheVariableGood.Type, "Type of the cache good flag");
        }

        [TestMethod]
        public void TestSaveSingleItem()
        {
            var qmb = new QueryModelBuilder();
            qmb.AddClause(new SelectClause(Expression.Constant(1.0)));
            qmb.AddResultOperator(new Remotion.Linq.Clauses.ResultOperators.FirstResultOperator(true));
            int[] list = new int[10];
            qmb.AddClause(new MainFromClause("r1", typeof(int), Expression.Constant(list)));

            var h = new QMFuncHeader() { Arguments = new object[] { }, IsSequence = false, QM = qmb.Build(), QMText = "hi" };
            var src = new QMFuncSource(h);
            Assert.AreEqual(typeof(double), src.ResultType, "result type");

            // Build the expression we are going to cache

            var decl = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var savers = src.CacheExpression(decl).ToArray();
            Assert.AreEqual(1, savers.Length, "# of savers");
            Assert.AreEqual("aDouble_4=aDouble_3", savers[0].ToString(), "Single saver");

            Assert.AreEqual(1, src.CacheVariables.Length, "# of cached variables");

            Assert.AreEqual(typeof(double), src.CacheVariables[0].Type, "Type of cache");
        }
    }
}
