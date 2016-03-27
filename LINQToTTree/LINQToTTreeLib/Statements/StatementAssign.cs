using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;
using System.Linq;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Emit an assignment statement
    /// </summary>
    /// <remarks>
    /// We used to have an option to declare the variable inline. This was something that made code easier to write.
    /// However, it turns out when doing optimization, having code declared mid-block make determining data flow
    /// much more complex. So that was removed.
    /// </remarks>
    public class StatementAssign : IStatement, ICMStatementInfo
    {
        public StatementAssign(IDeclaredParameter result, IValue val)
        {
            if (result == null)
                throw new ArgumentNullException("Accumulator must not be zero");
            if (val == null)
                throw new ArgumentNullException("funcResolved must not be null");

            ResultVariable = result;
            Expression = val;
            ResultVariables = new HashSet<string>() { result.RawValue };
            DependentVariables = new HashSet<string>(val.Dependants.Select(v => v.RawValue));
        }

        /// <summary>
        /// The guy that will be set.
        /// </summary>
        public IDeclaredParameter ResultVariable { get; private set; }

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
            {
                yield return result + "=" + setTo + ";"; ;
            }
        }

        public override string ToString()
        {
            return ResultVariable.RawValue + "=" + Expression.RawValue;
        }

        /// <summary>
        /// Rename the assignment we are making.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            ResultVariable.RenameRawValue(originalName, newName);
            ResultVariables = new HashSet<string>() { ResultVariable.RawValue };
            Expression.RenameRawValue(originalName, newName);
            DependentVariables = DependentVariables.Select(p => p.ReplaceVariableNames(originalName, newName)).ToHashSet();
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

            if (opt == null)
                throw new ArgumentNullException("opt");

            var otherAssign = statement as StatementAssign;
            if (otherAssign == null)
                return false;

            if (Expression.RawValue != otherAssign.Expression.RawValue)
                return false;

            // If the statements are identical, then we can combine by default without having to do any
            // further work.

            if (otherAssign.ResultVariable.RawValue == ResultVariable.RawValue)
                return true;

            // If we have declared, then we are sole owner - so we can force the change. Otherwise, we
            // need to let the infrastructure figure out where the declared is and change it from there.

            return opt.TryRenameVarialbeOneLevelUp(otherAssign.ResultVariable.RawValue, ResultVariable);
        }

        /// <summary>
        /// Can we figure out a way to make the second statement look like the first one?
        /// </summary>
        /// <param name="other"></param>
        /// <returns>What should be changed in other to make it equivalent to this statement</returns>
        public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null)
        {
            // Well, if we can't we can't.
            if (!(other is StatementAssign))
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }
            var s2 = other as StatementAssign;

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

        /// <summary>
        /// List of all variables we are depending on.
        /// </summary>
        public IEnumerable<string> DependentVariables { get; private set; }

        /// <summary>
        /// The list of variables that get altered as a side-effect of this statement.
        /// </summary>
        public IEnumerable<string> ResultVariables { get; private set; }

        /// <summary>
        /// REturns false: we can move this statement if we want.
        /// </summary>
        public bool NeverLift
        {
            get { return false; }
        }
    }
}
