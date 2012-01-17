
using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// A satement that will sort over the arguments to the map variable, and then loop
    /// over them - using as an index the map target.
    /// </summary>
    public class StatementLoopOverSortedPairValue : StatementInlineBlockBase, IStatementLoop
    {
        private IValue _mapRecord;
        private IValue _indexVariable;
        private bool _sortAscending;

        /// <summary>
        /// Save everything for later work!
        /// </summary>
        /// <param name="mapRecord"></param>
        /// <param name="goodIndex"></param>
        /// <param name="sortAscending"></param>
        public StatementLoopOverSortedPairValue(IValue mapRecord, IValue goodIndex, bool sortAscending)
        {
            // Null checks

            if (mapRecord == null)
                throw new ArgumentNullException("mapRecord");
            if (goodIndex == null)
                throw new ArgumentNullException("goodIndex");

            this._mapRecord = mapRecord;
            this._indexVariable = goodIndex;
            this._sortAscending = sortAscending;
        }

        /// <summary>
        /// Generate the code required.
        /// </summary>
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            if (Statements.Any())
            {
                var tempListingName = typeof(int[]).CreateUniqueVariableName();
                yield return string.Format("vector<int> {0};", tempListingName);
                yield return string.Format("for({0}::const_iterator i_itr = {1}.begin(); i_itr != {1}.end(); i_itr++) {{", _mapRecord.Type.AsCPPType(), _mapRecord.RawValue);
                yield return string.Format("  {0}.push_back(i_itr->first);", tempListingName);
                yield return string.Format("}}");
                yield return string.Format("sort({0}.begin(), {0}.end());", tempListingName);
                if (_sortAscending)
                {
                    yield return string.Format("for (int i_index = 0; i_index < {0}.size(); i_index++) {{", tempListingName);
                }
                else
                {
                    yield return string.Format("for (int i_index = {0}.size()-1; i_index >= 0; i_index--) {{", tempListingName);
                }
                yield return string.Format("  const vector<int> &sublist({0}[{1}[i_index]]);", _mapRecord.RawValue, tempListingName);
                var breakSeenVar = typeof(bool).CreateUniqueVariableName();
                yield return string.Format("  bool {0}breakSeen = true;", breakSeenVar);
                yield return string.Format("  for (int i_sindex = 0; i_sindex < sublist.size(); i_sindex++) {{", _indexVariable.RawValue);
                yield return string.Format("    {0} = sublist[i_sindex];", _indexVariable.RawValue);

                foreach (var l in RenderInternalCode())
                {
                    yield return "    " + l;
                }

                yield return string.Format("    {0}breakSeen = false;", breakSeenVar);

                yield return "  }";
                yield return string.Format("  if ({0}breakSeen) break;", breakSeenVar);
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

            return _indexVariable.RawValue == other._indexVariable.RawValue
                && _mapRecord.RawValue == other._mapRecord.RawValue
                && _sortAscending == other._sortAscending;
        }

        /// <summary>
        /// Rename the variables in the statement.
        /// </summary>
        /// <param name="origName"></param>
        /// <param name="newName"></param>
        public override void RenameVariable(string origName, string newName)
        {
            _indexVariable.RenameRawValue(origName, newName);
            _mapRecord.RenameRawValue(origName, newName);
        }
    }
}
