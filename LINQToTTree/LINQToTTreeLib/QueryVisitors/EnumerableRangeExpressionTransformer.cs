using System.Linq.Expressions;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;

namespace LINQToTTreeLib.QueryVisitors
{
    /// <summary>
    /// Watch the parsing for a Enumerable.Range. When it is spotted, convert it to a particular
    /// special expression that won't be looked at by re-linq. The reason is that the huristics
    /// that re-linq uses means it will dig inside the expression when we really don't want it
    /// to.
    /// </summary>
    class EnumerableRangeExpressionTransformer : IExpressionTransformer<MethodCallExpression>
    {
        public ExpressionType[] SupportedExpressionTypes
        {
            get { return new ExpressionType[] { ExpressionType.Call }; }
        }

        public Expression Transform(MethodCallExpression expression)
        {
            if (expression.Object == null && expression.Method.Name == "Range" && expression.Arguments.Count == 2)
            {
                if (expression.Method.DeclaringType == typeof(System.Linq.Enumerable))
                {
                    return new EnumerableRangeExpression(expression.Arguments[0], expression.Arguments[1]);
                }
            }
            return expression;
        }
    }
}
