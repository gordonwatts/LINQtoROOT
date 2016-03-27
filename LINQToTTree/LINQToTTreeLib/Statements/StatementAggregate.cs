using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using System.Linq;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// When aggregate assigns something new, we code it up. While this is normally just a
    /// StatementAggregate, we need more intelligence about it when it comes to combining statements. Hence
    /// this version.
    /// </summary>
    public class StatementAggregate : IStatement, ICMStatementInfo
    {
        /// <summary>
        /// The guy that will be set.
        /// </summary>
        public DeclarableParameter ResultVariable { get; private set; }

        /// <summary>
        /// Get the expression that we will be making things equiv to!
        /// </summary>
        public IValue Expression { get; private set; }

        /// <summary>
        /// Create with the accumulator and the function resolved that we will use to do the translation.
        /// </summary>
        /// <param name="accumulator"></param>
        /// <param name="funcResolved"></param>
        /// <param name="dependentVariables">List of variables that the val statement depends on</param>
        public StatementAggregate(DeclarableParameter result, IValue val)
        {
            if (result == null)
                throw new ArgumentNullException("Accumulator must not be zero");
            if (val == null)
                throw new ArgumentNullException("funcResolved must not be null");

            ResultVariable = result;
            Expression = val;

            // Which variables we have as input and output

            DependentVariables = new HashSet<string>(val.Dependants.Select(v => v.RawValue));
            ResultVariables = new HashSet<string>() { result.RawValue };
        }

        public IEnumerable<string> CodeItUp()
        {
            var result = ResultVariable.ParameterName;
            var setTo = Expression.RawValue;

            yield return result + "=" + setTo + ";";
        }

        public override string ToString()
        {
            return ResultVariable.ParameterName + "=" + Expression.RawValue;
        }

        /// <summary>
        /// Rename the variables.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            ResultVariable.RenameParameter(originalName, newName);
            Expression.RenameRawValue(originalName, newName);
            DependentVariables = new HashSet<string>(DependentVariables.Select(s => s.ReplaceVariableNames(originalName, newName)));
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

            if (opt == null)
                throw new ArgumentNullException("opt");

            //
            // Simple case: everything is the same
            //

            if (ResultVariable.ParameterName == otherAssign.ResultVariable.ParameterName
                && Expression.RawValue == otherAssign.Expression.RawValue)
                return true;

            //
            // Next, see if we rename the accumulator everything would be identical
            //

            string tempRaw = Expression.RawValue.ReplaceVariableNames(ResultVariable.ParameterName, otherAssign.ResultVariable.ParameterName);
            if (tempRaw == otherAssign.Expression.RawValue)
            {
                // In order for this to work, we have to attempt to rename the variable that the other
                // guy owns. Since this variable is declared outside here, we have to call up in order
                // to have it run. Note that in this call it will call down into here to do the rename!

                return opt.TryRenameVarialbeOneLevelUp(otherAssign.ResultVariable.ParameterName, ResultVariable);
            }

            //
            // There is nothing else we can do to figure out if this is the same, I"m afraid!
            //

            return false;
        }

        /// <summary>
        /// See if we can make these two statements the same.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null)
        {
            if (!(other is StatementAggregate))
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }
            var s2 = other as StatementAggregate;

            return StatementUtils.MakeEquivalentSimpleExpressionAndResult(
                ResultVariable.RawValue, s2.ResultVariable.RawValue,
                Expression.RawValue, s2.Expression.RawValue,
                DependentVariables, s2.DependentVariables,
                replaceFirst);
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }

        public IEnumerable<string> DependentVariables { get; private set; }

        public IEnumerable<string> ResultVariables { get; private set; }

        public bool NeverLift { get { return true; } }
    }
}
