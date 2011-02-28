using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.ResultOperators;

namespace LINQToTTreeLib.TypeHandlers.Enumerable
{
    /// <summary>
    /// Type handler to deal with converting calls to the Enumerable static functions (like Count, etc.). They can appear in the middle
    /// of a LINQ expression and, as a result, they aren't translated with the same infrastructure. On the other hand, they are translated
    /// by a fiarly simple functional structure.
    /// </summary>
    [Export(typeof(ITypeHandler))]
    class TypeHandlerEnumerable : ITypeHandler
    {
        /// <summary>
        /// We deal only with the enumerable type!
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool CanHandle(Type t)
        {
            if (t == null)
                throw new ArgumentNullException("type must not be null");

            return t == typeof(System.Linq.Enumerable);
        }

        /// <summary>
        /// These guys raen't data that can be passed back and forth - so this is a definate intenral error!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <returns></returns>
        public IValue ProcessConstantReference(System.Linq.Expressions.ConstantExpression expr, IGeneratedCode codeEnv)
        {
            throw new NotImplementedException("It is not possible to pass over Enumerable objects to ROOT!");
        }

        /// <summary>
        /// For each method, ship out and parse it.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedCode gc, ICodeContext context, CompositionContainer container)
        {
            if (expr.Method.Name == "Count")
            {
                return ProcessCountCall(expr, out result, gc, context, container);
            }

            ///
            /// They want us to do something that we can't deal with.
            /// 

            throw new NotImplementedException("Sorry, method Enumerable." + expr.Method.Name + " is not translated!");
        }

        /// <summary>
        /// Deal with the count method
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private Expression ProcessCountCall(MethodCallExpression expr, out IValue result, IGeneratedCode gc, ICodeContext context, CompositionContainer container)
        {
            if (expr.Arguments.Count != 1)
                throw new NotImplementedException("Enuemrable.Count is only implemented in the 1 argument flavor");
            result = null;

            ///
            /// We need to get a loop! To do this, we need to figure out what we are looping over. This will
            /// be whatever we have been handed as an argument.
            /// 

            var oldScope = gc.CurrentScope;

            var looper = ExpressionVisitor.GetExpression(expr.Arguments[0], gc, context, container);
            if (looper == null)
                throw new InvalidOperationException("Enumerable.Count needs to have a proper array as its argument");
            var loopArray = looper as ISequenceAccessor;
            if (loopArray == null)
                throw new InvalidOperationException("Enumerable.Count needs a real array as its argument.");

            IVariableScopeHolder popMe = null;
            var loopVar = loopArray.AddLoop(gc, context, "innerv", a => popMe = a);

            ///
            /// Now that are inside a loop, implement the counting
            /// 

            var r = ROCount.ImplementCount(gc);
            gc.AddOneLevelUp(r);
            result = r;

            ///
            /// no longer need the looping var!
            /// 

            popMe.Pop();
            gc.CurrentScope = oldScope;

            ///
            /// Just return what we were working on...
            /// 

            return expr;
        }
    }
}
