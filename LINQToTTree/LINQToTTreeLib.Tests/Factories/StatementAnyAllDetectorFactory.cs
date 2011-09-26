using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    /// <summary>A factory for LINQToTTreeLib.Statements.StatementAnyAllDetector instances</summary>
    public static partial class StatementAnyAllDetectorFactory
    {
        /// <summary>A factory for LINQToTTreeLib.Statements.StatementAnyAllDetector instances</summary>
        [PexFactoryMethod(typeof(StatementAnyAllDetector))]
        public static StatementAnyAllDetector Create(IValue predicate_iValue, IDeclaredParameter aresult_varSimple, string markedValue_s)
        {
            StatementAnyAllDetector statementAnyAllDetector = new StatementAnyAllDetector
                                                                  (predicate_iValue, aresult_varSimple, markedValue_s);
            return statementAnyAllDetector;
        }
    }
}
