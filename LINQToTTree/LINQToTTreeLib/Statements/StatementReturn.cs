using LinqToTTreeInterfacesLib;
using System.Collections.Generic;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Generate a return statement from a function.
    /// </summary>
    class StatementReturn : IStatement
    {
        /// <summary>
        /// Create a return statement.
        /// </summary>
        /// <param name="rtnValue"></param>
        public StatementReturn(IValue rtnValue)
        {
            _rtnValue = rtnValue;
        }

        public IEnumerable<string> CodeItUp()
        {
            yield return string.Format("return {0}", _rtnValue);
        }

        public void RenameVariable(string originalName, string newName)
        {
            _rtnValue.RenameRawValue(originalName, newName);
        }

        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
        {
            var otherRtn = statement as StatementReturn;
            if (otherRtn == null)
                return false;
            return otherRtn._rtnValue.RawValue == _rtnValue.RawValue;
        }

        public IStatement Parent { get; set; }

        /// <summary>
        /// Track the value of what we are going to actually return.
        /// </summary>
        public IValue _rtnValue;
    }
}
