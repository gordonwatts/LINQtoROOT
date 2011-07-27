using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
        /// Keep track of how many unique variables we've replaced!
        /// </summary>
        private int _uniqueCounter = 0;

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
            var paramLookup = paramsTranslated.ToDictionary(v => v.Name, v => v.Translated.ApplyParensIfNeeded());

            ///
            /// We also need a return variable. Since this can be multiple lines of code and we don't
            /// know how the result will be used, we have to declare it up front... and pray they
            /// use it correctly! :-)
            /// 

            var cppType = expr.Type.AsCPPType();
            var resultName = expr.Type.CreateUniqueVariableName();

            var cppStatement = new CPPCodeStatement(expr.Method, cppType, resultName);
            gc.Add(cppStatement);

            paramLookup.Add(expr.Method.Name, resultName);

            result = new ValSimple(resultName, expr.Type);

            //
            // Make sure a result exists in here!
            //

            var lookForResult = new Regex(string.Format(@"\b{0}\b", expr.Method.Name));
            bool didReference = code.Code.Any(l => lookForResult.Match(l).Success);
            if (!didReference)
                throw new ArgumentException(string.Format("The C++ code attached to the method '{0}' doesn't seem to set a result.", expr.Method.Name));

            //
            // Figure out if there are any Unique variables. If there are, then we need to do
            // a replacement on them.
            //

            var findUnique = new Regex(@"\b\w*Unique\b");
            var varUniqueRequests = (from l in code.Code
                                     let matches = findUnique.Matches(l)
                                     from m in Enumerable.Range(0, matches.Count)
                                     select matches[m].Value).Distinct();
            foreach (var varRepl in varUniqueRequests)
            {
                var uniqueName = varRepl.Substring(0, varRepl.Length - "Unique".Length);
                paramLookup.Add(varRepl, uniqueName + _uniqueCounter.ToString());
                _uniqueCounter++;
            }

            ///
            /// Now, go through the lines of code and translate things. We have to be careful 
            /// in the replacement. For example, if the parameter is "E", don't replace the "E"
            /// in SetPtPhiEtaE method name!
            /// 

            var paramReplaceRegex = (from kv in paramLookup
                                     select new
                                     {
                                         Key = kv.Key,
                                         Value = new Regex(string.Format(@"\b{0}\b", kv.Key))
                                     }).ToDictionary(k => k.Key, v => v.Value);

            foreach (var line in code.Code)
            {
                var tline = line;
                foreach (var k in paramLookup.Keys)
                {
                    tline = paramReplaceRegex[k].Replace(tline, paramLookup[k]);
                }
                cppStatement.AddLine(tline);
            }

            return expr;
        }

        /// <summary>
        /// A single statement that deals with this special code. We do this rather than make it up otherwise
        /// as we have special combination symantics.
        /// </summary>
        private class CPPCodeStatement : IStatement
        {
            private System.Reflection.MethodInfo methodInfo;
            private string cppType;
            private string resultName;

            /// <summary>
            /// Init a code block statement
            /// </summary>
            /// <param name="methodInfo"></param>
            /// <param name="cppType"></param>
            /// <param name="resultName"></param>
            public CPPCodeStatement(MethodInfo methodInfo, string typeOfResult, string resultName)
            {
                this.methodInfo = methodInfo;
                this.cppType = typeOfResult;
                this.resultName = resultName;
            }

            List<string> LinesOfCode = new List<string>();

            /// <summary>
            /// Add a line to the list of statements.
            /// </summary>
            /// <param name="tline"></param>
            internal void AddLine(string tline)
            {
                LinesOfCode.Add(tline);
            }

            /// <summary>
            /// Return the code that will be rendered to the C++ compiler.
            /// </summary>
            /// <returns></returns>
            public System.Collections.Generic.IEnumerable<string> CodeItUp()
            {
                //
                // First, the decl for the result variable.
                //

                yield return string.Format("{0} {1};", cppType, resultName);

                //
                // Now the various lines of code that the user entered.
                //

                foreach (var l in LinesOfCode)
                {
                    yield return l;
                }
            }

            public bool IsSameStatement(IStatement statement)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Rename all variables.
            /// </summary>
            /// <param name="originalName"></param>
            /// <param name="newName"></param>
            public void RenameVariable(string originalName, string newName)
            {
                resultName.ReplaceVariableNames(originalName, newName);
                LinesOfCode = LinesOfCode.Select(l => l.ReplaceVariableNames(originalName, newName)).ToList();
            }

            /// <summary>
            /// Combination means everything is identical except for the result variable. So the test is actually a little
            /// tricky.
            /// </summary>
            /// <param name="statement"></param>
            /// <param name="optimize"></param>
            /// <returns></returns>
            public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
            {
                if (statement == null)
                    throw new ArgumentNullException("statement");

                var other = statement as CPPCodeStatement;
                if (other == null)
                    return false;

                if (other.methodInfo != methodInfo)
                    return false;

                var fixedUpOtherCode = other.LinesOfCode.Select(l => l.ReplaceVariableNames(other.resultName, resultName));
                var areSame = LinesOfCode.Zip(fixedUpOtherCode, (us, them) => us == them).All(test => test);
                if (!areSame)
                    return false;

                //
                // Ok, they are the same. The one thing that has to be changed is how the result variable
                // in the other guy is used below. So we need to make that the "same".
                //

                optimize.ForceRenameVariable(other.resultName, resultName);

                return true;
            }

            /// <summary>
            /// Keep track of who is hosting us
            /// </summary>
            public IStatement Parent { get; set; }
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
