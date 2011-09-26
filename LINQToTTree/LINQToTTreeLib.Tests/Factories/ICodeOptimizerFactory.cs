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

        public int TimesCalled = 0;

        public CodeOptimizerTest(bool result)
        {
            ResultValue = result;
        }

        public string OldName { get; private set; }

        public IDeclaredParameter NewVariable { get; private set; }

        public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
        {
            TimesCalled++;
            OldName = oldName;
            NewVariable = newVariable;
            return ResultValue;
        }


        public void ForceRenameVariable(string originalName, string newName)
        {
            TimesCalled++;
            OldName = originalName;
        }
    }
}
