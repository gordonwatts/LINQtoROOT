using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementPairLoop instances</summary>
    public static partial class StatementPairLoopFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementPairLoop instances</summary>
        [PexFactoryMethod(typeof(Helpers), "LINQToTTreeLib.Statements.StatementPairLoop")]
        public static StatementPairLoop Create(
            VarArray arrayRecord_varArray,
            VarSimple index1_varSimple,
            VarSimple index2_varSimple1
        )
        {
            StatementPairLoop statementPairLoop = new StatementPairLoop
                                                      (arrayRecord_varArray, index1_varSimple, index2_varSimple1);
            return statementPairLoop;
        }
    }
}
