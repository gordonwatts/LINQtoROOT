using System;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Variables
{
    /// <summary>A factory for LINQToTTreeLib.Variables.VarArray instances</summary>
    public static partial class VarArrayFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Variables.VarArray instances</summary>
        [PexFactoryMethod(typeof(Helpers), "LINQToTTreeLib.Variables.VarArray")]
        public static VarArray Create()
        {
            Type type_type = typeof(int);

            VarArray varArray = new VarArray(type_type);
            return varArray;
        }
    }
}
