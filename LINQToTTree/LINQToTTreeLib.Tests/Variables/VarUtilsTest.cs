using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="VarUtilsTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Variables
{
    /// <summary>This class contains parameterized unit tests for VarUtils</summary>
    [TestClass]
    public partial class VarUtilsTest
    {
        class freak
        {
#pragma warning disable 0169
            int _bogus;
#pragma warning restore 0169
        };

        /// <summary>Test stub for AsCPPType(Type)</summary>
        ///[PexMethod]
        public string AsCPPType(Type t)
        {
            string result = VarUtils.AsCPPType(t);
            Assert.IsNotNull(result, "null return for the type! Bad!");
            Assert.IsTrue(result.Length > 0, "Length of string should not be zero!");
            return result;
        }

        [TestMethod]
        public void TestAsCppTypeFor()
        {
            string result = VarUtils.AsCPPType(typeof(Dictionary<int, double>));
            Assert.AreEqual("map<int, double >", result, "Map type");
        }

        [TestMethod]
        public void TestAsCppTypeForMapArray()
        {
            string result = VarUtils.AsCPPType(typeof(Dictionary<int, double[]>));
            Assert.AreEqual("map<int, vector<double> >", result, "map type of array");
        }

        [TestMethod]
        public void TestAsCppTypeForMapArrayIter()
        {
            string result = VarUtils.AsCPPType(typeof(IEnumerable<Dictionary<int, double[]>>));
            Assert.AreEqual("map<int, vector<double> >::const_iterator", result, "map type of array");
        }

        [TestMethod]
        public void TestCPPType()
        {
            Assert.AreEqual("int", AsCPPType(typeof(int)), "int incorrect");
            Assert.AreEqual("bool", AsCPPType(typeof(bool)), "bool incorrect");
            Assert.AreEqual("float", AsCPPType(typeof(float)), "float incorrect");
            Assert.AreEqual("double", AsCPPType(typeof(double)), "double incorrect");
            Assert.AreEqual("freak", AsCPPType(typeof(freak)), "freak incorrect");
        }

        [TestMethod]
        public void CPPTypeForFileInfo()
        {
            // There is no special integration for this guy - so we shouldn't try
            // to see one here.
            Assert.AreEqual("FileInfo", AsCPPType(typeof(FileInfo)), "FileInfo");
        }

        [CPPObjectRepresentationType("long double")]
        class MyObj
        {

        }

        [TestMethod]
        public void CPPTypeForCustomObject()
        {
            Assert.AreEqual("long double", AsCPPType(typeof(MyObj)), "MyObj");
        }

        [TestMethod]
        public void TestCPPArrayType()
        {
            Assert.AreEqual("vector<int>", AsCPPType(typeof(int[])), "int array");
        }

        [TestMethod]
        public void TestCPPArrayIteratorType()
        {
            Assert.AreEqual("vector<int>::const_iterator", AsCPPType(typeof(IEnumerable<int[]>)), "int array iterator");
        }

        [TestMethod]
        public void TestCPPTypeROOT()
        {
            Assert.AreEqual("TString*", AsCPPType(typeof(ROOTNET.NTString)), "root tstring incorrect");
            Assert.AreEqual("TString*", AsCPPType(typeof(ROOTNET.Interface.NTString)), "root tstring interface incorrect");
        }

#if false
        /// <summary>Test stub for AsCastString(IValue)</summary>
        [PexMethod]
        [PexAllowedException(typeof(ArgumentException))]
        public string AsCastString(IValue val, [PexAssumeNotNull]Type desType)
        {
            if (desType == null)
                throw new ArgumentException("destype must not be null");
            string result = VarUtils.CastToType(val, Expression.Variable(desType, "d"));
            Assert.IsTrue(result.Contains("(("), "Result doesn't seem to contains the cast operator!");
            return result;
        }
#endif

        [TestMethod]
        public void TestActualConversion()
        {
            Assert.AreEqual("((int)10)", new ValSimple("10", typeof(double)).CastToType(Expression.Constant((int)1)), "switch to int");
        }

        [TestMethod]
        public void TestFloatToDoubleConversion()
        {
            /// Compilers can do float -> double automatically, but they might complain going the other way - lets keep things
            /// as clean as possible.
            Assert.AreEqual("10.2", new ValSimple("10.2", typeof(float)).CastToType(Expression.Constant((double)1.0)), "switch to double");
            Assert.AreEqual("((float)10.2)", new ValSimple("10.2", typeof(double)).CastToType(Expression.Constant((float)1.0)), "switch to float");
        }

        class mainObject
        {
#pragma warning disable 0649
            public int[][] j;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestCastToMainObjectWithDimentions()
        {
            Expression mainObj = Expression.Variable(typeof(mainObject), "obj");
            var directJ = Expression.MakeMemberAccess(mainObj, typeof(mainObject).GetMember("j").First());
            var j1DAccess = Expression.ArrayIndex(directJ, Expression.Constant(1));
            var j2DAccess = Expression.ArrayIndex(j1DAccess, Expression.Constant(2));

            Assert.AreEqual("(*obj).j[1][2]", new ValSimple("(*obj).j[1][2]", typeof(int)).CastToType(j2DAccess), "2D access failed");
            Assert.AreEqual("(*obj).j[1]", new ValSimple("(*obj).j[1]", typeof(int[])).CastToType(j1DAccess), "1D access failed");
            Assert.AreEqual("(*obj)", new ValSimple("obj", typeof(int[][])).CastToType(directJ), "0D access failed");
        }

#if false
        /// <summary>Test stub for CastToType(IValue, Type)</summary>
        [PexMethod]
        public string CastToType(int sourceTypeSpec, int destTypeSpec)
        {
            IValue sourceType = null;
            switch (sourceTypeSpec)
            {
                case 0:
                    sourceType = new ValSimple("10", typeof(int));
                    break;

                case 1:
                    sourceType = new ValSimple("10.0", typeof(double));
                    break;

                default:
                    return "";
            }

            Type destType = null;
            switch (destTypeSpec)
            {
                case 0:
                    destType = typeof(int);
                    break;

                case 1:
                    destType = typeof(double);
                    break;

                default:
                    return "";
            }

            string result = VarUtils.CastToType(sourceType, Expression.Variable(destType, "d"));
            if (destType == sourceType.Type
                || sourceType.Type == typeof(float) && destType == typeof(double))
            {
                Assert.IsFalse(result.Contains("(("), "More that '((' in the list of items ('" + result + "')");
            }
            else
            {
                Assert.IsTrue(result.Contains("(("), "Incorrect number of  '((' in the list of items ('" + result + "') - expecting a cast from '" + sourceType.Type.Name + " to " + destType.Name + "'!");
            }
            return result;
        }
#endif
        ///[PexMethod]
        public bool IsPointerType(Type t)
        {
            bool result = VarUtils.IsPointerType(t);
            return result;
            // TODO: add assertions to method VarUtilsTest.IsPointerType(Type)
        }

        class ntup
        {
#pragma warning disable 0649
            public int bogus;
#pragma warning restore 0649
        };

        [TestMethod]
        public void TestPointerSpecific()
        {
            /// Probably need a type traits for all these guys - some way to keep this sort of info all in one place!
            Assert.IsFalse(IsPointerType(typeof(int)), "int");
            Assert.IsFalse(IsPointerType(typeof(bool)), "int");
            Assert.IsFalse(IsPointerType(typeof(double)), "int");
            Assert.IsFalse(IsPointerType(typeof(float)), "int");
            Assert.IsTrue(IsPointerType(typeof(ntup)), "ntup");
        }

        [TestMethod]
        public void TestIsRootClass()
        {
            Assert.IsFalse(typeof(int).IsROOTClass(), "type int");
            Assert.IsTrue(typeof(ROOTNET.NTH1F).IsROOTClass(), "type ROOTNET.NTH1F");
            Assert.IsTrue(typeof(ROOTNET.Interface.NTH1F).IsROOTClass(), "type ROOTNET.Interface.NTH1F");
        }

        [TestMethod]
        public void TestReplacement()
        {
            CodeContext cc = new CodeContext();
            cc.Add("expr1", Expression.Parameter(typeof(int), "expr2"));
            cc.Add("expr3", Expression.Parameter(typeof(int), "expr1"));

            Assert.AreEqual("expr5", (new ValSimple("expr5", typeof(int))).PerformAllSubstitutions(cc).RawValue, "no translation expected 1");
            Assert.AreEqual("expr2", (new ValSimple("expr1", typeof(int))).PerformAllSubstitutions(cc).RawValue, "no translation expected 2");
            Assert.AreEqual("expr2", (new ValSimple("expr3", typeof(int))).PerformAllSubstitutions(cc).RawValue, "no translation expected 3");
            Assert.AreEqual("expr2 + expr5", (new ValSimple("expr1 + expr5", typeof(int))).PerformAllSubstitutions(cc).RawValue, "no translation expected 4");
            Assert.AreEqual("expr2 + expr2", (new ValSimple("expr1 + expr1", typeof(int))).PerformAllSubstitutions(cc).RawValue, "no translation expected 5");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestReplacementInfiniate()
        {
            CodeContext cc = new CodeContext();
            cc.Add("expr2", Expression.Parameter(typeof(int), "expr1"));
            cc.Add("expr1", Expression.Parameter(typeof(int), "expr2"));

            Assert.AreEqual("expr1", (new ValSimple("expr1", typeof(int))).PerformAllSubstitutions(cc).RawValue, "no translation expected");
        }

        [TestMethod]
        public void TestIsSimpleTerm()
        {
            Assert.IsTrue(new ValSimple("a", typeof(int)).IsSimpleTerm(), "a");
            Assert.IsTrue(new ValSimple("phi_21", typeof(int)).IsSimpleTerm(), "phi_21");
            Assert.IsFalse(new ValSimple("a.phi()", typeof(int)).IsSimpleTerm(), "a.phi()");
            Assert.IsFalse(new ValSimple("a[1]", typeof(int)).IsSimpleTerm(), "a[1]");
        }

        [TestMethod]
        public void TestParameterArray()
        {
            var sv = new ValSimple("main", typeof(int[]));
            var p = Expression.Parameter(typeof(int[]), "main");
            var r = Expression.Constant(1);
            var arr = Expression.ArrayIndex(p, r);
            Assert.AreEqual("main", sv.CastToType(p), "Expression cast of parameter");
        }

        [TestMethod]
        public void TestParameterArrayObj()
        {
            var sv = new ValSimple("main", typeof(int[]));
            var p = Expression.Parameter(typeof(int[]), "main");
            var r = Expression.Constant(1);
            var arr = Expression.ArrayIndex(p, r);
            Assert.AreEqual("main", sv.AsObjectReference(arr), "deref param");
        }
    }
}
