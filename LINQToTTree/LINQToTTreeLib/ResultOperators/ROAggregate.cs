﻿using System;
using System.ComponentModel.Composition;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// The aggragate operator - implement everything!
    /// </summary>
    [Export(typeof(IQVResultOperator))]
    public class ROAggregate : IQVResultOperator
    {
        /// <summary>
        /// We deal only with the aggregate operator here.
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(AggregateFromSeedResultOperator);
        }

        /// <summary>
        /// Deal with the aggregate operator coming in here.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <returns></returns>
        public IVariable ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedCode _codeEnv, ICodeContext context)
        {
            ///
            /// Basic code checks
            /// 

            AggregateFromSeedResultOperator a = resultOperator as AggregateFromSeedResultOperator;
            if (a == null)
            {
                throw new ArgumentNullException("result Operator must not be null and must be of type AggregateFromSeedResultOperator!");
            }

            if (a.Func.Parameters.Count != 1)
            {
                throw new InvalidOperationException("Aggregate only allows for a function with one parameters!");
            }

            if (a.OptionalResultSelector != null)
            {
                throw new NotImplementedException("Can't do a selector function yet");
            }

            ///
            /// We need to declare a variable to hold the seed and its updates - the accumulator
            /// We then need to write the code that does the update to the seed.
            /// Finally, if there is a final funciton, we need to call that after the loop is done!
            ///

            IVariable accumulator = null;
            if (a.Seed.Type.IsPointerType())
            {
                accumulator = new Variables.VarObject(a.Seed.Type);
            }
            else
            {
                accumulator = new Variables.VarSimple(a.Seed.Type);
            }
            accumulator.InitialValue = ExpressionVisitor.GetExpression(a.Seed, _codeEnv);

            ///
            /// Now, parse the lambda expression, doing a substitution with this guy! Note that the only argument is our
            /// accumulator - the other arguments have all been replaced with subqueryexpressions and the like!
            /// 

            var p1 = context.Add(a.Func.Parameters[0].Name, accumulator);
            var funcResolved = ExpressionVisitor.GetExpression(a.Func.Body, _codeEnv, context);
            p1.Pop();

            _codeEnv.Add(new StatementAssign(accumulator, funcResolved));

            return accumulator;

        }
    }
}
