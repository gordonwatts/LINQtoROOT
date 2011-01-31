using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.TypeHandlers.ReplacementMethodCalls
{
    /// <summary>
    /// If the user makes an expression call that is very simple - like a static object,
    /// then we can do a pretty simple replacement
    /// </summary>
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
                _knownTypes.Add(kt);
            }
        }

        /// <summary>
        /// Called when trying to import a constant reference accross the wire... that should never
        /// happen here, so we should always crash!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <returns></returns>
        public IValue ProcessConstantReference(System.Linq.Expressions.ConstantExpression expr, IGeneratedCode codeEnv)
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
        public System.Linq.Expressions.Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedCode gc, ICodeContext context)
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
                                 where m.theMethod.Arguments.Zip(expr.Arguments, (us, them) => new Tuple<KnownTypeInfo.MechodArg, Expression>(us, them)).All(apair => apair.Item1.Type == apair.Item2.Type.Name)
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
            foreach (var arg in expr.Arguments)
            {
                if (!first)
                    rawValue.Append(",");
                first = false;
                rawValue.Append(ExpressionVisitor.GetExpression(arg, gc, context).RawValue);
            }
            rawValue.Append(")");

            result = new ValSimple(rawValue.ToString(), expr.Type);

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
        public static void AddMethod(string typeName, string methodName, string cppMethodName, Tuple<string, string>[] args = null)
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

            kt.Methods = new KnownTypeInfo.MethodInfo[] { new KnownTypeInfo.MethodInfo() { Name = methodName, CPPName = cppMethodName, Arguments = ourArgs } };
            gSetTypes.Add(kt);
        }

        /// <summary>
        /// The type list is done - clear it out.
        /// </summary>
        internal static void ClearTypeList()
        {
            gSetTypes.Clear();
        }
    }
}
