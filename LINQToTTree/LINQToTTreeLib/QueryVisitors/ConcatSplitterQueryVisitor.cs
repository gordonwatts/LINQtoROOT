using Remotion.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using LINQToTTreeLib.relinq;
using Remotion.Linq.Clauses.Expressions;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Remotion.Linq.Transformations;
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.QueryVisitors
{
    /// <summary>
    /// Scan a QueryModel for "Concat" result operators. Split the query each time we find a concat operator to produce multiple
    /// concat operators.
    /// </summary>
    public class ConcatSplitterQueryVisitor
    {
        /// <summary>
        /// Look at the QM and find all Concat operators - split the query into multiple queries.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static QueryModel[] Split(QueryModel source)
        {
            // Extract the from clauses, and then from that extract the result clauses.
            // This function, Split, is called recursively during this processing.
            var qvf = new SplitFromClauses();
            qvf.VisitQueryModel(source);

            var qvr = new SplitResultClauses();
            foreach(var q in qvf._models)
            {
                qvr.VisitQueryModel(q);
            }

            return qvr._allModels.ToArray();
        }

        private class SplitResultClauses : QueryModelVisitorBase
        {

            /// <summary>
            /// Accumulate the various choices after removing the Concat result operators.
            /// </summary>
            public List<QueryModel> _allModels = new List<QueryModel>();

            /// <summary>
            /// Look at all the result operators. The Concat operators can affect what happens before and after their
            /// position in the RO list. So we have to look at it as a collection, rather than individually.
            /// </summary>
            /// <param name="resultOperators"></param>
            /// <param name="queryModel"></param>
            protected override void VisitResultOperators(ObservableCollection<ResultOperatorBase> resultOperators, QueryModel queryModel)
            {
                // First, visit each individual result operator.
                for (int i = 0; i < queryModel.ResultOperators.Count; i++)
                {
                    VisitResultOperator(queryModel.ResultOperators[i], queryModel, i);
                }

                _allModels.AddRange(SplitQMByConcatResultOperator(queryModel));
            }

            /// <summary>
            /// Given a query model, look at all the result operators for Concat operators, and split everything up
            /// into separate query models.
            /// </summary>
            /// <param name="queryModel"></param>
            /// <returns></returns>
            private static IEnumerable<QueryModel> SplitQMByConcatResultOperator(QueryModel queryModel)
            {
                // If there are no concat result operators in the list, then we just bail quickly.
                // This is to specifically avoid the Clone operation unless we actually need it.
                if (!queryModel.ResultOperators.Where(r => r is ConcatResultOperator).Any())
                {
                    return new QueryModel[] { queryModel };
                }

                // Now, look for concat operators in the list. Pop them out when we find them.
                ConcatResultOperator ro = null;
                var qm = queryModel.Clone();
                var lst = new List<QueryModel>();
                while ((ro = qm.ResultOperators.Reverse().Where(r => r is ConcatResultOperator).Cast<ConcatResultOperator>().FirstOrDefault()) != null)
                {
                    // We are going to make q QueryModel here that uses the second source in the Concat operator.
                    // This means everything that comes before this query can be ignored - and we want this "source" to
                    // become the query from clause. Note this also means messing with the "select" clause to make sure it
                    // isn't doing anything special (select clause comes before result operators, semantically).

                    QueryModel newQM = NewQMFromOldWithLifting(ro.Source2, qm.MainFromClause.ItemType, qm.MainFromClause.ItemName);

                    var cc = new CloneContext(new QuerySourceMapping());

                    var indexToRemoveTo = qm.ResultOperators.IndexOf(ro);
                    for (int i = indexToRemoveTo + 1; i < qm.ResultOperators.Count; i++)
                    {
                        newQM.ResultOperators.Add(qm.ResultOperators[i].Clone(cc));
                    }
                    lst.Add(newQM.Flatten());

                    // Ok - we've taken one branch. We need to remove it from the list of things to look at, and work on the
                    // next one.
                    qm.ResultOperators.Remove(ro);
                }

                // The QueryModel left over needs to be added to the list.
                lst.Add(qm.Flatten());
                return lst;
            }

            /// <summary>
            /// Build a new QM given a from expression. If the from expression is just a sub-query expression,
            /// and it is "simple", then lift it.
            /// </summary>
            /// <param name="ro"></param>
            /// <param name="qm"></param>
            /// <returns></returns>
            /// <remarks>
            /// This is an optimization. From the semantic point of view, this code should not change anything.
            /// </remarks>
            private static QueryModel NewQMFromOldWithLifting(Expression fromExpr, Type itemType, string itemName)
            {
                // If this is a SQE, and it is "simple", then we can lift it.
                if (fromExpr is SubQueryExpression)
                {
                    var sqe = fromExpr as SubQueryExpression;
                    if (sqe.QueryModel.ResultOperators.Count == 0 && sqe.QueryModel.BodyClauses.Count == 0)
                    {
                        return sqe.QueryModel.Clone();
                    }
                }

                // Now simple - so we will have some trouble if we have to add on other result operators, etc.
                var mfc = new MainFromClause(itemName, itemType, fromExpr);
                var selector = new QuerySourceReferenceExpression(mfc);
                var newQM = new QueryModel(mfc, new SelectClause(selector));
                return newQM;
            }

            /// <summary>
            /// Look at the result operator to see if it has a concat embedded in it.
            /// </summary>
            /// <param name="resultOperator"></param>
            /// <param name="queryModel"></param>
            /// <param name="index"></param>
            public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
            {
                var ro = resultOperator as ConcatResultOperator;
                if (ro != null && (ro.Source2 is SubQueryExpression))
                {
                    var sq = ro.Source2 as SubQueryExpression;
                    var queryModels = SplitQMByConcatResultOperator(sq.QueryModel).ToArray();

                    if (queryModels.Length > 1)
                    {
                        // If there are more than one QM, then we need to recursively split the QM's down the line.
                        queryModels = queryModels.SelectMany(q => Split(q)).ToArray();

                        // The last QueryModel becomes the new target of the Concat result operator we are looking at.
                        ro.Source2 = new SubQueryExpression(queryModels.Last());

                        // The rest become new result operators. We put them in basically right where this one is (which
                        // has now been modified by the above line).
                        foreach (var qm in queryModels.Take(queryModels.Length - 1))
                        {
                            queryModel.ResultOperators.Insert(index + 1, new ConcatResultOperator(qm.MainFromClause.ItemName, qm.MainFromClause.ItemType, qm.MainFromClause.FromExpression));
                        }
                    }
                }

                // Default processing of this (possibly) updated result operator.
                base.VisitResultOperator(resultOperator, queryModel, index);
            }

        }

        /// <summary>
        /// Split the from clauses. This is a separate QV pattern because after we split the from
        /// clauses we have to stop processing on the query.
        /// </summary>
        private class SplitFromClauses : QueryModelVisitorBase
        {
            /// <summary>
            /// The models we end up finding
            /// </summary>
            public List<QueryModel> _models = new List<QueryModel>();

            /// <summary>
            /// The from clause can contain top level concat operators as well.
            /// </summary>
            /// <param name="fromClause"></param>
            /// <param name="queryModel"></param>
            public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
            {
                bool processed = false;

                if (fromClause.FromExpression is SubQueryExpression)
                {
                    var sq = fromClause.FromExpression as SubQueryExpression;
                    var qms = Split(sq.QueryModel);
                    if (qms.Length > 1)
                    {
                        processed = true;

                        // Find which Result operators we should remove for all but the last item.
                        var lastConcat = queryModel.ResultOperators.Reverse().Where(x => x is ConcatResultOperator).FirstOrDefault();
                        int? lastConcatIndex = lastConcat == null ? (int?) null : queryModel.ResultOperators.IndexOf(lastConcat);

                        // Create a new one for each from expression, and substitute in for the current one.
                        // Recursively process it.
                        foreach (var qSub in qms)
                        {
                            var qm = queryModel.Clone();
                            qm.MainFromClause.FromExpression = new SubQueryExpression(qSub);
                            qm.Flatten();

                            if (lastConcatIndex.HasValue && qSub != qms[qms.Length - 1])
                            {
                                for (int i = 0; i < lastConcatIndex + 1; i++)
                                {
                                    qm.ResultOperators.RemoveAt(0);
                                }
                            }

                            VisitQueryModel(qm);
                        }
                    }
                }

                if (!processed)
                {
                    base.VisitMainFromClause(fromClause, queryModel);

                    // Models that don't need further processing are "done".
                    _models.Add(queryModel);
                }
            }
        }
    }

    static class QueryModelHelpers
    {
        /// <summary>
        /// Make the creation semi-efficient.
        /// </summary>
        private static Lazy<SubQueryFromClauseFlattener> _flattener = new Lazy<SubQueryFromClauseFlattener>(() => new SubQueryFromClauseFlattener());

        /// <summary>
        /// Flatten the query model.
        /// </summary>
        /// <param name="qm"></param>
        /// <returns></returns>
        public static QueryModel Flatten(this QueryModel qm)
        {
            _flattener.Value.VisitQueryModel(qm);
            return qm;
        }
    }
}
