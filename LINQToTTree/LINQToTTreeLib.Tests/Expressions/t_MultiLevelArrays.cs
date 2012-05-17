using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.Expressions
{
    /// <summary>
    /// Test out some of the multi-level array functionality - similar to what we expect to see when we
    /// are dealing with 
    /// </summary>
    [TestClass]
    public class t_MultiLevelArrays
    {

        public class EventInfo_p3
        {

#pragma warning disable 0649
            public uint[] m_AllTheData;
#pragma warning restore 0649
        }


        [TClonesArrayImpliedClass]
        public class GenEvent_p4
        {

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
        public class GenVertex_p4
        {

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
        public class GenParticle_p4
        {

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

        public class McEventCollection_p4
        {

#pragma warning disable 0649
            [NotAPointer]
            public GenEvent_p4 m_genEvents;
            [NotAPointer]
            public GenVertex_p4 m_genVertices;
            [NotAPointer]
            public GenParticle_p4 m_genParticles;
#pragma warning restore 0649
        }

        [TranslateToClass(typeof(CollectionTreeTranslatedTo))]
        public class CollectionTree
        {
#pragma warning disable 0649
            /// <summary>
            /// (TTree Leaf name: EventInfo_p3_McEventInfo)
            /// </summary>
            [RenameVariable("EventInfo_p3_McEventInfo")]
            public EventInfo_p3 evt;
            /// <summary>
            /// (TTree Leaf name: McEventCollection_p4_GEN_EVENT)
            /// </summary>
            [RenameVariable("McEventCollection_p4_GEN_EVENT")]
            public McEventCollection_p4 mc;
#pragma warning restore 0649
            public static string _gProxyFile = @"C:\Users\gwatts\Documents\Visual Studio 2010\Projects\HVAssociatedTests\MCAccess\ntuple_CollectionTree.h";
            public static string[] _gObjectFiles = {
    };
            public static string[] _gCINTLines = {
    };
            public static string[] _gClassesToDeclare = {
      @"vector<long>",
      @"vector<float>",
    };
            public static string[] _gClassesToDeclareIncludes = {
      @"vector",
      @"vector",
    };
        }

        public class CollectionTreeTranslatedTo : IExpressionHolder
        {
            public Expression HeldExpression { get; private set; }
            public CollectionTreeTranslatedTo(Expression expr) { HeldExpression = expr; }

#pragma warning disable 0649
            [NotAPointer]
            public EventInfo_p3 EventInfo_p3_McEventInfo;
            [NotAPointer]
            public McEventCollection_p4 McEventCollection_p4_GEN_EVENT;
#pragma warning restore 0649
            public static string _gProxyFile = @"C:\Users\gwatts\Documents\Visual Studio 2010\Projects\HVAssociatedTests\MCAccess\ntuple_CollectionTree.h";
            public static string[] _gObjectFiles = {
    };
            public static string[] _gCINTLines = {
    };
            public static string[] _gClassesToDeclare = {
      @"vector<long>",
      @"vector<float>",
    };
            public static string[] _gClassesToDeclareIncludes = {
      @"vector",
      @"vector",
    };
        }

        [TestMethod]
        public void OneLevelRenameCheck()
        {
            Expression<Func<CollectionTree, uint>> lambda = ct => ct.evt.m_AllTheData[0];
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambda.Body, caches, e => e);
            Assert.AreEqual("value(LINQToTTreeLib.Tests.Expressions.t_MultiLevelArrays+CollectionTreeTranslatedTo).EventInfo_p3_McEventInfo.m_AllTheData[0]", result.ToString(), "Final expression, translated");
        }

        [TestMethod]
        public void TwoLevelRenameCheck()
        {
            Expression<Func<CollectionTree, int>> lambda = ct => ct.mc.m_genEvents.m_eventNbr[0];
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambda.Body, caches, e => e);
            Assert.AreEqual("value(LINQToTTreeLib.Tests.Expressions.t_MultiLevelArrays+CollectionTreeTranslatedTo).McEventCollection_p4_GEN_EVENT.m_genEvents.m_eventNbr[0]", result.ToString(), "Final expression, translated");
        }
    }
}