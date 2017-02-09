using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.QueryVisitors;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace LINQToTTreeLib.TypeHandlers.CPPCode
{
    /// <summary>This class contains tests TypeHandlerCPPCode</summary>
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

            /// <summary>
            /// A simple one with a string.
            /// </summary>
            /// <param name="var"></param>
            /// <returns></returns>
            [CPPCode(Code = new string[] { "DoItWithAString = strlen(arg);" }, IncludeFiles = new string[] { "stdlib.h" })]
            public static int DoItWithAString(string arg)
            {
                throw new NotImplementedException();
            }

            [CPPCode(Code = new string[] { "DoItWithArray = vector<float>();" }, IncludeFiles = new string[] { "vector" })]
            public static float[] DoItWithArray(int i)
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
            // See problems referencing return variables
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();

            var param = Expression.Parameter(typeof(int), "p");
            var expr = Expression.Call(typeof(DoItClass).GetMethod("DoItWithArray"), param);

            var result = target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            Assert.AreEqual(typeof(float[]), result.Type);
        }

        [TestMethod]
        public void CheckArrayReturnType()
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

            var expected = new StringBuilder();
            expected.AppendFormat("{0} = p*2;", vname);
            Assert.AreEqual(expected.ToString(), st1.CodeItUp().First(), "statement line incorrect");

            Assert.AreEqual(1, gc.IncludeFiles.Count(), "# of include files");
            Assert.AreEqual("TLorentzVector.h", gc.IncludeFiles.First(), "include file name");
        }

        [TestMethod]
        public void CodeAddonWithStringArgument()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();

            var param = Expression.Parameter(typeof(string), "p");
            var expr = Expression.Call(typeof(DoItClass).GetMethod("DoItWithAString"), param);

            var result = target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            Assert.IsNotNull(result, "result!");
            var vname = result.RawValue;

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "# of statements that came back");
            var st1 = gc.CodeBody.Statements.First() as IStatement;

            var expected = new StringBuilder();
            expected.AppendFormat("{0} = strlen(p);", vname);
            Assert.AreEqual(expected.ToString(), st1.CodeItUp().First(), "statement line incorrect");
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
            var st2 = gc.CodeBody.Statements.First().CodeItUp().First();

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
        public void CPPInputDependent()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var param = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var paramplus = Expression.MakeBinary(ExpressionType.Add, param, Expression.Constant(1));
            var expr = Expression.Call(typeof(DoItClass).GetMethod("DoIt"), paramplus);

            var result = target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            // Check the dependents.
            var st = gc.CodeBody.Statements.First() as ICMStatementInfo;
            Assert.AreEqual(1, st.DependentVariables.Count(), "# of dependents");
            Assert.AreEqual(param.RawValue, st.DependentVariables.First());
        }

        [TestMethod]
        public void RenameCPPInputVariableVariable()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var param = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var paramplus = Expression.MakeBinary(ExpressionType.Add, param, Expression.Constant(1));
            var expr = Expression.Call(typeof(DoItClass).GetMethod("DoIt"), paramplus);

            var result = target.CodeMethodCall(expr, gc, MEFUtilities.MEFContainer);

            // Check the dependents.
            gc.CodeBody.RenameVariable(param.RawValue, "abogus_1234");

            gc.DumpCodeToConsole();

            var st = gc.CodeBody.Statements.First() as ICMStatementInfo;
            Assert.AreEqual(1, st.DependentVariables.Count(), "# of dependents");
            Assert.AreEqual("abogus_1234", st.DependentVariables.First());
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

            var declStatement = gc.CodeBody.DeclaredVariables.ToArray();
            Assert.AreEqual(1, declStatement.Length);
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
            Assert.AreEqual(1, cmInfo.ResultVariables.Count(), "# of result variables");
            Assert.AreEqual(r.RawValue, cmInfo.ResultVariables.First(), "Result variable name");
            Assert.AreEqual(2, cmInfo.DependentVariables.Count(), "# of dependent variables");
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

            var ifstatement = gc.CodeBody.Statements.First().CodeItUp().First();
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
            var setStatement = gc.CodeBody.Statements.First().CodeItUp().Skip(1).First();
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
            var atEnding = gc.CodeBody.Statements.First().CodeItUp().Skip(1).FirstOrDefault();
            Assert.IsNotNull(atEnding, "Bad type for 3rd statement");
            Assert.IsTrue(atEnding.EndsWith("ptParam"), string.Format("Line '{0}' doesn't ends with parameter replacement", atEnding));
        }

        [TestMethod]
        public void CPPAreIdentical()
        {
            // two identical expressions
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_eta = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_phi = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_E = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));

            // Create two identical calls
            var e1 = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt, p_eta, p_phi, p_E);
            var e1Value = target.CodeMethodCall(e1, gc, MEFUtilities.MEFContainer);

            var e2 = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt, p_eta, p_phi, p_E);
            var e2Value = target.CodeMethodCall(e1, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            // Now, extract the two main statements.
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements");
            var s1 = gc.CodeBody.Statements.First() as ICMStatementInfo;
            var s2 = gc.CodeBody.Statements.Skip(1).First() as ICMStatementInfo;

            // Now, see if we can do the requirement.
            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1, "We should be able to do the translation");
            Assert.AreEqual(1, r.Item2.Count(), "# of variable translations required");
            Assert.IsTrue(r.Item2.First().Item1.StartsWith("aNTLorentz"), "# of variable translations required");
        }

        [TestMethod]
        public void CPPNotIdentical()
        {
            // two identical expressions
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_eta = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_phi = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_E = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));

            // Create two identical calls
            var e1 = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt, p_eta, p_phi, p_E);
            var e1Value = target.CodeMethodCall(e1, gc, MEFUtilities.MEFContainer);

            var c2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var e2 = Expression.Call(typeof(DoItClass).GetMethod("DoIt"), c2);
            var e2Value = target.CodeMethodCall(e2, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            // Now, extract the two main statements.
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements");
            var s1 = gc.CodeBody.Statements.First() as ICMStatementInfo;
            var s2 = gc.CodeBody.Statements.Skip(1).First() as ICMStatementInfo;

            // Now, see if we can do the requirement.
            var r = s1.RequiredForEquivalence(s2);
            Assert.IsFalse(r.Item1, "We should be able to do the translation");
        }

        [TestMethod]
        public void CPPNeedReplacements()
        {
            // two identical expressions
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt_1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_eta_1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_phi_1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_E_1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));

            // Create first call
            var e1 = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt_1, p_eta_1, p_phi_1, p_E_1);
            var e1Value = target.CodeMethodCall(e1, gc, MEFUtilities.MEFContainer);

            var p_pt_2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_eta_2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_phi_2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_E_2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));

            var e2 = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt_2, p_eta_2, p_phi_2, p_E_2);
            var e2Value = target.CodeMethodCall(e2, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            // Now, extract the two main statements.
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements");
            var s1 = gc.CodeBody.Statements.First() as ICMStatementInfo;
            var s2 = gc.CodeBody.Statements.Skip(1).First() as ICMStatementInfo;

            // Now, see if we can do the requirement.
            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1, "We should be able to do the translation");
            Assert.AreEqual(5, r.Item2.Count(), "# of variable translations required");
            Assert.AreEqual(p_pt_1.RawValue, r.Item2.Where(i => i.Item1 == p_pt_2.RawValue).First().Item2);
        }

        [TestMethod]
        public void CPPNeedSomeReplacements()
        {
            // two identical expressions
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt_1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_eta_1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_phi_1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_E_1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));

            // Create first call
            var e1 = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt_1, p_eta_1, p_phi_1, p_E_1);
            var e1Value = target.CodeMethodCall(e1, gc, MEFUtilities.MEFContainer);

            var p_pt_2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_eta_2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_phi_2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_E_2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));

            var e2 = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt_2, p_eta_2, p_phi_2, p_E_2);
            var e2Value = target.CodeMethodCall(e2, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            // Now, extract the two main statements.
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements");
            var s1 = gc.CodeBody.Statements.First() as ICMStatementInfo;
            var s2 = gc.CodeBody.Statements.Skip(1).First() as ICMStatementInfo;

            // Now, see if we can do the requirement.
            var renames = new Tuple<string, string>[] { new Tuple<string, string>(p_pt_2.RawValue, p_pt_1.RawValue), new Tuple<string, string>(p_eta_2.RawValue, p_eta_1.RawValue) };
            var r = s1.RequiredForEquivalence(s2, renames);
            Assert.IsTrue(r.Item1, "We should be able to do the translation");
            Assert.AreEqual(3, r.Item2.Count(), "# of variable translations required");
        }

        [TestMethod]
        public void CPPTryCombineSame()
        {
            // two identical expressions
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_eta = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_phi = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_E = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));

            // Create two identical calls
            var e1 = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt, p_eta, p_phi, p_E);
            var e1Value = target.CodeMethodCall(e1, gc, MEFUtilities.MEFContainer);

            var e2 = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt, p_eta, p_phi, p_E);
            var e2Value = target.CodeMethodCall(e1, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            // Now, extract the two main statements.
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements");
            var s1 = gc.CodeBody.Statements.First() as IStatement;
            var s2 = gc.CodeBody.Statements.Skip(1).First() as IStatement;

            // Now, try-combine should just "work", as it were.
            var opt = new OptTest();
            var r = s1.TryCombineStatement(s2, opt);
            Assert.IsTrue(r);
            Assert.AreEqual(1, opt.Renames.Count);
        }

        [TestMethod]
        public void CPPTryCombineNotSame()
        {
            // two identical expressions
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt_1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_eta = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_phi = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_E = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));

            // Create two identical calls
            var e1 = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt_1, p_eta, p_phi, p_E);
            var e1Value = target.CodeMethodCall(e1, gc, MEFUtilities.MEFContainer);

            var p_pt_2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var e2 = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt_2, p_eta, p_phi, p_E);
            var e2Value = target.CodeMethodCall(e2, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            // Now, extract the two main statements.
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements");
            var s1 = gc.CodeBody.Statements.First() as IStatement;
            var s2 = gc.CodeBody.Statements.Skip(1).First() as IStatement;

            // Now, try-combine should just "work", as it were.
            var opt = new OptTest();
            var r = s1.TryCombineStatement(s2, opt);
            Assert.IsFalse(r);
        }

        [TestMethod]
        public void CPPRenameVariables()
        {
            // two identical expressions
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var p_pt_1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_eta = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_phi = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var p_E = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));

            // Create call
            var e1 = Expression.Call(typeof(TLZHelper).GetMethod("CreateTLZBE"), p_pt_1, p_eta, p_phi, p_E);
            var e1Value = target.CodeMethodCall(e1, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            // Now, extract the main statement.
            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "# of statements");
            var s1 = gc.CodeBody.Statements.First() as IStatement;

            // Make sure the variable is there and then isn't.
            var p_pt_2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            Assert.IsTrue(gc.DumpCode().Where(l => l.Contains(p_pt_1.RawValue)).Any(), "the pt variable should be there.");
            Assert.IsFalse(gc.DumpCode().Where(l => l.Contains(p_pt_2.RawValue)).Any(), "the pt variable should be there.");

            s1.RenameVariable(p_pt_1.RawValue, p_pt_2.RawValue);
            Assert.IsFalse(gc.DumpCode().Where(l => l.Contains(p_pt_1.RawValue)).Any(), "the pt variable should be there.");
            Assert.IsTrue(gc.DumpCode().Where(l => l.Contains(p_pt_2.RawValue)).Any(), "the pt variable should be there.");
        }

        [TestMethod]
        public void CacheObjectWhenCPPChanges()
        {
            TestUtils.ResetLINQLibrary();
            // Make sure that when the C++ code changes, the cache key for lookup of results will also change.
            var q = new QueriableDummy<ntup>();
            var i = q.Select(e => DoItClass.DoIt(e.run)).Where(x => x > 2).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Look for a hash value.
            var str1 = FormattingQueryVisitor.Format(query);
            Console.WriteLine(str1);
            var m = Regex.Match(str1, @"DoIt([^\(]+)");
            Assert.IsTrue(m.Success);
        }


        private class OptTest : ICodeOptimizationService
        {
            public void ForceRenameVariable(string originalName, string newName)
            {
                Renames.Add(originalName);
            }

            public List<string> Renames = new List<string>();
            public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
            {
                Renames.Add(oldName);
                return true;
            }
        }
    }
}
