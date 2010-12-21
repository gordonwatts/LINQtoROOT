﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib.Statements;
using System.Diagnostics;
using System.ComponentModel.Composition;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Implement the result operator that will deal with the Count predicate.
    /// </summary>
    [Export(typeof(IQVResultOperator))]
    class ROCount : IQVResultOperator
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
        public IVariable ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedCode codeEnv)
        {
            if (codeEnv == null)
                throw new ArgumentNullException("CodeEnv must not be null!");

            var c = resultOperator as CountResultOperator;
            if (c == null)
                throw new ArgumentNullException("resultOperator can only be a CountResultOperator and must not be null");

            var intResult = new VarInteger();

            codeEnv.Add(new StatementIncrementInteger(intResult));

            return intResult;
        }
    }
}
