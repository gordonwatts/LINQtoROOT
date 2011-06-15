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
        /// Process the result operator, returning an expression that is the result, and also
        /// an initial value that the accumulator should be set to if needed. It has to be done out
        /// one from here b/c we may end up having to declare this in the class rather than in the code
        /// if it is at the outter level!
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        Tuple<Expression, IValue> ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, CompositionContainer container);
    }
}
