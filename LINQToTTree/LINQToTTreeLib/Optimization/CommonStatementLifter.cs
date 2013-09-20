using System.Linq;
using LinqToTTreeInterfacesLib;
using System;

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
                while (VisitCodeBlock(block, new IStatementCompound[] { }) != null)
                    ;
            }
        }

        /// <summary>
        /// Given a block of code, try to find statements that should be lifted up. For each statement
        /// that we find, see if there is one in the list that can "eat" it.
        /// </summary>
        /// <param name="block"></param>
        private static IStatementCompound VisitCodeBlock(IStatementCompound block, IStatementCompound[] codeStack)
        {
            var nextLevelStatementStack = codeStack.Concat(new IStatementCompound[] { block }).ToArray();

            bool rerun = true;
            while (rerun)
            {
                rerun = false;
                foreach (var s in block.Statements)
                {
                    if (s is IStatementCompound)
                    {
                        // If it is not a loop, try to move it.

                        if (!(s is IStatementLoop))
                        {
                            var whereDone = MoveStatement(block, s, codeStack);
                            if (whereDone != null)
                                return whereDone;
                        }

                        // Go down a level...
                        var r = VisitCodeBlock(s as IStatementCompound, nextLevelStatementStack);
                        if (r != null)
                        {
                            if (r != block)
                                return r;
                            if (r == block)
                            {
                                rerun = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // This statement, can it be combined above us?
                        var whereDone = MoveStatement(block, s, codeStack);
                        if (whereDone != null)
                            return whereDone;
                    }
                }
            }
            return null;
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

                    // We have to be a little careful here. The statement should be added at the right
                    // place. Since thet statement works inside parent, and it works inside stack, then
                    // it should work in stack, just before parent.

                    var stackStatement = FindStatementHolder(stack, parent);
                    stack.Remove(r);
                    stack.AddBefore(r, parent);                        

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
