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

        public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Allow statements to automatically bubble up past us. Anything we
        /// can get rid of is a good thing!
        /// </summary>
        public bool AllowNormalBubbleUp
        {
            get { return true; }
        }

        /// <summary>
        /// Return all declared variables in this guy
        /// </summary>
        public new ISet<string> DeclaredVariables
        {
            get
            {
                var r = new HashSet<string>(base.DeclaredVariables.Select(v => v.RawValue));
                r.Add(_loopVariable.RawValue);
                return r;
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
        public ISet<string> DependentVariables
        {
            get
            {
                var dependents = Statements
                    .Where(s => s is ICMStatementInfo)
                    .Cast<ICMStatementInfo>()
                    .SelectMany(s => s.DependentVariables)
                    .Where(v => v != _loopVariable.RawValue)
                    .Where(v => !DeclaredVariables.Contains(v))
                    ;
                return new HashSet<string>(dependents);
            }
        }

        /// <summary>
        /// A list of all the result variables
        /// </summary>
        public ISet<string> ResultVariables
        {
            get
            {
                var results = Statements
                    .Where(s => s is ICMStatementInfo)
                    .Cast<ICMStatementInfo>()
                    .SelectMany(s => s.ResultVariables)
                    .Where(v => !DeclaredVariables.Contains(v));
                return new HashSet<string>(results);
            }
        }

        /// <summary>
        /// As long as the dependent/result stuff is satisfied, then a lifting can occur no problem.
        /// </summary>
        public bool NeverLift
        {
            get { return false; }
        }
    }
}
