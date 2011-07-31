using System;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// A statement that looks for a min/max
    /// </summary>
    public class StatementMinMaxTest : IStatement
    {
        private IVariable vIsFilled;
        private IVariable vMaxMin;
        private IValue exprToMinOrMaximize;
        private string CompareOperator;
        private IVariable TempVariable;

        /// <summary>
        /// Create code block for a test
        /// </summary>
        /// <param name="vIsFilled"></param>
        /// <param name="vMaxMin"></param>
        /// <param name="exprToMinOrMaximize"></param>
        public StatementMinMaxTest(IVariable vIsFilled, IVariable vMaxMin, IValue exprToMinOrMaximize, bool doMax)
        {
            // TODO: Complete member initialization
            this.vIsFilled = vIsFilled;
            this.vMaxMin = vMaxMin;
            this.exprToMinOrMaximize = exprToMinOrMaximize;

            if (doMax)
            {
                CompareOperator = ">";
            }
            else
            {
                CompareOperator = "<";
            }

            TempVariable = new Variables.VarSimple(vMaxMin.Type);
        }

        /// <summary>
        /// Return the actual statements!
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            yield return string.Format("{0} {1} = {2};", TempVariable.Type.AsCPPType(), TempVariable.RawValue, exprToMinOrMaximize.RawValue);
            yield return string.Format("if (!{0} || ({1} {2} {3})) {{", vIsFilled.RawValue, TempVariable.RawValue, CompareOperator, vMaxMin.RawValue);
            yield return string.Format("  {0} = true;", vIsFilled.RawValue);
            yield return string.Format("  {0} = {1};", vMaxMin.RawValue, TempVariable.RawValue);
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
            vIsFilled.RenameRawValue(originalName, newName);
            vMaxMin.RenameRawValue(originalName, newName);
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

            var cando = opt.TryRenameVarialbeOneLevelUp(other.vMaxMin.RawValue, vMaxMin);
            if (!cando)
                return false;
            cando = opt.TryRenameVarialbeOneLevelUp(other.vIsFilled.RawValue, vIsFilled);
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
