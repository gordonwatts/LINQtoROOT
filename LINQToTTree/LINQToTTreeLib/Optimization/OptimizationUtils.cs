using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Optimization
{
    class OptimizationUtils
    {
        /// <summary>
        /// Can exchange the orders these statements are?
        /// </summary>
        /// <param name="s"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool StatementCommutes(IStatement s1, IStatement s2)
        {
            if (!(s1 is ICMStatementInfo))
                return false;
            if (!(s2 is ICMStatementInfo))
                return false;

            var c1Info = s1 as ICMStatementInfo;
            var c2Info = s2 as ICMStatementInfo;
            var c1Vars = new HashSet<string>(c1Info.ResultVariables.Concat(c1Info.DependentVariables));
            var c2Vars = new HashSet<string>(c2Info.ResultVariables.Concat(c2Info.DependentVariables));
            var r = c1Vars.Intersect(c2Vars);
            if (r.Count() > 0)
                return false;

            return true;
        }

        /// <summary>
        /// See if we can make these two statements the same. If so, then bubble it up and go.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <remarks>
        /// A statement is considered equivalent if it would only take variable renames to adjust the
        /// statements to look identical.
        /// </remarks>
        public static bool MakeStatmentsEquivalent(IStatement s1, IStatement s2)
        {
            // Make sure the meta-data is present to work with.
            var sc1 = s1 as ICMStatementInfo;
            var sc2 = s2 as ICMStatementInfo;

            if (sc1 == null || sc2 == null)
            {
                return false;
            }

            // Now, see if they are equivalent. If so, perform the required rename.
            var r = sc1.RequiredForEquivalence(sc2);
            if (r.Item1)
            {
                var s1Parent = s1.Parent as IBookingStatementBlock;
                var s2Parent = s2.Parent as IBookingStatementBlock;
                if (s1Parent == null || s2Parent == null)
                {
                    return false;
                }

                // We can't lift unless we can get at the declarations of all variables we want to rename.
                var allVarsDeclaredInS2 = s2Parent.AllDeclaredVariables.Select(v => v.RawValue).ToHashSet();
                if (!r.Item2.Select(p => p.Item1).All(v => allVarsDeclaredInS2.Contains(v)))
                {
                    return false;
                }

                // Remove te statement, and then do the renaming.
                var opt = new BlockRenamer(s2Parent, s1Parent);
                s2Parent.Remove(s2);
                foreach (var item in r.Item2)
                {
                    opt.ForceRemoveDeclaration(item.Item1, s2Parent);
                    opt.ForceRenameVariable(item.Item1, item.Item2);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Check to see if the statement is idempotent.
        /// a = a + 1 is not
        /// a = 10 is
        /// </summary>
        /// <param name="s"></param>
        /// <returns>true if it is, false other wise. If we can't tell, we assume false</returns>
        public static bool StatementIdempotent(IStatement s)
        {
            if (!(s is ICMStatementInfo))
                return false;

            var sInfo = s as ICMStatementInfo;
            var sResults = sInfo.ResultVariables;
            var sDependents = sInfo.DependentVariables;
            var r = sResults.Intersect(sDependents);

            // Note: the intersection with a set of zero size is the original set.
            // TODO: Is this a .NET bug?
            return r.Count() == 0;
        }

    }
}
