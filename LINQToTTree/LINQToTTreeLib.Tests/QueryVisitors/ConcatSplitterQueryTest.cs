using LINQToTTreeLib.QueryVisitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Tests.QueryVisitors
{
    [TestClass]
    public class ConcatQuerySplitterTest
    {
        [TestMethod]
        public void QMWithNoConcats()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var r1 = q1.Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm);

            Assert.AreEqual(1, qmList.Length);
            Assert.AreEqual(qm.ToString(), qmList[0].ToString());
        }

        [TestMethod]
        public void QMWith2SimpleConcats()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            var r1 = q1.Concat(q2).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm);

            foreach (var qmNew in qmList)
            {
                Console.WriteLine(qmNew);
            }

            Assert.AreEqual(2, qmList.Length);
            Assert.AreEqual(qmList[0].ToString(), qmList[1].ToString());

            // Make sure the query providers are correct! Since we don't care about the order.
            var providersUsed = ExtractProviders<ntup>(qmList);
            Assert.IsTrue(providersUsed.Contains(q1.Provider));
            Assert.IsTrue(providersUsed.Contains(q2.Provider));
            Assert.AreEqual(2, providersUsed.Count);
        }

        /// <summary>
        /// Extract the providers from each query sent in.
        /// </summary>
        /// <param name="qmList"></param>
        /// <returns></returns>
        private HashSet<IQueryProvider> ExtractProviders<T>(params QueryModel[] qmList)
        {
            var r = new HashSet<IQueryProvider>();

            foreach (var q in qmList)
            {
                var fromClause = q.MainFromClause.FromExpression;

                if (fromClause is SubQueryExpression)
                {
                    fromClause = (fromClause as SubQueryExpression).QueryModel.MainFromClause.FromExpression;
                }

                if (fromClause is ConstantExpression)
                {
                    var cVal = fromClause as ConstantExpression;
                    var provider = (cVal.Value as QMExtractorQueriable<T>);
                    if (provider != null)
                    {
                        r.Add(provider.Provider);
                    }
                }
                else
                {
                    Assert.Fail();
                }
            }

            return r;
        }

        [TestMethod]
        public void QMWith3SimpleConcats()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            var q3 = new QMExtractorQueriable<ntup>();
            var r1 = q1.Concat(q2).Concat(q3).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm);

            foreach (var qmNew in qmList)
            {
                Console.WriteLine(qmNew);
            }

            Assert.AreEqual(3, qmList.Length);
            Assert.AreEqual(qmList[0].ToString(), qmList[1].ToString());
            Assert.AreEqual(qmList[0].ToString(), qmList[2].ToString());

            // Make sure the query providers are correct! Since we don't care about the order.
            var providersUsed = ExtractProviders<ntup>(qmList);
            Assert.IsTrue(providersUsed.Contains(q1.Provider));
            Assert.IsTrue(providersUsed.Contains(q2.Provider));
            Assert.IsTrue(providersUsed.Contains(q3.Provider));
            Assert.AreEqual(3, providersUsed.Count);
        }

        [TestMethod]
        public void QMWith3EmbededConcat()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            var q3 = new QMExtractorQueriable<ntup>();
            var r1 = q1.Concat(q2.Concat(q3)).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm);

            foreach (var qmNew in qmList)
            {
                Console.WriteLine(qmNew);
            }

            Assert.AreEqual(3, qmList.Length);
            Assert.AreEqual(qmList[0].ToString(), qmList[2].ToString());
            Assert.IsTrue(qmList[1].ToString().EndsWith("Count()"), $"'{qmList[1].ToString()}' doesn't end with Count()");

            // Make sure the query providers are correct! Since we don't care about the order.
            var providersUsed = ExtractProviders<ntup>(qmList);
            Assert.IsTrue(providersUsed.Contains(q1.Provider));
            Assert.IsTrue(providersUsed.Contains(q2.Provider));
            Assert.IsTrue(providersUsed.Contains(q3.Provider));
            Assert.AreEqual(3, providersUsed.Count);
        }

        [TestMethod]
        public void QMWithSelectConcats()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            var r1 = q1.Select(r => r.run).Concat(q2.Select(r => r.run + 1)).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm);

            foreach (var qmNew in qmList)
            {
                Console.WriteLine(qmNew);
            }

            Assert.AreEqual(2, qmList.Length);
            Assert.AreNotEqual(qmList[0].ToString(), qmList[1].ToString());

            // Basically different by +1, not totally sure how to determine that easily here, so will leave it untested for now.
        }

        [TestMethod]
        public void QMWithDifferentSelectConcats()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            var q3 = new QMExtractorQueriable<ntup>();

            var r1 = q1.Select(r => r.run).Concat(q2.Select(r => r.run)).Select(r => r * 2).Concat(q3.Select(r => r.run)).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm);

            foreach (var qmNew in qmList)
            {
                Console.WriteLine(qmNew);
            }

            Assert.AreEqual(3, qmList.Length);
            Assert.AreNotEqual(qmList[0].ToString(), qmList[1].ToString());
        }

        /// <summary>
        /// Stumbled on this while running a much more complex test in to generate code.
        /// Obviously doing a bad replacement here!
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [TestMethod]
        public void QMWithDifferentSourcesAndSelectMany()
        {
            var q1 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupe>();
            var q2 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupeArrD>();

            var dude = q2.SelectMany(e => e.myvectorofdouble).Select(i => (int)1).Concat(q1.Select(i => (int)1)).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm);

            foreach (var qmNew in qmList)
            {
                Console.WriteLine(qmNew);
            }

            Assert.AreEqual(2, qmList.Length);

            // The queries should split as follows. Make sure.
            var dude2 = q2.SelectMany(e => e.myvectorofdouble).Select(i => (int)1).Count();
            var qm2String = QMExtractorExecutor.LastQM.CleanQMString();
            var dude1 = q1.Select(i => (int)1).Count();
            var qm1String = QMExtractorExecutor.LastQM.CleanQMString();

            Assert.IsTrue(qmList.Where(q => q.CleanQMString() == qm1String).Any(), qm1String);
            Assert.IsTrue(qmList.Where(q => q.CleanQMString() == qm2String).Any(), qm2String);

            // Make sure the query providers are correct! Since we don't care about the order.
            var providersUsed1 = ExtractProviders<TTreeQueryExecutorTest.TestNtupe>(qmList);
            var providersUsed2 = ExtractProviders<TTreeQueryExecutorTest.TestNtupeArrD>(qmList);
            foreach (var item in providersUsed2)
            {
                providersUsed1.Add(item);
            }
            Assert.AreEqual(2, providersUsed1.Count);
            Assert.IsTrue(providersUsed1.Contains(q1.Provider));
            Assert.IsTrue(providersUsed1.Contains(q2.Provider));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConcatOfArrays()
        {
            // Thsi should fail - we don't allow arrays to be concat'd currently.
            Assert.Inconclusive();
        }
    }
}
