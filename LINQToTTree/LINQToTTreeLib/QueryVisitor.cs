﻿using System;
using System.ComponentModel.Composition;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
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
            _codeEnv = code;
            _codeContext = context;
            if (_codeContext == null)
                _codeContext = new CodeContext();

            SubExpressionParse = false;
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
            var processor = _operators.FindROProcessor(resultOperator.GetType());
            if (processor == null)
            {
                throw new InvalidOperationException("LINQToTTree can't translate the operator '" + resultOperator.ToString() + "'");
            }

            var result = processor.ProcessResultOperator(resultOperator, queryModel, _codeEnv, _codeContext);
            if (result != null)
            {
                _codeEnv.SetResult(result);
            }
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
            ///
            /// For the main clause we will just define the variable as "this".
            /// 

            if (!SubExpressionParse)
            {
                var outter = new VarOutterLoopObjectPointer(fromClause.ItemName, fromClause.ItemType);
                _codeEnv.Add(outter);
                _codeContext.Add(outter.VariableName, outter);
            }
            else
            {
                var arrayRef = ExpressionVisitor.GetExpression(fromClause.FromExpression, _codeEnv, _codeContext);

                var loop = new StatementLoopOnVector(arrayRef, typeof(int).CreateUniqueVariableName());
                _codeEnv.Add(loop);
                _mainIndex = _codeContext.Add(fromClause.ItemName, loop.ObjectReference);
            }
        }

        /// <summary>
        /// Main driver. Parse the query model.
        /// </summary>
        /// <param name="queryModel"></param>
        public override void VisitQueryModel(QueryModel queryModel)
        {
            base.VisitQueryModel(queryModel);

            ///
            /// If a main index variable was declared that has now lost its usefulness, we should get rid of it.
            /// 

            if (_mainIndex != null)
                _mainIndex.Pop();
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

            var arrayToIterateOver = ExpressionVisitor.GetExpression(fromClause.FromExpression, _codeEnv, _codeContext);
            var loopstatement = new StatementLoopOnVector(arrayToIterateOver, fromClause.ItemName);
            _codeEnv.Add(loopstatement);
            _codeContext.Add(fromClause.ItemName, loopstatement.ObjectReference);
        }

        /// <summary>
        /// Deal with a where clause - we need to create an if statement that deals with the expression we are testing!
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            _codeEnv.Add(new Statements.StatementFilter(ExpressionVisitor.GetExpression(whereClause.Predicate, _codeEnv, _codeContext)));
        }
    }
}
