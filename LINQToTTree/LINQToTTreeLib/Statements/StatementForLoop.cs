using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Implement the looping statements to work over some simple counter from zero up to some number.
    /// </summary>
    public class StatementForLoop : StatementInlineBlockBase, IStatementLoop
    {
        public IValue ArrayLength { get; set; }
        string _loopVariable;

        /// <summary>
        /// Create a for loop statement.
        /// </summary>
        /// <param name="loopVariable"></param>
        /// <param name="arraySizeVar"></param>
        public StatementForLoop(string loopVariable, IValue arraySizeVar)
        {
            ArrayLength = arraySizeVar;
            _loopVariable = loopVariable;
        }

        /// <summary>
        /// Generate the code to do the looping. No need to generate anything if there is nothing to do! :-)
        /// </summary>
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            if (Statements.Any())
            {
                var arrIndex = typeof(int).CreateUniqueVariableName();
                yield return string.Format("int {0} = {1};", arrIndex, ArrayLength.RawValue);
                yield return string.Format("for (int {0}=0; {0} < {1}; {0}++)", _loopVariable, arrIndex);
                foreach (var l in RenderInternalCode())
                {
                    yield return l;
                }
            }
        }

        /// <summary>
        /// We need to try to combine statements here.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var other = statement as StatementForLoop;
            if (other == null)
                return false;

            // If we are looping over the same thing, then we can combine.

            if (other.ArrayLength.RawValue != ArrayLength.RawValue)
                return false;

            // We need to rename the loop variable in the second guy

            other.RenameVariable(other._loopVariable, _loopVariable);

            // Combine everything

            Combine(other, opt);

            return true;
        }

        /// <summary>
        /// Rename all the variables in this block
        /// </summary>
        /// <param name="origName"></param>
        /// <param name="newName"></param>
        public override void RenameVariable(string origName, string newName)
        {
            ArrayLength.RenameRawValue(origName, newName);
            _loopVariable = _loopVariable.ReplaceVariableNames(origName, newName);
            RenameBlockVariables(origName, newName);
        }
    }
}
