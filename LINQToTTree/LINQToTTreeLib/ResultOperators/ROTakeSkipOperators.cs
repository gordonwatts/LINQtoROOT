using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// Deal with the take and the skip operators in an expression
    /// </summary>
    [Export(typeof(IQVResultOperator))]
    class ROTakeSkipOperators : IQVResultOperator
    {
        /// <summary>
        /// We can handle either the take or the skipping of items! :-)
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(TakeResultOperator)
                || resultOperatorType == typeof(SkipResultOperator);
        }

        /// <summary>
        /// Implement the skipping. We have a main limitation: we currently know only how to implement integer skipping.
        /// We implement with "if" statements to support composability, even if it means running longer in the end...
        /// We actually return nothing when goes - we aren't really a final result the way "Count" is.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <returns></returns>
        public IVariable ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedCode codeEnv, ICodeContext codeContext, CompositionContainer container)
        {
            ///
            /// Quick checks to make sure
            /// 

            if (codeEnv == null)
                throw new ArgumentNullException("codeEnv cannot be null");

            var take = resultOperator as TakeResultOperator;
            var skip = resultOperator as SkipResultOperator;

            if (take == null && skip == null)
            {
                throw new ArgumentNullException("resultOperator must not be null and must represent either a take or a skip operation!");
            }

            if (take != null && take.Count.Type != typeof(int))
                throw new ArgumentException("Take operator count must be an integer!");
            if (skip != null && skip.Count.Type != typeof(int))
                throw new ArgumentException("Skip operator count must be an integer!");

            ///
            /// Now, we create a count variable and that is how we will tell if we are still skipping or
            /// taking. It must be declared in the current block, before our current code! :-)
            /// 

            var counter = new VarInteger();
            codeEnv.Add(counter);

            codeEnv.Add(new StatementIncrementInteger(counter));
            var comparison = StatementIfOnCount.ComparisonOperator.LessThanEqual;
            IValue comparisonValue = null;
            if (skip != null)
            {
                comparison = StatementIfOnCount.ComparisonOperator.GreaterThan;
                comparisonValue = ExpressionVisitor.GetExpression(skip.Count, codeEnv, codeContext, container);
            }
            else
            {
                comparisonValue = ExpressionVisitor.GetExpression(take.Count, codeEnv, codeContext, container);
            }

            codeEnv.Add(new StatementIfOnCount(counter, comparisonValue, comparison));

            ///
            /// Subsequent guys are going to want to take the result of this operator an iterate over it.
            /// So we need to return a loopable variable. But this is a funny loopable variable, of course,
            /// as we are already in the loop. If anyone else wants to add statements, they just add them
            /// in the normal way. So we create a sepcial loop variable.
            /// 

            return new TakeSkipLoopVariable(codeContext.LoopVariable);
        }

        /// <summary>
        /// An internal loop variable - so we can make sure that we are looping over the proper things
        /// </summary>
        class TakeSkipLoopVariable : IVariable, ISequenceAccessor
        {
            /// <summary>
            /// What is the main loop variable (the indexer) that we are holding onto??
            /// </summary>
            private IVariable _index;

            /// <summary>
            /// Setup with the proper index variable so that we can "return" a loop variable later on.
            /// </summary>
            /// <param name="iVariable"></param>
            public TakeSkipLoopVariable(IVariable iVariable)
            {
                if (iVariable == null)
                    throw new ArgumentNullException("Can't create a fake loop/skip variable based on a null looper");

                _index = iVariable;
            }

            public string VariableName
            {
                get { return _index.VariableName; }
            }

            public IValue InitialValue
            {
                get { return _index.InitialValue; }
                set { _index.InitialValue = value; }
            }

            public bool Declare
            {
                get { return _index.Declare; }
                set { _index.Declare = value; }
            }

            public string RawValue
            {
                get { return _index.RawValue; }
            }

            public Type Type
            {
                get { return _index.Type; }
            }

            /// <summary>
            /// Add the statements that will actually cause the loops. The fantastic thing about this is
            /// that the loop is already implied - we are already running it. So we need only declare
            /// the new index variable and we are set.
            /// </summary>
            /// <param name="env"></param>
            /// <param name="context"></param>
            /// <param name="indexName"></param>
            public IVariable AddLoop(IGeneratedCode env, ICodeContext context, string indexName)
            {
                context.Add(indexName, context.LoopVariable);
                return context.LoopVariable;
            }
        }
    }
}
