using LINQToTTreeLib.QueryVisitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
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
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(1, qmList.Length);
            CheckForQuery(() => q1.Count(), qmList);
        }

        [TestMethod]
        public void QMWith2SimpleConcats()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            var r1 = q1.Concat(q2).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(2, qmList.Length);
            CheckForQuery(() => q1.Count(), qmList, 2); // Can't really tell the difference between q1 and q2.

            // Make sure the query providers are correct! Since we don't care about the order.
            var providersUsed = ExtractProviders<ntup>(qmList);
            Assert.IsTrue(providersUsed.Contains(q1.Provider));
            Assert.IsTrue(providersUsed.Contains(q2.Provider));
            Assert.AreEqual(2, providersUsed.Count);
        }

        [TestMethod]
        public void QMWithConcatAndEarlyTake()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var r1 = q1.Take(10).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(1, qmList.Length);
            CheckForQuery(() => q1.Take(10).Count(), qmList);
        }

        [TestMethod]
        public void QMWith2ConcatsAndOneEarlyTake()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            var r1 = q1.Take(500).Concat(q2).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(2, qmList.Length);
            CheckForQuery(() => q1.Take(500).Count(), qmList, 1); // Can't really tell the difference between q1 and q2.
            CheckForQuery(() => q1.Count(), qmList, 1); // Can't really tell the difference between q1 and q2.

            // Make sure the query providers are correct! Since we don't care about the order.
            var providersUsed = ExtractProviders<ntup>(qmList);
            Assert.IsTrue(providersUsed.Contains(q1.Provider));
            Assert.IsTrue(providersUsed.Contains(q2.Provider));
            Assert.AreEqual(2, providersUsed.Count);
        }

        [TestMethod]
        public void QMWith2ConcatsAndTwoEarlyTake()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            var r1 = q1.Take(500).Concat(q2.Take(700)).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(2, qmList.Length);
            CheckForQuery(() => q1.Take(500).Count(), qmList, 1); // Can't really tell the difference between q1 and q2.
            CheckForQuery(() => q1.Take(700).Count(), qmList, 1); // Can't really tell the difference between q1 and q2.

            // Make sure the query providers are correct! Since we don't care about the order.
            var providersUsed = ExtractProviders<ntup>(qmList);
            Assert.IsTrue(providersUsed.Contains(q1.Provider));
            Assert.IsTrue(providersUsed.Contains(q2.Provider));
            Assert.AreEqual(2, providersUsed.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void QMWith2ConcatsAndOneLateTake()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            // THis is not allowed as the current infrastructure doesn't know how to do the Take properly (yet).
            // So this should cause an exception.
            var r1 = q1.Concat(q2).Take(300).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void QMWith2ConcatsAndOneBurriedLateTake()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            // THis is not allowed as the current infrastructure doesn't know how to do the Take properly (yet).
            // So this should cause an exception. It will print out, however, a way around this.
            var r1 = q1.Concat(q2).Where(x => x.run > 10).Take(300).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();
        }

        [TestMethod]
        public void QMWith2ConcatsAndOneLateTakePerSource()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            // THis is not allowed as the current infrastructure doesn't know how to do the Take properly (yet).
            // So this should cause an exception.
            var r1 = q1.Concat(q2).TakePerSource(300).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(2, qmList.Length);
            CheckForQuery(() => q1.Take(300).Count(), qmList, 2); // Can't really tell the difference between q1 and q2.
        }

        [TestMethod]
        public void QMWith2ConcatsAndOneBurriedLateTakePerSource()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            // THis is not allowed as the current infrastructure doesn't know how to do the Take properly (yet).
            // So this should cause an exception.
            var r1 = q1.Concat(q2).Where(x => x.run > 10).TakePerSource(300).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(2, qmList.Length);
            CheckForQuery(() => q1.Where(x => x.run > 10).Take(300).Count(), qmList, 2, "x"); // Can't really tell the difference between q1 and q2.
        }

        [TestMethod]
        public void QMWith2ConcatsAndOneLateSkipPerSource()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            // THis is not allowed as the current infrastructure doesn't know how to do the Take properly (yet).
            // So this should cause an exception.
            var r1 = q1.Concat(q2).SkipPerSource(300).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(2, qmList.Length);
            CheckForQuery(() => q1.Skip(300).Count(), qmList, 2); // Can't really tell the difference between q1 and q2.
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
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(3, qmList.Length);
            CheckForQuery(() => q1.Count(), qmList, 3);

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
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(3, qmList.Length);
            CheckForQuery(() => q1.Count(), qmList, 3);

            // Make sure the query providers are correct! Since we don't care about the order.
            var providersUsed = ExtractProviders<ntup>(qmList);
            Assert.IsTrue(providersUsed.Contains(q1.Provider));
            Assert.IsTrue(providersUsed.Contains(q2.Provider));
            Assert.IsTrue(providersUsed.Contains(q3.Provider));
            Assert.AreEqual(3, providersUsed.Count);
        }

        [TestMethod]
        public void QMWith3Aggregate()
        {
            var q1 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupeArrD>();
            var q2 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupeArrD>();
            var q3 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupeArrD>();
            var seq = new IQueryable<TTreeQueryExecutorTest.TestNtupeArrD>[] { q1, q2, q3 };

            var all = seq.Skip(1).Aggregate(seq[0], (allc, next) => allc.Concat(next));
            var r = all.Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(3, qmList.Length);
            CheckForQuery(() => q1.Count(), qmList, 3);
        }

        [TestMethod]
        public void QMWithThreeAndSearchOperator()
        {
            var q1 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupeArrD>();
            var q2 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupeArrD>();
            var q3 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupeArrD>();
            var seq = new IQueryable<TTreeQueryExecutorTest.TestNtupeArrD>[] { q1, q2, q3 };

            var all = seq.Skip(1).Aggregate(seq[0], (allc, next) => allc.Concat(next));

            all
                .Select(e => e.myvectorofdouble.OrderByDescending(j => j).First())
                .Where(x => x > 5)
                .Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(3, qmList.Length);
            CheckForQuery(() => q1.Select(e => e.myvectorofdouble.OrderByDescending(j => j).First()).Where(x => x > 5).Count(), qmList, 3, "e");
        }

        [TestMethod]
        public void QMWithBadClone()
        {
            // Found when running the test - with a global Clone as a test. Fail to do the Clone.

            var q1 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupeArrD>();

            var dudeQ = from evt in q1
                        select evt.myvectorofdouble.Count();
            var dude = dudeQ.Aggregate(0.0, (acc, val) => acc + val);

            var qm = QMExtractorExecutor.LastQM;
            qm.Clone();
        }

        [TestMethod]
        public void QMWithSelectConcats()
        {
            var q1 = new QMExtractorQueriable<ntup>();
            var q2 = new QMExtractorQueriable<ntup>();
            var r1 = q1.Select(r => r.run).Concat(q2.Select(r => r.run + 1)).Count();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(2, qmList.Length);
            CheckForQuery(() => q1.Select(r => r.run).Count(), qmList);
            CheckForQuery(() => q2.Select(r => r.run + 1).Count(), qmList);
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
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(3, qmList.Length);
            CheckForQuery(() => q3.Select(r => r.run).Count(), qmList);
            CheckForQuery(() => q1.Select(r => r.run).Select(r => r * 2).Count(), qmList, 2);
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

            CheckForQuery(() => q1.Select(r => r.run).Select(r => r * 2).Count(), qmList);
            CheckForQuery(() => q1.Select(r => r.run + 1).Select(r => r * 2).Count(), qmList);
        }

        /// <summary>
        /// Look to see if a query appears in our list.
        /// </summary>
        /// <param name="mq1"></param>
        /// <param name="qmList"></param>
        private void CheckForQuery<T>(Func<T> generateQuery, QueryModel[] qmList, int count = 1, string generatedReplacement = null)
        {
            generateQuery();
            var qm = QMExtractorExecutor.LastQM.CleanQMString();

            Assert.AreEqual(count, qmList.Where(q => q.CleanQMString().ReplaceGenerated(generatedReplacement) == qm).Count(), $"Could not find {count} instances of the query model {qm}.");
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

            CheckForQuery(() => q1.Select(r => r.run).Select(r => r * 2).Count(), qmList, 2, "r");
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
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(2, qmList.Length);

            CheckForQuery(() => q1.Select(i => (int)1).Count(), qmList);
            CheckForQuery(() => q2.SelectMany(e => e.myvectorofdouble).Select(i => (int)1).Count(), qmList);

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
        [Ignore]
        public void WithConcatInSelectMany()
        {
            var q1 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupe>();
            var q2 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupeArrD>();

            var dude = q2.SelectMany(e => e.myvectorofdouble.Select(i => (int)1).Concat(q1.Select(eb => (int)1))).Count();
            var qm = QMExtractorExecutor.LastQM;

            var QMList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(2, QMList.Length);

        }

        [TestMethod]
        public void ConcatOfArrays()
        {
            // We need to fail badly when we have the Concat operator in a select clause.
            // We don't support this sort of thing, and none of the code that enables splitting
            // of queries should affect that.

            var q1 = new QMExtractorQueriable<TTreeQueryExecutorTest.TestNtupeArrD>();
            var r = q1.Select(e => e.myvectorofdouble.Concat(e.myvectorofdouble).Count()).Sum();

            var qm = QMExtractorExecutor.LastQM;
            var qmList = ConcatSplitterQueryVisitor.Split(qm)
                .DumpToConsole();

            Assert.AreEqual(1, qmList.Length);
        }
    }

    static class QMHelpers
    {
        public static string ReplaceGenerated (this string qm, string replacementVariable)
        {
            var matches = Regex.Match(qm, "<generated>_[0-9]+");
            if (matches.Success)
            {
                qm = qm.Replace(matches.Value, replacementVariable);
            }
            return qm;
        }
    }
}
