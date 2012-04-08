using System.Linq.Expressions;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;

namespace LINQToTTreeLib.QueryVisitors
{
    /// <summary>
    /// If we spot a Create.Tuple(arg1, arg2, ...) go by, change it into a new tuple<t1, t2, t3>(arg1, arg2, ....). The latter
    /// our code knows how to translate.
    /// </summary>
    public class CreateTupleExpressionTransformer : IExpressionTransformer<MethodCallExpression>
    {
        /// <summary>
        /// Return the id's of the expressions we support - only call us with these guys!
        /// </summary>
        public System.Linq.Expressions.ExpressionType[] SupportedExpressionTypes
        {
            get { return new ExpressionType[] { ExpressionType.Call }; }
        }

        /// <summary>
        /// If this is a Tuple.Create call, then do the transformation.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public System.Linq.Expressions.Expression Transform(MethodCallExpression expression)
        {
            // Fail as quickly as possible if this isn't interesting to us
            if (expression.Object != null || expression.Method.Name != "Create")
                return expression;

            // Make sure the type is a tuple type
            var t = expression.Type;
            if (!t.IsGenericType || t.Name != "Tuple`2")
                return expression;

            // Ok, just move it over into a new object expression.

            var ct = t.GetConstructors()[0];
            var ne = Expression.New(ct, expression.Arguments);

            return ne;
        }
    }
}
