using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// The user wants either the first or the last item in this list. Also, if we have to have a value, then throw if we don't!
    /// </summary>
    [Export(typeof(IQVScalarResultOperator))]
    public class ROFirstLast : IQVScalarResultOperator
    {
        /// <summary>
        /// We support the first/last guys...
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(FirstResultOperator)
                || resultOperatorType == typeof(LastResultOperator);

        }

        /// <summary>
        /// Process the First/last. This means adding a pointer (or not if we are looking at a plane type) and
        /// then filling it till it is full or filling it till the loop is done. Bomb out if we are asked to at the end!!
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <returns></returns>
        public Expression ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel,
            IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            ///
            /// First, do data normalization
            /// 

            var asFirst = resultOperator as FirstResultOperator;
            var asLast = resultOperator as LastResultOperator;

            if (asFirst == null && asLast == null)
            {
                throw new ArgumentNullException("First/Last operator must be either first or last, and not null!");
            }

            bool isFirst = asFirst != null;
            bool bombIfNothing = true;
            if (isFirst)
            {
                bombIfNothing = !asFirst.ReturnDefaultWhenEmpty;
            }
            else
            {
                bombIfNothing = !asLast.ReturnDefaultWhenEmpty;
            }

            //
            // Next, make sure we are looping over something. This had better be an array we are looking at!
            //

            if (cc.LoopIndexVariable == null)
            {
                throw new InvalidOperationException(string.Format("Can't apply First operator when we aren't looping over some well formed array '{0}'", cc.LoopVariable.ToString()));
            }
            var indexExpr = cc.LoopIndexVariable;

            //
            // We need to hold onto either the first or the last item here, so we create a statement that holds nnto the
            // first or the last time. It also has to mark the thing as valid! It will break when it is done.
            // While the bool can be used later on to get at the exception we might be throwing, the actual
            // result may be used much further on down. To protect against that, we set the array index to be -1,
            // and then hope there is a crash later on! :-)
            //

            var valueWasSeen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var indexSeen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            indexSeen.SetInitialValue("-1");

            gc.AddOutsideLoop(valueWasSeen);
            gc.AddOutsideLoop(indexSeen);

            var indexValue = ExpressionToCPP.GetExpression(indexExpr, gc, cc, container);
            gc.Add(new Statements.StatementRecordValue(indexSeen, indexValue, valueWasSeen, isFirst));

            //
            // Ok - we now pop out, and throw an exception if we are supposed to always have a number, and then continue on.
            //

            gc.Pop();

            if (bombIfNothing)
            {
                var test = ExpressionToCPP.GetExpression(Expression.Not(valueWasSeen), gc, cc, container);
                gc.Add(new Statements.StatementThrowIfTrue(test, "First predicate executed on a null sequence"));
            }

            var firstlastValue = cc.LoopVariable.ReplaceSubExpression(cc.LoopIndexVariable, Expression.Parameter(typeof(int), indexSeen.RawValue));
            return firstlastValue;
#if false
            var actualValue = DeclarableParameter.CreateDeclarableParameterExpression(cc.LoopVariable.Type);

            gc.Add(new Statements.StatementAssign(actualValue,
                ExpressionToCPP.GetExpression(firstlastValue, gc, cc, container)));

            return actualValue;
#endif
        }
    }
}
