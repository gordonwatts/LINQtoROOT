using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// We record a value (like an int, etc.) when it comes by. We also mark a value as true once we've seen
    /// it. Finally, if asked, we also will do a break once we've seen the value once.
    /// </summary>
    public class StatementRecordValue : IStatement, ICMStatementInfo
    {
        /// <summary>
        /// The holder for the index variable.
        /// </summary>
        private IDeclaredParameter _indexSeen;

        /// <summary>
        /// The value we should record above
        /// </summary>
        private IValue _indexValue;

        /// <summary>
        /// Set this to true when we have seen a first value.
        /// </summary>
        private IDeclaredParameter _valueWasSeen;

        /// <summary>
        /// If true, then only record the first value we see.
        /// </summary>
        private bool _recordOnlyFirstValue;

        /// <summary>
        /// Create the statement block
        /// </summary>
        /// <param name="indexSeen"></param>
        /// <param name="indexValue"></param>
        /// <param name="valueWasSeen"></param>
        /// <param name="recordOnlyFirstValue"></param>
        public StatementRecordValue(IDeclaredParameter indexSaveLocation, IValue indexExpression,
            IDeclaredParameter markWhenSeen, bool recordOnlyFirstValue)
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
            this._recordOnlyFirstValue = recordOnlyFirstValue;
        }

        /// <summary>
        /// Render the code for this guy!
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            string indent = "";
            if (_recordOnlyFirstValue)
            {
                yield return string.Format("if (!{0}) {{", _valueWasSeen.RawValue);
                indent = "  ";
            }
            yield return string.Format("{2}{0} = {1};", _indexSeen.RawValue, _indexValue.RawValue, indent);
            yield return string.Format("{1}{0} = true;", _valueWasSeen.RawValue, indent);
            if (_recordOnlyFirstValue)
            {
                yield return "}";
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

            if (other._recordOnlyFirstValue != _recordOnlyFirstValue)
                return false;

            if (optimize == null)
                throw new ArgumentNullException("optimize");

            optimize.TryRenameVarialbeOneLevelUp(other._indexSeen.RawValue, _indexSeen);
            optimize.TryRenameVarialbeOneLevelUp(other._valueWasSeen.RawValue, _valueWasSeen);

            return true;
        }

        /// <summary>
        /// Get/Set the compound statement this is embeded in.
        /// </summary>
        public IStatement Parent { get; set; }

        /// <summary>
        /// What are the varsa that we need as input.
        /// </summary>
        public ISet<string> DependentVariables
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// What are teh variables that are a result.
        /// </summary>
        public ISet<string> ResultVariables
        {
            get { throw new NotImplementedException(); }
        }


        /// <summary>
        /// This particular statement should never be lifted. Ever.
        /// </summary>
        public bool NeverMove
        {
            get { return true; }
        }
    }
}
