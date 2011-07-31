
using System;
using System.ComponentModel.Composition.Hosting;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
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

            ///
            /// Next, make sure the index variable can be used for later references!
            /// 

            var result = cc.Add(query, indexVar);
            cc.SetLoopVariable(indexVar);
            return result;
        }

        /// <summary>
        /// Search for a single variable name, that fills the whole string.
        /// </summary>
        static Regex gVarNameFinder = new Regex(@"^\b\w+\b$");

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
    }
}
