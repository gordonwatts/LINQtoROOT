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
        public static IEnumerable<IStatementCompound> WalkParents(this IStatement s, bool includeThisStatment = false)
        {
            bool skipAStatement = !includeThisStatment;
            while (s != null)
            {
                if (!skipAStatement)
                {
                    if (s is IStatementCompound)
                    yield return s as IStatementCompound;
                }
                skipAStatement = false;

                s = s.Parent;
            }
        }

        /// <summary>
        /// Find a parent that is a booking block. Return null otherwise.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IBookingStatementBlock FindBookingParent(this IStatement s)
        {
            return s.WalkParents(true)
                .Where(m => m is IBookingStatementBlock)
                .Cast<IBookingStatementBlock>()
                .FirstOrDefault();
        }
    }
}
