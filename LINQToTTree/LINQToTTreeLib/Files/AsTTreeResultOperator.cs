using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using System;
using System.IO;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// This class represents the result operator in the QueryModel for TTree ROOT file.
    /// </summary>
    /// <remarks>
    /// Following the pattern seen here: https://www.re-motion.org/blogs/mix/2010/10/28/re-linq-extensibility-custom-query-operators
    /// </remarks>
    class AsTTreeResultOperator : AsFileResultOperator
    {
        /// <summary>
        /// Initalize the result operator with the appropriate items.
        /// </summary>
        /// <param name="outputfile"></param>
        /// <param name="headerColumnTitle"></param>
        public AsTTreeResultOperator(FileInfo outputfile, string[] headerColumnTitle)
            : base(outputfile, headerColumnTitle)
        {
        }

        /// <summary>
        /// Clone the operator
        /// </summary>
        /// <param name="cloneContext"></param>
        /// <returns></returns>
        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new AsTTreeResultOperator(OutputFile, HeaderColumns);
        }
    }
}
