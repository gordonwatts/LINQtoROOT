using System;
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Variables
{
    /// <summary>A factory for LINQToTTreeLib.Variables.VarArray instances</summary>
    public static partial class VarArrayFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Variables.VarArray instances</summary>
        [PexFactoryMethod(typeof(VarArray))]
        public static VarArray Create(Type type_type, IValue initValue, bool declare)
        {
            VarArray varArray = new VarArray(type_type);
            varArray.InitialValue = initValue;
            varArray.Declare = declare;
            return varArray;
        }
    }
}
