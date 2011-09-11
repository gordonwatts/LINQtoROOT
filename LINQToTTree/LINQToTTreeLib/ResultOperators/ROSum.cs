using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
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
            return resultOperatorType == typeof(SumResultOperator);
        }

        public IVariable ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel,
            IGeneratedQueryCode gc, ICodeContext cc,
            CompositionContainer container)
        {
            if (resultOperator == null)
                throw new ArgumentNullException("resultOperator");
            if (cc == null)
                throw new ArgumentNullException("CodeContext can't be null");

            var sumOperator = resultOperator as SumResultOperator;

            //
            // We only know how to sum basic types
            //

            var sumType = cc.LoopVariable.Type;
            if (sumType != typeof(int)
                && sumType != typeof(short)
                && sumType != typeof(double)
                && sumType != typeof(float))
            {
                throw new InvalidOperationException(string.Format("Do not know how to generate C++ to sum type {0}.", sumType.Name));
            }

            var accumulator = new Variables.VarSimple(sumType);
            accumulator.InitialValue = new ValSimple("0", sumType);
            accumulator.Declare = true;

            //
            // Now, in the loop we are currently in, we do the "add".
            //

            var stack = cc.Add(accumulator.VariableName, accumulator.AsExpression());
            var p = Expression.Parameter(sumType, accumulator.VariableName);
            var add = Expression.Add(p, cc.LoopVariable);

            var addResolved = ExpressionToCPP.GetExpression(add, gc, cc, container);
            gc.Add(new StatementAggregate(accumulator, addResolved));

            stack.Pop();

            return accumulator;
        }
    }
}
