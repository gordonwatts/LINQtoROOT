
using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
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
            if (sortAscending == null)
                throw new ArgumentNullException("sortAscending");

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
                yield return string.Format("vector<int> tempListing;");
                yield return string.Format("for({0}::const_iterator i_itr = {1}.begin(); i_itr != {1}.end(); i_itr++) {{", _mapRecord.Type.AsCPPType(), _mapRecord.RawValue);
                yield return string.Format("  tempListing.push_back(i_itr->first);");
                yield return string.Format("}}");
                yield return string.Format("sort(tempListing.begin(), tempListing.end());");
                if (_sortAscending)
                {
                    yield return string.Format("for (int i_index = 0; i_index < tempListing.size(); i_index++) {{");
                }
                else
                {
                    yield return string.Format("for (int i_index = tempListing.size()-1; i_index >= 0; i_index--) {{");
                }
                yield return string.Format("  const vector<int> &sublist({0}[tempListing[i_index]];", _mapRecord.RawValue);
                yield return string.Format("  for (int {0} = 0; {0} < sublist.size(); {0}++) {{", _indexVariable.RawValue);

                foreach (var l in RenderInternalCode())
                {
                    yield return "    " + l;
                }

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
