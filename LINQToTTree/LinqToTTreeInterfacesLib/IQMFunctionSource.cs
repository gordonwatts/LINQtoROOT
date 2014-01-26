
using System.Collections.Generic;
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
        /// Returns the index variable that was used when this sequence was collected.
        /// </summary>
        IDeclaredParameter OldLoopIndexVariable { get; }

        /// <summary>
        /// Returns the old loop expression when this sequence was collected (which should contain the old
        /// index variable.
        /// </summary>
        Expression OldLoopExpression { get; }

        /// <summary>
        /// Called to cache an expression. Returns a set of statements that can be used to record
        /// the values for the expression. If this is a sequence expression, then the loop index variable
        /// needs to be provided as well.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="loopIndexVariable"></param>
        /// <returns></returns>
        IEnumerable<IStatement> CacheExpression(Expression expression, IDeclaredParameter loopIndexVariable = null);
    }
}
