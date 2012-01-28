
using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using Remotion.Linq.Clauses;
namespace LINQToTTreeLib.Utils
{
    internal static class ExpressionUtilities
    {
        /// <summary>
        /// Given array info, code a loop over it.
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="arrayRef"></param>
        public static IVariableScopeHolder CodeLoopOverArrayInfo(this IArrayInfo arrayRef, IQuerySource query, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            var indexVar = arrayRef.AddLoop(gc, cc, container);
            if (indexVar == null)
                return null;

            ///
            /// Next, make sure the index variable can be used for later references!
            /// 

            var result = cc.Add(query, indexVar.Item1);
            cc.SetLoopVariable(indexVar.Item1, indexVar.Item2);
            return result;
        }

        /// <summary>
        /// Search for a single variable name, that fills the whole string.
        /// </summary>
        static Regex gVarNameFinder = new Regex(@"^\b\w+\b$");

        static Regex gNumberFinder = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");

        /// <summary>
        /// Given a value, see if it is not a single term. If not, add parens.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ApplyParensIfNeeded(this IValue val)
        {
            if (val == null)
                throw new ArgumentNullException("Value must not be null");
            return val.RawValue.ApplyParensIfNeeded();
        }

        /// <summary>
        /// Look at a string and decide if it contains something that looks like one or more terms.
        /// </summary>
        /// <param name="rv"></param>
        /// <returns></returns>
        public static string ApplyParensIfNeeded(this string rv)
        {
            if (string.IsNullOrWhiteSpace(rv))
                throw new ArgumentNullException("Value must not be null");

            // If it is a single variable then we don't need to protect it
            // when used in an expression.

            var match = gVarNameFinder.Match(rv);
            if (match.Success)
                return rv;

            // If it is just a number or similar, then match that

            if (gNumberFinder.Match(rv).Success)
                return rv;

            // Special case where we already have this thing surrounded by parens.

            if (rv[0] == '(')
            {
                int depth = 1;
                int indexer = 1;
                while (depth != 0 && indexer != rv.Length)
                {
                    if (rv[indexer] == '(')
                        depth++;
                    if (rv[indexer] == ')')
                        depth--;
                    indexer++;
                }
                if (indexer == rv.Length)
                    return rv;
            }

            // Protect it from "later" use.

            return "(" + rv + ")";
        }

        /// <summary>
        /// Is this something like:
        ///   obj.jets where jets is an array that points off to a "regrouped" variable? If so, then we
        ///   don't want to do the translation here.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsRootObjectArrayReference(this MemberExpression expression)
        {
            if (!expression.Type.IsArray)
                return false;

            if (expression.Expression.Type.TypeHasAttribute<TranslateToClassAttribute>() == null)
                return false;

            if (TypeUtils.TypeHasAttribute<TTreeVariableGroupingAttribute>(expression.Member) == null)
                return false;

            return true;
        }

        /// <summary>
        /// If this is a parameter of some sort, returns the name. Otherwise, throws.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string ParameterName(this Expression expr)
        {
            var lv = expr as ParameterExpression;
            if (lv != null)
                return lv.Name;

            var dv = expr as DeclarableParameter;
            if (dv == null)
                throw new InvalidOperationException("Unable to look at loop index variable that isn't a parameter");
            return dv.ParameterName;
        }

    }
}
