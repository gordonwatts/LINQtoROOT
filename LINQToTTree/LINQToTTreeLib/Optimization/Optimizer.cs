
using LinqToTTreeInterfacesLib;
namespace LINQToTTreeLib.Optimization
{
    /// <summary>
    /// This is the top level code statement optimizer. It runs on GeneratedCode, and does its best to optimize
    /// the statement ordering.
    /// </summary>
    class Optimizer
    {
        /// <summary>
        /// Apply all known optimizations to the GenerateCode result we have on hand.
        /// </summary>
        /// <param name="result"></param>
        internal static void Optimize(GeneratedCode result)
        {
            StatementLifter.Optimize(result);
        }

        /// <summary>
        /// Optimize the common code.
        /// </summary>
        /// <param name="result"></param>
        internal static void Optimize(IExecutableCode result)
        {
            CommonStatementLifter.Optimize(result);
        }
    }
}
