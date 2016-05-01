using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Record everything into a map - pair-values for later lookup.
    /// </summary>
    public class StatementRecordPairValues : IStatement, ICMStatementInfo
    {
        private IValue _index;

        struct saverInfo
        {
            public IDeclaredParameter mapRecord;
            public IValue indexValue;
        }

        private List<saverInfo> _savers = new List<saverInfo>();

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
                yield return string.Format("{0}[{1}].push_back({2});", saver.mapRecord.RawValue, _index.RawValue, saver.indexValue.RawValue);
            }
        }

        /// <summary>
        /// If we can make it all look the same...
        /// </summary>
        /// <param name="other"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null)
        {
            var otherS = other as StatementRecordPairValues;
            if (otherS == null)
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }
            if (_savers.Count != otherS._savers.Count)
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }

            var r = Tuple.Create(true, replaceFirst)
                .RequireForEquivForExpression(_index, otherS._index);

            foreach (var spair in _savers.Zip(otherS._savers, (us, them) => Tuple.Create(us, them)))
            {
                r = r
                    .RequireForEquivForExpression(spair.Item1.indexValue, spair.Item2.indexValue)
                    .RequireForEquivForExpression(spair.Item1.mapRecord, spair.Item2.mapRecord);
            }

            return r.ExceptFor(replaceFirst);
        }

        /// <summary>
        /// Rename a variable we are using.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            _index.RenameRawValue(originalName, newName);

            foreach (var saver in _savers)
            {
                saver.mapRecord.RenameRawValue(originalName, newName);
                saver.indexValue.RenameRawValue(originalName, newName);
            }
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

            var isTheSame = _savers.Zip(other._savers, (f, s) => f.indexValue.RawValue == s.indexValue.RawValue && f.mapRecord.Type == s.mapRecord.Type).All(b => b);

            // Now we can do them all.
            foreach (var saver in _savers.Zip(other._savers, (f, s) => Tuple.Create(f, s)))
            {
                optimize.TryRenameVarialbeOneLevelUp(saver.Item2.mapRecord.RawValue, saver.Item1.mapRecord);
            }

            return true;
        }

        /// <summary>
        /// Get/Set the statement we are sitting in.
        /// </summary>
        public IStatement Parent { get; set; }

        public IEnumerable<string> DependentVariables
        {
            get
            {
                var sIndex = _savers.SelectMany(s => s.indexValue.Dependants);
                var sMap = _savers.SelectMany(s => s.mapRecord.Dependants);
                return sIndex.Concat(sMap).Concat(new IValue[] { _index }).Select(v => v.RawValue);
            }
        }

        /// <summary>
        /// We only change the mapping variable.
        /// </summary>
        public IEnumerable<string> ResultVariables
        {
            get
            {
                return _savers.SelectMany(s => s.mapRecord.Dependants).Select(v => v.RawValue);
            }
        }

        /// <summary>
        /// Ok to lift if there are no crazy dependencies.
        /// </summary>
        public bool NeverLift
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<Tuple<string, string>> Emuerable { get; private set; }

        /// <summary>
        /// Add a new saver to the list of things we are saving.
        /// </summary>
        /// <param name="val">The value that we should be caching</param>
        /// <param name="mr">The map we should cache it into</param>
        internal void AddSaver(IDeclaredParameter mr, IValue val)
        {
            _savers.Add(new saverInfo() { mapRecord = mr, indexValue = val });
        }
    }
}
