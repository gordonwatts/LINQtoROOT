using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Get back the index variables.
        /// </summary>
        public IEnumerable<IDeclaredParameter> LoopIndexVariable
        {
            get { return new IDeclaredParameter[] { _groupIndex }; }
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
        /// Return all declared variables in this guy
        /// </summary>
        public override IEnumerable<IDeclaredParameter> DeclaredVariables
        {
            get
            {
                return base.DeclaredVariables
                    .Concat(new IDeclaredParameter[] { _groupIndex });
            }
        }

        /// <summary>
        /// Can we move a statement past the for loop?
        /// </summary>
        /// <param name="followStatement"></param>
        /// <returns></returns>
        public override bool CommutesWithGatingExpressions(ICMStatementInfo followStatement)
        {
            return !followStatement.ResultVariables.Intersect(_mapOfGroups.Dependants.Select(p => p.RawValue)).Any();
        }

        /// <summary>
        /// Return the group key reference - so this is the key that is currently being processed.
        /// </summary>
        public IValue GroupKeyReference
        {
            get
            {
                return new ValSimple($"{_groupIndex.RawValue}->first", _mapOfGroups.Type.GetGenericArguments()[0], _groupIndex.AsArray());
            }
        }

        /// <summary>
        /// Returns the IEnumerable array type for the group items.
        /// </summary>
        public IValue GroupItemsReference
        {
            get
            {
                return new ValSimple($"{_groupIndex.RawValue}->second", _mapOfGroups.Type.GetGenericArguments()[1].MakeArrayType(), _groupIndex.AsArray());
            }
        }

        /// <summary>
        /// Get the index that we are currently using for looping
        /// </summary>
        public DeclarableParameter IndexVariable { get { return _groupIndex; } }

        /// <summary>
        /// We are a straight up loop, so we want every statement out of our interior that
        /// we can get out!
        /// </summary>
        public override bool AllowNormalBubbleUp { get { return true; } }
    }
}
