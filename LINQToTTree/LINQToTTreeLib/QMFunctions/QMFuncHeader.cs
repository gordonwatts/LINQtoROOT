
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

        /// <summary>
        /// The text QM translated. Cache it since it is fairly expensive to do.
        /// </summary>
        public string QMText { get; set; }

        /// <summary>
        /// This QM represents a sequence (e.g. it ends with a "select" rather than a First()
        /// or similar.
        /// </summary>
        public bool IsSequence { get; set; }
    }
}
