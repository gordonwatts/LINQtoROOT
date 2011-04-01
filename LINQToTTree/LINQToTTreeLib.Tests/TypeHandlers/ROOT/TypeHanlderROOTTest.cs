// <copyright file="TypeHanlderROOTTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
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
        /// <summary>Test stub for CanHandle(Type)</summary>
        [PexMethod]
        public bool CanHandle([PexAssumeUnderTest]TypeHandlerROOT target, Type t)
        {
            bool result = target.CanHandle(t);
            return result;
            // TODO: add assertions to method TypeHanlderROOTTest.CanHandle(TypeHanlderROOT, Type)
        }

        /// <summary>Test stub for ProcessConstantReference(ConstantExpression, IGeneratedCode)</summary>
        [PexMethod]
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
    }
}
