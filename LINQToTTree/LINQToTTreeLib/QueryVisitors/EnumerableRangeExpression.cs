using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace LINQToTTreeLib.QueryVisitors
{
    /// <summary>
    /// Expression to hold onto info about a Enumerable.Range so it isn't incorrectly
    /// parsed by the code.
    /// </summary>
    class EnumerableRangeExpression : ExtensionExpression
    {
        public const ExpressionType ExpressionType = (ExpressionType)110003;

        /// <summary>
        /// Get the low boundary expression.
        /// </summary>
        public Expression LowBoundary { get; private set; }

        /// <summary>
        /// Get the high boundary expression
        /// </summary>
        public Expression HighBoundary { get; private set; }

        /// <summary>
        /// Create a new expression object, and hold onto the low and high
        /// expressions.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        public EnumerableRangeExpression(Expression low, Expression high)
            : base(typeof(IEnumerable<int>), ExpressionType)
        {
            LowBoundary = low;
            HighBoundary = high;
        }

        /// <summary>
        /// Loop in and make sure the sub-expressions are correctly "visited" - mainly the
        /// low and high guys.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected override Expression VisitChildren(ExpressionTreeVisitor visitor)
        {
            LowBoundary = visitor.VisitExpression(LowBoundary);
            HighBoundary = visitor.VisitExpression(HighBoundary);
            return this;
        }
    }
}
