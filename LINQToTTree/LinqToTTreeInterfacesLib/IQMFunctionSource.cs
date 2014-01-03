
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Interface to implement a query model that has been turned into a routine.
    /// </summary>
    public interface IQMFunctionSource : IQMFuncExecutable
    {
        /// <summary>
        /// Set the code body for this result.
        /// </summary>
        /// <param name="statements">The statements that make up the body of the function.</param>
        /// <param name="expression">The result expression - what should be returned from the body.</param>
        void SetCodeBody(IStatementCompound statements);
    }
}
