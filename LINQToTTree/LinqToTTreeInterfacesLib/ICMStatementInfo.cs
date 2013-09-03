
using System.Collections.Generic;
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Statement level information for code movement
    /// </summary>
    public interface ICMStatementInfo
    {
        /// <summary>
        /// List of variables that this statment depends on. These are considered
        /// the "input" variables for this statement.
        /// </summary>
        ISet<string> DependentVariables { get; }

        /// <summary>
        /// The resulting variables - things that this statement updates.
        /// </summary>
        ISet<string> ResultVariables { get; }
    }
}
