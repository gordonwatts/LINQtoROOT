using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Some helpful routines that can be used during code combinations
    /// </summary>
    public interface ICodeOptimizationService
    {
        /// <summary>
        /// Rename all variables in the block one level up from old to new.
        /// It is possible to fail - in which case false should be returned.
        /// </summary>
        /// <param name="oldName">The old variable which we shoudl find in the enclosing block and rename</param>
        /// <param name="newVariable">The new variable we want it renamed to</param>
        /// <returns></returns>
        bool TryRenameVarialbeOneLevelUp(string oldName, IVariable newVariable);
    }
}
