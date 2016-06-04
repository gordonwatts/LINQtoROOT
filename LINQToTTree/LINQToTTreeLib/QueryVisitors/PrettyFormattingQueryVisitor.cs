using LINQToTTreeLib.Expressions;
using Remotion.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.QueryVisitors
{
    /// <summary>
    /// Pretty print (for human readablity) a big fat query model. Do our best!
    /// </summary>
    class PrettyFormattingQueryVisitor : QueryModelVisitorBase
    {
        /// <summary>
        /// Do the formatting
        /// </summary>
        /// <param name="qm"></param>
        /// <returns></returns>
        public static string Format (QueryModel qm)
        {
            var f = new PrettyFormattingQueryVisitor();
            f.VisitQueryModel(qm);
            return f._queryAsString.ToString();
        }

        /// <summary>
        /// The result of the build
        /// </summary>
        private StringBuilder _queryAsString = new StringBuilder();

        /// <summary>
        /// Do the from clause, one item to a line.
        /// </summary>
        /// <param name="fromClause"></param>
        /// <param name="queryModel"></param>
        public override void VisitMainFromClause(Remotion.Linq.Clauses.MainFromClause fromClause, QueryModel queryModel)
        {
            _queryAsString.AppendLine($"from {fromClause.ItemType.Name} {fromClause.ItemName} in {ExpressionStringConverter.Format(fromClause.FromExpression)}");
        }

        /// <summary>
        /// Deal with an order by, on a seperate line.
        /// </summary>
        /// <param name="ordering"></param>
        /// <param name="queryModel"></param>
        /// <param name="orderByClause"></param>
        /// <param name="index"></param>
        public override void VisitOrdering(Remotion.Linq.Clauses.Ordering ordering, QueryModel queryModel, Remotion.Linq.Clauses.OrderByClause orderByClause, int index)
        {
            var d = ordering.OrderingDirection == Remotion.Linq.Clauses.OrderingDirection.Asc ? "asc" : "desc";
            _queryAsString.AppendLine($"orderby {ExpressionStringConverter.Format(ordering.Expression)} {d}");
        }

        /// <summary>
        /// Put the result operators in there, one to a line.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
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
            _queryAsString.AppendLine(")");
        }

        /// <summary>
        /// Do the select clause, one to a line.
        /// </summary>
        /// <param name="selectClause"></param>
        /// <param name="queryModel"></param>
        public override void VisitSelectClause(Remotion.Linq.Clauses.SelectClause selectClause, QueryModel queryModel)
        {
            _queryAsString.AppendLine($"select {ExpressionStringConverter.Format(selectClause.Selector)}");
        }

        /// <summary>
        /// A where clause, one to a line.
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitWhereClause(Remotion.Linq.Clauses.WhereClause whereClause, QueryModel queryModel, int index)
        {
            _queryAsString.AppendLine($"where {ExpressionStringConverter.Format(whereClause.Predicate)}");
        }

        /// <summary>
        /// More from clauses
        /// </summary>
        /// <param name="fromClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitAdditionalFromClause(Remotion.Linq.Clauses.AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            _queryAsString.AppendLine($"from {fromClause.ItemType.Name} {fromClause.ItemName} in {ExpressionStringConverter.Format(fromClause.FromExpression)}");
        }

    }
}
