using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework.Using;

[assembly: PexUseType(typeof(LINQToTTreeLib.Tests.Factories.CodeOptimizerTest))]

namespace LINQToTTreeLib.Tests.Factories
{
    /// <summary>
    /// A simple code optimization service for use in testing.
    /// </summary>
    public class CodeOptimizerTest : ICodeOptimizationService
    {
        public bool ResultValue { get; private set; }
        public CodeOptimizerTest(bool result)
        {
            ResultValue = result;
        }

        public string OldName { get; private set; }

        public IVariable NewVariable { get; private set; }

        public bool TryRenameVarialbeOneLevelUp(string oldName, IVariable newVariable)
        {
            OldName = oldName;
            NewVariable = newVariable;
            return ResultValue;
        }
    }
}
