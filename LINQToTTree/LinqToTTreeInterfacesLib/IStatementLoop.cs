
using System.Collections.Generic;
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Mixin interface to denote a particular statement as a loop
    /// </summary>
    public interface IStatementLoop
    {
        /// <summary>
        /// Returns whatever loop index variable is being used.
        /// </summary>
        IEnumerable<IDeclaredParameter> LoopIndexVariable { get; }
    }
}
