using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Optimization
{
    /// <summary>
    /// Code to optimize a sequence of statements by doing its best to lift a statement out
    /// of an inner loop that doesn't depend on anything in the inner loop.
    /// </summary>
    class StatementLifter
    {
        /// <summary>
        /// Find all statements that can be lifted out of their current block, and bubble them up
        /// as much as possible.
        /// </summary>
        /// <param name="result"></param>
        /// <remarks>
        /// We only lift things outside of loops, we don't lift them outside of if statements.
        /// If statements are an efficiency thing on their own - they should prevent expensive tests
        /// from happening if they don't need to.
        /// </remarks>
        internal static void Optimize(IGeneratedQueryCode result)
        {
            var statements = result.CodeBody as IStatementCompound;
            VisitOptimizableStatements(statements);
        }

        /// <summary>
        /// Loop through optimizable statements
        /// </summary>
        /// <param name="statements"></param>
        private static bool VisitOptimizableStatements(IStatementCompound statements)
        {
            bool modified = true;
            bool returnModified = false;
            while (modified)
            {
                modified = false;
                foreach (var item in statements.Statements)
                {
                    if (item is IStatementCompound)
                    {
                        // If the statement is a compound statement, then we try to go down a level.
                        modified = VisitOptimizableStatements(item as IStatementCompound);
                    }
                    else if (item is ICMStatementInfo)
                    {
                        // if we have optimize info, then see what we can do with it.
                        modified = BubbleUp(statements, item);
                        if (modified)
                            returnModified = true;
                    }

                    // If anything was modified, we need to re-run since all the various pointers, etc., will have
                    // been changed.

                    if (modified)
                        break;

                    // Now, we can only go on if we have any hope of moving statements further down (like the next one)
                    // up past this statement we've just looked at. So, break if we don't have any code movement
                    // info for it.

                    if (!(item is ICMStatementInfo))
                        break;
                }
            }
            return returnModified;
        }

        /// <summary>
        /// Given a statement that has buble-up info in it, we will try to move
        /// it up one.
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="item"></param>
        /// <remarks>For now only support bubbling up one level</remarks>
        private static bool BubbleUp(IStatementCompound parent, IStatement item)
        {
            // First, is the parent statement that we can bubble up past?
            var cmpInfo = parent as ICMCompoundStatementInfo;
            if (cmpInfo == null)
                return false;
            if (!cmpInfo.AllowNormalBubbleUp)
                return false;

            // Next job - if this isn't the first statement, then we have to
            // be able to move it past all the other statements.

            if (!MoveToFirst(parent, item))
            {
                return false;
            }

            // Finally, does this guy need anything from the compound statement?

            var declared = cmpInfo.DeclaredVariables;
            var inputs = (item as ICMStatementInfo).DependentVariables;
            declared.IntersectWith(inputs);
            if (declared.Count > 0)
                return false;

            // Ok, now insert it one level up, just before the parent.
            return MoveStatement(parent, item);
        }

        /// <summary>
        /// Move a statement up one level, and put it right in front of the parent statement.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="item"></param>
        private static bool MoveStatement(IStatementCompound parent, IStatement item)
        {
            if (parent.Parent != null && (parent.Parent is IStatementCompound))
            {
                parent.Remove(item);
                (parent.Parent as IStatementCompound).AddBefore(item, parent);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Are we allowed to move this statement to be the first statement in this block?
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool MoveToFirst(IStatementCompound parent, IStatement item)
        {
            // Loop through each statement, and see if we can go past each one.

            var allvars = new HashSet<string>((item as ICMStatementInfo).DependentVariables.Concat((item as ICMStatementInfo).ResultVariables));
            foreach (var cStatement in parent.Statements)
            {
                if (cStatement == item)
                    return true;
                if (!(cStatement is ICMStatementInfo))
                    return false;

                var cInfo = cStatement as ICMStatementInfo;
                var cVars = new HashSet<string>(cInfo.ResultVariables.Concat(cInfo.DependentVariables));
                cVars.IntersectWith(allvars);
                if (cVars.Count > 0)
                    return false;
            }

            // That is weird. It never appeared here!
            throw new InvalidOperationException();
        }
    }
}
