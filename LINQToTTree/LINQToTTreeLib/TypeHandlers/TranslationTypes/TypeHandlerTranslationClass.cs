using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;

namespace LINQToTTreeLib.TypeHandlers.TranslationTypes
{
    /// <summary>
    /// Deal with a translation object - it holds onto an expression.
    /// </summary>
    [Export(typeof(ITypeHandler))]
    class TypeHandlerTranslationClass : ITypeHandler
    {
        /// <summary>
        /// Any class that has the translation tag, we deal with it.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool CanHandle(Type t)
        {
            return t.GetInterface("IExpressionHolder") != null;
        }

        /// <summary>
        /// This is how we store the original expression.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <returns></returns>
        public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedQueryCode codeEnv, ICodeContext context, CompositionContainer container)
        {
            var holder = expr.Value as IExpressionHolder;
            if (holder == null)
                throw new InvalidOperationException("Can't get at the interface to get at the expression.");

            var held = holder.HeldExpression;

            return ExpressionToCPP.GetExpression(held, codeEnv, context, container);
        }

        /// <summary>
        /// We should never have a method call made on this guy
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public System.Linq.Expressions.Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The IExpressionHolder should never be pushed over - so bomb!
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
