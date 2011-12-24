using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementAssign instances</summary>
    public static partial class StatementAssignFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementAssign instances</summary>
        [PexFactoryMethod(typeof(StatementAssign))]
        public static StatementAssign Create(IDeclaredParameter result_iVariable, IValue val_iValue)
        {
            StatementAssign statementAssign
               = new StatementAssign(result_iVariable, val_iValue);
            return statementAssign;

            // TODO: Edit factory method of StatementAssign
            // This method should be able to configure the object in all possible ways.
            // Add as many parameters as needed,
            // and assign their values to each field by using the API.
        }
    }
}
