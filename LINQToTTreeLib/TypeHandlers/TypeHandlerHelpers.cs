using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;
using System.Linq.Expressions;
using System.Diagnostics;
using System.ComponentModel.Composition;

namespace LINQToTTreeLib.TypeHandlers
{
    /// <summary>
    /// Handle type translation for the Helpers methods - this are just translation helpers
    /// that we are using to make life simpler.
    /// </summary>
    [Export(typeof(ITypeHandler))]
    class TypeHandlerHelpers : ITypeHandler
    {
        /// <summary>
        /// Only deal with helpers
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool CanHandle(Type t)
        {
            return t == typeof(Helpers);
        }

        /// <summary>
        /// These things never move across the boundry. So if this gets called then something very bad has gone wrong!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <returns></returns>
        public IValue ProcessConstantReference(System.Linq.Expressions.ConstantExpression expr, IGeneratedCode codeEnv)
        {
            /// There is something very broken if this guy is getting called!
            throw new NotImplementedException();
        }

        /// <summary>
        /// If one of the helper functions needs to be parsed, we end up here.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public System.Linq.Expressions.Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedCode gc, ICodeContext context)
        {
            if (expr == null)
                throw new ArgumentNullException("expr");

            if (expr.Method.Name == "ApplyReturnFirst")
            {
                ///
                /// Load out the parameter names we are looking at so we cna do the translation.
                /// 

                var parameters = expr.Method.GetParameters();
                var action = RaiseLambda(expr.Arguments[2]);

                var methodGenericArguments = expr.Method.GetGenericArguments();
                var actionType = typeof(Action<,>).MakeGenericType(new Type[] { methodGenericArguments[0], methodGenericArguments[1] });
                var expressionGeneric = typeof(Expression<>).MakeGenericType(new Type[] { actionType });
                var parameterSpec = expressionGeneric.GetProperty("Parameters");
                var lambdaParameters = (parameterSpec.GetValue(action, null) as IEnumerable<ParameterExpression>).ToArray();

                ///
                /// Next, do the lambda expression
                /// 

                var returnedValue = ExpressionVisitor.GetExpression(expr.Arguments[0], gc, context);
                context.Add(lambdaParameters[0].Name, returnedValue);
                context.Add(lambdaParameters[1].Name, ExpressionVisitor.GetExpression(expr.Arguments[1], gc, context));

                var statementBody = ExpressionVisitor.GetExpression(action, gc, context);
                gc.Add(new Statements.StatementSimpleStatement(statementBody.RawValue));

                ///
                /// Finally, what we will return if this is the last thing we are doing!
                /// 

                result = returnedValue;

                return expr;
            }
            else
            {
                throw new NotImplementedException("Helpers." + expr.Method.Name + " is not handled!");
            }
        }

        /// <summary>
        /// Pull the lambda out of there
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression RaiseLambda(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            if (expression.NodeType == ExpressionType.Constant)
            {
                var o = (expression as ConstantExpression);
                return RaiseLambda(o.Value as Expression);
            }
            else if (expression.NodeType == ExpressionType.Quote)
            {
                var o = (expression as UnaryExpression);
                return RaiseLambda(o.Operand);
            }
            else if (expression.NodeType == ExpressionType.Lambda)
            {
                return expression as LambdaExpression;
            }
            else
            {
                throw new ArgumentException("Unknown object - can't deal! - '" + expression.GetType().FullName + "'");
            }
        }
    }
}
