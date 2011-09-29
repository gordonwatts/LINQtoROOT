using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.relinq;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// Deal with the UniqueCombinations operator - which takes all possible pairs where j1 != j2, and order doesn't
    /// matter (i.e. if it returns (j1,j2) then it shouldn't return (j2, j1)).
    /// </summary>
    [Export(typeof(IQVCollectionResultOperator))]
    class ROUniqueCombinations : IQVCollectionResultOperator
    {
        /// <summary>
        /// There is, really, only one guy that we deal with! Thankgoodness it is pretty simple! :-)
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(UniqueCombinationsResultOperator);
        }

        /// <summary>
        /// Take the incoming stream of items, and send them along! :-)
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        public void ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            //
            // Some basic checks on the input.
            //

            if (cc == null)
                throw new ArgumentNullException("cc");
            if (gc == null)
                throw new ArgumentNullException("gc");
            if (cc.LoopVariable == null)
                throw new ArgumentNullException("No defined loop variable!");

            //
            // Get the indexer that is being used to access things. We will just push that onto a temp vector of int's. That will be
            // a list of the items that we want to come back and look at. That said, once done we need to pop-up one level in our
            // depth.
            //

            var arrayRecord = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            gc.AddOutsideLoop(arrayRecord);

            var recordIndexStatement = new Statements.StatementRecordIndicies(ExpressionToCPP.GetExpression(cc.LoopIndexVariable, gc, cc, container), arrayRecord);
            gc.Add(recordIndexStatement);

            gc.Pop();

            //
            // Now, we go down one loop and run over the pairs with a special loop.
            //

            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var indexIterator = new Statements.StatementPairLoop(arrayRecord, index1, index2);
            gc.Add(indexIterator);

            //
            // Finally, build the resulting loop variable. For now it is just a tuple, which is basically the formed expression we started with,
            // but with the other index properties. Other bits will have to do the translation for us. :-)
            //

            var item1 = cc.LoopVariable.ReplaceSubExpression(cc.LoopIndexVariable, index1);
            var item2 = cc.LoopVariable.ReplaceSubExpression(cc.LoopIndexVariable, index2);

            var tupleType = typeof(Tuple<,>).MakeGenericType(cc.LoopVariable.Type, cc.LoopVariable.Type);
            var newTuple = Expression.New(tupleType.GetConstructor(new Type[] { cc.LoopVariable.Type, cc.LoopVariable.Type }), item1, item2);
            cc.SetLoopVariable(newTuple, null);
        }
    }
}
