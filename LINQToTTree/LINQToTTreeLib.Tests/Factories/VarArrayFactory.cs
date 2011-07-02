using System;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Variables
{
    /// <summary>A factory for LINQToTTreeLib.Variables.VarArray instances</summary>
    public static partial class VarArrayFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Variables.VarArray instances</summary>
        [PexFactoryMethod(typeof(Helpers), "LINQToTTreeLib.Variables.VarArray")]
        public static VarArray Create(int index)
        {
            Type type_type = null;
            switch (index)
            {
                case 0:
                    type_type = typeof(int);
                    break;

                case 1:
                    type_type = typeof(bool);
                    break;

                case 2:
                    type_type = typeof(float);
                    break;

                default:
                    break;
            }

            VarArray varArray = new VarArray(type_type);
            return varArray;
        }
    }
}
