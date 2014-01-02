
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Interface to implement a query model that has been turned into a routine.
    /// </summary>
    public interface IQMFunctionSource
    {
        /// <summary>
        /// The list of statements for this code block.
        /// Null until defined.
        /// </summary>
        IStatementCompound StatementBlock { get; }

        /// <summary>
        /// Set the code body for this result.
        /// </summary>
        /// <param name="statements">The statements that make up the body of the function.</param>
        /// <param name="expression">The result expression - what should be returned from the body.</param>
        void SetCodeBody(IStatementCompound statements, Expression resultExpression);

        /// <summary>
        /// The name of this function.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The return type of this function.
        /// </summary>
        Type ResultType { get; }

        /// <summary>
        /// The function arguments.
        /// </summary>
        IEnumerable<object> Arguments { get; }
    }
}
