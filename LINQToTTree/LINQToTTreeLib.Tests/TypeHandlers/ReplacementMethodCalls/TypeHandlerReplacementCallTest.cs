// <copyright file="TypeHandlerReplacementCallTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.TypeHandlers.ReplacementMethodCalls
{
    /// <summary>This class contains parameterized unit tests for TypeHandlerReplacementCall</summary>
    [PexClass(typeof(TypeHandlerReplacementCall))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class TypeHandlerReplacementCallTest
    {
        [TestInitialize]
        public void TestInit()
        {
            TypeHandlerReplacementCall.ClearTypeList();
        }
        /// <summary>Test stub for CanHandle(Type)</summary>
        [PexMethod]
        public bool CanHandle([PexAssumeUnderTest]TypeHandlerReplacementCall target, Type t)
        {
            bool result = target.CanHandle(t);
            return result;
            // TODO: add assertions to method TypeHandlerReplacementCallTest.CanHandle(TypeHandlerReplacementCall, Type)
        }

        /// <summary>Test stub for ProcessConstantReference(ConstantExpression, IGeneratedCode)</summary>
        [PexMethod]
        public IValue ProcessConstantReference(
            [PexAssumeUnderTest]TypeHandlerReplacementCall target,
            ConstantExpression expr,
            IGeneratedCode codeEnv
        )
        {
            IValue result = target.ProcessConstantReference(expr, codeEnv);
            return result;
            // TODO: add assertions to method TypeHandlerReplacementCallTest.ProcessConstantReference(TypeHandlerReplacementCall, ConstantExpression, IGeneratedCode)
        }

        /// <summary>Test stub for ProcessMethodCall(MethodCallExpression, IValue&amp;, IGeneratedCode, ICodeContext)</summary>
        [PexMethod]
        public Expression ProcessMethodCall(
            [PexAssumeUnderTest]TypeHandlerReplacementCall target,
            MethodCallExpression expr,
            out IValue result,
            IGeneratedCode gc,
            ICodeContext context
        )
        {
            Expression result01 = target.ProcessMethodCall(expr, out result, gc, context);
            return result01;
            // TODO: add assertions to method TypeHandlerReplacementCallTest.ProcessMethodCall(TypeHandlerReplacementCall, MethodCallExpression, IValue&, IGeneratedCode, ICodeContext)
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestProcessConstantReference()
        {
            ProcessConstantReference(new TypeHandlerReplacementCall(), null, null);
        }

        class SimpleTest
        {
            public static double noArg() { return 0.0; }
            public static double oneArg(int freak) { return 1.0; }
        }

        [TestMethod]
        public void TestCanHandleSimpleTestWithNoFile()
        {
            var r = CanHandle(new TypeHandlerReplacementCall(), typeof(SimpleTest));
            Assert.IsFalse(r, "no spec file and so nothing should have been loaded");
        }

        [TestMethod]
        public void TestCanHandleSimpleTestWithLoadedType()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "noArg", "noArg");
            var r = CanHandle(new TypeHandlerReplacementCall(), typeof(SimpleTest));
            Assert.IsTrue(r, "everything should have been loaded");
        }

        [TestMethod]
        public void TestNoArg()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "noArg", "noArg");

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("noArg"));
            IValue result = null;
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = ProcessMethodCall(new TypeHandlerReplacementCall(), e, out result, gc, context);

            Assert.AreEqual(typeof(double), result.Type, "the type that came back ins't right");
            Assert.AreEqual("noArg()", result.RawValue, "raw translation incorrect");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRightClassBadMethod()
        {
            TypeHandlerReplacementCall.AddMethod("SimpleTest", "noArgDude", "noArg");

            var e = Expression.Call(null, typeof(SimpleTest).GetMethod("noArg"));
            IValue result = null;
            var gc = new GeneratedCode();
            var context = new CodeContext();
            var r = ProcessMethodCall(new TypeHandlerReplacementCall(), e, out result, gc, context);

            Assert.AreEqual(typeof(double), result.Type, "the type that came back ins't right");
            Assert.AreEqual("noArg()", result.RawValue, "raw translation incorrect");
        }

        [TestMethod]
        public void TestRigthClassMethodBadArgs()
        {
            Assert.Inconclusive("Class we know about, method we know about, wrong # of args");
        }

        [TestMethod]
        public void TestSameMethodsDifferentArgTypes()
        {
            Assert.Inconclusive("different argument types but otherwise identical");
        }
    }
}
