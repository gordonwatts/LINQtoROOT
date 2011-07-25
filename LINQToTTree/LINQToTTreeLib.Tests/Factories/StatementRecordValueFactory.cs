using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementRecordValue instances</summary>
    public static partial class StatementRecordValueFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementRecordValue instances</summary>
        [PexFactoryMethod(typeof(StatementRecordValue))]
        public static StatementRecordValue Create(
            IVariable indexSaveLocation_varSimple,
            IValue indexExpression_iValue,
            IVariable markWhenSeen_varSimple1,
            bool breakOnFirstSet_b
        )
        {
            StatementRecordValue statementRecordValue = new StatementRecordValue
                                                            (indexSaveLocation_varSimple, indexExpression_iValue,
                                                             markWhenSeen_varSimple1, breakOnFirstSet_b);
            return statementRecordValue;
        }
    }
}
