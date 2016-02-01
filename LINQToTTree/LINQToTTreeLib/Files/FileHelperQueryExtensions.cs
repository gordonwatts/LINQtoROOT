﻿using LinqToTTreeInterfacesLib;
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
        /// A given sequence is turned into a CSV file with a single column. The sequence can be one of int's, doubles,
        /// tuples made up of int's or double, or a custom object that contains ints or doubles. In the latter case
        /// the column names are pulled from the field names. Otherwise you should specify them in the column headers.
        /// </summary>
        /// <param name="source">Sequence to save to the file</param>
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
        /// A given sequence is turned into a CSV file with a single column. The sequence can be one of int's, doubles,
        /// tuples made up of int's or double, or a custom object that contains ints or doubles. In the latter case
        /// the column names are pulled from the field names. Otherwise you should specify them in the column headers.
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

        /// <summary>
        /// A given sequence is turned into a TTree in a ROOT file. The sequence can be one of int's, doubles,
        /// tuples made up of int's or double, or a custom object that contains ints or doubles. In the latter case
        /// the column names are pulled from the field names. Otherwise you should specify them in the column headers.
        /// </summary>
        /// <typeparam name="T">Sequence type. Can be int, double, Tuple (of int or double), or POCO (of int or double)</typeparam>
        /// <param name="source">Sequence of double's to save to the TTree stored in a ROOT file</param>
        /// <param name="outputROOTFile">The file to be written. Will be deleted if already present.</param>
        /// <param name="columnHeader">Header to be used in the leaves.</param>
        /// <returns>A FileInfo that points to the resulting ROOT file.</FileInfo></returns>
        /// <remarks>
        /// Because TTree's can get large, we don't hold this in memory. Rather the TTree is attached to the
        /// specified ROOT file. The file is also not cached. Becareful about modifying it - if it is modified,
        /// the framework will almost certianly detect it, and cause it to be regenerated.
        /// </remarks>
        public static FileInfo AsTTree<T>(this IQueryable<T> source, FileInfo outputROOTFile, params string[] leaveHeaders)
        {
            // Translate into an expression call
            return source.Provider.Execute(
                Expression.Call(
                    ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(T)),
                    source.Expression, Expression.Constant(outputROOTFile), Expression.Constant(leaveHeaders)))
                    as FileInfo;
        }

        /// <summary>
        /// A given sequence is turned into a TTree in a ROOT file. The sequence can be one of int's, doubles,
        /// tuples made up of int's or double, or a custom object that contains ints or doubles. In the latter case
        /// the column names are pulled from the field names. Otherwise you should specify them in the column headers.
        /// </summary>
        /// <typeparam name="T">Sequence type. Can be int, double, Tuple (of int or double), or POCO (of int or double)</typeparam>
        /// <param name="source"></param>
        /// <param name="outputFile"></param>
        /// <param name="columnHeader"></param>
        /// <returns>A Future<FileInfo> that points to the resulting ROOT file.</FileInfo></returns>
        /// Because TTree's can get large, we don't hold this in memory. Rather the TTree is attached to the
        /// specified ROOT file. The file is also not cached. Becareful about modifying it - if it is modified,
        /// the framework will almost certianly detect it, and cause it to be regenerated.
        public static IFutureValue<FileInfo> FutureAsTTree<T>(this IQueryable<T> source, FileInfo outputFile, params string[] columnHeader)
        {
            var q = FutureResultOperators.CheckSource<T>(source);

            // Build up the AsCSV expression.

            var countMethodGeneric = typeof(FileHelperQueryExtensions).GetMethods().Where(m => m.Name == "AsTTree").First();
            var countMethod = countMethodGeneric.MakeGenericMethod(typeof(T));
            var expr = Expression.Call(null, countMethod, q.Expression);

            return FutureResultOperators.FutureExecuteScalarHelper<T, FileInfo>(q, expr);
        }
    }
}
