using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Emit an assignment statement
    /// </summary>
    public class StatementAssign : IStatement, ICMStatementInfo
    {
        public StatementAssign(IDeclaredParameter result, IValue val, IEnumerable<IDeclaredParameter> dependentVariables, bool declare = false)
        {
            if (result == null)
                throw new ArgumentNullException("Accumulator must not be zero");
            if (val == null)
                throw new ArgumentNullException("funcResolved must not be null");

            ResultVariable = result;
            Expression = val;
            DeclareResult = declare;
            ResultVariables = new HashSet<string>() { result.RawValue };
            var dvars = new HashSet<string>();
            if (dependentVariables != null)
            {
                foreach (var item in dependentVariables)
                {
                    dvars.Add(item.RawValue);
                }
            }
            DependentVariables = dvars;
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
                var line = "";
                if (DeclareResult)
                {
                    line += ResultVariable.Type.AsCPPType() + " ";
                }
                line += result + "=" + setTo + ";";
                yield return line;
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

            if (opt == null)
                throw new ArgumentNullException("opt");

            var otherAssign = statement as StatementAssign;
            if (otherAssign == null)
                return false;

            if (Expression.RawValue != otherAssign.Expression.RawValue)
                return false;

            if (DeclareResult != otherAssign.DeclareResult)
                return false;

            // If we have delcared, then we are sole owner - so we can force the change. Otherwise, we
            // need to let the infrastructure figure out where the decl is and change it from there.

            if (DeclareResult)
            {
                opt.ForceRenameVariable(otherAssign.ResultVariable.RawValue, ResultVariable.RawValue);
                return true;
            }
            else
            {
                return opt.TryRenameVarialbeOneLevelUp(otherAssign.ResultVariable.RawValue, ResultVariable);
            }
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }

        /// <summary>
        /// True if we should declare this result when we emit it the assignment.
        /// </summary>
        public bool DeclareResult { get; set; }

        /// <summary>
        /// List of all variables we are depending on.
        /// </summary>
        public ISet<string> DependentVariables { get; private set; }

        /// <summary>
        /// The list of variables that get altered as a side-effect of this statement.
        /// </summary>
        public ISet<string> ResultVariables { get; private set; }

        /// <summary>
        /// REturns false: we can move this statement if we want.
        /// </summary>
        public bool NeverMove
        {
            get { return false; }
        }
    }
}
