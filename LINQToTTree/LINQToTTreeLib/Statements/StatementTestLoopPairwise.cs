using System;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Simple statement that will code up the test for a pair-wise check. We do it
    /// in a separate statement like this because the "test" may well generate some
    /// code lines of its own - which will cause some scoping problems if we aren't
    /// careful!
    /// </summary>
    class StatementTestLoopPairwise : IStatement
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
    }
}
