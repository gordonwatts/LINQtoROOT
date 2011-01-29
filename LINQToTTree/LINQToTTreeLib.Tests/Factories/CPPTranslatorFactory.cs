using Microsoft.Pex.Framework;

namespace LINQToTTreeLib
{
    /// <summary>A factory for LINQToTTreeLib.CPPTranslator instances</summary>
    public static partial class CPPTranslatorFactory
    {
        /// <summary>A factory for LINQToTTreeLib.CPPTranslator instances</summary>
        [PexFactoryMethod(typeof(CPPTranslator))]
        public static CPPTranslator Create()
        {
            CPPTranslator cPPTranslator = new CPPTranslator();
            return cPPTranslator;
        }
    }
}
