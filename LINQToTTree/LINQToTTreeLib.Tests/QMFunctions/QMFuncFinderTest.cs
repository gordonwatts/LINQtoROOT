﻿using LINQToTTreeLib.QMFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Tests.QMFunctions
{
    /// <summary>
    /// Test the QM Function Finder and Resolvers.
    /// </summary>
    [TestClass]
    public class QMFuncFinderTest
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
        public void TestNoneQMFunc()
        {
            var q = new QueriableDummyNoExe<ntup>();
            var r1 = q.Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(0, sf.Count(), "# of qm functions");
        }

        [TestMethod]
        public void TestSingleQMFuncInWhere()
        {
            var q = new QueriableDummyNoExe<dummyntup>();
            var a = from evt in q
                    where evt.valC1D.Where(i => i > 5).Count() > 5
                    select evt;
            var r1 = a.Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(1, sf.Count(), "# of qm functions");
            var f = sf.First();
            Assert.AreEqual(0, f.Arguments.Count(), "# of arguments");
        }

        /// <summary>
        /// Make sure that we will do a QM function when the First isn't a number, or simple type, but also
        /// something like a single object. In the rest of the code that will translate to being
        /// an index or similar.
        /// </summary>
        [TestMethod]
        public void TestAnonymousObjectSingleResult()
        {
            var q = new QueriableDummyNoExe<dummyntup>();
            var a = from evt in q
                    select new
                    {
                        Jets = evt.valC1D,
                        Tracks = evt.valC1D
                    };

            var b = from evt in a
                    select new
                    {
                        MatchedJets = from j in evt.Jets
                                      where evt.Tracks.Where(t => Math.Abs(t - j) < 0.1).Any()
                                      select new
                                      {
                                          Jet = j,
                                          Track = (from t in evt.Tracks orderby Math.Abs(j - t) ascending select t).First()
                                      }
                    };

            var c = from evt in b
                    select (from j in evt.MatchedJets orderby j.Track - j.Jet select j).First();

            var d = from evt in c
                    select evt.Jet;

            var r1 = d.Sum();
            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(2, sf.Count(), "# of qm functions");
        }

        /// <summary>
        /// Make sure no functions that return a list of things that
        /// are to be selected are found!
        /// </summary>
        [TestMethod]
        public void TestQMNoFunctionWithSelect()
        {
            var q = new QueriableDummyNoExe<dummyntup>();
            var evt = from e in q
                      select new
                      {
                          Jets = e.valC1D.Where(i => i > 1),
                          Tracks = e.valC1D.Where(i => i > 1)
                      };
            var r = from e in evt
                    from t in e.Tracks
                    from j in e.Jets
                    where t != j
                    select t;

            var result = r.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            var sf = QMFuncFinder.FindQMFunctions(query);

            Assert.AreEqual(1, sf.Count(), "# of functions");
            Assert.IsTrue(sf.First().IsSequence, "is sequence");
        }

        [TestMethod]
        public void TestSingleQMFuncInSelect()
        {
            var q = new QueriableDummyNoExe<dummyntup>();
            var a = from evt in q
                    select evt.valC1D.Where(i => i > 5).Count();
            var r1 = a.Sum();
            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(1, sf.Count(), "# of qm functions");
            var f = sf.First();
            Assert.AreEqual(0, f.Arguments.Count(), "# of arguments");
            Assert.IsFalse(f.IsSequence, "IsSequence");
        }

        [TestMethod]
        public void TestGroupByFunction()
        {
            var q = new QueriableDummyNoExe<LINQToTTreeLib.QueryVisitorTest.NtupWithObjectsDest>();

            var r = from evt in q
                    select from v in evt.var1 group v by v;

            var cnt = from evt in r
                      from grp in evt
                      where grp.Key == 2 && grp.Count() > 5
                      select grp.Key;

            var final = cnt.Count();

            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.AreEqual(1, sf.Count(), "# of qm functions");
        }

        [TestMethod]
        public void TestAnonymousFunctionResults()
        {
            var q = new QueriableDummy<LINQToTTreeLib.QueryVisitorTest.Ntup3>();

            var resultA = from evt in q
                          select new
                          {
                              jets = evt.run1,
                              tracks = evt.run2,
                              truth = evt.run1
                          };
            var resultB = from e in resultA
                          select new
                          {
                              joinedR = from r1 in e.jets
                                        select new
                                        {
                                            Jet = r1,
                                            CloseTrack = (from r2 in e.tracks
                                                          orderby r1 - r2 ascending
                                                          select r2).First(),
                                            Truth = (from t in e.truth
                                                     orderby t - r1 descending
                                                     select t).First() == 21
                                        }
                          };
            var resultC = resultB.Where(e => e.joinedR.Count() == 2);
            var result2j = from e in resultB
                           select new
                           {
                               Jet1 = e.joinedR.First(),
                               Jet2 = e.joinedR.Skip(1).First()
                           };

            Expression<Func<bool, double, double, double>> calc = (t, r1, r2) => t ? r1 : r2;

            var resultToSum = result2j.Select(e => calc.Invoke(e.Jet1.Truth, 5, 10) * calc.Invoke(e.Jet2.Truth, 5, 10));
            var result = resultToSum.Sum();

            var qm = DummyQueryExectuor.FinalResult;
            Assert.IsTrue(qm.QMFunctions.Where(f => f.StatementBlock != null).All(f => !f.ResultType.Name.Contains("Anon")), "contains anon");
        }

        [TestMethod]
        public void TestDoubleFrom()
        {
            var q = new QueriableDummyNoExe<dummyntup>();
            var a = from evt in q
                    from i in evt.valC1D
                    select i;
            var r1 = a.Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(0, sf.Count(), "# of qm functions");
        }

        [TestMethod]
        public void TestDoubleFromWithFuncWithArg()
        {
            var q = new QueriableDummyNoExe<dummyntup>();
            var a = from evt in q
                    from i in evt.valC1D
                    where evt.valC1D.Where(j => i > j).Count() > 5
                    select i;
            var r1 = a.Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(1, sf.Count(), "# of qm functions");
            var f = sf.First();
            Assert.AreEqual(1, f.Arguments.Count(), "# of arguments");
        }

        [TestMethod]
        public void TestDoubleFromWithFuncWithSameArgTwice()
        {
            var q = new QueriableDummyNoExe<dummyntup>();
            var a = from evt in q
                    from i in evt.valC1D
                    where evt.valC1D.Where(j => i > j).Count() > 5
                    select evt;

            var b = from evt in q
                    from i in evt.valC1D
                    where evt.valC1D.Where(j => i > j).Count() > 10
                    select i;
            var r1 = b.Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(1, sf.Count(), "# of qm functions");
            var f = sf.First();
            Assert.AreEqual(1, f.Arguments.Count(), "# of arguments");
        }

        [TestMethod]
        public void TestDoubleFromWithFuncWithArgDoubleRef()
        {
            var q = new QueriableDummyNoExe<dummyntup>();
            var a = from evt in q
                    from i in evt.valC1D
                    where evt.valC1D.Where(j => i > j && j >= i).Count() > 5
                    select i;
            var r1 = a.Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(1, sf.Count(), "# of qm functions");
            var f = sf.First();
            Assert.AreEqual(1, f.Arguments.Count(), "# of arguments");
        }

        [TestMethod]
        public void TestAdditonalFromClause()
        {
            var q = new QueriableDummyNoExe<dummyntup>();
            var a = from evt in q
                    select new
                    {
                        Jets = evt.valC1D,
                        Tracks = evt.valC1D
                    };

            var b = from evt in a
                    select new
                    {
                        MatchedJets = from j in evt.Jets
                                      where evt.Tracks.Where(t => Math.Abs(t - j) < 0.1).Any()
                                      select new
                                      {
                                          Jet = j,
                                          Track = (from t in evt.Tracks orderby Math.Abs(j - t) ascending select t).First()
                                      }
                    };

            var c = from evt in b
                    from j in evt.MatchedJets
                    where j.Jet > 30
                    select j.Jet + j.Track;
            var r1 = c.Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(2, sf.Count(), "# of qm functions");
        }

        /// <summary>
        /// When from clauses get tricky.
        /// </summary>
        [TestMethod]
        public void TestDuplicateQM()
        {
            var q = new QueriableDummyNoExe<dummyntup>();
            var a = from evt in q.Where(e => e.valC1D.First() > 0)
                    select evt.valC1D.First();
            var r1 = a.Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(1, sf.Count(), "# of qm functions");
        }

        /// <summary>
        /// Make sure additional from clauses are correctly parsed.
        /// </summary>
        [TestMethod]
        public void TestBogus()
        {
            var q = new QueriableDummyNoExe<dummyntup>();
            var a = from evt in q
                    select new
                    {
                        Jets = evt.valC1D,
                        Tracks = evt.valC1D
                    };

            var b = from evt in a
                    select new
                    {
                        MatchedJets = from j in evt.Jets
                                      where evt.Tracks.Where(t => Math.Abs(t - j) < 0.1).Any()
                                      select new
                                      {
                                          Jet = j,
                                          Track = (from t in evt.Tracks orderby Math.Abs(j - t) ascending select t).First()
                                      }
                    };

            var r1 = b.Where(evt => evt.MatchedJets.Count() > 1).Count();
            var qm = DummyQueryExectuor.LastQueryModel;

            var sf = QMFuncFinder.FindQMFunctions(qm);
            Assert.IsNotNull(sf);
            Assert.AreEqual(3, sf.Count(), "# of qm functions");
            var f = sf.First();
            Assert.AreEqual(1, f.Arguments.Count(), "# of arguments");
        }

        /// <summary>
        /// We can't transfer an temp object accross the boundary, much the same way we can't
        /// do an anonymous function.
        /// </summary>
        [TestMethod]
        public void NoObjectFunctionReturns()
        {
            var q = new QueriableDummyNoExe<LINQToTTreeLib.ResultOperators.ROFirstLastTest.ntupWithObjects>();

            // Create a dual object. Avoid anonymous objects just for the sake of it.
            var matched = from evt in q
                          select new LINQToTTreeLib.ResultOperators.ROFirstLastTest.TestTranslatedNestedCompareAndSortHolderEvent()
                          {
                              matches = from j in evt.jets
                                        orderby j.v3 ascending
                                        select new LINQToTTreeLib.ResultOperators.ROFirstLastTest.TestTranslatedNestedCompareAndSortHolder()
                                        {
                                            jet = j
                                        }
                          };

            // Filter on the first jet in the sequence.
            var goodmatched = from evt in matched
                              where evt.matches.First().jet.v3 > 0
                              select evt;

            var r = goodmatched.Count();

            var qm = DummyQueryExectuor.LastQueryModel;
            var sf = QMFuncFinder.FindQMFunctions(qm);

            Assert.AreEqual(0, sf.Where(f => !f.IsSequence).Count(), "# of functions");
            Assert.AreEqual(0, sf.Where(f => f.IsSequence).Count(), "# of functions"); // it returns an object...
        }

        [TestMethod]
        public void CatchArgsInOrderBy()
        {
            // This test produces somethign caught in the wild (caused a compile error).
            // The bug has to do with a combination of the First predicate and the CPPCode statement conspiring
            // to cause the problem, unfortunately. So, the test is here.
            var q = new QueriableDummyNoExe<LINQToTTreeLib.ResultOperators.ROFirstLastTest.ntup3>();

            var resultA = from evt in q
                          select new
                          {
                              r1 = evt.run1.Where(r => r > 3).Select(r => LINQToTTreeLib.ResultOperators.ROFirstLastTest.DoItClass.DoIt(r)),
                              r2 = evt.run2.Where(r => r > 4).Select(r => LINQToTTreeLib.ResultOperators.ROFirstLastTest.DoItClass.DoIt(r))
                          };
            var resultB = from e in resultA
                          select new
                          {
                              joinedR = from r1 in e.r1
                                        select (from r2 in e.r2
                                                orderby r1 - r2 ascending
                                                select r2).First()
                          };
            var result = from e in resultB
                         from r in e.joinedR
                         select r;
            var c = result.Sum();

            var qm = DummyQueryExectuor.LastQueryModel;
            var sf = QMFuncFinder.FindQMFunctions(qm);

            Assert.AreEqual(1, sf.Where(f => !f.IsSequence).Count(), "# of non-functions");
            Assert.AreEqual(3, sf.Where(f => f.IsSequence).Count(), "# of sequence functions");
        }
    }
}
