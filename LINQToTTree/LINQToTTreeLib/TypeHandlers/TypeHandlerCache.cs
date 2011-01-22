using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
        public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedCode codeEnv)
        {
            // <pex>
            if (expr == (ConstantExpression)null)
                throw new ArgumentNullException("expr");
            // </pex>

            var h = FindHandler(expr.Type);
            return h.ProcessConstantReference(expr, codeEnv);
        }

        /// <summary>
        /// Run the method call against the expressoins we know.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <returns></returns>
        public Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedCode gc, ICodeContext context)
        {
            // <pex>
            if (expr == (MethodCallExpression)null)
                throw new ArgumentNullException("expr");
            // </pex>

            var h = FindHandler(expr.Method.DeclaringType);
            return h.ProcessMethodCall(expr, out result, gc, context);
        }

        /// <summary>
        /// Find the handler. Acc-vio if we can't!
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ITypeHandler FindHandler(Type type)
        {
            if (_handlers == null)
                throw new InvalidOperationException("TypeHandlerCache has not been initalized via MEF!");

            var h = (from t in _handlers
                     where t.CanHandle(type)
                     select t).First();
            return h;
        }
    }
}
