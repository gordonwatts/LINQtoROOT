using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Record everything into a map - pair-values for later lookup.
    /// </summary>
    public class StatementRecordPairValues : IStatement
    {
        private IDeclaredParameter _mapRecord;
        private IValue _index;
        private IValue _indexValue;

        /// <summary>
        /// Save how we are going to go after the statement and generate it.
        /// </summary>
        /// <param name="mapStorage"></param>
        /// <param name="indexVar"></param>
        /// <param name="indexValue"></param>
        public StatementRecordPairValues(IDeclaredParameter mapStorage, IValue indexVar, IValue indexValue)
        {
            //
            // Input checks.
            //

            if (mapStorage == null)
                throw new ArgumentNullException("mapStorage");
            if (indexVar == null)
                throw new ArgumentNullException("indexVar");
            if (indexValue == null)
                throw new ArgumentNullException("indexValue");

            //
            // Save for later
            //

            this._mapRecord = mapStorage;
            this._index = indexVar;
            this._indexValue = indexValue;
        }

        /// <summary>
        /// Dump out the code for this
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            yield return string.Format("{0}[{1}].push_back({2});", _mapRecord.RawValue, _index.RawValue, _indexValue.RawValue);
        }

        /// <summary>
        /// Rename a variable we are using.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            _index.RenameRawValue(originalName, newName);
            _indexValue.RenameRawValue(originalName, newName);
            _mapRecord.RenameRawValue(originalName, newName);
        }

        /// <summary>
        /// Can we combine? Yes, if and only if the same guy and same variables!
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="optimize"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
        {
            if (statement.GetType() != typeof(StatementRecordPairValues))
                return false;
            var other = statement as StatementRecordPairValues;
            if (other._index.RawValue != _index.RawValue)
                return false;
            if (other._indexValue.RawValue != _indexValue.RawValue)
                return false;
            if (other._mapRecord.RawValue != _mapRecord.RawValue)
                return false;
            return true;
        }

        /// <summary>
        /// Get/Set the statement we are sitting in.
        /// </summary>
        public IStatement Parent { get; set; }
    }
}
