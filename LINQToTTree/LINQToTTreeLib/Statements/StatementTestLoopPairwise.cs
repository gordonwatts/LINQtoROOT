using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using System.Linq;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Simple statement that will code up the test for a pair-wise check. We do it
    /// in a separate statement like this because the "test" may well generate some
    /// code lines of its own - which will cause some scoping problems if we aren't
    /// careful!
    /// </summary>
    class StatementTestLoopPairwise : IStatement, ICMStatementInfo
    {
        private IDeclaredParameter _whatIsGood;
        private IValue _test;

        /// <summary>
        /// Create a loop pair wise test.
        /// </summary>
        /// <param name="passAll"></param>
        /// <param name="iValue"></param>
        public StatementTestLoopPairwise(IDeclaredParameter passAll, IValue iValue)
        {
            this._whatIsGood = passAll;
            this._test = iValue;
        }

        public System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            yield return string.Format("if (!({0}))", _test.RawValue);
            yield return "{";
            yield return string.Format("  {0}[index1] = false;", _whatIsGood.ParameterName);
            yield return string.Format("  {0}[index2] = false;", _whatIsGood.ParameterName);
            yield return "  break;";
            yield return "}";
        }

        /// <summary>
        /// Rename variables for this guy - pretty simple here!
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            _whatIsGood.RenameParameter(originalName, newName);
            _test.RenameRawValue(originalName, newName);
        }

        /// <summary>
        /// See what would be needed to combine these two things.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null)
        {
            var otherS = other as StatementTestLoopPairwise;
            if (otherS == null)
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());

            return Tuple.Create(true, replaceFirst)
                .RequireForEquivForExpression(_whatIsGood, otherS._whatIsGood)
                .RequireForEquivForExpression(_test, otherS._test)
                .ExceptFor(replaceFirst);
        }

        /// <summary>
        /// We can only combine if everything is the same!
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var other = statement as StatementTestLoopPairwise;
            if (other == null)
                return false;

            if (_whatIsGood.ParameterName != other._whatIsGood.ParameterName
                || _test.RawValue != _test.RawValue)
                return false;

            //
            // Combining - nothing to do here.
            //

            return true;
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }

        public IEnumerable<string> DependentVariables
        {
            get { return _test.Dependants.Concat(_whatIsGood.Dependants).Select(v => v.RawValue); }
        }

        /// <summary>
        /// We change the array, but that is it.
        /// </summary>
        public IEnumerable<string> ResultVariables
        {
            get { return _whatIsGood.Dependants.Select(v => v.RawValue); }
        }

        /// <summary>
        /// Ok to lift us out if we don't depend on anything!
        /// </summary>
        public bool NeverLift
        {
            get
            {
                return false;
            }
        }
    }
}
