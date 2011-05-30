using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Variables
{
    /// <summary>A factory for LINQToTTreeLib.Variables.VarInteger instances</summary>
    public static partial class VarIntegerFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Variables.VarInteger instances</summary>
        [PexFactoryMethod(typeof(VarInteger))]
        public static VarInteger Create(bool declare, int value)
        {
            VarInteger varInteger = new VarInteger();
            varInteger.Declare = declare;
            varInteger.InitialValue = new ValSimple(value.ToString(), typeof(int));
            return varInteger;
        }
    }
}
