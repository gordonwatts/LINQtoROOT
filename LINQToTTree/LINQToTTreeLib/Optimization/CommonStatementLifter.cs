using System.Linq;
using LinqToTTreeInterfacesLib;

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
            var sArr = new IStatement[] { s };
            foreach (var stack in codeStack)
            {
                if (stack.Combine(sArr, s.Parent as IBookingStatementBlock, false))
                {
                    parent.Remove(s);
                    return stack;
                }
            }
            return null;
        }
    }
}
