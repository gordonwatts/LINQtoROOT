using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;
using System;
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
            throw new NotImplementedException();
            //yield return string.Format("{0} {1};", func.CacheVariable.Type.AsCPPType(), func.CacheVariable.RawValue);
            yield return string.Format("{0} {1};", func.CacheVariableGood.Type.AsCPPType(), func.CacheVariableGood.RawValue);
            yield return string.Format("{0} {1} ()", func.ResultType.AsCPPType(), func.Name);
            foreach (var l in func.StatementBlock.CodeItUp())
            {
                yield return "  " + l;
            }

            yield return "";
        }
    }
}
