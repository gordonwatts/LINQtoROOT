using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// This gets called when a result operator is run.
    /// </summary>
    public interface IQVScalarResultOperator
    {
        /// <summary>
        /// Return true if this plug-in can handle this particlar result operator type.
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        bool CanHandle(Type resultOperatorType);

        /// <summary>
        /// Return an expression that represents the result for this scalar operator.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        Expression ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, CompositionContainer container);
    }
}
