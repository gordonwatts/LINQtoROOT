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
        public static FileInfo AsCSV (this IQueryable<double> source, FileInfo outputFile, string columnHeader)
        {
            // Translate into an expression call
            return source.Provider.Execute(
                Expression.Call(
                    (MethodInfo)MethodBase.GetCurrentMethod(),
                    source.Expression, Expression.Constant(outputFile), Expression.Constant(columnHeader)))
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
        public static IFutureValue<FileInfo> FutureAsCSV(this IQueryable<double> source, FileInfo outputFile, string columnHeader)
        {
            var q = FutureResultOperators.CheckSource<double>(source);

            // Build up the AsCSV expression.

            var countMethod = typeof(FileHelperQueryExtensions).GetMethods().Where(m => m.Name == "AsCSV").Where(m => m.GetParameters().Length == 3 && m.GetParameters()[0].ParameterType.Name == "IQueryable<double>").First();
            var expr = Expression.Call(null, countMethod, q.Expression);

            return FutureResultOperators.FutureExecuteScalarHelper<double, FileInfo>(q, expr);
        }

        /// <summary>
        /// Do the translation for a custom class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="outputFile"></param>
        /// <param name="columnHeader"></param>
        /// <returns></returns>
        public static FileInfo AsCSV<T>(this IQueryable<T> source, FileInfo outputFile)
            where T : class
        {
            // Translate into an expression call
            return source.Provider.Execute(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T)),
                    source.Expression, Expression.Constant(outputFile)))
                    as FileInfo;
        }

        /// <summary>
        /// Do a future for a custom object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static IFutureValue<FileInfo> FutureAsCSV<T>(this IQueryable<T> source, FileInfo outputFile)
        {
            var q = FutureResultOperators.CheckSource<T>(source);

            // Build up the AsCSV expression.

            var countMethodGeneric = typeof(FileHelperQueryExtensions).GetMethods().Where(m => m.Name == "AsCSV").Where(m => m.GetParameters().Length == 2).First();
            var countMethod = countMethodGeneric.MakeGenericMethod(typeof(T));
            var expr = Expression.Call(null, countMethod, q.Expression);

            return FutureResultOperators.FutureExecuteScalarHelper<T, FileInfo>(q, expr);
        }

        /// <summary>
        /// Turn a list of tuples into a csv file.
        /// WARNING: Only tuple tupes like int and double can be used as column values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="outputFile"></param>
        /// <param name="columnHeader"></param>
        /// <returns></returns>
        public static FileInfo AsCSV<T1, T2>(this IQueryable<Tuple<T1, T2>> source, FileInfo outputFile, string item1Header, string item2Header)
        {
            // Translate into an expression call
            return source.Provider.Execute(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T1), typeof(T2)),
                    source.Expression, Expression.Constant(outputFile), Expression.Constant(item1Header), Expression.Constant(item2Header)))
                    as FileInfo;
        }
        public static FileInfo AsCSV<T1, T2, T3>(this IQueryable<Tuple<T1, T2, T3>> source, FileInfo outputFile, string item1Header, string item2Header, string item3Header)
        {
            // Translate into an expression call
            return source.Provider.Execute(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3)),
                    source.Expression, Expression.Constant(outputFile), Expression.Constant(item1Header), Expression.Constant(item2Header), Expression.Constant(item3Header)))
                    as FileInfo;
        }
        public static FileInfo AsCSV<T1, T2, T3, T4>(this IQueryable<Tuple<T1, T2, T3, T4>> source, FileInfo outputFile, string item1Header, string item2Header, string item3Header, string item4Header)
        {
            // Translate into an expression call
            return source.Provider.Execute(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4)),
                    source.Expression, Expression.Constant(outputFile), Expression.Constant(item1Header), Expression.Constant(item2Header), Expression.Constant(item3Header), Expression.Constant(item4Header)))
                    as FileInfo;
        }
        public static FileInfo AsCSV<T1, T2, T3, T4, T5>(this IQueryable<Tuple<T1, T2, T3, T4, T5>> source, FileInfo outputFile, string item1Header, string item2Header, string item3Header, string item4Header, string item5Header)
        {
            // Translate into an expression call
            return source.Provider.Execute(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)),
                    source.Expression, Expression.Constant(outputFile), Expression.Constant(item1Header), Expression.Constant(item2Header), Expression.Constant(item3Header), Expression.Constant(item4Header), Expression.Constant(item5Header)))
                    as FileInfo;
        }
        public static FileInfo AsCSV<T1, T2, T3, T4, T5, T6>(this IQueryable<Tuple<T1, T2, T3, T4, T5, T6>> source, FileInfo outputFile, string item1Header, string item2Header, string item3Header, string item4Header, string item5Header, string item6Header)
        {
            // Translate into an expression call
            return source.Provider.Execute(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)),
                    source.Expression, Expression.Constant(outputFile), Expression.Constant(item1Header), Expression.Constant(item2Header), Expression.Constant(item3Header), Expression.Constant(item4Header), Expression.Constant(item5Header), Expression.Constant(item6Header)))
                    as FileInfo;
        }
        public static FileInfo AsCSV<T1, T2, T3, T4, T5, T6, T7>(this IQueryable<Tuple<T1, T2, T3, T4, T5, T6, T7>> source, FileInfo outputFile, string item1Header, string item2Header, string item3Header, string item4Header, string item5Header, string item6Header, string item7Header)
        {
            // Translate into an expression call
            return source.Provider.Execute(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)),
                    source.Expression, Expression.Constant(outputFile), Expression.Constant(item1Header), Expression.Constant(item2Header), Expression.Constant(item3Header), Expression.Constant(item4Header), Expression.Constant(item5Header), Expression.Constant(item6Header), Expression.Constant(item7Header)))
                    as FileInfo;
        }
    }
}
