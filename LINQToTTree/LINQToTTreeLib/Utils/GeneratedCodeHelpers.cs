using LinqToTTreeInterfacesLib;
using System.Collections.Generic;

namespace LINQToTTreeLib.Utils
{
    /// <summary>
    /// Code to help deal with stacks, etc.
    /// </summary>
    static class GeneratedCodeHelpers
    {

        /// <summary>
        /// Find all used query variables that sit above us and have been currently being referenced.
        /// </summary>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="alreadySeen"></param>
        /// <param name="startStatement">The first statement to start our scan from</param>
        public static IEnumerable<IDeclaredParameter> GetUsedQuerySourceVariables(this IGeneratedQueryCode gc, IStatement startStatement, IDeclaredParameter alreadySeen = null)
        {
            var scope = startStatement as IStatement;
            HashSet<string> whatWeSaw = new HashSet<string>();
            if (alreadySeen != null)
                whatWeSaw.Add(alreadySeen.RawValue);

            while (scope != gc.CurrentResultScope)
            {
                if (scope is IStatementLoop)
                {
                    var ls = scope as IStatementLoop;
                    foreach (var loopIndexVar in ls.LoopIndexVariable)
                    {
                        if (!whatWeSaw.Contains(loopIndexVar.RawValue))
                        {
                            yield return loopIndexVar;
                            whatWeSaw.Add(loopIndexVar.RawValue);
                        }
                    }
                }
                scope = scope.Parent;
            }
        }
    }
}
