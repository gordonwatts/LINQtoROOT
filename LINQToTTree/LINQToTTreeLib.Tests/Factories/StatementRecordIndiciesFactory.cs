using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementRecordIndicies instances</summary>
    public static partial class StatementRecordIndiciesFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementRecordIndicies instances</summary>
        [PexFactoryMethod(typeof(Helpers), "LINQToTTreeLib.Statements.StatementRecordIndicies")]
        public static StatementRecordIndicies Create(IValue intToRecord_iValue, IVariable storageArray_iValue1)
        {
            StatementRecordIndicies statementRecordIndicies
               = new StatementRecordIndicies(intToRecord_iValue, storageArray_iValue1);
            return statementRecordIndicies;
        }
    }
}
