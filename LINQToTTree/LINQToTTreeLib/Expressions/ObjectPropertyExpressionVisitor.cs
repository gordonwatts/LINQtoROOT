using Remotion.Linq.Parsing;
using System;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Sometimes you'll find things like "new Tuple<int, int>(5, 10).Item1" in our code.
    /// The job of this translator is to replace the above with "5". Called by
    /// the Translating Expression Visitor to automatically deal with this sort of thing.
    /// </summary>
    class ObjectPropertyExpressionVisitor : ExpressionTreeVisitor
    {
        /// <summary>
        /// Property access. Look to see if the parent type is one of these type of things.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override System.Linq.Expressions.Expression VisitMemberExpression(System.Linq.Expressions.MemberExpression expression)
        {
            //
            // If this is an item reference to a tuple type, and it is on top of a new for the tuple, then
            // extract the new value argument.
            //

            var exprType = expression.Expression.Type;
            if (exprType.Name.StartsWith("Tuple`") && expression.Expression.NodeType == ExpressionType.New)
            {
                int itemIndex = Convert.ToInt32(expression.Member.Name.Substring(4));
                var newExpr = expression.Expression as NewExpression;
                return newExpr.Arguments[itemIndex - 1];
            }

            return base.VisitMemberExpression(expression);
        }
    }
}
