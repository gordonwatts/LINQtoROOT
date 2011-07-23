﻿using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// A statement that looks for a min/max
    /// </summary>
    public class StatementMinMaxTest : IStatement
    {
        private Variables.VarSimple vIsFilled;
        private Variables.VarSimple vMaxMin;
        private LinqToTTreeInterfacesLib.IValue exprToMinOrMaximize;
        private string CompareOperator;
        private IValue TempVariable;

        /// <summary>
        /// Create code block for a test
        /// </summary>
        /// <param name="vIsFilled"></param>
        /// <param name="vMaxMin"></param>
        /// <param name="exprToMinOrMaximize"></param>
        public StatementMinMaxTest(Variables.VarSimple vIsFilled, Variables.VarSimple vMaxMin, LinqToTTreeInterfacesLib.IValue exprToMinOrMaximize, bool doMax)
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


        public bool IsSameStatement(IStatement statement)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rename a variable
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            TempVariable.RenameRawValue(originalName, newName);
            exprToMinOrMaximize.RenameRawValue(originalName, newName);
            vIsFilled.RenameRawValue(originalName, newName);
            vMaxMin.RenameRawValue(originalName, newName);
        }


        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            throw new System.NotImplementedException();
        }
    }
}
