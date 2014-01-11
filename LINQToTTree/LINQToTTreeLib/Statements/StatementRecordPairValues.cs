using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Record everything into a map - pair-values for later lookup.
    /// </summary>
    public class StatementRecordPairValues : IStatement
    {
        private IValue _index;
        private List<Tuple<IValue, IValue>> _savers = new List<Tuple<IValue, IValue>>();

        /// <summary>
        /// Save how we are going to go after the statement and generate it.
        /// </summary>
        /// <param name="mapStorage">We own this variable - we can change its name</param>
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

            this._index = indexVar;

            AddSaver(mapStorage, indexValue);

            Debug.WriteLine("Emit StatementRecordPairValues: IndexVar {0}, indexValue {1}", indexVar.ToString(), indexValue.ToString());
        }

        /// <summary>
        /// Dump out the code for this
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            foreach (var saver in _savers)
            {
                yield return string.Format("{0}[{1}].push_back({2});", saver.Item1.RawValue, _index.RawValue, saver.Item2.RawValue);
            }
        }

        /// <summary>
        /// Rename a variable we are using.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            throw new NotImplementedException();
#if false
            _index.RenameRawValue(originalName, newName);
            _indexValue.RenameRawValue(originalName, newName);
            _mapRecord.RenameRawValue(originalName, newName);
#endif
        }

        /// <summary>
        /// Can we combine? Yes, if and only if the same guy and same variables!
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="optimize"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
        {
            throw new NotImplementedException();
#if false
            if (statement.GetType() != typeof(StatementRecordPairValues))
                return false;
            var other = statement as StatementRecordPairValues;
            if (other._index.RawValue != _index.RawValue)
                return false;
            if (other._indexValue.RawValue != _indexValue.RawValue)
                return false;
            if (other._mapRecord.Type != _mapRecord.Type)
                return false;

            optimize.TryRenameVarialbeOneLevelUp(other._mapRecord.ParameterName, _mapRecord);

            return true;
#endif
        }

        /// <summary>
        /// Get/Set the statement we are sitting in.
        /// </summary>
        public IStatement Parent { get; set; }

        /// <summary>
        /// Add a new saver to the list of things we are saving.
        /// </summary>
        /// <param name="val">The value that we should be caching</param>
        /// <param name="mr">The map we should cache it into</param>
        internal void AddSaver(IValue mr, IValue val)
        {
            _savers.Add(Tuple.Create(mr, val));
        }
    }
}
