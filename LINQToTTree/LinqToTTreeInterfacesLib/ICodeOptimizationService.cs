
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Some helpful routines that can be used during code combinations
    /// </summary>
    public interface ICodeOptimizationService
    {
        /// <summary>
        /// Rename all variables in the block one level up from old to new.
        /// It is possible to fail - in which case false should be returned.
        /// </summary>
        /// <param name="oldName">The old variable which we shoudl find in the enclosing block and rename</param>
        /// <param name="newVariable">The new variable we want it renamed to</param>
        /// <returns>True if the renaming was allowed to proceed (and the renaming is complete)</returns>
        /// <remarks>
        /// If the new variable is an external result (i.e. not declared) then the rename can't proceed. Further,
        /// if the new variable is declared at a different level above the new holder than the old stataement above the older holder,
        /// that means the two are used for different things - so it can't procced, again.
        /// </remarks>
        bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable);

        /// <summary>
        /// Forces a rename of all text. No checking to find a declared variable, etc.
        /// This should be called if you own the decl in some private code block that
        /// is not known by the rest of the system.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        void ForceRenameVariable(string originalName, string newName);
    }
}
