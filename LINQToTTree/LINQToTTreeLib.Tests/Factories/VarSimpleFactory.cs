using System;
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Variables
{
    /// <summary>A factory for LINQToTTreeLib.Variables.VarSimple instances</summary>
    public static partial class VarSimpleFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Variables.VarSimple instances</summary>
        [PexFactoryMethod(typeof(VarSimple))]
        public static VarSimple Create(Type type_type, IValue initialValue, bool declared)
        {
            VarSimple varSimple = new VarSimple(type_type);
            varSimple.InitialValue = initialValue;
            varSimple.Declare = declared;
            return varSimple;
        }
    }
}
