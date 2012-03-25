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
            public uint[] m_AllTheData;
#pragma warning restore 0649
        }

        public class GenEvent_p4 : IExpressionHolder
        {
            public Expression HeldExpression { get; private set; }
            public GenEvent_p4(Expression expr) { HeldExpression = expr; }

#pragma warning disable 0649
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public int[] m_signalProcessId;
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public int[] m_eventNbr;
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public double[] m_eventScale;
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public double[] m_alphaQCD;
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public double[] m_alphaQED;
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public int[] m_signalProcessVtx;
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public double[][] m_weights;
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public double[][] m_pdfinfo;
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public int[][] m_randomStates;
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public uint[] m_verticesBegin;
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public uint[] m_verticesEnd;
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public uint[] m_particlesBegin;
            [ArraySizeIndex("m_genEvents.GetEntries()", Index = 0)]
            public uint[] m_particlesEnd;
#pragma warning restore 0649
        }

        public class GenVertex_p4 : IExpressionHolder
        {
            public Expression HeldExpression { get; private set; }
            public GenVertex_p4(Expression expr) { HeldExpression = expr; }

#pragma warning disable 0649
            [ArraySizeIndex("m_genVertices.GetEntries()", Index = 0)]
            public float[] m_x;
            [ArraySizeIndex("m_genVertices.GetEntries()", Index = 0)]
            public float[] m_y;
            [ArraySizeIndex("m_genVertices.GetEntries()", Index = 0)]
            public float[] m_z;
            [ArraySizeIndex("m_genVertices.GetEntries()", Index = 0)]
            public float[] m_t;
            [ArraySizeIndex("m_genVertices.GetEntries()", Index = 0)]
            public int[][] m_particlesIn;
            [ArraySizeIndex("m_genVertices.GetEntries()", Index = 0)]
            public int[][] m_particlesOut;
            [ArraySizeIndex("m_genVertices.GetEntries()", Index = 0)]
            public int[] m_id;
            [ArraySizeIndex("m_genVertices.GetEntries()", Index = 0)]
            public float[][] m_weights;
            [ArraySizeIndex("m_genVertices.GetEntries()", Index = 0)]
            public int[] m_barcode;
#pragma warning restore 0649
        }

        public class GenParticle_p4 : IExpressionHolder
        {
            public Expression HeldExpression { get; private set; }
            public GenParticle_p4(Expression expr) { HeldExpression = expr; }

#pragma warning disable 0649
            [ArraySizeIndex("m_genParticles.GetEntries()", Index = 0)]
            public float[] m_px;
            [ArraySizeIndex("m_genParticles.GetEntries()", Index = 0)]
            public float[] m_py;
            [ArraySizeIndex("m_genParticles.GetEntries()", Index = 0)]
            public float[] m_pz;
            [ArraySizeIndex("m_genParticles.GetEntries()", Index = 0)]
            public float[] m_m;
            [ArraySizeIndex("m_genParticles.GetEntries()", Index = 0)]
            public int[] m_pdgId;
            [ArraySizeIndex("m_genParticles.GetEntries()", Index = 0)]
            public int[] m_status;
            [ArraySizeIndex("m_genParticles.GetEntries()", Index = 0)]
            public float[] m_thetaPolarization;
            [ArraySizeIndex("m_genParticles.GetEntries()", Index = 0)]
            public float[] m_phiPolarization;
            [ArraySizeIndex("m_genParticles.GetEntries()", Index = 0)]
            public int[] m_prodVtx;
            [ArraySizeIndex("m_genParticles.GetEntries()", Index = 0)]
            public int[] m_endVtx;
            [ArraySizeIndex("m_genParticles.GetEntries()", Index = 0)]
            public int[] m_barcode;
            [ArraySizeIndex("m_genParticles.GetEntries()", Index = 0)]
            public short[] m_recoMethod;
#pragma warning restore 0649
        }

        public class McEventCollection_p4 : IExpressionHolder
        {
            public Expression HeldExpression { get; private set; }
            public McEventCollection_p4(Expression expr) { HeldExpression = expr; }

#pragma warning disable 0649
            public GenEvent_p4 m_genEvents;
            public GenVertex_p4 m_genVertices;
            public GenParticle_p4 m_genParticles;
#pragma warning restore 0649
        }

        public class CollectionTree : IExpressionHolder
        {
            public Expression HeldExpression { get; private set; }
            public CollectionTree(Expression expr) { HeldExpression = expr; }

#pragma warning disable 0649
            public EventInfo_p3 EventInfo_p3_McEventInfo;
            public McEventCollection_p4 McEventCollection_p4_GEN_EVENT;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestClassSubObjectArrayReference()
        {
            Expression<Func<CollectionTree, uint>> arrayAccessLambda = arr => arr.EventInfo_p3_McEventInfo.m_AllTheData[0];
            var result = RunArrayLengthOnExpression(arrayAccessLambda, typeof(uint));
            Assert.AreEqual("((*arr).EventInfo_p3_McEventInfo).m_AllTheData.At(0)", result.RawValue, "Value of array access in sub-object");
            Assert.Inconclusive(result.RawValue);
        }

        [TestMethod]
        public void TestClassSubObjectArraySize()
        {
            Expression<Func<CollectionTree, int>> arrayAccessLambda = arr => arr.McEventCollection_p4_GEN_EVENT.m_genParticles.m_px.Length;
            var result = RunArrayLengthOnExpression(arrayAccessLambda, typeof(int));
            Assert.AreEqual("((*arr).EventInfo_p3_McEventInfo).GetEntries()", result.RawValue, "Value of array access in sub-object");
            Assert.Inconclusive(result.RawValue);
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
