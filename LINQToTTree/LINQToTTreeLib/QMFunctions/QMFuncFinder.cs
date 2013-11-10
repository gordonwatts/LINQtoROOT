
using Remotion.Linq;
using System.Collections.Generic;
namespace LINQToTTreeLib.QMFunctions
{
    /// <summary>
    /// Look at a QueryModel and return all the functions that we will want to cache,
    /// along with their arguments.
    /// </summary>
    class QMFuncFinder
    {
        /// <summary>
        /// Scan the QM for all header functions.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IEnumerable<QMFuncHeader> FindQMFunctions(QueryModel model)
        {
            var qmf = new QMFuncVisitor();
            qmf.VisitQueryModel(model);
            return qmf.FoundFunctions;
        }

        /// <summary>
        /// Internal class that does the QM visiting.
        /// </summary>
        private class QMFuncVisitor : QueryModelVisitorBase
        {
            /// <summary>
            /// Get the class setup to look for QM functions.
            /// </summary>
            public QMFuncVisitor()
            {
                FoundFunctions = new List<QMFuncHeader>();
            }

            /// <summary>
            /// The list of QM headers that we have found in this query model.
            /// </summary>
            public List<QMFuncHeader> FoundFunctions { get; set; }
        }
    }

}
