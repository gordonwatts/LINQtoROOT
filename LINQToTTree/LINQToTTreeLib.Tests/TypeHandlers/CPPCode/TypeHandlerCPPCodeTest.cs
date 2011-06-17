// <copyright file="TypeHandlerCPPCodeTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.TypeHandlers.CPPCode
{
    /// <summary>This class contains parameterized unit tests for TypeHandlerCPPCode</summary>
    [PexClass(typeof(TypeHandlerCPPCode))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class TypeHandlerCPPCodeTest
    {
        /// <summary>Test stub for CanHandle(Type)</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        internal bool CanHandle([PexAssumeUnderTest]TypeHandlerCPPCode target, Type t)
        {
            bool result = target.CanHandle(t);

            var attr = t.TypeHasAttribute<CPPHelperClassAttribute>();
            Assert.AreEqual(attr != null, result, "type attribute not correct");

            return result;
        }

        static class FreeClass
        {
            public static int DoIt(int arg)
            {
                throw new NotImplementedException();
            }
        }

        [CPPHelperClass]
        static class DoItClass
        {
            [CPPCode(Code = new string[] { "DoIt = arg*2;" }, IncludeFiles = new string[] { "TLorentzVector.h" })]
            public static int DoIt(int arg)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void TestCanHandleNo()
        {
            CanHandle(new TypeHandlerCPPCode(), typeof(FreeClass));
        }
        [TestMethod]
        public void TestCanHandleYes()
        {
            CanHandle(new TypeHandlerCPPCode(), typeof(DoItClass));
        }

        /// <summary>Test stub for ProcessConstantReference(ConstantExpression, IGeneratedQueryCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod, PexAllowedException(typeof(NotImplementedException))]
        internal IValue ProcessConstantReference(
            [PexAssumeUnderTest]TypeHandlerCPPCode target,
            ConstantExpression expr,
            GeneratedCode codeEnv,
            CodeContext context
        )
        {
            IValue result
               = target.ProcessConstantReference(expr, codeEnv, context, MEFUtilities.MEFContainer);
            return result;
            // TODO: add assertions to method TypeHandlerCPPCodeTest.ProcessConstantReference(TypeHandlerCPPCode, ConstantExpression, IGeneratedQueryCode, ICodeContext, CompositionContainer)
        }

        [TestMethod]
        public void TestSimpleCodeAddon()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var param = Expression.Parameter(typeof(int), "p");
            var expr = Expression.Call(typeof(DoItClass).GetMethod("DoIt"), param);

            IValue result;

            target.ProcessMethodCall(expr, out result, gc, context, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            Assert.IsNotNull(result, "result!");
            var vname = result.RawValue;

            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements that came back");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(Statements.StatementSimpleStatement), "statement type #1");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.Skip(1).First(), typeof(Statements.StatementSimpleStatement), "statement type #2");
            var st1 = gc.CodeBody.Statements.First() as Statements.StatementSimpleStatement;
            var st2 = gc.CodeBody.Statements.Skip(1).First() as Statements.StatementSimpleStatement;

            Assert.AreEqual("int " + vname, st1.Line, "line #1 is incorrect");

            var expected = new StringBuilder();
            expected.AppendFormat("{0} = p*2;", vname);
            Assert.AreEqual(expected.ToString(), st2.Line, "statement line incorrect");

            Assert.AreEqual(1, gc.IncludeFiles.Count(), "# of include files");
            Assert.AreEqual("TLorentzVector.h", gc.IncludeFiles.First(), "include file name");
        }

        /// <summary>Test stub for ProcessMethodCall(MethodCallExpression, IValue&amp;, IGeneratedQueryCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod]
        internal Expression ProcessMethodCall(
            [PexAssumeUnderTest]TypeHandlerCPPCode target,
            MethodCallExpression expr,
            out IValue result,
            IGeneratedQueryCode gc,
            ICodeContext context,
            CompositionContainer container
        )
        {
            Expression result01
               = target.ProcessMethodCall(expr, out result, gc, context, container);
            return result01;
            // TODO: add assertions to method TypeHandlerCPPCodeTest.ProcessMethodCall(TypeHandlerCPPCode, MethodCallExpression, IValue&, IGeneratedQueryCode, ICodeContext, CompositionContainer)
        }

        /// <summary>Test stub for ProcessNew(NewExpression, IValue&amp;, IGeneratedQueryCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod, PexAllowedException(typeof(NotImplementedException))]
        internal Expression ProcessNew(
            [PexAssumeUnderTest]TypeHandlerCPPCode target,
            NewExpression expression,
            out IValue result,
            IGeneratedQueryCode gc,
            ICodeContext context,
            CompositionContainer container
        )
        {
            Expression result01
               = target.ProcessNew(expression, out result, gc, context, container);
            return result01;
            // TODO: add assertions to method TypeHandlerCPPCodeTest.ProcessNew(TypeHandlerCPPCode, NewExpression, IValue&, IGeneratedQueryCode, ICodeContext, CompositionContainer)
        }

        [TestMethod]
        public void TestForUniqueReplacement()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt = Expression.Parameter(typeof(double), "ptParam");
            var p_eta = Expression.Parameter(typeof(double), "etaParam");
            var p_phi = Expression.Parameter(typeof(double), "phiParam");
            var p_E = Expression.Parameter(typeof(double), "EParam");
            var expr = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZ"), p_pt, p_eta, p_phi, p_E);

            IValue result;

            target.ProcessMethodCall(expr, out result, gc, context, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            var declStatement = gc.CodeBody.Statements.Skip(1).First() as Statements.StatementSimpleStatement;
            var setStatement = gc.CodeBody.Statements.Skip(2).First() as Statements.StatementSimpleStatement;

            Assert.IsFalse(declStatement.Line.Contains("Unique"), string.Format("Line '{0}' contains a referecen to a unique variable", declStatement.Line));
            Assert.IsFalse(setStatement.Line.Contains("Unique"), string.Format("Line '{0}' contains a referecen to a unique variable", setStatement.Line));
        }

        [TestMethod]
        public void TestReturnVariableDecl()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt = Expression.Parameter(typeof(double), "ptParam");
            var p_eta = Expression.Parameter(typeof(double), "etaParam");
            var p_phi = Expression.Parameter(typeof(double), "phiParam");
            var p_E = Expression.Parameter(typeof(double), "EParam");
            var expr = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZ"), p_pt, p_eta, p_phi, p_E);

            IValue result;

            target.ProcessMethodCall(expr, out result, gc, context, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            var declStatement = gc.CodeBody.Statements.First() as Statements.StatementSimpleStatement;

            Assert.IsTrue(declStatement.CodeItUp().First().StartsWith("TLorentzVector* aNTLorentzVector"), "return variable decl in correct");
        }

        [TestMethod]
        public void TestTwoUniqueReplacements()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt = Expression.Parameter(typeof(double), "ptParam");
            var p_eta = Expression.Parameter(typeof(double), "etaParam");
            var p_phi = Expression.Parameter(typeof(double), "phiParam");
            var p_E = Expression.Parameter(typeof(double), "EParam");
            var expr = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZUniqueTest"), p_pt, p_eta, p_phi, p_E);

            IValue result;

            target.ProcessMethodCall(expr, out result, gc, context, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            foreach (var line in gc.CodeBody.Statements)
            {
                var st = line as Statements.StatementSimpleStatement;
                Assert.IsNotNull(st, "bad statement type");
                Assert.IsFalse(st.Line.Contains("Unique"), string.Format("Line '{0}' contains a referecen to a unique variable", st.Line));
            }
        }

        [TestMethod]
        public void TestScopingBlock()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt = Expression.Parameter(typeof(double), "ptParam");
            var p_eta = Expression.Parameter(typeof(double), "etaParam");
            var p_phi = Expression.Parameter(typeof(double), "phiParam");
            var p_E = Expression.Parameter(typeof(double), "EParam");
            var expr = Expression.Call(typeof(TLZHelper).GetMethod("TestIF"), p_pt, p_eta, p_phi, p_E);

            IValue result;

            target.ProcessMethodCall(expr, out result, gc, context, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            var ifstatement = gc.CodeBody.Statements.Skip(1).First() as Statements.StatementSimpleStatement;
            Assert.IsFalse(ifstatement.Line.EndsWith(";"), string.Format("Line '{0}' ends with a semicolon", ifstatement.Line));
            string line = ifstatement.CodeItUp().First();
            Assert.IsFalse(line.EndsWith(";"), string.Format("Line '{0}' ends with a semicolon", line));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestForMissingResult()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt = Expression.Parameter(typeof(double), "ptParam");
            var p_eta = Expression.Parameter(typeof(double), "etaParam");
            var p_phi = Expression.Parameter(typeof(double), "phiParam");
            var p_E = Expression.Parameter(typeof(double), "EParam");
            var expr = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZNoResult"), p_pt, p_eta, p_phi, p_E);

            IValue result;

            target.ProcessMethodCall(expr, out result, gc, context, MEFUtilities.MEFContainer);
        }

        [CPPHelperClass]
        static class TLZHelper
        {
            [CPPCode(IncludeFiles = new string[] { "TLorentzVector.h" },
                Code = new string[]{
                "TLorentzVector tlzUnique;",
                "tlzUnique.SetPtEtaPhiE(pt, eta, phi, E);",
                "CreateTLZ = &tlzUnique;"
            })]
            public static ROOTNET.NTLorentzVector CreateTLZ(double pt, double eta, double phi, double E)
            {
                throw new NotImplementedException("This should never get called!");
                var tlz = new ROOTNET.NTLorentzVector();
                tlz.SetPtEtaPhiE(pt, eta, phi, E);
                return tlz;
            }

            [CPPCode(IncludeFiles = new string[] { "TLorentzVector.h" },
                Code = new string[]{
                "if (int != dude) {",
                "  tlzUnique.SetPtEtaPhiE(pt, eta, phi, E);",
                "}",
                "TestIF = &tlzUnique;"
            })]
            public static ROOTNET.NTLorentzVector TestIF(double pt, double eta, double phi, double E)
            {
                throw new NotImplementedException("This should never get called!");
                var tlz = new ROOTNET.NTLorentzVector();
                tlz.SetPtEtaPhiE(pt, eta, phi, E);
                return tlz;
            }

            [CPPCode(IncludeFiles = new string[] { "TLorentzVector.h" },
                Code = new string[]{
                "TLorentzVector tlzUnique;",
                "tlzUnique.SetPtEtaPhiE(pt, eta, phi, E);",
            })]
            public static ROOTNET.NTLorentzVector CreateTLZNoResult(double pt, double eta, double phi, double E)
            {
                throw new NotImplementedException("This should never get called!");
                var tlz = new ROOTNET.NTLorentzVector();
                tlz.SetPtEtaPhiE(pt, eta, phi, E);
                return tlz;
            }

            [CPPCode(IncludeFiles = new string[] { "TLorentzVector.h" },
                Code = new string[]{
                "E = 55",
                "int tlzUnique = pt",
                "CreateTLZBE = 10;"
            })]

            public static ROOTNET.NTLorentzVector CreateTLZBE(double pt, double eta, double phi, double E)
            {
                throw new NotImplementedException();
            }
            [CPPCode(IncludeFiles = new string[] { "TLorentzVector.h" },
                Code = new string[]{
                "int ttUnique, zzUnique;",
                "ttUnique = 10;",
                "zzUnique = 20;",
                "CreateTLZUniqueTest = 10;"
            })]
            public static ROOTNET.NTLorentzVector CreateTLZUniqueTest(double pt, double eta, double phi, double E)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void TestComplexArgumentReplacement()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt = Expression.Parameter(typeof(double), "ptParam");
            var p_eta = Expression.Parameter(typeof(double), "etaParam");
            var p_phi = Expression.Parameter(typeof(double), "phiParam");
            var p_E = Expression.Parameter(typeof(double), "EParam");
            var expr = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZ"), p_pt, p_eta, p_phi, p_E);

            IValue result;

            target.ProcessMethodCall(expr, out result, gc, context, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            Assert.AreEqual(4, gc.CodeBody.Statements.Count(), "# of statements total");
            var setStatement = gc.CodeBody.Statements.Skip(2).First() as Statements.StatementSimpleStatement;
            Assert.IsNotNull(setStatement, "Bad type for 3rd statement");
            Assert.IsTrue(setStatement.Line.Contains("SetPtEtaPhiE(ptParam, etaParam, phiParam, EParam)"), string.Format("Line '{0}' doesn't have correct set statement.", setStatement.Line));
        }

        [TestMethod]
        public void TestArgReplacementAtStartAndEnd()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt = Expression.Parameter(typeof(double), "ptParam");
            var p_eta = Expression.Parameter(typeof(double), "etaParam");
            var p_phi = Expression.Parameter(typeof(double), "phiParam");
            var p_E = Expression.Parameter(typeof(double), "EParam");
            var expr = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt, p_eta, p_phi, p_E);

            IValue result;

            target.ProcessMethodCall(expr, out result, gc, context, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            Assert.AreEqual(4, gc.CodeBody.Statements.Count(), "# of statements total");
            var atBeginning = gc.CodeBody.Statements.Skip(1).First() as Statements.StatementSimpleStatement;
            var atEnding = gc.CodeBody.Statements.Skip(2).First() as Statements.StatementSimpleStatement;
            Assert.IsNotNull(atBeginning, "Bad type for 3rd statement");
            Assert.IsNotNull(atEnding, "Bad type for 3rd statement");
            Assert.IsTrue(atBeginning.Line.StartsWith("EParam"), string.Format("Line '{0}' doesn't start with param replacement", atBeginning.Line));
            Assert.IsTrue(atEnding.Line.EndsWith("ptParam"), string.Format("Line '{0}' doesn't ends with param replacement", atBeginning.Line));
        }
    }
}
