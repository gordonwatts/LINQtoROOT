using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.relinq
{
    class PairWiseAllResultOperator : SequenceTypePreservingResultOperatorBase
    {
        public Expression Test { get; private set; }

        public PairWiseAllResultOperator(Expression mustSeeTest)
        {
            Test = mustSeeTest;
        }

        /// <summary>
        /// For debugging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "PairWiseAll (" + Test.ToString() + ")";
        }

        public override Remotion.Linq.Clauses.ResultOperatorBase Clone(Remotion.Linq.Clauses.CloneContext cloneContext)
        {
            return new PairWiseAllResultOperator(Test);
        }

        public override Remotion.Linq.Clauses.StreamedData.StreamedSequence ExecuteInMemory<T>(Remotion.Linq.Clauses.StreamedData.StreamedSequence input)
        {
            throw new NotImplementedException();
        }


        public override void TransformExpressions(Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> transformation)
        {
            Test = transformation(Test);
        }
    }
}
