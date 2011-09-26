using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Increment an integer
    /// </summary>
    public class StatementIncrementInteger : IStatement
    {
        /// <summary>
        /// Create a statement that will increment this integer.
        /// </summary>
        /// <param name="i"></param>
        public StatementIncrementInteger(IDeclaredParameter i)
        {
            if (i == null)
                throw new ArgumentNullException("The statement can't increment a null integer");
            if (i.Type != typeof(int))
                throw new ArgumentException("parameter i must be an integer");

            Integer = i;
        }

        /// <summary>
        /// The integer we will increment.
        /// </summary>
        public IDeclaredParameter Integer { get; private set; }

        /// <summary>
        /// Return code to increment an integer
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            yield return Integer.RawValue + "++;";
        }

        /// <summary>
        /// If the variable name is known, then do the rename here.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            Integer.RenameRawValue(originalName, newName);
        }

        /// <summary>
        /// See if we can combine two integer combines. This works only if we have
        /// identical statements!
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var asint = statement as StatementIncrementInteger;
            if (asint == null)
                return false;

            if (asint.Integer.RawValue != Integer.RawValue)
                return false;

            //
            // We really don't have to do anything to combine. The
            // two statements are exactly identical. So, we can just
            // say "yes" and assume our caller will drop the combining.
            //

            return true;
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }
    }
}
