using LINQToTTreeLib.QueryVisitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTTreeLib.Tests.QueryVisitors
{
    [TestClass]
    public class ReferencedQueryExpressionReplacementTests
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();
            MEFUtilities.AddPart(new TypeHandlers.TypeHandlerConvert());
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        class bogusObject2
        {
            public int p1 { get; set; }
            public int p2 { get; set; }
        }

        class bogusObject2F
        {
            public int p1;
            public int p2;
        }

        /// <summary>
        /// The select statement contains both the new and the member de-ref.
        /// </summary>
        [TestMethod]
        public void TestMemberDeRefAndCreateInSelect()
        {
            var q = new QueriableDummyNoExe<ntup>();
            var result = from d in q
                         select new bogusObject2() { p1 = d.run, p2 = d.run }.p2;
            var c = result.Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsTrue(!qm.ToString().Contains("bogusObject2"), "still has bogusobject");
        }

        /// <summary>
        /// The select statement contains both the new and the member de-ref.
        /// </summary>
        [TestMethod]
        public void TestMemberDeRefAndCreateInSelectField()
        {
            var q = new QueriableDummyNoExe<ntup>();
            var result = from d in q
                         select new bogusObject2F() { p1 = d.run, p2 = d.run }.p2;
            var c = result.Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsTrue(!qm.ToString().Contains("bogusObject2"), "still has bogusobject");
        }

        [TestMethod]
        public void TestCreateInFromDeRefInSelect()
        {
            var q = new QueriableDummyNoExe<ntup>();
            var result = from d in q.Select(n => new bogusObject2() { p1 = n.run, p2 = n.run })
                         select d.p2;
            var c = result.Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsTrue(!qm.ToString().Contains("bogusObject2"), "still has bogusobject");
        }

        [TestMethod]
        public void TestCreateInFromUseInWhere()
        {
            var q = new QueriableDummyNoExe<ntup>();
            var result = from d in q.Select(n => new bogusObject2() { p1 = n.run, p2 = n.run })
                         where d.p2 > 5
                         select d.p2;
            var c = result.Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsTrue(!qm.ToString().Contains("bogusObject2"), "still has bogusobject");
        }

        [TestMethod]
        public void TestCreateInFromUseInWhereCrossCut()
        {
            var q = new QueriableDummyNoExe<ntup>();
            var result = from d in q.Select(n => new bogusObject2() { p1 = n.run + 1, p2 = n.run })
                         where d.p1 > 5
                         select d.p2;
            var c = result.Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsTrue(!qm.ToString().Contains("bogusObject2"), "still has bogusobject");
        }

        [TestMethod]
        public void TestCreateAnonAndSelect()
        {
            var q = new QueriableDummyNoExe<ntup>();
            var result = from d in q
                         select new
                         {
                             R1 = d.run,
                             R2 = d.run * 2
                         };
            var r1 = result.Select(d => d.R2);
            var c = r1.Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsTrue(!qm.ToString().Contains("Anon"), "still has bogusobject");
        }

        [TestMethod]
        public void TestCreateAnonWithLists()
        {
            var q = new QueriableDummyNoExe<LINQToTTreeLib.ResultOperators.ROFirstLastTest.ntup3>();
            var result = from d in q
                         select new
                         {
                             Jets = d.run1,
                             Tracks = d.run2
                         };
            var r1 = result.SelectMany(d => d.Jets);
            var c = r1.Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsTrue(!qm.ToString().Contains("Anon"), "still has bogusobject");
        }

        [TestMethod]
        public void TestCreateAnonWithListsQueried()
        {
            var q = new QueriableDummyNoExe<LINQToTTreeLib.ResultOperators.ROFirstLastTest.ntup3>();
            var result = from d in q
                         select new
                         {
                             Jets = d.run1,
                             Tracks = from t in d.run2
                                      where t < 4
                                      select t
                         };
            var r1 = result.SelectMany(d => d.Jets);
            var c = r1.Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsTrue(!qm.ToString().Contains("Anon"), "still has bogusobject");
        }

        [TestMethod]
        public void CreateAnonWithLinkedWheres()
        {
            var q = new QueriableDummyNoExe<LINQToTTreeLib.ResultOperators.ROFirstLastTest.ntup3>();
            var result = from d in q
                         select new
                         {
                             Jets = d.run1,
                             Tracks = from t in d.run2
                                      where t < 4
                                      select t
                         };
            var r1 = result.Where(t => t.Tracks.Count() > 5).SelectMany(d => d.Jets);
            var c = r1.Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsTrue(!qm.ToString().Contains("Anon"), "still has bogusobject");
        }

        class Jet
        {
            public double pt { get; set; }
        }

        class TruthJet
        {
            public double pt { get; set; }
        }

        class MatchedJet
        {
            public Jet jet { get; set; }
            public TruthJet truth { get; set; }
        }

        class matchedEvent
        {
            public IEnumerable<MatchedJet> matches { get; set; }
        }

        [TestMethod]
        public void CreateFullEventNoCrossCutAllAnon()
        {
            var q = new QueriableDummyNoExe<LINQToTTreeLib.ResultOperators.ROFirstLastTest.ntup3>();
            var result = from d in q
                         select new
                         {
                             matches = from j in d.run1.Select(i => new { pt = i })
                                       select new
                                       {
                                           jet = j,
                                           truth = new { pt = j.pt }
                                       }
                         };
            var r1 = result.SelectMany(m => m.matches).Select(j => j.jet.pt).Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsFalse(qm.ContainsClassRef("Anon", "Jet", "TruthJet", "MatchedJet", "matchedEvent"), "still has bogusobject");
        }

        [TestMethod]
        public void CreateFullEventNoCrossCutAllAnonMin()
        {
            var q = new QueriableDummyNoExe<LINQToTTreeLib.ResultOperators.ROFirstLastTest.ntup3>();
            var result = from d in q
                         select new
                         {
                             matches = from j in d.run1.Select(i => new { pt = i })
                                       select j
                         };
            var r1 = result.SelectMany(m => m.matches).Select(j => j.pt).Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsFalse(qm.ContainsClassRef("Anon", "Jet", "TruthJet", "MatchedJet", "matchedEvent"), "still has bogusobject");
        }

        [TestMethod]
        public void CreateFullEventNoCrossCutOnJetsAndAnonEventObject()
        {
            var q = new QueriableDummyNoExe<LINQToTTreeLib.ResultOperators.ROFirstLastTest.ntup3>();
            var result = from d in q
                         select new
                         {
                             matches = from j in d.run1.Select(i => new Jet() { pt = i })
                                       select new MatchedJet()
                                       {
                                           jet = j,
                                           truth = new TruthJet() { pt = j.pt }
                                       }
                         };
            var r1 = result.SelectMany(m => m.matches).Select(j => j.jet.pt).Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsFalse(qm.ContainsClassRef("Anon", "Jet", "TruthJet", "MatchedJet", "matchedEvent"), "still has bogusobject");
        }


        [TestMethod]
        public void CreateFullEventNoCrossCutOnJetsAndAnonEventObjectAndAnonMatchedJet()
        {
            var q = new QueriableDummyNoExe<LINQToTTreeLib.ResultOperators.ROFirstLastTest.ntup3>();
            var result = from d in q
                         select new
                         {
                             matches = from j in d.run1.Select(i => new Jet() { pt = i })
                                       select new
                                       {
                                           jet = j,
                                           truth = new TruthJet() { pt = j.pt }
                                       }
                         };
            var r1 = result.SelectMany(m => m.matches).Select(j => j.jet.pt).Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsFalse(qm.ContainsClassRef("Anon", "Jet", "TruthJet", "MatchedJet", "matchedEvent"), "still has bogusobject");
        }

        [TestMethod]
        public void CreateFullEventNoCrossCutOnJets()
        {
            var q = new QueriableDummyNoExe<LINQToTTreeLib.ResultOperators.ROFirstLastTest.ntup3>();
            var result = from d in q
                         select new matchedEvent
                         {
                             matches = from j in d.run1.Select(i => new Jet() { pt = i })
                                       select new MatchedJet()
                                       {
                                           jet = j,
                                           truth = new TruthJet() { pt = j.pt }
                                       }
                         };
            var r1 = result.SelectMany(m => m.matches).Select(j => j.jet.pt).Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsFalse(qm.ContainsClassRef("Jet", "TruthJet", "MatchedJet", "matchedEvent"), "still has bogusobject");
        }

        [TestMethod]
        public void CreateFullEventCrossCutOnJets()
        {
            var q = new QueriableDummyNoExe<LINQToTTreeLib.ResultOperators.ROFirstLastTest.ntup3>();
            var result = from d in q
                         select new matchedEvent
                         {
                             matches = from j in d.run1.Select(i => new Jet() { pt = i })
                                       select new MatchedJet()
                                       {
                                           jet = j,
                                           truth = (from t in d.run2
                                                    orderby t - j.pt ascending
                                                    select new TruthJet() { pt = t }).First()
                                       }
                         };
            var r1 = result.SelectMany(m => m.matches).Select(j => j.jet.pt).Sum();
            var qm = DummyQueryExectuor.LastQueryModel;
            ReferencedQueryExpressionReplacement.Replace(qm);

            Console.WriteLine("Replaced qm: {0}", qm.ToString());
            Assert.IsFalse(qm.ContainsClassRef("Jet", "TruthJet", "MatchedJet", "matchedEvent"), "still has bogusobject");
        }
    }

    public static class TestHelpers
    {
        public static bool ContainsClassRef(this QueryModel qm, params string[] classnames)
        {
            var s = qm.ToString();
            Console.WriteLine("QM: {0}", s);

            return classnames.Where(cname => s.Contains(cname)).Any();
        }
    }
}
