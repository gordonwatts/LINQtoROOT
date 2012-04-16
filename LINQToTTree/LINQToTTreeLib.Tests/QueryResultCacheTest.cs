// <copyright file="QueryResultCacheTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.IO;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using ROOTNET.Interface;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for QueryResultCache</summary>
    [PexClass(typeof(QueryResultCache))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class QueryResultCacheTest
    {
        public static void SetupCacheDir()
        {
            var cdir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LINQToTTree\\QueryCacheForTesting");
            if (cdir.Exists)
                cdir.Delete(true);
            QueryResultCache.CacheDirectory = cdir;
        }
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            MEFUtilities.AddPart(new TypeHandlers.TypeHandlerConvert());
            DummyQueryExectuor.GlobalInitalized = false;
            QueryResultCacheTest.SetupCacheDir();
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>Test stub for CacheItem(FileInfo, QueryModel, NTObject)</summary>
        [PexMethod]
        internal void CacheItem(
            [PexAssumeUnderTest]QueryResultCache target,
            object[] inputObjs,
            Uri _rootFile,
            string[] cachecookies,
            QueryModel qm,
            NTObject o
        )
        {
            target.CacheItem(target.GetKey(new Uri[] { _rootFile }, "test", inputObjs, cachecookies, qm), o);
            // TODO: add assertions to method QueryResultCacheTest.CacheItem(QueryResultCache, FileInfo, QueryModel, NTObject)
        }

        /// <summary>Test stub for Lookup(FileInfo, QueryModel, IVariable)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        internal Tuple<bool, T> Lookup<T>(
            [PexAssumeUnderTest]QueryResultCache target,
            Uri _rootFile,
            string treeName,
            object[] inputObjects,
            string[] cacheStrings,
            QueryModel queryModel,
            IVariableSaver varSaver,
            bool checkDates = false
        )
        {
            var result = target.Lookup<T>(target.GetKey(new Uri[] { _rootFile }, treeName, inputObjects, cacheStrings, queryModel, recheckDates: checkDates), varSaver, null);
            Assert.IsNotNull(result, "Should never return a null lookup");
            return result;
            // TODO: add assertions to method QueryResultCacheTest.Lookup(QueryResultCache, FileInfo, QueryModel, IVariable)
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
            /// We return an int! :-)
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="iVariable"></param>
            /// <param name="obj"></param>
            /// <returns></returns>
            public T LoadResult<T>(IDeclaredParameter iVariable, NTObject obj)
            {
                var h = obj as ROOTNET.Interface.NTH1F;
                if (h == null)
                    throw new InvalidOperationException("must be a histo that is passed");

                int result = (int)h.GetBinContent(1);
                object i = result;
                return (T)i;
            }
        }

        /// <summary>
        /// Histo return
        /// </summary>
        class DummyHistoSaver : IVariableSaver
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

            public T LoadResult<T>(IDeclaredParameter iVariable, NTObject obj)
            {
                var h = obj.Clone();
                return (T)h;
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
        /// Create a query model that we cna use for tests. We have several possible ones we can create that should be different.
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
                var r = result.Plot("hi1", "there is no sppon", 20, 0.0, 10.0);
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
        /// <param name="subDirName"></param>
        /// <returns></returns>
        private Uri MakeRootFile(string subDirName)
        {
            DirectoryInfo inf = new DirectoryInfo(".\\" + subDirName);
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
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", null, null, query), h);

            var r = Lookup<int>(q, f, "test", null, null, query, new DummySaver());
            Assert.IsTrue(r.Item1, "expected hit");
            Assert.AreEqual(5, r.Item2, "incorrect return value");
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
            var k1 = q.GetKey(new Uri[] { f1, f2 }, "test", null, null, query);
            var k2 = q.GetKey(new Uri[] { f2, f1 }, "test", null, null, query);
            q.CacheItem(k1, h);

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
            q.CacheItem(q.GetKey(new Uri[] { u }, "test", null, null, query), h);

            /// Modify the file

            System.Threading.Thread.Sleep(500);
            using (var w = f.CreateText())
            {
                w.WriteLine("fork it!");
                w.Close();
            }
            f.Refresh();

            /// And make sure the lookup fails now!

            var r = Lookup<int>(q, u, "test", null, null, query, new DummySaver(), checkDates: true);
            Assert.IsFalse(r.Item1, "altered file should have made this fail");

            // Next, update the cache and look to make sure that the cache returns a hit this time!
            q.CacheItem(q.GetKey(new Uri[] { u }, "test", null, null, query), h);
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
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", null, null, query), h);

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
        public void TestForTreeNameChanges()
        {
            var f = MakeRootFile("TestForTreeNameChanges");
            var query = MakeQuery(0);

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);
            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", null, null, query), h);

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
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", inputs, null, query), h);

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

                q.CacheItem(q.GetKey(new Uri[] { f }, "test", inputs, null, query), h);
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
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", inputs, null, query), h);

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
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", inputs, null, query), h);

            /// And make sure the lookup works now - make a different query, which is the same
            /// but with a slightly different query guy.

            var query1 = MakeQuery(3);
            var r = Lookup<int>(q, f, "test", new object[0], null, query1, new DummySaver());
            Assert.IsFalse(r.Item1, "Unexpected cache hit");
        }

        [TestMethod]
        public void TestForSameResultHistosDiffNameTitle()
        {
            var f = MakeRootFile("TestForDiifResultHistos");
            var query = MakeQuery(2);

            var inputs = new object[0];

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);

            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", inputs, null, query), h);

            /// And make sure the lookup works now - make a different query, which is the same
            /// but with a slightly different query guy.

            var query1 = MakeQuery(4);
            var r = Lookup<int>(q, f, "test", new object[0], null, query1, new DummySaver());
            Assert.IsTrue(r.Item1, "Expected a cache hit");
        }

        [TestMethod]
        public void TestNoStuckInOpenFile()
        {
            ///
            /// When we load up and return a cache, we are storing a histo in the file - make sure it comes back w/out errors.
            ///

            var f = MakeRootFile("TestHitDriver");
            var query = MakeQuery(0);

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.Directory = null;
            h.SetBinContent(1, 5.0);
            var q = new QueryResultCache();
            q.CacheItem(q.GetKey(new Uri[] { f }, "test", null, null, query), h);

            var r = Lookup<ROOTNET.Interface.NTH1F>(q, f, "test", null, null, query, new DummyHistoSaver());
            Assert.IsTrue(r.Item1, "expected hit");
            Assert.AreEqual("hi", r.Item2.Name, "inproper histo came back");

        }
    }
}
