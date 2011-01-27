// <copyright file="VarUtilsTest.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
using System;
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Variables
{
    /// <summary>This class contains parameterized unit tests for VarUtils</summary>
    [PexClass(typeof(VarUtils))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
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
#if false
        /// Pex exploration problems.
        [PexMethod]
#endif
        public string AsCPPType(Type t)
        {
            string result = VarUtils.AsCPPType(t);
            Assert.IsNotNull(result, "null return for the type! Bad!");
            Assert.IsTrue(result.Length > 0, "Length of string should not be zero!");
            return result;
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
        public void TestCPPTypeROOT()
        {
            Assert.AreEqual("TString*", AsCPPType(typeof(ROOTNET.NTString)), "root tstring incorrect");
            Assert.AreEqual("TString*", AsCPPType(typeof(ROOTNET.Interface.NTString)), "root tstring interface incorrect");
        }

        /// <summary>Test stub for AsCastString(IValue)</summary>
        [PexMethod]
        public string AsCastString(IValue val)
        {
            string result = VarUtils.AsCastString(val);
            Assert.IsTrue(result.Contains("(("), "Result doesn't seem to contains the cast operator!");
            return result;
        }

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

            string result = VarUtils.CastToType(sourceType, destType);
            if (destType == sourceType.Type)
            {
                Assert.IsFalse(result.Contains(")("), "More that '((' in the list of items ('" + result + "')");
            }
            else
            {
                Assert.IsTrue(result.Contains(")("), "Incorrect number of  '((' in the list of items ('" + result + "') - expecting a cast!");
            }
            return result;
        }
#if false
        /// Pex exploration problems.
        [PexMethod]
#endif
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
    }
}
