using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;

namespace LINQToTTreeLib.relinq
{
    /// <summary>
    /// re-linq class to help re-linq parse our UniqueCombinations operator. The template for this class
    /// is taken from fabian's blog entry: https://www.re-motion.org/blogs/mix/2010/10/28/re-linq-extensibility-custom-query-operators
    /// as well as looking at re-linq's source code. This represents the information in the re-linq query model.
    /// </summary>
    class UniqueCombinationsResultOperator : ResultOperatorBase
    {
        /// <summary>
        /// Make a clone of ourselves. We hold no extra information, so this is pretty easy.
        /// </summary>
        /// <param name="cloneContext"></param>
        /// <returns></returns>
        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new UniqueCombinationsResultOperator();
        }

        /// <summary>
        /// Used 
        /// </summary>
        /// <param name="inputInfo"></param>
        /// <returns></returns>
        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
#if false
            //
            // Build up the tuple type
            // 

            var seqInfo = inputInfo as StreamedSequenceInfo;
            if (seqInfo == null)
                throw new ArgumentException("Input info is not of type StreamSequenceInfo");
            var seqType = seqInfo.ItemExpression.Type;

            var tupleType = typeof(Tuple<>).MakeGenericType(seqType, seqType);

            //
            // Return the stream info
            //

            return new StreamedSequenceInfo(typeof(IQueryable<>).MakeGenericType(tupleType), seqInfo.ItemExpression);
#endif
            throw new NotImplementedException();
        }

        /// <summary>
        /// Transform our parameters by whatever transformation is being requested. No worries, since we
        /// have no parameters!
        /// </summary>
        /// <param name="transformation"></param>
        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
        }

        /// <summary>
        /// Return a debugging string representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "UniqueCombinations()";
        }

        /// <summary>
        /// We do not allow executing in memory this operator.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override IStreamedData ExecuteInMemory(IStreamedData input)
        {
            throw new NotImplementedException();
        }
    }
}
