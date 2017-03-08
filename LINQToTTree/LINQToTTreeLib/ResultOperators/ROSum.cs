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
    /// Thrown if you try to do an average at the top level.
    /// </summary>
    [Serializable]
    public class AverageNotAllowedAtTopLevelException : Exception
    {
        public AverageNotAllowedAtTopLevelException() { }
        public AverageNotAllowedAtTopLevelException(string message) : base(message) { }
        public AverageNotAllowedAtTopLevelException(string message, Exception inner) : base(message, inner) { }
        protected AverageNotAllowedAtTopLevelException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

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

            // Determine the type of the result operator we are processing and
            // anything we need to know about it.
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

            // We only know how to sum basic types
            if (!sumType.IsNumberType())
            {
                throw new InvalidOperationException(string.Format("Do not know how to generate C++ to sum type {0}.", sumType.Name));
            }

            var accumulator = DeclarableParameter.CreateDeclarableParameterExpression(sumType);
            accumulator.SetInitialValue("0");

            // Sum and average are a alike in that we are going to add everything we see up.
            var add = Expression.Add(accumulator, cc.LoopVariable);
            var addResolved = ExpressionToCPP.GetExpression(add, gc, cc, container);
            gc.Add(new StatementAggregate(accumulator, addResolved));

            // If this is a sum no further work needs to happen.
            if (!doAverage)
                return accumulator;

            // If this is a average then we need to add a simple count.
            var counter = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            counter.SetInitialValue("0");
            var incbyone = Expression.Add(counter, Expression.Constant(1));
            gc.Add(new StatementAggregate(counter, ExpressionToCPP.GetExpression(incbyone, gc, cc, container)));

            // Next, we have to delcare the counter and the accumulator. These are now both temprorary variables.
            if (cc.LoopIndexVariable == null)
            {
                throw new AverageNotAllowedAtTopLevelException("Attempt to use Average at top level, accross events. Not currently implemented.");
            }
            gc.AddOutsideLoop(counter);
            gc.AddOutsideLoop(accumulator);

            // It is an error to average a sequence with no elements. So we need to throw a C++ exception. We need to pop up out of the loop in order
            // to do this.
            // http://msdn.microsoft.com/en-us/library/bb354760.aspx (for specs on Average on this).

            var testForSomething = Expression.Equal(counter, Expression.Constant(0));
            gc.AddAtResultScope(new StatementThrowIfTrue(ExpressionToCPP.GetExpression(testForSomething, gc, cc, container), "Can't take an average of a null sequence"));

            var returnType = DetermineAverageReturnType(sumType);
            var faccumulator = Expression.Convert(accumulator, returnType);
            var fcount = Expression.Convert(counter, returnType);
            var divide = Expression.Divide(faccumulator, fcount);

            // We are done with this calculation, so pop up and out.
            gc.Pop();

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
