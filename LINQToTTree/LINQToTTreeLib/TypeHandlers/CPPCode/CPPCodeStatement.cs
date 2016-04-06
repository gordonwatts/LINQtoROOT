using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LINQToTTreeLib.TypeHandlers.CPPCode
{
    /// <summary>
    /// A single statement that deals with this special code. We do this rather than make it up otherwise
    /// as we have special combination semantics.
    /// </summary>
    class CPPCodeStatement : IStatement, ICMStatementInfo
    {
        private System.Reflection.MethodInfo _methodInfo;
        private IValue _cppResult;

        /// <summary>
        /// Initialize a code block statement
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="cppType"></param>
        /// <param name="resultName"></param>
        /// <param name="dependents">The dependent variables. Null if there are none (or an empty set)</param>
        public CPPCodeStatement(MethodInfo methodInfo, IValue cppResult, IEnumerable<string> loc, HashSet<string> dependents = null)
        {
            _methodInfo = methodInfo;
            _cppResult = cppResult;
            ResultVariables = new HashSet<string>() { _cppResult.RawValue };

            _linesOfCode.AddRange(loc);

            if (dependents == null)
            {
                dependents = new HashSet<string>();
            }
            DependentVariables = dependents;
        }

        /// <summary>
        /// Cache the lines of code we will insert
        /// </summary>
        List<string> _linesOfCode = new List<string>();

        /// <summary>
        /// All variables that we need to do a replacement in
        /// </summary>
        List<Tuple<string, string>> _paramReplacesments = new List<Tuple<string, string>>();

        public void AddParamReplacement(string paramName, string argument)
        {
            _paramReplacesments.Add(new Tuple<string, string>(paramName, argument));
        }

        /// <summary>
        /// Return the code that will be rendered to the C++ compiler.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            //
            // Now the various lines of code that the user entered.
            //

            foreach (var l in _linesOfCode)
            {
                yield return l
                    .ReplaceVariableNames(_paramReplacesments)
                    .ReplaceVariableNames(_uniqueVariableTranslations.Select(e => Tuple.Create(e.Key, e.Value)));
            }
        }

        /// <summary>
        /// Rename all variables.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            _cppResult.RenameRawValue(originalName, newName);
            ResultVariables = new HashSet<string>() { _cppResult.RawValue };
            _paramReplacesments = _paramReplacesments
                .Select(p => Tuple.Create(p.Item1, p.Item2.ReplaceVariableNames(originalName, newName)))
                .ToList();
            _linesOfCode = _linesOfCode.Select(l => l.ReplaceVariableNames(originalName, newName)).ToList();
            DependentVariables = DependentVariables.Select(r => r.ReplaceVariableNames(originalName, newName)).ToHashSet();
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

            if (other._methodInfo != _methodInfo)
                return false;

            // Make sure the lines of code are identical.
            var loc = _linesOfCode.Zip(other._linesOfCode, (u, t) => Tuple.Create(u, t));
            if (loc.Where(l => l.Item1 != l.Item2).Any())
            {
                return false;
            }

            // Next, look at the parameters and make sure they are also the same.
            var plist = _paramReplacesments.Zip(other._paramReplacesments, (u, t) => Tuple.Create(u, t)).Where(p => p.Item1.Item2 != _cppResult.RawValue);
            if (plist.Where(p => p.Item1.Item1 != p.Item2.Item1).Any())
            {
                return false;
            }
            if (plist.Where(p => p.Item1.Item2 != p.Item2.Item2).Any())
            {
                return false;
            }

            // OK, they are the same. The one thing that has to be changed is how the result variable
            // in the other guy is used below. So we need to make that the "same".
            if (_cppResult.RawValue != other._cppResult.RawValue)
            {
                optimize.ForceRenameVariable(other._cppResult.RawValue, _cppResult.RawValue);
            }
            return true;
        }

        /// <summary>
        /// Keep track of who is hosting us
        /// </summary>
        public IStatement Parent { get; set; }

        /// <summary>
        /// Keep track of unique names for this statement
        /// </summary>
        private Dictionary<string, string> _uniqueVariableTranslations = new Dictionary<string, string>();

        /// <summary>
        /// Remember a unique variable name that has been translated.
        /// </summary>
        /// <param name="varRepl"></param>
        /// <param name="uniqueTranslated"></param>
        internal void AddUniqueVariable(string varRepl, string uniqueTranslated)
        {
            _uniqueVariableTranslations.Add(varRepl, uniqueTranslated);
        }

        /// <summary>
        /// Can we move the other to look like this code?
        /// </summary>
        /// <param name="other"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        /// <remarks>
        /// We do not have to worry about unique variables since they are internal only, and never make it outside
        /// one of these statements.
        /// </remarks>
        public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst)
        {
            // Do some basic tests to try to fail early.
            if (!(other is CPPCodeStatement))
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }
            var s2 = other as CPPCodeStatement;

            if ((s2._linesOfCode.Count != _linesOfCode.Count)
                || (s2._uniqueVariableTranslations.Count != _uniqueVariableTranslations.Count)
                || (s2._paramReplacesments.Count != _paramReplacesments.Count))
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }

            // Lines of C++ code have to be identical (obviously).
            var differentCode = _linesOfCode
                .Zip(s2._linesOfCode, (u, t) => Tuple.Create(u, t))
                .Where(i => i.Item1 != i.Item2);
            if (differentCode.Any())
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }

            // First, handle the result.
            var renames = Tuple.Create(true, replaceFirst)
                .RequireForEquivForExpression(_cppResult.RawValue, s2._cppResult.RawValue);

            // Finally, we have to look at the parameters. We do this check in order.
            foreach (var pTwo in _paramReplacesments.Zip(s2._paramReplacesments, (u, t) => Tuple.Create(u, t)))
            {
                // Make sure parameter names are the same.
                if (pTwo.Item1.Item1 != pTwo.Item2.Item1)
                {
                    return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
                }

                renames = renames
                    .RequireForEquivForExpression(pTwo.Item1.Item2, DependentVariables, pTwo.Item2.Item2, s2.DependentVariables);
            }

            // If here, then we are set.
            return renames
                .ExceptFor(replaceFirst);
        }

        /// <summary>
        /// List of variables that we depend on for results
        /// </summary>
        public IEnumerable<string> DependentVariables { get; private set; }

        /// <summary>
        /// List of variables that we have that contain our results.
        /// </summary>
        public IEnumerable<string> ResultVariables { get; private set; }

        /// <summary>
        /// Return false - we are allowed to move.
        /// </summary>
        public bool NeverLift
        {
            get { return false; }
        }

        /// <summary>
        /// Build a code statement from the include files, the expression for the method call, and the generated lines of code.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="container"></param>
        /// <param name="includeFiles"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        public static IValue BuildCPPCodeStatement(MethodCallExpression expr, IGeneratedQueryCode gc, CompositionContainer container, string[] includeFiles, string[] loc)
        {
            // Get include files in.
            if (includeFiles != null)
            {
                foreach (var inc in includeFiles)
                {
                    gc.AddIncludeFile(inc);
                }
            }

            // Next, go after the lines of code. We have to first sort out what parameter names we are looking at,
            // and then force a translation of those parameters into simple values we can pass to the C++ code we
            // are going to pull back.
            var paramsTranslated = from p in expr.Arguments.Zip(expr.Method.GetParameters(), (arg, param) => Tuple.Create(arg, param))
                                   select new
                                   {
                                       Name = p.Item2.Name,
                                       Translated = ExpressionToCPP.InternalGetExpression(p.Item1, gc, null, container)
                                   };
            var paramLookup = paramsTranslated.ToDictionary(v => v.Name, v => v.Translated.ApplyParensIfNeeded());

            // Parse out the list of variables that are used. We will be passing these up the line as needed
            // so that we can tell how to optimize things.
            var dependents = new HashSet<string>(FindDeclarableParameters.FindAll(expr).Select(e => e.RawValue));

            // We also need a return variable. Since this can be multiple lines of code and we don't
            // know how the result will be used, we have to declare it up front... and pray they
            // use it correctly! :-)

            var cppResult = DeclarableParameter.CreateDeclarableParameterExpression(expr.Type);

            var cppStatement = new CPPCodeStatement(expr.Method, cppResult, loc, dependents);
            gc.Add(cppStatement);
            gc.Add(cppResult);

            paramLookup.Add(expr.Method.Name, cppResult.RawValue);

            var result = new ValSimple(cppResult.RawValue, expr.Type, DeclarableParameter.CreateDeclarableParameterExpression(cppResult.RawValue, expr.Type).AsArray());

            // Make sure a result exists in here! This at least will prevent some bad C++ code from getting generated!
            var lookForResult = new Regex(string.Format(@"\b{0}\b", expr.Method.Name));
            bool didReference = loc.Any(l => lookForResult.Match(l).Success);
            if (!didReference)
                throw new ArgumentException(string.Format("The C++ code attached to the method '{0}' doesn't seem to set a result.", expr.Method.Name));

            // Figure out if there are any Unique variables. If there are, then we need to do
            // a replacement on them.
            var findUnique = new Regex(@"\b\w*Unique\b");
            var varUniqueRequests = (from l in loc
                                     let matches = findUnique.Matches(l)
                                     from m in Enumerable.Range(0, matches.Count)
                                     select matches[m].Value).Distinct();
            foreach (var varRepl in varUniqueRequests)
            {
                var uniqueName = varRepl.Substring(0, varRepl.Length - "Unique".Length);
                var uniqueTranslated = uniqueName + _uniqueCounter.ToString();
                cppStatement.AddUniqueVariable(varRepl, uniqueTranslated);
                _uniqueCounter++;
            }

            // Add the parameters that need to be translated here.
            foreach (var paramName in paramLookup)
            {
                cppStatement.AddParamReplacement(paramName.Key, paramName.Value);
            }

            return result;
        }

        /// <summary>
        /// Count number of times a unique varaible name is used.
        /// </summary>
        static int _uniqueCounter = 0;
    }
}
