using Remotion.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// Queriable that is soley designed to extract a query model, and do no processing on the result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleTTreeExecutorQueriable<T> : QueryableBase<T>
    {
        /// <summary>
        /// Get ourselves setup!
        /// </summary>
        public SimpleTTreeExecutorQueriable(Uri[] uris, string treeName, Type outputType)
            : base(QueriableTTree<T>.CreateLINQToTTreeParser(), new TTreeQueryExecutor(uris, treeName, outputType, typeof(T)))
        {
        }

        public SimpleTTreeExecutorQueriable(IQueryProvider provider, Expression expr)
            : base(provider, expr)
        {
        }
    }
}
