using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementPairLoop instances</summary>
    public static partial class StatementPairLoopFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementPairLoop instances</summary>
        [PexFactoryMethod(typeof(Helpers), "LINQToTTreeLib.Statements.StatementPairLoop")]
        public static StatementPairLoop Create(
            IDeclaredParameter arrayRecord_varArray,
            IDeclaredParameter index1_varSimple,
            IDeclaredParameter index2_varSimple1,
            IStatement[] statements,
            IDeclaredParameter[] vars
        )
        {
            StatementPairLoop statementPairLoop = new StatementPairLoop
                                                      (arrayRecord_varArray, index1_varSimple, index2_varSimple1);
            if (statements != null)
                foreach (var s in statements)
                    statementPairLoop.Add(s);

            if (vars != null)
                foreach (var v in vars)
                    statementPairLoop.Add(v);


            return statementPairLoop;
        }
    }
}
