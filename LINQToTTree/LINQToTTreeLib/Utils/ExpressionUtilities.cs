
using System.ComponentModel.Composition.Hosting;
using LinqToTTreeInterfacesLib;
namespace LINQToTTreeLib.Utils
{
    internal static class ExpressionUtilities
    {
        /// <summary>
        /// Given array info, code a loop over it.
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="arrayRef"></param>
        public static IVariableScopeHolder CodeLoopOverArrayInfo(this IArrayInfo arrayRef, string indexName, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            var indexVar = arrayRef.AddLoop(gc, cc, container);

            ///
            /// Next, make sure the index variable can be used for later references!
            /// 

            var result = cc.Add(indexName, indexVar);
            cc.SetLoopVariable(indexVar);
            return result;
        }

    }
}
