using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementLoopOverGroupItems instances</summary>
    public static partial class StatementLoopOverGroupItemsFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementLoopOverGroupItems instances</summary>
        [PexFactoryMethod(typeof(StatementLoopOverGroupItems))]
        public static StatementLoopOverGroupItems Create(
            IValue arrayToLoopOver_iValue,
            IStatement value_iStatement
        )
        {
            StatementLoopOverGroupItems statementLoopOverGroupItems
               = new StatementLoopOverGroupItems(arrayToLoopOver_iValue);
            ((StatementInlineBlockBase)statementLoopOverGroupItems).Parent =
              value_iStatement;
            return statementLoopOverGroupItems;
        }
    }
}
