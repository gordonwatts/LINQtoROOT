using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementIfOnCount instances</summary>
    public static partial class StatementIfOnCountFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementIfOnCount instances</summary>
        [PexFactoryMethod(typeof(StatementIfOnCount))]
        public static StatementIfOnCount Create(IDeclaredParameter valueLeft_iValue, IValue valueRight_iValue1, StatementIfOnCount.ComparisonOperator comp_i, IStatement[] statements, IDeclaredParameter[] vars)
        {
            StatementIfOnCount statementIfOnCount
               = new StatementIfOnCount(valueLeft_iValue, valueRight_iValue1, comp_i);

            if (statements != null)
                foreach (var s in statements)
                {
                    statementIfOnCount.Add(s);
                }

            if (vars != null)
                foreach (var v in vars)
                {
                    statementIfOnCount.Add(v);
                }

            return statementIfOnCount;
        }
    }
}
