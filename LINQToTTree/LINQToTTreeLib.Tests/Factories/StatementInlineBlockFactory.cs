using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementInlineBlock instances</summary>
    public static partial class StatementInlineBlockFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementInlineBlock instances</summary>
        [PexFactoryMethod(typeof(StatementInlineBlock))]
        public static StatementInlineBlock Create(IStatement[] statements, IVariable[] vars)
        {
            StatementInlineBlock statementInlineBlock = new StatementInlineBlock();

            if (statements != null)
                foreach (var s in statements)
                {
                    statementInlineBlock.Add(s);
                }

            if (vars != null)
                foreach (var v in statements)
                    statementInlineBlock.Add(v);

            return statementInlineBlock;
        }
    }
}
