using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Someone is using a break statement to pop out of a loop.
    /// </summary>
    class StatementBreak : IStatement
    {
        public IEnumerable<string> CodeItUp()
        {
            yield return "break;";
        }

        /// <summary>
        /// There is nothing to do here since we know nothing about variables.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
        }


        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException();

            var other = statement as StatementBreak;
            if (other == null)
                return false;

            return true;
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }
    }
}
