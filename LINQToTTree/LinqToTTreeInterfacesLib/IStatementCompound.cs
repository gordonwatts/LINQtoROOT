using System.Collections.Generic;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Any statement that implements this contains sub-statements.
    /// </summary>
    public interface IStatementCompound : IStatement
    {
        /// <summary>
        /// The list of statements that are in our "context"
        /// </summary>
        IEnumerable<IStatement> Statements { get; }

        /// <summary>
        /// Add a new statement to the compound block. It will be appended to the
        /// end of the block (after the last added statement).
        /// </summary>
        /// <param name="statement"></param>
        void Add(IStatement statement);

        /// <summary>
        /// Remove a statement from our list of statements.
        /// </summary>
        /// <param name="statement">Statement to remove - must be an exact object match.</param>
        void Remove(IStatement statement);

        /// <summary>
        /// Add a statement into the current set of statements, just before the given statement.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="beforeThisStatement"></param>
        void AddBefore(IStatement statement, IStatement beforeThisStatement);

        /// <summary>
        /// Given a set of statements, it will attempt to add each one to this block. However, it will first
        /// check to see if there is an "identical" statement. If so, it will just merge them.
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="parent"></param>
        /// <param name="appendIfNoCombine"></param>
        /// <returns>The "other" statement if the combination was successful, null if it was not</returns>
        bool Combine(IEnumerable<IStatement> statements, IBookingStatementBlock parent, bool appendIfNoCombine = true);

        /// <summary>
        /// Attempt to combine statements. If we can. If we can't, return null. If we can, return the statement it
        /// was folded into.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="parent"></param>
        /// <param name="appendIfNoCombine"></param>
        /// <returns></returns>
        IStatement CombineAndMark(IStatement statement, IBookingStatementBlock parent, bool appendIfNoCombine = true);

        /// <summary>
        /// Returns true if the statement first occurs before the statement second in the list.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>True if that is the case</returns>
        /// <remarks>Assume that both first and second are members of our current statement list. Results are not well defined
        /// if that isn't the case.</remarks>
        bool IsBefore(IStatement first, IStatement second);
    }
}
