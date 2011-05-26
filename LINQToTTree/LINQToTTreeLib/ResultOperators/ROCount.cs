using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// Implement the result operator that will deal with the Count predicate.
    /// </summary>
    [Export(typeof(IQVScalarResultOperator))]
    class ROCount : IQVScalarResultOperator
    {
        /// <summary>
        /// Get the proper value here.
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(CountResultOperator);
        }

        /// <summary>
        /// Actually try and process this! The count consisits of a count integer and something to increment it
        /// at its current spot.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="codeEnv"></param>
        /// <returns></returns>
        public IVariable ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode codeEnv, ICodeContext codeContext, CompositionContainer container)
        {
            if (codeEnv == null)
                throw new ArgumentNullException("CodeEnv must not be null!");

            var c = resultOperator as CountResultOperator;
            if (c == null)
                throw new ArgumentNullException("resultOperator can only be a CountResultOperator and must not be null");

            var intResult = ImplementCount(codeEnv);

            return intResult;
        }

        /// <summary>
        /// Actually write the code for the count operator.
        /// </summary>
        /// <param name="codeEnv"></param>
        /// <returns></returns>
        public static IVariable ImplementCount(IGeneratedQueryCode codeEnv)
        {
            var intResult = new VarInteger();

            codeEnv.Add(new StatementIncrementInteger(intResult));
            return intResult;
        }
    }
}
