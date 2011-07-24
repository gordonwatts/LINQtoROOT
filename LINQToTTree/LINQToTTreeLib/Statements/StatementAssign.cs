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
        public StatementAssign(IVariable result, IValue val)
        {
            if (result == null)
                throw new ArgumentNullException("Accumulator must not be zero");
            if (val == null)
                throw new ArgumentNullException("funcResolved must not be null");

            ResultVariable = result;
            Expression = val;
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
            var result = ResultVariable.RawValue;
            var setTo = Expression.RawValue;

            if (result != setTo)
                yield return result + "=" + setTo + ";";
        }

        public override string ToString()
        {
            return ResultVariable.RawValue + "=" + Expression.RawValue;
        }

        /// <summary>
        /// Check to see if this assign is the same or not
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public bool IsSameStatement(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement must not be null");

            var other = statement as StatementAssign;
            if (other == null)
                return false;

            return ResultVariable.RawValue == other.ResultVariable.RawValue
                && Expression.RawValue == other.Expression.RawValue;
        }

        /// <summary>
        /// Rename the assignment we are making.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            ResultVariable.RenameRawValue(originalName, newName);
            Expression.RenameRawValue(originalName, newName);
        }

        /// <summary>
        /// Try to combine two assign statements. Since this will be for totally
        /// trivial cases, this should be "easy" - only when they are the same.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var otherAssign = statement as StatementAssign;
            if (otherAssign == null)
                return false;

            if (ResultVariable.RawValue != otherAssign.ResultVariable.RawValue
                || Expression.RawValue != otherAssign.Expression.RawValue)
                return false;

            //
            // Combining is pretty trivial: these are identical. So we do nothing
            // adn the caller should drop this from their statement list!
            //

            return true;
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }
    }
}
