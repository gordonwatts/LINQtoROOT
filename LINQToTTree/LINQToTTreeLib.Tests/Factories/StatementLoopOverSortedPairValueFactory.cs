using System;
using Microsoft.Pex.Framework;
using LINQToTTreeLib.Statements;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementLoopOverSortedPairValue instances</summary>
    public static partial class StatementLoopOverSortedPairValueFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementLoopOverSortedPairValue instances</summary>
        [PexFactoryMethod(typeof(StatementLoopOverSortedPairValue))]
        public static StatementLoopOverSortedPairValue Create(
            IValue mapRecord_iValue,
            IValue goodIndex_iValue1,
            bool sortAscending_b,
            IStatement value_iStatement
        )
        {
            StatementLoopOverSortedPairValue statementLoopOverSortedPairValue
               = new StatementLoopOverSortedPairValue
                     (mapRecord_iValue, goodIndex_iValue1, sortAscending_b);
            ((StatementInlineBlockBase)statementLoopOverSortedPairValue).Parent =
              value_iStatement;
            return statementLoopOverSortedPairValue;

            // TODO: Edit factory method of StatementLoopOverSortedPairValue
            // This method should be able to configure the object in all possible ways.
            // Add as many parameters as needed,
            // and assign their values to each field by using the API.
        }
    }
}
