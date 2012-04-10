using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;

namespace LINQToTTreeLib.QueryVisitors
{
    /// <summary>
    /// When a user puts a variable of type Expression with an argument of Func and appends .Invoke(arg1, arg2),
    /// this translator lifts the expression out and stuffs it inline... this is a way to do function calls, baby!
    /// </summary>
    public class ExpressionVariableInvokeExpressionTransformer : IExpressionTransformer<MethodCallExpression>
    {
        /// <summary>
        /// We deal only with the method calls.
        /// </summary>
        public ExpressionType[] SupportedExpressionTypes
        {
            get { return new ExpressionType[] { ExpressionType.Call }; }
        }

        /// <summary>
        /// If this is one of our Invoke method calls, then we can try to do expression replacement.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Expression Transform(MethodCallExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            //
            // Fail quickly if this isn't something we are interested in.
            //

            if (expression.Object != null
                || expression.Method.Name != "Invoke")
                return expression;

            if (expression.Method.DeclaringType != typeof(Helpers))
                return expression;

            //
            // Ok, this is real. Get the expression. The compiler should make sure that
            // all types match properly.
            //

            var exprFinder = new ExpressionFunctionExpander();
            return exprFinder.VisitExpression(expression);
        }

        /// <summary>
        /// Internal class that will expand a function and do parameter replacement.
        /// </summary>
        class ExpressionFunctionExpander : ExpressionTreeVisitor
        {
            /// <summary>
            /// A list of the parameters we will need to 
            /// </summary>
            private Dictionary<ParameterExpression, Expression> _parameterLookup = new Dictionary<ParameterExpression, Expression>();

            /// <summary>
            /// If this is a method call expression, then we should do the "usual" with it.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
            {
                //
                // Fail quickly if this isn't the right type of expression.
                //

                if (expression.Object != null
                    || expression.Method.Name != "Invoke")
                    return base.VisitMethodCallExpression(expression);

                if (expression.Method.DeclaringType != typeof(Helpers))
                    return base.VisitMethodCallExpression(expression);

                //
                // Get the expression. We have to extract the expression from somewhere in order to get at it here.
                // So this is a little ugly.
                //

                var functionExpression = ExtractFunctionExpression(expression.Arguments[0]) as Expression;
                if (functionExpression.NodeType != ExpressionType.Lambda)
                    throw new NotSupportedException("Expression isn't of type lambda");
                var lambda = functionExpression as LambdaExpression;

                //
                // Ok - we got a real one. So the next thing to do is take the arguments
                // and add them to our list and dive down.
                //

                if (lambda.Parameters.Count != (expression.Arguments.Count - 1))
                    throw new InvalidOperationException("Number of parameters does not match!");

                foreach (var apair in lambda.Parameters.Zip(expression.Arguments.Skip(1), (p, a) => Tuple.Create(p, a)))
                {
                    if (_parameterLookup.ContainsKey(apair.Item1))
                        throw new NotSupportedException("Can't resuse a parameter that is already used!");
                    _parameterLookup[apair.Item1] = VisitExpression(apair.Item2);
                }

                //
                // Now we are ready to process the body of the lambda and do the replacement.
                //

                var r = VisitExpression(lambda.Body);

                //
                // Next, clean up the parameter replacement
                //

                foreach (var p in lambda.Parameters)
                {
                    _parameterLookup.Remove(p);
                }

                return r;
            }

            /// <summary>
            /// We are given an expression that should evaluate to the expression that contains the function
            /// we are going to be lifting. Try to get it.
            /// </summary>
            /// <param name="expr"></param>
            /// <returns></returns>
            private Expression ExtractFunctionExpression(Expression expr)
            {
                var finder = new EvaluteExpressionFinder();
                var funcExpr = finder.VisitExpression(expr);
                if (funcExpr == null)
                    throw new NotSupportedException(string.Format("Unable to extract a function expression from '{0}' to use in an expression Invoke", expr.ToString()));

                if (funcExpr.NodeType != ExpressionType.Constant)
                    throw new NotSupportedException(string.Format("Unable to extract a function from expression '{0}'. It evaluated to an '{1}' expression, not the expected constant", expr.ToString(), funcExpr.NodeType.ToString()));
                var cexpr = funcExpr as ConstantExpression;
                if (!typeof(Expression).IsInstanceOfType(cexpr.Value))
                    throw new NotSupportedException(string.Format("Unable to extract a function from expression '{0}'. It evaluated to an expression of type '{1}', not 'Expression'.", expr.ToString(), cexpr.Type.Name));

                return cexpr.Value as Expression;
            }

            /// <summary>
            /// Helper class to evalute an expression to extract its actual value. We do the actual work. :(
            /// </summary>
            class EvaluteExpressionFinder : ExpressionTreeVisitor
            {
                /// <summary>
                /// A member of some item.
                /// </summary>
                /// <param name="expression"></param>
                /// <returns></returns>
                protected override Expression VisitMemberExpression(MemberExpression expression)
                {
                    //
                    // Get the base expression we can refer to access. If a static object
                    // comes through then we will have lots of ugly nulls, so that is why
                    // the expressions get a little complex here.
                    //

                    Expression val = null;
                    if (expression.Expression != null)
                    {
                        val = VisitExpression(expression.Expression);
                        if (val.NodeType != ExpressionType.Constant)
                        {
                            return Expression.MakeMemberAccess(val, expression.Member);
                        }
                    }

                    object obj = null;
                    if (val != null)
                        obj = (val as ConstantExpression).Value;

                    //
                    // Depending on what sort of member access we are getting ourselves into
                    // here, fetch the value.
                    //

                    object result = null;
                    if (expression.Member is PropertyInfo)
                    {
                        var pinfo = expression.Member as PropertyInfo;
                        result = pinfo.GetValue(obj, null);
                    }
                    else if (expression.Member is FieldInfo)
                    {
                        var finfo = expression.Member as FieldInfo;
                        result = finfo.GetValue(obj);
                    }
                    else
                    {
                        throw new NotImplementedException("Do not know how to deal with this type of access");
                    }

                    return Expression.Constant(result);
                }
            }

            /// <summary>
            /// A parameter has been seen. Do the replacement if we know about the parameter.
            /// </summary>
            /// <param name="paramExpr"></param>
            /// <returns></returns>
            protected override Expression VisitParameterExpression(ParameterExpression paramExpr)
            {
                Expression r;
                if (_parameterLookup.TryGetValue(paramExpr, out r))
                    return r;
                return paramExpr;
            }
        }
    }
}
