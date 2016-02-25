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
        // TODO: replace all the for print with DumpToConsole()
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
        public void QMWithMixedSelecAndConcats()
        {
            // This produces a rather nasty combination: one of the Concat operators ends
            // up in the main from clause, and the other one ends up in one of the result operators
            // of the main query model.
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            var q3 = new QMExtractorQueriable<ntup>();

            var r1 = q1
                .Select(r => r.run)
                .Concat(q2.Select(r => r.run))
                .Select(r => r * 2)
                .Concat(q3.Select(r => r.run))
                .Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm);

            foreach (var qmNew in qmList)
            {
                Console.WriteLine(qmNew);
            }

            Assert.AreEqual(3, qmList.Length);
            // TODO: fix this so it works.
            //CheckForQuery(() => q1.Select(r => r.run).Select(r => r * 2).Count(), qmList);
            //CheckForQuery(() => q2.Select(r => r.run).Select(r => r * 2).Count(), qmList);
            //CheckForQuery(() => q3.Select(r => r.run).Count(), qmList);
        }

        [TestMethod]
        public void QMWithSelectInConcat()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();

            var r1 = q1
                .Select(r => r.run + 1)
                .Concat(q2.Select(r => r.run))
                .Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(2, qmList.Length);

            CheckForQuery(() => q1.Select(r => r.run + 1).Count(), qmList);
            CheckForQuery(() => q1.Select(r => r.run).Count(), qmList);
        }

        [TestMethod]
        public void QMWithSelectAfterAndInConcat()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();

            var r1 = q1
                .Select(r => r.run + 1)
                .Concat(q2.Select(r => r.run))
                .Select(r => r * 2)
                .Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(2, qmList.Length);

            // Generates the proper thing, but does it "down one" with an extra from int i in xxx". :(
            // TODO: fix this so it doesn't unnessecarrily introduce done level down.
            //CheckForQuery(() => q1.Select(r => r.run).Select(r => r * 2).Count(), qmList);
            //CheckForQuery(() => q1.Select(r => r.run + 1).Select(r => r * 2).Count(), qmList);
        }

        /// <summary>
        /// Look to see if a query appears in our list.
        /// </summary>
        /// <param name="mq1"></param>
        /// <param name="qmList"></param>
        private void CheckForQuery<T>(Func<T> generateQuery, QueryModel[] qmList, int count = 1)
        {
            generateQuery();
            var qm = QMExtractorExecutor.LastQM.CleanQMString();
            Assert.AreEqual(count, qmList.Where(q => q.CleanQMString() == qm).Count(), $"Could not find {count} instances of the query model {qm}.");
        }

        [TestMethod]
        public void QMWithSelectAfterConcat()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();

            var r1 = q1
                .Concat(q2)
                .Select(r => r.run)
                .Select(r => r * 2)
                .Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(2, qmList.Length);
            Assert.AreEqual(qmList[0].ToString(), qmList[1].ToString());

            var rwanted = q1.Select(r => r.run).Select(r => r * 2).Count();
            var qmExpected = QMExtractorExecutor.LastQM;

            Assert.AreEqual(qmExpected.CleanQMString(), qmList[0].CleanQMString());
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
        [ExpectedException(typeof(NotSupportedException))]
        public void ConcatOfArrays()
        {
            // We need to fail badly when we have the Concat operator in a select clause.
            // We don't support this sort of thing, and none of the code that enables splitting
            // of queries should affect that.

            var q1 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupeArrD>();
            var r = q1.Select(e => e.myvectorofdouble.Concat(e.myvectorofdouble).Count()).Sum();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm);

            Assert.AreEqual(1, qmList.Length);
        }
    }
}
