﻿using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Visitor patter class to move through the expression we've been handed.
    /// </summary>
    public class QueryVisitor : QueryModelVisitorBase
    {
        /// <summary>
        /// Hold onto the code we are writing.
        /// </summary>
        private IGeneratedCode _codeEnv;

        /// <summary>
        /// Keep track of the code context
        /// </summary>
        private ICodeContext _codeContext;

        /// <summary>
        /// Create a new visitor and add our code to the current spot we are in the "code".
        /// </summary>
        /// <param name="code"></param>
        public QueryVisitor(IGeneratedCode code, ICodeContext context = null)
        {
#if false
            _codeEnv = code;
            _codeContext = context;
            if (_codeContext == null)
                _codeContext = new CodeContext();

            SubExpressionParse = false;
#endif
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
#if false
            var processor = _operators.FindROProcessor(resultOperator.GetType());
            if (processor == null)
            {
                throw new InvalidOperationException("LINQToTTree can't translate the operator '" + resultOperator.ToString() + "'");
            }

            var result = processor.ProcessResultOperator(resultOperator, queryModel, _codeEnv, _codeContext, MEFContainer);
            if (result != null)
            {
                _codeEnv.SetResult(result);
            }
#endif
            base.VisitResultOperator(resultOperator, queryModel, index);
        }

        /// <summary>
        /// Get/Set indicator if we are parsing a sub expression and thus should generate teh loop ourselves
        /// </summary>
        public bool SubExpressionParse { get; set; }

        /// <summary>
        /// Keep track of the main index variable if it should be gotten rid of!
        /// </summary>
        private IVariableScopeHolder _mainIndex = null;

        /// <summary>
        /// Run the main from clause in a query. This can be called more than  once - when you have
        /// multiple from queries at the top level.
        /// </summary>
        /// <param name="fromClause"></param>
        /// <param name="queryModel"></param>
        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            base.VisitMainFromClause(fromClause, queryModel);
#if false
            ///
            /// For the main clause we will just define the variable as "this".
            /// 

            if (!SubExpressionParse)
            {
                var outter = new VarOutterLoopObjectPointer(fromClause.ItemName, fromClause.ItemType);
                _codeEnv.Add(outter);
                _codeContext.Add(outter.VariableName, outter);
                _codeContext.SetLoopVariable(outter);
            }
            else
            {
                CodeLoopOverExpression(fromClause.FromExpression, fromClause.ItemName);
            }
#endif
        }

        /// <summary>
        /// Main driver. Parse the query model.
        /// </summary>
        /// <param name="queryModel"></param>
        public override void VisitQueryModel(QueryModel queryModel)
        {
            base.VisitQueryModel(queryModel);
#if false
            base.VisitQueryModel(queryModel);

            ///
            /// If a main index variable was declared that has now lost its usefulness, we should get rid of it.
            /// 

            if (_mainIndex != null)
                _mainIndex.Pop();
#endif
        }

        /// <summary>
        /// We are going to run a sub-class - a nested loop...
        /// </summary>
        /// <param name="fromClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            base.VisitAdditionalFromClause(fromClause, queryModel, index);
#if false
            ///
            /// With the expression to loop over, just iterate! :-) Some of this probably will need to be
            /// generalized when we loop over more than just a "std::vector".
            /// 

            CodeLoopOverExpression(fromClause.FromExpression, fromClause.ItemName);
#endif
        }

        /// <summary>
        /// Given an expression which we can turn into a loop, add the loop to the current
        /// code block.
        /// </summary>
        /// <param name="loopExpr"></param>
        /// <param name="indexName"></param>
        private void CodeLoopOverExpression(Expression loopExpr, string indexName)
        {
#if false
            var arrayToIterateOver = ExpressionVisitor.GetExpression(loopExpr, _codeEnv, _codeContext, MEFContainer);

            var seqAcc = arrayToIterateOver as ISequenceAccessor;
            if (seqAcc == null)
                throw new InvalidOperationException("A sequence should have been returned, but it doesn't seem to know how to be iterated over!");

            var indexv = seqAcc.AddLoop(_codeEnv, _codeContext, indexName, m => _mainIndex = m);
            _codeContext.SetLoopVariable(indexv);
#endif
        }

        /// <summary>
        /// Deal with a where clause - we need to create an if statement that deals with the expression we are testing!
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            base.VisitWhereClause(whereClause, queryModel, index);
#if false
            _codeEnv.Add(new Statements.StatementFilter(ExpressionVisitor.GetExpression(whereClause.Predicate, _codeEnv, _codeContext, MEFContainer)));
#endif
        }

        /// <summary>
        /// Get/Set the container that can be usef for MEF'ing things.
        /// </summary>
        public CompositionContainer MEFContainer { get; set; }
    }
}
