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
                // There is one condition under which this combination should not happen:
                // The statements are identical and they are not idempotent. That means we would
                // have "a = a + 1" twice in a row, and we can't combine those.
                if ((r.Item2 == null || r.Item2.Count() == 0) && !StatementIdempotent(s2))
                {
                    return false;
                }

                // Ok, go after the parents to see if we can make sure there aren't common variables to be
                // declared.
                var s1Parent = s1.Parent as IBookingStatementBlock;
                var s2Parent = s2.Parent as IBookingStatementBlock;
                if (s1Parent == null || s2Parent == null)
                {
                    return false;
                }

                // We can't lift unless we can get at the declarations of all variables we want to rename.
                var allVarsDeclaredInS2 = s2Parent.AllDeclaredVariables.Select(v => v.RawValue).ToHashSet();
                var renames = r.Item2 == null ? Enumerable.Empty<Tuple<string, string>>() : r.Item2;
                if (!renames.Select(p => p.Item1).All(v => allVarsDeclaredInS2.Contains(v)))
                {
                    return false;
                }

                // Key to altering this is that this is the only site that allows modifications for these
                // variables that we are going to rename. If there are more than one, then we can't modify
                // unless we somehow know the modifications are identical.

                if (CheckForVariableAsResult(s2, renames.Select(i => i.Item1))
                    || CheckForVariableAsResult(s1, renames.Select(i => i.Item2)))
                {
                    return false;
                }

                // Remove the statement, and then do the renaming.
                var opt = new BlockRenamer(s2Parent, s1Parent);
                s2Parent.Remove(s2);
                foreach (var item in renames)
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
            // Check that we can get past each parental boundary.
            if (s2.WalkParents(false).Select(s => s.CheckForVariableAsInternalResult(variables)).Where(t => t).Any())
                return true;

            // Now check that we aren't also going to try to do something with a variable that is
            // being set somewhere else... that gets into grouping statements which is too much for this
            // optimizer. :-)
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

        /// <summary>
        /// Move a statement up one level, and put it right in front of the parent statement.
        /// Make sure to shift any declared variables as well.
        /// </summary>
        /// <param name="oldPparent"></param>
        /// <param name="item"></param>
        public static bool MoveStatementUpOneLevel(IStatement item)
        {
            // Next, the new parent one up had better be "good".
            if (item.Parent == null)
            {
                return false;
            }
            if (item.Parent.Parent == null && !(item.Parent.Parent is IStatementCompound))
            {
                return false;
            }

            return MoveStatement(item, item.Parent);
        }

        /// <summary>
        /// Move a statement from its old parent to its new parent. It will be removed from its current
        /// context, and put just before the other statement. Any variable decl that have to be moved will be
        /// moved. They are force-moved, so make sure it is safe before calling this method!
        /// </summary>
        /// <param name="s">The statement to move</param>
        /// <param name="insertBeforeThisStatement">The statement before which we should insert s</param>
        /// <returns></returns>
        public static bool MoveStatement(IStatement s, IStatement insertBeforeThisStatement)
        {
            // Get the current parent where we will remove it.
            var oldPparent = s.Parent as IStatementCompound;
            if (oldPparent == null)
                throw new InvalidOperationException("How can a statement's parent not be a compound statement?");

            // If there are declared variables, then we need to move them too.
            if (!MoveDeclaredResultsUp(oldPparent, s))
            {
                return false;
            }

            // Move the statement and put it in the next level up, just before
            // this parent.
            oldPparent.Remove(s);
            (oldPparent.Parent as IStatementCompound).AddBefore(s, oldPparent);
            return true;
        }

        /// <summary>
        /// If a result of a statement is declared in the parent block, then move it up one.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool MoveDeclaredResultsUp(IStatementCompound parent, IStatement item)
        {
            if (item is ICMStatementInfo && parent is IBookingStatementBlock)
            {
                var results = (item as ICMStatementInfo).ResultVariables;
                var booking = (parent as IBookingStatementBlock);
                var declaredVariables = new HashSet<string>(booking.DeclaredVariables.Select(p => p.RawValue));
                var declaredResults = results.Intersect(declaredVariables).ToArray();
                foreach (var varToMove in declaredResults)
                {
                    var declVarToMove = booking.DeclaredVariables.Where(p => p.RawValue == varToMove).First();
                    booking.Remove(declVarToMove);
                    if (!(AddBookingToParentOf(parent, declVarToMove)))
                    {
                        booking.Add(declVarToMove);
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Add a declaration to a booking parent, if there is one. Return true if we could.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="varToDeclare"></param>
        /// <returns></returns>
        private static bool AddBookingToParentOf(IStatementCompound s, IDeclaredParameter varToDeclare)
        {
            var parent = s.Parent;
            while (parent != null)
            {
                var book = parent as IBookingStatementBlock;
                if (book != null)
                {
                    book.Add(varToDeclare);
                    return true;
                }
            }

            return false;
        }
    }
}
