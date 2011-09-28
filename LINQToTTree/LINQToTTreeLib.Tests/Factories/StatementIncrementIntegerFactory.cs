using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementIncrementInteger instances</summary>
    public static partial class StatementIncrementIntegerFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementIncrementInteger instances</summary>
        [PexFactoryMethod(typeof(StatementIncrementInteger))]
        public static StatementIncrementInteger Create(IDeclaredParameter i_varInteger)
        {
            StatementIncrementInteger statementIncrementInteger
               = new StatementIncrementInteger(i_varInteger);
            return statementIncrementInteger;
        }
    }
}
