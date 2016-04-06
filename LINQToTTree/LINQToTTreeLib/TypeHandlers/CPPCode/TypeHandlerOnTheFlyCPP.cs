using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using System.Text.RegularExpressions;

namespace LINQToTTreeLib.TypeHandlers.CPPCode
{
    /// <summary>
    /// Type handler for code that is built on-the-fly by the application.
    /// </summary>

    [Export(typeof(ITypeHandler))]
    class TypeHandlerOnTheFlyCPP : ITypeHandler
    {
        /// <summary>
        /// Return true if the class type we are looking at comes from IOnTheFlyCPPObject
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool CanHandle(Type t)
        {
            // If the type implement ITypeHandler then we are good.
            return t.GetInterfaces().Contains(typeof(IOnTheFlyCPPObject));
        }

        /// <summary>
        /// Do the work of translating this to code by fetching the data from the interface.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public IValue CodeMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, CompositionContainer container)
        {
            if (expr == null)
                throw new ArgumentNullException("expr");

            // Get a reference to the object so we can code the call to get back the C++ code.
            var onTheFly = (expr?.Object as ConstantExpression)?.Value as IOnTheFlyCPPObject;
            if (onTheFly == null)
            {
                throw new InvalidOperationException("Unable to find the IOnTheFlyCPPObject!");
            }

            var includeFiles = onTheFly.IncludeFiles();
            var loc = onTheFly.LinesOfCode(expr.Method.Name).ToArray();

            return CPPCodeStatement.BuildCPPCodeStatement(expr, gc, container, includeFiles, loc);
        }

        /// <summary>
        /// We have no version of a constant reference - we can't pass this object back and forth between .NET and the C++ world.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedQueryCode codeEnv, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// No constant version of anything.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessConstantReferenceExpression(ConstantExpression expr, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// No member lookup is possible (though it might be eventually!).
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public IValue ProcessMemberReference(MemberExpression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A method call is our bread and butter. However, there is no internal translation to be done, so we can wait
        /// until the end of the translation process when it is time to turn it into real IValue.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
        {
            return expr;
        }

        /// <summary>
        /// There should never be a new on one of these objects mid-stream!
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, CompositionContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
