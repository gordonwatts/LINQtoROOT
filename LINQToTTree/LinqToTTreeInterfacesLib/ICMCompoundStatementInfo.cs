
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
        /// but if statements wouldn't. Dosen't mean it can't happen, just that it isn't recommended to
        /// let it happen naturally without other pointers indicating it should.
        /// </summary>
        bool AllowNormalBubbleUp { get; }

        /// <summary>
        /// All variables declared by this guy.
        /// </summary>
        ISet<string> DeclaredVariables { get; }
    }
}
