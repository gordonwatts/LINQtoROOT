using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using System.IO;

namespace LINQToTTreeLib.TypeHandlers.ROOT
{
    /// <summary>
    /// Deal with all ROOT classes
    /// </summary>
    [Export(typeof(ITypeHandler))]
    public class TypeHandlerROOT : ITypeHandler
    {
        /// <summary>
        /// Anything that begins with ROOT!
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool CanHandle(Type t)
        {
            // <pex>
            if (t == (Type)null)
                throw new ArgumentNullException("t");
            // </pex>
            return t.FullName.StartsWith("ROOTNET.");
        }

        /// <summary>
        /// For a root variable we create a special variable which holds onto the inital value, and
        /// also will get loaded at the correct time.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <returns></returns>
        public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedQueryCode codeEnv, CompositionContainer container)
        {
            ///
            /// The value is a reference that will do the loading.
            /// 

            var rootObject = expr.Value as ROOTNET.Interface.NTObject;
            if (rootObject == null)
                throw new ArgumentException("the object to be stored must derive from NTObject! Instead it is of type '" + expr.Value.GetType().Name + "'");

            //
            // Queue this object for transfer, get a "unique" name back. This will also double check
            // to see if the object is already up there read yto be queued.
            //

            var varNameForTransport = codeEnv.QueueForTransfer(rootObject);

            //
            // Now, we need to generate an IValue for the object that can be used in our expression parsing.
            //

            var CPPType = rootObject.GetType().AsCPPType();
            var val = new ROOTObjectCopiedValue(varNameForTransport, rootObject.GetType(), CPPType, rootObject.Name, rootObject.Title);

            return val;
        }

        /// <summary>
        /// When we hit this early in the review process we ignore it - we will get it right at the end.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessConstantReferenceExpression(ConstantExpression expr, CompositionContainer container)
        {
            return expr;
        }


        /// <summary>
        /// Called during the high-level expression parsing. We will parse the expression and method to other expressions...
        /// So we drive everything through (get rid of sub-queries, etc.). First part of two pass parsing. Second part is below
        /// that will actually generate the C++ code.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
        {
            //
            // Pick apart the various things in the method call we need.
            //

            var robj = expr.Object.Resolve(gc, context, container);
            var method = expr.Method;
            var rargs = expr.Arguments.Select(e => e.Resolve(gc, context, container));

            return Expression.Call(robj, method, rargs);
        }

        /// <summary>
        /// Someone is accessing a method on our ROOT object. We do the translation to C++ here.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <returns></returns>
        /// <remarks>Static methods and instance methods are both handled correctly.</remarks>
        public IValue CodeMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, CompositionContainer container)
        {
            var objRef = ExpressionToCPP.InternalGetExpression(expr.Object, gc, null, container);
            StringBuilder bld = new StringBuilder();

            if (expr.Method.IsStatic && objRef != null)
                throw new ArgumentException(string.Format("Call to ROOT instance method '{0}' where the instance is null", expr.Method.Name));
            if (!expr.Method.IsStatic && objRef == null)
                throw new ArgumentException(string.Format("Call to ROOT static method '{0}' where the instance is not null", expr.Method.Name));

            //
            // Code up the local invokation or the static invokation to the method
            //

            if (objRef != null)
            {
                bld.AppendFormat("{0}.{1}", objRef.AsObjectReference(), expr.Method.Name);
            }
            else
            {
                bld.AppendFormat("{0}::{1}", expr.Method.DeclaringType.Name.Substring(1), expr.Method.Name);
            }

            //
            // Put in the arguments
            //

            AddMethodArguments(expr.Arguments, gc, container, bld);

            return new ValSimple(bld.ToString(), expr.Type);
        }

        /// <summary>
        /// New a ROOT object. Make sure that it gets dtor'd!
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, CompositionContainer container)
        {
            ///
            /// Do checks
            ///

            if (gc == null)
                throw new ArgumentException("gc");
            if (expression == null)
                throw new ArgumentNullException("expression");

            ///
            /// Figure out the type. We can only get here if we get through ROOTNET.xxx
            /// 

            result = null;
            string tname = expression.Type.FullName.Substring(8);
            if (tname[0] != 'N')
            {
                throw new ArgumentException(string.Format("Don't know how to translate to a ROOT type '{0}'", expression.Type.FullName));
            }
            tname = tname.Substring(1);

            ///
            /// We assume the include file "just works" - this is ROOT, after all. But lets hope.
            /// This is something we might have to deal with later. :-)
            /// 

            gc.AddIncludeFile(string.Format("{0}.h", tname));

            ///
            /// Now, build the ctor, and add it to the statement list.
            /// 

            var ctor = new StringBuilder();
            var ctorName = expression.Type.CreateUniqueVariableName();
            ctor.AppendFormat("{0} {1}", tname, ctorName);

            AddMethodArguments(expression.Arguments, gc, container, ctor);

            gc.Add(new Statements.StatementSimpleStatement(ctor.ToString()));

            ///
            /// Now, everything in the C++ translation is a pointer, so we will
            /// not create a pointer to this guy.
            /// 

            var ptrDecl = new StringBuilder();
            var ptrName = expression.Type.CreateUniqueVariableName();
            ptrDecl.AppendFormat("{0} *{1} = &{2}", tname, ptrName, ctorName);
            gc.Add(new Statements.StatementSimpleStatement(ptrDecl.ToString()));

            ///
            /// That pointer is what we return for later use!
            /// 

            result = new ValSimple(ptrName, expression.Type);
            return expression;
        }

        /// <summary>
        /// Add the arguments for call
        /// </summary>
        /// <param name="args"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <param name="builtArgs"></param>
        private void AddMethodArguments(System.Collections.ObjectModel.ReadOnlyCollection<Expression> args, IGeneratedQueryCode gc, CompositionContainer container, StringBuilder builtArgs)
        {
            builtArgs.Append("(");
            bool first = true;
            foreach (var a in args)
            {
                if (!first)
                {
                    builtArgs.Append(",");
                }
                first = false;
                builtArgs.Append(ExpressionToCPP.GetExpression(a, gc, null, container).CastToType(a));
            }
            builtArgs.Append(")");
        }

        /// <summary>
        /// Processed later on in the stack usign the default symbols.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public IValue ProcessMemberReference(MemberExpression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            return null;
        }

        /// <summary>
        /// Take this ROOT object and turn it into an object that can be properly sent along to the
        /// TSelector::Process or PROOF correctly.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="objToStore"></param>
        /// <returns></returns>
        public ROOTNET.Interface.NTObject CreateObjectForTListTransfer(string name, object objToStore)
        {
            // If this is a named object, then we just store it, and assume that
            // everything is taken care of for us.

            var obj = objToStore as ROOTNET.Interface.NTNamed;
            if (obj != null)
            {
                var cloned = obj.Clone(name);
                return cloned;
            }

            // Ok, this is a ROOT.NET object (or we'd not be here) so we will need to wrap it in a TParameter.
            // The thign with this is we have to make sure three operators are defined.

            var cls = objToStore.GetType().GetROOTClass();
            using (var operatorDef = new MemoryStream())
            {
                var output = new StreamWriter(operatorDef);
                var objIncludeFile = CleanROOTInclude(cls.DeclFileName_GetSetProperty);

                output.WriteLine("#include \"{0}\"", objIncludeFile);

                // Needs and output operator
                output.WriteLine("ostream & operator<< (ostream &os, const {0} &v) {{", cls.Name);
                output.WriteLine("  os << \"{0}\";", cls.Name);
                output.WriteLine("  return os;");
                output.WriteLine("}");

                // If there is no method to do +, then add that in.
                string constArgs = string.Format("const {0} &v1, const {0} &v2", cls.Name);
                if (!HasSelfOperator(cls, "operator+="))
                {
                    output.WriteLine("{0} operator += ({1}) {{", cls.Name, constArgs);
                    output.WriteLine("  return {0}();", cls.Name);
                    output.WriteLine("}");
                }

                // If there is no method to do *, then add one in.
                if (!HasSelfOperator(cls, "operator*="))
                {
                    output.WriteLine("{0} operator *= ({1}) {{", cls.Name, constArgs);
                    output.WriteLine("  return {0}();", cls.Name);
                    output.WriteLine("}");
                }

                // Write out a file and generate a dictionary.
                output.Flush();
                operatorDef.Seek(0, SeekOrigin.Begin);
                string operatorHeaderName = string.Format("{0}_op.h", cls.Name);
                using (var writer = File.CreateText(operatorHeaderName))
                {
                    var reader = new StreamReader(operatorDef);
                    writer.Write(reader.ReadToEnd());
                    reader.Close();
                    writer.Close();
                }

                var r = ROOTNET.NTInterpreter.Instance().GenerateDictionary(
                    string.Format("TParameter<{0}>", cls.Name),
                    string.Format("{0};TParameter.h;{1}", operatorHeaderName, objIncludeFile)
                    );

                if (r != 0)
                    throw new InvalidOperationException(string.Format("Unable to generate a TParameter<{0}> dictionary to carry over one of your initial values!", cls.Name));
            }

            // Now, create the parameter and set it as the thing that will be written out. Clone it as well
            // to make sure we don't... well, it doesn't die.

            var objParam = objToStore;
            if (objParam is ROOTNET.NTObject)
                objParam = (objToStore as ROOTNET.NTObject).Clone(name);

            dynamic param = ROOTNET.Utility.ROOTCreator.CreateByName(
                string.Format("TParameter<{0}>", cls.Name),
                name,
                objParam
                );

            return param;
        }

        /// <summary>
        /// If the self operator exists, return true
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        private bool HasSelfOperator(ROOTNET.Interface.NTClass cls, string opName)
        {
            var addMethod = cls
                .GetListOfMethods()
                .Cast<ROOTNET.Interface.NTMethod>()
                .Where(m => m.Name == opName)
                .Where(m => m.ListOfMethodArgs.Entries == 1)
                .Where(m => m.ListOfMethodArgs.At(0).Title.Contains(cls.Name))
                .FirstOrDefault();
            return addMethod != null;
        }

        /// <summary>
        /// Given a file that is in some subdir - get it back as the file only.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string CleanROOTInclude(string p)
        {
            var lastSlash = p.LastIndexOf('/');
            if (lastSlash < 0)
                return p;
            return p.Substring(lastSlash+1);
        }
    }
}
