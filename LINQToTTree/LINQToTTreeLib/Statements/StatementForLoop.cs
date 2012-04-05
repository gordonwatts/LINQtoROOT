using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib.Expressions;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Implement the looping statements to work over some simple counter from zero up to some number.
    /// </summary>
    public class StatementForLoop : StatementInlineBlockBase, IStatementLoop
    {
        public IValue ArrayLength { get; set; }
        public IValue InitialValue { get; set; }

        IDeclaredParameter _loopVariable;

        /// <summary>
        /// Create a for loop statement.
        /// </summary>
        /// <param name="loopVariable"></param>
        /// <param name="arraySizeVar"></param>
        /// <param name="startValue">Inital spot in array, defaults to zero</param>
        public StatementForLoop(IDeclaredParameter loopVariable, IValue arraySizeVar, IValue startValue = null)
        {
            if (loopVariable == null)
                throw new ArgumentNullException("loopVariable");
            if (arraySizeVar == null)
                throw new ArgumentNullException("arraySizeVar");

            ArrayLength = arraySizeVar;
            _loopVariable = loopVariable;
            InitialValue = startValue;
            if (InitialValue == null)
                InitialValue = new ValSimple("0", typeof(int));

            if (ArrayLength.Type != typeof(int))
                throw new ArgumentException("arraySizeVar must be an integer");
            if (InitialValue.Type != typeof(int))
                throw new ArgumentException("startValue must be an integer");

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
                yield return string.Format("for (int {0}={2}; {0} < {1}; {0}++)", _loopVariable.RawValue, arrIndex, InitialValue.RawValue);
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

            // This shouldn't happen... we own the loop variable!!
            if (other._loopVariable == _loopVariable)
                throw new InvalidOperationException("Loop variables identical in attempt to combine StatementForLoop!");

            // If we are looping over the same thing, then we can combine.

            if (other.ArrayLength.RawValue != ArrayLength.RawValue)
                return false;
            if (other.InitialValue.RawValue != InitialValue.RawValue)
                return false;

            // We need to rename the loop variable in the second guy

            // Are they the same? _index is independent and we can alter it.
            if (!(opt.TryRenameVarialbeOneLevelUp(other._loopVariable.RawValue, _loopVariable)))
                    return false;


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
            InitialValue.RenameRawValue(origName, newName);
            _loopVariable.RenameParameter(origName, newName);
            RenameBlockVariables(origName, newName);
        }
    }
}
