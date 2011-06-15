using System.Collections.Generic;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Emit an assignment statement
    /// </summary>
    public class StatementAssign : IStatement
    {
        public StatementAssign(ParameterExpression accumulator, IValue funcResolved)
        {
            ResultVariable = accumulator;
            Expression = funcResolved;
        }

        /// <summary>
        /// The guy that will be set.
        /// </summary>
        public ParameterExpression ResultVariable { get; private set; }

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
            var result = ResultVariable.Name;
            var setTo = Expression.RawValue;

            if (result != setTo)
                yield return result + "=" + setTo + ";";
        }

        public override string ToString()
        {
            return ResultVariable + "=" + Expression.RawValue;
        }
    }
}
