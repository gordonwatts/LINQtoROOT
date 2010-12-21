using System;
using Microsoft.Pex.Framework;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementIncrementInteger instances</summary>
    public static partial class StatementIncrementIntegerFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementIncrementInteger instances</summary>
        [PexFactoryMethod(typeof(StatementIncrementInteger))]
        public static StatementIncrementInteger Create(VarInteger i_varInteger)
        {
            StatementIncrementInteger statementIncrementInteger
               = new StatementIncrementInteger(i_varInteger);
            return statementIncrementInteger;
        }
    }
}
