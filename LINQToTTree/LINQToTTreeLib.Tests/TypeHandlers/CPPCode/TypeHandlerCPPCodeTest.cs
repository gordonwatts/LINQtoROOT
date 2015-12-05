using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LINQToTTreeLib.TypeHandlers.CPPCode
{
    /// <summary>This class contains parameterized unit tests for TypeHandlerCPPCode</summary>
    [TestClass]
    public partial class TypeHandlerCPPCodeTest
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();
            var t = new TypeHandlerCache();
            MEFUtilities.Compose(t);
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>Test stub for CanHandle(Type)</summary>
        internal bool CanHandle(TypeHandlerCPPCode target, Type t)
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

        [TestMethod]
        public void TestSimpleCodeAddon()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();

            var param = Expression.Parameter(typeof(int), "p");
            var expr = Expression.Call(typeof(DoItClass).GetMethod("DoIt"), param);

            var result = target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            Assert.IsNotNull(result, "result!");
            var vname = result.RawValue;

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "# of statements that came back");
            var st1 = gc.CodeBody.Statements.First() as IStatement;

            Assert.AreEqual("int " + vname + ";", st1.CodeItUp().First(), "line #1 is incorrect");

            var expected = new StringBuilder();
            expected.AppendFormat("{0} = p*2;", vname);
            Assert.AreEqual(expected.ToString(), st1.CodeItUp().Skip(1).First(), "statement line incorrect");

            Assert.AreEqual(1, gc.IncludeFiles.Count(), "# of include files");
            Assert.AreEqual("TLorentzVector.h", gc.IncludeFiles.First(), "include file name");
        }

        [TestMethod]
        public void TestSimpleTimesTwo()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var param = Expression.Parameter(typeof(int), "p");
            var paramplus = Expression.MakeBinary(ExpressionType.Add, param, Expression.Constant(1));
            var expr = Expression.Call(typeof(DoItClass).GetMethod("DoIt"), paramplus);

            var result = target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            var vname = result.RawValue;
            var st2 = gc.CodeBody.Statements.First().CodeItUp().Skip(1).First();

            var expected = new StringBuilder();
            expected.AppendFormat("{0} = (p+1)*2;", vname);
            Assert.AreEqual(expected.ToString(), st2, "statement line incorrect");
        }

        [TestMethod]
        public void RenameCPPResultVariable()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var param = Expression.Parameter(typeof(int), "p");
            var paramplus = Expression.MakeBinary(ExpressionType.Add, param, Expression.Constant(1));
            var expr = Expression.Call(typeof(DoItClass).GetMethod("DoIt"), paramplus);

            var result = target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);
            gc.CodeBody.RenameVariable(result.RawValue, "abogus_1234");

            gc.DumpCodeToConsole();

            Assert.IsTrue(gc.DumpCode().Where(s => s.Contains("int abogus_1234")).Any(), "Didn't find the variable name in the code");
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

            target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            var declStatement = gc.CodeBody.Statements.First().CodeItUp().Skip(1).First();
            var setStatement = gc.CodeBody.Statements.First().CodeItUp().Skip(2).First();

            Assert.IsFalse(declStatement.Contains("Unique"), string.Format("Line '{0}' contains a reference to a unique variable", declStatement));
            Assert.IsFalse(setStatement.Contains("Unique"), string.Format("Line '{0}' contains a reference to a unique variable", setStatement));
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

            target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            var declStatement = gc.CodeBody.Statements.First();

            Assert.IsTrue(declStatement.CodeItUp().First().StartsWith("TLorentzVector* aNTLorentzVector"), "return variable declaration in correct");
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

            target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            foreach (var line in gc.CodeBody.Statements.First().CodeItUp())
            {
                Assert.IsNotNull(line, "bad statement type");
                Assert.IsFalse(line.Contains("Unique"), string.Format("Line '{0}' contains a reference to a unique variable", line));
            }
        }

        [TestMethod]
        public void TestCMVariables()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_eta = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_phi = p_pt;
            var p_E = p_eta;
            var expr = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZ"), p_pt, p_eta, p_phi, p_E);

            var r = target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            var ccpstatement = gc.CodeBody.Statements.First();
            Assert.IsNotNull(ccpstatement);
            var cmInfo = ccpstatement as ICMStatementInfo;
            Assert.IsNotNull(cmInfo);
            Assert.AreEqual(1, cmInfo.ResultVariables.Count, "# of result variables");
            Assert.AreEqual(r.RawValue, cmInfo.ResultVariables.First(), "Result variable name");
            Assert.AreEqual(2, cmInfo.DependentVariables.Count, "# of dependent variables");
            Assert.IsTrue(cmInfo.DependentVariables.Contains(p_pt.RawValue), "doesn't have pt");
            Assert.IsTrue(cmInfo.DependentVariables.Contains(p_eta.RawValue), "Doesn't have eta");
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

            target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            var ifstatement = gc.CodeBody.Statements.First().CodeItUp().Skip(1).First();
            Assert.IsFalse(ifstatement.EndsWith(";"), string.Format("Line '{0}' ends with a semicolon", ifstatement));
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

            target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);
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
#pragma warning disable 0162
                var tlz = new ROOTNET.NTLorentzVector();
                tlz.SetPtEtaPhiE(pt, eta, phi, E);
                return tlz;
#pragma warning disable 0162
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

            target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "# of statements total");
            var setStatement = gc.CodeBody.Statements.First().CodeItUp().Skip(2).First();
            Assert.IsNotNull(setStatement, "Bad type for 3rd statement");
            Assert.IsTrue(setStatement.Contains("SetPtEtaPhiE(ptParam, etaParam, phiParam, EParam)"), string.Format("Line '{0}' doesn't have correct set statement.", setStatement));
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

            target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "# of statements total");
            var atBeginning = gc.CodeBody.Statements.First().CodeItUp().Skip(1).FirstOrDefault();
            var atEnding = gc.CodeBody.Statements.First().CodeItUp().Skip(2).FirstOrDefault();
            Assert.IsNotNull(atBeginning, "Bad type for 3rd statement");
            Assert.IsNotNull(atEnding, "Bad type for 3rd statement");
            Assert.IsTrue(atBeginning.StartsWith("EParam"), string.Format("Line '{0}' doesn't start with parameter replacement", atBeginning));
            Assert.IsTrue(atEnding.EndsWith("ptParam"), string.Format("Line '{0}' doesn't ends with parameter replacement", atBeginning));
        }
    }
}
