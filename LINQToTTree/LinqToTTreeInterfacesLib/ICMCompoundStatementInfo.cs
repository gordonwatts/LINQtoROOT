
using System.Collections.Generic;
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Code Movement information for a compound statement. If the statement knows how to deal, it should also
    /// implement the CMStatementInfo.
    /// </summary>
    public interface ICMCompoundStatementInfo
    {
        /// <summary>
        /// True if normal bubble up of statements is allowed. For example, for loops would allow this,
        /// but if statements wouldn't. Doesn't mean it can't happen, just that it isn't recommended to
        /// let it happen naturally without other pointers indicating it should.
        /// </summary>
        bool AllowNormalBubbleUp { get; }

        /// <summary>
        /// All variables declared by this guy.
        /// </summary>
        IEnumerable<IDeclaredParameter> DeclaredVariables { get; }

        /// <summary>
        /// Blocks, like if statements or for statements, have a statement that is at the top of the block.
        /// Sometimes we need to move an expression beyond that, and double check it can just commute with
        /// that expression.
        /// </summary>
        /// <param name="followStatement"></param>
        /// <returns></returns>
        bool CommutesWithGatingExpressions(ICMStatementInfo followStatement);
    }
}
