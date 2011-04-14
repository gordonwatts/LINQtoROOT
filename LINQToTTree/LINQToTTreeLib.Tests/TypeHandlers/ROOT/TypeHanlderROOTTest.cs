// <copyright file="TypeHanlderROOTTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
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
            [PexAssumeNotNull] IGeneratedCode codeEnv
        )
        {
            IValue result = target.ProcessConstantReference(expr, codeEnv, null, null);
            Assert.IsNotNull(result);
            return result;
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
    }
}
