using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementFilter instances</summary>
    public static partial class StatementFilterFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementFilter instances</summary>
        [PexFactoryMethod(typeof(StatementFilter))]
        public static StatementFilter Create(IValue testExpression_iValue, IStatement[] statements, IVariable[] varsToAdd)
        {
            StatementFilter statementFilter = new StatementFilter(testExpression_iValue);
            foreach (var s in statements)
            {
                statementFilter.Add(s);
            }
            foreach (var v in varsToAdd)
            {
                statementFilter.Add(v);
            }
            return statementFilter;
        }
    }
}
