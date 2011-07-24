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
        /// Since we know nothing, this is a trival statement!
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public bool IsSameStatement(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement must not be null");
            return statement is StatementBreak;
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }
    }
}
