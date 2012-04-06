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
            var block = query1.CodeBody.Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(block, "top level statement");
            Assert.AreEqual(1, block.Statements.Count(), "# of statements in the code");
        }
    }
}
