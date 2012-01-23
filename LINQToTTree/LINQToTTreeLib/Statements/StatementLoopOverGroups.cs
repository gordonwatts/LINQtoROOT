using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Statements
{
    class StatementLoopOverGroups : StatementInlineBlockBase, IStatementLoop
    {
        private IValue _mapOfGroups;
        private DeclarableParameter _groupIndex;

        /// <summary>
        /// Do a loop over a list of declareable groups.
        /// </summary>
        /// <param name="mapOfGroups"></param>
        /// <param name="groupIndex"></param>
        public StatementLoopOverGroups(IValue mapOfGroups)
        {
            // TODO: Complete member initialization
            this._mapOfGroups = mapOfGroups;
            this._groupIndex = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
        }

        public override System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            if (Statements.Any())
            {
                //
                // First, convert the keys into an array. We do this because everything is based
                // on index variables.
                //

                var tmpSizeName = typeof(int).CreateUniqueVariableName();
                yield return string.Format("int {0} = {1}.size();", tmpSizeName, _mapOfGroups.RawValue);
                yield return string.Format("for (int {0} = 0; {0} < {1}; {0}++)", _groupIndex.RawValue, tmpSizeName);
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
                return new ValSimple(string.Format("({0}.begin() + {1})->first", _mapOfGroups.RawValue, _groupIndex.RawValue), _mapOfGroups.Type.GetGenericArguments()[0]);
            }
        }

        /// <summary>
        /// Returns the IEnumerable array type for the group items.
        /// </summary>
        public IValue GroupItemsReference
        {
            get
            {
                return new ValSimple(string.Format("({0}.begin() + {1})->second", _mapOfGroups.RawValue, _groupIndex.RawValue), _mapOfGroups.Type.GetGenericArguments()[1].MakeArrayType());
            }
        }

        /// <summary>
        /// Get the index that we are currently using for looping
        /// </summary>
        public DeclarableParameter IndexVariable { get { return _groupIndex; } }
    }
}
