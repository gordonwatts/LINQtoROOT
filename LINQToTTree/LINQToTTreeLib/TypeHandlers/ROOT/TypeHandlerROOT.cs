using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;
using System.Linq.Expressions;
using System.ComponentModel.Composition;
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
        public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedCode codeEnv)
        {
            var val = new ROOTObjectVariable(expr.Value as ROOTNET.Interface.NTObject);
            codeEnv.AddTransfered(val);
            return val;
        }

        /// <summary>
        /// Someone is accessing a method on our ROOT object. We do the translation to C++
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <returns></returns>
        public Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedCode gc, ICodeContext context)
        {
            var objRef = ExpressionVisitor.GetExpression(expr.Object, gc, context);
            StringBuilder bld = new StringBuilder();
            bld.AppendFormat("{0}.{1}(", objRef.AsObjectReference(), expr.Method.Name);

            bool first = true;
            foreach (var a in expr.Arguments)
            {
                if (!first)
                {
                    bld.Append(",");
                }
                first = false;
                bld.Append(ExpressionVisitor.GetExpression(a, gc, context).AsCastString());
            }
            bld.Append(")");

            result = new ValSimple(bld.ToString(), expr.Type);
            return expr;
        }
    }
}
