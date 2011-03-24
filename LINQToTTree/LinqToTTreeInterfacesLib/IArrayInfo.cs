using System.Linq.Expressions;
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Represents info about an array - everything we would need to use with it.
    /// </summary>
    public interface IArrayInfo
    {
        /// <summary>
        /// Generate statements to loop over this array. We will generate a unique iterator (often an integer) and
        /// make sure there is a mapping from the indexName (which is locally used) to that unique index name.
        /// </summary>
        /// <param name="env">Code block we can add code to</param>
        /// <param name="context">Context so we can add parameters and other things</param>
        /// <param name="indexName">The name of the index that is being used by the expression we are calling.</param>
        /// <param name="popVariableContext">So things can be popped off the parameter list when not required (scoping)</param>
        /// <returns>An expression that references this item of the loop</returns>
        Expression AddLoop(IGeneratedCode env, ICodeContext context);
    }
}
