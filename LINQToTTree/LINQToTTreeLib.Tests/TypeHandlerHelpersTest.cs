// <copyright file="TypeHandlerHelpersTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>

using System;
using LINQToTTreeLib.TypeHandlers;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.TypeHandlers
{
    [TestClass]
    [PexClass(typeof(TypeHandlerHelpers))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class TypeHandlerHelpersTest
    {
        [PexMethod, PexAllowedException(typeof(NotImplementedException))]
        internal Expression ProcessMethodCall(
            [PexAssumeUnderTest]TypeHandlerHelpers target,
            MethodCallExpression expr,
            out IValue result,
            IGeneratedCode gc,
            ICodeContext context
        )
        {
            Expression result01 = target.ProcessMethodCall(expr, out result, gc, context);
            return result01;
            // TODO: add assertions to method TypeHandlerHelpersTest.ProcessMethodCall(TypeHandlerHelpers, MethodCallExpression, IValue&, IGeneratedCode, ICodeContext)
        }
        [PexMethod, PexAllowedException(typeof(NotImplementedException))]
        internal IValue ProcessConstantReference(
            [PexAssumeUnderTest]TypeHandlerHelpers target,
            ConstantExpression expr,
            IGeneratedCode codeEnv
        )
        {
            IValue result = target.ProcessConstantReference(expr, codeEnv);
            return result;
            // TODO: add assertions to method TypeHandlerHelpersTest.ProcessConstantReference(TypeHandlerHelpers, ConstantExpression, IGeneratedCode)
        }
    }
}
