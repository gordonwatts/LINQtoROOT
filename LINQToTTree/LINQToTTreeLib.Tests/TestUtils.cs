using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NVelocity.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

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

            public IEnumerable<IDeclaredParameter> LoopIndexVariable
            {
                get { throw new NotImplementedException(); }
            }
        }

        /// <summary>
        /// Look through all the code, and dump out everythign to an ienumerable.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IEnumerable<string> DumpCode(this GeneratedCode code)
        {
            yield return ("Declared Variables:");
            foreach (var var in code.CodeBody.DeclaredVariables)
            {
                string initalValue = "default()";
                if (var.InitialValue != null && var.InitialValue != null)
                    initalValue = var.InitialValue.RawValue;

                yield return (var.Type.Name + " " + var.ParameterName + " = " + initalValue + ";");
            }
            yield return ("Code:");

            foreach (var line in code.CodeBody.DumpCode())
            {
                yield return line;
            }

            if (code.ResultValue == null)
            {
                yield return ("Result Variable: <not set (null)>");
            }
            else
            {
                yield return ("Result Variable: " + code.ResultValue.ToString());
            }
        }

        /// <summary>
        /// Dump the code to the console - for debugging a test...
        /// </summary>
        /// <param name="code"></param>
        public static void DumpCodeToConsole(this GeneratedCode code)
        {
            foreach (var line in code.DumpCode())
            {
                Console.WriteLine(line);
            }
        }
        /// <summary>
        /// Dump the code to the console - for debugging a test...
        /// </summary>
        /// <param name="code"></param>
        public static void DumpCodeToConsole(this IExecutableCode code)
        {
            foreach (var line in code.DumpCode())
            {
                Console.WriteLine(line);
            }
        }

        /// <summary>
        /// Look through a code block, make sure all the data structures are in order.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool CheckCodeBlock(this IExecutableCode code)
        {
            // Check parent
            foreach (var block in code.QueryCode())
            {
                var r = CheckQueryCodeBlock(block, null);
                if (!r)
                    return r;
            }
            return true;
        }

        /// <summary>
        /// Check a code block for proper linkages, etc.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool CheckCodeBlock(this GeneratedCode code)
        {
            return CheckQueryCodeBlock(code.CodeBody, null);
        }

        private static bool CheckQueryCodeBlock(IStatementCompound codeBlock, IStatement parent)
        {
            if (parent != codeBlock.Parent)
            {
                Console.WriteLine("ERROR: satement {0} does not point back to proper parent {1}", codeBlock.ToString(), parent.ToString());
                return false;
            }
            foreach (var s in codeBlock.Statements)
            {
                if (s is IStatementCompound)
                {
                    var r = CheckQueryCodeBlock(s as IStatementCompound, codeBlock);
                    if (!r)
                        return r;
                }
                else
                {
                    if (s.Parent != codeBlock)
                    {
                        Console.WriteLine("ERROR: statement {0} does not point back to proper parent {1}", s.ToString(), codeBlock.ToString());
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Dump out info from a executable code dude.
        /// </summary>
        /// <param name="code"></param>
        public static IEnumerable<string> DumpCode(this IExecutableCode code)
        {
            Console.WriteLine("There are {0} Query Blocks:", code.QueryCode().Count());
            foreach (var qb in code.QueryCode())
            {
                yield return ("Query Block:");
                foreach (var line in qb.DumpCode("  "))
                {
                    yield return line;
                };
            }

            if (code.ResultValues == null)
            {
                yield return ("Result Variable: <not set (null)>");
            }
            else
            {
                yield return string.Format("There are {0} result variables.", code.ResultValues.Count());
                foreach (var rv in code.ResultValues)
                {
                    yield return string.Format("  Result Variable: {0}", rv.RawValue);
                }
            }
        }

        /// <summary>
        /// Dump a compound statement
        /// </summary>
        /// <param name="code"></param>
        /// <param name="indent"></param>
        /// <returns></returns>
        public static IEnumerable<string> DumpCode(this IStatementCompound code, string indent = "")
        {
            if (code is IBookingStatementBlock)
            {
                var bs = code as IBookingStatementBlock;
                yield return string.Format("{0}There are {1} declared variables", indent, bs.DeclaredVariables.Count());
                foreach (var var in bs.DeclaredVariables)
                {
                    string initalValue = "default()";
                    if (var.InitialValue != null && var.InitialValue != null)
                        initalValue = var.InitialValue.RawValue;

                    yield return string.Format(indent + "  " + var.Type.Name + " " + var.ParameterName + " = " + initalValue + ";");
                }
            }
            yield return string.Format("{0}Lines of code:", indent);
            foreach (var l in code.CodeItUp())
            {
                yield return string.Format("{0}  {1}", indent, l);
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
        public static Uri CreateFileOf(string filename, Func<ROOTNET.NTTree> maker)
        {
            FileInfo result = new FileInfo(filename);
            var u = new Uri("file://" + result.FullName);
            if (result.Exists)
                return u;

            var f = new ROOTNET.NTFile(filename, "RECREATE");
            var tree = maker();
            f.Write();
            f.Close();
            return u;
        }

        /// <summary>
        /// Create an output int file... unique so we don't have to regenerate...
        /// </summary>
        /// <param name="numberOfIter"></param>
        /// <returns></returns>
        public static Uri CreateFileOfVectorInt(int numberOfIter, int vectorsize = 10)
        {
            string filename = "vectorintonly_" + numberOfIter.ToString() + ".root";
            return CreateFileOf(filename, () => TTreeParserCPPTests.CreateTrees.CreateTreeWithSimpleSingleVector(numberOfIter, vectorsize));
        }

        /// <summary>
        /// Create an output int file... unique so we don't have to regenerate...
        /// </summary>
        /// <param name="numberOfIter"></param>
        /// <returns></returns>
        public static Uri CreateFileOfVectorDouble(int numberOfIter, int vectorsize = 10)
        {
            string filename = "vectordoubleonly_" + numberOfIter.ToString() + ".root";
            return CreateFileOf(filename, () => TTreeParserCPPTests.CreateTrees.CreateTreeWithSimpleSingleDoubleVector(numberOfIter, vectorsize));
        }

        /// <summary>
        /// Create an output int file... unique so we don't have to regenerate...
        /// </summary>
        /// <param name="numberOfIter"></param>
        /// <returns></returns>
        public static Uri CreateFileOfIndexedInt(int numberOfIter)
        {
            string filename = "FileOfIndexedInt" + numberOfIter.ToString() + ".root";
            return CreateFileOf(filename, () => TTreeParserCPPTests.CreateTrees.CreateTreeWithIndexedSimpleVector(numberOfIter));
        }

        /// <summary>
        /// Create an output int file... unique so we don't have to regenerate...
        /// </summary>
        /// <param name="numberOfIter"></param>
        /// <returns></returns>
        public static Uri CreateFileOfInt(int numberOfIter)
        {
            string filename = "intonly_" + numberOfIter.ToString() + ".root";
            return CreateFileOf(filename, () => TTreeParserCPPTests.CreateTrees.CreateOneIntTree(numberOfIter));
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
        public static FileInfo GenerateROOTProxy(Uri rootFile, string rootTupleName)
        {
            ///
            /// First, load up the TTree
            /// 

            var tfile = new ROOTNET.NTFile(rootFile.LocalPath, "READ");
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

        /// <summary>
        /// Reset all the counters, etc., in the LINQ library
        /// </summary>
        public static void ResetLINQLibrary()
        {
            var a = ROOTNET.NTROOT.gROOT.GetApplication();
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;
            ArrayExpressionParser.ResetParser();
            TypeUtils._variableNameCounter = 0;
            LINQToTTreeLib.TypeHandlers.ReplacementMethodCalls.TypeHandlerReplacementCall.ClearTypeList();
            var eng = new VelocityEngine();
            eng.Init();
            QueryResultCacheTest.SetupCacheDir();
        }

        /// <summary>
        /// Return the next identifier in the string source after startpattern is found.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startpattern"></param>
        /// <returns></returns>
        public static string NextIdentifier(this string source, string startpattern)
        {
            var sindex = source.IndexOf(startpattern);
            if (sindex < 0)
                throw new Exception(string.Format("Unable to find '{0}' in string '{1}' - test failure.", startpattern, source));

            var matches = Regex.Match(source.Substring(sindex + startpattern.Length), @"\w+");
            if (!matches.Success)
                throw new Exception(string.Format("Unable to find an identifier in '{0}' after '{1}'", source, startpattern));

            return matches.Value;
        }
    }
}
