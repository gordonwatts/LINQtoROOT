using System;
using Microsoft.Pex.Framework;
using LINQToTTreeLib.Statements;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementInlineBlock instances</summary>
    public static partial class StatementInlineBlockFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementInlineBlock instances</summary>
        [PexFactoryMethod(typeof(StatementInlineBlock))]
        public static StatementInlineBlock Create()
        {
            StatementInlineBlock statementInlineBlock = new StatementInlineBlock();
            return statementInlineBlock;
        }
    }
}
