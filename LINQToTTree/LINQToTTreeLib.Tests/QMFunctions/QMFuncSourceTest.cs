using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.QMFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System.Linq.Expressions;
using System.Linq;
using System;

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
        public void CacheOfNonSequence()
        {
            var qmb = new QueryModelBuilder();
            qmb.AddClause(new SelectClause(Expression.Constant(1.0)));
            qmb.AddResultOperator(new Remotion.Linq.Clauses.ResultOperators.FirstResultOperator(true));
            int[] list = new int[10];
            qmb.AddClause(new MainFromClause("r1", typeof(int), Expression.Constant(list)));

            var h = new QMFuncHeader() { Arguments = new object[] { }, IsSequence = false, QM = qmb.Build(), QMText = "hi" };
            var src = new QMFuncSource(h);
            Assert.AreEqual(typeof(double), src.ResultType, "result type");

            Assert.AreEqual(typeof(bool), src.CacheVariableGood.Type, "Type of the cache good flag");
        }

        [TestMethod]
        public void CacheInSequence()
        {
            var qmb = new QueryModelBuilder();
            qmb.AddClause(new SelectClause(Expression.Constant(1.0)));
            int[] list = new int[10];
            qmb.AddClause(new MainFromClause("r1", typeof(int), Expression.Constant(list)));

            var h = new QMFuncHeader() { Arguments = new object[] { }, IsSequence = true, QM = qmb.Build(), QMText = "hi" };
            var src = new QMFuncSource(h);
            Assert.AreEqual(typeof(double[]), src.ResultType, "result type");

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

        [TestMethod]
        public void TestSaveSequenceItem()
        {
            var qmb = new QueryModelBuilder();
            qmb.AddClause(new SelectClause(Expression.Constant(1.0)));
            int[] list = new int[10];
            qmb.AddClause(new MainFromClause("r1", typeof(int), Expression.Constant(list)));

            var h = new QMFuncHeader() { Arguments = new object[] { }, IsSequence = true, QM = qmb.Build(), QMText = "hi" };
            var src = new QMFuncSource(h);
            Assert.AreEqual(typeof(double[]), src.ResultType, "result type");

            // Build the expression we are going to cache

            var decl = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var savers = src.CacheExpression(decl, decl).ToArray();
            Assert.AreEqual(1, savers.Length, "# of savers");
            Assert.AreEqual("aDouble_4Array.push_back(aDouble_3);", savers[0].ToString(), "Single sequence saver");

            Assert.AreEqual(1, src.CacheVariables.Length, "# of cached variables");
            Assert.AreEqual("Double[*]", src.CacheVariables[0].Type.Name, "Type of cache");
        }

        [TestMethod]
        public void TestSaveSequenceTwoItem()
        {
            var qmb = new QueryModelBuilder();
            qmb.AddClause(new SelectClause(Expression.Constant(1.0)));
            int[] list = new int[10];
            qmb.AddClause(new MainFromClause("r1", typeof(int), Expression.Constant(list)));

            var h = new QMFuncHeader() { Arguments = new object[] { }, IsSequence = true, QM = qmb.Build(), QMText = "hi" };
            var src = new QMFuncSource(h);
            Assert.AreEqual(typeof(double[]), src.ResultType, "result type");

            // Build the expression we are going to cache

            var decl1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var decl2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var decl = Expression.Add(decl1, Expression.Convert(decl2, typeof(double)));
            var savers = src.CacheExpression(decl, decl2).ToArray();
            Assert.AreEqual(2, savers.Length, "# of savers");
            Assert.AreEqual("aDouble_5Array.push_back(aDouble_3);", savers[0].ToString(), "Single sequence saver 1");
            Assert.AreEqual("aInt32_6Array.push_back(aInt32_4);", savers[1].ToString(), "Single sequence saver 2");

            Assert.AreEqual(2, src.CacheVariables.Length, "# of cached variables");
            Assert.AreEqual("Double[*]", src.CacheVariables[0].Type.Name, "Type of cache 1");
            Assert.AreEqual("Int32[*]", src.CacheVariables[1].Type.Name, "Type of cache 2");
        }

        /// <summary>
        /// Since this QM function really does duty as two types of functions - one that returns a sequence
        /// and one that returns a single value - we have to protect against one "version" doing the work
        /// of the other. This looks for the appropriate exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSaveSequenceFailed()
        {
            var qmb = new QueryModelBuilder();
            qmb.AddClause(new SelectClause(Expression.Constant(1.0)));
            int[] list = new int[10];
            qmb.AddClause(new MainFromClause("r1", typeof(int), Expression.Constant(list)));

            var h = new QMFuncHeader() { Arguments = new object[] { }, IsSequence = true, QM = qmb.Build(), QMText = "hi" };
            var src = new QMFuncSource(h);

            // Build the expression we are going to cache

            var decl1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var decl2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var decl = Expression.Add(decl1, Expression.Convert(decl2, typeof(double)));
            var savers = src.CacheExpression(decl).ToArray();
        }

        /// <summary>
        /// Since this QM function really does duty as two types of functions - one that returns a sequence
        /// and one that returns a single value - we have to protect against one "version" doing the work
        /// of the other. This looks for the appropriate exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSaveSingleFailed()
        {
            var qmb = new QueryModelBuilder();
            qmb.AddClause(new SelectClause(Expression.Constant(1.0)));
            int[] list = new int[10];
            qmb.AddClause(new MainFromClause("r1", typeof(int), Expression.Constant(list)));

            var h = new QMFuncHeader() { Arguments = new object[] { }, IsSequence = false, QM = qmb.Build(), QMText = "hi" };
            var src = new QMFuncSource(h);

            // Build the expression we are going to cache

            var decl1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var decl2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var decl = Expression.Add(decl1, Expression.Convert(decl2, typeof(double)));
            var savers = src.CacheExpression(decl, decl2).ToArray();
        }

        [TestMethod]
        public void TestSaveSingleComplexItem()
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

            var decl1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var decl2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var decl = Expression.Add(decl1, decl2);
            var savers = src.CacheExpression(decl).ToArray();
            Assert.AreEqual(2, savers.Length, "# of savers");
            Assert.AreEqual("aDouble_5=aDouble_3", savers[0].ToString(), "Single saver 1");
            Assert.AreEqual("aDouble_6=aDouble_4", savers[1].ToString(), "Single saver 2");

            Assert.AreEqual(2, src.CacheVariables.Length, "# of cached variables");

            Assert.AreEqual(typeof(double), src.CacheVariables[0].Type, "Type of cache");
            Assert.AreEqual(typeof(double), src.CacheVariables[1].Type, "Type of cache");
        }
    }
}
