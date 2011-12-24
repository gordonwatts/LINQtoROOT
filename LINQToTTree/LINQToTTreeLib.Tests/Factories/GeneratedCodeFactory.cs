using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib
{
    /// <summary>A factory for LINQToTTreeLib.GeneratedCode instances</summary>
    public static partial class GeneratedCodeFactory
    {
        /// <summary>A factory for LINQToTTreeLib.GeneratedCode instances</summary>
        [PexFactoryMethod(typeof(GeneratedCode))]
        public static GeneratedCode Create(DeclarableParameter result, IStatement[] statements, string[] includeFiles, ROOTNET.NTObject[] variables, string[] leafnames)
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
                    generatedCode.QueueForTransfer(item);
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
