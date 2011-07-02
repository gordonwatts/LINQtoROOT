using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementCheckPairwise instances</summary>
    public static partial class StatementCheckPairwiseFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementCheckPairwise instances</summary>
        [PexFactoryMethod(typeof(Helpers), "LINQToTTreeLib.Statements.StatementCheckPairwise")]
        public static StatementCheckPairwise Create(
            VarArray indiciesToInspect_varArray,
            VarSimple index1_varSimple,
            VarSimple index2_varSimple1,
            VarArray passedArray_varArray1,
            IValue test_iValue
        )
        {
            StatementCheckPairwise statementCheckPairwise
               = new StatementCheckPairwise(indiciesToInspect_varArray, index1_varSimple,
                                            index2_varSimple1, passedArray_varArray1, test_iValue);

            return statementCheckPairwise;
        }
    }
}
