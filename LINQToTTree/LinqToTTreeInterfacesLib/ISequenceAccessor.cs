
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Usually adorns an IValue or IVariable and allows one to
    /// to loop over the variable. In short - it assumes that whatever
    /// is getting returned is a sequence and this guy knows how to iterate
    /// over the sequence.
    /// </summary>
    public interface ISequenceAccessor
    {
        /// <summary>
        /// Add a loop to the statement list for this guy, using the indexname as the
        /// thing that will do the iteration. All statements, etc., should be done for this.
        /// </summary>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="indexName"></param>
        void AddLoop(IGeneratedCode env, ICodeContext context, string indexName);
    }
}
