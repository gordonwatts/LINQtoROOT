using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NVelocity.App;
using Remotion.Linq;
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

            /// <summary>
            /// Return the index variables for this loop.
            /// </summary>
            public override IEnumerable<IDeclaredParameter> InternalResultVarialbes
            {
                get
                {
                    return new IDeclaredParameter[] { };
                }
            }

            public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
            {
                throw new NotImplementedException();
            }

            public override void RenameVariable(string origName, string newName)
            {
                throw new NotImplementedException();
            }

            public override bool CommutesWithGatingExpressions(ICMStatementInfo followStatement)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IDeclaredParameter> LoopIndexVariable
            {
                get { throw new NotImplementedException(); }
            }

            public override bool AllowNormalBubbleUp
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Look through all the code, and dump out everything to an IEnumerable.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IEnumerable<string> DumpCode(this GeneratedCode code, bool dumpQM = true)
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
            yield return ("");

            foreach (var f in code.QMFunctions)
            {
                yield return (string.Format("Function: {0}", f.Name));
                if (dumpQM)
                    yield return (string.Format("  -> QM: {0}", f.QueryModelText));
                yield return (string.Format("  {0} {1} ()", f.ResultType, f.Name));
                if (f.StatementBlock != null)
                {
                    foreach (var line in f.StatementBlock.DumpCode())
                    {
                        yield return string.Format("  {0}", line);
                    }
                }
                else
                {
                    yield return "  ** No statements ever set";
                }
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
        /// Given a QM, turn it into code, and then dump it.
        /// </summary>
        /// <param name="qm"></param>
        /// <returns></returns>
        public static IEnumerable<string> DumpCode<T>(this QueryModel qm)
        {
            var qp = (qm.FindQueryProvider() as DefaultQueryProvider)?.Executor as DummyQueryExectuor;
            Assert.IsNotNull(qp);

            var r = qp.ExecuteScalar<T>(qm);

            return DummyQueryExectuor
                .FinalResult
                .DumpCode();
        }

        /// <summary>
        /// Given a QM, turn it into code, and then dump it.
        /// </summary>
        /// <param name="qm"></param>
        /// <returns></returns>
        public static GeneratedCode GenerateCode<T>(this QueryModel qm)
        {
            var qp = (qm.FindQueryProvider() as DefaultQueryProvider)?.Executor as DummyQueryExectuor;
            Assert.IsNotNull(qp, "We need the dummy query executor for this to work otherwise we can't get at the query!");

            var r = qp.ExecuteScalar<T>(qm);

            return DummyQueryExectuor
                .FinalResult;
        }

        internal static IEnumerable<string> DumpCode(this IStatement block)
        {
            return block.CodeItUp();
        }

        /// <summary>
        /// Dump the code to the console - for debugging a test...
        /// </summary>
        /// <param name="code"></param>
        public static void DumpCodeToConsole(this GeneratedCode code)
        {
            code.DumpCode().DumpToConsole();
        }

        public static void DumpCodeToConsole(this IBookingStatementBlock code)
        {
            code.DumpCode().DumpToConsole();
        }

        /// <summary>
        /// Dump lines to the console.
        /// </summary>
        /// <param name="lines"></param>
        public static void DumpToConsole(this IEnumerable<string> lines)
        {
            foreach (var item in lines)
            {
                Console.WriteLine(item);
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

            foreach (var f in code.Functions)
            {
                yield return (string.Format("Function: {0}", f.Name));
                yield return (string.Format("  {0} {1} ()", f.ResultType.Name, f.Name));
                if (f.StatementBlock != null)
                {
                    foreach (var line in f.StatementBlock.DumpCode())
                    {
                        yield return string.Format("  {0}", line);
                    }
                }
                else
                {
                    yield return "  ** No statements ever set";
                }
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
        /// Find the statement of a particular type, or return null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="statementType"></param>
        /// <returns></returns>
        public static T FindStatement<T>(this IStatementCompound source)
            where T : class
        {
            var here = source.Statements.Where(s => s.GetType() == typeof(T)).FirstOrDefault();
            if (here != null)
                return (T)here;

            return source.Statements
                .Where(sc => sc is IStatementCompound)
                .Cast<IStatementCompound>()
                .Select(s => s.FindStatement<T>())
                .Where(found => found != null)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the statement where the parameter was declared, or null.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static IBookingStatementBlock FindDeclarationStatement(this IStatementCompound source, IDeclaredParameter param)
        {
            if (source is IBookingStatementBlock)
            {
                var book = source as IBookingStatementBlock;
                var found = book.DeclaredVariables.Where(v => v.ParameterName == param.ParameterName).Any();
                if (found)
                    return book;
            }

            return source.Statements
                .Where(s => s is IStatementCompound)
                .Cast<IStatementCompound>()
                .Select(s => s.FindDeclarationStatement(param))
                .Where(v => v != null)
                .FirstOrDefault();
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
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
            var seed = new ROOTNET.NTH1F(plotID, plotTitle, nbins, lowBin, highBin)
            {
                Directory = null
            };
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
            f.cd();
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
            string filename = string.Format("vectorintonly_{0}_{1}.root", numberOfIter, vectorsize);
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
        /// Delete everything that is in the query cache directory
        /// </summary>
        public static void ResetCacheDir()
        {
            var cdir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LINQToTTree\\QueryCacheForTesting");
            if (cdir.Exists)
                cdir.Delete(true);
            QueryResultCache.CacheDirectory = cdir;
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
            TTreeQueryExecutor.Reset();
            ResetCacheDir();
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

        /// <summary>
        /// Make sure each function has a return statement on it.
        /// </summary>
        /// <param name="funcs"></param>
        public static void CheckForReturnStatement(this IEnumerable<IQMFuncExecutable> funcs)
        {
            bool allgood = funcs
                .All(f => f.StatementBlock.Statements.LastOrDefault() is StatementReturn);
            Assert.IsTrue(allgood, "One function has no top level return!");
        }

        /// <summary>
        /// Find the declaration, then follow scope until it goes out. Then pass every single line of code on.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="decl">The string that tells us we've hit the delcaration</param>
        /// <param name="atScopeDeclared">If true, then this is declared in a for loop or similar, otherwise it is declared on a seperate line.</param>
        /// <returns></returns>
        public static IEnumerable<string> WhereScopeCloses(this IEnumerable<string> source, string decl, bool atScopeDeclared)
        {
            // First thing is to find the declration.
            var next = source.GetEnumerator();
            bool found = false;
            string line = null;
            while (!found)
            {
                Assert.IsTrue(next.MoveNext(), string.Format("Never found {0} in the code!", decl));
                line = next.Current;
                var ptr = line.IndexOf(decl);
                if (ptr >= 0)
                {
                    found = true;
                    line = line.Substring(ptr);
                }
            }

            // We found it. Now we start counting brackets to see when we go out of scope.

            int depth = 1;
            while (depth != 0)
            {
                while (line.Length > 0)
                {
                    var openb = line.IndexOf('{');
                    var closeb = line.IndexOf('}');

                    // If they are both here, process the first one.
                    if (openb > 0 && closeb > 0)
                    {
                        if (openb < closeb)
                        {
                            closeb = -1;
                        }
                        else
                        {
                            openb = -1;
                        }
                    }

                    // If neither went, then we are done with this line.
                    if (openb < 0 && closeb < 0)
                        line = "";

                    int newStart = 0;
                    if (openb > 0)
                    {
                        newStart = openb + 1;
                        depth++;
                        if (atScopeDeclared)
                        {
                            depth--;
                            atScopeDeclared = false;
                        }
                    }

                    if (closeb > 0)
                    {
                        newStart = closeb + 1;
                        depth--;
                        Assert.IsFalse(atScopeDeclared, "Can't start out with a close bracket on the decl without an open bracket");
                    }

                    if (newStart > 0)
                    {
                        line = line.Substring(newStart);
                    }
                }

                Assert.IsTrue(next.MoveNext(), "Ran out of input while waiting for a close bracket");
                line = next.Current;
            }

            // Out of scope, now return all the code!

            while (next.MoveNext())
                yield return next.Current;
        }

        /// <summary>
        /// Look through the given source code for a particular search string. Somewhere in the search string should be
        /// $$ twice. That is the variable name. We will return the first match with that as the variable name.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public static string FindVariableIn(this IEnumerable<string> source, string searchString)
        {
            var r = FindVariablesIn(source, searchString).FirstOrDefault();
            if (r == null)
                Assert.Fail(string.Format("Unable to find '{0}' in the source stream!", searchString));
            return r;
        }

        public static IEnumerable<string> FindVariablesIn(this IEnumerable<string> source, string searchString)
        {
            var sStr = searchString
                .Replace("\\", @"\\")
                .Replace("(", @"\(")
                .Replace(")", @"\)")
                .Replace("[", @"\[")
                .Replace("]", @"\]")
                .Replace("?", @"\?")
                .Replace(".", @"\.")
                .Replace("^", @"\^");

            sStr = sStr.Replace("$$", @"(?<var>[\w]+)");
            var finder = new Regex(sStr);
            foreach (var l in source)
            {
                var m = finder.Match(l);
                if (m.Success)
                    yield return m.Groups["var"].Value;
            }
        }

        /// <summary>
        /// Return a string form of the QM that might be a bit better suited for comparisons
        /// </summary>
        /// <param name="qm"></param>
        /// <returns></returns>
        public static string CleanQMString(this QueryModel qm)
        {
            return qm.ToString()
                .Replace("{", "").Replace("}", "") // Get rid of sub-query differentiators
                ;
        }

        /// <summary>
        /// Dump a list of query models to the console.
        /// </summary>
        /// <param name="qms"></param>
        /// <returns></returns>
        public static QueryModel[] DumpToConsole(this QueryModel[] qms)
        {
            foreach (var qmNew in qms)
            {
                Console.WriteLine(qmNew);
            }

            return qms;
        }

        /// <summary>
        /// If the object is of type U, then cast it to type U.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<U> WhereCast<T,U> (this IEnumerable<T> source)
        {
            return source
                .Where(s => s is U)
                .Cast<U>();
        }
    }
}
