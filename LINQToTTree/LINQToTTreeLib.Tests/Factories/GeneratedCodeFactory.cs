using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib
{
    /// <summary>A factory for LINQToTTreeLib.GeneratedCode instances</summary>
    public static partial class GeneratedCodeFactory
    {
        /// <summary>A factory for LINQToTTreeLib.GeneratedCode instances</summary>
        [PexFactoryMethod(typeof(GeneratedCode))]
        public static GeneratedCode Create(IVariable result, IStatement[] statements, string[] includeFiles, string[] variables, string[] leafnames)
        {
            GeneratedCode generatedCode = new GeneratedCode();

            generatedCode.SetResult(result);

            if (statements != null)
            {
                foreach (var item in statements)
                {
                    generatedCode.Add(item);
                }
            }

            if (includeFiles != null)
                foreach (var item in includeFiles)
                {
                    generatedCode.AddIncludeFile(item);
                }

            if (variables != null)
                foreach (var item in variables)
                {
                    generatedCode.QueueForTransfer(item, new ROOTNET.NTH1F(item, item + "hi", 10, 0.0, 10.0));
                }

            if (leafnames != null)
                foreach (var item in leafnames)
                {
                    generatedCode.AddReferencedLeaf(item);
                }

            return generatedCode;
        }
    }
}
