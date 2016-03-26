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

            // If the results of 1 will alter 2 or vice versa
            var c1DependsOnC2 = c1Info.DependentVariables.Intersect(c2Info.ResultVariables);
            if (c1DependsOnC2.Count() > 0)
                return false;

            var c2DependsOnC1 = c2Info.DependentVariables.Intersect(c1Info.ResultVariables);
            if (c2DependsOnC1.Count() > 0)
                return false;

            // If they both change the same variables, then we have an ordering problem.
            var resultsDependent = c1Info.ResultVariables.Intersect(c2Info.ResultVariables);
            if (resultsDependent.Count() > 0)
                return false;

            return true;
        }

        /// <summary>
        /// See if s1 commutes past s2.
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="statements"></param>
        /// <returns></returns>
        public static bool StatementCommutes(IStatement s1, IEnumerable<IStatement> statements)
        {
            foreach (var s in statements)
            {
                if (!(StatementCommutes(s1, s)))
                {
                    return false;
                }
            }
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

                // Key to altering this is that this is the only site that allows modifications for these
                // variables that we are going to rename. If there are more than one, then we can't modify
                // unless we somehow know the modifications are identical.

                if (CheckForVariableAsResult(s2, r.Item2.Select(i => i.Item1))
                    || CheckForVariableAsResult(s1, r.Item2.Select(i => i.Item2)))
                {
                    return false;
                }

                // Remove the statement, and then do the renaming.
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
        /// Given a statement, check to see if somewhere else the variable is modified or altered, both before or after
        /// the current statement.
        /// </summary>
        /// <param name="s2"></param>
        /// <param name="variables"></param>
        /// <returns>true if it was used as a result somewhere else, false otherwise</returns>
        private static bool CheckForVariableAsResult(IStatement s2, IEnumerable<string> variables)
        {
            if (s2.AllStatementsPrevious().Select(s => s.CheckForVariableAsReult(variables)).Where(t => t).Any())
                return true;

            if (s2.AllStatementsAfter().Select(s => s.CheckForVariableAsReult(variables)).Where(t => t).Any())
                return true;

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
