using LINQToTTreeLib.Utils;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Utils
{
    static class QueryModelUtils
    {
        /// <summary>
        /// Given a query model, determine who the provider is.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static IQueryProvider FindQueryProvider(this QueryModel q)
        {
            var fromExpression = q.MainFromClause.FromExpression;
            while (fromExpression is SubQueryExpression)
            {
                fromExpression = (fromExpression as SubQueryExpression).QueryModel.MainFromClause.FromExpression;
            }

            var cVal = (fromExpression as ConstantExpression)
                ?.Value as IQueryable;
            return cVal?.Provider;
        }

        /// <summary>
        /// Starting from this one, find all query models that are nested, limited to those in the MainFromClause.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static IEnumerable<QueryModel> QMAllMainOnly(this QueryModel q)
        {
            while (q != null)
            {
                yield return q;
                q = (q.MainFromClause.FromExpression as SubQueryExpression)?.QueryModel;
            }
        }

        /// <summary>
        /// See if the QM has a stateful operator or not (like Take/Skip).
        /// Used to determine if this QM can be run on multiple files, or not.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static bool HasStatefulOperator(this QueryModel q)
        {
            // Look to see if there is a take burried in the from expression
            if (IsStatefulSequence(q.MainFromClause.FromExpression))
            {
                return true;
            }

            // If this top level QM has a take/skip, then done.
            return QMHasStatefulResultOperator(q);
        }

        /// <summary>
        /// Is this sequence stateful?
        /// </summary>
        /// <param name="fromExpression"></param>
        /// <returns></returns>
        private static bool IsStatefulSequence(Expression fromExpression)
        {
            if (fromExpression is SubQueryExpression sq)
            {
                if (IsStatefulSequence(sq.QueryModel.MainFromClause.FromExpression))
                {
                    return true;
                }
                return QMHasStatefulResultOperator(sq.QueryModel);
            }
            return false;
        }

        /// <summary>
        /// Check to see if there is a stateful query operator.
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private static bool QMHasStatefulResultOperator(QueryModel queryModel)
        {
            return HasResultOperator(queryModel, typeof(TakeResultOperator))
                || HasResultOperator(queryModel, typeof(SkipResultOperator));
        }

        /// <summary>
        /// Look to see if the result operators contain a particular type
        /// </summary>
        /// <param name="queryModel"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool HasResultOperator(QueryModel queryModel, Type type)
        {
            return queryModel.ResultOperators
                .Where(ro => ro.GetType() == type)
                .Any();
        }

    }
}
