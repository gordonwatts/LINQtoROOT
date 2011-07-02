using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementSimpleStatement instances</summary>
    public static partial class StatementSimpleStatementFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementSimpleStatement instances</summary>
        [PexFactoryMethod(typeof(StatementSimpleStatement))]
        public static StatementSimpleStatement Create(string line_s, bool addSemicolon_b)
        {
            StatementSimpleStatement statementSimpleStatement
               = new StatementSimpleStatement(line_s, addSemicolon_b);
            return statementSimpleStatement;
        }
    }
}
