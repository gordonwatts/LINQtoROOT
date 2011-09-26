using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// We record a value (like an int, etc.) when it comes by. We also mark a value as true once we've seen
    /// it. Finally, if asked, we also will do a break once we've seen the value once.
    /// </summary>
    public class StatementRecordValue : IStatement
    {
        /// <summary>
        /// The holder for the index variable.
        /// </summary>
        private IVariable _indexSeen;

        /// <summary>
        /// The value we should record above
        /// </summary>
        private IValue _indexValue;

        /// <summary>
        /// Set this to true when we have seen a first value.
        /// </summary>
        private IVariable _valueWasSeen;

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
        public StatementRecordValue(IVariable indexSaveLocation, IValue indexExpression,
            IVariable markWhenSeen, bool breakOnFirstSet)
        {
            if (indexSaveLocation == null)
                throw new ArgumentNullException("_indexSeen");
            if (indexExpression == null)
                throw new ArgumentNullException("indexExpression");
            if (markWhenSeen == null)
                throw new ArgumentNullException("markWhenSeen");

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

        /// <summary>
        /// Rename our variables
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            _indexSeen.RenameRawValue(originalName, newName);
            _indexValue.RenameRawValue(originalName, newName);
            _valueWasSeen.RenameRawValue(originalName, newName);
        }

        /// <summary>
        /// Combine these statements.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="optimize"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var other = statement as StatementRecordValue;
            if (other == null)
                return false;

            if (other._indexValue.RawValue != _indexValue.RawValue)
                return false;

            if (optimize == null)
                throw new ArgumentNullException("optimize");

            throw new NotImplementedException();
#if false
            optimize.TryRenameVarialbeOneLevelUp(other._indexSeen.RawValue, _indexSeen);
            optimize.TryRenameVarialbeOneLevelUp(other._valueWasSeen.RawValue, _valueWasSeen);
#endif

            return true;
        }

        public IStatement Parent { get; set; }
    }
}
