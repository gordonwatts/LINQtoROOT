using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// Tests that deal with implied objects
    /// </summary>
    [TestClass]
    public class QueryVisitorOnImpliedObjectsTest
    {
        [TestInitialize]
        public void Setup()
        {
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;
            ArrayExpressionParser.ResetParser();
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        public class EventInfo_p3 : IExpressionHolder
        {
            public Expression HeldExpression { get; private set; }
            public EventInfo_p3(Expression expr) { HeldExpression = expr; }

#pragma warning disable 0649
            public uint[] m_AllTheData;
#pragma warning restore 0649
        }

        [TClonesArrayImpliedClass]
        public class GenEvent_p4 : IExpressionHolder
        {
            public Expression HeldExpression { get; private set; }
            public GenEvent_p4(Expression expr) { HeldExpression = expr; }

#pragma warning disable 0649
            [NotAPointer]
            public int[] m_signalProcessId;
            [NotAPointer]
            public int[] m_eventNbr;
            [NotAPointer]
            public double[] m_eventScale;
            [NotAPointer]
            public double[] m_alphaQCD;
            [NotAPointer]
            public double[] m_alphaQED;
            [NotAPointer]
            public int[] m_signalProcessVtx;
            [NotAPointer]
            public double[][] m_weights;
            [NotAPointer]
            public double[][] m_pdfinfo;
            [NotAPointer]
            public int[][] m_randomStates;
            [NotAPointer]
            public uint[] m_verticesBegin;
            [NotAPointer]
            public uint[] m_verticesEnd;
            [NotAPointer]
            public uint[] m_particlesBegin;
            [NotAPointer]
            public uint[] m_particlesEnd;
#pragma warning restore 0649
        }

        [TClonesArrayImpliedClass]
        public class GenVertex_p4 : IExpressionHolder
        {
            public Expression HeldExpression { get; private set; }
            public GenVertex_p4(Expression expr) { HeldExpression = expr; }

#pragma warning disable 0649
            [NotAPointer]
            public float[] m_x;
            [NotAPointer]
            public float[] m_y;
            [NotAPointer]
            public float[] m_z;
            [NotAPointer]
            public float[] m_t;
            [NotAPointer]
            public int[][] m_particlesIn;
            [NotAPointer]
            public int[][] m_particlesOut;
            [NotAPointer]
            public int[] m_id;
            [NotAPointer]
            public float[][] m_weights;
            [NotAPointer]
            public int[] m_barcode;
#pragma warning restore 0649
        }

        [TClonesArrayImpliedClass]
        public class GenParticle_p4 : IExpressionHolder
        {
            public Expression HeldExpression { get; private set; }
            public GenParticle_p4(Expression expr) { HeldExpression = expr; }

#pragma warning disable 0649
            [NotAPointer]
            public float[] m_px;
            [NotAPointer]
            public float[] m_py;
            [NotAPointer]
            public float[] m_pz;
            [NotAPointer]
            public float[] m_m;
            [NotAPointer]
            public int[] m_pdgId;
            [NotAPointer]
            public int[] m_status;
            [NotAPointer]
            public float[] m_thetaPolarization;
            [NotAPointer]
            public float[] m_phiPolarization;
            [NotAPointer]
            public int[] m_prodVtx;
            [NotAPointer]
            public int[] m_endVtx;
            [NotAPointer]
            public int[] m_barcode;
            [NotAPointer]
            public short[] m_recoMethod;
#pragma warning restore 0649
        }

        public class McEventCollection_p4 : IExpressionHolder
        {
            public Expression HeldExpression { get; private set; }
            public McEventCollection_p4(Expression expr) { HeldExpression = expr; }

#pragma warning disable 0649
            [NotAPointer]
            public GenEvent_p4 m_genEvents;
            [NotAPointer]
            public GenVertex_p4 m_genVertices;
            [NotAPointer]
            public GenParticle_p4 m_genParticles;
#pragma warning restore 0649
        }

        public class CollectionTree : IExpressionHolder
        {
            public Expression HeldExpression { get; private set; }
            public CollectionTree(Expression expr) { HeldExpression = expr; }

#pragma warning disable 0649
            [NotAPointer]
            public EventInfo_p3 EventInfo_p3_McEventInfo;
            [NotAPointer]
            public McEventCollection_p4 McEventCollection_p4_GEN_EVENT;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestQueryOnNonTClonesObject()
        {
            var q = new QueriableDummy<CollectionTree>();

            var r1 = from evt in q
                     from p in evt.EventInfo_p3_McEventInfo.m_AllTheData
                     select p;
            var r = r1.Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.AreEqual(1, query1.CodeBody.Statements.Count(), "# statements in the code body");
            var forblock = query1.CodeBody.Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(forblock, "top level statement");
            Assert.AreEqual(1, forblock.Statements.Count(), "# of statements in the code");
        }

        [TestMethod]
        public void TestQueryOnNonTClonesObjectCombine()
        {
            var q = new QueriableDummy<CollectionTree>();

            var r1 = from evt in q
                     from p in evt.EventInfo_p3_McEventInfo.m_AllTheData
                     select p;
            var r = r1.Count();
            var query1 = DummyQueryExectuor.FinalResult;
            var r2 = r1.Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);

            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query code blocks");
            Assert.AreEqual(1, query.QueryCode().First().Statements.Count(), "# statements in the code body");
            var forblock = query.QueryCode().First().Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(forblock, "top level statement");
            Assert.AreEqual(2, forblock.Statements.Count(), "# of statements in the code");
        }

        [TestMethod]
        public void TestQueryOnTClonesObject()
        {
            var q = new QueriableDummy<CollectionTree>();

            var r1 = from evt in q
                     from p in evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px
                     select p;
            var r = r1.Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.AreEqual(1, query1.CodeBody.Statements.Count(), "# statements in the code body");
            var forblock = query1.CodeBody.Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(forblock, "top level statement");
            Assert.AreEqual(1, forblock.Statements.Count(), "# of statements in the code");
        }

        [TestMethod]
        public void TestQueryOnTClonesObjectCombine()
        {
            var q = new QueriableDummy<CollectionTree>();

            var r1 = from evt in q
                     from p in evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px
                     select p;
            var r = r1.Count();
            var query1 = DummyQueryExectuor.FinalResult;
            r = r1.Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);

            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# of query blocks");
            var qu = query.QueryCode().First();
            Assert.AreEqual(1, qu.Statements.Count(), "# statements in the code body");
            var forblock = qu.Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(forblock, "top level statement");
            Assert.AreEqual(2, forblock.Statements.Count(), "# of statements in the code");
        }

        [TestMethod]
        public void TestQueryOnTClonesObjectWithEnumerble()
        {
            var q = new QueriableDummy<CollectionTree>();

            var r1 = from evt in q
                     from pindex in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px.Length)
                     select pindex;
            var r = r1.Count();

            var query1 = DummyQueryExectuor.FinalResult;
            query1.DumpCodeToConsole();

            Assert.AreEqual(1, query1.CodeBody.Statements.Count(), "# statements in the code body");
            var forblock2 = query1.CodeBody.Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(forblock2, "2nd for block");
            Assert.AreEqual(1, forblock2.Statements.Count(), "# of for #2 statement statements");
        }

        [TestMethod]
        public void TestQueryOnTClonesObjectWithEnumerbleCombine()
        {
            var q = new QueriableDummy<CollectionTree>();

            var r1 = from evt in q
                     from pindex in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px.Length)
                     select pindex;
            var r = r1.Count();
            var query1 = DummyQueryExectuor.FinalResult;
            r = r1.Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);

            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# fo code blocks");
            Assert.AreEqual(1, query.QueryCode().First().Statements.Count(), "# statements in the code body");
            var forblock2 = query.QueryCode().First().Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(forblock2, "2nd for block");
            Assert.AreEqual(2, forblock2.Statements.Count(), "# of for #2 statement statements");
        }

        [TestMethod]
        public void TestQueryOnTClonesObjectWithEnumerableAndLenghtCombine()
        {
            var q = new QueriableDummy<CollectionTree>();

            var r1 = from evt in q
                     from pindex in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px.Length)
                     select pindex;
            var r = r1.Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var r2 = from evt in q
                     from p in evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px
                     select p;
            r = r2.Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query1, query2);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# fo code blocks");
            Assert.AreEqual(1, query.QueryCode().First().Statements.Count(), "# statements in the code body");
            var forblock2 = query.QueryCode().First().Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(forblock2, "2nd for block");
            Assert.AreEqual(2, forblock2.Statements.Count(), "# of for #2 statement statements");
        }

        [TestMethod]
        public void TestQueryOnTClonesObjectWithEnumerableAndLenghtCombineR()
        {
            var q = new QueriableDummy<CollectionTree>();

            var r1 = from evt in q
                     from pindex in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px.Length)
                     select pindex;
            var r = r1.Count();
            var query1 = DummyQueryExectuor.FinalResult;

            var r2 = from evt in q
                     from p in evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px
                     select p;
            r = r2.Count();
            var query2 = DummyQueryExectuor.FinalResult;

            var query = CombineQueries(query2, query1);
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.QueryCode().Count(), "# fo code blocks");
            Assert.AreEqual(1, query.QueryCode().First().Statements.Count(), "# statements in the code body");
            var forblock2 = query.QueryCode().First().Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(forblock2, "2nd for block");
            Assert.AreEqual(2, forblock2.Statements.Count(), "# of for #2 statement statements");
        }

        [TestMethod]
        public void TestJoinOnTClonesObjectWithEnumerableExplicit()
        {
            var q = new QueriableDummy<CollectionTree>();

            var particles = from evt in q
                            select new
                            {
                                Particles = from pindex in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px.Length)
                                            let vtxInitBC = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_prodVtx[pindex]
                                            where vtxInitBC < 0
                                            let vtxInitIndex = (from vindex in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode.Length)
                                                                where evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode[vindex] == vtxInitBC
                                                                select vindex).FirstOrDefault()
                                            select new
                                            {
                                                Px = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px[pindex],
                                                Py = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_py[pindex],
                                                PInitX = evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_x[vtxInitIndex]
                                            }
                            };
            var r = particles.Where(plst => plst.Particles.Any(p => p.PInitX > 0)).Count();
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();
            // Make sure we don't throw here.
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestJoinOnTClonesObjectWithEnumerable()
        {
            var q = new QueriableDummy<CollectionTree>();

            var particles = from evt in q
                            select new
                            {
                                Particles = from pindex in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px.Length)
                                            join vindex in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_x.Length)
                                            on evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_prodVtx[pindex] equals evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode[vindex] into productionVertexList
                                            where productionVertexList.Count() == 1
                                            let vtxInitIndex = productionVertexList.First()
                                            select new
                                            {
                                                Px = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px[pindex],
                                                Py = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_py[pindex],
                                                PInitX = evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_x[vtxInitIndex]
                                            }
                            };
            var r = particles.Where(plst => plst.Particles.Any(p => p.PInitX > 0)).Count();
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();
        }

        [TestMethod]
        public void TestJoinOnTClonesObjectBoth()
        {
            var q = new QueriableDummy<CollectionTree>();

            var pvPairs = from evt in q
                          from pindex in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_prodVtx.Length)
                          select Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode.Length)
                              .Where(i => evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_prodVtx[pindex] == evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode[i])
                              .FirstOrDefault();

            var r = pvPairs.Where(i => i > 4).Count();
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.CodeBody.Statements.Count(), "# of statements");
            var scnd = query.CodeBody.Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(scnd, "Booking block fro 2nd statement");
            Assert.AreEqual(3, scnd.Statements.Count(), "# of statements in second for loop");
        }

        [TestMethod]
        public void TestJoinOnTClonesObjectWithFunctionCall()
        {
            var q = new QueriableDummy<CollectionTree>();

            Expression<Func<CollectionTree, int, int>> finder = (evt, particleIndex) =>
                Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode.Length)
                .Where(i => evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_prodVtx[particleIndex] == evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode[i])
                .FirstOrDefault();

            var pvPairs = from evt in q
                          from pindex in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_prodVtx.Length)
                          select finder.Invoke(evt, pindex);

            var r = pvPairs.Where(i => i > 4).Count();
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.CodeBody.Statements.Count(), "# of statements");
            var scnd = query.CodeBody.Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(scnd, "Booking block fro 2nd statement");
            Assert.AreEqual(3, scnd.Statements.Count(), "# of statements in second for loop");
        }

        [TestMethod]
        public void TestJoinOnTClonesObjectWithNestedFunctionCall()
        {
            var q = new QueriableDummy<CollectionTree>();

            Expression<Func<CollectionTree, int, int>> finder = (evt, particleIndex) =>
                Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode.Length)
                .Where(i => evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_prodVtx[particleIndex] == evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode[i])
                .FirstOrDefault();

            Expression<Func<CollectionTree, int, ROOTNET.NTVector3>> vertexPostion =
                (evt, i) => new ROOTNET.NTVector3(
                    evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_x[i],
                    evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_y[i],
                    evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_z[i]
                    );

            var pvPairs = from evt in q
                          from pindex in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_prodVtx.Length)
                          let idx = finder.Invoke(evt, pindex)
                          select vertexPostion.Invoke(evt, idx);

            var r = pvPairs.Where(i => i.Mag() > 4).Count();
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.CodeBody.Statements.Count(), "# of statements");
            var scnd = query.CodeBody.Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(scnd, "Booking block fro 2nd statement");
            Assert.AreEqual(5, scnd.Statements.Count(), "# of statements in second for loop");
        }

        public class ParticleInfo
        {
            public ROOTNET.Interface.NTLorentzVector TLZ { get; set; }
            public int PDGID { get; set; }
            public ROOTNET.Interface.NTVector3 vtxInit { get; set; }
            public ROOTNET.Interface.NTVector3 vtxTerm { get; set; }
            public bool vtxTermOK { get; set; }
            public bool vtxInitOK { get; set; }
        }

        public static Expression<Func<CollectionTree, int, int>> FindVertexFromBC = (evt, vtxBC) =>
            (from vtxIdx in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode.Length)
             where vtxBC == evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode[vtxIdx]
             select vtxIdx).First();

        /// <summary>
        /// Given a vertex index, return the 3D vector for the position.
        /// </summary>
        public static Expression<Func<CollectionTree, int, ROOTNET.Interface.NTVector3>> VertexVector = (evt, index) =>
            new ROOTNET.NTVector3(evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_x[index],
                evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_x[index],
                evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_x[index]);

        [TestMethod]
        public void TestRepeatedInvokationCrash()
        {
            // A crash that happened in one of my seperate programs...

            var q = new QueriableDummy<CollectionTree>();
            var particles = from evt in q
                            select (from i_p in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px.Length)
                                    let px = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px[i_p]
                                    let py = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_py[i_p]
                                    let pz = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_pz[i_p]
                                    let m = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_m[i_p]
                                    let pdgid = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_pdgId[i_p]
                                    let vtxInitBC = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_prodVtx[i_p]
                                    let vtxTermBC = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_endVtx[i_p]
                                    let vtxInitIdx = FindVertexFromBC.Invoke(evt, vtxInitBC)
                                    let vtxTermIdx = FindVertexFromBC.Invoke(evt, vtxTermBC)
                                    select new ParticleInfo
                                    {
                                        PDGID = pdgid,
                                        vtxInit = VertexVector.Invoke(evt, vtxInitIdx),
                                        vtxTerm = VertexVector.Invoke(evt, vtxTermIdx)
                                    });

            var prs = particles.SelectMany(p => p).Where(p => p.vtxTerm.Mag() > 1.0).Count();
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();
        }

        /// <summary>
        /// Given a vertex index, return the 3D vector for the position. Null if the index is -1.
        /// </summary>
        public static Expression<Func<CollectionTree, int, ROOTNET.Interface.NTVector3>> VertexVectorQ = (evt, index) =>
            new ROOTNET.NTVector3(evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_x[index],
                evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_x[index],
                evt.McEventCollection_p4_GEN_EVENT.m_genVertices.m_x[index]);

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))] // No reference queries in the true/false of an if
        public void TestInlineIfWithComplexIfthenAnswers()
        {
            // A crash that happened in one of my seperate programs...

            var q = new QueriableDummy<CollectionTree>();
            var particles = from evt in q
                            select (from i_p in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px.Length)
                                    let px = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px[i_p]
                                    let py = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_py[i_p]
                                    let pz = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_pz[i_p]
                                    let m = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_m[i_p]
                                    let pdgid = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_pdgId[i_p]
                                    let vtxInitBC = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_prodVtx[i_p]
                                    let vtxTermBC = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_endVtx[i_p]
                                    let vtxInitIdx = vtxInitBC == 0 ? -1 : FindVertexFromBC.Invoke(evt, vtxInitBC)
                                    let vtxTermIdx = vtxTermBC == 0 ? -1 : FindVertexFromBC.Invoke(evt, vtxTermBC)
                                    select new ParticleInfo
                                    {
                                        PDGID = pdgid,
                                        vtxInit = VertexVectorQ.Invoke(evt, vtxInitIdx),
                                        vtxTerm = VertexVectorQ.Invoke(evt, vtxTermIdx)
                                    });

            var prs = particles.SelectMany(p => p).Where(p => p.vtxTerm != null).Where(p => p.vtxTerm.Mag() > 1.0).Count();
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            // Just check that we don't crash... For now. The code generated here is
            // awful, and definately needs improvement.
            //  -> common sub-expression lifting.
            //  -> make sure that stuff on the other side of the if statement doesn't get
            //     when the if branch isn't taken (currently it is moved outside).
        }

        [TestMethod]
        public void TestInlineIfWithGoodTest()
        {
            // A crash that happened in one of my seperate programs...

            var q = new QueriableDummy<CollectionTree>();
            var particles = from evt in q
                            select (from i_p in Enumerable.Range(0, evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px.Length)
                                    let px = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px[i_p]
                                    let py = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_py[i_p]
                                    let pz = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_pz[i_p]
                                    let m = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_m[i_p]
                                    let pdgid = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_pdgId[i_p]
                                    let vtxInitBC = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_prodVtx[i_p]
                                    let vtxTermBC = evt.McEventCollection_p4_GEN_EVENT.m_genParticles.m_endVtx[i_p]
                                    let vtxInitIdx = FindVertexFromBC.Invoke(evt, vtxInitBC)
                                    let vtxTermIdx = FindVertexFromBC.Invoke(evt, vtxTermBC)
                                    select new ParticleInfo
                                    {
                                        PDGID = pdgid,
                                        vtxInitOK = vtxInitBC != 0,
                                        vtxTermOK = vtxTermBC != 0,
                                        vtxInit = VertexVectorQ.Invoke(evt, vtxInitIdx),
                                        vtxTerm = VertexVectorQ.Invoke(evt, vtxTermIdx)
                                    });

            var prs = particles.SelectMany(p => p).Where(p => p.vtxTermOK).Where(p => p.vtxTerm.Mag() > 1.0).Count();
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            // We want to make sure that the vtxTerm stuff is not evaluated unless the vtxTermOK passes first...
            Assert.AreEqual(1, DummyQueryExectuor.FinalResult.CodeBody.Statements.Count(), "# of statements in top level loop");
            var topLevelLoop = DummyQueryExectuor.FinalResult.CodeBody.Statements.First() as LINQToTTreeLib.Statements.StatementForLoop;
            Assert.IsNotNull(topLevelLoop, "Top level loop isn't right");
            Assert.AreEqual(1, topLevelLoop.Statements.Count(), "# of statements in the top level block are incorrect");
            var ifStatement = topLevelLoop.Statements.First() as LINQToTTreeLib.Statements.StatementFilter;
            Assert.IsNotNull(ifStatement, "If statement first in the for loop");
        }

        /// <summary>
        /// Do the code combination we require!
        /// </summary>
        /// <param name="gcs"></param>
        /// <returns></returns>
        private IExecutableCode CombineQueries(params IExecutableCode[] gcs)
        {
            var combinedInfo = new CombinedGeneratedCode();
            foreach (var cq in gcs)
            {
                combinedInfo.AddGeneratedCode(cq);
            }

            return combinedInfo;
        }
    }
}
