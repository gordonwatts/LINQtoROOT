using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Tests.Statements
{
    /// <summary>
    /// An optimization service that will track all requests made to it.
    /// </summary>
    class DummyTrackingOptimizationService : ICodeOptimizationService
    {
        public DummyTrackingOptimizationService(bool allowRename = true)
        {
            _allowRename = allowRename;
        }

        public List<Tuple<string, string>> _renameRequests = new List<Tuple<string, string>>();
        private bool _allowRename;

        public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
        {
            _renameRequests.Add(Tuple.Create(oldName, newVariable.RawValue));
            return _allowRename;
        }

        public void ForceRenameVariable(string originalName, string newName)
        {
            throw new NotImplementedException();
        }
    }
}
