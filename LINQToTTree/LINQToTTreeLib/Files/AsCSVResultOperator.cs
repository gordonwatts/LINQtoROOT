using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq.Clauses.StreamedData;
using System.Linq.Expressions;
using System.IO;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// This class represents the result operator in the QueryModel for AsCSV.
    /// </summary>
    /// <remarks>
    /// Following the pattern seen here: https://www.re-motion.org/blogs/mix/2010/10/28/re-linq-extensibility-custom-query-operators
    /// </remarks>
    class AsCSVResultOperator : ValueFromSequenceResultOperatorBase
    {
        /// <summary>
        /// Initalize the result operator with the appropriate items.
        /// </summary>
        /// <param name="outputfile"></param>
        /// <param name="headerColumnTitle"></param>
        public AsCSVResultOperator(FileInfo outputfile, string headerColumnTitle)
        {
            OutputFile = outputfile;
        }
        
        /// <summary>
        /// Get the output file we are to write to.
        /// </summary>
        public FileInfo OutputFile { get; private set; }

        /// <summary>
        /// Called by re-linq when cloning is required.
        /// </summary>
        /// <param name="cloneContext"></param>
        /// <returns></returns>
        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Run the operation in memory.
        /// WARNING: This is not supported, and will bomb.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public override StreamedValue ExecuteInMemory<T>(StreamedSequence sequence)
        {
            throw new NotImplementedException("The result operator AsCSV can't be executed in memory currently.");
        }

        /// <summary>
        /// Return the type that we will be returning.
        /// </summary>
        /// <param name="inputInfo"></param>
        /// <returns></returns>
        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            return new StreamedScalarValueInfo(typeof(FileInfo));
        }

        /// <summary>
        /// Make sure that all the expressions we are holding onto are properly translated.
        /// </summary>
        /// <param name="transformation"></param>
        /// <remarks>
        /// We aren't actually holding onto anything - everything is fixed. So, no translation is required!
        /// </remarks>
        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
        }
    }
}
