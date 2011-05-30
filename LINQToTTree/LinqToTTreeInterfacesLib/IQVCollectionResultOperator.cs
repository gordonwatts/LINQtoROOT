
using System;
using System.ComponentModel.Composition.Hosting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
namespace LinqToTTreeInterfacesLib
{
    public interface IQVCollectionResultOperator
    {
        /// <summary>
        /// Return true if this plug-in can handle this particlar result operator type.
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        bool CanHandle(Type resultOperatorType);

        /// <summary>
        /// Sets upt eh code context, etc., so that we are now looping over whatever the results from this guy are.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        void ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, CompositionContainer container);
    }
}
