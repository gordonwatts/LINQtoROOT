// <copyright file="ConstantExpressionFactory.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>

using Microsoft.Pex.Framework;

namespace System.Linq.Expressions
{
    /// <summary>A factory for System.Linq.Expressions.ConstantExpression instances</summary>
    public static partial class ConstantExpressionFactory
    {
        /// <summary>A factory for System.Linq.Expressions.ConstantExpression instances</summary>
        [PexFactoryMethod(typeof(ConstantExpression))]
        public static ConstantExpression Create(int index)
        {
            if (index == 0)
            {
                return Expression.Constant(10.0);
            }
            else
            {
                return Expression.Constant(new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0));
            }
        }
    }
}
