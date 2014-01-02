
using System;
using System.Collections.Generic;
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Executable code for a function.
    /// </summary>
    public interface IQMFuncExecutable
    {
        /// <summary>
        /// Returns true if the other function is evaluating the same query model as this one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool Matches(IQMFuncExecutable other);

        /// <summary>
        /// Returns the name of the function
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Rename any internal references to a function from one to the other.
        /// </summary>
        /// <param name="oldfname"></param>
        /// <param name="newfname"></param>
        void RenameFunctionReference(string oldfname, string newfname);

        /// <summary>
        /// Return the QM string this function represents.
        /// </summary>
        string QueryModelText { get; }

        /// <summary>
        /// The return type of this function.
        /// </summary>
        Type ResultType { get; }

        /// <summary>
        /// The function arguments.
        /// </summary>
        IEnumerable<IQMArgument> Arguments { get; }

        /// <summary>
        /// The list of statements for this code block.
        /// Null until defined.
        /// </summary>
        IStatementCompound StatementBlock { get; }
    }
}
