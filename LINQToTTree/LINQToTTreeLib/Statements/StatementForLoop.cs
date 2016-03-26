using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Implement the looping statements to work over some simple counter from zero up to some number.
    /// </summary>
    public class StatementForLoop : StatementInlineBlockBase, IStatementLoop, ICMCompoundStatementInfo, ICMStatementInfo
    {
        public IValue ArrayLength { get; set; }
        public IValue InitialValue { get; set; }

        IDeclaredParameter _loopVariable;

        /// <summary>
        /// Create a for loop statement.
        /// </summary>
        /// <param name="loopVariable">The variable that will be running in the loop</param>
        /// <param name="arraySizeVar">The size of the array. Evaluated just once before the loop is run.</param>
        /// <param name="startValue">Initial spot in array, defaults to zero</param>
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
                InitialValue = new ValSimple("0", typeof(int), null);

            if (ArrayLength.Type != typeof(int))
                throw new ArgumentException("arraySizeVar must be an integer");
            if (InitialValue.Type != typeof(int))
                throw new ArgumentException("startValue must be an integer");

            arrIndex = typeof(int).CreateUniqueVariableName();
        }

        /// <summary>
        /// Get back the index variables.
        /// </summary>
        public IEnumerable<IDeclaredParameter> LoopIndexVariable
        {
            get { return new IDeclaredParameter[] { _loopVariable }; }
        }

        /// <summary>
        /// Keep track of the array index name - and hold it constant no matter how many times we have to code
        /// things up! :-)
        /// </summary>
        private string arrIndex;

        /// <summary>
        /// Generate the code to do the looping. No need to generate anything if there is nothing to do! :-)
        /// </summary>
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            if (Statements.Any())
            {
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
            other.RenameVariable(other._loopVariable.ParameterName, _loopVariable.ParameterName);


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

        /// <summary>
        /// Test to see if this for loop is the same as the other statement. Identical, in a way that we can delete the "other" totally after
        /// appropriate renaming.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        public override Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null)
        {
            // Make sure it is a for statement.
            if (!(other is StatementForLoop))
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }
            var s2 = other as StatementForLoop;

            // Make sure the limit is the same, after applying the replacements.
            var renames = Tuple.Create(true, replaceFirst)
                .RequireForEquivForExpression(ArrayLength.RawValue, DependentVariables, s2.ArrayLength.RawValue, s2.DependentVariables);

            // Now, the initial list of renames includes the loop variable and anything handed to us.
            renames = renames
                .RequireForEquivForExpression(_loopVariable.RawValue, s2._loopVariable.RawValue);

            // And do everything in the block
            return RequiredForEquivalenceForBase(other, renames)
                .ExceptFor(replaceFirst);
        }

        /// <summary>
        /// Check to see if the statement commutes with our loop expression.
        /// </summary>
        /// <param name="followStatement"></param>
        /// <returns></returns>
        public override bool CommutesWithGatingExpressions(ICMStatementInfo followStatement)
        {
            var varInConflict = followStatement.ResultVariables.Intersect(ArrayLength.Dependants.Select(s => s.RawValue));
            return !varInConflict.Any();
        }

        /// <summary>
        /// Allow statements to automatically bubble up past us. Anything we
        /// can get rid of is a good thing!
        /// </summary>
        public override bool AllowNormalBubbleUp
        {
            get { return true; }
        }

        /// <summary>
        /// Return the list of declared variables.
        /// </summary>
        public override IEnumerable<IDeclaredParameter> DeclaredVariables
        {
            get
            {
                return base.DeclaredVariables
                    .Concat(new IDeclaredParameter[] { _loopVariable });
            }
        }

        /// <summary>
        /// Override AllDeclaredVariables to add on the variables we need here.
        /// </summary>
        public new ISet<string> AllDeclaredVariables
        {
            get
            {
                var r = new HashSet<string>(base.AllDeclaredVariables.Select(v => v.RawValue));
                r.Add(_loopVariable.RawValue);
                return r;
            }
        }

        /// <summary>
        /// Return a list of all dependent variables. Will not include the counter
        /// </summary>
        /// <remarks>We calculate this on the fly as we have no good way to know when we've been modified</remarks>
        public override ISet<string> DependentVariables
        {
            get
            {
                var dependents = base.DependentVariables
                    .Concat(ArrayLength.Dependants.Select(p => p.RawValue))
                    ;
                return new HashSet<string>(dependents);
            }
        }
    }
}
