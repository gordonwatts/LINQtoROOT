using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Throw a runtime error if the IValue is true (it should be of type bool).
    /// </summary>
    class StatementThrowIfTrue : IStatement, ICMStatementInfo
    {
        private IValue _testValue;
        private string _message;

        public StatementThrowIfTrue(IValue valueWasSeen, string message)
        {
            this._testValue = valueWasSeen;
            this._message = message;
        }

        public System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            yield return string.Format("if ({0}) {{", _testValue.RawValue);
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
            _testValue.RenameRawValue(originalName, newName);
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
        /// Can we make these two things identical?
        /// </summary>
        /// <param name="other"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null)
        {
            var otherThrow = other as StatementThrowIfTrue;
            if (otherThrow == null)
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string,string>>());
            }

            if (_message != otherThrow._message)
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }

            // Last, see if the tests can be made the same.
            return Tuple.Create(true, replaceFirst)
                .RequireAreSame(_testValue, otherThrow._testValue)
                .ExceptFor(replaceFirst);
        }

        /// <summary>
        /// Get the parent so we can climb.
        /// </summary>
        public IStatement Parent { get; set; }

        /// <summary>
        /// What are our dependent variables?
        /// </summary>
        public IEnumerable<string> DependentVariables
        {
            get { return _testValue.Dependants.Select(v => v.RawValue); }
        }

        /// <summary>
        /// Result variables.
        /// </summary>
        public IEnumerable<string> ResultVariables
        {
            get { return new string[0]; }
        }

        /// <summary>
        /// Ok to lift, as long as we can move past that test expression out!
        /// </summary>
        public bool NeverLift
        {
            get { return false; }
        }
    }
}
