using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Statement that loops over the groups found by a previous GroupBy predicate.
    /// </summary>
    public class StatementLoopOverGroups : StatementInlineBlockBase, IStatementLoop
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
            if (mapOfGroups == null)
                throw new ArgumentNullException("mapOfGroups");

            this._mapOfGroups = mapOfGroups;
            var iteratorType = typeof(IEnumerable<int>).GetGenericTypeDefinition().MakeGenericType(new Type[] { mapOfGroups.Type });
            this._groupIndex = DeclarableParameter.CreateDeclarableParameterExpression(iteratorType);
        }

        /// <summary>
        /// Return the code for this statement.
        /// </summary>
        /// <returns></returns>
        public override System.Collections.Generic.IEnumerable<string> CodeItUp()
        {
            if (Statements.Any())
            {
                //
                // First, convert the keys into an array. We do this because everything is based
                // on index variables.
                //

                yield return string.Format("for ({0} {1} = {2}.begin(); {1} != {2}.end(); {1}++)", _groupIndex.Type.AsCPPType(), _groupIndex.RawValue, _mapOfGroups.RawValue);
                foreach (var l in RenderInternalCode())
                {
                    yield return l;
                }
            }
        }

        /// <summary>
        /// If we are the same, then combine!
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var other = statement as StatementLoopOverGroups;
            if (other == null)
                return false;

            if (_mapOfGroups.RawValue != other._mapOfGroups.RawValue)
                return false;

            // We declare group index by hand, so we have to do the renaming by hand here too.
            other.RenameVariable(other._groupIndex.RawValue, _groupIndex.RawValue);

            Combine(other, opt);
            return true;
        }

        /// <summary>
        /// Rename all variables in this statement.
        /// </summary>
        /// <param name="origName"></param>
        /// <param name="newName"></param>
        public override void RenameVariable(string origName, string newName)
        {
            _mapOfGroups.RenameRawValue(origName, newName);
            _groupIndex.RenameParameter(origName, newName);
            _groupIndex.RenameRawValue(origName, newName);
            RenameBlockVariables(origName, newName);
        }

        /// <summary>
        /// Return the group key reference - so this is the key that is currently being processed.
        /// </summary>
        public IValue GroupKeyReference
        {
            get
            {
                return new ValSimple(string.Format("{0}->first", _groupIndex.RawValue), _mapOfGroups.Type.GetGenericArguments()[0]);
            }
        }

        /// <summary>
        /// Returns the IEnumerable array type for the group items.
        /// </summary>
        public IValue GroupItemsReference
        {
            get
            {
                return new ValSimple(string.Format("{0}->second", _groupIndex.RawValue), _mapOfGroups.Type.GetGenericArguments()[1].MakeArrayType());
            }
        }

        /// <summary>
        /// Get the index that we are currently using for looping
        /// </summary>
        public DeclarableParameter IndexVariable { get { return _groupIndex; } }
    }
}
