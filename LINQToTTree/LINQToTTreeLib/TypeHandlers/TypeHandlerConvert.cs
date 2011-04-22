﻿using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;

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
        public IValue ProcessConstantReference(System.Linq.Expressions.ConstantExpression expr, IGeneratedCode codeEnv, ICodeContext context, System.ComponentModel.Composition.Hosting.CompositionContainer container)
        {
            throw new NotImplementedException();
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
        public System.Linq.Expressions.Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedCode gc, ICodeContext context, CompositionContainer container)
        {
            if (expr.Method.Name == "ToDouble")
                return ProcessToDouble(expr, out result, gc, context, container);

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
        private Expression ProcessToDouble(MethodCallExpression expr, out IValue result, IGeneratedCode gc, ICodeContext context, CompositionContainer container)
        {
            var srcExpr = expr.Arguments[0];
            if (srcExpr.NodeType != ExpressionType.Convert)
                throw new NotImplementedException("Expecting a Convert expression inside the call to Convert.ToDouble");
            var cvtExpr = srcExpr as UnaryExpression;

            result = ExpressionToCPP.GetExpression(cvtExpr.Operand, gc, context, container);

            if (
                result.Type != typeof(int)
                && result.Type != typeof(double)
                && result.Type != typeof(float)
                )
            {
                throw new NotImplementedException("Do not know how to convert '" + srcExpr.Type.Name + "' to a double!");
            }

            return expr;
        }
    }
}