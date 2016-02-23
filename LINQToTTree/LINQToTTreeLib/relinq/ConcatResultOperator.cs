using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;
using System.Linq.Expressions;

namespace LINQToTTreeLib.relinq
{
    /// <summary>
    /// The concat result operator.
    /// </summary>
    class ConcatResultOperator : SequenceTypePreservingResultOperatorBase
    {
        public Expression Source2;

        public ConcatResultOperator(Expression _source2)
        {
            this.Source2 = _source2;
        }

        /// <summary>
        /// Create a copy of ourselves.
        /// </summary>
        /// <param name="cloneContext"></param>
        /// <returns></returns>
        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new ConcatResultOperator(Source2);
        }

        public override StreamedSequence ExecuteInMemory<T>(StreamedSequence input)
        {
            throw new NotImplementedException();
        }

        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
            Source2 = transformation(Source2);
        }
    }
}
