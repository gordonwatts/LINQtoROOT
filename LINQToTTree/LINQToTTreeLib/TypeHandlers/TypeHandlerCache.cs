﻿using System;
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
        public Expression ProcessMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
        {
            if (expr == (MethodCallExpression)null)
                throw new ArgumentNullException("expr");

            var h = FindHandler(expr.Method.DeclaringType);
            return h.ProcessMethodCall(expr, gc, context, container);
        }

        /// <summary>
        /// Run the method call against the expressions we know.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <returns></returns>
        public IValue CodeMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, CompositionContainer container)
        {
            if (expr == (MethodCallExpression)null)
                throw new ArgumentNullException("expr");

            var h = FindHandler(expr.Method.DeclaringType);
            return h.CodeMethodCall(expr, gc, container);
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
        internal Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, CompositionContainer container)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var h = FindHandler(expression.Type);
            return h.ProcessNew(expression, out result, gc, container);
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
                throw new InvalidOperationException("TypeHandlerCache has not been initialized via MEF!");

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

        /// <summary>
        /// Try to do a member reference. Return null if we can't do it.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        internal IValue TryMemberReference(MemberExpression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            var h = FindHandler(expr.Expression.Type, false);
            if (h == null)
                return null;

            return h.ProcessMemberReference(expr, gc, cc, container);
            throw new NotImplementedException();
        }
    }
}
