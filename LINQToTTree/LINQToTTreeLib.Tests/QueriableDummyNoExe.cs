using Remotion.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// A queriable class that won't actually run the execution. Useful, really, only
    /// if you want to get at the query model, and no more.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueriableDummyNoExe<T> : QueryableBase<T>
    {
        /// <summary>
        /// Get ourselves setup!
        /// </summary>
        public QueriableDummyNoExe()
            : base(QueriableTTree<T>.CreateLINQToTTreeParser(), new DummyQueryExectuor(typeof(T), false))
        {
        }

        public QueriableDummyNoExe(IQueryProvider provider, Expression expr)
            : base(provider, expr)
        {
        }
    }
}
