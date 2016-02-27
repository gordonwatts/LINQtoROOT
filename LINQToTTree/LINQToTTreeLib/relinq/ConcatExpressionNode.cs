using Remotion.Linq.Parsing.Structure.IntermediateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using System.Linq.Expressions;
using System.Reflection;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.relinq
{
    /// <summary>
    /// Expression node for the Concat result operator.
    /// </summary>
    class ConcatExpressionNode : ResultOperatorExpressionNodeBase
    {
        /// <summary>
        /// We support the one concat operation.
        /// </summary>
        /// <remarks>
        /// Note that all the object types have to be the same.
        /// Our infrastrucutre doesn't support it, but Concat's signature should enforce it. So even though this
        /// does look weird, it should still work.
        /// </remarks>
        public static MethodInfo[] SupportedMethods = new[]
            {
                TypeUtils.GetSupportedMethod (() => System.Linq.Queryable.Concat<object>((IQueryable<object>) null, (IQueryable<object>) null)),
            };

        private Expression _source2;

        /// <summary>
        /// This is called with the argument to Concat.
        /// </summary>
        /// <param name="parseInfo"></param>
        /// <param name="source2"></param>
        public ConcatExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression source2)
            : base(parseInfo, null, null)
        {
            _source2 = source2;
        }

        /// <summary>
        /// Create the result operator that matches this concat result operator.
        /// </summary>
        /// <param name="clauseGenerationContext"></param>
        /// <returns></returns>
        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new ConcatResultOperator(_source2);
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            throw new NotImplementedException();
        }
    }
}
