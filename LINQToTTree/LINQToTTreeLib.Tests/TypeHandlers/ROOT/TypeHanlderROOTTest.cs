// <copyright file="TypeHanlderROOTTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.TypeHandlers.ROOT
{
    /// <summary>This class contains parameterized unit tests for TypeHanlderROOT</summary>
    [PexClass(typeof(TypeHandlerROOT))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class TypeHanlderROOTTest
    {
        [TestInitialize]
        public void Setup()
        {
            MEFUtilities.MyClassInit();
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.Compose(new TypeHandlerCache());
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>Test stub for CanHandle(Type)</summary>
        [PexMethod]
        public bool CanHandle([PexAssumeUnderTest]TypeHandlerROOT target, Type t)
        {
            bool result = target.CanHandle(t);
            return result;
            // TODO: add assertions to method TypeHanlderROOTTest.CanHandle(TypeHanlderROOT, Type)
        }

        /// <summary>Test stub for ProcessConstantReference(ConstantExpression, IGeneratedCode)</summary>
        [PexMethod, PexAllowedException(typeof(NullReferenceException))]
        public IValue ProcessConstantReference(
            [PexAssumeUnderTest]TypeHandlerROOT target,
            [PexAssumeNotNull] ConstantExpression expr,
            [PexAssumeNotNull] IGeneratedQueryCode codeEnv
        )
        {
            IValue result = target.ProcessConstantReference(expr, codeEnv, null);
            Assert.IsNotNull(result);
            return result;
        }

        [TestMethod]
        public void TestQueueForTransferNoNameChange()
        {
            var t = new TypeHandlerROOT();

            var origRootObj = new ROOTNET.NTH1F("hi", "there", 10, 10.0, 20.0);
            var rootObj = Expression.Constant(origRootObj);

            var gc = new GeneratedCode();
            var result = t.ProcessConstantReference(rootObj, gc, MEFUtilities.MEFContainer);

            Assert.IsNotNull(result);

            Assert.AreEqual(1, gc.VariablesToTransfer.Count(), "Variables to transfer");
            Assert.AreEqual("hi", origRootObj.Name, "Name of original root object");
            Assert.AreEqual("there", origRootObj.Title, "Title of original root object");
        }

        [TestMethod]
        public void TestQueueTwice()
        {
            var t = new TypeHandlerROOT();

            var origRootObj = new ROOTNET.NTH1F("hi", "there", 10, 10.0, 20.0);
            var rootObj = Expression.Constant(origRootObj);

            var gc = new GeneratedCode();
            var result1 = t.ProcessConstantReference(rootObj, gc, MEFUtilities.MEFContainer);
            var result2 = t.ProcessConstantReference(rootObj, gc, MEFUtilities.MEFContainer);

            Assert.AreEqual(1, gc.VariablesToTransfer.Count(), "Variables to transfer");

            Assert.AreEqual(result1.RawValue, result2.RawValue, "Expected the same raw value for the same object in the same expression");
        }

        [TestMethod]
        public void TestROOTMethodReference()
        {
            var expr = Expression.Variable(typeof(ROOTNET.NTH1F), "myvar");
            var getEntriesMethod = typeof(ROOTNET.NTH1F).GetMethod("GetEntries", new Type[0]);
            var theCall = Expression.Call(expr, getEntriesMethod);

            var target = new TypeHandlerROOT();



            IValue resultOfCall;
            var gc = new GeneratedCode();
            var cc = new CodeContext();
            var returned = target.ProcessMethodCall(theCall, out resultOfCall, gc, cc, MEFUtilities.MEFContainer);

            Assert.AreEqual("(*myvar).GetEntries()", resultOfCall.RawValue, "call is incorrect");
        }

        [TestMethod]
        public void TestBasicProcessNew()
        {
            /// Test a very simple process new

            var createTLZ = Expression.New(typeof(ROOTNET.NTLorentzVector).GetConstructor(new Type[0]));
            var target = new TypeHandlerROOT();
            IValue resultOfCall;
            var gc = new GeneratedCode();
            var cc = new CodeContext();
            var expr = target.ProcessNew(createTLZ, out resultOfCall, gc, cc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            Assert.AreEqual(createTLZ.ToString(), expr.ToString(), "Returned expression");
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of coded statements");
            var s1 = gc.CodeBody.Statements.First();
            var s2 = gc.CodeBody.Statements.Skip(1).First();
            Assert.IsInstanceOfType(s1, typeof(Statements.StatementSimpleStatement), "s1 type");
            Assert.IsInstanceOfType(s2, typeof(Statements.StatementSimpleStatement), "s1 type");
            var s1s = s1 as Statements.StatementSimpleStatement;
            var s2s = s2 as Statements.StatementSimpleStatement;

            Assert.IsTrue(s1s.Line.Contains("TLorentzVector"), "first line is not that good");
            Assert.IsTrue(s2s.Line.Contains("TLorentzVector *"), "second line is not that good");
        }

        /// <summary>Test stub for ProcessNew(NewExpression, IValue&amp;, IGeneratedQueryCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod]
        public Expression ProcessNew(
            [PexAssumeUnderTest]TypeHandlerROOT target,
            NewExpression expression,
            out IValue result,
            GeneratedCode gc,
            CodeContext context
        )
        {
            Expression result01
               = target.ProcessNew(expression, out result, gc, context, MEFUtilities.MEFContainer);

            return result01;
        }
    }
}
