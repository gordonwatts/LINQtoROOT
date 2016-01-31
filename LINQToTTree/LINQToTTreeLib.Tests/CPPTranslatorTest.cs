using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for CPPTranslator</summary>
    [TestClass]
    public partial class CPPTranslatorTest
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();
            MEFUtilities.AddPart(new VSBasic());
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

#if false
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public CPPTranslator Constructor()
        {
            CPPTranslator target = new CPPTranslator();
            MEFUtilities.Compose(target);
            return target;
            // TODO: add assertions to method CPPTranslatorTest.Constructor()
        }

#endif

        /// <summary>Test stub for TranslateGeneratedCode(GeneratedCode)</summary>
        public Dictionary<string, object> TranslateGeneratedCode(CPPTranslator target, GeneratedCode code)
        {
            MEFUtilities.Compose(target);
            Dictionary<string, object> result = target.TranslateGeneratedCode(code);
            return result;
        }

        [TestMethod]
        public void TestTranslateWithInitialValue()
        {
            CPPTranslator target = new CPPTranslator();
            var vInt = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            vInt.SetInitialValue("2");
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
        public void TestForFunctionNumber()
        {
            CPPTranslator target = new CPPTranslator();
            var vInt = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            vInt.SetInitialValue("2");
            GeneratedCode code = new GeneratedCode();
            code.SetResult(vInt);

            var innerBlock = new StatementInlineBlock();
            var vInt2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            vInt2.SetInitialValue("5");
            innerBlock.Add(vInt2);
            code.Add(innerBlock);

            var r = TranslateGeneratedCode(target, code);

            Assert.IsTrue(r.ContainsKey("NumberOfQueryFunctions"), "Number of functions isn't here");
            Assert.IsInstanceOfType(r["NumberOfQueryFunctions"], typeof(int), "# function type");
            Assert.AreEqual(1, r["NumberOfQueryFunctions"], "# of functions");

            Assert.IsTrue(r.ContainsKey("QueryFunctionBlocks"), "Missing query function blocks");
            Assert.IsInstanceOfType(r["QueryFunctionBlocks"], typeof(IEnumerable<IEnumerable<string>>), "Type is incorrect");
            var codeBlocks = r["QueryFunctionBlocks"] as IEnumerable<IEnumerable<string>>;
            Assert.AreEqual(1, codeBlocks.Count(), "Wrong number of code blocks");
        }

        class tooManyStatemnets : IExecutableCode
        {
            public tooManyStatemnets()
            {
                var vInt = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
                vInt.SetInitialValue("2");
                ResultValues = new IDeclaredParameter[] { vInt };
            }
            public IEnumerable<KeyValuePair<string, object>> VariablesToTransfer
            {
                get { return Enumerable.Empty<KeyValuePair<string, object>>(); }
            }

            public IEnumerable<IDeclaredParameter> ResultValues { get; set; }

            public void AddIncludeFile(string includeName)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<string> IncludeFiles
            {
                get { return Enumerable.Empty<string>(); }
            }

            public IEnumerable<IStatementCompound> QueryCode()
            {
                for (int i = 0; i < 300; i++)
                {
                    var innerBlock = new StatementInlineBlock();
                    var vInt2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
                    vInt2.SetInitialValue("5");
                    innerBlock.Add(vInt2);
                    yield return innerBlock;
                }
            }


            public IEnumerable<string> ReferencedLeafNames
            {
                get { throw new NotImplementedException(); }
            }


            public IEnumerable<IQMFuncExecutable> Functions
            {
                get { return new IQMFuncExecutable[] { }; }
            }

            public IEnumerable<IStatement> InitalizationStatements
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Make sure we get the "proper" number of code blocks when we have something WAY too bid.
        /// </summary>
        [TestMethod]
        public void TestForTooManyCodeBlocks()
        {
            CPPTranslator target = new CPPTranslator();
            MEFUtilities.Compose(target);

            var toomany = new tooManyStatemnets();

            var result = target.TranslateGeneratedCode(toomany);

            Assert.IsTrue(((int)result["NumberOfQueryFunctions"]) > 1, string.Format("Number of queries was not larger than 1, it was {0}", result["NumberOfQueryFunctions"]));
            var codeBlocks = result["QueryFunctionBlocks"] as IEnumerable<IEnumerable<string>>;
            Assert.AreEqual(result["NumberOfQueryFunctions"], codeBlocks.Count(), "Non-matching number of code blocks");
        }

        [TestMethod]
        public void TestObjectInitalizerInInnerBlock()
        {
            CPPTranslator target = new CPPTranslator();
            var vInt = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            vInt.SetInitialValue("2");
            GeneratedCode code = new GeneratedCode();
            code.SetResult(vInt);

            var innerBlock = new StatementInlineBlock();
            var vInt2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            vInt2.SetInitialValue("5");
            innerBlock.Add(vInt2);
            innerBlock.Add(new StatementSimpleStatement("fork = dork"));
            code.Add(innerBlock);

            var r = TranslateGeneratedCode(target, code);

            var st = (r["QueryFunctionBlocks"] as IEnumerable<IEnumerable<string>>).First().ToArray();
            Assert.AreEqual(6, st.Length, "incorrect number of statements");
            Assert.AreEqual("int " + vInt2.RawValue + "=5;", st[2].Trim(), "incorrect initialization");
        }

        [TestMethod]
        public void TestObjectPointer()
        {
            CPPTranslator target = new CPPTranslator();
            var obj = DeclarableParameter.CreateDeclarableParameterExpression(typeof(ROOTNET.NTH1F));
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

        [TestMethod]
        public void TestTranslateForIncludeFiles()
        {
            CPPTranslator target = new CPPTranslator();
            var obj = DeclarableParameter.CreateDeclarableParameterExpression(typeof(ROOTNET.NTH1F));
            GeneratedCode code = new GeneratedCode();
            code.SetResult(obj);

            var r = TranslateGeneratedCode(target, code);

            Assert.AreEqual(1, code.IncludeFiles.Count(), "# of include files");
            Assert.AreEqual("TH1F.h", code.IncludeFiles.First(), "include file name is incorrect");
        }
    }
}
