using LinqToTTreeInterfacesLib;
using System;
using System.Linq;

namespace LINQToTTreeLib.Optimization
{
    /// <summary>
    /// If variables are changing names (as part of optimization or combination) we take care
    /// of that here.
    /// </summary>
    class BlockRenamer : ICodeOptimizationService
    {
        /// <summary>
        /// Track the holder block for old variables.
        /// </summary>
        private IBookingStatementBlock _holderBlockOld;

        /// <summary>
        /// Track the holder block for new variables.
        /// </summary>
        private IBookingStatementBlock _holderBlockNew;

        public BlockRenamer(IBookingStatementBlock holderOldStatements, IBookingStatementBlock holderNewStatements)
        {
            if (holderOldStatements == null)
                throw new ArgumentNullException("holder");
            this._holderBlockOld = holderOldStatements;
            if (holderNewStatements == null)
                throw new ArgumentNullException("holder");
            this._holderBlockNew = holderNewStatements;
        }

        /// <summary>
        /// Rename succeeds if we can find the declared variable, among other things.
        /// </summary>
        /// <param name="oldName">Name of the old parameter that we are replacing</param>
        /// <param name="newParam">The new parameter we will replace it with</param>
        /// <param name="newHolderBlock">The booking context we are currently looking at for the new name (the _holder) of the statement we are looking at</param>
        /// <returns>True if the variables could be renamed (and the rename is done), false otherwise</returns>
        /// <remarks>
        /// The newHolderBlock is needed because it is used to determine if the new variable is declared in the same place
        /// or not.
        /// </remarks>
        public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newParam)
        {
            //
            // First, see if we can find the block where the variable is declared.
            //

            var vr = FindDeclaredVariable(oldName, _holderBlockOld);

            if (vr == null)
                return false;

            //
            // Make sure that the variable we are switching to is also declared. If it is an "external" then we
            // are going to have a problem here! And, the variables had better be declared the same "scope" above, or
            // that means they are also being used for something different.
            //

            var vrNew = FindDeclaredVariable(newParam.ParameterName, _holderBlockNew);
            if (vrNew == null || vrNew.Item3 != vr.Item3)
                return false;

            // Check that its initialization is the same!
            bool initValueSame = (vr.Item1.InitialValue == null && newParam.InitialValue == null)
                || (vr.Item1.InitialValue != null && (vr.Item1.InitialValue.Type == newParam.InitialValue.Type && vr.Item1.InitialValue.RawValue == newParam.InitialValue.RawValue));
            if (!initValueSame)
                return false;

            // Rename the variable!
            vr.Item2.RenameVariable(oldName, newParam.ParameterName);

            return true;
        }

        /// <summary>
        /// Walk the tree back looking for a variable
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="statement"></param>
        /// <returns>A tuple of the declared old variable, the block it was booked in, and how far up the chain we had to go to find it.</returns>
        private Tuple<IDeclaredParameter, IBookingStatementBlock, int> FindDeclaredVariable(string oldName, IStatement statement)
        {
            if (statement == null)
                return null;

            if (statement is IBookingStatementBlock)
            {
                var hr = statement as IBookingStatementBlock;
                var vr = hr.DeclaredVariables.Where(v => v.ParameterName == oldName).FirstOrDefault();
                if (vr != null)
                    return Tuple.Create(vr, hr, 0);
            }

            var onedown = FindDeclaredVariable(oldName, statement.Parent);
            if (onedown == null)
                return null;
            return Tuple.Create(onedown.Item1, onedown.Item2, onedown.Item3 + 1);
        }

        /// <summary>
        /// Do the rename in this block and deeper.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void ForceRenameVariable(string originalName, string newName)
        {
            _holderBlockOld.RenameVariable(originalName, newName);
        }

        /// <summary>
        /// Remove a variable declaration totally. Do it without checking that it is being
        /// used (or not).
        /// </summary>
        /// <param name="item2"></param>
        internal void ForceRemoveDeclaration(string v, IStatement s)
        {
            var p = FindDeclaredVariable(v, s);
            if (p != null)
            {
                p.Item2.Remove(p.Item1);
            }
        }
    }
}
