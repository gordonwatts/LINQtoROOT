
using System.Linq.Expressions;
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

        /// <summary>
        /// True if this is part of a sequence (QM ends with a select). False otherwise
        /// (QM ends with something like a First()).
        /// </summary>
        bool IsSequence { get; }

        /// <summary>
        /// Cache the variables that are at the center of the loop. In short, the data
        /// that is collected to run the sequence.
        /// </summary>
        /// <param name="loopIndexVariable">The variable that is counting (loop parameter)</param>
        /// <param name="loopExpression">The "selected" item - what appears in the select statement, and is dependent on the index variable.</param>
        void SequenceVariable(IDeclaredParameter loopIndexVariable, Expression loopExpression);

        /// <summary>
        /// Returns the index variable that was used when this sequence was collected.
        /// </summary>
        IDeclaredParameter OldLoopIndexVariable { get; }

        /// <summary>
        /// Returns the old loop expression when this sequence was collected (which should contain the old
        /// index variable.
        /// </summary>
        Expression OldLoopExpression { get; }
    }
}
