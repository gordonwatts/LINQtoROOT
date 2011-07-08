using System;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;

namespace LINQToTTreeLib.relinq
{
    /// <summary>
    /// The AsQueriable result operator
    /// </summary>
    class AsQueriableResultOperator : SequenceTypePreservingResultOperatorBase
    {
        /// <summary>
        /// Something simple for when we need to build a string from a query model.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "AsQueriable()";
        }

        /// <summary>
        /// We don't execute in memory, so we can blow this off.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public override StreamedSequence ExecuteInMemory<T>(Remotion.Linq.Clauses.StreamedData.StreamedSequence input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Make a clone of this object
        /// </summary>
        /// <param name="cloneContext"></param>
        /// <returns></returns>
        public override ResultOperatorBase Clone(Remotion.Linq.Clauses.CloneContext cloneContext)
        {
            return new AsQueriableResultOperator();
        }

        /// <summary>
        /// We need to translfor the variables we are holding onto. Since we don't have
        /// any, we do nothing!
        /// </summary>
        /// <param name="transformation"></param>
        public override void TransformExpressions(Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> transformation)
        {
        }
    }
}
