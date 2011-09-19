using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
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
        public static Expression Resolve(this Expression source, ICodeContext cc)
        {
            if (cc == null)
            {
                cc = new CodeContext();
            }

            ///
            /// First, see if there are any parameter replacements that can be done out-of-band
            /// 

            var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(source, cc);

            //
            // Next, attempt to translate the expr (if needed). This deals with moving from
            // one pre-done space to another.
            // 

            string oldExpr = "";
            while (expr.ToString() != oldExpr)
            {
                oldExpr = expr.ToString();
                expr = TranslatingExpressionVisitor.Translate(expr, cc.CacheCookies);
            }

            //
            // Finally, if there any calls or other things that need to be recurively translated,
            // do that now.
            //

            expr = ResolveToExpression.Translate(expr, cc);

            //
            // Done. Return it!
            //

            return expr;
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
            public static Expression Translate(Expression expr, ICodeContext cc)
            {
                var tr = new ResolveToExpression() { _codeContext = cc };
                return tr.VisitExpression(expr);
            }

            public ICodeContext _codeContext;

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

                var result = lambda.Body.Resolve(_codeContext);

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

        }
    }
}
