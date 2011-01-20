using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Emit an assignment statement
    /// </summary>
    public class StatementAssign : IStatement
    {
        public StatementAssign(IVariable accumulator, IValue funcResolved)
        {
            ResultVariable = accumulator;
            Expression = funcResolved;
        }
        /// <summary>
        /// The guy that will be set.
        /// </summary>
        public IVariable ResultVariable { get; private set; }

        /// <summary>
        /// Get the expression that we will be making things equiv to!
        /// </summary>
        public IValue Expression { get; private set; }

        /// <summary>
        /// Return code for this statement
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            throw new NotImplementedException();
        }
    }
}
