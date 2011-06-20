using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// Helpers
    /// </summary>
    public static class TestUtils
    {
        /// <summary>
        /// Dump the code to the console - for debugging a test...
        /// </summary>
        /// <param name="code"></param>
        public static void DumpCodeToConsole(this IGeneratedQueryCode code)
        {
            Console.WriteLine("Declared Variables:");
            foreach (var var in code.CodeBody.DeclaredVariables)
            {
                string initalValue = "default()";
                if (var.InitialValue != null && var.InitialValue.RawValue != null)
                    initalValue = var.InitialValue.RawValue;

                Console.WriteLine(var.Type.Name + " " + var.VariableName + " = " + initalValue + ";");
            }
            Console.WriteLine("Code:");

            foreach (var l in code.CodeBody.CodeItUp())
            {
                Console.WriteLine("  " + l);
            }
            Console.WriteLine("Result Variable: ", code.ResultValue.ToString());
        }

        public static IStatementCompound GetDeepestStatementLevel(GeneratedCode target)
        {
            return GetDeepestStatementLevel(target.CodeBody);
        }

        public static IStatementCompound GetDeepestStatementLevel(IStatementCompound target)
        {
            IStatementCompound result = target;

            IStatementCompound last = result;
            while (last != null)
            {
                result = last;
                if (result.Statements == null)
                {
                    last = null;
                }
                else
                {
                    last = result.Statements.LastOrDefault() as IStatementCompound;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all files below the base directory whose name (including extension) match the regex pattern.
        /// </summary>
        /// <param name="baseDir"></param>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> FindAllFiles(this DirectoryInfo baseDir, string pattern)
        {
            var subfiles = from subdir in baseDir.EnumerateDirectories()
                           from f in subdir.FindAllFiles(pattern)
                           select f;

            Regex matcher = new Regex(pattern);
            var goodFiles = from f in baseDir.EnumerateFiles()
                            where matcher.Match(f.Name).Success
                            select f;

            var allfiles = subfiles.Concat(goodFiles);

            return allfiles;
        }

        public static IBookingStatementBlock GetDeepestBookingLevel(GeneratedCode target)
        {
            IBookingStatementBlock result = target.CodeBody;

            IBookingStatementBlock last = result;
            while (last != null)
            {
                result = last;
                last = result.Statements.LastOrDefault() as IBookingStatementBlock;
            }

            return result;
        }

        /// <summary>
        /// Create a TH1F plot from a stream of objects (with a lambda function to give flexability in conversion).
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="plotID"></param>
        /// <param name="plotTitle"></param>
        /// <param name="nbins"></param>
        /// <param name="lowBin"></param>
        /// <param name="highBin"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static ROOTNET.NTH1F Plot<TSource>
            (
            this IQueryable<TSource> source,
            string plotID, string plotTitle,
            int nbins, double lowBin, double highBin,
            Expression<Func<TSource, double>> getter)
        {
            var hParameter = Expression.Parameter(typeof(ROOTNET.NTH1F), "h");
            var vParameter = Expression.Parameter(typeof(TSource), "v");

            /// h.Fill(getter(v)) is what we want to code up

            var callGetter = Expression.Invoke(getter, vParameter);

            var fillMethod = typeof(ROOTNET.NTH1F).GetMethod("Fill", new Type[] { typeof(double) });
            var callFill = Expression.Call(hParameter, fillMethod, callGetter);

            var lambda = Expression.Lambda<Action<ROOTNET.NTH1F, TSource>>(callFill, hParameter, vParameter);
            return source.ApplyToObject(new ROOTNET.NTH1F(plotID, plotTitle, nbins, lowBin, highBin), lambda);
        }

        /// <summary>
        /// Generate a TH1F from a stream of T's that can be converted to a double.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="plotID"></param>
        /// <param name="plotTitle"></param>
        /// <param name="nbins"></param>
        /// <param name="lowBin"></param>
        /// <param name="highBin"></param>
        /// <returns></returns>
        public static ROOTNET.NTH1F Plot<T>
            (
            this IQueryable<T> source,
            string plotID, string plotTitle,
            int nbins, double lowBin, double highBin
            )
            where T : IConvertible
        {
            return source.Plot(plotID, plotTitle, nbins, lowBin, highBin, v => Convert.ToDouble(v));
        }

    }
}
