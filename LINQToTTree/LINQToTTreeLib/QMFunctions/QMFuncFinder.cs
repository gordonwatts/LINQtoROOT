
using LINQToTTreeLib.QueryVisitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace LINQToTTreeLib.QMFunctions
{
    /// <summary>
    /// Look at a QueryModel and return all the functions that we will want to cache,
    /// along with their arguments.
    /// </summary>
    class QMFuncFinder
    {
        /// <summary>
        /// Scan the QM for all header functions.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IEnumerable<QMFuncHeader> FindQMFunctions(QueryModel model)
        {
            var qmf = new QMFuncVisitor();
            qmf.VisitQueryModel(model);
            var funcs = qmf.FoundFunctions;
            DumpFuncs(funcs);
            return funcs;
        }

        [Conditional("DEBUG")]
        private static void DumpFuncs(List<QMFuncHeader> funcs)
        {
            foreach (var f in funcs)
            {
                Trace.WriteLine(string.Format("Searching for QM function {0}", f.QM.ToString()));
            }
        }

        /// <summary>
        /// Internal class that does the QM visiting. Figure out what arguments, etc., are needed to be
        /// saved for later reference.
        /// </summary>
        private class QMFuncVisitor : QueryModelVisitorBase
        {
            private QMExpressionVisitor _exprVisitor;

            /// <summary>
            /// Get the class setup to look for QM functions.
            /// </summary>
            public QMFuncVisitor()
            {
                FoundFunctions = new List<QMFuncHeader>();
                _exprVisitor = new QMExpressionVisitor(this);
            }

            /// <summary>
            /// The list of QM headers that we have found in this query model.
            /// </summary>
            public List<QMFuncHeader> FoundFunctions { get; set; }

            /// <summary>
            /// Internal class to track the QM what it defines and what it references.
            /// </summary>
            private class QMContext
            {
                /// <summary>
                /// Keep track of the arguments that are used by this QM.
                /// </summary>
                public List<QuerySourceReferenceExpression> _arguments = new List<QuerySourceReferenceExpression>();

                /// <summary>
                /// Keep track of the query sources that this QM generates.
                /// </summary>
                private List<IQuerySource> _qmItemIndex = new List<IQuerySource>();

                /// <summary>
                /// Track arguments needed by this QM.
                /// </summary>
                /// <param name="expression"></param>
                internal void AddQSReference(QuerySourceReferenceExpression expression)
                {
                    if (!_qmItemIndex.Contains(expression.ReferencedQuerySource)
                        && !_arguments.Contains(expression))
                        _arguments.Add(expression);
                }

                /// <summary>
                /// Return true if this expression is contained in our list.
                /// </summary>
                /// <param name="expression"></param>
                /// <returns></returns>
                private bool ContainsQSReference(QuerySourceReferenceExpression expression)
                {
                    return _qmItemIndex.Contains(expression.ReferencedQuerySource);
                }

                /// <summary>
                /// A from item is being generated in the QM clause. We need to record it so we don't
                /// do an argument reference to it.
                /// </summary>
                /// <param name="p"></param>
                internal void AddFromItem(IQuerySource p)
                {
                    _qmItemIndex.Add(p);
                }
            }

            /// <summary>
            /// Keep track of what QM we are currently working on.
            /// </summary>
            private Stack<QMContext> _qmContextStack = new Stack<QMContext>();

            /// <summary>
            /// The main entry point that we watch - we generate a method info stub when we need it
            /// from this.
            /// </summary>
            /// <param name="queryModel"></param>
            public override void VisitQueryModel(QueryModel queryModel)
            {
                // If the type is something that is friendly to be returned from a
                // method, then we should cache this guy.

                _qmContextStack.Push(new QMContext());

                // Now, run through the query model.

                base.VisitQueryModel(queryModel);

                // And if the QM result type is something we can reasonably cache, then we should do it.
                //  - Do not cache the outter most QM. This guy has the best place to start combining things.
                //  - Do not cache anything that is enumerable. We'll have to deal with that later.
                //  - Do not cache any anonymous types
                //  - Deal with later somethign that is an iterator (used in a later loop).

                if (_qmContextStack.Count > 1
                    && !typeof(IEnumerable).IsAssignableFrom(queryModel.GetResultType())
                    && !queryModel.GetResultType().IsClass
                    && !queryModel.GetResultType().Name.Contains("Anon"))
                {
                    if (queryModel.ResultOperators.Any())
                    {
                        var qmText = FormattingQueryVisitor.Format(queryModel);
                        if (!FoundFunctions.Where(ff => ff.QMText == qmText).Any())
                        {
                            var sref = _qmContextStack.Peek();
                            var f = new QMFuncHeader()
                            {
                                QM = queryModel,
                                QMText = qmText,
                                Arguments = sref._arguments.Cast<object>()
                            };
                            FoundFunctions.Add(f);
                        }
                    }
                }

                // Go back to working on the previous qm.
                _qmContextStack.Pop();
            }

            /// <summary>
            /// We are going to generate a QueryRefernce guy. Record it - any references to it aren't
            /// references to external arguments.
            /// </summary>
            /// <param name="fromClause"></param>
            /// <param name="queryModel"></param>
            public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
            {
                if (_mainFromQS == null)
                    _mainFromQS = fromClause;

                _qmContextStack.Peek().AddFromItem(fromClause);
                _exprVisitor.VisitExpression(fromClause.FromExpression);
                base.VisitMainFromClause(fromClause, queryModel);
            }

            /// <summary>
            /// Additional from clauses attached to the top here.
            /// </summary>
            /// <param name="fromClause"></param>
            /// <param name="queryModel"></param>
            /// <param name="index"></param>
            public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
            {
                _qmContextStack.Peek().AddFromItem(fromClause);
                _exprVisitor.VisitExpression(fromClause.FromExpression);
                base.VisitAdditionalFromClause(fromClause, queryModel, index);
            }

            /// <summary>
            /// Look for the expression associated with a Where clause, and run the parser on that.
            /// </summary>
            /// <param name="whereClause"></param>
            /// <param name="queryModel"></param>
            /// <param name="index"></param>
            public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
            {
                _exprVisitor.VisitExpression(whereClause.Predicate);
            }

            /// <summary>
            /// Visit the select clause
            /// </summary>
            /// <param name="selectClause"></param>
            /// <param name="queryModel"></param>
            public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
            {
                _exprVisitor.VisitExpression(selectClause.Selector);
            }

            /// <summary>
            /// While parsing an expression (or similar) we are referencing a query expression. This
            /// is potentially an argument.
            /// </summary>
            /// <param name="expression"></param>
            /// <remarks>Ignore refernces to the main query source - this works out to be "this" when we do code translation, and so is special.</remarks>
            internal void AddQSReference(QuerySourceReferenceExpression expression)
            {
                if (expression.ReferencedQuerySource != _mainFromQS)
                    _qmContextStack.Peek().AddQSReference(expression);
            }

            /// <summary>
            /// Keep track fo who is the main from clause.
            /// </summary>
            private IQuerySource _mainFromQS;
        }

        /// <summary>
        /// Look at an expression for QM that should be parsed.
        /// </summary>
        private class QMExpressionVisitor : ExpressionTreeVisitor
        {
            private QMFuncVisitor _qmVisitor;
            public QMExpressionVisitor(QMFuncVisitor qmFinder)
            {
                _qmVisitor = qmFinder;
            }

            /// <summary>
            /// We are going after a query model - bounce this back up a level.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override System.Linq.Expressions.Expression VisitSubQueryExpression(Remotion.Linq.Clauses.Expressions.SubQueryExpression expression)
            {
                _qmVisitor.VisitQueryModel(expression.QueryModel);
                return expression;
            }

            /// <summary>
            /// These are arguments that need to be passed into...
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override System.Linq.Expressions.Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
            {
                _qmVisitor.AddQSReference(expression);
                return base.VisitQuerySourceReferenceExpression(expression);
            }
        }

        public static object funcs { get; set; }
    }

}
