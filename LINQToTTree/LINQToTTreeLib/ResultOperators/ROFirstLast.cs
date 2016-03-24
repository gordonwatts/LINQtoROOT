using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq.Expressions;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// The user wants either the first or the last item in this list.
    /// There are two versions of this predicate we have to deal with. First is just the First/Last
    /// operator. If we find nothign in the sequence, then we throw an exception. That is the easy bit.
    /// 
    /// However, if, instead, it is FirstOrDefault or LastOrDefault then we have to be a little more careful.
    /// If it is an object, then by default we index into an array location at -1. This is how we indicate
    /// "null" - and that is what happens when the user attempts to look for something using a null deref.
    /// If it is an int, double, or float, then we will have to put in a cache value instead. Ugh.
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
            // Figure out if we need to cache the result:
            //  - simple variable which has a default value which can be used later on.
            //      like a double, etc.
            //  - We actually allow for a default variable.
            //

            bool cacheResult = cc.LoopVariable.Type.IsNumberType();
            cacheResult = cacheResult && !bombIfNothing;

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
            // It is possible that this is part of a dual selection. For example, if you are interested in the jet that has the closest track, and the
            // loop is constructed over the jets first, and then the tracks. This First will likely be for a track index, but we will be looking up the
            // track later. So we need to record both the jet and track index. To get the other indicies, we just look for all loop variables between here and
            // the result scope.
            //

            var valueWasSeen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var indexSeen = DeclarableParameter.CreateDeclarableParameterExpression(indexExpr.Type);
            if (indexSeen.Type.IsNumberType())
                indexSeen.SetInitialValue("-1");

            gc.AddAtResultScope(valueWasSeen);
            gc.AddAtResultScope(indexSeen);

            var rv = new Statements.StatementRecordValue(indexSeen, indexExpr, valueWasSeen, isFirst);
            gc.Add(rv);

            foreach (var v in gc.GetUsedQuerySourceVariables(rv, indexExpr))
            {
                var saver = DeclarableParameter.CreateDeclarableParameterExpression(v.Type);
                gc.AddAtResultScope(saver);
                rv.AddNewSaver(saver, v);
                cc.Add(v.RawValue, saver);
            }

            gc.Pop(true);

            if (bombIfNothing)
            {
                var test = ExpressionToCPP.GetExpression(Expression.Not(valueWasSeen), gc, cc, container);
                gc.Add(new Statements.StatementThrowIfTrue(test, string.Format("First/Last predicate executed on a null sequence: {0}", queryModel.ToString())));
            }

            //
            // Finally, we need the new expression. For this we basically just ask for the translated expression. We
            // also add a substitution for later on for more complex expressions.
            //

            var firstlastValue = cc.LoopVariable;
            cc.Add(indexExpr.RawValue, indexSeen);

            Debug.WriteLine("First/Last: {0} for QM {1}", indexSeen.ToString(), queryModel.ToString());

            // Reset the expression we are looking at in the loop.
            var newIndexExpr = firstlastValue.ReplaceSubExpression(indexExpr.AsExpression(), indexSeen);
            cc.SetLoopVariable(newIndexExpr, indexSeen);

            if (cacheResult)
            {
                //
                // Set the default value
                //

                var actualValue = DeclarableParameter.CreateDeclarableParameterExpression(cc.LoopVariable.Type);
                actualValue.SetInitialValue("0");

                //
                // If everything went well, then we can do the assignment. Otherwise, we leave
                // it as above (having the default value).
                //

                gc.Add(new Statements.StatementFilter(valueWasSeen));
                gc.Add(new Statements.StatementAssign(actualValue,
                    ExpressionToCPP.GetExpression(firstlastValue, gc, cc, container)));
                gc.Pop();

                return actualValue;
            }
            else
            {
                // No need to cache the result - so no need to add extra code.
                return newIndexExpr;
            }
        }

        public Tuple<bool, Expression> ProcessIdentityQuery(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, CompositionContainer container)
        {
            return null;
        }

    }
}
