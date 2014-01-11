
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// A satement that will sort over the arguments to the map variable, and then loop
    /// over them - using as an index the map target.
    /// </summary>
    public class StatementLoopOverSortedPairValue : StatementInlineBlockBase, IStatementLoop
    {
        private bool _sortAscending;

        /// <summary>
        /// Save everything for later work!
        /// </summary>
        /// <param name="mapRecord"></param>
        /// <param name="goodIndex"></param>
        /// <param name="sortAscending"></param>
        public StatementLoopOverSortedPairValue(IValue mapRecord, bool sortAscending)
        {
            // Null checks
            if (mapRecord == null)
                throw new ArgumentNullException("mapRecord");

            this._sortAscending = sortAscending;

            // Add this saver
            RestoreOtherSaver(mapRecord);
        }

        /// <summary>
        /// Get back the index variables.
        /// </summary>
        public IEnumerable<IDeclaredParameter> LoopIndexVariable
        {
            get { return _mapRecords.Select(p => p.indexVariable).Cast<IDeclaredParameter>(); }
        }

        /// <summary>
        /// Returns the indexing variable that loops over the items we are looking at.
        /// It returns the index variable associated with the first saver given.
        /// </summary>
        public DeclarableParameter IndexVariable
        {
            get { return _mapRecords.First().indexVariable; }
        }

        private struct mapPlaybackInfo
        {
            /// <summary>
            /// Keep track of what name we will use for listing. Mostly so if CodeItUp is called more than once we get back the same answer!
            /// </summary>
            public string tempListingName;

            /// <summary>
            /// Make sure that the array type is stored.
            /// </summary>
            public Type sortValueTypeArray;

            /// <summary>
            /// The actual map we will play back
            /// </summary>
            public IValue mapRecords;

            /// <summary>
            /// The sequence number for the item (1, 2, 3, etc.) so that we
            /// can keep temp variables isolated.
            /// </summary>
            public int sequence;

            public DeclarableParameter indexVariable;
        }

        /// <summary>
        /// Track the savers we are restoring and indexing through.
        /// </summary>
        private List<mapPlaybackInfo> _mapRecords = new List<mapPlaybackInfo>();


        /// <summary>
        /// Generate the code to sort and play back.
        /// </summary>
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            if (Statements.Any())
            {
                // Use the first map index guy to run the sort - though it really doesn't matter which
                // one we use.
                var first = _mapRecords.First();

                yield return string.Format("{0} {1};", first.sortValueTypeArray.AsCPPType(), first.tempListingName);
                yield return string.Format("for({0}::const_iterator i_itr = {1}.begin(); i_itr != {1}.end(); i_itr++) {{", first.mapRecords.Type.AsCPPType(), first.mapRecords.RawValue);
                yield return string.Format("  {0}.push_back(i_itr->first);", first.tempListingName);
                yield return string.Format("}}");
                yield return string.Format("sort({0}.begin(), {0}.end());", first.tempListingName);
                if (_sortAscending)
                {
                    yield return string.Format("for (int i_index = 0; i_index < {0}.size(); i_index++) {{", first.tempListingName);
                }
                else
                {
                    yield return string.Format("for (int i_index = {0}.size()-1; i_index >= 0; i_index--) {{", first.tempListingName);
                }

                // Next, for each of the arrays we are moving through, we need to generate a temp variable. Create a temp
                // var to make access below simpler.
                foreach (var saver in _mapRecords)
                {
                    var subListType = saver.mapRecords.Type.GetGenericArguments()[1];
                    yield return string.Format("  const {0} &sublist{3}({1}[{2}[i_index]]);", subListType.AsCPPType(), saver.mapRecords.RawValue, saver.tempListingName, saver.sequence);

                }

                //
                // Now, loop over the sublist...
                //

                yield return string.Format("  for (int i_sindex = 0; i_sindex < sublist{0}.size(); i_sindex++) {{", first.sequence);

                //
                // The value of the sequence is used by everyone, so we need to pop it out here.
                //

                foreach (var saver in _mapRecords)
                {
                    yield return string.Format("    const {0} {1} = sublist{2}[i_sindex];", saver.indexVariable.Type.AsCPPType(), saver.indexVariable.RawValue, saver.sequence);
                }

                //
                // Render the code in the inner loop
                //

                foreach (var l in RenderInternalCode())
                {
                    yield return "      " + l;
                }

                //
                // And clean up!
                //

                yield return "  }";
                yield return "}";
            }
        }

        /// <summary>
        /// See if the two statements are compatible
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var other = statement as StatementLoopOverSortedPairValue;
            if (other == null)
                return false;

            // Now inspect that the map records are the same

            var combinedInfo = _mapRecords.Zip(other._mapRecords, (f, s) => Tuple.Create(f, s));

            var candoIt = combinedInfo
                .Select(i => i.Item1.mapRecords.RawValue == i.Item2.mapRecords.RawValue)
                .All(b => b);

            if (!candoIt || _sortAscending != other._sortAscending)
                return false;

            foreach (var item in combinedInfo)
            {
                other.RenameVariable(item.Item2.indexVariable.ParameterName, item.Item1.indexVariable.RawValue);
            }

            return true;
        }

        /// <summary>
        /// Rename the variables in the statement.
        /// </summary>
        /// <param name="origName"></param>
        /// <param name="newName"></param>
        public override void RenameVariable(string origName, string newName)
        {
            foreach (var item in _mapRecords)
            {
                item.indexVariable.RenameRawValue(origName, newName);
                item.mapRecords.RenameRawValue(origName, newName);
            }

            RenameBlockVariables(origName, newName);
        }

        /// <summary>
        /// Add other savers to restore.
        /// </summary>
        /// <param name="mapParameter"></param>
        /// <returns></returns>
        internal DeclarableParameter RestoreOtherSaver(IValue mapParameter)
        {
            // Get the array type that we will use to accumulate items for sorting. This is the "key"
            var arrtype = mapParameter.Type.GetGenericArguments()[0].MakeArrayType();

            // Get an index variable - this is the variable that the contents of the map are set to and are used in
            // the rest of the expression.
            var indexVariableType = mapParameter.Type.GetGenericArguments()[1];
            if (!indexVariableType.IsArray)
                throw new ArgumentException(string.Format("Unable to loop over a map that isn't a map of arrays ({0}).", mapParameter.Type.FullName));
            indexVariableType = indexVariableType.GetElementType();

            // Create the saver which we will index over.
            var saver = new mapPlaybackInfo()
            {
                sequence = _mapRecords.Count(),
                mapRecords = mapParameter,
                sortValueTypeArray = arrtype,
                tempListingName = arrtype.CreateUniqueVariableName(),
                indexVariable = DeclarableParameter.CreateDeclarableParameterExpression(indexVariableType)
            };
            _mapRecords.Add(saver);
            return saver.indexVariable;
        }
    }
}
