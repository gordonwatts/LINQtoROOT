using System;
using Microsoft.Pex.Framework;
using Remotion.Data.Linq.Clauses.ResultOperators;
using System.Linq.Expressions;

namespace Remotion.Data.Linq.Clauses.ResultOperators
{
    /// <summary>A factory for Remotion.Data.Linq.Clauses.ResultOperators.TakeResultOperator instances</summary>
    public static partial class TakeResultOperatorFactory
    {
        /// <summary>A factory for Remotion.Data.Linq.Clauses.ResultOperators.TakeResultOperator instances</summary>
        [PexFactoryMethod(typeof(TakeResultOperator))]
        public static TakeResultOperator Create(int count_expression)
        {
            TakeResultOperator takeResultOperator = new TakeResultOperator(Expression.Constant(count_expression));
            return takeResultOperator;
        }
    }
}
