using Remotion.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// The queriable object we will use to drive this testing!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueriableDummy<T> : QueryableBase<T>
    {
        /// <summary>
        /// Get ourselves setup!
        /// </summary>
        public QueriableDummy()
            : base(QueriableTTree<T>.CreateLINQToTTreeParser(), new DummyQueryExectuor(typeof(T)))
        {
        }

        public QueriableDummy(IQueryProvider provider, Expression expr)
            : base(provider, expr)
        {
        }

        public bool DOQueryFunctions
        {
            get { return ((base.Provider as DefaultQueryProvider).Executor as DummyQueryExectuor).DoQMFunctions; }
            set { ((base.Provider as DefaultQueryProvider).Executor as DummyQueryExectuor).DoQMFunctions = value; }
        }
    }
}
