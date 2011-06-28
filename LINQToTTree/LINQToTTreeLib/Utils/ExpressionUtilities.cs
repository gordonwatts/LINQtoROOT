
using System;
using System.ComponentModel.Composition.Hosting;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
namespace LINQToTTreeLib.Utils
{
    internal static class ExpressionUtilities
    {
        /// <summary>
        /// Given array info, code a loop over it.
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="arrayRef"></param>
        public static IVariableScopeHolder CodeLoopOverArrayInfo(this IArrayInfo arrayRef, string indexName, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            var indexVar = arrayRef.AddLoop(gc, cc, container);

            ///
            /// Next, make sure the index variable can be used for later references!
            /// 

            var result = cc.Add(indexName, indexVar);
            cc.SetLoopVariable(indexVar);
            return result;
        }

        static Regex varName = new Regex(@"^\b\w+\b$");

        /// <summary>
        /// Given a value, see if it is not a single term. If not, add parens.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ApplyParensIfNeeded(this IValue val)
        {
            if (val == null)
                throw new ArgumentNullException("Value must not be null");

            var rv = val.RawValue;
            var match = varName.Match(rv);
            if (match.Success)
                return val.RawValue;
            return "(" + val.RawValue + ")";
        }

    }
}
