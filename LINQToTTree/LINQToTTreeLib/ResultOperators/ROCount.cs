using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

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
        public Expression ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            if (gc == null)
                throw new ArgumentNullException("CodeEnv must not be null!");

            var c = resultOperator as CountResultOperator;
            if (c == null)
                throw new ArgumentNullException("resultOperator can only be a CountResultOperator and must not be null");

            //
            // The accumulator where we will store the result.
            //

            var accumulator = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            accumulator.SetInitialValue("0");

            //
            // Use the Aggregate infrasturcutre to do the adding. This
            // has the advantage that it will correctly combine with
            // similar statements during query optimization.
            //

            var add = Expression.Add(accumulator, Expression.Constant((int)1));
            var addResolved = ExpressionToCPP.GetExpression(add, gc, cc, container);

            gc.Add(new StatementAggregate(accumulator, addResolved));
            return accumulator;
        }

        /// <summary>
        /// Try to do a fast count. Basically, what we are dealing with here is the fact that we have
        /// a simple array, we need only take its length, and return that.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Tuple<bool, Expression> ProcessIdentityQuery(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, CompositionContainer container)
        {
            //
            // We just need to return a length expression. We are low enough level we need to do some basic resolution.
            //

            if (!queryModel.MainFromClause.FromExpression.Type.IsArray)
                return Tuple.Create(false, null as Expression);

            var lengthExpr = Expression.ArrayLength(queryModel.MainFromClause.FromExpression).Resolve(_codeEnv, _codeContext, container);
            return Tuple.Create(true, lengthExpr as Expression);
        }
    }
}
