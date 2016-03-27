
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

        /// <summary>
        /// Return a list of result variables that are part of the statement. The loop counter in a for loop, for example.
        /// </summary>
        /// <remarks>
        /// For example,
        /// if you have "for (i = 0; i != 10; i++)" you then have i as an internal result variable.
        /// It can be folded out such that there is a "i = i + 1" at the end of the list of statements.
        /// This is important when one is trying to move statements out of the loop: no statement will set
        /// the loop counter as a result variable (or BAD), but if they depend on it, then we really
        /// can't lift it past without ramifications. On the other hand, if you trying to lift the whole
        /// for statement, you don't care about the loop counter as it is "hidden".
        /// </remarks>
        IEnumerable<IDeclaredParameter> InternalResultVarialbes { get; }
    }
}
