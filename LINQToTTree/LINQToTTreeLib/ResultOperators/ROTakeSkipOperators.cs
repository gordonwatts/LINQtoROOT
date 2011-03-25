﻿using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// Deal with the take and the skip operators in an expression
    /// </summary>
    [Export(typeof(IQVCollectionResultOperator))]
    class ROTakeSkipOperators : IQVCollectionResultOperator
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
        public void ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedCode codeEnv, ICodeContext codeContext, CompositionContainer container)
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

            if (codeEnv.Depth <= 1)
            {
                throw new InvalidOperationException("Unable to use the Take or Skip operators at the ntuple level - need to use them only on objects inside the ntuple");
            }

            ///
            /// Now, we create a count variable and that is how we will tell if we are still skipping or
            /// taking. It must be declared in the current block, before our current code! :-)
            /// 

            var counter = new VarInteger();
            codeEnv.AddOneLevelUp(counter);

            codeEnv.Add(new StatementIncrementInteger(counter));
            var comparison = StatementIfOnCount.ComparisonOperator.LessThanEqual;
            IValue comparisonValue = null;
            if (skip != null)
            {
                comparison = StatementIfOnCount.ComparisonOperator.GreaterThan;
                comparisonValue = ExpressionVisitor.GetExpression(skip.Count, codeEnv, codeContext, container);
            }
            else
            {
                comparisonValue = ExpressionVisitor.GetExpression(take.Count, codeEnv, codeContext, container);
            }

            codeEnv.Add(new StatementIfOnCount(counter, comparisonValue, comparison));

            ///
            /// We are particlarly fortunate here. We don't have to update the Loop variable - whatever it is is
            /// still the right one! Normally we'd have to futz with the LoopVariable in code context because we
            /// were iterating over something new. :-) Easy peasy.
            /// 
        }
    }
}
