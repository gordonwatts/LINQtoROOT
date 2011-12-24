using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementMinMaxTest instances</summary>
    public static partial class StatementMinMaxTestFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementMinMaxTest instances</summary>
        [PexFactoryMethod(typeof(Helpers), "LINQToTTreeLib.Statements.StatementMinMaxTest")]
        public static StatementMinMaxTest Create(
            DeclarableParameter vIsFilled_varSimple,
            DeclarableParameter vMaxMin_varSimple1,
            IValue exprToMinOrMaximize_iValue,
            bool doMax_b
        )
        {
            StatementMinMaxTest statementMinMaxTest
               = new StatementMinMaxTest(vIsFilled_varSimple, vMaxMin_varSimple1,
                                         exprToMinOrMaximize_iValue, doMax_b);
            return statementMinMaxTest;
        }
    }
}
