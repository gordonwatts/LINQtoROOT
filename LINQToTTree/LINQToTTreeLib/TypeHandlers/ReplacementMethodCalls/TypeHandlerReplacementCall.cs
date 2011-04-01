using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib.Expressions;

namespace LINQToTTreeLib.TypeHandlers.ReplacementMethodCalls
{
    /// <summary>
    /// If the user makes an expression call that is very simple - like a static object,
    /// then we can do a pretty simple replacement
    /// </summary>
    [Export(typeof(ITypeHandler))]
    public class TypeHandlerReplacementCall : ITypeHandler
    {
        /// <summary>
        /// See if we know about this sort of replacement.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool CanHandle(Type t)
        {
            Init();
            return _knownTypes.Where(kt => kt.Name == t.Name).Any();
        }

        /// <summary>
        /// Known info about a type.
        /// </summary>
        class KnownTypeInfo
        {

            public class MechodArg
            {
                public string Type { get; set; }
                public string CPPType { get; set; }
            }

            public class MethodInfo
            {
                /// <summary>
                /// The .NET name of the method
                /// </summary>
                public string Name { get; set; }

                /// <summary>
                /// The method name in C++
                /// </summary>
                public string CPPName { get; set; }

                /// <summary>
                /// Arguments to the method
                /// </summary>
                public MechodArg[] Arguments { get; set; }

                /// <summary>
                /// Returns a list of include files that this type will need when it is translated.
                /// </summary>
                public string[] IncludeFiles { get; set; }
            }

            /// <summary>
            /// The type name (Type.Name).
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// List of methods that we know about for this type
            /// </summary>
            public MethodInfo[] Methods { get; set; }
        }

        /// <summary>
        /// List of known types
        /// </summary>
        private List<KnownTypeInfo> _knownTypes = null;

        /// <summary>
        /// Init internal data structures that we use to track what we know about. This must
        /// be done only once. And once it is done it can't change.
        /// </summary>
        private void Init()
        {
            ///
            /// If we've done it already, then we are done!
            /// 

            if (_knownTypes != null)
                return;
            _knownTypes = new List<KnownTypeInfo>();

            ///
            /// First, copy the highest priority items in - tohse that someone has set internally.
            /// This path is used mostly during testing.
            /// 

            foreach (var kt in gSetTypes)
            {
                Merge(kt);
            }

            ///
            /// Look for local files
            /// 

            Parse(new DirectoryInfo("."));
            Parse(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LINQToTTree\\Config"));
        }

        /// <summary>
        /// Parsae all mapping files in a directory.
        /// </summary>
        /// <param name="dir"></param>
        private void Parse(DirectoryInfo dir)
        {
            if (!dir.Exists)
                return;

            foreach (var file in dir.EnumerateFiles("*.classmethodmappings"))
            {
                using (var reader = file.OpenText())
                {
                    Parse(reader);
                }
            }
        }

        /// <summary>
        /// Called when trying to import a constant reference accross the wire... that should never
        /// happen here, so we should always crash!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <returns></returns>
        public IValue ProcessConstantReference(System.Linq.Expressions.ConstantExpression expr, IGeneratedCode codeEnv, ICodeContext context, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate the method call
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public System.Linq.Expressions.Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedCode gc, ICodeContext context, CompositionContainer container)
        {
            Init();

            ///
            /// First see if we can't locate the method call that at least matches in names
            /// 

            var matchingMethodNames = from kt in _knownTypes
                                      where kt.Name == expr.Method.DeclaringType.Name
                                      from m in kt.Methods
                                      where m.Name == expr.Method.Name
                                      select new
                                      {
                                          theType = kt,
                                          theMethod = m
                                      };

            ///
            /// Next, match with the arguments
            /// 

            var matchingMethod = from m in matchingMethodNames
                                 where m.theMethod.Arguments.Length == expr.Arguments.Count
                                 where m.theMethod.Arguments.Zip(expr.Arguments, (us, them) => new Tuple<KnownTypeInfo.MechodArg, Expression>(us, them)).All(apair => apair.Item1.Type == apair.Item2.Type.FullName)
                                 select m;

            ///
            /// Ok, at this point, we should have only one guy. If we have more then just choose the first
            /// 

            var method = matchingMethod.FirstOrDefault();
            if (method == null)
                throw new ArgumentException("Could not find a matching method to translate for the call " + expr.ToString());

            ///
            /// And now translate the call
            /// 

            StringBuilder rawValue = new StringBuilder();

            rawValue.Append(method.theMethod.CPPName);
            rawValue.Append("(");
            bool first = true;
            foreach (var arg in expr.Arguments.Zip(method.theMethod.Arguments, (m, a) => Tuple.Create(m, a)))
            {
                if (!first)
                    rawValue.Append(",");
                first = false;
                rawValue.AppendFormat("({0}){1}", arg.Item2.CPPType, ExpressionToCPP.GetExpression(arg.Item1, gc, context, container).RawValue);
            }
            rawValue.Append(")");

            result = new ValSimple(rawValue.ToString(), expr.Type);

            ///
            /// Include files
            /// 

            foreach (var ifile in method.theMethod.IncludeFiles)
            {
                gc.AddIncludeFile(ifile);
            }

            ///
            /// We aren't re-writing this expression, so just return it.
            /// 

            return expr;
        }

        /// <summary>
        /// If someone locally updates this before we've done the lookup, then
        /// we will copy this as the highest priority info source.
        /// </summary>
        static List<KnownTypeInfo> gSetTypes = new List<KnownTypeInfo>();

        /// <summary>
        /// Add a known type to the list. Args is (.net type/cpp type)
        /// </summary>
        /// <param name="typeName"></param>
        public static void AddMethod(string typeName, string methodName, string cppMethodName, Tuple<string, string>[] args = null, string[] includeFiles = null)
        {
            var kt = new KnownTypeInfo() { Name = typeName };
            var ourArgs = new KnownTypeInfo.MechodArg[0];
            if (args != null)
                ourArgs = (from t in args
                           select new KnownTypeInfo.MechodArg()
                           {
                               Type = t.Item1,
                               CPPType = t.Item2
                           }).ToArray();

            var incfiles = includeFiles;
            if (incfiles == null)
            {
                incfiles = new string[0];
            }

            kt.Methods = new KnownTypeInfo.MethodInfo[] { new KnownTypeInfo.MethodInfo() { Name = methodName, CPPName = cppMethodName, Arguments = ourArgs, IncludeFiles = incfiles } };
            gSetTypes.Add(kt);
        }

        /// <summary>
        /// The type list is done - clear it out.
        /// </summary>
        internal static void ClearTypeList()
        {
            gSetTypes.Clear();
        }

        /// <summary>
        /// Parse a stream from a file (or whatever) for function definitions and add them. Don't superseed ones that are already there, but
        /// allow duplicates.
        /// </summary>
        /// <param name="stringReader"></param>
        internal void Parse(TextReader stringReader)
        {
            ///
            /// Get the "good" lines
            /// 

            var goodLines = from l in ReadLinesAsIterator(stringReader)
                            let ltrm = l.Trim()
                            where !ltrm.StartsWith("#") && !string.IsNullOrWhiteSpace(ltrm)
                            select ltrm;

            ///
            /// Is the format good enough that we can split things up?
            /// 

            Regex funcFinder = new Regex("(?<class>[^\\s]+)\\s+(?<netfunc>[^\\s\\(]+)\\s*\\((?<netargs>[^\\)]+)\\)\\s*=>\\s*(?<cppfunc>[^\\s\\(]+)\\s*\\((?<cppargs>[^\\)]+)\\)");
            string[] includfiles = new string[0];

            foreach (var line in goodLines)
            {
                if (line.StartsWith("include:"))
                {
                    var files = from l in line.Substring(8).Split(' ')
                                let lstuff = l.Trim()
                                where !string.IsNullOrWhiteSpace(lstuff)
                                select l;
                    includfiles = files.ToArray();
                    continue;
                }

                var m = funcFinder.Match(line);
                if (!m.Success)
                    throw new InvalidDataException("Unable to interpret line '" + line + "'");

                var netargs = ParseArgumentListTokens(m.Groups["netargs"].Value);

                var cppargs = ParseArgumentListTokens(m.Groups["cppargs"].Value);

                if (cppargs.Length != netargs.Length)
                {
                    throw new InvalidDataException("Arguments for cpp and net are not the same in line '" + line + "'");
                }

                ///
                /// Finaly, ready to create the mapping!
                /// 

                var kt = new KnownTypeInfo()
                {
                    Name = m.Groups["class"].Value,
                    Methods = new KnownTypeInfo.MethodInfo[] {new KnownTypeInfo.MethodInfo() {
                         Name = m.Groups["netfunc"].Value,
                         CPPName = m.Groups["cppfunc"].Value,
                         Arguments = (from a in netargs.Zip(cppargs, (n, c) => new KnownTypeInfo.MechodArg() { CPPType = c, Type = n}) select a).ToArray(),
                         IncludeFiles = includfiles
                     }
                     }
                };

                Merge(kt);
            }

        }

        /// <summary>
        /// Merge a type in with another type we alreayd have.
        /// </summary>
        /// <param name="kt"></param>
        private void Merge(KnownTypeInfo kt)
        {
            Init();

            var already = (from k in _knownTypes
                           where k.Name == kt.Name
                           select k).FirstOrDefault();

            if (already == null)
            {
                _knownTypes.Add(kt);
            }
            else
            {
                List<KnownTypeInfo.MethodInfo> methods = new List<KnownTypeInfo.MethodInfo>(already.Methods);
                methods.AddRange(kt.Methods);
                already.Methods = methods.ToArray();
            }

        }

        /// <summary>
        /// Parse the argument string
        /// </summary>
        /// <param name="argstring"></param>
        /// <returns></returns>
        private static string[] ParseArgumentListTokens(string argstring)
        {
            var netargs = (from a in argstring.Split(',')
                           let arg = a.Trim()
                           where !string.IsNullOrWhiteSpace(arg)
                           select arg).ToArray();
            return netargs;
        }

        /// <summary>
        /// Iterator pattern for reading lines from a stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private IEnumerable<string> ReadLinesAsIterator(TextReader reader)
        {
            string line = reader.ReadLine();
            if (line != null)
            {
                do
                {
                    yield return line;
                    line = reader.ReadLine();
                } while (line != null);
            }
        }
    }
}
