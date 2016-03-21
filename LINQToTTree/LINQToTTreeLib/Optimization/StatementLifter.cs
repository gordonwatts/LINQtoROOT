using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Linq;

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
                var opter = new BlockRenamer(statements);
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

        private static bool BubleUpAndCombine(IStatementCompound statements, IStatement item, ICodeOptimizationService opter)
        {
            return MoveFirstWithCombine(statements, item, opter);
        }

        /// <summary>
        /// Move to the first one, seeing if we can combine as we go.
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
                if (StatementCommutes(s, item))
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
        /// Can exchange the orders these statements are?
        /// </summary>
        /// <param name="s"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool StatementCommutes(IStatement s1, IStatement s2)
        {
            if (!(s1 is ICMStatementInfo))
                return false;
            if (!(s2 is ICMStatementInfo))
                return false;

            var c1Info = s1 as ICMStatementInfo;
            var c2Info = s2 as ICMStatementInfo;
            var c1Vars = new HashSet<string>(c1Info.ResultVariables.Concat(c1Info.DependentVariables));
            var c2Vars = new HashSet<string>(c2Info.ResultVariables.Concat(c2Info.DependentVariables));
            c1Vars.IntersectWith(c2Vars);
            if (c1Vars.Count > 0)
                return false;

            return true;
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

        /// <summary>
        /// Used to rename variables one level up
        /// </summary>
        private class BlockRenamer : ICodeOptimizationService
        {
            private IStatementCompound statements;

            public BlockRenamer(IStatementCompound statements)
            {
                this.statements = statements;
            }

            public void ForceRenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }

            public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
            {
                throw new NotImplementedException();
            }
        }
    }
}
