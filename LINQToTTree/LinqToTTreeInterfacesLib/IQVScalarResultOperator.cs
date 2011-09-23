﻿using System;
using System.ComponentModel.Composition.Hosting;
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

        IVariable ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, CompositionContainer container);
    }
}
