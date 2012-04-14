using System.Collections.Generic;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// A block fo statements that has declarations at the start, and provides its own scoping
    /// (that means the variables go away as soon as the block is left).
    /// </summary>
    public interface IBookingStatementBlock : IStatementCompound
    {
        /// <summary>
        /// This will add a declaration and initalization code for that variable
        /// at the start of the block.
        /// </summary>
        /// <param name="variableToDeclare"></param>
        void Add(IDeclaredParameter variableToDeclare, bool failIfAlreadyPresent = true);

        /// <summary>
        /// Returns the list of variables that are declared in this compound block
        /// </summary>
        IEnumerable<IDeclaredParameter> DeclaredVariables { get; }
    }
}
