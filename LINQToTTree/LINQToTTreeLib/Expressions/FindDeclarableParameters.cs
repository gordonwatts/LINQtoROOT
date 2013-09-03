using Remotion.Linq.Parsing;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// This class will find all declarable parameters and execute some small amount 
    /// code for each declarable parameter that is found.
    /// </summary>
    public class FindDeclarableParameters : ExpressionTreeVisitor
    {
        public static IEnumerable<DeclarableParameter> FindAll(Expression expr)
        {
            var e = new FindDeclarableParameters();
            e.VisitExpression(expr);
            return e._foundParams;
        }

        /// <summary>
        /// List of the found parameters.
        /// </summary>
        private List<DeclarableParameter> _foundParams = new List<DeclarableParameter>();

        /// <summary>
        /// Visit an extension expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitExtensionExpression(Remotion.Linq.Clauses.Expressions.ExtensionExpression expression)
        {
            if (expression is DeclarableParameter)
            {
                _foundParams.Add(expression as DeclarableParameter);
                return expression;
            }
            return base.VisitExtensionExpression(expression);
        }
    }
}
