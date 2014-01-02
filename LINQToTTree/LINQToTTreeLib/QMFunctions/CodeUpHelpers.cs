using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;
using System.Collections.Generic;

namespace LINQToTTreeLib.QMFunctions
{
    /// <summary>
    /// Code up a function
    /// </summary>
    public static class CodeUpHelpers
    {
        /// <summary>
        /// Emit C++ code for a function
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<string> CodeItUp(this IQMFuncExecutable func)
        {
            // The header and decl.
            yield return string.Format("// {0} - {1}", func.Name, func.QueryModelText);
            yield return string.Format("{0} {1} ()", func.ResultType.AsCPPType(), func.Name);
            foreach (var l in func.StatementBlock.CodeItUp())
            {
                yield return "  " + l;
            }

            yield return "";
        }
    }
}
