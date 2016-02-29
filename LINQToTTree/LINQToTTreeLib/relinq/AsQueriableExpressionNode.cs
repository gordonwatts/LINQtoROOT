using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.relinq
{
    /// <summary>
    /// Deal with the as queriable converter
    /// </summary>
    class AsQueriableExpressionNode : ResultOperatorExpressionNodeBase
    {
        /// <summary>
        /// List of methods in the re-linq expression tree that can be parsed
        /// by this node parser.
        /// </summary>
        public static MethodInfo[] SupportedMethods = new[]
            {
                TypeUtils.GetSupportedMethod (() => Queryable.AsQueryable<object>((IEnumerable<object>) null))
            };

        /// <summary>
        /// Create the expresson node parser
        /// </summary>
        /// <param name="parseInfo"></param>
        public AsQueriableExpressionNode(MethodCallExpressionParseInfo parseInfo)
            : base(parseInfo, null, null)
        {
        }

        /// <summary>
        /// Create the result operator that will sit in the tree
        /// </summary>
        /// <param name="clauseGenerationContext"></param>
        /// <returns></returns>
        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new AsQueriableResultOperator();
        }

        /// <summary>
        /// We don't modify the sequence - so just pass things on directly.
        /// </summary>
        /// <param name="inputParameter"></param>
        /// <param name="expressionToBeResolved"></param>
        /// <param name="clauseGenerationContext"></param>
        /// <returns></returns>
        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }
    }
}
