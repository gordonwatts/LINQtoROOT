using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.TypeHandlers
{
    /// <summary>
    /// The Convert.ToDouble is sometimes used in our code. Deal with it.
    /// cleanly here.
    /// Convert.ToDouble (value) for example.
    /// </summary>
    [Export(typeof(ITypeHandler))]
    class TypeHandlerConvert : ITypeHandler
    {
        public bool CanHandle(Type t)
        {
            return t == typeof(Convert);
        }

        /// <summary>
        /// There is never a constant reference, so don't let it go!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public IValue ProcessConstantReference(System.Linq.Expressions.ConstantExpression expr, IGeneratedQueryCode codeEnv, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// NOt processing any sort of expression of this type!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessConstantReferenceExpression(ConstantExpression expr, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// No need to transform a conversion - so we just plow on.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
        {
            return expr;
        }

        /// <summary>
        /// Deal with the various method calls to convert.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public IValue CodeMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, CompositionContainer container)
        {
            if (expr.Method.Name == "ToDouble")
                return ProcessToDouble(expr, gc, container);

            ///
            /// We don't know how to deal with this particular convert!
            /// 

            throw new NotImplementedException("Can't translate the call Convert." + expr.Method.Name);
        }

        /// <summary>
        /// Convert something to a double. We don't actually do anything as long as this is an expression that we
        /// can naturally convert (int, float, etc.).
        /// 
        /// We are expecting an expressio nthat is ToDouble(Convert()), so if we can't see the convert, then we bail.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        private IValue ProcessToDouble(MethodCallExpression expr, IGeneratedQueryCode gc, CompositionContainer container)
        {
            var srcExpr = expr.Arguments[0];
            if (srcExpr.NodeType != ExpressionType.Convert)
                throw new NotImplementedException("Expecting a Convert expression inside the call to Convert.ToDouble");
            var cvtExpr = srcExpr as UnaryExpression;

            var result = ExpressionToCPP.InternalGetExpression(cvtExpr.Operand, gc, null, container);

            if (!result.Type.IsNumberType())
            {
                throw new NotImplementedException("Do not know how to convert '" + srcExpr.Type.Name + "' to a double!");
            }

            return result;

        }

        /// <summary>
        /// Convert - should never try to new a static class! :-)
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, CompositionContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
