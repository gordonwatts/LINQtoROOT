using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

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
                //GetSupportedMethod (() => Helpers.PairWiseAll<object>((IQueryable<object>) null, null))
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
            var resolvedTestP1 = Source.Resolve(_test.Parameters[0], _test.Body, clauseGenerationContext);
            var resolvedTest = Source.Resolve(_test.Parameters[1], resolvedTestP1, clauseGenerationContext);

            return new PairWiseAllResultOperator(resolvedTest);
        }

        public override System.Linq.Expressions.Expression Resolve(System.Linq.Expressions.ParameterExpression inputParameter, System.Linq.Expressions.Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }
    }
}
