using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementRecordPairValues instances</summary>
    public static partial class StatementRecordPairValuesFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementRecordPairValues instances</summary>
        [PexFactoryMethod(typeof(StatementRecordPairValues))]
        public static StatementRecordPairValues Create(
            DeclarableParameter mapStorage_iDeclaredParameter,
            IValue indexVar_iValue,
            IValue indexValue_iValue1,
            IStatement value_iStatement
        )
        {
            StatementRecordPairValues statementRecordPairValues
               = new StatementRecordPairValues
                     (mapStorage_iDeclaredParameter, indexVar_iValue, indexValue_iValue1);
            statementRecordPairValues.Parent = value_iStatement;
            return statementRecordPairValues;
        }
    }
}
