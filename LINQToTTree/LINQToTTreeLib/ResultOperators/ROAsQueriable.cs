using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.relinq;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// Deal with teh AsQueriable operator
    /// </summary>
    [Export(typeof(IQVCollectionResultOperator))]
    class ROAsQueriable : IQVCollectionResultOperator
    {
        /// <summary>
        /// Only the AsQueriable.
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(AsQueriableResultOperator);
        }

        /// <summary>
        /// The AsQueriable, for us, is a no-op. We will just pass things along.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        public void ProcessResultOperator(ResultOperatorBase resultOperator,
            QueryModel queryModel,
            IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, CompositionContainer container)
        {
        }
    }
}
