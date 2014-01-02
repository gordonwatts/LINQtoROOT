
using System.Collections.Generic;
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Provides the interface to access executable code
    /// </summary>
    public interface IExecutableCode
    {
        /// <summary>
        /// List of variables to transfer over that are required to run for the queries
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> VariablesToTransfer { get; }

        /// <summary>
        /// The final result of this query.
        /// </summary>
        IEnumerable<IDeclaredParameter> ResultValues { get; }

        /// <summary>
        /// This include file should be added to the list of include files for
        /// this run in C++.
        /// </summary>
        /// <param name="includeName"></param>
        void AddIncludeFile(string includeName);

        /// <summary>
        /// Return a list of all the include files that need to be added to make this
        /// code run.
        /// </summary>
        IEnumerable<string> IncludeFiles { get; }

        /// <summary>
        /// Each iteration returns a new query code block.
        /// </summary>
        IEnumerable<IStatementCompound> QueryCode();

        /// <summary>
        /// Returns the list of TTree leaves referenced by this query
        /// </summary>
        IEnumerable<string> ReferencedLeafNames { get; }

        /// <summary>
        /// Returns the list of functions that can be executed.
        /// </summary>
        IEnumerable<IQMFuncExecutable> Functions { get; }
    }
}
