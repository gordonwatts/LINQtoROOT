using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Tests.Factories
{
    /// <summary>A factory for LINQToTTreeLib.Tests.Factories.CodeOptimizerTest instances</summary>
    public static partial class CodeOptimizerTestFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Tests.Factories.CodeOptimizerTest instances</summary>
        [PexFactoryMethod(typeof(LINQToTTreeLib.Tests.Factories.CodeOptimizerTest), "LINQToTTreeLib.Tests.Factories.CodeOptimizerTest")]
        public static LINQToTTreeLib.Tests.Factories.CodeOptimizerTest Create(bool result)
        {
            return new CodeOptimizerTest(result);
        }
    }
}
