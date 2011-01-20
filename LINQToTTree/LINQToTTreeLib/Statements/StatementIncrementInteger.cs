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
    }
}
