using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;

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

            ///
            /// Next, attempt to translate the expr (if needed)
            /// 

            string oldExpr = "";
            while (expr.ToString() != oldExpr)
            {
                oldExpr = expr.ToString();
                expr = TranslatingExpressionVisitor.Translate(expr, cc.CacheCookies);
            }

            //
            // Done. Return it!
            //

            return expr;
        }
    }
}
