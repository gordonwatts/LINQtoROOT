using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using static LINQToTTreeLib.Optimization.OptimizationUtils;

namespace LINQToTTreeLib.Optimization
{
    /// <summary>
    /// After a combine, sometimes an identical statement can exist at two different levels.
    /// The reason for this is that a statement can't really move past an if statement. However,
    /// this could mean the same thing is calculated twice. This optimization step looks for that,
    /// and removes the deeper one in scope.
    /// </summary>
    class CommonStatementLifter
    {
        public static void Optimize(IExecutableCode code)
        {
            foreach (var block in code.QueryCode())
            {
                VisitCodeBlock(block);
            }
        }

        /// <summary>
        /// Given a block of code, try to find statements that should be lifted up. For each statement
        /// that we find, see if there is one in the list that is "identical" - in short - already there.
        /// </summary>
        /// <param name="block">Examine this block for statements that can be lifted</param>
        /// <param name="codeStack">The list of parents where we might put this statement</param>
        private static void VisitCodeBlock(IStatementCompound block)
        {
            // Do decent first scan.
            foreach (var s in block.Statements.RetryIfModified())
            {
                if (s is IStatementCompound)
                {
                    VisitCodeBlock(s as IStatementCompound);
                }
            }

            // Now look at each item in the block and see if we can't find something above us that contains
            // the very same thing. In order to pop any statement up we must
            // 1. Be able to move it to the front of whatever block we are in
            // 2. Find a statement previous to this block above us that will combine with it.

            foreach (var s in block.Statements.RetryIfModified())
            {
                FindEquivalentAboveAndCombine(block, s);
            }
        }

        /// <summary>
        /// sToPop is a member of block, and can be or is the first statement. We see if we can pop it up
        /// a level, and then start a scan for an equivalent statement, marching up the list. If we
        /// find an equivalent statement, we will perform the combination and removal.
        /// </summary>
        /// <param name="block">The block of statements that holds sToPop</param>
        /// <param name="sToPop">The statement to try to up level.</param>
        /// <param name="previousStatements">Statements before this one in this block. This block might not actually contain this statement, in which case we must have this!</param>
        private static void FindEquivalentAboveAndCombine(IStatementCompound block, IStatement sToPop, IEnumerable<IStatement> previousStatements = null)
        {
            // If we can't get data flow information about a statement, then we can't do anything.
            var sInfo = sToPop as ICMStatementInfo;
            if (sInfo == null)
            {
                return;
            }

            // For this next step we need to fetch the list of statements above us. Either it has
            // been supplied to us, or we will have to generate it.
            if (previousStatements == null)
            {
                previousStatements = block.Statements.TakeWhile(s => s != sInfo).Reverse();
            }

            // Make sure we can get the statement to the top of the block. As we move it
            // forward, we want to also see if we can combine it in a straight-up way
            // with each statement as we go by it.
            foreach (var prevStatement in previousStatements)
            {
                if (MakeStatmentsEquivalent(prevStatement, sToPop))
                {
                    return;
                }
                if (!StatementCommutes(prevStatement, sToPop))
                {
                    return;
                }
            }

            // The statement can be moved to the top of the block, and isn't the same as
            // anything else we passed. Can we pull it out one level?
            // The key to answering this is: are all the variables it needs defined at the next
            // level up? And if not, are the missing ones simply declared down here and need to be moved up?
            var nParent = block.Parent.FindBookingParent();
            if (nParent == null)
            {
                return;
            }

            var sDependent = sInfo.DependentVariables;
            var availAtParent = nParent.AllDeclaredVariables.Select(n => n.RawValue).Intersect(sDependent);
            IEnumerable<string> declaredInBlock = Enumerable.Empty<string>();
            if (block is IBookingStatementBlock)
            {
                declaredInBlock = (block as IBookingStatementBlock).DeclaredVariables.Select(np => np.RawValue).Intersect(sDependent);
            }
            if ((availAtParent.Count() + declaredInBlock.Count()) != sDependent.Count())
            {
                return;
            }

            // If we are going to try to lift past a loop, we have to make sure the statement is idempotent.
            if (block is IStatementLoop && !StatementIdempotent(sToPop))
            {
                return;
            }

            // And the next figure out where we are in the list of statements.
            var nPrevStatements = nParent.Statements.TakeWhile(ps => ps != block).Reverse();

            // And repeat one level up with some tail recursion!
            FindEquivalentAboveAndCombine(nParent, sToPop, nPrevStatements);
        }

        /// <summary>
        /// See if we can move the statement to the front of the line.
        /// </summary>
        /// <param name="statementToMove"></param>
        /// <param name="statements"></param>
        /// <returns></returns>
        private static bool MoveFirst(IStatement statementToMove, IEnumerable<IStatement> statements)
        {
            // Walk backwards through previous statements
            var prevStatements = statements.TakeWhile(s => s != statementToMove).Reverse();

            // See if we can move.
            foreach (var s in prevStatements)
            {
                if (!StatementCommutes(s, statementToMove))
                {
                    return false;
                }
            }

            // If we can commute everywhere, then we are done!
            return true;
        }

        /// <summary>
        /// We have a statement. Loop down through everything and see if we can't find
        /// another statement that will "take" it.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="codeStack"></param>
        /// <returns></returns>
        private static IStatementCompound MoveStatement(IStatementCompound parent, IStatement s, IStatementCompound[] codeStack)
        {
            // Never move a statement that doesn't want to move. :-)

            if (s is ICMStatementInfo)
            {
                if ((s as ICMStatementInfo).NeverLift)
                    return null;
            }

            // Walk the code stack and find someone that can take us.

            foreach (var stack in codeStack)
            {
                var r = stack.CombineAndMark(s, s.Parent as IBookingStatementBlock, false);
                if (r != null)
                {
                    parent.Remove(s);

                    // During the CombineAndMark there is a chance that a declaration has been orphaned.
                    // If aBoolean_23 was declared in parent, it may also be declared up where it was moved to.
                    // If that is the case, we need to remove it.
                    if (parent is IBookingStatementBlock && r.Parent is IBookingStatementBlock)
                    {
                        var newParent = r.Parent as IBookingStatementBlock;
                        var oldParent = parent as IBookingStatementBlock;

                        var varsToRemove = from declVar in oldParent.DeclaredVariables
                                           from parVars in newParent.DeclaredVariables
                                           where declVar.ParameterName == parVars.ParameterName
                                           select declVar;
                        foreach (var declVar in varsToRemove.ToArray())
                        {
                            oldParent.Remove(declVar);
                        }
                    }

                    // We have to be a little careful here. The statement should be added at the right
                    // place. Since the statement works inside parent, and it works inside stack, then
                    // it should work in stack, just before parent.

                    var stackStatement = FindStatementHolder(stack, parent);
                    if (!stack.IsBefore(r, parent))
                    {
                        stack.Remove(r);
                        stack.AddBefore(r, parent);
                    }

                    return stack;
                }
            }
            return null;
        }

        /// <summary>
        /// s is in some code block in parent. Find the statement in parent that contains (or is) s.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private static IStatement FindStatementHolder(IStatementCompound parent, IStatement s)
        {
            var finalParent = s;
            while (finalParent != null && finalParent != parent)
            {
                finalParent = finalParent.Parent;
            }
            if (finalParent == null)
                throw new ArgumentException("Statement s does not seem to be contained in parent");
            return finalParent;
        }
    }
}
