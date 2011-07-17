﻿using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// The aggragate operator - implement everything!
    /// </summary>
    [Export(typeof(IQVScalarResultOperator))]
    public class ROAggregate : IQVScalarResultOperator
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
        public IVariable ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext context, CompositionContainer container)
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
            accumulator.InitialValue = ExpressionToCPP.GetExpression(a.Seed, _codeEnv, context, container);
            accumulator.Declare = true;

            ///
            /// Now, parse the lambda expression, doing a substitution with this guy! Note that the only argument is our
            /// accumulator - the other arguments have all been replaced with subqueryexpressions and the like!
            /// 

            var p1 = context.Add(a.Func.Parameters[0].Name, accumulator);
            var funcResolved = ExpressionToCPP.GetExpression(a.Func.Body, _codeEnv, context, container);
            p1.Pop();

            _codeEnv.Add(new StatementAssign(accumulator, funcResolved));

            return accumulator;

        }
    }
}
