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
    public class QMExtractorQueriable<T> : QueryableBase<T>
    {
        /// <summary>
        /// Get ourselves setup!
        /// </summary>
        public QMExtractorQueriable()
            : base(QueriableTTree<T>.CreateLINQToTTreeParser(), new QMExtractorExecutor(typeof(T)))
        {
        }

        public QMExtractorQueriable(IQueryProvider provider, Expression expr)
            : base(provider, expr)
        {
        }
    }

    class QMExtractorExecutor : IQueryExecutor
    {
        private Type type;

        /// <summary>
        /// Get the last qm that was sent to us.
        /// </summary>
        public static QueryModel LastQM { get; private set; }

        public QMExtractorExecutor(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Collections are not supported.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// We do nothing but return the default and save the QM.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            LastQM = queryModel;
            return default(T);
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            throw new NotImplementedException();
        }
    }
}
