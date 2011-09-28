using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.relinq;
using LINQToTTreeLib.Statements;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// Implement the pair-wise all operator in code
    /// </summary>
    [Export(typeof(IQVCollectionResultOperator))]
    class ROPairWiseAll : IQVCollectionResultOperator
    {
        /// <summary>
        /// We can handle only the pair-wise all operator.
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(PairWiseAllResultOperator);
        }

        /// <summary>
        /// Add the code to do the pair-wise loop.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        public void ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            var ro = resultOperator as PairWiseAllResultOperator;
            if (ro == null)
                throw new ArgumentNullException("Result operator is not of PairWiseAll type");

            //
            // First, record all the good indicies for this array
            // 

            var arrayRecord = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            gc.AddOneLevelUp(arrayRecord);

            var recordIndexStatement = new StatementRecordIndicies(ExpressionToCPP.GetExpression(cc.LoopIndexVariable, gc, cc, container), arrayRecord);
            gc.Add(recordIndexStatement);

            gc.Pop();

            ///
            /// Next, we create a loop that will mark all the guys as "good" that
            /// the pair-wise function. Hopefully the statement below will be efficient and
            /// not double-try anything! The lambda we've been passed we have to evaluate - twice -
            /// for each, and pass it as a "test" to the statement. It will be some horrendus expression
            /// I suppose!
            /// 

            var passAll = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(bool));
            gc.Add(passAll);
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var index1Lookup = cc.LoopVariable.ReplaceSubExpression(cc.LoopIndexVariable, index1); //Expression.ArrayIndex(array, index1);
            var index2Lookup = cc.LoopVariable.ReplaceSubExpression(cc.LoopIndexVariable, index2);//Expression.ArrayIndex(array, index2);

            var callLambda = Expression.Invoke(ro.Test,
                index1Lookup,
                index2Lookup
                );

            var xcheck = new Statements.StatementCheckLoopPairwise(arrayRecord,
                index1, index2, passAll);
            gc.Add(xcheck);
            var test = new Statements.StatementTestLoopPairwise(
                passAll,
                ExpressionToCPP.GetExpression(callLambda, gc, cc, container));
            gc.Add(test);
            gc.Pop();

            //
            // Ok, the result of that will be the array we have here is now filled with the
            // "proper" stuff. That is - we have "true" in everthing that is good. So we will
            // now just loop over that and apply the index as needed.
            //

            var goodIndex = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            gc.Add(goodIndex);
            var loopOverGood = new Statements.StatementLoopOverGood(arrayRecord, passAll, goodIndex);
            gc.Add(loopOverGood);

            var pindex = Expression.Parameter(typeof(int), goodIndex.RawValue);
            cc.SetLoopVariable(cc.LoopVariable.ReplaceSubExpression(cc.LoopIndexVariable, pindex), pindex);
        }
    }
}
