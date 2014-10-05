using LINQToTTreeLib.Expressions;
using Remotion.Linq;
using System.Text;

namespace LINQToTTreeLib.QueryVisitors
{
    /// <summary>
    /// Help with formatting a query model.
    /// </summary>
    class FormattingQueryVisitor : QueryModelVisitorBase
    {
        /// <summary>
        /// Do the required formatting for a query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string Format(QueryModel query)
        {
            if (query == null)
                return "QueryModel(null)";

            var fmt = new FormattingQueryVisitor();
            fmt.VisitQueryModel(query);

            return fmt._queryAsString.ToString();
        }

        private StringBuilder _queryAsString = new StringBuilder();

        /// <summary>
        /// Only built from above
        /// </summary>
        private FormattingQueryVisitor()
        {
        }

        public override void VisitMainFromClause(Remotion.Linq.Clauses.MainFromClause fromClause, QueryModel queryModel)
        {
            _queryAsString.AppendFormat("from {0} {1} in {2}", fromClause.ItemType.Name, fromClause.ItemName, ExpressionStringConverter.Format(fromClause.FromExpression));
        }

        public override void VisitOrdering(Remotion.Linq.Clauses.Ordering ordering, QueryModel queryModel, Remotion.Linq.Clauses.OrderByClause orderByClause, int index)
        {
            _queryAsString.AppendFormat("orderby {0} {1}", ordering.Expression.ToString(), ordering.OrderingDirection == Remotion.Linq.Clauses.OrderingDirection.Asc ? "asc" : "desc");
        }

        protected override void VisitResultOperators(System.Collections.ObjectModel.ObservableCollection<Remotion.Linq.Clauses.ResultOperatorBase> resultOperators, QueryModel queryModel)
        {
            int index = 0;
            foreach (var item in resultOperators)
            {
                _queryAsString.Append(" => ");
                VisitResultOperator(item, queryModel, index);
                index++;
            }
        }

        public override void VisitResultOperator(Remotion.Linq.Clauses.ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            bool first = true;
            _queryAsString.AppendFormat("{0}(", resultOperator.GetType().Name);
            resultOperator.TransformExpressions(e =>
            {
                string comma = ", ";
                if (first)
                {
                    comma = "";
                    first = false;
                }
                _queryAsString.AppendFormat("{0}{1}", comma, ExpressionStringConverter.Format(e));
                return e;
            });
            _queryAsString.Append(")");
        }

        public override void VisitSelectClause(Remotion.Linq.Clauses.SelectClause selectClause, QueryModel queryModel)
        {
            _queryAsString.AppendFormat(" select {0}", ExpressionStringConverter.Format(selectClause.Selector));
        }

        public override void VisitWhereClause(Remotion.Linq.Clauses.WhereClause whereClause, QueryModel queryModel, int index)
        {
            _queryAsString.AppendFormat(" where {0}", ExpressionStringConverter.Format(whereClause.Predicate));
        }

        public override void VisitAdditionalFromClause(Remotion.Linq.Clauses.AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            _queryAsString.AppendFormat(" from {0} {1} in {2}",
                fromClause.ItemType.Name,
                fromClause.ItemName,
                ExpressionStringConverter.Format(fromClause.FromExpression));
        }
    }
}
