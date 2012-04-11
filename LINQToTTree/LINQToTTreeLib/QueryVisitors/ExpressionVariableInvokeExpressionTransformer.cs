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
    public class ExpressionVariableInvokeExpressionTransformer : IExpressionTransformer<MethodCallExpression>, IExpressionTransformer<InvocationExpression>
    {
        public ExpressionVariableInvokeExpressionTransformer(ExpressionType[] typesIknow = null)
        {
            if (typesIknow == null)
            {
                SupportedExpressionTypes = new ExpressionType[] { ExpressionType.Call };
            }
            else
            {
                SupportedExpressionTypes = typesIknow;
            }
        }

        /// <summary>
        /// What we can deal with.
        /// </summary>
        public ExpressionType[] SupportedExpressionTypes { get; private set; }

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
        /// An invokation. Attempt to decode the Compile from an Expression. Support the normal ".NET" way of doing things
        /// as opposed to the "invoke" method above (which has a slightly cleaner syntax, but not much!).
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Expression Transform(InvocationExpression expression)
        {
            //
            // Fail quickly looking for the call
            //

            if (expression.Expression.NodeType != ExpressionType.Call)
                return expression;

            var callExpr = expression.Expression as MethodCallExpression;
            if (callExpr.Object == null)
                return expression;
            if (!typeof(Expression).IsAssignableFrom(callExpr.Object.Type))
                return expression;
            if (callExpr.Method.Name != "Compile")
                return expression;

            //
            // Now, pick a part the expression type and make sure it is a func!
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
            /// Keep track of the lambda's we are processing - so we can detect recursion.
            /// </summary>
            private Stack<LambdaExpression> _lambdasInProgress = new Stack<LambdaExpression>();

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

                var lexpr = expression.Arguments[0];
                var exprArgs = expression.Arguments.Skip(1).ToArray();
                return CallLambdaExpression(lexpr, exprArgs);
            }

            /// <summary>
            /// Given an expression that will resolve to our actual lambda expression, lift it and do
            /// argument replacement as instructed by the arguments that go along with it.
            /// </summary>
            /// <param name="exprOfLambda">Expression that when evaluated will lead to a ConstantExpression that is the Expression we should be lifting in.</param>
            /// <param name="args">The arguments to supply to the lambda function.</param>
            /// <returns></returns>
            private Expression CallLambdaExpression(Expression exprOfLambda, Expression[] args)
            {
                //
                // Resolve the expression to get a hold of the actual object.
                //

                var functionExpression = ExtractFunctionExpression(exprOfLambda) as Expression;
                if (functionExpression.NodeType != ExpressionType.Lambda)
                    throw new NotSupportedException("Expression isn't of type lambda");
                var lambda = functionExpression as LambdaExpression;

                //
                // Check for recursion. We don't actually alter teh stack at this point - if we call ourselves
                // again while processing an argument we don't care - that isn't recursion!
                //

                if (_lambdasInProgress.Any(l => l == lambda))
                    throw new NotSupportedException("Recursion isn't supported in expression function calling!");

                //
                // Ok - we got a real one. So the next thing to do is take the arguments
                // and add them to our list and dive down.
                //

                if (lambda.Parameters.Count != args.Length)
                    throw new InvalidOperationException("Number of parameters does not match!");

                foreach (var apair in lambda.Parameters.Zip(args, (p, a) => Tuple.Create(p, a)))
                {
                    if (_parameterLookup.ContainsKey(apair.Item1))
                        throw new InvalidOperationException("Can't resuse a parameter that is already used!");
                    _parameterLookup[apair.Item1] = VisitExpression(apair.Item2);
                }

                //
                // Now we are ready to process the body of the lambda and do the replacement. Mark this lambda as
                // being processed so we can detect recursion.
                //

                _lambdasInProgress.Push(lambda);
                var r = VisitExpression(lambda.Body);
                _lambdasInProgress.Pop();

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

            /// <summary>
            /// Someone is trying to invoke a delegate. Unless this look like expression.Compile()(args), we will just pass it on.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override Expression VisitInvocationExpression(InvocationExpression expression)
            {
                //
                // See if we can figure it out quickly, and just go on if this isn't the right type.
                // The x-checking isn't water tight here, but you'll have to go to some effort to
                // fool it (i.e. we don't check that the generic type is Expression, for example).
                //

                if (expression.Expression.NodeType != ExpressionType.Call)
                    return base.VisitInvocationExpression(expression);

                var callExpr = expression.Expression as MethodCallExpression;
                if (callExpr.Object == null
                    || !typeof(Expression).IsAssignableFrom(callExpr.Object.Type)
                    || callExpr.Method.Name != "Compile")
                {
                    return base.VisitInvocationExpression(expression);
                }

                Expression functionExpression = callExpr.Object as Expression;
                if (!functionExpression.Type.IsGenericType
                    || functionExpression.Type.GetGenericArguments().Length != 1)
                    return base.VisitInvocationExpression(expression);

                var expressionGenericArgs = functionExpression.Type.GetGenericArguments()[0];
                if (!expressionGenericArgs.IsGenericType
                    || !expressionGenericArgs.FullName.StartsWith("System.Func"))
                    return base.VisitInvocationExpression(expression);

                //
                // Great! We are set. Next, get ahold of the actual expression (the
                // const value).
                //

                return CallLambdaExpression(functionExpression, expression.Arguments.ToArray());
            }
        }
    }
}
