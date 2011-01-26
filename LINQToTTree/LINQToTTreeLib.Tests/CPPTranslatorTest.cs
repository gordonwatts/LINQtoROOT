// <copyright file="CPPTranslatorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.TypeHandlers.ROOT;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for CPPTranslator</summary>
    [PexClass(typeof(CPPTranslator))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class CPPTranslatorTest
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            MEFUtilities.AddPart(new VSBasic());
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public CPPTranslator Constructor()
        {
            CPPTranslator target = new CPPTranslator();
            MEFUtilities.Compose(target);
            return target;
            // TODO: add assertions to method CPPTranslatorTest.Constructor()
        }

        /// <summary>Test stub for get_IncludeFiles()</summary>
        [PexMethod]
        public IEnumerable<string> IncludeFilesGet([PexAssumeUnderTest]CPPTranslator target)
        {
            MEFUtilities.Compose(target);
            IEnumerable<string> result = target.IncludeFiles;
            Assert.IsNotNull(result);
            return result;
        }

        /// <summary>Test stub for TranslateGeneratedCode(GeneratedCode)</summary>
        [PexMethod]
        [PexUseType(typeof(StatementInlineBlock))]
        public Dictionary<string, object> TranslateGeneratedCode([PexAssumeUnderTest]CPPTranslator target, GeneratedCode code)
        {
            MEFUtilities.Compose(target);
            Dictionary<string, object> result = target.TranslateGeneratedCode(code);
            return result;
            // TODO: add assertions to method CPPTranslatorTest.TranslateGeneratedCode(CPPTranslator, GeneratedCode)
        }
        [TestMethod]
        public void TranslateGeneratedCodeThrowsNullReferenceException584()
        {
            CPPTranslator cPPTranslator;
            VarInteger varInteger;
            GeneratedCode generatedCode;
            Dictionary<string, object> dictionary;
            cPPTranslator = CPPTranslatorFactory.Create();
            varInteger = new VarInteger();
            generatedCode = GeneratedCodeFactory.Create((IVariable)varInteger);
            dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
        }

        [TestMethod]
        public void TestTranslateWithInitialValue()
        {
            CPPTranslator target = new CPPTranslator();
            VarInteger vInt = new VarInteger() { InitialValue = new ValSimple("2", typeof(int)) };
            GeneratedCode code = new GeneratedCode();
            code.SetResult(vInt);

            var r = TranslateGeneratedCode(target, code);

            Assert.IsTrue(r.ContainsKey("ResultVariable"), "Result variable is missing");
            Assert.IsInstanceOfType(r["ResultVariable"], typeof(CPPTranslator.VarInfo), "bad type for the result variable");
            var rv = r["ResultVariable"] as CPPTranslator.VarInfo;
            Assert.AreEqual("2", rv.InitialValue, "initial value");
        }

        [TestMethod]
        public void TestTranslateWithObjectInitialValue()
        {
            /// This is an object we have to load from the input list - make sure we call our template method to do it!

            CPPTranslator target = new CPPTranslator();
            var vObj = new ROOTObjectValue(new ROOTNET.NTH1F("hi", "title", 10, 0.0, 10.0));

            GeneratedCode code = new GeneratedCode();
            code.SetResult(vObj);

            var r = TranslateGeneratedCode(target, code);

            Assert.IsTrue(r.ContainsKey("ResultVariable"), "Result variable is missing");
            Assert.IsInstanceOfType(r["ResultVariable"], typeof(CPPTranslator.VarInfo), "bad type for the result variable");
            var rv = r["ResultVariable"] as CPPTranslator.VarInfo;
            Assert.AreEqual("LoadFromInputList<TH1F>(\"" + vObj.RawValue + "\")", rv.InitialValue, "initial value");
        }
    }
}
