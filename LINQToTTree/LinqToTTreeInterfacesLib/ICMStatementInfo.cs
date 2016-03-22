
using System;
using System.Collections.Generic;
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Statement level information for code movement
    /// </summary>
    public interface ICMStatementInfo
    {
        /// <summary>
        /// List of variables that this statement depends on. These are considered
        /// the "input" variables for this statement.
        /// </summary>
        ISet<string> DependentVariables { get; }

        /// <summary>
        /// The resulting variables - things that this statement updates.
        /// </summary>
        ISet<string> ResultVariables { get; }

        /// <summary>
        /// This statement should never be optimized and moved by moving it outside an existing for or if block. Ever.
        /// </summary>
        /// <remarks>
        /// This is used for things that have side effects - for example, the First or Last operators,
        /// where you want to record stuff in a loop. That should never get moved up or down.
        /// </remarks>
        bool NeverLift { get; }

        /// <summary>
        /// Return if this statement and the other statement are equivalent, without altering either one.
        /// </summary>
        /// <param name="other">The other statement to compare to</param>
        /// <param name="replaceFirst">These substitutions must be made before any others</param>
        /// <returns>True if this statement can be made equivalent, and if true, a list of variable renames required on other to make it equivalent.</returns>
        Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null);
    }
}
