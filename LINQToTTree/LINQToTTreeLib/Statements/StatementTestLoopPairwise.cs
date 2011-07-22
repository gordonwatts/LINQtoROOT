using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Simple statement that will code up the test for a pair-wise check. We do it
    /// in a seperate statement like this because the "test" may well generate some
    /// code lines of its own - which will cause some scoping problems if we aren't
    /// careful!
    /// </summary>
    class StatementTestLoopPairwise : IStatement
    {
        private Variables.VarArray _whatIsGood;
        private IValue _test;

        /// <summary>
        /// Create a loop pair wise test.
        /// </summary>
        /// <param name="passAll"></param>
        /// <param name="iValue"></param>
        public StatementTestLoopPairwise(VarArray passAll, IValue iValue)
        {
            this._whatIsGood = passAll;
            this._test = iValue;
        }

        public System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            yield return string.Format("if (!({0}))", _test.RawValue);
            yield return "{";
            yield return string.Format("  {0}[index1] = false;", _whatIsGood.RawValue);
            yield return string.Format("  {0}[index2] = false;", _whatIsGood.RawValue);
            yield return "  break;";
            yield return "}";
        }


        public bool IsSameStatement(IStatement statement)
        {
            throw new System.NotImplementedException();
        }

        public void RenameVariable(string originalName, string newName)
        {
            throw new System.NotImplementedException();
        }


        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            throw new System.NotImplementedException();
        }
    }
}
