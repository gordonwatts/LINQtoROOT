using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Variables;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// A scalar operator for dealing with the min/max operators
    /// </summary>
    [Export(typeof(IQVScalarResultOperator))]
    internal class ROMinMax : IQVScalarResultOperator
    {
        /// <summary>
        /// Deal with the min and max result operators
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            if (resultOperatorType == typeof(MaxResultOperator)
                || resultOperatorType == typeof(MinResultOperator))
                return true;
            return false;
        }

        /// <summary>
        /// Code up the min/max result operators. We run the loop out, and then
        /// we return the result whatever it is. We only work when the type is
        /// something simple we can deal with!
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            ///
            /// Some argument checking
            /// 

            if (cc == null)
                throw new ArgumentNullException("cc");

            if (gc == null)
                throw new ArgumentNullException("gc");

            if (gc.Depth == 1)
                throw new ArgumentException("The Max/Min operators can't be used as result operators for a query - they can only be used in a sub-query");

            ///
            /// Is it min or max?
            /// 

            var minOperator = resultOperator as MinResultOperator;
            var maxOperator = resultOperator as MaxResultOperator;

            if (minOperator == null && maxOperator == null)
                throw new InvalidOperationException("Should always have min or max operator!");

            bool doMax = maxOperator != null;

            bool returnDefaultValue = false;
            if (doMax)
                returnDefaultValue = maxOperator.ReturnDefaultWhenEmpty;
            else
                returnDefaultValue = minOperator.ReturnDefaultWhenEmpty;

            ///
            /// Next, look at the type of the current result that is running.
            /// 

            var valueExpr = queryModel.SelectClause.Selector;
            if (!TimeCanBeCompared(valueExpr.Type))
                throw new ArgumentException(string.Format("I don't know how to fix the min or max of a sequence of '{0}'s", cc.LoopVariable.Type.Name));

            ///
            /// Now, declare two variables, one bool which gets set when we get the first value,
            /// and the other to hold the min/max value! Note that we initalize the variable to
            /// the proper type. We don't declare minmax holder - as it may end up be used
            /// externally.
            /// 

            var vIsFilled = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            vIsFilled.InitialValue = new ValSimple("false", typeof(bool));
            var vMaxMin = DeclarableParameter.CreateDeclarableParameterExpression(valueExpr.Type);
            vMaxMin.InitialValue = new ValSimple("0", typeof(int));

            gc.AddOneLevelUp(vIsFilled);

            ///
            /// The expression we want to mimize or maximize
            /// 

            var exprToMinOrMaximize = ExpressionToCPP.GetExpression(valueExpr, gc, cc, container);

            ///
            /// Now, we just have to put the x-checks in there.
            /// 

            var ifStatement = new Statements.StatementMinMaxTest(vIsFilled, vMaxMin, exprToMinOrMaximize, doMax);
            gc.Add(ifStatement);

            return vMaxMin;
        }

        /// <summary>
        /// Is this a type C++ knows how to compare??
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool TimeCanBeCompared(Type type)
        {
            if (type == typeof(char))
                return true;
            if (type == typeof(int))
                return true;
            if (type == typeof(short))
                return true;
            if (type == typeof(float))
                return true;
            if (type == typeof(double))
                return true;

            return false;
        }
    }
}
