﻿using System;
using System.ComponentModel.Composition;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.TypeHandlers.CPPCode
{
    /// <summary>
    /// Type hander to help insert lines of code
    /// that the user has specified.
    /// </summary>
    [Export(typeof(ITypeHandler))]
    class TypeHandlerCPPCode : ITypeHandler
    {
        /// <summary>
        /// We can handle only classes that are correctly set!
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool CanHandle(Type t)
        {
            return t.TypeHasAttribute<CPPHelperClassAttribute>() != null;
        }

        /// <summary>
        /// This should never ever happen - there is no such thing as passing over a
        /// constant reference of one of these guys!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public IValue ProcessConstantReference(System.Linq.Expressions.ConstantExpression expr, IGeneratedQueryCode codeEnv, ICodeContext context, System.ComponentModel.Composition.Hosting.CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Where the real work happens!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public System.Linq.Expressions.Expression ProcessMethodCall(System.Linq.Expressions.MethodCallExpression expr, out IValue result, IGeneratedQueryCode gc, ICodeContext context, System.ComponentModel.Composition.Hosting.CompositionContainer container)
        {
            if (expr == null)
                throw new ArgumentNullException("expr");

            ///
            /// Get the coding attribute off the method
            /// 

            var code = expr.Method.TypeHasAttribute<CPPCodeAttribute>();
            if (code == null)
                throw new InvalidOperationException(string.Format("Asked to generate code for a CPP method '{0}' but no CPPCode attribute found on that method!", expr.Method.Name));

            ///
            /// Do the include files first
            /// 

            if (code.IncludeFiles != null)
            {
                foreach (var inc in code.IncludeFiles)
                {
                    gc.AddIncludeFile(inc);
                }
            }

            ///
            /// And the lines of code. Here things are a little tricky as we want to translate the
            /// parameters! As usual, assume these are all stateless!
            ///

            var paramsTranslated = from p in expr.Arguments.Zip(expr.Method.GetParameters(), (arg, param) => Tuple.Create(arg, param))
                                   select new
                                   {
                                       Name = p.Item2.Name,
                                       Translated = ExpressionToCPP.GetExpression(p.Item1, gc, context, container)
                                   };
            var paramLookup = paramsTranslated.ToDictionary(v => v.Name, v => v.Translated.RawValue);

            ///
            /// We also need a return variable. Since this can be multiple lines of code and we don't
            /// know how the result will be used, we have to declare it up front... and pray they
            /// use it correctly! :-)
            /// 

            var cppType = expr.Type.AsCPPType();
            if (expr.Type.IsPointerType())
            {
                cppType = cppType + " *";
            }
            var resultName = expr.Type.CreateUniqueVariableName();
            gc.Add(new Statements.StatementSimpleStatement(string.Format("{0} {1}", cppType, resultName)));

            paramLookup.Add(expr.Method.Name, resultName);

            result = new ValSimple(resultName, expr.Type);

            ///
            /// Now, go through the lines of code and translate things!
            /// 

            foreach (var line in code.Code)
            {
                var tline = line;
                foreach (var k in paramLookup.Keys)
                {
                    tline = tline.Replace(k, paramLookup[k]);
                }
                gc.Add(new Statements.StatementSimpleStatement(tline));
            }

            return expr;
        }

        /// <summary>
        /// These are static classes as far as we are concerned - so they should never be able to run.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public System.Linq.Expressions.Expression ProcessNew(System.Linq.Expressions.NewExpression expression, out IValue result, IGeneratedQueryCode gc, ICodeContext context, System.ComponentModel.Composition.Hosting.CompositionContainer container)
        {
            throw new NotImplementedException();
        }
    }
}