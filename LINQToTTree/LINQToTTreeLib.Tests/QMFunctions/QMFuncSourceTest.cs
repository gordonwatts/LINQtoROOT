using LINQToTTreeLib.QMFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Tests.QMFunctions
{
    [TestClass]
    public class QMFuncSourceTest
    {
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

            Assert.AreEqual(typeof(double), src.CacheVariable.Type, "Type of cache");
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

            Assert.AreEqual(typeof(double[]), src.CacheVariable.Type, "Type of cache");
            Assert.AreEqual(typeof(bool), src.CacheVariableGood.Type, "Type of the cache good flag");
        }
    }
}
