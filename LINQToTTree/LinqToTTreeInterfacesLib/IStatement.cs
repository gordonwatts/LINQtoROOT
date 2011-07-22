using System.Collections.Generic;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Any statement that is to be translated needs to be stored here. These are strictly C++
    /// statements, and not meant to be language agnostic.
    /// </summary>
    public interface IStatement
    {
        /// <summary>
        /// Generate the C++ code. If, eventually, we want to make this lang agnostic, we need
        /// to pass in an object that is responsible for changing info into statements...
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> CodeItUp();

        /// <summary>
        /// Returns true if executing this statement and the one passed in would be
        /// redundant. That is - they do the same thing, store their results in the same
        /// place, etc.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        bool IsSameStatement(IStatement statement);

        /// <summary>
        /// A variable rename is being done from above. If the variable appears
        /// in our statement we will rename it.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        void RenameVariable(string originalName, string newName);

        /// <summary>
        /// Try to combine the statement with this one. If possible, it should
        /// be appended, or combined with statements that are already in here. If
        /// the return is true, then the statement can be ignored as it has been
        /// "abosrbed" into this one.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize);
    }
}
