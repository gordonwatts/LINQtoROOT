//
// This file is almost completely copied, word-for-word, from the re-linq distribution. 
// 
using System.Linq.Expressions;
using System.Text;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Utilities;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Transforms an expression tree into a human-readable string, taking all the custom expression nodes into account.
    /// It does so by replacing all instances of custom expression nodes by parameters that have the desired string as their names. This is done
    /// to circumvent a limitation in the <see cref="Expression"/> class, where overriding <see cref="Expression.ToString"/> in custom expressions
    /// will not work.
    /// </summary>
    public class ExpressionStringConverter : ExpressionTreeVisitor
    {
        /// <summary>
        /// Starting from a simple expression, do the translation
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string Format(Expression expression)
        {
            ArgumentUtility.CheckNotNull("expression", expression);
            var transformedExpression = new ExpressionStringConverter().VisitExpression(expression);
            return transformedExpression.ToString();
        }

        private ExpressionStringConverter()
        {
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            ArgumentUtility.CheckNotNull("expression", expression);
            return Expression.Parameter(expression.Type, "[" + expression.ReferencedQuerySource.ItemName + "]");
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            ArgumentUtility.CheckNotNull("expression", expression);
            return Expression.Parameter(expression.Type, "{" + expression.QueryModel + "}");
        }

        protected override Expression VisitUnknownNonExtensionExpression(Expression expression)
        {
            ArgumentUtility.CheckNotNull("expression", expression);
            return Expression.Parameter(expression.Type, expression.ToString());
        }

        protected override Expression VisitExtensionExpression(ExtensionExpression expression)
        {
            ArgumentUtility.CheckNotNull("expression", expression);
            return Expression.Parameter(expression.Type, expression.ToString());
        }

        /// <summary>
        /// Some "opque" types need some help in their formatting.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitConstantExpression(ConstantExpression expression)
        {

            if (expression.Type.GetInterface(typeof(ROOTNET.Interface.NTH1).Name) != null)
            {
                StringBuilder bld = new StringBuilder();
                var h = expression.Value as ROOTNET.Interface.NTH1;
                bld.AppendFormat("value({0} - ({1},{2},{3}) bins ({4},{5},{6})-({7},{8},{9}) range)", expression.Type.Name,
                    h.NbinsX, h.NbinsY, h.NbinsZ,
                    h.Xaxis.Xmin, h.Yaxis.Xmin, h.Zaxis.Xmin,
                    h.Xaxis.Xmax, h.Yaxis.Xmax, h.Zaxis.Xmax
                    );
                return Expression.Parameter(expression.Type, bld.ToString());
            }
            else
            {
                return base.VisitConstantExpression(expression);
            }
        }
    }
}
