using System;
using Microsoft.Pex.Framework;
using LINQToTTreeLib;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib
{
    /// <summary>A factory for LINQToTTreeLib.GeneratedCode instances</summary>
    public static partial class GeneratedCodeFactory
    {
        /// <summary>A factory for LINQToTTreeLib.GeneratedCode instances</summary>
        [PexFactoryMethod(typeof(GeneratedCode))]
        public static GeneratedCode Create()
        {
            GeneratedCode generatedCode = new GeneratedCode();
            return generatedCode;
        }
    }
}
