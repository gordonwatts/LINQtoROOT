using LinqToTTreeInterfacesLib;
using System.Collections.Generic;
using LINQToTTreeLib.Utils;
using System;
using System.Linq;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Generate a return statement from a function.
    /// </summary>
    class StatementReturn : IStatement, ICMStatementInfo
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
            yield return string.Format("return {0};", _rtnValue);
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

        /// <summary>
        /// Can this get combined? Weird if it does, actually!
        /// </summary>
        /// <param name="other"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null)
        {
            var otherS = other as StatementReturn;
            if (otherS == null)
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }

            return Tuple.Create(true, replaceFirst)
                .RequireAreSame(_rtnValue, otherS._rtnValue)
                .ExceptFor(replaceFirst);
        }

        public IStatement Parent { get; set; }

        public IEnumerable<string> DependentVariables
        {
            get { return _rtnValue.Dependants.Select(v => v.RawValue); }
        }

        public IEnumerable<string> ResultVariables
        {
            get { return new string[0]; }
        }

        /// <summary>
        /// Well, we will see. ;-)
        /// </summary>
        public bool NeverLift
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Track the value of what we are going to actually return.
        /// </summary>
        public IValue _rtnValue;
    }
}
