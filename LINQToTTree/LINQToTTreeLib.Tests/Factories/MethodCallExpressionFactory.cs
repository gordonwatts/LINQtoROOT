using Microsoft.Pex.Framework;

namespace System.Linq.Expressions
{
    /// <summary>A factory for System.Linq.Expressions.MethodCallExpression instances</summary>
    public static partial class MethodCallExpressionFactory
    {
        /// <summary>A factory for System.Linq.Expressions.MethodCallExpression instances</summary>
        [PexFactoryMethod(typeof(MethodCallExpression))]
        public static MethodCallExpression Create()
        {
            var tostring = typeof(int).GetMethods().Where(m => m.Name == "ToString").First();
            var me = Expression.Call(Expression.Constant(10), tostring);
            return me;
        }
    }
}
