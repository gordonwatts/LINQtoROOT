using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementFlipBool instances</summary>
    public static partial class StatementFlipBoolFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementFlipBool instances</summary>
        [PexFactoryMethod(typeof(Helpers), "LINQToTTreeLib.Statements.StatementFlipBool")]
        public static StatementFlipBool Create(VarSimple aresult_varSimple)
        {
            StatementFlipBool statementFlipBool = new StatementFlipBool(aresult_varSimple);
            return statementFlipBool;
        }
    }
}
