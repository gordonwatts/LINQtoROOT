using Microsoft.Pex.Framework;

namespace System.Linq.Expressions
{
    /// <summary>A factory for System.Linq.Expressions.UnaryExpression instances</summary>
    public static partial class UnaryExpressionFactory
    {
        /// <summary>A factory for System.Linq.Expressions.UnaryExpression instances</summary>
        [PexFactoryMethod(typeof(UnaryExpression))]
        public static object Create(int unaryExprType)
        {
            switch (unaryExprType)
            {
                case 0:
                    return Expression.Negate(Expression.Constant(34));

                case 1:
                    return Expression.Not(Expression.Constant(true));

                case 2:
                    return Expression.Convert(Expression.Constant(10), typeof(double));

                default:
                    break;
            }
            return null;
        }
    }
}
