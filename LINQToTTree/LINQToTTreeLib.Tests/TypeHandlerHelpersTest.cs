using System;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.TypeHandlers
{
    [TestClass]
    public partial class TypeHandlerHelpersTest
    {
#if false
        [PexMethod, PexAllowedException(typeof(NotImplementedException))]
        internal Expression ProcessMethodCall(
            [PexAssumeUnderTest]TypeHandlerHelpers target,
            MethodCallExpression expr,
            IGeneratedQueryCode gc,
            ICodeContext context
        )
        {
            Expression result01 = target.ProcessMethodCall(expr, gc, context, null);
            return result01;
            // TODO: add assertions to method TypeHandlerHelpersTest.ProcessMethodCall(TypeHandlerHelpers, MethodCallExpression, IValue&, IGeneratedCode, ICodeContext)
        }
        [PexMethod, PexAllowedException(typeof(NotImplementedException))]
        internal IValue ProcessConstantReference(
            [PexAssumeUnderTest]TypeHandlerHelpers target,
            ConstantExpression expr,
            IGeneratedQueryCode codeEnv
        )
        {
            IValue result = target.ProcessConstantReference(expr, codeEnv, null);
            return result;
            // TODO: add assertions to method TypeHandlerHelpersTest.ProcessConstantReference(TypeHandlerHelpers, ConstantExpression, IGeneratedCode)
        }
#endif
    }
}
