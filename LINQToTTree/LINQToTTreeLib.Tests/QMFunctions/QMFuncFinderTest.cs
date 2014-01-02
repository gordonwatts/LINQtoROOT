using LINQToTTreeLib.QMFunctions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

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
            Assert.AreEqual(3, sf.Count(), "# of qm functions");
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

            Assert.AreEqual(0, sf.Count(), "# of functions");
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
    }
}
