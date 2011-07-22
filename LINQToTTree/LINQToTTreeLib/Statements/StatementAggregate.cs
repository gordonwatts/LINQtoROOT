using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        #region IStatement Members

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

        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var otherAssign = statement as StatementAggregate;
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

        #endregion
    }
}
