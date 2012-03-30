using System;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.ResultOperators;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.TypeHandlers.TranslationTypes;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.Expressions
{
    /// <summary>
    /// Tests to run against implied objects - these are objects that exist in the TTree as TClonesArrays and similar. They
    /// require some special handling by the code.
    /// </summary>
    [TestClass]
    public class t_ExpressionsOnImpliedObjects
    {
        [TestInitialize]
        public void Setup()
        {
            MEFUtilities.MyClassInit();
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            MEFUtilities.AddPart(new HandleGroupType());
            MEFUtilities.AddPart(new SubQueryExpressionArrayInfoFactory());
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
            [NotAPointer]
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
        public void TestClassSubObjectArrayReference()
        {
            Expression<Func<CollectionTree, uint>> arrayAccessLambda = arr => arr.EventInfo_p3_McEventInfo.m_AllTheData[0];
            var result = RunArrayLengthOnExpression(arrayAccessLambda, typeof(uint));
            Assert.AreEqual("((*arr).EventInfo_p3_McEventInfo.m_AllTheData).at(0)", result.RawValue, "Value of array access in sub-object");
        }

        [TestMethod]
        public void TestClassSubObjectArraySize()
        {
            Expression<Func<CollectionTree, int>> arrayAccessLambda = arr => arr.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px.Length;
            var result = RunArrayLengthOnExpression(arrayAccessLambda, typeof(int));
            Assert.AreEqual("(*arr).McEventCollection_p4_GEN_EVENT.m_genParticles.GetEntries()", result.RawValue, "Value of array access in sub-object");
        }

        [TestMethod]
        public void TestClassSubObjectVectorSize()
        {
            Expression<Func<CollectionTree, int>> arrayAccessLambda = arr => arr.EventInfo_p3_McEventInfo.m_AllTheData.Length;
            var result = RunArrayLengthOnExpression(arrayAccessLambda, typeof(int));
            Assert.AreEqual("(*((*arr).EventInfo_p3_McEventInfo.m_AllTheData)).size()", result.RawValue, "Value of array access in vector in proxy container");
        }

        private static IValue RunArrayLengthOnExpression(Expression arrayLenLambda, Type expectedType)
        {
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));

            var result = ExpressionToCPP.GetExpression(arrayLenLambda, gc, cc, MEFUtilities.MEFContainer);

            Assert.IsNotNull(result, "result");
            Assert.AreEqual(expectedType, result.Type, "result type");
            return result;
        }
    }
}
