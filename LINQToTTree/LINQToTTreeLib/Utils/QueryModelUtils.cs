using LINQToTTreeLib.Utils;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
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
    }
}
