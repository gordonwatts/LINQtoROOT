using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.TypeHandlers;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Resolve expressions - parameter replacement, sub-expression resolution, etc. Basically, everything
    /// but the actual work of turning it into C++ code. This will make the C++ expression translator as simple
    /// as pie.
    /// </summary>
    static class ExpressionResolver
    {
        public static Expression Resolve(this Expression source, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            if (cc == null)
            {
                cc = new CodeContext();
            }

            return ResolveToExpression.Translate(source, gc, cc, container);
        }

        /// <summary>
        /// Walk the expression, resolving whatever needs to be resolved.
        /// </summary>
        class ResolveToExpression : ExpressionTreeVisitor
        {
            /// <summary>
            /// Translate an expression.
            /// </summary>
            /// <param name="expr"></param>
            /// <param name="cc"></param>
            /// <returns></returns>
            public static Expression Translate(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
            {
                var tr = new ResolveToExpression() { _codeContext = cc, GeneratedCode = gc, MEFContainer = container };
                if (container != null)
                {
                    container.SatisfyImportsOnce(tr);
                }

                return tr.VisitExpression(expr);
            }

            /// <summary>
            /// Visit an expression. For each expression first make sure that it has been
            /// translated and parameter replaced. We do this recusively...
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public override Expression VisitExpression(Expression expression)
            {
                //
                // Sometimes we are called with a null - we ignore that! :-)
                //

                if (expression == null)
                    return null;

                ///
                /// First, see if there are any parameter replacements that can be done out-of-band
                /// 

                var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(expression, _codeContext);

                //
                // Next, attempt to translate the expr (if needed). This deals with moving from
                // one pre-done space to another.
                // 

                string oldExpr = "";
                while (expr.ToString() != oldExpr)
                {
                    oldExpr = expr.ToString();
                    expr = TranslatingExpressionVisitor.Translate(expr, _codeContext.CacheCookies);
                }

                //
                // Now do the rest of the parsing.
                //

                return base.VisitExpression(expr);
            }

            /// <summary>
            /// Keep track of the code context.
            /// </summary>
            public ICodeContext _codeContext;

            /// <summary>
            /// The code we are generating.
            /// </summary>
            public IGeneratedQueryCode GeneratedCode { get; set; }

            /// <summary>
            /// We are doing an inline call to a lambda expression.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override Expression VisitInvocationExpression(InvocationExpression expression)
            {
                ///
                /// Declare all the parameters for lookup.
                /// 

                if (!(expression.Expression is LambdaExpression))
                    throw new NotImplementedException("Do not know how to invoke a non-lambda call like '" + expression.ToString() + "'");

                var lambda = expression.Expression as LambdaExpression;

                var paramArgs = lambda.Parameters.Zip(expression.Arguments, (p, a) => Tuple.Create(p, a));
                var paramDefineToPopers = from pair in paramArgs
                                          select _codeContext.Add(pair.Item1.Name, pair.Item2);
                var allParamDefineToPopers = paramDefineToPopers.ToArray();

                ///
                /// Do the work. We parse the body of the lambda expression. The references to the parameters should be automatically
                /// dealt with.
                /// 

                var result = lambda.Body.Resolve(GeneratedCode, _codeContext, MEFContainer);

                ///
                /// Now, pop everything off!
                /// 

                foreach (var param in allParamDefineToPopers)
                {
                    param.Pop();
                }

                ///
                /// Done!
                /// 

                return result;
            }

            /// <summary>
            /// There are certian expressions we really don't want to delve into - in
            /// particular an array length operator!
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override Expression VisitUnaryExpression(UnaryExpression expression)
            {
                switch (expression.NodeType)
                {
                    // Skip this b/c the array length can require a
                    // name translation. And by the time we get here, it
                    // will have been fully translated.
                    //case ExpressionType.ArrayLength:
                    //    return expression;

                    default:
                        return base.VisitUnaryExpression(expression);
                }
            }

            /// <summary>
            /// Some method is being called. Offer plug-ins a chance to transform this method call.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
            {
                return TypeHandlers.ProcessMethodCall(expression, GeneratedCode, _codeContext, MEFContainer);
            }

            /// <summary>
            /// List of the type handlers that we can use to process things.
            /// </summary>
            [Import]
            private TypeHandlerCache TypeHandlers { get; set; }


            /// <summary>
            /// Get/Set the MEF container used when we create new objects (like a QV).
            /// </summary>
            public CompositionContainer MEFContainer { get; set; }

            /// <summary>
            /// Process a constant expression, if possible. Some forms of constant expressions
            /// are actually just stubs for dealing with something that needs to be injected into
            /// the process.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override Expression VisitConstantExpression(ConstantExpression expression)
            {
                if (TypeHandlers == null)
                    throw new InvalidOperationException("Can't visit a constant expression unless the typehandlers have been initalized");
                return TypeHandlers.ProcessConstantReferenceAsExpression(expression, MEFContainer);
            }

            /// <summary>
            /// Run a sub-query for the user. The sub-query is a full-blown expression that
            /// must usually run in its own loop (or similar).
            /// </summary>
            /// <remarks>
            /// The query visitor must be re-invoked - this ends up being a full-blown
            /// parsing.
            /// </remarks>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
            {
                if (MEFContainer == null)
                    throw new InvalidOperationException("MEFContainer can't be null if we need to analyze a sub query!");

                QueryVisitor qv = new QueryVisitor(GeneratedCode, _codeContext, MEFContainer);
                qv.SubExpressionParse = true;
                MEFContainer.SatisfyImportsOnce(qv);

                ///
                /// Run it - since this result is out of this loop, we pop-back-out when done.
                /// 

                var scope = GeneratedCode.CurrentScope;
                qv.VisitQueryModel(expression.QueryModel);

                ///
                /// Two possible results from the sub-expression query, and how we proceed depends
                /// on what happened in the sub query
                /// 
                /// 1. <returns a value> - an operator like Count() comes back from the sequence.
                ///    it will get used in some later sequence (like # of jets in each event). So,
                ///    we need to make sure it is declared and kept before it is used. The # that comes
                ///    back needs to be used outside the scope we are sitting in - the one that we were at
                ///    when we started this. Since this is a sub-query expression, the result isn't the final
                ///    result, so we need to reset it so no one notices it.
                /// 2. <return a sequence> - this is weird - What we are actually doing here is putting the
                ///    sequence into code. So the loop variable has been updated with the new sequence iterator
                ///    value. But there isn't really a result! So the result will be null...
                /// 

                if (GeneratedCode.ResultValue != null)
                {
                    GeneratedCode.CurrentScope = scope;
                    //GeneratedCode.Add(GeneratedCode.ResultValue);
                    var r = GeneratedCode.ResultValue;
                    GeneratedCode.ResetResult();
                    return r;
                }

                return expression;
            }
        }
    }
}
