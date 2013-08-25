using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.Utils;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;

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
                var tr = new ResolveToExpression() { CodeContext = cc, GeneratedCode = gc, MEFContainer = container };
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
                /// See if there are any parameter replacements that can be done out-of-band
                /// 

                var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(expression, CodeContext);

                //
                // Next, attempt to translate the expr (if needed). This deals with moving from
                // one pre-done space to another.
                // 

                string oldExpr = "";
                while (expr.ToString() != oldExpr)
                {
                    oldExpr = expr.ToString();
                    expr = TranslatingExpressionVisitor.Translate(expr, CodeContext.CacheCookies, e => e.Resolve(GeneratedCode, CodeContext, MEFContainer));
                }

                //
                // Now do the rest of the parsing.
                //

                return base.VisitExpression(expr);
            }

            /// <summary>
            /// Keep track of the code context.
            /// </summary>
            public ICodeContext CodeContext;

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
                                          select CodeContext.Add(pair.Item1.Name, pair.Item2);
                var allParamDefineToPopers = paramDefineToPopers.ToArray();

                ///
                /// Do the work. We parse the body of the lambda expression. The references to the parameters should be automatically
                /// dealt with.
                /// 

                var result = lambda.Body.Resolve(GeneratedCode, CodeContext, MEFContainer);

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
            /// When looking at the member expression it is possible that we'll do a sub-query expression (or similar), and that
            /// will yield the first part of an array. It will only make sense for us to look at it when it comes back and is
            /// re-combined. So...
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override Expression VisitMemberExpression(MemberExpression expression)
            {
                var expr = base.VisitMemberExpression(expression);
                if (expr != expression)
                    return expr.Resolve(GeneratedCode, CodeContext, MEFContainer);
                return expr;
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
                //
                // Give the various type handlers a chance to alter the expression call if they want.
                //

                var expr = TypeHandlers.ProcessMethodCall(expression, GeneratedCode, CodeContext, MEFContainer);

                //
                // If it is still an expression call, then we need to allow the arguments to transform.
                //

                if (expr is MethodCallExpression)
                {
                    var mc = expr as MethodCallExpression;

                    var transformedArgs = from a in mc.Arguments
                                          select a.Resolve(GeneratedCode, CodeContext, MEFContainer);

                    expr = Expression.Call(mc.Object, mc.Method, transformedArgs);
                }

                return expr;
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

                QueryVisitor qv = new QueryVisitor(GeneratedCode, CodeContext, MEFContainer);
                MEFContainer.SatisfyImportsOnce(qv);

                ///
                /// Run it - since this result is out of this loop, we pop-back-out when done.
                /// 

                var scope = GeneratedCode.CurrentScope;
                GeneratedCode.SetCurrentScopeAsResultScope();
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

                var r = GeneratedCode.ResultValue;
                if (r != null)
                {
                    GeneratedCode.CurrentScope = scope;
                    if (r is IDeclaredParameter)
                    {
                        GeneratedCode.Add(r as IDeclaredParameter, false);
                    }
                    GeneratedCode.ResetResult();
                    return r;
                }

                //
                // The fact that we returned null means we are dealing with a
                // sequence. There really is no translated version of this expression
                // in that case - so we will return null. If someone above is depending
                // on doing something with it they are going to run into some
                // trouble!

                return null;
            }

            /// <summary>
            /// Look for object compares (like "==" and "!=") between objects. If we see one, then we
            /// parse it out explicitly.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override Expression VisitBinaryExpression(BinaryExpression expression)
            {
                // If this is a binary expression, and it is == or !=, it could be that we have an object compare
                // of the translated objects.
                if (expression.NodeType == ExpressionType.Equal
                    || expression.NodeType == ExpressionType.NotEqual)
                {
                    var b = expression as BinaryExpression;
                    var cmpLeft = Resolve(b.Left, GeneratedCode, CodeContext, MEFContainer);
                    var cmpRight = Resolve(b.Right, GeneratedCode, CodeContext, MEFContainer);
                    var blArray = ExtractObjectArrayName(cmpLeft);
                    var brArray = ExtractObjectArrayName(cmpRight);
                    if (blArray == brArray
                        && blArray != null)
                    {
                        var lindex = CheckNotConst(ExtractObjectArrayIndex(cmpLeft));
                        var rindex = CheckNotConst(ExtractObjectArrayIndex(cmpRight));

                        return Expression.MakeBinary(expression.NodeType, lindex, rindex);
                    }
                    else if (blArray != null && IsNullConstant(b.Right))
                    {
                        var index = CheckNotConst(ExtractObjectArrayIndex(cmpLeft));
                        return Expression.Equal(index, Expression.Constant(-1));
                    }
                    else if (brArray != null && IsNullConstant(cmpLeft))
                    {
                        var index = CheckNotConst(ExtractObjectArrayIndex(cmpRight));
                        return Expression.Equal(Expression.Constant(-1), index);
                    }
                    else
                    {
                        // In this case it isn't a special binary expression, so just build it up as we would expect.
                        return Expression.MakeBinary(expression.NodeType, cmpLeft, cmpRight);
                    }
                }
                else
                {
                    // Everything else we let go as we might otherwise.
                    return base.VisitBinaryExpression(expression);
                }
            }

            /// <summary>
            /// See if this is a constant reference to the "null" keyword.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            private bool IsNullConstant(Expression expression)
            {
                var cexpr = expression as ConstantExpression;
                if (cexpr == null)
                    return false;
                return cexpr.Value == null;
            }

            /// <summary>
            /// If the expression passed in is a const expression, then throw.
            /// </summary>
            /// <param name="lindex"></param>
            /// <returns></returns>
            private Expression CheckNotConst(Expression expr)
            {
                if (expr is ConstantExpression)
                    throw new NotSupportedException(string.Format("A constant expression array reference is not allowed ('{0}').", expr.ToString()));
                return expr;
            }

            /// <summary>
            /// Check to see if a generic expression is a tranlsated array reference.
            /// </summary>
            /// <param name="exp"></param>
            /// <returns></returns>
            private bool GenericExpressionIsRootObjectArrayReference(Expression exp)
            {
                var arrayLookup = exp as BinaryExpression;
                if (arrayLookup == null)
                    return false;

                var me = arrayLookup.Left as MemberExpression;
                if (me == null)
                    return false;

                return me.IsRootObjectArrayReference();
            }

            /// <summary>
            /// This should represent somethign that points into an array list of translated objects. Make sure that is
            /// true, and if it is, attempt to reconstruct the actual name of the array. Return null if it isn't.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            private string ExtractObjectArrayName(Expression expression)
            {
                if (!GenericExpressionIsRootObjectArrayReference(expression))
                    return null;

                var index = expression as BinaryExpression;
                return index.Left.ToString();
            }

            /// <summary>
            /// Given we know this is an index lookup, return the expression used for the array index.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            private Expression ExtractObjectArrayIndex(Expression expression)
            {
                var lookup = expression as BinaryExpression;
                return lookup.Right;
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

            #region Conditional Expression Checking
            /// <summary>
            /// We only support a sub-class of expressions for now - so we'd better make sure we are protected!
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override Expression VisitConditionalExpression(ConditionalExpression expression)
            {
                if (CheckForSubQueries.CheckExpression(expression.IfFalse)
                    || CheckForSubQueries.CheckExpression(expression.IfTrue))
                    throw new NotSupportedException(string.Format("Complex true/false clauses in a conditional expression are not supported: '{0}'", expression.ToString()));

                return base.VisitConditionalExpression(expression);
            }

            /// <summary>
            /// Quick expression traversal object to look for sub-expressions.
            /// </summary>
            private class CheckForSubQueries : ExpressionTreeVisitor
            {
                public static bool CheckExpression(Expression expr)
                {
                    var e = new CheckForSubQueries();
                    e.VisitExpression(expr);
                    return e.SawSubQuery;
                }

                /// <summary>
                /// Get us configured correctly.
                /// </summary>
                private CheckForSubQueries()
                {
                    SawSubQuery = false;
                }

                /// <summary>
                /// Returns true if a sub-query was found.
                /// </summary>
                public bool SawSubQuery { get; set; }

                protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
                {
                    SawSubQuery = true;
                    return expression;
                }
            }

            #endregion
        }
    }
}
