using System.Linq.Expressions;
using Microsoft.Pex.Framework;
using Remotion.Linq.Clauses.ResultOperators;

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
