using LINQToTTreeLib.Expressions;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementLoopOverSortedPairValue instances</summary>
    public static partial class StatementLoopOverSortedPairValueFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementLoopOverSortedPairValue instances</summary>
        [PexFactoryMethod(typeof(StatementLoopOverSortedPairValue))]
        public static StatementLoopOverSortedPairValue Create(
            bool sortAscending_b
        )
        {
            //
            // The value type is actually a map - so an IGrouping usually, and the second guy is an array over
            // something.
            //

            var keyType = typeof(int);
            var indexType = typeof(int);
            var iv = DeclarableParameter.CreateDeclarableParameterMapExpression(keyType, indexType.MakeArrayType());

            StatementLoopOverSortedPairValue statementLoopOverSortedPairValue
               = new StatementLoopOverSortedPairValue
                     (iv, sortAscending_b);
            return statementLoopOverSortedPairValue;
        }
    }
}
