// <copyright file="CPPTranslatorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
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
        public void TestTranslateWithInitialValue()
        {
            CPPTranslator target = new CPPTranslator();
            VarInteger vInt = new VarInteger() { InitialValue = new ValSimple("2", typeof(int)) };
            GeneratedCode code = new GeneratedCode();
            code.SetResult(vInt);

            var r = TranslateGeneratedCode(target, code);

            Assert.IsTrue(r.ContainsKey("ResultVariables"), "Result variable is missing");
            Assert.IsInstanceOfType(r["ResultVariables"], typeof(IEnumerable<CPPTranslator.VarInfo>), "bad type for the result variable");
            var rList = r["ResultVariables"] as IEnumerable<CPPTranslator.VarInfo>;
            Assert.AreEqual(1, rList.Count(), "incorrect # of result variables");
            var rv = rList.First();
            Assert.AreEqual("2", rv.InitialValue, "initial value");
        }

        [TestMethod]
        public void TestObjectInitalizerInInnerBlock()
        {
            CPPTranslator target = new CPPTranslator();
            VarInteger vInt = new VarInteger() { InitialValue = new ValSimple("2", typeof(int)) };
            GeneratedCode code = new GeneratedCode();
            code.SetResult(vInt);

            var innerBlock = new StatementInlineBlock();
            VarInteger vInt2 = new VarInteger() { InitialValue = new ValSimple("5", typeof(int)) };
            innerBlock.Add(vInt2);
            code.Add(innerBlock);

            var r = TranslateGeneratedCode(target, code);

            Assert.IsTrue(r.ContainsKey("ProcessStatements"), "ProcessStatements missing");
            Assert.IsInstanceOfType(r["ProcessStatements"], typeof(IEnumerable<string>), "bad processing statements type");
            var st = (r["ProcessStatements"] as IEnumerable<string>).ToArray();
            Assert.AreEqual(5, st.Length, "incorrect number of statements");
            Assert.AreEqual("int " + vInt2.RawValue + "=5;", st[2].Trim(), "incorrect initalization");
        }

        [TestMethod]
        public void TestObjectPointer()
        {
            CPPTranslator target = new CPPTranslator();
            VarObject obj = new VarObject(typeof(ROOTNET.NTH1F));
            GeneratedCode code = new GeneratedCode();
            code.SetResult(obj);

            var r = TranslateGeneratedCode(target, code);

            var rlist = r["ResultVariables"] as IEnumerable<CPPTranslator.VarInfo>;
            var rv = rlist.First();
            Assert.AreEqual("TH1F*", rv.VariableType, "type is not right");
            var inFiles = code.IncludeFiles.ToArray();
            foreach (var item in inFiles)
            {
                Console.WriteLine("include file '{0}'", item);
            }

            Assert.IsTrue(inFiles.Contains("TH1F.h"), "Missing include file");
        }
    }
}
