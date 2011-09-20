using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;

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

            var rootObject = expr.Value as ROOTNET.Interface.NTNamed;
            if (rootObject == null)
                throw new ArgumentException("the object to be stored must derive from NTNamed! Instead it is of type '" + expr.Value.GetType().Name + "'");

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
        /// Someone is accessing a method on our ROOT object. We do the translation to C++
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <returns></returns>
        public Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
        {
            var objRef = ExpressionToCPP.GetExpression(expr.Object, gc, context, container);
            StringBuilder bld = new StringBuilder();
            bld.AppendFormat("{0}.{1}", objRef.AsObjectReference(), expr.Method.Name);

            AddMethodArguments(expr.Arguments, gc, context, container, bld);

            result = new ValSimple(bld.ToString(), expr.Type);
            return expr;
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
        public Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
        {
            ///
            /// Do checks
            ///

            if (gc == null)
                throw new ArgumentException("gc");
            if (context == null)
                throw new ArgumentException("context");
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

            AddMethodArguments(expression.Arguments, gc, context, container, ctor);

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
        private void AddMethodArguments(System.Collections.ObjectModel.ReadOnlyCollection<Expression> args, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container, StringBuilder builtArgs)
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
                builtArgs.Append(ExpressionToCPP.GetExpression(a, gc, context, container).CastToType(a));
            }
            builtArgs.Append(")");
        }
    }
}
