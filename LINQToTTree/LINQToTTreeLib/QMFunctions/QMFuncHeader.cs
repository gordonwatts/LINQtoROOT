
using System.Collections.Generic;
namespace LINQToTTreeLib.QMFunctions
{
    /// <summary>
    /// The specification for a QM Function.
    /// </summary>
    public class QMFuncHeader
    {
        public QMFuncHeader()
        {
            QM = null;
            Arguments = new List<object>();
        }
        /// <summary>
        /// The query model this function implements
        /// </summary>
        public Remotion.Linq.QueryModel QM { get; set; }

        /// <summary>
        /// The list of arguments that have to be passed in so that this guy can work.
        /// </summary>
        public IEnumerable<object> Arguments { get; set; }

        public string QMText { get; set; }
    }
}
