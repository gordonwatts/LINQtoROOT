﻿using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using System.Linq.Expressions;
using LINQToTTreeLib.Expressions;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// Implement the result operator that will deal with the Count predicate.
    /// </summary>
    [Export(typeof(IQVScalarResultOperator))]
    class ROCount : IQVScalarResultOperator
    {
        /// <summary>
        /// Get the proper value here.
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(CountResultOperator);
        }

        /// <summary>
        /// Actually try and process this! The count consisits of a count integer and something to increment it
        /// at its current spot.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="codeEnv"></param>
        /// <returns></returns>
        public IVariable ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            if (gc == null)
                throw new ArgumentNullException("CodeEnv must not be null!");

            var c = resultOperator as CountResultOperator;
            if (c == null)
                throw new ArgumentNullException("resultOperator can only be a CountResultOperator and must not be null");

            //
            // The accumulator where we will store the result.
            //

            var accumulator = new VarInteger();

            //
            // Use the Aggregate infrasturcutre to do the adding. This
            // has the advantage that it will correctly combine with
            // similar statements during query optimization.
            //

            var p = Expression.Parameter(typeof(int), accumulator.VariableName);
            var add = Expression.Add(p, Expression.Constant((int)1));
            var addResolved = ExpressionToCPP.GetExpression(add, gc, cc, container);

            gc.Add(new StatementAggregate(accumulator, addResolved));
            return accumulator;
        }
    }
}
