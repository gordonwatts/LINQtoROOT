using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

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
        /// Process a result operator. If this result is amenable to be made into a function, then
        /// do so.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            // Look for a single-result processor

            var processor = _operators.FindScalarROProcessor(resultOperator.GetType());
            if (processor != null)
            {
                var result = processor.ProcessResultOperator(resultOperator, queryModel, _codeEnv, _codeContext, MEFContainer);
                if (result != null)
                {
                    _codeEnv.SetResult(result);
                    _scoping.Add(_codeContext.Add(queryModel, result));
                }
                return;
            }

            // Look for a sequence processor

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
        /// Keep the list of scoping guys around.
        /// </summary>
        private List<IVariableScopeHolder> _scoping = new List<IVariableScopeHolder>();

        /// <summary>
        /// List of variable scope's for things that were cached while we ran.
        /// </summary>
        /// <remarks>Pop these off when things out out of context otherwise they can infect later parts of the query.</remarks>
        public IEnumerable<IVariableScopeHolder> VariableScopeHolders { get { return _scoping; } }

        /// <summary>
        /// Keep track of the main index variable if it should be gotten rid of!
        /// </summary>
        public IVariableScopeHolder MainIndexScope { get; private set; }

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
            public Tuple<Expression, IDeclaredParameter> AddLoop(IGeneratedQueryCode env, ICodeContext context, CompositionContainer container)
            {
                return Tuple.Create<Expression, IDeclaredParameter>(Expression.Variable(thisType, "this"), null);
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
                MainIndexScope = new OutterLoopArrayInfo(fromClause.ItemType).CodeLoopOverArrayInfo(fromClause, _codeEnv, _codeContext, MEFContainer);
            }
            else
            {
                CodeLoopOverExpression(fromClause, fromClause.FromExpression, fromClause.ItemName);
            }
        }

        /// <summary>
        /// The main driver to parse the query model. We will cache the result into a function
        /// if it is something we know how to cache.
        /// </summary>
        /// <param name="queryModel"></param>
        public override void VisitQueryModel(QueryModel queryModel)
        {
            Debug.WriteLine("VisitQueryModel: {0}{1}", queryModel.ToString(), "");
            Debug.Indent();

            // If this is a QM function we are referencing, then process it as such.
            var qmSource = _codeEnv.FindQMFunction(queryModel);
            if (qmSource == null)
            {
                VisitQueryModelNoCache(queryModel);
            }
            else
            {
                VisitQueryModelCache(queryModel, qmSource);
            }
            Debug.Unindent();
        }

        /// <summary>
        /// Cache the result of a query model into a function.
        /// </summary>
        /// <param name="queryModel"></param>
        /// <param name="qmSource"></param>
        private void VisitQueryModelCache(QueryModel queryModel, IQMFunctionSource qmSource)
        {
            // If we already have the answer for this cache, then we should just re-call the routine.
            if (qmSource.StatementBlock != null)
            {
                Debug.WriteLine("Using previously cached QM result");
                GenerateQMFunctionCall(qmSource);
                return;
            }
            Debug.WriteLine("Cache: Gathering Data");
            Debug.Indent();

            // Since we don't have it cached, we need to re-run things, and carefully watch for
            // everything new that shows up. What shows up will be what we declare as the function
            // body.
            var currentScope = _codeEnv.CurrentScope;
            var topLevelStatement = new StatementInlineBlock();
            _codeEnv.Add(topLevelStatement);
            _codeEnv.SetCurrentScopeAsResultScope();

            // If this variable has been cached, then return it. Otherwise, mark the cache as filled.
            _codeEnv.Add(new StatementFilter(qmSource.CacheVariableGood));
            _codeEnv.Add(new StatementReturn(qmSource.CacheVariable));
            _codeEnv.Pop();
            _codeEnv.Add(new StatementAssign(qmSource.CacheVariableGood, new ValSimple("true", typeof(bool)), new IDeclaredParameter[] { }));

            // Now, run the code to process the query model!

            VisitQueryModelNoCache(queryModel);

            // The result is treated differently depending on it being a sequence or a single value.
            if (qmSource.IsSequence)
            {
                // Push the good values into our cache object.
                if (!(_codeContext.LoopIndexVariable is IDeclaredParameter))
                    throw new InvalidOperationException("Can't deal with anythign that isn't a loop var");
                _codeEnv.Add(new StatementRecordIndicies(_codeContext.LoopIndexVariable as IDeclaredParameter, qmSource.CacheVariable));

                // Remember what the loop index variable is, as we are going to need it when we generate the return function!
                qmSource.SequenceVariable(_codeContext.LoopIndexVariable, _codeContext.LoopVariable);
            }
            else
            {
                // This is a specific result. Save just the result and return it.
                // Grab the result, cache it, and return it.
                var rtnExpr = ExpressionToCPP.GetExpression(_codeEnv.ResultValue, _codeEnv, _codeContext, MEFContainer);
                topLevelStatement.Add(new StatementAssign(qmSource.CacheVariable, rtnExpr, FindDeclarableParameters.FindAll(_codeEnv.ResultValue)));

                // If the return is a declared parameter, then it must be actually defined somewhere (we normally don't).
                var declParam = _codeEnv.ResultValue as IDeclaredParameter;
                if (declParam != null)
                    topLevelStatement.Add(declParam, false);
            }

            // Always return the proper value...
            topLevelStatement.Add(new StatementReturn(qmSource.CacheVariable));

            // Now extract the block of code and put it in the function block.
            _codeEnv.CurrentScope = currentScope;
            qmSource.SetCodeBody(topLevelStatement);

            // Reset our state and remove the function code. And put in the function call in its place.
            _codeEnv.Remove(topLevelStatement);
            GenerateQMFunctionCall(qmSource);

            Debug.Unindent();
        }

        /// <summary>
        /// Generate a function call statement for the cached function we are going to emit.
        /// </summary>
        /// <param name="qmSource"></param>
        /// <returns></returns>
        private void GenerateQMFunctionCall(IQMFunctionSource qmSource)
        {
            // Assemble what we need for the sequence call.
            if (qmSource.Arguments.Any())
                throw new NotImplementedException("Can only deal with internal functions with no arguments.");
            var call = string.Format("{0} ()", qmSource.Name);

            if (qmSource.IsSequence)
            {
                // For the sequence we get the resulting vector array.
                var cvar = DeclarableParameter.CreateDeclarableParameterExpression(qmSource.ResultType);
                _codeEnv.Add(new StatementAssign(cvar, new ValSimple(call, qmSource.ResultType), new IDeclaredParameter[] { }, true));

                // Now, do a loop over it.
                var loopVar = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
                _codeEnv.Add(new StatementForLoop(loopVar, new ValSimple(string.Format("{0}.size()", cvar.RawValue), typeof(int))));

                // Finally, we setup the loop index variables to match what they did when we ran the functoin.
                var oldLoopIndex = qmSource.OldLoopIndexVariable;
                Expression oldLoopVariable = qmSource.OldLoopExpression;

                var arrayLookup = Expression.ArrayIndex(cvar, loopVar);
                _codeContext.SetLoopVariable(arrayLookup, loopVar);
            }
            else
            {
                // For the non-sequence this just returns a value that we need.
                _codeEnv.SetResult(Expression.Parameter(qmSource.ResultType, call));
            }
        }

        /// <summary>
        /// Turn a query model into code.
        /// </summary>
        /// <param name="queryModel"></param>
        private void VisitQueryModelNoCache(QueryModel queryModel)
        {
            // Cache the referenced query expressions and restore them at the end.

            var cachedReferencedQS = _codeContext.GetAndResetQuerySourceLookups();

            // Protect against all returns...

            try
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
                            Debug.WriteLine("Identity Query being processed");
                            _codeEnv.SetResult(result.Item2);
                            return;
                        }
                    }
                }

                // Have we seen this query model before? If so, perhaps we can just short-circuit this?

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

                // If we drop through here, then let the full machinery parse the thing

                base.VisitQueryModel(queryModel);
            }
            finally
            {
                _codeContext.RestoreQuerySourceLookups(cachedReferencedQS);
            }
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
            MainIndexScope = Expressions.ArrayExpressionParser.ParseArrayExpression(query, loopExpr, _codeEnv, _codeContext, MEFContainer);
            if (MainIndexScope == null)
                MainIndexScope = _codeContext.Add(query, _codeContext.LoopVariable);
            Debug.WriteLine("CodeLoopOverExpression: LoopExpr: {0} LoopVar: {1}", loopExpr.ToString(), _codeContext.LoopVariable.ToString());
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
                _codeContext.LoopIndexVariable);
            _codeEnv.Add(savePairValues);

            _codeEnv.PopToResultsLevel();

            //
            // Now, we need to sort and loop over the variables in the map. This is a bit of a messy
            // multi-line statement, and it is a compound statement.
            //

            var sortAndRunLoop = new StatementLoopOverSortedPairValue(mapRecord, ordering.OrderingDirection == OrderingDirection.Asc);
            _codeEnv.Add(sortAndRunLoop);

            var pindex = sortAndRunLoop.IndexVariable;
            var lv = _codeContext.LoopIndexVariable.RawValue;
            _codeContext.Add(lv, pindex);
            _codeContext.SetLoopVariable(_codeContext.LoopVariable.ReplaceSubExpression(_codeContext.LoopIndexVariable.AsExpression(), pindex), pindex);
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
