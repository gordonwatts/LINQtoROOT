﻿using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
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
            // Dummy check.
            if (oldName == newParam.ParameterName)
                return true;

            // First, see if we can find the block where the variable is declared.

            var vr = FindDeclaredVariable(oldName, _holderBlockOld);

            if (vr == null)
                return false;

            // Make sure that the variable we are switching to is also declared. If it is an "external" then we
            // are going to have a problem here! And, the variables had better be declared the same "scope" above, or
            // that means they are also being used for something different.

            var vrNew = FindDeclaredVariable(newParam.ParameterName, _holderBlockNew);
            if (vrNew == null || vrNew.Item3 != vr.Item3)
                return false;

            // Check that its initialization is the same!
            bool initValueSame = (vr.Item1.InitialValue == null && newParam.InitialValue == null)
                || (vr.Item1.InitialValue != null && (vr.Item1.InitialValue.Type == newParam.InitialValue.Type && vr.Item1.InitialValue.RawValue == newParam.InitialValue.RawValue));
            if (!initValueSame)
                return false;

            // So, then the next question is - is the variable used in the same way below? Tracking this carefully
            // requires a real data flow. So we are going to do this simply - if the variables are used downstream
            // for any reason and are altered or changed/updated - then we won't combine them.
            var newModified = vrNew.Item2
                .Statements
                .SelectMany(s => s is IStatementCompound ? (s as IStatementCompound).Statements : new IStatement[] { s })
                .Where(s => s is ICMStatementInfo)
                .Where(s => (s as ICMStatementInfo).ResultVariables.Where(v => v == newParam.ParameterName).Any())
                .ToArray();

            var oldModified = vr.Item2
                .Statements
                .SelectMany(s => s is IStatementCompound ? (s as IStatementCompound).Statements : new IStatement[] { s })
                .Where(s => s is ICMStatementInfo)
                .Where(s => (s as ICMStatementInfo).ResultVariables.Where(v => v == oldName).Any())
                .ToArray();

            if (newModified.Count() != oldModified.Count())
                return false;

            var pairedStatements = oldModified.Zip(newModified, (oldS, newS) => Tuple.Create(oldS, newS)).ToArray();
            if (pairedStatements.Where(sp => sp.Item1.GetType() != sp.Item2.GetType()).Any())
                return false;

            // Next, we have to make sure that the statements really are the same. Do this by runnign the renamer on them.
            var renameStatus = pairedStatements
                .Select(spair => ((spair.Item1 as ICMStatementInfo), (spair.Item2 as ICMStatementInfo)))
                .Select(spair => spair.Item1 == null || spair.Item2 == null
                                ? false
                                : spair.Item1.RequiredForEquivalence(spair.Item2, new[] { Tuple.Create(oldName, newParam.ParameterName) }).Item1);
            if (renameStatus.Any(s => !s))
            {
                return false;
            }

            // Rename the variable - we need to do this to do the next level of checks.
            vr.Item2.RenameVariable(oldName, newParam.ParameterName);

            // Finally, combine the statements so we can get rid of them as they are now duplicates.
            foreach (var spair in pairedStatements)
            {
                // Due to checking, there is no need to look at the result of teh try combine.
                spair.Item2.TryCombineStatement(spair.Item1, this);
            }
            return true;
        }

        /// <summary>
        /// Simple dummy optimization service.
        /// </summary>
        private class SimpleOptimizer : ICodeOptimizationService
        {
            public void ForceRenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }

            public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
            {
                return false;
            }
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
        /// <param name="v">Variable name</param>
        /// <param name="s">Statement to start looking for the decl in</param>
        /// <remarks>The statement where the decl was found</remarks>
        internal IBookingStatementBlock ForceRemoveDeclaration(string v, IStatement s)
        {
            var p = FindDeclaredVariable(v, s);
            if (p != null)
            {
                p.Item2.Remove(p.Item1);
            }
            return p?.Item2;
        }
    }
}
