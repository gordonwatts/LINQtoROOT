using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementLoopOverGood instances</summary>
    public static partial class StatementLoopOverGoodFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementLoopOverGood instances</summary>
        [PexFactoryMethod(typeof(Helpers), "LINQToTTreeLib.Statements.StatementLoopOverGood")]
        public static StatementLoopOverGood Create(IValue indiciesToCheck_iValue,
            IValue indexIsGood_iValue1,
            IValue index_iValue2,
            IStatement[] statements,
            IVariable[] vars)
        {
            StatementLoopOverGood statementLoopOverGood = new StatementLoopOverGood
                                                              (indiciesToCheck_iValue, indexIsGood_iValue1, index_iValue2);

            if (statements != null)
                foreach (var s in statements)
                    statementLoopOverGood.Add(s);

            if (vars != null)
                foreach (var v in vars)
                    statementLoopOverGood.Add(v);
            return statementLoopOverGood;

            // TODO: Edit factory method of StatementLoopOverGood
            // This method should be able to configure the object in all possible ways.
            // Add as many parameters as needed,
            // and assign their values to each field by using the API.
        }
    }
}
