using Remotion.Linq.Parsing;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// This class will find all declarable parameters and execute some small amount 
    /// code for each declarable parameter that is found.
    /// </summary>
    public class FindDeclarableParameters : RelinqExpressionVisitor
    {
        public static IEnumerable<DeclarableParameter> FindAll(Expression expr)
        {
            var e = new FindDeclarableParameters();
            e.Visit(expr);
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
        protected override Expression VisitExtension(Expression expression)
        {
            if (expression is DeclarableParameter)
            {
                _foundParams.Add(expression as DeclarableParameter);
                return expression;
            }
            return base.VisitExtension(expression);
        }
    }
}
