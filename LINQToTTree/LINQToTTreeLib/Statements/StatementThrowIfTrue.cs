using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Throw a runtime error if the IValue is true (it should be of type bool).
    /// </summary>
    class StatementThrowIfTrue : IStatement
    {
        private string _testValue;
        private string _message;

        public StatementThrowIfTrue(IValue valueWasSeen, string message)
        {
            this._testValue = valueWasSeen.RawValue;
            this._message = message;
        }

        public System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            yield return string.Format("if ({0}) {{", _testValue);
            yield return string.Format("  throw std::runtime_error(\"{0}\");", _message);
            yield return "}";
        }

        /// <summary>
        /// Variable renaming is pretty easy...
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            _testValue = _testValue.ReplaceVariableNames(originalName, newName);
        }

        /// <summary>
        /// We can combine only if the test strings are the same.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="optimize"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
        {
            if (!(statement is StatementThrowIfTrue))
            {
                return false;
            }
            var s = statement as StatementThrowIfTrue;
            var can = s._testValue == _testValue;

            return can;
        }

        /// <summary>
        /// Get the parent so we can climb.
        /// </summary>
        public IStatement Parent { get; set; }
    }
}
