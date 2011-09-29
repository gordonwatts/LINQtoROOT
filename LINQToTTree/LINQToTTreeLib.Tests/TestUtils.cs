using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// Helpers
    /// </summary>
    public static class TestUtils
    {
        /// <summary>
        /// Dummy loop to help with tests below.
        /// </summary>
        public class SimpleLoop : LINQToTTreeLib.Statements.StatementInlineBlockBase, IStatementLoop
        {

            public override IEnumerable<string> CodeItUp()
            {
                yield return "Dummyloop {";
                foreach (var l in RenderInternalCode())
                {
                    yield return "  " + l;
                }
                yield return "}";
            }

            public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
            {
                throw new NotImplementedException();
            }

            public override void RenameVariable(string origName, string newName)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Dump the code to the console - for debugging a test...
        /// </summary>
        /// <param name="code"></param>
        public static void DumpCodeToConsole(this GeneratedCode code)
        {
            Console.WriteLine("Declared Variables:");
            foreach (var var in code.CodeBody.DeclaredVariables)
            {
                string initalValue = "default()";
                if (var.InitialValue != null && var.InitialValue != null)
                    initalValue = var.InitialValue.RawValue;

                Console.WriteLine(var.Type.Name + " " + var.ParameterName + " = " + initalValue + ";");
            }
            Console.WriteLine("Code:");

            code.CodeBody.DumpCodeToConsole();

            if (code.ResultValue == null)
            {
                Console.WriteLine("Result Variable: <not set (null)>");
            }
            else
            {
                Console.WriteLine("Result Variable: ", code.ResultValue.ToString());
            }
        }

        /// <summary>
        /// Dump out info from a executable code dude.
        /// </summary>
        /// <param name="code"></param>
        public static void DumpCodeToConsole(this IExecutableCode code)
        {
            Console.WriteLine("There are {0} Query Blocks:", code.QueryCode().Count());
            foreach (var qb in code.QueryCode())
            {
                Console.WriteLine("Query Block:");
                qb.DumpCodeToConsole("  ");
            }

            if (code.ResultValues == null)
            {
                Console.WriteLine("Result Variable: <not set (null)>");
            }
            else
            {
                Console.WriteLine("There are {0} result variables.", code.ResultValues.Count());
                foreach (var rv in code.ResultValues)
                {
                    Console.WriteLine("  Result Variable: {0}", rv.RawValue);
                }
            }
        }

        /// <summary>
        /// Dump some code to the console for debugging.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="indent"></param>
        public static void DumpCodeToConsole(this IStatementCompound code, string indent = "")
        {
            if (code is IBookingStatementBlock)
            {
                var bs = code as IBookingStatementBlock;
                Console.WriteLine("{0}There are {1} declared variables", indent, bs.DeclaredVariables.Count());
                foreach (var var in bs.DeclaredVariables)
                {
                    string initalValue = "default()";
                    if (var.InitialValue != null && var.InitialValue != null)
                        initalValue = var.InitialValue.RawValue;

                    Console.WriteLine(indent + "  " + var.Type.Name + " " + var.ParameterName + " = " + initalValue + ";");
                }
            }
            Console.WriteLine("{0}Lines of code:", indent);
            foreach (var l in code.CodeItUp())
            {
                Console.WriteLine("{0}  {1}", indent, l);
            }
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
            var seed = new ROOTNET.NTH1F(plotID, plotTitle, nbins, lowBin, highBin);
            seed.Directory = null;
            return source.ApplyToObject(seed, lambda);
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

        /// <summary>
        /// Create an output int file... unique so we don't have to regenerate...
        /// </summary>
        /// <param name="numberOfIter"></param>
        /// <returns></returns>
        public static FileInfo CreateFileOfVectorInt(int numberOfIter, int vectorsize = 10)
        {
            string filename = "vectorintonly_" + numberOfIter.ToString() + ".root";
            FileInfo result = new FileInfo(filename);
            if (result.Exists)
                return result;

            var f = new ROOTNET.NTFile(filename, "RECREATE");
            var tree = TTreeParserCPPTests.CreateTrees.CreateTreeWithSimpleSingleVector(numberOfIter, vectorsize);
            f.Write();
            f.Close();
            result.Refresh();
            return result;
        }

        /// <summary>
        /// Dirt simply test ntuple. Actually matches one that exists on disk.
        /// </summary>
        public class TestNtupeArr
        {
#pragma warning disable 0169
            public int[] myvectorofint;
#pragma warning restore 0169
        }

        /// <summary>
        /// Given the root file and the root-tuple name, generate a proxy file 
        /// </summary>
        /// <param name="rootFile"></param>
        /// <returns></returns>
        public static FileInfo GenerateROOTProxy(FileInfo rootFile, string rootTupleName)
        {
            ///
            /// First, load up the TTree
            /// 

            var tfile = new ROOTNET.NTFile(rootFile.FullName, "READ");
            var tree = tfile.Get(rootTupleName) as ROOTNET.Interface.NTTree;
            Assert.IsNotNull(tree, "Tree couldn't be found");

            ///
            /// Create the proxy sub-dir if not there already, and put the dummy macro in there
            /// 

            using (var w = File.CreateText("junk.C"))
            {
                w.Write("int junk() {return 10.0;}");
                w.Close();
            }

            ///
            /// Create the macro proxy now
            /// 

            tree.MakeProxy("scanner", "junk.C", null, "nohist");
            return new FileInfo("scanner.h");
        }
    }
}
