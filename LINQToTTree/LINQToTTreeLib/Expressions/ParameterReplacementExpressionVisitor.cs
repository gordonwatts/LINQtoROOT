using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using Remotion.Data.Linq.Parsing;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Replaces parameter references with known parameters
    /// </summary>
    internal class ParameterReplacementExpressionVisitor : ExpressionTreeVisitor
    {
        private ICodeContext _context;

        /// <summary>
        /// Creat the object and cahce the context for later parameter lookup.
        /// </summary>
        /// <param name="context"></param>
        protected ParameterReplacementExpressionVisitor(ICodeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return a new expression with the parameters replaced.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Expression ReplaceParameters(Expression expr, ICodeContext context)
        {
            var prep = new ParameterReplacementExpressionVisitor(context);
            return prep.VisitExpression(expr);
        }

        /// <summary>
        /// We are looking at some sort of parameter. If there is a replacement, then make it!
        /// </summary>
        /// <param name="paramExpr"></param>
        /// <returns></returns>
        protected override Expression VisitParameterExpression(ParameterExpression paramExpr)
        {
            var replaceit = _context.GetReplacement(paramExpr.Name);
            if (replaceit == null)
                return base.VisitParameterExpression(paramExpr);

            return replaceit;
        }
    }
}
