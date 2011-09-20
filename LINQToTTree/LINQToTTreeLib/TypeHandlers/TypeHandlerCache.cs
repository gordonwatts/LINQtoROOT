using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.TypeHandlers
{
    /// <summary>
    /// This cache helps manage all the known type handlers.
    /// </summary>
    [Export]
    public class TypeHandlerCache
    {
#pragma warning disable 649
        [ImportMany]
        IEnumerable<ITypeHandler> _handlers;
#pragma warning restore 649

        /// <summary>
        /// Process the constant reference
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <returns></returns>
        public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedQueryCode codeEnv, CompositionContainer container)
        {
            // <pex>
            if (expr == (ConstantExpression)null)
                throw new ArgumentNullException("expr");
            // </pex>

            var h = FindHandler(expr.Type);
            return h.ProcessConstantReference(expr, codeEnv, container);
        }

        /// <summary>
        /// Do the early call for processing an expression. If we don't know about the item, then
        /// just return it.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="_codeContext"></param>
        /// <returns></returns>
        public Expression ProcessConstantReferenceAsExpression(ConstantExpression expression, CompositionContainer container)
        {
            if (expression == (ConstantExpression)null)
                throw new ArgumentNullException("expression");

            var h = FindHandler(expression.Type, throwIfNotThere: false);
            if (h == null)
                return expression;
            return h.ProcessConstantReferenceExpression(expression, container);
        }

        /// <summary>
        /// Run the method call against the expressoins we know.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <returns></returns>
        public Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
        {
            // <pex>
            if (expr == (MethodCallExpression)null)
                throw new ArgumentNullException("expr");
            // </pex>

            var h = FindHandler(expr.Method.DeclaringType);
            return h.ProcessMethodCall(expr, out result, gc, context, container);
        }

        /// <summary>
        /// Process a new against an expression - that hopefully we know!
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        internal Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var h = FindHandler(expression.Type);
            return h.ProcessNew(expression, out result, gc, context, container);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find the handler. Acc-vio if we can't!
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ITypeHandler FindHandler(Type type, bool throwIfNotThere = true)
        {
            if (_handlers == null)
                throw new InvalidOperationException("TypeHandlerCache has not been initalized via MEF!");

            var h = (from t in _handlers
                     where t.CanHandle(type)
                     select t).FirstOrDefault();

            if (h == null)
            {
                if (!throwIfNotThere)
                    return null;

                throw new InvalidOperationException("I don't know how to deal with the type " + type.Name);
            }
            return h;
        }
    }
}
