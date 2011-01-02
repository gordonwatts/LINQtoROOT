using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using LinqToTTreeInterfacesLib;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib.Statements;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// Deal with the take and the skip operators in an expression
    /// </summary>
    [Export(typeof(IQVResultOperator))]
    class ROTakeSkipOperators : IQVResultOperator
    {
        /// <summary>
        /// We can handle either the take or the skipping of items! :-)
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(TakeResultOperator)
                || resultOperatorType == typeof(SkipResultOperator);
        }

        /// <summary>
        /// Implement the skipping. We have a main limitation: we currently know only how to implement integer skipping.
        /// We implement with "if" statements to support composability, even if it means running longer in the end...
        /// We actually return nothing when goes - we aren't really a final result the way "Count" is.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <returns></returns>
        public IVariable ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedCode codeEnv)
        {
            ///
            /// Quick checks to make sure
            /// 

            if (codeEnv == null)
                throw new ArgumentNullException("codeEnv cannot be null");

            var take = resultOperator as TakeResultOperator;
            var skip = resultOperator as SkipResultOperator;

            if (take == null && skip == null)
            {
                throw new ArgumentNullException("resultOperator must not be null and must represent either a take or a skip operation!");
            }

            if (take != null && take.Count.Type != typeof(int))
                throw new ArgumentException("Take operator count must be an integer!");
            if (skip != null && skip.Count.Type != typeof(int))
                throw new ArgumentException("Skip operator count must be an integer!");

            ///
            /// Now, we create a count variable and that is how we will tell if we are still skipping or
            /// taking. It must be declared in the current block, before our current code! :-)
            /// 

            var counter = new VarInteger();
            codeEnv.Add(counter);

            codeEnv.Add(new StatementIncrementInteger(counter));
            var comparison = StatementIfOnCount.ComparisonOperator.LessThanEqual;
            IValue comparisonValue = null;
            if (skip != null)
            {
                comparison = StatementIfOnCount.ComparisonOperator.GreaterThan;
                comparisonValue = ExpressionVisitor.GetExpression(skip.Count, codeEnv);
            }
            else
            {
                comparisonValue = ExpressionVisitor.GetExpression(take.Count, codeEnv);
            }

            codeEnv.Add(new StatementIfOnCount(counter, comparisonValue, comparison));

            return null;
        }
    }
}
