using LINQToTTreeLib.Utils;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;

namespace LINQToTTreeLib.relinq
{
    class TakeSkipExpressionNode : ResultOperatorExpressionNodeBase
    {
        /// <summary>
        /// List of methods in the re-linq expression tree that can be parsed
        /// by this node parser.
        /// </summary>
        public static MethodInfo[] SupportedMethods = new[]
            {
                TypeUtils.GetSupportedMethod (() => Helpers.TakePerSource<object>((IQueryable<object>) null, (int) 0)),
            };

        /// <summary>
        /// Hold onto how many we want to take or skip.
        /// </summary>
        private Expression _count;

        /// <summary>
        /// Is this thing a skip or take?
        /// </summary>
        private bool _isTake;

        /// <summary>
        /// Create the expression node
        /// </summary>
        /// <param name="parseInfo"></param>
        public TakeSkipExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression count)
            : base(parseInfo, null, null)
        {
            _count = count;
            _isTake = parseInfo.ParsedExpression.Method.Name == "TakePerSource";
        }

        protected override Remotion.Linq.Clauses.ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
            return new TakeSkipResultOperator(_count, _isTake);
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            return Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
        }
    }

    class TakeSkipResultOperator : SequenceTypePreservingResultOperatorBase
    {
        private Expression _count;
        private bool _isTake;

        public TakeSkipResultOperator(Expression _count, bool _isTake)
        {
            this._count = _count;
            this._isTake = _isTake;
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new TakeSkipResultOperator(_count, _isTake);
        }

        public override StreamedSequence ExecuteInMemory<T>(StreamedSequence input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// There is nothing to transform
        /// </summary>
        /// <param name="transformation"></param>
        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
        }

        /// <summary>
        /// Return true if this is a take operator, otherwise it is a skip.
        /// </summary>
        public bool IsTake { get { return _isTake; } }

        /// <summary>
        /// Get the expression for the number of iterations
        /// </summary>
        public Expression Count { get { return _count; } }
    }
}
