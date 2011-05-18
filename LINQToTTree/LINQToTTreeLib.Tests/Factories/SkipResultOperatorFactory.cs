using System.Linq.Expressions;
using Microsoft.Pex.Framework;
using Remotion.Linq.Clauses.ResultOperators;

namespace Remotion.Data.Linq.Clauses.ResultOperators
{
    /// <summary>A factory for Remotion.Data.Linq.Clauses.ResultOperators.SkipResultOperator instances</summary>
    public static partial class SkipResultOperatorFactory
    {
        /// <summary>A factory for Remotion.Data.Linq.Clauses.ResultOperators.SkipResultOperator instances</summary>
        [PexFactoryMethod(typeof(SkipResultOperator))]
        public static SkipResultOperator Create(int count)
        {
            SkipResultOperator skipResultOperator = new SkipResultOperator(Expression.Constant(count));
            return skipResultOperator;
        }
    }
}
