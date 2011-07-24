using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// When aggregate assigns something new, we code it up. While this is normally just a
    /// StatementAggregate, we need more intelligence about it when it comes to combining statements. Hence
    /// this version.
    /// </summary>
    public class StatementAggregate : IStatement
    {
        /// <summary>
        /// The guy that will be set.
        /// </summary>
        public IVariable ResultVariable { get; private set; }

        /// <summary>
        /// Get the expression that we will be making things equiv to!
        /// </summary>
        public IValue Expression { get; private set; }

        /// <summary>
        /// Create with the accumulator and the function resolved that we will use to do the translation.
        /// </summary>
        /// <param name="accumulator"></param>
        /// <param name="funcResolved"></param>
        public StatementAggregate(IVariable result, IValue val)
        {
            if (result == null)
                throw new ArgumentNullException("Accumulator must not be zero");
            if (val == null)
                throw new ArgumentNullException("funcResolved must not be null");

            ResultVariable = result;
            Expression = val;
        }

        public IEnumerable<string> CodeItUp()
        {
            var result = ResultVariable.RawValue;
            var setTo = Expression.RawValue;

            yield return result + "=" + setTo + ";";
        }

        public override string ToString()
        {
            return ResultVariable.RawValue + "=" + Expression.RawValue;
        }

        public bool IsSameStatement(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement must not be null");

            var other = statement as StatementAggregate;
            if (other == null)
                return false;

            return ResultVariable.RawValue == other.ResultVariable.RawValue
                && Expression.RawValue == other.Expression.RawValue;
        }

        public void RenameVariable(string originalName, string newName)
        {
            ResultVariable.RenameRawValue(originalName, newName);
            Expression.RenameRawValue(originalName, newName);
        }

        /// <summary>
        /// Attempt to combine this statement. This is a little tricky. As it could be
        /// that the value we are accumulating is all that is different. In that case,
        /// we might be able to do the combination.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var otherAssign = statement as StatementAggregate;
            if (otherAssign == null)
                return false;

            //
            // Simple case: everything is the same
            //

            if (ResultVariable.RawValue == otherAssign.ResultVariable.RawValue
                || Expression.RawValue == otherAssign.Expression.RawValue)
                return true;

            if (ResultVariable.RawValue == otherAssign.ResultVariable.RawValue)
                return false;

            //
            // Next, see if we rename the accumulator everything would be identical
            //

            string tempRaw = Expression.RawValue.Replace(ResultVariable.RawValue, otherAssign.ResultVariable.RawValue);
            if (tempRaw == otherAssign.Expression.RawValue)
            {
                // In order for this to work, we have to attempt to rename the variable that the other
                // guy owns. Since this variable is declared outside here, we have to call up in order
                // to have it run. Note that in this call it will call down into here to do the rename!

                return opt.TryRenameVarialbeOneLevelUp(otherAssign.ResultVariable.RawValue, ResultVariable);
            }

            //
            // There is nothing else we cna do to figure out if this is the same, I"m afraid!
            //

            return true;
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }
    }
}
