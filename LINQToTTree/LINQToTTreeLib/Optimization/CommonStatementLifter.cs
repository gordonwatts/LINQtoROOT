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
                VisitCodeBlock(block, new IStatementCompound[] { });
            }
        }

        /// <summary>
        /// Given a block of code, try to find statements that should be lifted up. For each statement
        /// that we find, see if there is one in the list that can "eat" it.
        /// </summary>
        /// <param name="block"></param>
        private static void VisitCodeBlock(IStatementCompound block, IStatementCompound[] codeStack)
        {
            var nextLevelStatementStack = codeStack.Concat(new IStatementCompound[] { block }).ToArray();

            foreach (var s in block.Statements)
            {
                if (s is IStatementCompound)
                {
                    // Go down a level...
                    VisitCodeBlock(s as IStatementCompound, nextLevelStatementStack);
                }
                else
                {
                    // This statement, can it be combined above us?
                    var whereDone = MoveStatement(block, s, codeStack);
                }
            }
        }

        /// <summary>
        /// We have a statement. Loop down through everything and see if we can't find
        /// another statement that will "take" it.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="codeStack"></param>
        /// <returns></returns>
        private static object MoveStatement(IStatementCompound parent, IStatement s, IStatementCompound[] codeStack)
        {
            foreach (var stack in codeStack)
            {

            }
            return null;
        }
    }
}
