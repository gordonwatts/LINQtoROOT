using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static LINQToTTreeLib.Optimization.OptimizationUtils;

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

            foreach (var f in result.Functions.Where(f => f.StatementBlock != null))
            {
                VisitOptimizableStatements(f.StatementBlock);
            }
        }

        /// <summary>
        /// Loop through optimizable statements
        /// </summary>
        /// <param name="statements"></param>
        /// <returns>If we have modified something that has bubble up one level, we return true. This means we've messed up the statement list at the outer level and the optimization loop needs to be re-run.</returns>
        private static bool VisitOptimizableStatements(IStatementCompound statements)
        {
            bool returnModified = false;

            bool modified = true;
            while (modified)
            {
                modified = false;
                var opter = new BlockRenamer(statements.FindBookingParent(), statements.FindBookingParent());
                foreach (var item in statements.Statements)
                {
                    // If it is a compound statement, there may be statements that are "invariant" in it,
                    // so we can lift them out.
                    if (item is IStatementCompound)
                    {
                        // If the statement is a compound statement, then we try to go down a level.
                        modified = VisitOptimizableStatements(item as IStatementCompound);
                    }

                    // Perhaps the whole statement could be pulled up?
                    if (!modified && (item is ICMStatementInfo))
                    {
                        // if we have optimize info, then see what we can do with it.
                        modified = BubbleUp(statements, item);
                        if (modified)
                            returnModified = true;
                    }

                    // Finally, check to see if this statement is identical to anyone else further up or not.
                    if (!modified)
                    {
                        modified = BubleUpAndCombine(statements, item, opter);
                    }

                    // If anything was modified, we need to re-run since all the various pointers, etc., will have
                    // been changed.
                    if (modified)
                        break;
                }
            }
            return returnModified;
        }

        /// <summary>
        /// See if we can't propagate this up, trying to combine it or bubble it up if there is something identical further up.
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="item"></param>
        /// <param name="opter"></param>
        /// <returns></returns>
        private static bool BubleUpAndCombine(IStatementCompound statements, IStatement item, ICodeOptimizationService opter)
        {
            return MoveFirstWithCombine(statements, item, opter);
        }

        /// <summary>
        /// Move to the first one, seeing if we can combine as we go.
        /// We will attempt two things:
        /// 1. Is the statement above the "same"? If so, try to eliminate the down-level statement.
        /// 2. Can it be combined?
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="item"></param>
        private static bool MoveFirstWithCombine(IStatementCompound statements, IStatement item, ICodeOptimizationService opter)
        {
            // First, move this forward as far as we can, and try to combine as we go.
            var previousStatements = statements.Statements.TakeWhile(s => s != item);

            // Now, see if we can move past each statement. If we can, see if they can be combined.
            foreach (var s in previousStatements.Reverse())
            {
                if (MakeStatmentsEquivalent(s, item))
                {
                    return true;
                }
                else if (StatementCommutes(s, item))
                {
                    if (s.TryCombineStatement(item, opter))
                    {
                        statements.Remove(item);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Given a statement that has bubble-up info in it, we will try to move
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

            // OK, now lets see if we can bubble it up one level.
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
            var requiredDeclared = declared.Select(p => p.RawValue).Intersect(inputs);
            if (requiredDeclared.Count() > 0)
                return false;

            // And does this statement idempotent? "a = a + 1" has the side effect of altering
            // a - so if it is repeated it won't have the same result. On the other hand,
            // "a = 10" will always have the same effect. The former should not be lifted
            // out of a loop.
            if (parent is IStatementLoop
                && !StatementIdempotent(item))
                return false;

            // OK, now insert it one level up, just before the parent. MoveStatementUpOneLevel
            return MoveStatementUpOneLevel(item);
        }

        /// <summary>
        /// Are we allowed to move this statement to be the first statement in this block?
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool MoveToFirst(IStatementCompound parent, IStatement item)
        {
            // If we can't move, then don't.
            if ((item as ICMStatementInfo).NeverLift)
                return false;

            // Loop through each statement, and see if we can go past each one.

            foreach (var cStatement in parent.Statements)
            {
                if (cStatement == item)
                    return true;
                if (!StatementCommutes(cStatement, item))
                {
                    return false;
                }
            }

            // That is weird. It never appeared here!
            throw new InvalidOperationException();
        }
    }
}
