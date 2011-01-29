// <copyright file="QueryResultCacheTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.IO;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Data.Linq;
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
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;
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
            FileInfo _rootFile,
            QueryModel qm,
            NTObject o
        )
        {
            target.CacheItem(_rootFile, qm, o);
            // TODO: add assertions to method QueryResultCacheTest.CacheItem(QueryResultCache, FileInfo, QueryModel, NTObject)
        }

        /// <summary>Test stub for Lookup(FileInfo, QueryModel, IVariable)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        internal Tuple<bool, T> Lookup<T>(
            [PexAssumeUnderTest]QueryResultCache target,
            FileInfo _rootFile,
            QueryModel queryModel,
            IVariableSaver varSaver
        )
        {
            var result = target.Lookup<T>(_rootFile, queryModel, varSaver, null);
            Assert.IsNotNull(result, "Should never return a null lookup");
            return result;
            // TODO: add assertions to method QueryResultCacheTest.Lookup(QueryResultCache, FileInfo, QueryModel, IVariable)
        }

        /// <summary>
        /// Dummy saver and loader for a variable.
        /// </summary>
        class DummySaver : IVariableSaver
        {
            public bool CanHandle(IVariable iVariable)
            {
                throw new NotImplementedException();
            }

            public System.Collections.Generic.IEnumerable<string> SaveToFile(IVariable iVariable)
            {
                throw new NotImplementedException();
            }

            public System.Collections.Generic.IEnumerable<string> IncludeFiles(IVariable iVariable)
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
            public T LoadResult<T>(IVariable iVariable, NTObject obj)
            {
                var h = obj as ROOTNET.Interface.NTH1F;
                if (h == null)
                    throw new InvalidOperationException("must be a histo that is passed");

                int result = (int)h.GetBinContent(1);
                object i = result;
                return (T)i;
            }
        }


        [TestMethod]
        public void TestNoHit()
        {
            var f = MakeRootFile("TestNoHit");
            var query = MakeQuery(0);

            Assert.IsFalse(Lookup<int>(new QueryResultCache(), f, query, new DummySaver()).Item1, "cache should be empty for this guy!");
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
        private FileInfo MakeRootFile(string subDirName)
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
            return f;
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
            var f = MakeRootFile("TestNoHit");
            var query = MakeQuery(queryIndex);

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.SetBinContent(1, 5.0);
            var q = new QueryResultCache();
            q.CacheItem(f, query, h);

            var r = Lookup<int>(q, f, query, new DummySaver());
            Assert.IsTrue(r.Item1, "expected hit");
            Assert.AreEqual(5, r.Item2, "incorrect return value");
        }

        [TestMethod]
        public void TestForFileOutOfDate()
        {
            var f = MakeRootFile("TestNoHit");
            var query = MakeQuery(0);

            /// Cache a result

            var h = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0);
            h.SetBinContent(1, 5.0);
            var q = new QueryResultCache();
            q.CacheItem(f, query, h);

            /// Modify the file

            using (var w = f.CreateText())
            {
                w.WriteLine("fork it!");
                w.Close();
            }

            /// And make sure the lookup fails now!

            var r = Lookup<int>(q, f, query, new DummySaver());
            Assert.IsFalse(r.Item1, "altered file should have made this fail");
        }

    }
}
