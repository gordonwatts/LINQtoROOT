using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// Extension methods that can be added to the end of
    /// sequences to dump them into files.
    /// </summary>
    public static class FileHelperQueryExtensions
    {
        /// <summary>
        /// A given sequence is turned into a CSV file with a single column.
        /// </summary>
        /// <param name="source">Sequence of double's to save to the CSV file</param>
        /// <param name="outputFile">The file to be written. Will be deleted if already present.</param>
        /// <param name="columnHeader">Header to be used in the output file.</param>
        public static FileInfo AsCSV (this IQueryable<double> source, FileInfo outputFile, string columnHeader)
        {
            // Translate into an expression call
            return source.Provider.Execute(
                Expression.Call(
                    (MethodInfo)MethodBase.GetCurrentMethod(),
                    source.Expression, Expression.Constant(outputFile), Expression.Constant(columnHeader)))
                    as FileInfo;
        }
    }
}
