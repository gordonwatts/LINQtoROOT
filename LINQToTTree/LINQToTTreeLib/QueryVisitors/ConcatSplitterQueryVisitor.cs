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

namespace LINQToTTreeLib.QueryVisitors
{
    /// <summary>
    /// Scan a qm for "Concat" result operators. Split the query each time we find a concat operator to produce multiple
    /// concat operators.
    /// </summary>
    public class ConcatSplitterQueryVisitor : QueryModelVisitorBase
    {
        /// <summary>
        /// Look at the QM and find all Concats - split the query into multiple queries.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static QueryModel[] Split(QueryModel source)
        {
            var qv = new ConcatSplitterQueryVisitor();

            // Run the model until we've taken all the concat branches.
            qv.VisitQueryModel(source);

            return qv._allModels.ToArray();
        }

        /// <summary>
        /// Protected ctor to make sure that we run this from a controled environment.
        /// </summary>
        protected ConcatSplitterQueryVisitor()
        { }

        /// <summary>
        /// Accumulate the various choices after removing the Concat result operators.
        /// </summary>
        private List<QueryModel> _allModels = new List<QueryModel>();

        /// <summary>
        /// Look at all the result operators.
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
        /// into seperate query models.
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private static IEnumerable<QueryModel> SplitQMByConcatResultOperator(QueryModel queryModel)
        {
            // If there are no concat result operators in the list, then we just bail quickly.
            // This is to specifically avoid the Clone operation.
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
                var newQM = qm.Clone();
                var indexToRemoveTo = qm.ResultOperators.IndexOf(ro);
                for (int i = 0; i < indexToRemoveTo + 1; i++)
                {
                    newQM.ResultOperators.RemoveAt(0);
                }
                newQM.MainFromClause.FromExpression = ro.Source2;
                newQM.SelectClause.Selector = new QuerySourceReferenceExpression(newQM.MainFromClause);
                newQM.BodyClauses.Clear();
                lst.Add(newQM);

                // Ok - we've taken one branch. We need to remove it from the list of things to look at, and work on the
                // next one.
                qm.ResultOperators.Remove(ro);
            }

            // The qm left over needs to be added to the list.
            // TODO: add code to prevent the ".Clone()" in the case that the QM has no Concat operators?
            lst.Add(qm);
            return lst;
        }

        /// <summary>
        /// Look at the result operator to see if it has a concat embeded in it.
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

                // if there are seperate query models, then the first N-1 become result operators to our QM,
                // and the last one remains as the sub expression for this one.
                // TODO: bad refactorying - SplitQMByConcatREsultOperator needs to return a list of expressions.
                //       leave for now till we understand how to do other result operators between the Concat RO's!
                ro.Source2 = new SubQueryExpression(queryModels.Last());
                foreach (var qm in queryModels.Take(queryModels.Length-1))
                {
                    queryModel.ResultOperators.Insert(index+1, new ConcatResultOperator(qm.MainFromClause.FromExpression));
                }
            }
            base.VisitResultOperator(resultOperator, queryModel, index);
        }
    }
}
