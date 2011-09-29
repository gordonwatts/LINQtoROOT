using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using System.Linq;

namespace LINQToTTreeLib.relinq
{
    class PairWiseAllExpressionNode : ResultOperatorExpressionNodeBase
    {
        /// <summary>
        /// List of methods in the re-linq expression tree that can be parsed
        /// by this node parser.
        /// </summary>
        public static MethodInfo[] SupportedMethods = new[]
            {
                GetSupportedMethod (() => Helpers.PairWiseAll<object>((IEnumerable<object>) null, null)),
                GetSupportedMethod (() => Helpers.PairWiseAll<object>((IQueryable<object>) null, null))
            };

        private readonly LambdaExpression _test;

        /// <summary>
        /// Create the expression node
        /// </summary>
        /// <param name="parseInfo"></param>
        public PairWiseAllExpressionNode(MethodCallExpressionParseInfo parseInfo, LambdaExpression test)
            : base(parseInfo, null, null)
        {
            _test = test;
        }

        protected override Remotion.Linq.Clauses.ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new PairWiseAllResultOperator(_test);
        }

        public override System.Linq.Expressions.Expression Resolve(System.Linq.Expressions.ParameterExpression inputParameter, System.Linq.Expressions.Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }
    }
}
