using System;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// This gets called when a result operator is run.
    /// </summary>
    public interface IQVResultOperator
    {
        /// <summary>
        /// Return true if this plug-in can handle this particlar result operator type.
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        bool CanHandle(Type resultOperatorType);

        IVariable ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedCode _codeEnv, ICodeContext _codeContext);
    }
}
