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
    }
}
