using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;

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
        public StatementIncrementInteger(VarInteger i)
        {
            if (i == null)
                throw new ArgumentNullException("The statement can't increment a null integer");

            Integer = i;
        }

        /// <summary>
        /// The integer we will increment.
        /// </summary>
        public VarInteger Integer { get; private set; }

        /// <summary>
        /// Return code to increment an integer
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            yield return Integer.RawValue + "++;";
        }


        /// <summary>
        /// Are we identical statements, one after the other?
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public bool IsSameStatement(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("Statement to compare to must not be null");
            var other = statement as StatementIncrementInteger;
            if (other == null)
                return false;

            return Integer.RawValue == other.Integer.RawValue;
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
        public bool TryCombineStatement(IStatement statement)
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
    }
}
