
using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
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
        private DeclarableParameter _indexVariable;
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

            this._mapRecord = mapRecord;
            this._sortAscending = sortAscending;

            //
            // We will need an index to go through the sorted items that are
            // stored in the array.
            //

            var indexVariableType = mapRecord.Type.GetGenericArguments()[1];
            if (!indexVariableType.IsArray)
                throw new ArgumentException(string.Format("Unable to loop over a map that isn't a map of arrays ({0}).", mapRecord.Type.FullName));
            indexVariableType = indexVariableType.GetElementType();

            this._indexVariable = DeclarableParameter.CreateDeclarableParameterExpression(indexVariableType);
        }

        /// <summary>
        /// Returns the indexing variable that loops over the items we are looking at.
        /// </summary>
        public DeclarableParameter IndexVariable
        {
            get { return _indexVariable; }
        }

        /// <summary>
        /// Generate the code required.
        /// </summary>
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            if (Statements.Any())
            {
                var sortValueTypeArray = _mapRecord.Type.GetGenericArguments()[0].MakeArrayType();
                var tempListingName = sortValueTypeArray.CreateUniqueVariableName();
                yield return string.Format("{0} {1};", sortValueTypeArray.AsCPPType(), tempListingName);
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

                //
                // To make life simple, create an alias to the list so we can write the code more
                // cleanly below (and some hints to the compiler??).
                //

                var subListType = _mapRecord.Type.GetGenericArguments()[1];
                yield return string.Format("  const {0} &sublist({1}[{2}[i_index]]);", subListType.AsCPPType(), _mapRecord.RawValue, tempListingName);

                //
                // Protect ourselves from break's that occur in the inner loop.
                //

                var breakSeenVar = typeof(bool).CreateUniqueVariableName();
                yield return string.Format("  bool {0}breakSeen = true;", breakSeenVar);
                yield return string.Format("  for (int i_sindex = 0; i_sindex < sublist.size(); i_sindex++) {{", _indexVariable.RawValue);

                //
                // The index variable is what will be used by everyone - so we will just set it here.
                //

                yield return string.Format("    const {0} {1} = sublist[i_sindex];", _indexVariable.Type.AsCPPType(), _indexVariable.RawValue);

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

            bool candoIt = _indexVariable.RawValue == other._indexVariable.RawValue
                && _mapRecord.RawValue == other._mapRecord.RawValue
                && _sortAscending == other._sortAscending;

            if (candoIt)
                Combine(other, opt);

            return candoIt;
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
            RenameBlockVariables(origName, newName);
        }
    }
}
