using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.TypeHandlers.ReplacementMethodCalls
{
    /// <summary>A factory for LINQToTTreeLib.TypeHandlers.ReplacementMethodCalls.TypeHandlerReplacementCall instances</summary>
    public static partial class TypeHandlerReplacementCallFactory
    {
        /// <summary>A factory for LINQToTTreeLib.TypeHandlers.ReplacementMethodCalls.TypeHandlerReplacementCall instances</summary>
        [PexFactoryMethod(typeof(TypeHandlerReplacementCall))]
        public static TypeHandlerReplacementCall Create()
        {
            TypeHandlerReplacementCall typeHandlerReplacementCall
               = new TypeHandlerReplacementCall();
            return typeHandlerReplacementCall;
        }
    }
}
