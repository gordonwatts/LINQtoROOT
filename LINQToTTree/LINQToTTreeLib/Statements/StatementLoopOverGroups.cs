using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Statements
{
    class StatementLoopOverGroups : StatementInlineBlockBase, IStatementLoop
    {
        private IValue _mapOfGroups;
        private IValue _groupIndex;

        string _iterator;

        /// <summary>
        /// Do a loop over a list of declareable groups.
        /// </summary>
        /// <param name="mapOfGroups"></param>
        /// <param name="groupIndex"></param>
        public StatementLoopOverGroups(IValue mapOfGroups, IValue groupIndex)
        {
            // TODO: Complete member initialization
            this._mapOfGroups = mapOfGroups;
            this._groupIndex = groupIndex;
            _iterator = string.Format("itr_{0}", _mapOfGroups.Type.CreateUniqueVariableName());
        }

        public override System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            if (Statements.Any())
            {
                //
                // First, convert the keys into an array. We do this because everything is based
                // on index variables.
                //

                yield return string.Format("for ({0}::const_iterator {1} = {2}.begin(); {1} != {2}.end(); {1}++)", _mapOfGroups.Type.AsCPPType(), _iterator, _mapOfGroups.RawValue);
                foreach (var l in RenderInternalCode())
                {
                    yield return l;
                }
            }
        }

        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            throw new System.NotImplementedException();
        }

        public override void RenameVariable(string origName, string newName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Return the group key reference - so this is the key that is currently being processed.
        /// </summary>
        public IValue GroupKeyReference
        {
            get
            {
                return new ValSimple(string.Format("{0}->first", _iterator), _mapOfGroups.Type.GetGenericArguments()[0]);
            }
        }

        /// <summary>
        /// Returns the IEnumerable array type for the group items.
        /// </summary>
        public IValue GroupItemsReference
        {
            get
            {
                return new ValSimple(string.Format("{0}->second", _iterator), _mapOfGroups.Type.GetGenericArguments()[1].MakeArrayType());
            }
        }
    }
}
