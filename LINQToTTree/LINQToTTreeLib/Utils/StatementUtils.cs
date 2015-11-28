using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Utils
{
    public static class StatementUtils
    {
        /// <summary>
        /// Return the list of parents until we hit null.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IEnumerable<IStatementCompound> WalkParents(this IStatement s)
        {
            while (s != null)
            {
                var p = s.Parent;
                if (p != null && p is IStatementCompound)
                {
                    yield return p as IStatementCompound;
                }
                s = p;
            }
        }
    }
}
