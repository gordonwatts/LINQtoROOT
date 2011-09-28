using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementCheckLoopPairwise instances</summary>
    public static partial class StatementCheckLoopPairwiseFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementCheckLoopPairwise instances</summary>
        [PexFactoryMethod(typeof(StatementCheckLoopPairwise))]
        public static StatementCheckLoopPairwise Create(
            IDeclaredParameter indiciesToInspect_varArray,
            IDeclaredParameter index1_varSimple,
            IDeclaredParameter index2_varSimple1,
            IDeclaredParameter passedArray_varArray1,
            IStatement value_iStatement
        )
        {
            StatementCheckLoopPairwise statementCheckLoopPairwise
               = new StatementCheckLoopPairwise
                     (indiciesToInspect_varArray, index1_varSimple,
                      index2_varSimple1, passedArray_varArray1);
            ((StatementInlineBlockBase)statementCheckLoopPairwise).Parent = value_iStatement;
            return statementCheckLoopPairwise;
        }
    }
}
