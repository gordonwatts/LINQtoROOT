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
            else if (expr.Method.Name == "Where")
            {
                return ProcessWhereCall(expr, out result, gc, context, container);

            }

            ///
            /// They want us to do something that we can't deal with.
            /// 

            throw new NotImplementedException("Sorry, method Enumerable." + expr.Method.Name + " is not translated!");
        }

        /// <summary>
        /// Process the Where method
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        private Expression ProcessWhereCall(MethodCallExpression expr, out IValue result, IGeneratedCode gc, ICodeContext context, CompositionContainer container)
        {
            if (expr.Arguments.Count != 2)
                throw new NotImplementedException("Enumerable.Where is only implemented in the 2 argument flavor");
            if (expr.Arguments[1].Type.GetGenericArguments().Length != 2)
                throw new NotImplementedException("Enumerable.Where is only implemented in the Func<x, bool> flavor");

            ///
            /// We need to loop over our first argument
            /// 

            IScopeInfo oldScope;
            IVariableScopeHolder popMe;
            CodeUpLoop(expr.Arguments[0], gc, context, container, out oldScope, out popMe);

            ///
            /// Next job is to actually code up the if statement.
            /// 

            gc.Add(new Statements.StatementFilter(ExpressionVisitor.GetExpression(expr.Arguments[1], gc, context, container)));

            ///
            /// For a result we return a simple non-looping vector style guy.
            /// 

            result = new ImpliedLoopVariable();
            return expr;
        }

        class ImpliedLoopVariable : ISequenceAccessor, IValue
        {
            public string RawValue
            {
                get { throw new NotImplementedException(); }
            }

            public Type Type
            {
                get { throw new NotImplementedException(); }
            }

            public IVariable AddLoop(IGeneratedCode env, ICodeContext context, string indexName, Action<IVariableScopeHolder> popVariableContext)
            {
                throw new NotImplementedException();
            }
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

            IScopeInfo oldScope;
            IVariableScopeHolder popMe;
            CodeUpLoop(expr.Arguments[0], gc, context, container, out oldScope, out popMe);

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

        /// <summary>
        /// Form a loop around the (presumably) enumerable arg.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <param name="oldScope"></param>
        /// <param name="popMe"></param>
        private static void CodeUpLoop(Expression arg, IGeneratedCode gc, ICodeContext context, CompositionContainer container, out IScopeInfo oldScope, out IVariableScopeHolder popMe)
        {
            oldScope = gc.CurrentScope;

            var looper = ExpressionVisitor.GetExpression(arg, gc, context, container);
            if (looper == null)
                throw new InvalidOperationException("Enumerable.Count needs to have a proper array as its argument");
            var loopArray = looper as ISequenceAccessor;
            if (loopArray == null)
                throw new InvalidOperationException("Enumerable.Count needs a real array as its argument.");

            popMe = null;
            IVariableScopeHolder newPop = null;
            var loopVar = loopArray.AddLoop(gc, context, "innerv", a => newPop = a);
            popMe = newPop;
        }
    }
}
