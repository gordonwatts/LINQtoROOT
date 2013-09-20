using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Visitor patter class to move through the expression we've been handed.
    /// 
    /// This QV deals with things that are going to be scalar results and also collection
    /// results. Logically, it would be nice to have a QV that deals with scalar results and
    /// one that deals with collections. However, if you think about what a query looks like
    /// (and the fact it can contain Where clauses, etc.) the logic for the two cases become
    /// very intertwined. If you split things up, then it is likely to lead to a set of
    /// very inter-connected objects. Sub-queries suck in this respect!
    /// </summary>
    public class QueryVisitor : QueryModelVisitorBase
    {
        /// <summary>
        /// Hold onto the code we are writing.
        /// </summary>
        private IGeneratedQueryCode _codeEnv;

        /// <summary>
        /// Keep track of the code context
        /// </summary>
        private ICodeContext _codeContext;

        /// <summary>
        /// Create a new visitor and add our code to the current spot we are in the "code".
        /// </summary>
        /// <param name="code"></param>
        public QueryVisitor(IGeneratedQueryCode code, ICodeContext context, CompositionContainer container)
        {
            _codeEnv = code;
            _codeContext = context;
            MEFContainer = container;

            if (_codeContext == null)
                _codeContext = new CodeContext();
        }

        /// <summary>
        /// Keep all the operators we know about hanging around!
        /// </summary>
#pragma warning disable 649
        [Import]
        QVResultOperators _operators;
#pragma warning restore 649

        /// <summary>
        /// We need to process a result operator.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            var processor = _operators.FindScalarROProcessor(resultOperator.GetType());
            if (processor != null)
            {
                var result = processor.ProcessResultOperator(resultOperator, queryModel, _codeEnv, _codeContext, MEFContainer);
                if (result != null)
                {
                    _codeEnv.SetResult(result);
                    _codeContext.Add(queryModel, result);
                }
                return;
            }

            var collectionProcessor = _operators.FindCollectionROProcessor(resultOperator.GetType());
            if (collectionProcessor != null)
            {
                collectionProcessor.ProcessResultOperator(resultOperator, queryModel, _codeEnv, _codeContext, MEFContainer);
                _codeEnv.ResetResult();
                return;
            }

            ///
            /// Uh oh - no idea how to do this!
            /// 

            throw new InvalidOperationException("LINQToTTree can't translate the operator '" + resultOperator.ToString() + "'");
        }

        /// <summary>
        /// Keep track of the main index variable if it should be gotten rid of!
        /// </summary>
        private IVariableScopeHolder _mainIndex = null;

        /// <summary>
        /// Helper class for dealing with an outter array - which means we do no actual looping! :-)
        /// </summary>
        class OutterLoopArrayInfo : IArrayInfo
        {
            private Type thisType;

            /// <summary>
            /// Create an outter loop array reference - with the given type.
            /// </summary>
            /// <param name="type"></param>
            public OutterLoopArrayInfo(Type type)
            {
                thisType = type;
            }

            /// <summary>
            /// We add no statements to the loop as we are the outter variable - and
            /// since we are in the middle of a TSelector, we reference "this" - ourselves!
            /// </summary>
            /// <param name="env"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            public Tuple<Expression, Expression> AddLoop(IGeneratedQueryCode env, ICodeContext context, CompositionContainer container)
            {
                return Tuple.Create<Expression, Expression>(Expression.Variable(thisType, "this"), null);
            }
        }


        /// <summary>
        /// Run the main from clause in a query. This can be called more than  once - when you have
        /// multiple from queries at the top level.
        /// </summary>
        /// <param name="fromClause"></param>
        /// <param name="queryModel"></param>
        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            //
            // For the main clause we will just define the variable as "this".
            // 

            if (fromClause.ItemType == _codeContext.BaseNtupleObjectType)
            {
                _mainIndex = new OutterLoopArrayInfo(fromClause.ItemType).CodeLoopOverArrayInfo(fromClause, _codeEnv, _codeContext, MEFContainer);
            }
            else
            {
                CodeLoopOverExpression(fromClause, fromClause.FromExpression, fromClause.ItemName);
            }
        }

        /// <summary>
        /// Main driver. Parse the query model.
        /// </summary>
        /// <param name="queryModel"></param>
        public override void VisitQueryModel(QueryModel queryModel)
        {
            //
            // If the query model is something that is trivial, then
            // perhaps there is a short-cut we can take?
            //

            if (queryModel.IsIdentityQuery() && queryModel.ResultOperators.Count == 1)
            {
                var ro = queryModel.ResultOperators[0];
                var processor = _operators.FindScalarROProcessor(ro.GetType());
                if (processor != null)
                {
                    var result = processor.ProcessIdentityQuery(ro, queryModel, _codeEnv, _codeContext, MEFContainer);
                    if (result != null
                        && result.Item1)
                    {
                        _codeEnv.SetResult(result.Item2);
                        return;
                    }
                }
            }

            //
            // Have we seen this query model before? If so, perhaps we can just short-circuit this?
            //

            var cachedResult = _codeContext.GetReplacement(queryModel);
            if (cachedResult != null)
            {
                var context = _codeEnv.FirstAllInScopeFromNow(FindDeclarableParameters.FindAll(cachedResult));
                if (context != null)
                {
                    _codeEnv.SetResult(cachedResult);
                    return;
                }
            }

            //
            // If we drop through here, then let the full machinery parse the thing
            //

            base.VisitQueryModel(queryModel);
        }

        /// <summary>
        /// We are going to run a sub-class - a nested loop...
        /// </summary>
        /// <param name="fromClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            ///
            /// With the expression to loop over, just iterate! :-) Some of this probably will need to be
            /// generalized when we loop over more than just a "std::vector".
            /// 

            CodeLoopOverExpression(fromClause, fromClause.FromExpression, fromClause.ItemName);
        }

        /// <summary>
        /// Given an expression which we can turn into a loop, add the loop to the current
        /// code block.
        /// </summary>
        /// <param name="loopExpr"></param>
        /// <param name="indexName"></param>
        private void CodeLoopOverExpression(IQuerySource query, Expression loopExpr, string indexName)
        {
            Expressions.ArrayExpressionParser.ParseArrayExpression(query, loopExpr, _codeEnv, _codeContext, MEFContainer);
            _mainIndex = _codeContext.Add(query, _codeContext.LoopVariable);
        }

        /// <summary>
        /// Deal with a where clause - we need to create an if statement that deals with the expression we are testing!
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            _codeEnv.Add(new Statements.StatementFilter(ExpressionToCPP.GetExpression(whereClause.Predicate, _codeEnv, _codeContext, MEFContainer)));
        }

        /// <summary>
        /// If the user is doing some selection that contains parameters that require replacements, then
        /// we have to make sure they are done. Otherwise we leave these alone. Note, we *only* want to do
        /// the parameter replacement however! Whatever that select is, it becomes our new loop variable.
        /// </summary>
        /// <param name="selectClause"></param>
        /// <param name="queryModel"></param>
        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            var expr = ParameterReplacementExpressionVisitor.ReplaceParameters(selectClause.Selector, _codeContext);
            _codeContext.SetLoopVariable(expr, _codeContext.LoopIndexVariable);
        }

        /// <summary>
        /// Sort the current stream of the query. To do this we run through all the results, sort them,
        /// and then start a new loop.
        /// </summary>
        /// <param name="ordering"></param>
        /// <param name="queryModel"></param>
        /// <param name="orderByClause"></param>
        /// <param name="index"></param>
        public override void VisitOrdering(Ordering ordering, QueryModel queryModel, OrderByClause orderByClause, int index)
        {
            //
            // Only number types can be sorted.
            //

            if (!ordering.Expression.Type.IsNumberType())
                throw new InvalidOperationException(string.Format("Don't know how to sort query by type '{0}'.", ordering.Expression.Type.Name));

            //
            // First, record all the indicies and the values. This is what we are going to be sorting.
            // 

            var mapRecord = DeclarableParameter.CreateDeclarableParameterMapExpression(ordering.Expression.Type, _codeContext.LoopIndexVariable.Type.MakeArrayType());
            _codeEnv.AddOutsideLoop(mapRecord);

            var savePairValues = new StatementRecordPairValues(mapRecord,
                ExpressionToCPP.GetExpression(ordering.Expression, _codeEnv, _codeContext, MEFContainer),
                ExpressionToCPP.GetExpression(_codeContext.LoopIndexVariable, _codeEnv, _codeContext, MEFContainer));
            _codeEnv.Add(savePairValues);

            _codeEnv.PopToResultsLevel();

            //
            // Now, we need to sort and loop over the variables in the map. This is a bit of a messy
            // multi-line statement, and it is a compound statement.
            //

            var sortAndRunLoop = new StatementLoopOverSortedPairValue(mapRecord, ordering.OrderingDirection == OrderingDirection.Asc);
            _codeEnv.Add(sortAndRunLoop);

            var pindex = sortAndRunLoop.IndexVariable;
            var lv = _codeContext.LoopIndexVariable.ParameterName();
            _codeContext.Add(lv, pindex);
            _codeContext.SetLoopVariable(_codeContext.LoopVariable.ReplaceSubExpression(_codeContext.LoopIndexVariable, pindex), pindex);
        }

        /// <summary>
        /// We don't implement this yet. Make that explicit.
        /// </summary>
        /// <param name="groupJoinClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
        {
            throw new NotImplementedException("Group Join clauses in LINQ are not yet implemented");
            //base.VisitGroupJoinClause(groupJoinClause, queryModel, index);
        }

        /// <summary>
        /// Get/Set the container that can be usef for MEF'ing things.
        /// </summary>
        public CompositionContainer MEFContainer { get; private set; }
    }
}
