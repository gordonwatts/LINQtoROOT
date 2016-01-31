using LinqToTTreeInterfacesLib;
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
        public static FileInfo AsCSV<T> (this IQueryable<T> source, FileInfo outputFile, params string[] columnHeaders)
        {
            // Translate into an expression call
            return source.Provider.Execute(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T)),
                    source.Expression, Expression.Constant(outputFile), Expression.Constant(columnHeaders)))
                    as FileInfo;
        }

        /// <summary>
        /// Run a future for the simple double AsCSV.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="outputFile"></param>
        /// <param name="columnHeader"></param>
        /// <returns></returns>
        public static IFutureValue<FileInfo> FutureAsCSV<T>(this IQueryable<T> source, FileInfo outputFile, params string[] columnHeader)
        {
            var q = FutureResultOperators.CheckSource<T>(source);

            // Build up the AsCSV expression.

            var countMethodGeneric = typeof(FileHelperQueryExtensions).GetMethods().Where(m => m.Name == "AsCSV").First();
            var countMethod = countMethodGeneric.MakeGenericMethod(typeof(T));
            var expr = Expression.Call(null, countMethod, q.Expression);

            return FutureResultOperators.FutureExecuteScalarHelper<T, FileInfo>(q, expr);
        }
    }
}
