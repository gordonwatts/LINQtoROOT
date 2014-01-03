using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Variables;
using System;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// A statement that looks for a min/max
    /// </summary>
    public class StatementMinMaxTest : IStatement
    {
        private IDeclaredParameter vIsFilled;
        public IDeclaredParameter MaxMinVariable { get; set; }
        private IValue exprToMinOrMaximize;
        private string CompareOperator;
        private IDeclaredParameter TempVariable;

        /// <summary>
        /// Create code block for a test
        /// </summary>
        /// <param name="vIsFilled"></param>
        /// <param name="vMaxMin"></param>
        /// <param name="exprToMinOrMaximize"></param>
        public StatementMinMaxTest(IDeclaredParameter vIsFilled, IDeclaredParameter vMaxMin, IValue exprToMinOrMaximize, bool doMax)
        {
            // TODO: Complete member initialization
            this.vIsFilled = vIsFilled;
            this.MaxMinVariable = vMaxMin;
            this.exprToMinOrMaximize = exprToMinOrMaximize;

            if (doMax)
            {
                CompareOperator = ">";
            }
            else
            {
                CompareOperator = "<";
            }

            TempVariable = DeclarableParameter.CreateDeclarableParameterExpression(vMaxMin.Type);
        }

        /// <summary>
        /// Return the actual statements!
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            yield return string.Format("{0} {1} = {2};", TempVariable.Type.AsCPPType(), TempVariable.ParameterName, exprToMinOrMaximize.RawValue);
            yield return string.Format("if (!{0} || ({1} {2} {3})) {{", vIsFilled.ParameterName, TempVariable.ParameterName, CompareOperator, MaxMinVariable.ParameterName);
            yield return string.Format("  {0} = true;", vIsFilled.ParameterName);
            yield return string.Format("  {0} = {1};", MaxMinVariable.ParameterName, TempVariable.ParameterName);
            yield return "}";
        }

        /// <summary>
        /// Rename a variable
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            exprToMinOrMaximize.RenameRawValue(originalName, newName);
            vIsFilled.RenameParameter(originalName, newName);
            MaxMinVariable.RenameParameter(originalName, newName);
        }

        /// <summary>
        /// Try to combine two of these statements. The key is what we are trying to minimize or mazimize.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var other = statement as StatementMinMaxTest;
            if (other == null)
                return false;

            if (CompareOperator != other.CompareOperator
                || exprToMinOrMaximize.RawValue != other.exprToMinOrMaximize.RawValue)
                return false;

            //
            // Everything else is dependent - so we can just rename it.
            //

            var cando = opt.TryRenameVarialbeOneLevelUp(other.MaxMinVariable.ParameterName, MaxMinVariable);
            if (!cando)
                return false;
            cando = opt.TryRenameVarialbeOneLevelUp(other.vIsFilled.ParameterName, vIsFilled);
            if (!cando)
                throw new InvalidOperationException("Unable to rename second variable in a chain for Min/Max operator!");

            return true;
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }
    }
}
