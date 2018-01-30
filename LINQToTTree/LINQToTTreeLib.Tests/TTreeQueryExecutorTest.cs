using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LINQToTTreeLib.Files;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Diagnostics;
using static LINQToTTreeLib.TTreeQueryExecutor;
using Remotion.Linq;
using System.Threading.Tasks;
using LINQToTTreeLib.ExecutionCommon;

namespace LINQToTTreeLib
{
    [TestClass]
    [DeploymentItem(@"Templates\TSelectorTemplate.cxx")]
    public partial class TTreeQueryExecutorTest
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();

            ntuple.Reset();
            ntuple._gCINTLines = null;
            ntuple._gObjectFiles = null;
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupe
        {
#pragma warning disable 0169
            public int run;
#pragma warning restore 0169
        }

        /// <summary>
        /// Ntuple with nothing in it - proxy and obj files missing.
        /// </summary>
        public class ntuple_nothing
        {

        }

        /// <summary>
        /// Ntuple with a proxy guy
        /// </summary>
        public class ntuple_with_proxy
        {
        }

        /// <summary>
        /// Ntuple with emptys for everything.
        /// </summary>
        public class ntuple
        {
            public static string[] _gObjectFiles = { };
            public static string[] _gCINTLines = { };

            internal static void Reset()
            {
                _gObjectFiles = new string[0];
            }
        }

        /// <summary>
        /// Same, in case we need two at once
        /// </summary>
        public class ntuple2
        {
            public static string _gProxyFile = "";
            public static string[] _gObjectFiles = { };
            public static string[] _gCINTLines = { };

            internal static void Reset()
            {
                _gProxyFile = "";
                _gObjectFiles = new string[0];
            }
        }

        /// <summary>
        /// Create a file that can be loaded by ACLIC
        /// </summary>
        /// <param name="objName"></param>
        /// <returns></returns>
        public static FileInfo CreateCommonObject(string objName, DirectoryInfo baseDir)
        {
            FileInfo result = new FileInfo(baseDir + "\\" + objName + ".cpp");
            using (var writer = result.CreateText())
            {
                writer.WriteLine("#include \"{0}.hpp\"", objName);
                writer.WriteLine("ClassImp({0});", objName);
                writer.Close();
            }

            FileInfo resulth = new FileInfo(baseDir + "\\" + objName + ".hpp");
            using (var writer = resulth.CreateText())
            {
                writer.WriteLine("#ifndef __{0}__", objName);
                writer.WriteLine("#define __{0}__", objName);
                writer.WriteLine("#include <TObject.h>");
                writer.WriteLine("class {0} : public TObject {{", objName);
                writer.WriteLine("public:");
                writer.WriteLine("  int j;");
                writer.WriteLine("  ClassDef({0},2);", objName);
                writer.WriteLine("};");
                writer.WriteLine("#endif");
                writer.WriteLine();

                writer.Close();
            }

            return result;
        }

        [CPPHelperClass]
        public static class CPPHelperFunctions
        {
            [CPPCode(Code = new string[] { "Calc = arg*2;" })]
            public static int Calc(int arg)
            {
                throw new NotImplementedException();
            }

            [CPPCode(Code = new string[] { "CalcLen = strlen(arg);" }, IncludeFiles = new string[] { "stdlib.h" })]
            public static int CalcLen(string arg)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Simple custom function in an include file. The include file must be written locally.
            /// </summary>
            /// <param name="arg"></param>
            /// <returns></returns>
            [CPPCode(Code = new string[] { "ReturnCustomFuncValue = bogus();" }, IncludeFiles = new string[] { "bogus_function.h" })]
            public static int ReturnCustomFuncValue()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupeArr : IExpressionHolder
        {
            public TestNtupeArr(Expression holder)
            {
                HeldExpression = holder;
            }
#pragma warning disable 0169
            public int[] myvectorofint;
#pragma warning restore 0169

            public System.Linq.Expressions.Expression HeldExpression { get; set; }
        }

        public class TestNtupeArrD : IExpressionHolder
        {
            public TestNtupeArrD(Expression holder)
            {
                HeldExpression = holder;
            }
#pragma warning disable 0169
            public double[] myvectorofdouble;
#pragma warning restore 0169

            public System.Linq.Expressions.Expression HeldExpression { get; set; }
        }

        public class TestNtupeArrJets
        {
            [TTreeVariableGrouping]
            public int myvectorofint;
        }

        [TranslateToClass(typeof(TestNtupeArr))]
        public class TestNtupeArrEvents
        {
            [TTreeVariableGrouping]
            public TestNtupeArrJets[] jets;
        }

        /// <summary>
        /// Test out a simple result base ntuple object base.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSimpleReultOperatorNtupleEmptyObj()
        {
            int numberOfIter = 10;

            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple_nothing));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        /// <summary>
        /// Test out a simple result base ntuple object base and null proxy file base.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSimpleReultOperatorNtupleWithProxy()
        {
            int numberOfIter = 10;

            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple_with_proxy));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        /// <summary>
        /// Test out a simple result operator.
        /// </summary>
        [TestMethod]
        public void TestSimpleResultOperator()
        {
            RunSimpleCountResult(10);
        }

        private void RunSimpleCountResult(int numberOfIter)
        {
            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        // Bogus Test Files
        public class TestNtupeFull
        {
#pragma warning disable 0169
            public int run;
#pragma warning restore 0169

            public static string[] _gObjectFiles = null;
            public static string[] _gCINTLines = null;
        }

        [TestMethod]
        public async Task CachedValueCanBeAWaited()
        {
            const int numberOfIter = 10;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter);
            var q1 = new QueriableTTree<TestNtupeFull>(rootFile1, "dude");

            // This should wait while we execute
            var dude1 = await q1.FutureCount();

            // This should fetch the value from the cache, and so return right away.
            var dude2 = await q1.FutureCount();

            Assert.AreEqual(dude1, dude2);
        }

        [TestMethod]
        public async Task CachedValueCanBeAWaitedFromTwoSources()
        {
            const int numberOfIter = 10;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter);
            var q1 = new QueriableTTree<TestNtupeFull>(rootFile1, "dude");
            var q2 = new QueriableTTree<TestNtupeFull>(rootFile1, "dude");

            // This should wait while we execute
            var dude1 = await q1.FutureCount();

            // This should fetch the value from the cache, and so return right away.
            var dude2 = await q2.FutureCount();

            Assert.AreEqual(dude1, dude2);
        }

        [TestMethod]
        public async Task WaitTwiceOnValue()
        {
            const int numberOfIter = 10;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter);
            var q1 = new QueriableTTree<TestNtupeFull>(rootFile1, "dude");

            // This should wait while we execute
            var tsk = q1.FutureCount();
            var dude1 = await tsk;
            var dude2 = await tsk;

            Assert.AreEqual(dude1, dude2);
        }

        [TestMethod]
        [Ignore]
        // Seemst o have died having to do with data manager stuff. Need to investigate!
        public async Task SimpleResultOperatorWithGridDSLocationLocal()
        {
            // Use a file that is on the GRID. This might change over time,
            // so this may file as files are deleted and need to be updated.

            const string dsName = "user.gwatts.361032.Pythia8EvtGen_A14NNPDF23LO_jetjet_JZ12W.DAOD_EXOT15.p2711.DiVertAnalysis_v15_C448D50D_22DCBF53_hist";
            var files = await AtlasWorkFlows.DataSetManager.ListOfFilesInDataSetAsync(dsName);
            var gfile = files.First();
            var places = await AtlasWorkFlows.DataSetManager.ListOfPlacesHoldingAllFilesAsync(new[] { gfile });
            Assert.IsTrue(places.Contains("Local"));
            var gfileUri = await AtlasWorkFlows.DataSetManager.LocalPathToFileAsync("Local", gfile);

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { gfileUri }, "recoTree", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(98600, result);
        }

        [TestMethod]
        [Ignore]
        // Seems to have died due to data manager stuff. Need to investigate!
        public async Task SimpleResultOperatorWithGridDSLocationRemote()
        {
            // Use a file that is on the GRID. This might change over time,
            // so this may file as files are deleted and need to be updated.

            const string dsName = "user.gwatts.361032.Pythia8EvtGen_A14NNPDF23LO_jetjet_JZ12W.DAOD_EXOT15.p2711.DiVertAnalysis_v15_C448D50D_22DCBF53_hist";
            var files = await AtlasWorkFlows.DataSetManager.ListOfFilesInDataSetAsync(dsName);
            var gfile = files.First();
            var places = await AtlasWorkFlows.DataSetManager.ListOfPlacesHoldingAllFilesAsync(new[] { gfile });
            Assert.IsTrue(places.Contains("UWTeV-linux"));
            var gfileUriR = await AtlasWorkFlows.DataSetManager.LocalPathToFileAsync("UWTeV-linux", gfile);
            var gfileUri = new UriBuilder(gfileUriR)
            {
                Scheme = "remotebash"
            };

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { gfileUri.Uri}, "recoTree", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(98600, result);
        }

        [TestMethod]
        public void RunSimpleConcatSameSourceCountResult()
        {
            const int numberOfIter = 10;
            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);

            var q = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFile }, "dude", typeof(ntuple));
            var dude = q.Concat(q).Count();

            Assert.AreEqual(numberOfIter*2, dude);
        }

        // Do a quick translation switching Uri's back and forth.
        [Export(typeof(IDataFileSchemeHandler))]
        class UriOneToOneTranslator : IDataFileSchemeHandler
        {
            public string Scheme => "test1to1scheme";

            public DateTime GetUriLastModificationDate(Uri u)
            {
                return DateTime.Now;
            }

            public bool GoodUri(Uri u)
            {
                return true;
            }

            public Uri Normalize(Uri u)
            {
                return u;
            }

            /// <summary>
            /// Return it to being a regular file
            /// </summary>
            /// <param name="u"></param>
            /// <returns></returns>
            public Task<IEnumerable<Uri>> ResolveUri(Uri u)
            {
                return Task.FromResult(new[] { new UriBuilder(u) { Scheme = "file" }.Uri }.AsEnumerable());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DataSchemeNotKnownException))]
        public void UriTranslatedNoSchemeHandlerAtAll()
        {
            const int numberOfIter = 10;
            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);
            var testUri = new UriBuilder(rootFile) { Scheme = "test1to1scheme" }.Uri;

            // Query model
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { testUri }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        [ExpectedException(typeof(BadUriException))]
        public void UriTranslatedSchemeHandlerFalsesOut()
        {
            const int numberOfIter = 10;
            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);
            var testUri = new UriBuilder(rootFile) { Scheme = "test1to1scheme" }.Uri;

            // Query model
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Add the translator to the TTExecutor's
            var myBatch = new CompositionBatch();
            myBatch.AddPart(new UriFalseScheme());
            TTreeQueryExecutor.CContainer.Compose(myBatch);

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { testUri }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        // Do a quick translation switching Uri's back and forth.
        [Export(typeof(IDataFileSchemeHandler))]
        class UriFalseScheme : IDataFileSchemeHandler
        {
            public string Scheme => "test1to1scheme";

            public DateTime GetUriLastModificationDate(Uri u)
            {
                return DateTime.Now;
            }

            public bool GoodUri(Uri u)
            {
                return false;
            }

            public Uri Normalize(Uri u)
            {
                return u;
            }

            /// <summary>
            /// Return it to being a regular file
            /// </summary>
            /// <param name="u"></param>
            /// <returns></returns>
            public Task<IEnumerable<Uri>> ResolveUri(Uri u)
            {
                return Task.FromResult(new[] { new UriBuilder(u) { Scheme = "file" }.Uri }.AsEnumerable());
            }
        }

        [TestMethod]
        public void UriTranslatedToOtherType()
        {
            const int numberOfIter = 10;
            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);
            var testUri = new UriBuilder(rootFile) { Scheme = "test1to1scheme" }.Uri;

            // Query model
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Add the translator to the TTExecutor's
            var myBatch = new CompositionBatch();
            myBatch.AddPart(new UriOneToOneTranslator());
            TTreeQueryExecutor.CContainer.Compose(myBatch);

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { testUri }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void UriTranslatedDoesQuery()
        {
            const int numberOfIter = 10;
            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);
            var testUri = new UriBuilder(rootFile) { Scheme = "testsubqueryscheme" }.Uri;
            UriSubQueryScheme.ROOTFile = rootFile;

            // Query model
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Add the translator to the TTExecutor's
            var myBatch = new CompositionBatch();
            myBatch.AddPart(new UriSubQueryScheme());
            TTreeQueryExecutor.CContainer.Compose(myBatch);

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { testUri }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        /// <summary>
        /// Execute little sub-query to see if some order of recursiveness can be done by the system.
        /// </summary>
        [Export(typeof(IDataFileSchemeHandler))]
        class UriSubQueryScheme : IDataFileSchemeHandler
        {
            public static Uri ROOTFile { get; internal set; }

            public string Scheme => "testsubqueryscheme";

            public DateTime GetUriLastModificationDate(Uri u)
            {
                return DateTime.Now;
            }

            public bool GoodUri(Uri u)
            {
                return true;
            }

            public Uri Normalize(Uri u)
            {
                return u;
            }

            public Task<IEnumerable<Uri>> ResolveUri(Uri u)
            {
                var f = new UriBuilder(u) { Scheme = "file" }.Uri;

                var q = new QueriableDummy<TestNtupe>();
                var dude = q.Count();
                var query = DummyQueryExectuor.LastQueryModel;

                var exe = new TTreeQueryExecutor(new[] { f }, "dude", typeof(ntuple), typeof(TestNtupe));
                int result = exe.ExecuteScalar<int>(query);
                Assert.AreEqual(10, result);
                return Task.FromResult(new[] { f }.AsEnumerable());
            }
        }

        // Do a quick translation switching Uri's back and forth.
        [Export(typeof(IDataFileSchemeHandler))]
        class UriOneToTwoTranslator : IDataFileSchemeHandler
        {
            public string Scheme => "test1to2scheme";

            public DateTime GetUriLastModificationDate(Uri u)
            {
                return DateTime.Now;
            }

            public bool GoodUri(Uri u)
            {
                return true;
            }

            public Uri Normalize(Uri u)
            {
                return u;
            }

            /// <summary>
            /// Return it to being a regular file
            /// </summary>
            /// <param name="u"></param>
            /// <returns></returns>
            public Task<IEnumerable<Uri>> ResolveUri(Uri u)
            {
                return Task.FromResult(new[] {
                    new UriBuilder(u) { Scheme = "file" }.Uri,
                    new UriBuilder(u) { Scheme = "file" }.Uri,
                }.AsEnumerable());
            }
        }

        [TestMethod]
        public void UriTranslatedToMultipleTypes()
        {
            const int numberOfIter = 10;
            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);
            var testUri = new UriBuilder(rootFile) { Scheme = "test1to2scheme" }.Uri;

            // Query model
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Add the translator to the TTExecutor's
            var myBatch = new CompositionBatch();
            myBatch.AddPart(new UriOneToTwoTranslator());
            TTreeQueryExecutor.CContainer.Compose(myBatch);

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { testUri }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter*2, result);
        }

        [TestMethod]
        public void UriTranslateWithNormalization()
        {
            const int numberOfIter = 10;
            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);
            var testUri = new UriBuilder(rootFile) { Scheme = "testnormalizeuri", Query = "nfiles=10" }.Uri;

            // Query model
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Add the translator to the TTExecutor's
            var myBatch = new CompositionBatch();
            myBatch.AddPart(new UriNormalizer());
            TTreeQueryExecutor.CContainer.Compose(myBatch);

            // Ok, now we can actually see if we can make it "go".
            var s = new System.IO.StringWriter();
            var tracer = new TextWriterTraceListener(s);
            TraceHelpers.Source.Switch = new SourceSwitch("console", "ActivityTracing");
            var tidx = TraceHelpers.Source.Listeners.Add(tracer);
            try
            {
                var exe = new TTreeQueryExecutor(new[] { testUri }, "dude", typeof(ntuple), typeof(TestNtupe));
                int result = exe.ExecuteScalar<int>(query);
                Assert.AreEqual(numberOfIter, result);
            } finally
            {
                TraceHelpers.Source.Flush();
                TraceHelpers.Source.Listeners.Remove(tracer);
            }
        }

        [Export(typeof(IDataFileSchemeHandler))]
        class UriNormalizer : IDataFileSchemeHandler
        {
            public string Scheme => "testnormalizeuri";

            public DateTime GetUriLastModificationDate(Uri u)
            {
                return DateTime.Now;
            }

            public bool GoodUri(Uri u)
            {
                return true;
            }

            public Uri Normalize(Uri u)
            {
                return new UriBuilder(u) { Query = "" }.Uri;
            }

            public Task<IEnumerable<Uri>> ResolveUri(Uri u)
            {
                return Task.FromResult(new[] { new UriBuilder(u) { Scheme = "file", Query="" }.Uri }.AsEnumerable());
            }
        }

        [TestMethod]
        public void RunSimpleConcatWithLatetake()
        {
            const int numberOfIter = 10;
            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);

            var q = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFile }, "dude", typeof(ntuple));
            var dude = q.Concat(q).TakePerSource(6).Count();

            Assert.AreEqual(12, dude);
        }

        [TestMethod]
        public void RunSimpleConcatTwoSourceCountResult()
        {
            const int numberOfIter = 10;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter);
            var q1 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFile1 }, "dude", typeof(ntuple));

            var rootFile2 = TestUtils.CreateFileOfVectorDouble(numberOfIter);
            var q2 = new SimpleTTreeExecutorQueriable<TestNtupeArrD>(new[] { rootFile2 }, "dude", typeof(ntuple2));

            var dude = q2.SelectMany(e => e.myvectorofdouble).Select(i => (int) 1).Concat(q1.Select(e => (int) 1)).Count();

            Assert.AreEqual(numberOfIter * 10 + numberOfIter, dude);
        }

        [TestMethod]
        public void RunSimpleConcatTwoSourceAsFilesDifferentQueries()
        {
            const int numberOfIter = 10;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter);
            var q1 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFile1 }, "dude", typeof(ntuple));

            var rootFile2 = TestUtils.CreateFileOfVectorDouble(numberOfIter);
            var q2 = new SimpleTTreeExecutorQueriable<TestNtupeArrD>(new[] { rootFile2 }, "dude", typeof(ntuple2));

            var dude = q2.SelectMany(e => e.myvectorofdouble).Select(i => (int)1).Concat(q1.Select(e => (int)1)).AsTTree();

            foreach (var f in dude)
            {
                Console.WriteLine(f.FullName);
            }

            Assert.AreEqual(2, dude.Length);
            Assert.AreNotEqual(dude[0].Name, dude[1].Name);
        }

        [TestMethod]
        public void RunSimpleConcatTwoSourceAsFiles()
        {
            const int numberOfIter = 10;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter);
            var rootFile2 = TestUtils.CreateFileOfInt(numberOfIter*2);

            var q1 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFile1 }, "dude", typeof(ntuple));

            var q2 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFile2 }, "dude", typeof(ntuple));

            var dude = q1.Concat(q2).AsTTree();

            foreach (var f in dude)
            {
                Console.WriteLine(f.FullName);
            }

            Assert.AreEqual(2, dude.Length);
            Assert.AreNotEqual(dude[0].Name, dude[1].Name);
        }

        [TestMethod]
        public void RunSimpleConcatTwoSourceAsCSVFile()
        {
            const int numberOfIter = 10;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter);
            var rootFile2 = TestUtils.CreateFileOfInt(numberOfIter * 2);

            var q1 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFile1 }, "dude", typeof(ntuple));

            var q2 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFile2 }, "dude", typeof(ntuple));

            var dude = q1.Concat(q2).AsCSV(new FileInfo("RunSimpleConcatTwoSourceAsCSVFile.csv"));

            foreach (var f in dude)
            {
                Console.WriteLine(f.FullName);
            }

            Assert.AreEqual(2, dude.Length);
            Assert.AreNotEqual(dude[0].Name, dude[1].Name);
        }

        [TestMethod]
        public void RunSimpleConcatTwoSourceAsCSVFileNoItems()
        {
            const int numberOfIter = 10;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter);
            var rootFile2 = TestUtils.CreateFileOfInt(numberOfIter * 2);

            var q1 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFile1 }, "dude", typeof(ntuple));

            var q2 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFile2 }, "dude", typeof(ntuple));

            var dude = q1.Concat(q2).Where(q => q.run > 10000).AsCSV(new FileInfo("RunSimpleConcatTwoSourceAsCSVFile.csv"));

            foreach (var f in dude)
            {
                Console.WriteLine(f.FullName);
            }

            Assert.AreEqual(2, dude.Length);
            Assert.AreNotEqual(dude[0].Name, dude[1].Name);
        }

        [TestMethod]
        [DeploymentItem("testmachine.txt")]
        public void RunRemoteAsCSV()
        {
            const int numberOfIter = 10;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter);
            var info = File.ReadAllLines("testmachine.txt").First().Split('@');
            var remoteUri1 = new UriBuilder(rootFile1) { Scheme = "remotebash", Host = info[1], UserName = info[0] }.Uri;

            var q1 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { remoteUri1 }, "dude", typeof(ntuple));

            var dude = q1.Where(q => q.run > 10000).AsCSV(new FileInfo("RunSimpleConcatTwoSourceAsCSVFile.csv"));
            Assert.IsNotNull(dude);
            Assert.AreEqual(1, dude.Length);
            Assert.IsTrue(dude[0].Exists);
        }

        [TestMethod]
        [DeploymentItem("testmachine.txt")]
        public void RunRemote2FilesAsCSV1Connection()
        {
            const int numberOfIter1 = 10;
            const int numberOfIter2 = 20;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter1);
            var rootFile2 = TestUtils.CreateFileOfInt(numberOfIter2);

            var info = File.ReadAllLines("testmachine.txt").First().Split('@');
            var remoteUri1 = new UriBuilder(rootFile1) { Scheme = "remotebash", Host = info[1], UserName = info[0], Query="connections=1" }.Uri;
            var remoteUri2 = new UriBuilder(rootFile2) { Scheme = "remotebash", Host = info[1], UserName = info[0], Query = "connections=1" }.Uri;

            var q1 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { remoteUri1, remoteUri2 }, "dude", typeof(ntuple));

            var dude = q1.Where(q => q.run > 10000).AsCSV(new FileInfo("RunSimpleConcatTwoSourceAsCSVFile.csv"));
            Assert.IsNotNull(dude);
            Assert.AreEqual(1, dude.Length);
            Assert.IsTrue(dude[0].Exists);
        }

        [TestMethod]
        [DeploymentItem("testmachine.txt")]
        public void RunRemote2FilesAsCSV10Connection()
        {
            const int numberOfIter1 = 10;
            const int numberOfIter2 = 20;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter1);
            var rootFile2 = TestUtils.CreateFileOfInt(numberOfIter2);

            var info = File.ReadAllLines("testmachine.txt").First().Split('@');
            var remoteUri1 = new UriBuilder(rootFile1) { Scheme = "remotebash", Host = info[1], UserName = info[0], Query = "connections=10" }.Uri;
            var remoteUri2 = new UriBuilder(rootFile2) { Scheme = "remotebash", Host = info[1], UserName = info[0], Query = "connections=10" }.Uri;

            var q1 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { remoteUri1, remoteUri2 }, "dude", typeof(ntuple));

            var dude = q1.Where(q => q.run > 10000).AsCSV(new FileInfo("RunSimpleConcatTwoSourceAsCSVFile.csv"));
            Assert.IsNotNull(dude);
            Assert.AreEqual(2, dude.Length);
            Assert.IsTrue(dude[0].Exists);
            Assert.IsTrue(dude[1].Exists);
        }

        [TestMethod]
        [DeploymentItem("testmachine.txt")]
        [DeploymentItem("testmachine2.txt")]
        public void RunRemote2FilesAsCSV10Connection2Machines()
        {
            const int numberOfIter1 = 10;
            const int numberOfIter2 = 20;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter1);
            var rootFile2 = TestUtils.CreateFileOfInt(numberOfIter2);

            var info1 = File.ReadAllLines("testmachine.txt").First().Split('@');
            var remoteUri1 = new UriBuilder(rootFile1) { Scheme = "remotebash", Host = info1[1], UserName = info1[0], Query = "connections=10" }.Uri;
            var info2 = File.ReadAllLines("testmachine2.txt").First().Split('@');
            var remoteUri2 = new UriBuilder(rootFile2) { Scheme = "remotebash", Host = info2[1], UserName = info2[0], Query = "connections=10" }.Uri;

            var q1 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { remoteUri1, remoteUri2 }, "dude", typeof(ntuple));

            var dude = q1.Where(q => q.run > 10000).AsCSV(new FileInfo("RunSimpleConcatTwoSourceAsCSVFile.csv"));
            Assert.IsNotNull(dude);
            Assert.AreEqual(2, dude.Length);
            Assert.IsTrue(dude[0].Exists);
            Assert.IsTrue(dude[1].Exists);
        }

#if false
        This isn't working b.c. the SelectMany contains the Concat, and we don't have code yet that lifts that out.

        [TestMethod]
        public void RunNestedConcat()
        {
            const int numberOfIter = 10;
            var rootFile1 = TestUtils.CreateFileOfInt(numberOfIter);
            var proxyFile1 = TestUtils.GenerateROOTProxy(rootFile1, "dude");
            ntuple._gProxyFile = proxyFile1.FullName;
            var q1 = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFile1 }, "dude", typeof(ntuple));

            var rootFile2 = TestUtils.CreateFileOfVectorDouble(numberOfIter);
            var proxyFile2 = TestUtils.GenerateROOTProxy(rootFile2, "dude");
            ntuple2._gProxyFile = proxyFile2.FullName;
            var q2 = new SimpleTTreeExecutorQueriable<TestNtupeArrD>(new[] { rootFile2 }, "dude", typeof(ntuple2));

            var dude = q2.SelectMany(e => e.myvectorofdouble).Select(i => (int)1).Concat(q1.Select(eb => (int)1)).Count();

            Assert.AreEqual(numberOfIter * 10 + numberOfIter, dude);
        }
#endif

        [TestMethod]
        public void ConcatByDifferentUris()
        {
            // We use Uri's from two places. The contact is done
            // automatically as a result.

            const int numberOfIter = 10;
            var rootFileLocal = TestUtils.CreateFileOfInt(numberOfIter);
            var rootFileBashLocal = new UriBuilder(rootFileLocal) { Scheme = "localbash" }.Uri;

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFileLocal, rootFileBashLocal }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter*2, result);
        }

        [TestMethod]
        public void ConcatAsTTreeByDifferentUris()
        {
            // We use Uri's from two places. Produce something we can't easily combine, like
            // FileInfo structs.
            const int numberOfIter = 10;
            var rootFileLocal = TestUtils.CreateFileOfInt(numberOfIter);
            var rootFileBashLocal = new UriBuilder(rootFileLocal) { Scheme = "localbash" }.Uri;

            // Do a simple query on the TTree.
            var q = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFileLocal, rootFileBashLocal }, "dude", typeof(ntuple));
            var dude = q.AsTTree("recoTree", outputROOTFile: new FileInfo("ConcatAsTTreeByDifferentUris.root"));

            // Make sure we got some out for each one.
            Assert.IsNotNull(dude);
            Assert.AreEqual(2, dude.Length);
            Assert.AreNotEqual(dude[0].FullName, dude[1].FullName);
        }

        [TestMethod]
        public void ConcatAsTTreeByDifferentUrisCache()
        {
            // We use Uri's from two places. The contact is done
            // automatically as a result.
            const int numberOfIter = 10;
            var rootFileLocal = TestUtils.CreateFileOfInt(numberOfIter);
            var rootFileBashLocal = new UriBuilder(rootFileLocal) { Scheme = "localbash" }.Uri;

            // Create TTree files.
            var q = new SimpleTTreeExecutorQueriable<TestNtupe>(new[] { rootFileLocal, rootFileBashLocal }, "dude", typeof(ntuple));
            var dude1 = q.AsTTree("recoTree", outputROOTFile: new FileInfo("ConcatAsTTreeByDifferentUris.root"));
            var len = dude1.Length;

            // Now, re-do the same query, and pull from the cache (hopefully). We should get back
            // exactly the same thing as no combining or other manitpulation is done my the underlying framework.
            var dude2 = q.AsTTree("recoTree", outputROOTFile: new FileInfo("ConcatAsTTreeByDifferentUris.root"));
            Assert.IsNotNull(dude2);
            Assert.AreEqual(len, dude2.Length);
            Assert.AreNotEqual(dude2[0].FullName, dude2[1].FullName);

            // Check the caching.
            var t = ((DefaultQueryProvider)q.Provider).Executor as TTreeQueryExecutor;
            Assert.AreEqual(1, t.CountCacheHits);
        }

        [TestMethod]
        public async Task StressMultipleQueriesOnFile1()
        {
            await StressMultipleRuns(1, "file");
        }

        [TestMethod]
        public async Task StressMultipleQueriesOnLocalBash1()
        {
            await StressMultipleRuns(1, "localbash");
        }

        [TestMethod]
        public async Task StressMultipleQueriesOnLocalBash10()
        {
            await StressMultipleRuns(10, "localbash");
        }

        [TestMethod]
        [DeploymentItem("testmachine.txt")]
        public async Task StressMultipleQueriesOnRemoteBash1()
        {
            await StressMultipleRuns(1, "remotebash");
            Console.WriteLine($"Number of tunnels: {RemoteBashExecutor.NumberOfRecoveringConnections}");
            Console.WriteLine($"Number of connections: {RemoteBashExecutor.NumberOfSSHTunnels}");
            Console.WriteLine($"Number of disposes: {RemoteBashExecutor.NumberOfSSHRecoverDisposes}");
        }

        [TestMethod]
        [DeploymentItem("testmachine.txt")]
        public async Task StressMultipleQueriesOnRemoteBash10()
        {
            await StressMultipleRuns(10, "remotebash");
        }

        [TestMethod]
        [DeploymentItem("testmachine.txt")]
        [DeploymentItem("testmachine2.txt")]
        public async Task StressMultipleQueriesOnRemoteBash20On2Machines()
        {
            await StressMultipleRuns(20, "remotebash", machine_names: new[] { "testmachine.txt", "testmachine2.txt" });
            Console.WriteLine($"Number of tunnels: {RemoteBashExecutor.NumberOfRecoveringConnections}");
            Console.WriteLine($"Number of connections: {RemoteBashExecutor.NumberOfSSHTunnels}");
            Console.WriteLine($"Number of disposes: {RemoteBashExecutor.NumberOfSSHRecoverDisposes}");
        }

        [TestMethod]
        [DeploymentItem("testmachine.txt")]
        public async Task StressMultipleQueriesOnRemoteBash100With50Connections()
        {
            await StressMultipleRuns(100, "remotebash", connections: 50);
            Console.WriteLine($"Number of tunnels: {RemoteBashExecutor.NumberOfRecoveringConnections}");
            Console.WriteLine($"Number of connections: {RemoteBashExecutor.NumberOfSSHTunnels}");
            Console.WriteLine($"Number of disposes: {RemoteBashExecutor.NumberOfSSHRecoverDisposes}");
        }

        [TestMethod]
        [Ignore] // THis works fine, but takes too long during normal runs.
        [DeploymentItem("testmachine.txt")]
        public async Task StressMultipleQueriesOnRemoteBash100()
        {
            await StressMultipleRuns(100, "remotebash");
        }

        private async Task StressMultipleRuns(int numberOfRuns, string scheme, string[] machine_names = null, int connections = 1)
        {
            var returns = Enumerable.Range(10, numberOfRuns)
                .Select(cnt => GetCount(cnt, scheme, machine_names, connections))
                .ToArray();

            var results = await Task.WhenAll(returns);

            var tocompare = results.Zip(Enumerable.Range(10, numberOfRuns), (r, correct) => (res: r, shouldbe: correct));
            foreach (var c in tocompare)
            {
                Assert.AreEqual(c.shouldbe, c.res);
            }
        }

        private async Task<int> GetCount(int countNumber, string scheme, string [] machine_names = null, int connections = 1)
        {
            // The posibility of reading more than one file to run on more than one machine at a time.
            var mnameList = machine_names ?? new[] { "testmachine.txt" };
            var mnameFile = mnameList[countNumber % mnameList.Length];

            var rootFile = TestUtils.CreateFileOfInt(countNumber);
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            var info = scheme == "remotebash" ? File.ReadAllLines(mnameFile).First().Split('@') : null;

            var u = scheme != "remotebash"
                ? new UriBuilder(rootFile) { Scheme = scheme }.Uri
                : new UriBuilder(rootFile) { Scheme = scheme, UserName = info[0], Host = info[1], Query=$"connections={connections}" }.Uri;

            var exe = new TTreeQueryExecutor(new Uri[] { u }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.Verbose = true;

            var result = exe.ExecuteScalarAsFuture<int>(query);
            return await result;
        }

        [TestMethod]
        public void TestSimpleResultDebugCompile()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CompileDebug = true;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestCreateTupleWithNew()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = from evt in q
                       select new Tuple<int, int>(evt.run, evt.run);
            var r = dude.Where(i => i.Item1 > 0).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }


        [TestMethod]
        public void WhereProtectedConditionalWithComplexStatement()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            // A query that uses a conditional or.
            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                where v >= 0 && v < 100
                                orderby v ascending
                                select v).Take(2).Sum();

            // The first two elements are 0 and 1, so 0 + 1 == 1.
            var dude = dudeQ.Where(x => x == 1).Count();

            var query = DummyQueryExectuor.LastQueryModel;

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            exe.CleanupQuery = false;
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestCreateTupleWithCreate()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = from evt in q
                       select Tuple.Create(evt.run, evt.run);
            var r = dude.Where(i => i.Item1 > 0).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestInclusiveCPPInfo()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var listing = from evt in q
                          where CPPHelperFunctions.Calc(evt.run) > 10.0
                          select evt;
            var dude = listing.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void LocalIncludeFile()
        {
            // Write out the local include file. The system should pick it up from here.
            using (var writer = File.CreateText("bogus_function.h"))
            {
                writer.WriteLine("int bogus() { return 15; }");
                writer.WriteLine();
                writer.Close();
            }

            // Run on ints, though for this test it won't matter.
            var rootFile = TestUtils.CreateFileOfInt(10);

            // Run the special function.
            var q = new QueriableDummy<TestNtupe>();
            var listing = from evt in q
                          where CPPHelperFunctions.ReturnCustomFuncValue() > 10.0
                          select evt;
            var dude = listing.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Run the execution environment.
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            exe.CleanupQuery = false;
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestCPPCodeAndStringPassing()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var listing = from evt in q
                          where CPPHelperFunctions.CalcLen("hi there dude my butt") > 10.0
                          select evt;
            var dude = listing.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestNewOfObject()
        {
            /// Make sure the "new" gets translated to C++ correctly and there are no errors!

            var rootFile = TestUtils.CreateFileOfInt(5);

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var letResult = from evt in q
                            let temp = new ROOTNET.NTLorentzVector(evt.run, evt.run, evt.run, evt.run)
                            where temp.Pt() > 0.0
                            select temp;
            var cnt = letResult.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            var result = exe.ExecuteScalar<int>(query);

            Assert.AreEqual(1, exe.CountExecutionRuns, "exe after exe run");
            Assert.AreEqual(5, result, "count incorrect");
        }

        [TestMethod]
        public void TestCachingOfSimpleHisto()
        {
            // Do two identical queries. Make sure only one causes an actual run!
            var rootFile = TestUtils.CreateFileOfInt(5);

            // Ok, now we can actually see if we can make it "go".
            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            Assert.AreEqual(0, exe.CountExecutionRuns, "exe runs initialization");
            Assert.AreEqual(0, exe.CountCacheHits, "cache hits initialization");

            // Get a simple query we can "play" with
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Plot("hi", "there", 10, 0.0, 20.0, d => d.run);
            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            var result = exe.ExecuteScalar<ROOTNET.Interface.NTH1>(query);

            Assert.AreEqual(1, exe.CountExecutionRuns, "exe after exe run");
            Assert.AreEqual(0, exe.CountCacheHits, "cache after exe run");

            // Re-run the idential query. We have to remake the query b/c the histogram
            // stored internally will have now counts in it!! Ops!! :-)
            var dude2 = q.Plot("hi", "there", 10, 0.0, 20.0, d => d.run);
            var query2 = DummyQueryExectuor.LastQueryModel;

            var result2 = exe.ExecuteScalar<ROOTNET.Interface.NTH1>(query2);

            Assert.AreEqual(1, exe.CountExecutionRuns, "exe after exe and cache run");
            Assert.AreEqual(1, exe.CountCacheHits, "cache after exe and cache run");
        }

        [TestMethod]
        public void TestCachingOfSimpleHistoWithNameTitleChange()
        {
            /// Do two identical queries. Make sure only one causes an actual run!

            var rootFile = TestUtils.CreateFileOfInt(5);

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            Assert.AreEqual(0, exe.CountExecutionRuns, "exe runs initialization");
            Assert.AreEqual(0, exe.CountCacheHits, "cache hits initialization");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Plot("hi", "there", 10, 0.0, 20.0, d => d.run);
            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            var result = exe.ExecuteScalar<ROOTNET.Interface.NTH1>(query);

            Assert.AreEqual(1, exe.CountExecutionRuns, "after exe run");
            Assert.AreEqual(0, exe.CountCacheHits, "after exe run");

            ///
            /// Re-run a slightly different query. Should still be cached.
            /// 

            var dude2 = q.Plot("there", "high", 10, 0.0, 20.0, d => d.run);
            var query2 = DummyQueryExectuor.LastQueryModel;

            var result2 = exe.ExecuteScalar<ROOTNET.Interface.NTH1>(query2);

            Assert.AreEqual(1, exe.CountExecutionRuns, "after exe and cache run");
            Assert.AreEqual(1, exe.CountCacheHits, "after exe and cache run");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestForZeroInputFiles()
        {
            var rootFile = TestUtils.CreateFileOfInt(5);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { }, "dude", typeof(ntuple));
            int result = exe.ExecuteScalar<int>(query);
        }

        [TestMethod]
        public void TestTempDirectoryLocationAndEmptying()
        {
            var rootFile = TestUtils.CreateFileOfInt(1);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);

            var dir = new DirectoryInfo(Path.GetTempPath() + "\\LINQToROOT"); ;
            dir.Refresh();
            Assert.IsTrue(dir.Exists, "Temp directory doesn't exist");
            Assert.AreEqual(0, dir.EnumerateFiles().Count(), "Expected no spare files in there!");
            Assert.AreEqual(2, dir.EnumerateDirectories().Count(), "Incorrect # of subdirectories");
            Assert.AreEqual("CommonFiles", dir.GetDirectories()[0].Name, "incorrect name of single existing directory");
        }

        /// <summary>
        /// Prepare/create a common directory that we can use that has no spaces in the name!
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        private DirectoryInfo PrepNonSpaceDir(string dirName)
        {
            string currentDir = System.Environment.CurrentDirectory;
            while (currentDir.Contains(' '))
            {
                currentDir = Path.GetDirectoryName(currentDir);
            }

            ///
            /// If this went all the way back to their user name - that blows. Just bomb and
            /// we will address this when it is time.
            /// 

            if (currentDir.Length < 10)
                Assert.Fail("Something is wrong with the directory - please fix the test harness!");

            var d = new DirectoryInfo(currentDir + "\\" + dirName);
            if (d.Exists)
            {
                d.Delete(true);
            }
            d.Create();
            d.Refresh();

            return d;
        }

        [TestMethod]
        public void TestDualQueries()
        {
            RunSimpleCountResult(10);
            RunSimpleCountResult(20);
        }

        [TestMethod]
        public void TestDualQueryOnSameQueriable()
        {
            var rootFile = TestUtils.CreateFileOfInt(5);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);

            ///
            /// Run the second one now
            /// 

            var dude1 = q.Where(e => e.run > 10).Count();
            query = DummyQueryExectuor.LastQueryModel;
            result = exe.ExecuteScalar<int>(query);
        }

        [TestMethod]
        public void TestLoadingCommonFiles()
        {
            /// Create a common C++ object that can be loaded and checked for.
            var d = PrepNonSpaceDir("TestLoadingCommonFiles");
            string fnamebase = "TestLoadingCommonFilesObj";
            var f = CreateCommonObject(fnamebase, d);

            ///
            /// Run a simple query - but fool it against another ntuple
            /// 

            var rootFile = TestUtils.CreateFileOfInt(1);
            var q = new QueriableDummy<TestNtupe>();
            var dude = q.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// First, make sure to clear out the common area. We are relying on the fact that we know
            /// where the common area is for this step.
            /// 

            var commonArea = new DirectoryInfo(Path.GetTempPath() + @"\LINQToROOT\CommonFiles");
            if (commonArea.Exists)
            {
                var filesToKill = (from fd in commonArea.EnumerateFiles()
                                   where fd.Name.Contains(fnamebase)
                                   select fd).ToArray();
                foreach (var theFile in filesToKill)
                {
                    theFile.Delete();
                }
            }

            ///
            /// Setup all the files we will have to have!
            /// 

            ///
            /// Generate a proxy .h file that we can use
            /// 

            ntuple._gObjectFiles = new string[] { f.FullName };

            ///
            /// Now run
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            int result = exe.ExecuteScalar<int>(query);

            Assert.AreEqual(1, result, "The result should have run correctly.");

            ///
            /// Next, see if we can find the files in the common area.
            /// 

            commonArea.Refresh();
            Assert.IsTrue(commonArea.Exists, string.Format("The common build area doesn't exist currently ({0}).", commonArea.FullName));
            var filesFromOurObj = (from fd in commonArea.EnumerateFiles()
                                   where fd.Name.Contains(fnamebase)
                                   select fd).ToArray();
            Assert.IsTrue(filesFromOurObj.Length > 0, "no files from our common object");
        }

        // For ntuples generated with CreateFileOfIndexedInt
        public class TestSingleIndexArray : IExpressionHolder
        {
            public TestSingleIndexArray(Expression holder)
            {
                HeldExpression = holder;
            }
            public System.Linq.Expressions.Expression HeldExpression { get; set; }

#pragma warning disable 0169
            [ArraySizeIndex("n")]
            public int[] arr;
            public int n;
#pragma warning restore 0169

        }

        [TestMethod]
        public void TestIndexArray()
        {
            // Make sure we can process an index array (an array that is specified as arr[n]).

            // Create the ntuple file and the proxy that we will be using.
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfIndexedInt(numberOfIter);

            // Do a simple query to make sure the # of items in each array is "10", and that
            // there are 25 such events.

            var q = new QueriableDummy<TestSingleIndexArray>();
            var dudeQ = from evt in q
                        where (evt.arr.Count() == 10)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestSingleIndexArray));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }


        [TestMethod]
        public void TestFirstCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.First() > 0)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestFirstCodeCombine()
        {
            // Run a First(), but do it twice. The reason is to make sure that
            // the code doesn't step on itself with a break statement.

            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ1 = from evt in q
                         where (evt.myvectorofint.First() > 0)
                         select evt;
            var dude1 = dudeQ1.Count();
            var query1 = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            var dudeQ2 = from evt in q
                         where (evt.myvectorofint.Skip(1).First() > 0)
                         select evt;
            var dude2 = dudeQ2.Count();
            var query2 = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result2 = exe.ExecuteScalarAsFuture<int>(query2);
            var result1 = exe.ExecuteScalarAsFuture<int>(query1);
            Assert.AreEqual(0, result1.Value, "result 1");
            Assert.AreEqual(numberOfIter, result2.Value, "result 2");
        }

        [TestMethod]
        public void TestFirstCodeTranslated()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArrEvents>();
            var dudeQ = from evt in q
                        where (evt.jets.First().myvectorofint > 0)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArrEvents));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void TopLevelTest()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArrEvents>();
            var dudeQ = from evt in q
                        select evt;
            var dude = dudeQ.Take(10).Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArrEvents));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestFirstCodeDefaultTranslated()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArrEvents>();
            var dudeQ = from evt in q
                        where (evt.jets.Take(1).Skip(2).FirstOrDefault() == null)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArrEvents));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestFirstOrDefaultCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Take(2).Skip(3).FirstOrDefault() == 0)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(25, result, "Incorrect number of iterations found");
        }

        [TestMethod]
        [ExpectedException(typeof(System.Runtime.InteropServices.SEHException))]
        public void TestFirstButNothing()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Skip(50).First() == 0)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            try
            {
                var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
                var result = exe.ExecuteScalar<int>(query);
            } catch (AggregateException exp)
            {
                throw exp.UnrollAggregateExceptions();
            }
        }

        [TestMethod]
        public void TestLastCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Last() > 0)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestMaxCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Max() > 5)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestMinCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Min() > 5)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void TestAllCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.All(r => r > 5))
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestAnyCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Any(r => r > 0))
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            exe.CleanupQuery = false;
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestSumCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        let r = evt.myvectorofint.Sum()
                        where r == 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1
                        select r;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);

        }

        [TestMethod]
        public void TestAverageCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        let r = evt.myvectorofint.Average()
                        where r == ((double)(9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1) / 10)
                        select r;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);

        }

        [TestMethod]
        public void TestAggregateCodeForSimpleVariableType()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        let r = evt.myvectorofint.Aggregate(0, (s, v) => s + v)
                        where r == 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1
                        select r;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);

        }

        [TestMethod]
        public void TestAggregateCodeForSimpleDoubleVariableType()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorDouble(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArrD>();
            var evtvalue = 9.5 + 8.5 + 7.5 + 6.5 + 5.5 + 4.5 + 3.5 + 2.5 + 1.5 + 0.5;
            var dudeQ = from evt in q
                        let r = evt.myvectorofdouble.Aggregate(0.0, (s, v) => s + v)
                        where r == 9.5 + 8.5 + 7.5 + 6.5 + 5.5 + 4.5 + 3.5 + 2.5 + 1.5 + 0.5
                        select r;
            var dude = dudeQ.Aggregate(0.0, (acc, val) => acc + val);

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArrD));
            var result = exe.ExecuteScalar<double>(query);
            Assert.AreEqual(result, numberOfIter * evtvalue);

        }

        [TestMethod]
        public void TestNestedCount()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.Count() > 5)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestUniqueCombinations()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// We are looking at an array that has 10 entries in it. So if we create a
            /// unique combo, then we will have 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1 items,
            /// or 45 items.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (evt.myvectorofint.UniqueCombinations().Count() == 45)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestUniqueCombinationsWithBreak()
        {
            //
            // The way the unique combo works is it builds in a double loop. So if something inside that loop
            // throws a break it has to be transmitted out two levels. This is meant to test for that.
            //

            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, 9);

            ///
            /// We are looking at an array that has 10 entries in it. So if we create a
            /// unique combo, then we will have 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1 items,
            /// or 45 items.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (from cmb in evt.myvectorofint.UniqueCombinations() where (cmb.Item1 + cmb.Item2 < 4) select cmb).Any()
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestPairWiseAll()
        {
            //
            // The way the unique combo works is it builds in a double loop. So if something inside that loop
            // throws a break it has to be transmitted out two levels. This is meant to test for that.
            //

            const int numberOfIter = 25;
            const int vecSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vecSize);

            ///
            /// We are looking at an array that has 10 entries in it. So if we create a
            /// unique combo, then we will have 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1 items,
            /// or 45 items.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (from cmb in evt.myvectorofint.PairWiseAll((i1, i2) => i1 != i2) select cmb).Count() == vecSize
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestPairwiseAllWithExternalRef()
        {
            // Definatly some problems with the code generated here:
            // TODO:
            //aBoolean_18Array[index1] = false;
            //aBoolean_18Array[index2] = false;
            // So this must be addressed.

            //
            // Look for a bug that happens when we have an external guy that references
            // something in the inside loop.
            //

            const int numberOfIter = 25;
            const int vecSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vecSize);

            ///
            /// We are looking at an array that has 10 entries in it. So if we create a
            /// unique combo, then we will have 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1 items,
            /// or 45 items.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        where (from cmb in evt.myvectorofint.PairWiseAll((i1, i2) => CPPHelperFunctions.Calc(i1) != CPPHelperFunctions.Calc(i2)) select cmb).Count() == vecSize
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            Console.WriteLine("Unoptimized");
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestSortAscending()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                orderby v ascending
                                select v).Take(2).Sum();

            // The first two elements are 0 and 1, so 0 + 1 == 1.
            var dude = dudeQ.Where(x => x == 1).Count();

            var query = DummyQueryExectuor.LastQueryModel;

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestSortAscendingTranslated()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArrEvents>();

            var dudeQ = from evt in q
                        select (from v in evt.jets
                                orderby v.myvectorofint ascending
                                select v).Take(2).Sum(j => j.myvectorofint);

            // The first two elements are 0 and 1, so 0 + 1 == 1.
            var dude = dudeQ.Sum();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArrEvents));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        /// <summary>
        /// First sometimes uses a break in the code - make sure that we deal with it correctly.
        /// </summary>
        [TestMethod]
        public void TestSortAscendingWithFirst()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                orderby v ascending
                                select v).First();

            // The first element is 0.
            var dude = dudeQ.Where(x => x == 0).Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestSortDescending()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                orderby v descending
                                select v).Take(2).Sum();

            // The last two elements are 9 and 8, so 9 + 8 = 17.
            var dude = dudeQ.Where(x => x == 17).Count();

            var query = DummyQueryExectuor.LastQueryModel;

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestGroupByAccessKey()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         from grp in evt
                         where grp.Key == 2
                         select grp.Key;

            var dudq = dudeQ1.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(result, numberOfIter);
        }

        [TestMethod]
        public void TestGroupByCountItems()
        {
            const int numberOfIter = 25;
            const int vectorSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vectorSize);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         from grp in evt
                         where grp.Count() == 1
                         select grp.Key;

            var dudq = dudeQ1.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter * vectorSize, result);
        }

        [TestMethod]
        public void TestGroupBySortItems()
        {
            const int numberOfIter = 25;
            const int vectorSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vectorSize);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         from grp in evt
                         where grp.OrderBy(v => v).First() == 0
                         select grp.Key;

            var dudq = dudeQ1.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestGroupByAccessItems1()
        {
            const int numberOfIter = 25;
            const int vectorSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vectorSize);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         from grp in evt
                         where grp.Where(v => v == 2).Any()
                         select grp.Key;

            var dudq = dudeQ1.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestGroupByAccessItems2()
        {
            const int numberOfIter = 25;
            const int vectorSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vectorSize);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         from grp in evt
                         where grp.Where(v => v == 22).Any()
                         select grp.Key;

            var dudq = dudeQ1.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestGroupByAndSortKey()
        {
            const int numberOfIter = 25;
            const int vectorSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vectorSize);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        select (from v in evt.myvectorofint
                                group v by v);

            var dudeQ1 = from evt in dudeQ
                         select (from grp in evt
                                 orderby grp.Key descending
                                 select grp).First();

            var dudeQ2 = from evt in dudeQ1
                         where evt.Key == 9 && evt.Count() == 1
                         select evt;

            var dudq = dudeQ2.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestGroupByWithInto()
        {
            const int numberOfIter = 25;
            const int vectorSize = 10;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter, vectorSize);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        from v in evt.myvectorofint
                        group v by v into lists
                        from i in lists
                        where i == 5
                        select i;

            var r = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            //
            // Ok, now we can actually see if we can make it "go".
            // 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestInitalizerWithROOTVariable()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var holder = new ROOTNET.NTH1F("hi", "title", 2, 0.0, 2.0);
            holder.Directory = null;
            var dude = q.ApplyToObject(holder, (h, n) => h.Fill(n.run));
            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            var result = exe.ExecuteScalar<ROOTNET.Interface.NTH1F>(query);
            Assert.AreEqual(result.Entries, numberOfIter);
            Assert.AreEqual("hi", result.Name, "histogram name");
        }

        [TestMethod]
        public void TestInlineIfIntCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        let j = evt.myvectorofint[0] == 0 ? 1 : 0
                        where (j == 1)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestComplexSimpleTypeInlineIfIntCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        let j = evt.myvectorofint[0] == 0 ? evt.myvectorofint.First() : evt.myvectorofint.Skip(1).First()
                        where (j != 1)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(numberOfIter, result);
        }

        [TestMethod]
        public void TestInlineIfObjectCode()
        {
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOfVectorInt(numberOfIter);

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeArr>();
            var dudeQ = from evt in q
                        let k = evt.myvectorofint[0]
                        let j = k == 0 ? new ROOTNET.NTVector3(k, k, k) : null
                        where (j == null)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestTransferSingleObject()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var mainHist = new ROOTNET.NTH1F("hi", "there", 1000, 0.0, 1000.0);
            mainHist.Directory = null;

            var dude = from evt in q
                       where mainHist.GetBinContent(evt.run) > 0.0
                       select evt;
            var final = dude.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result, "Didn't add correctly");
            Assert.AreEqual("hi", mainHist.Name, "histogram name changed");
        }

        [TestMethod]
        public void TestSameHistoOverTice()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var mainHist = new ROOTNET.NTH1F("hi", "there", 1000, 0.0, 1000.0);
            mainHist.Directory = null;

            var dude = from evt in q
                       where mainHist.GetBinContent(evt.run) * mainHist.GetBinContent(evt.run) > 0.0
                       select evt;
            var final = dude.Count();
            var query = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(0, result, "Didn't add correctly");
            Assert.AreEqual("hi", mainHist.Name, "histogram name changed");

        }

        [TestMethod]
        public void TestSameHistInCombinedQueries()
        {
            var rootFile = TestUtils.CreateFileOfInt(10);

            ///
            /// Get two querries we can play with
            /// 

            var q = new QueriableDummy<TestNtupe>();
            var mainHist = new ROOTNET.NTH1F("hi", "there", 1000, 0.0, 1000.0);
            mainHist.Directory = null;

            var dude1 = from evt in q
                        where mainHist.GetBinContent(evt.run) > 0.0
                        select evt;
            var final1 = dude1.Count();
            var query1 = DummyQueryExectuor.LastQueryModel;

            var dude2 = from evt in q
                        where mainHist.GetBinContent(evt.run) > 0.0
                        select evt;
            var final2 = dude2.Count();
            var query2 = DummyQueryExectuor.LastQueryModel;

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupe));

            var r1f = exe.ExecuteScalarAsFuture<int>(query1);
            var r2f = exe.ExecuteScalarAsFuture<int>(query2);

            var r1 = r1f.Value;
            var r2 = r2f.Value;

            Assert.AreEqual(0, r1, "r1 Didn't add correctly");
            Assert.AreEqual(0, r2, "r2 Didn't add correctly");
            Assert.AreEqual("hi", mainHist.Name, "histogram name changed");
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupeCConstArr : IExpressionHolder
        {
            public TestNtupeCConstArr(Expression holder)
            {
                HeldExpression = holder;
            }
#pragma warning disable 0169
            [ArraySizeIndex("5", Index = 0, IsConstantExpression = true)]
            public int[] arr;
#pragma warning restore 0169

            public System.Linq.Expressions.Expression HeldExpression { get; set; }
        }

        [TestMethod]
        public void TestCArrayConst()
        {
            // Test arr[5].
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOf("TestCArrayConst.root", () => TTreeParserCPPTests.CreateTrees.CreateTreeWithIndexedConstSimpleVector(numberOfIter));

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeCConstArr>();
            var dudeQ = from evt in q
                        where (evt.arr.Count() == 5)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeCConstArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(25, result, "Incorrect number of iterations found");
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupeC2DConstArr : IExpressionHolder
        {
            public TestNtupeC2DConstArr(Expression holder)
            {
                HeldExpression = holder;
            }
#pragma warning disable 0169
            [ArraySizeIndex("5", Index = 0, IsConstantExpression = true)]
            [ArraySizeIndex("5", Index = 1, IsConstantExpression = true)]
            public int[][] arr;
#pragma warning restore 0169

            public System.Linq.Expressions.Expression HeldExpression { get; set; }
        }

        [TestMethod]
        public void TestCArray2DConst()
        {
            // Test arr[5].
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOf("TestCArray2DConst.root", () => TTreeParserCPPTests.CreateTrees.CreateTreeWithIndexed2DConstSimpleVector(numberOfIter));

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeC2DConstArr>();
            var dudeQ = from evt in q
                        let c = (from c1 in evt.arr
                                 from c2 in c1
                                 select c2).Count()
                        where c == 25
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeC2DConstArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(25, result, "Incorrect number of iterations found");
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupeCIndexedArr : IExpressionHolder
        {
            public TestNtupeCIndexedArr(Expression holder)
            {
                HeldExpression = holder;
            }
#pragma warning disable 0169
            [ArraySizeIndex("n")]
            public int[] arr;
            public int n;
#pragma warning restore 0169

            public System.Linq.Expressions.Expression HeldExpression { get; set; }
        }

        [TestMethod]
        public void TestCArrayIndexed()
        {
            // Test arr[5].
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOf("TestCArrayIndexed.root", () => TTreeParserCPPTests.CreateTrees.CreateTreeWithIndexedSimpleVector(numberOfIter));

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeCIndexedArr>();
            var dudeQ = from evt in q
                        where (evt.arr.Count() == evt.n)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeCIndexedArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(25, result, "Incorrect number of iterations found");
        }

        [TestMethod]
        [Ignore]
        // This is a real bug, and has affected us - needs to be fixed, but not used very much.
        public void TestCArrayIndexedSum()
        {
            // Test arr[5].
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOf("TestCArrayIndexed.root", () => TTreeParserCPPTests.CreateTrees.CreateTreeWithIndexedSimpleVector(numberOfIter));

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeCIndexedArr>();
            var dudeQ = from evt in q
                        where (evt.arr.Sum() > 10)
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeCIndexedArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(25, result, "Incorrect number of iterations found");
        }

        [TestMethod]
        public void TestCArrayConstEnumerable()
        {
            // Test arr[5].
            const int numberOfIter = 25;
            var rootFile = TestUtils.CreateFileOf("TestCArrayConstEnumerable.root", () => TTreeParserCPPTests.CreateTrees.CreateTreeWithIndexedConstSimpleVector(numberOfIter));

            ///
            /// Get a simple query we can "play" with. That this works
            /// depends on each event having 10 entries in the array, which contains
            /// the numbers 0-10.
            /// 

            var q = new QueriableDummy<TestNtupeCConstArr>();
            var dudeQ = from evt in q
                        let tmp = (from index in Enumerable.Range(0, 2)
                                   select evt.arr[index]).Count()
                        where tmp == 2
                        select evt;
            var dude = dudeQ.Count();

            var query = DummyQueryExectuor.LastQueryModel;
            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            ///
            /// Ok, now we can actually see if we can make it "go".
            /// 

            var exe = new TTreeQueryExecutor(new Uri[] { rootFile }, "dude", typeof(ntuple), typeof(TestNtupeCConstArr));
            var result = exe.ExecuteScalar<int>(query);
            Assert.AreEqual(25, result, "Incorrect number of iterations found");
        }
    }
}
