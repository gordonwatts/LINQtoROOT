using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// Implement the Sum operator in LINQ.
    /// </summary>
    [Export(typeof(IQVScalarResultOperator))]
    public class ROSum : IQVScalarResultOperator
    {
        /// <summary>
        /// Deal with just the Sum operator
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(SumResultOperator)
                || resultOperatorType == typeof(AverageResultOperator);
        }

        public Expression ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel,
            IGeneratedQueryCode gc, ICodeContext cc,
            CompositionContainer container)
        {
            if (resultOperator == null)
                throw new ArgumentNullException("resultOperator");
            if (cc == null)
                throw new ArgumentNullException("CodeContext can't be null");

            //
            // Determine the type of the result operator we are processing and
            // anything we need to know about it.
            //

            Type sumType;
            sumType = cc.LoopVariable.Type;
            bool doAverage = false;

            if (resultOperator is SumResultOperator)
            {
                doAverage = false;
            }
            else
            {
                doAverage = true;
            }

            //
            // We only know how to sum basic types
            //

            if (!sumType.IsNumberType())
            {
                throw new InvalidOperationException(string.Format("Do not know how to generate C++ to sum type {0}.", sumType.Name));
            }

            var accumulator = DeclarableParameter.CreateDeclarableParameterExpression(sumType);
            accumulator.SetInitialValue("0");

            //
            // Now, in the loop we are currently in, we do the "add".
            //

            var add = Expression.Add(accumulator, cc.LoopVariable);

            var addResolved = ExpressionToCPP.GetExpression(add, gc, cc, container);
            gc.Add(new StatementAggregate(accumulator, addResolved, FindDeclarableParameters.FindAll(add).Select(p => p.RawValue)));

            //
            // The sum will just be this accumulator - so return it.
            //

            if (!doAverage)
                return accumulator;

            //
            // If this is a average then we need to add a simple count on. Further, we need to declare
            // everything we are going to need for later.
            //

            var counter = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            counter.SetInitialValue("0");
            gc.AddOutsideLoop(counter);
            gc.AddOutsideLoop(accumulator);
            var incbyone = Expression.Add(counter, Expression.Constant(1));
            gc.Add(new StatementAggregate(counter, ExpressionToCPP.GetExpression(incbyone, gc, cc, container), FindDeclarableParameters.FindAll(incbyone).Select(p => p.RawValue)));

            // Pop out and calculate the average and return it.

            gc.Pop();

            var testForSomething = Expression.Equal(counter, Expression.Constant(0));
            gc.Add(new StatementThrowIfTrue(ExpressionToCPP.GetExpression(testForSomething, gc, cc, container), "Can't take an average of a null sequence"));

            var returnType = DetermineAverageReturnType(sumType);
            var faccumulator = Expression.Convert(accumulator, returnType);
            var fcount = Expression.Convert(counter, returnType);
            var divide = Expression.Divide(faccumulator, fcount);

            return divide;
        }


        public Tuple<bool, Expression> ProcessIdentityQuery(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, CompositionContainer container)
        {
            return null;
        }

        /// <summary>
        /// Given the input type, return the type for the Average operator.
        /// </summary>
        /// <param name="sumType"></param>
        /// <returns></returns>
        private Type DetermineAverageReturnType(Type sumType)
        {
            if (sumType == typeof(int)
                || sumType == typeof(long)
                || sumType == typeof(double))
                return typeof(double);

            if (sumType == typeof(float))
                return typeof(float);

            throw new NotSupportedException(string.Format("Average return for averaging over '{0}' not supported.", sumType.ToString()));
        }
    }
}
