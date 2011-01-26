using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib
{
    /// <summary>A factory for LINQToTTreeLib.GeneratedCode instances</summary>
    public static partial class GeneratedCodeFactory
    {
        /// <summary>A factory for LINQToTTreeLib.GeneratedCode instances</summary>
        [PexFactoryMethod(typeof(GeneratedCode))]
        public static GeneratedCode Create(IVariable result)
        {
            GeneratedCode generatedCode = new GeneratedCode();

            generatedCode.SetResult(result);

            return generatedCode;
        }
    }
}
