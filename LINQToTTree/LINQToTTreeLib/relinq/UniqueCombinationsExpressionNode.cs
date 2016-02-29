using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.relinq
{
    /// <summary>
    /// expression node parser used by re-linq to parse the UniqueCombinations operator.
    /// </summary>
    class UniqueCombinationsExpressionNode : ResultOperatorExpressionNodeBase
    {
        /// <summary>
        /// List of methods in the re-linq expression tree that can be parsed
        /// by this node parser.
        /// </summary>
        public static MethodInfo[] SupportedMethods = new[]
            {
                TypeUtils.GetSupportedMethod (() => Helpers.UniqueCombinations<object>((IEnumerable<object>) null)),
                TypeUtils.GetSupportedMethod (() => Helpers.UniqueCombinations<object>((IQueryable<object>) null))
            };

        public UniqueCombinationsExpressionNode(MethodCallExpressionParseInfo parseInfo)
            : base(parseInfo, null, null)
        {
        }

        /// <summary>
        /// Now that re-linq knows what this is, we need to create the data that will sit in the QueryModel
        /// tree. This is the data that will eventually be passed to the ROUniqueCombinations operator
        /// in our re-linq translation infrastructure.
        /// </summary>
        /// <param name="clauseGenerationContext"></param>
        /// <returns></returns>
        protected override Remotion.Linq.Clauses.ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            // No parameters to resolve, so off we go!
            return new UniqueCombinationsResultOperator();
        }

        /// <summary>
        /// We are streaming our input data to our output - without changes, though we do duplicate it.
        /// So resolution is mearly just passing things along.
        /// </summary>
        /// <param name="inputParameter"></param>
        /// <param name="expressionToBeResolved"></param>
        /// <param name="clauseGenerationContext"></param>
        /// <returns></returns>
        public override System.Linq.Expressions.Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }
    }
}
