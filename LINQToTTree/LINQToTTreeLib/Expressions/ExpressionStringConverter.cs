//
// This file is almost completely copied, word-for-word, from the re-linq distribution. 
// 
using LinqToTTreeInterfacesLib;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Transforms an expression tree into a human-readable string, taking all the custom expression nodes into account.
    /// It does so by replacing all instances of custom expression nodes by parameters that have the desired string as their names. This is done
    /// to circumvent a limitation in the <see cref="Expression"/> class, where overriding <see cref="Expression.ToString"/> in custom expressions
    /// will not work.
    /// </summary>
    public class ExpressionStringConverter : RelinqExpressionVisitor
    {
        private bool _useHashCodes;
        /// <summary>
        /// Starting from a simple expression, do the translation
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string Format(Expression expression, bool useUniqueHashCodes = false)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            var transformedExpression = new ExpressionStringConverter(useUniqueHashCodes).Visit(expression);
            return transformedExpression.ToString();
        }

        private ExpressionStringConverter(bool useHashCodes)
        {
            _useHashCodes = useHashCodes;
        }

        protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            return Expression.Parameter(expression.Type, "[" + expression.ReferencedQuerySource.ItemName + "]");
        }

        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            return Expression.Parameter(expression.Type, "{" + expression.QueryModel + "}");
        }

        protected override Expression VisitExtension(Expression node)
        {
            return Expression.Parameter(node.Type, node.ToString());
        }

        /// <summary>
        /// If the method call is over an on-the-fly object, then we need to look
        /// at the C++ code when turning this into an expression.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object is ConstantExpression && node.Object.Type.GetInterfaces().Contains(typeof(IOnTheFlyCPPObject)))
            {
                // Get the C++ code and the include file listing, and turn it into a hash.
                var bld = new StringBuilder();
                var codeGenerator = (node.Object as ConstantExpression).Value as IOnTheFlyCPPObject;
                foreach (var l in codeGenerator.LinesOfCode(node.Method.Name))
                {
                    bld.Append(l);
                }
                if (codeGenerator.IncludeFiles() != null)
                {
                    foreach (var i in codeGenerator.IncludeFiles())
                    {
                        bld.Append(i);
                    }
                }

                // We alter the method name in the final string in order to deal with this.
                var r = base.VisitMethodCall(node);
                var rep = r.ToString()
                    .Replace(node.Method.Name, $"{node.Method.Name}-{bld.ToString().GetHashCode()}");
                return Expression.Parameter(node.Type, rep);
            }
            return base.VisitMethodCall(node);
        }

        /// <summary>
        /// Some "opque" types need some help in their formatting.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression expression)
        {
            if (expression.Type.GetInterface(typeof(ROOTNET.Interface.NTH1).Name) != null)
            {
                if (expression.Value == null)
                {
                    return Expression.Parameter(expression.Type, string.Format("value({0}-null)", expression.Type.FullName));
                }

                StringBuilder bld = new StringBuilder();
                var h = expression.Value as ROOTNET.Interface.NTH1;
                bld.AppendFormat("value({0} - ({1},{2},{3}) bins ({4},{5},{6})-({7},{8},{9}) range hash {10})", expression.Type.Name,
                    h.NbinsX, h.NbinsY, h.NbinsZ,
                    h.Xaxis.Xmin, h.Yaxis.Xmin, h.Zaxis.Xmin,
                    h.Xaxis.Xmax, h.Yaxis.Xmax, h.Zaxis.Xmax,
                    _useHashCodes ? h.GetHashCode() : -1
                    );
                return Expression.Parameter(expression.Type, bld.ToString());
            }
            else if (_useHashCodes && expression.Type.FullName.StartsWith("ROOTNET"))
            {
                var bld = new StringBuilder();
                bld.AppendFormat("value({0} - {1})", expression.Type.FullName, expression.Value == null ? 0 : expression.Value.GetHashCode());
                return Expression.Parameter(expression.Type, bld.ToString());
            }
            else
            {
                return base.VisitConstant(expression);
            }
        }
    }
}
