using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using ROOTNET.Interface;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for QueryResultCache</summary>
    [TestClass]
    public partial class QueryResultCacheTest
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

        internal Tuple<bool, T> Lookup<T>(
            QueryResultCache target,
            Uri _rootFile,
            string treeName,
            object[] inputObjects,
            string[] cacheStrings,
            QueryModel queryModel,
            IVariableSaver varSaver,
            bool checkDates = false,
            Func<IAddResult> generateAdder = null
        )
        {
            var result = target.Lookup<T>(target.GetKey(new Uri[] { _rootFile }, treeName, inputObjects, cacheStrings, queryModel, recheckDates: checkDates, dateChecker: u => File.GetLastWriteTime(u.LocalPath)), varSaver, null, generateAdder);
            Assert.IsNotNull(result, "Should never return a null lookup");
            return result;
        }

        /// <summary>
        /// Dummy saver and loader for a variable.
        /// </summary>
        class DummySaver : IVariableSaver
        {
            public bool CanHandle(IDeclaredParameter iVariable)
            {
                throw new NotImplementedException();
            }

            public System.Collections.Generic.IEnumerable<string> SaveToFile(IDeclaredParameter iVariable)
            {
                throw new NotImplementedException();
            }

            public System.Collections.Generic.IEnumerable<string> IncludeFiles(IDeclaredParameter iVariable)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Return the list of all the things we want to cache to make up this variable.
            /// </summary>
            /// <param name="iVariable"></param>
            /// <returns></returns>
            public string[] GetCachedNames(IDeclaredParameter iVariable)
            {
                return new string[] { iVariable.RawValue };
            }

            /// <summary>
            /// We return an int! :-)
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="iVariable"></param>
            /// <param name="obj"></param>
            /// <returns></returns>
            public T LoadResult<T>(IDeclaredParameter iVariable, NTObject[] obj, int cycle)
            {
                var h = obj[0] as ROOTNET.Interface.NTH1F;
                if (h == null)
                    throw new InvalidOperationException("must be a histogram that is passed");

                int result = (int)h.GetBinContent(1);
                object i = result;
                return (T)i;
            }

            public void RenameForQueryCycle(IDeclaredParameter iVariable, NTObject[] obj, int cycle)
            {
            }
        }

        /// <summary>
        /// Histogram return
        /// </summary>
        class DummyHistoSaver : IVariableSaver
        {
            /// <summary>
            /// Return the list of all the things we want to cache to make up this variable.
            /// </summary>
            /// <param name="iVariable"></param>
            /// <returns></returns>
            public string[] GetCachedNames(IDeclaredParameter iVariable)
            {
                return new string[] { iVariable.RawValue };
            }

            public bool CanHandle(IDeclaredParameter iVariable)
            {
                throw new NotImplementedException();
            }

            public System.Collections.Generic.IEnumerable<string> SaveToFile(IDeclaredParameter iVariable)
            {
                throw new NotImplementedException();
            }

            public System.Collections.Generic.IEnumerable<string> IncludeFiles(IDeclaredParameter iVariable)
            {
                throw new NotImplementedException();
            }

            public T LoadResult<T>(IDeclaredParameter iVariable, NTObject[] obj, int cycle)
            {
                var h = obj[0].Clone();
                return (T)h;
            }
            public void RenameForQueryCycle(IDeclaredParameter iVariable, NTObject[] obj, int cycle)
            {
            }
        }

        [TestMethod]
        public void TestNoHit()
        {
            var f = MakeRootFile("TestNoHit");
            var query = MakeQuery(0);

            Assert.IsFalse(Lookup<int>(new QueryResultCache(), f, "test", null, null, query, new DummySaver()).Item1, "cache should be empty for this guy!");
        }

        /// <summary>
        /// Create a query model that we can use for tests. We have several possible ones we can create that should be different.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private QueryModel MakeQuery(int queryIndex)
        {
            if (queryIndex == 0)
            {
                var q = new QueriableDummy<ntup>();
                var result = from d in q
                             select d;
                var c = result.Count();

                return DummyQueryExectuor.LastQueryModel;
            }

            if (queryIndex == 1)
            {
                var q = new QueriableDummy<ntup>();
                var result = from d in q
                             where d.run > 20
                             select d;
                var c = result.Count();

                return DummyQueryExectuor.LastQueryModel;
            }

            if (queryIndex == 2)
            {
                var q = new QueriableDummy<ntup>();
                var result = from d in q
                             select d.run;
                var r = result.Plot("hi", "there", 20, 0.0, 10.0);
                return DummyQueryExectuor.LastQueryModel;
            }
            if (queryIndex == 3)
            {
                var q = new QueriableDummy<ntup>();
                var result = from d in q
                             select d.run;
                var r = result.Plot("hi", "there", 40, 0.0, 10.0);
                return DummyQueryExectuor.LastQueryModel;
            }
            if (queryIndex == 4)
            {
                var q = new QueriableDummy<ntup>();
                var result = from d in q
                             select d.run;
                var r = result.Plot("hi1", "there is no spoon", 20, 0.0, 10.0);
                return DummyQueryExectuor.LastQueryModel;
            }

            ///
            /// Ops! Anything else and they are asking too much! :-)
            /// 

            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a pretend root file.
        /// </summary>
        /// <param name="subDirName">A name we can use to write this in - to keep tests separate</param>
        /// <returns>URI to the root file</returns>
        /// <remarks>
        /// This creates the file in the temp area. It turns out to be important that this file and the query cache
        /// files be created on the same disk. Otherwise subtle timing issues can come into play. Yeah. Surprised the
        /// heck out of me too.
        /// </remarks>
        private Uri MakeRootFile(string subDirName)
        {
            var inf = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LINQToTTreeTests\\" + subDirName);
            if (inf.Exists)
                inf.Delete(true);
            inf.Create();
            var f = new FileInfo(inf.FullName + "\\bogus.root");
            using (var r = f.CreateText())
            {
                r.WriteLine("hi");
                r.Close();
            }
            return new Uri("file://" + f.FullName);
        }

        [TestMethod]
        public void TestHit0()
        {
            TestHitDriver(0);
        }

        [TestMethod]
        public void TestHit1()
        {
            TestHitDriver(1);
        }

        private void TestHitDriver(int queryIndex)
        {
            var f = MakeRootFile("TestHitDriver");
            TestForCachingOnUri(queryIndex, f);
        }

        /// <summary>
        /// Look to see if we can figure out what the hit is on this Uri.
        /// </summary>
        /// <param name="queryIndex"></param>
        /// <param name="f"></param>
        private void TestForCachingOnUri(int queryIndex, Uri f)
        {
            var query = MakeQuery(queryIndex);

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);
            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", null, null, query, dateChecker: u => File.GetLastWriteTime(u.LocalPath)), new NTObject[] { h });

            var r = Lookup<int>(q, f, "test", null, null, query, new DummySaver());
            Assert.IsTrue(r.Item1, "expected hit");
            Assert.AreEqual(5, r.Item2, "incorrect return value");
        }

        [TestMethod]
        public void CacheSingleInteger()
        {
            // A simple query
            var query = MakeQuery(0);
            var f = new Uri("http://www.nytimes.com");

            // Cache an integer
            var h = new ROOTNET.NTH1F("hi", "there", 1, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);
            var q = new QueryResultCache();
            var date = DateTime.Now;
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", null, null, query, dateChecker: u => date), new NTObject[] { h });

            // Now, do the lookup.
            var r = Lookup<int>(q, f, "test", null, null, query, new DummySaver());
            Assert.IsTrue(r.Item1, "expected hit");
            Assert.AreEqual(5, r.Item2, "incorrect return value");
        }

        [TestMethod]
        public void CacheWithVeryLongSourceFilename()
        {
            // A simple query
            var query = MakeQuery(0);
            var f = new Uri("http://www.nytimes.com/mc15_1111111111111111111111111111111111111111111111111111111111111111111111111113TeV_304810_MadGraphPythia8EvtGen_A14NNPDF23LO_HSS_LLP_mH400_mS50_lt5m_merge_DAOD_EXOT15_e5102_s2698_r7772_r7676_p2877?nFiles=1");

            // Cache an integer
            var h = new ROOTNET.NTH1F("hi", "there", 1, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);
            var q = new QueryResultCache();
            var date = DateTime.Now;
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", null, null, query, dateChecker: u => date), new NTObject[] { h });

            // Now, do the lookup.
            var r = Lookup<int>(q, f, "test", null, null, query, new DummySaver());
            Assert.IsTrue(r.Item1, "expected hit");
            Assert.AreEqual(5, r.Item2, "incorrect return value");
        }

        [TestMethod]
        public void CacheCycledInteger()
        {
            // A simple query
            var query = MakeQuery(0);
            var f = new Uri("http://www.nytimes.com");

            // Cache an integer
            var h1 = new ROOTNET.NTH1F("hi", "there", 1, 0.0, 10.0);
            h1.Directory = null;
            h1.SetBinContent(1, 5.0);
            var h2 = new ROOTNET.NTH1F("hi", "there", 1, 0.0, 10.0);
            h2.Directory = null;
            h2.SetBinContent(1, 2.0);

            var cacheCycles = new NTObject[][] { new NTObject[] { h1 }, new NTObject[] { h2 } };

            var q = new QueryResultCache();
            var date = DateTime.Now;
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", null, null, query, dateChecker: u => date), cacheCycles);

            // Now, do the lookup.
            var r = Lookup<int>(q, f, "test", null, null, query, new DummySaver(), generateAdder: () => new DummyIntAdder());
            Assert.IsTrue(r.Item1, "expected hit");
            Assert.AreEqual(7, r.Item2, "incorrect return value");
        }

        class DummyIntAdder : IAddResult
        {
            public bool CanHandle(Type t)
            {
                throw new NotImplementedException();
            }

            public T Clone<T>(T o)
            {
                return o;
            }

            public T Update<T>(T accumulator, T o2)
            {
                var iacc = accumulator as int?;
                var io2 = o2 as int?;

                return (T) (object) (iacc.Value + io2.Value);
            }
        }

        [TestMethod]
        public void CacheCanDealWithUriWithQueryString()
        {
            var q = new QueryResultCache();
            var query = MakeQuery(0);
            var f = new Uri("remotebash://bogus/dude?nFIles=1&DoItNow");
            var key = q.GetKey(new Uri[] { f }, "test", null, null, query, dateChecker: u => DateTime.Now);
        }

        [TestMethod]
        public void TestNoProofHit()
        {
            // Make sure that a new proof dataset works just fine.
            var f = new Uri("proof://tev03.phys.washington.edu/JetBackToBack_v006_PeriodA");
            var query = MakeQuery(0);

            Assert.IsFalse(Lookup<int>(new QueryResultCache(), f, "test", null, null, query, new DummySaver()).Item1, "cache should be empty for this proof guy!");
        }

        [TestMethod]
        public void TestHitForProofDataset()
        {
            var f = new Uri("proof://tev03.phys.washington.edu/JetBackToBack_v006_PeriodA");
            TestForCachingOnUri(0, f);
        }

        [TestMethod]
        public void TestForFilesInDifferentOrders()
        {
            var f1 = MakeRootFile("TestHitDriver1");
            var f2 = MakeRootFile("TestHitDriver2");
            var query = MakeQuery(0);

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);
            var q = new QueryResultCache();
            var k1 = q.GetKey(new Uri[] { f1, f2 }, "test", null, null, query, dateChecker: u => File.GetLastWriteTime(u.LocalPath));
            var k2 = q.GetKey(new Uri[] { f2, f1 }, "test", null, null, query, dateChecker: u => File.GetLastWriteTime(u.LocalPath));
            q.CacheItem(k1, new NTObject[] { h });

            //
            // Now, do the lookup, but with files in a different order.
            //

            var r1 = q.Lookup<int>(k1, new DummySaver(), null);
            var r2 = q.Lookup<int>(k2, new DummySaver(), null);
            Assert.IsTrue(r1.Item1, "expected hit for same key");
            Assert.IsTrue(r2.Item1, "expected hit for second key with different files");
        }

        [TestMethod]
        public void TestForFileOutOfDate()
        {
            var u = MakeRootFile("TestForFileOutOfDate");
            var f = new FileInfo(u.LocalPath);
            var query = MakeQuery(0);

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);
            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { u }, "test", null, null, query, dateChecker: uf => File.GetLastWriteTime(uf.LocalPath)), new NTObject[] { h });

            /// Modify the file

            using (var w = f.CreateText())
            {
                w.WriteLine("fork it!");
                w.Close();
            }
            f.Refresh();
            System.Threading.Thread.Sleep(250);

            /// And make sure the lookup fails now!

            var r = Lookup<int>(q, u, "test", null, null, query, new DummySaver(), checkDates: true);
            Assert.IsFalse(r.Item1, "altered file should have made this fail");

            // Next, update the cache and look to make sure that the cache returns a hit this time!
            q.CacheItem(q.GetKey(new Uri[] { u }, "test", null, null, query, dateChecker: uf => File.GetLastWriteTime(uf.LocalPath)), new NTObject[] { h });
            r = Lookup<int>(q, u, "test", null, null, query, new DummySaver(), checkDates: true);
            Assert.IsTrue(r.Item1, "altered file should have made this fail");
        }

        [TestMethod]
        public void TestForFileOutOfDateNoCheck()
        {
            var f = MakeRootFile("TestForFileOutOfDate");
            var query = MakeQuery(0);

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);
            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", null, null, query, dateChecker: u => File.GetLastWriteTime(u.LocalPath)), new NTObject[] { h });

            /// Modify the file

            using (var w = File.CreateText(f.LocalPath))
            {
                w.WriteLine("fork it!");
                w.Close();
            }

            /// And make sure the lookup fails now!

            var r = Lookup<int>(q, f, "test", null, null, query, new DummySaver());
            Assert.IsTrue(r.Item1, "altered file should not have triggered the re-check");
        }

        [TestMethod]
        public void TestForNonTObjectCaching()
        {
            var f = MakeRootFile("TestForNonTObjectCaching");
            var query = MakeQuery(0);

            /// Cache a result

            var h = new ROOTNET.NTLorentzVector(1.0, 2.0, 3.0, 4.0);
            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", null, null, query, dateChecker: u => File.GetLastWriteTime(u.LocalPath)), new NTObject[] { h });

            /// And make sure the lookup gets back the same object!

            var r = Lookup<ROOTNET.Interface.NTLorentzVector>(q, f, "test", null, null, query, new DummyHistoSaver());
            Assert.IsTrue(r.Item1, "SHould get back the same object");
            Assert.IsNotNull(r.Item2, "tlz should not be null");
            Assert.AreEqual(1.0, r.Item2.X(), "x value");
            Assert.AreEqual(2.0, r.Item2.Y(), "y value");
            Assert.AreEqual(4.0, r.Item2.T(), "t value");
        }

        [TestMethod]
        public void TestForTreeNameChanges()
        {
            var f = MakeRootFile("TestForTreeNameChanges");
            var query = MakeQuery(0);

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);
            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", null, null, query, dateChecker: u => File.GetLastWriteTime(u.LocalPath)), new NTObject[] { h });

            /// And make sure the lookup fails now!

            var r = Lookup<int>(q, f, "test1", null, null, query, new DummySaver());
            Assert.IsFalse(r.Item1, "different tree should have made this fail");
        }

        [TestMethod]
        public void TestForSameHistos()
        {
            var f = MakeRootFile("TestForTreeNameChanges");
            var query = MakeQuery(0);

            /// Histogram that is feed as input

            var hInput = new ROOTNET.NTH1F("ops", "notthere", 10, 0.0, 30.0);
            hInput.Directory = null;
            hInput.SetBinContent(2, 5.0);

            var inputs = new object[] { hInput };

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);

            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", inputs, null, query, dateChecker: u => File.GetLastWriteTime(u.LocalPath)), new NTObject[] { h });

            /// And make sure the lookup works now!

            var hInputLookup = new ROOTNET.NTH1F("ops", "notthere", 10, 0.0, 30.0);
            hInputLookup.Directory = null;
            hInputLookup.SetBinContent(2, 5.0);

            var r = Lookup<int>(q, f, "test", new object[] { hInputLookup }, null, query, new DummySaver());
            Assert.IsTrue(r.Item1, "Cache should have been there");

        }

        [TestMethod]
        public void TestForSameHistosEmpty()
        {
            var f = MakeRootFile("TestForTreeNameChanges");
            var query = MakeQuery(0);
            var q = new QueryResultCache();

            /// Histogram that is feed as input

            {
                var hInput = new ROOTNET.NTH1F("ops", "notthere", 10, 0.0, 30.0);
                hInput.Directory = null;

                var inputs = new object[] { hInput };

                /// Cache a result

                var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
                h.Directory = null;
                h.SetBinContent(1, 5.0);

                q.CacheItem(q.GetKey(new Uri[] { f }, "test", inputs, null, query, dateChecker: u => File.GetLastWriteTime(u.LocalPath)), new NTObject[] { h });
            }

            /// And make sure the lookup works now!

            var hInputLookup = new ROOTNET.NTH1F("ops", "notthere", 10, 0.0, 30.0);
            hInputLookup.Directory = null;

            var r = Lookup<int>(q, f, "test", new object[] { hInputLookup }, null, query, new DummySaver());
            Assert.IsTrue(r.Item1, "Cache should have been there");

        }

        [TestMethod]
        public void TestForDiffHistos()
        {
            var f = MakeRootFile("TestForTreeNameChanges");
            var query = MakeQuery(0);

            /// Histogram that is feed as input

            var hInput = new ROOTNET.NTH1F("ops", "notthere", 10, 0.0, 30.0);
            hInput.Directory = null;
            hInput.SetBinContent(2, 5.0);

            var inputs = new object[] { hInput };

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);

            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", inputs, null, query, dateChecker: u => File.GetLastWriteTime(u.LocalPath)), new NTObject[] { h });

            /// And make sure the lookup works now!

            var hInputLookup = new ROOTNET.NTH1F("ops", "notthere", 10, 0.0, 30.0);
            hInputLookup.Directory = null;
            hInputLookup.SetBinContent(2, 5.5);

            var r = Lookup<int>(q, f, "test", new object[] { hInputLookup }, null, query, new DummySaver());
            Assert.IsFalse(r.Item1, "Cache should have been there");
        }

        [TestMethod]
        public void TestForDiffResultHistos()
        {
            var f = MakeRootFile("TestForDiifResultHistos");
            var query = MakeQuery(2);

            var inputs = new object[0];

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);

            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", inputs, null, query, dateChecker: u => File.GetLastWriteTime(u.LocalPath)), new NTObject[] { h });

            /// And make sure the lookup works now - make a different query, which is the same
            /// but with a slightly different query guy.

            var query1 = MakeQuery(3);
            var r = Lookup<int>(q, f, "test", new object[0], null, query1, new DummySaver());
            Assert.IsFalse(r.Item1, "Unexpected cache hit");
        }

        [TestMethod]
        public void TestForSameResultHistosDiffNameTitle()
        {
            var f = MakeRootFile("TestForSameResultHistosDiffNameTitle");
            var query = MakeQuery(2);

            var inputs = new object[0];

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);

            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", inputs, null, query, dateChecker: u => File.GetLastWriteTime(u.LocalPath)), new NTObject[] { h });

            /// And make sure the lookup works now - make a different query, which is the same
            /// but with a slightly different query guy.

            var query1 = MakeQuery(4);
            var r = Lookup<int>(q, f, "test", new object[0], null, query1, new DummySaver());
            Assert.IsTrue(r.Item1, "Expected a cache hit");
        }

        [TestMethod]
        public void TestNoStuckInOpenFile()
        {
            // When we load up and return a cache, we are storing a histogram in the file - make sure it comes back w/out errors.
            var f = MakeRootFile("TestHitDriver");
            var query = MakeQuery(0);

            // Cache a result
            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);
            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", null, null, query, dateChecker: u => File.GetLastWriteTime(u.LocalPath)), new NTObject[] { h });

            // Do the lookup.
            var r = Lookup<ROOTNET.Interface.NTH1F>(q, f, "test", null, null, query, new DummyHistoSaver());
            Assert.IsTrue(r.Item1, "expected hit");
            Assert.AreEqual("hi", r.Item2.Name, "improper histogram came back");

        }
    }
}
