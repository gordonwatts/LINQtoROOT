using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// We record a value (like an int, etc.) when it comes by. We also mark a value as true once we've seen
    /// it. Finally, if asked, we also will do a break once we've seen the value once.
    /// </summary>
    class StatemenRecordValue : IStatement
    {
        /// <summary>
        /// The holder for the index variable.
        /// </summary>
        private Variables.VarSimple _indexSeen;

        /// <summary>
        /// The value we should record above
        /// </summary>
        private IValue _indexValue;

        /// <summary>
        /// Set this to true when we have seen a first value.
        /// </summary>
        private Variables.VarSimple _valueWasSeen;

        /// <summary>
        /// If true, then break from our current loop once something has been seen.
        /// </summary>
        private bool _breakOnSeen;

        /// <summary>
        /// Create the statement block
        /// </summary>
        /// <param name="indexSeen"></param>
        /// <param name="indexValue"></param>
        /// <param name="valueWasSeen"></param>
        /// <param name="breakOnFirstSet"></param>
        public StatemenRecordValue(VarSimple indexSaveLocation, IValue indexExpression,
            VarSimple markWhenSeen, bool breakOnFirstSet)
        {
            // TODO: Complete member initialization
            this._indexSeen = indexSaveLocation;
            this._indexValue = indexExpression;
            this._valueWasSeen = markWhenSeen;
            this._breakOnSeen = breakOnFirstSet;
        }

        /// <summary>
        /// Render the code for this guy!
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            yield return string.Format("{0} = {1};", _indexSeen.RawValue, _indexValue.RawValue);
            yield return string.Format("{0} = true;", _valueWasSeen.RawValue);
            if (_breakOnSeen)
            {
                yield return "break;";
            }
        }
    }
}
