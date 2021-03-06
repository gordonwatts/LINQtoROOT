﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;

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
        public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedQueryCode codeEnv, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// These guys never get in as a constant - if they do then who knows what is going on! BOOM!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="codeEnv"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessConstantReferenceExpression(ConstantExpression expr, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If one of the helper functions needs to be parsed, we end up here.
        /// Note: this is a bit tricky, and, worse, this is not tested (too complex :-)).
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public System.Linq.Expressions.Expression ProcessMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
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
                /// Next, do the lambda expression. Order of p1 and p2 is b/c we should make sure that it happens
                /// before any parameters are replaced! Note we parse the body of the lambda here! Parameters are defined and should
                /// correctly deal with any substitution in process.
                /// 

                var p2 = context.Add(lambdaParameters[1].Name, expr.Arguments[1]);
                var p1 = context.Add(lambdaParameters[0].Name, expr.Arguments[0]);

                var statementBody = ExpressionToCPP.GetExpression(action.Body.Resolve(gc, context, container), gc, context, container);

                p1.Pop();
                p2.Pop();

                gc.Add(new Statements.StatementSimpleStatement(statementBody.RawValue, resultVars: new string[0] { }, dependentVars: statementBody.Dependants.Select(i => i.RawValue).ToArray()));

                ///
                /// Finally, what we will return if this is the last thing we are doing!
                /// 

                return expr.Arguments[0];
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
        private LambdaExpression RaiseLambda(Expression expression)
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

        /// <summary>
        /// No one should ever be trying to pass a new of these guys over! :-)
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        /// <param name="gc"></param>
        /// <param name="context"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// By the time we get to coding nothing like this guy should be around!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public IValue CodeMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, CompositionContainer container)
        {
            throw new NotImplementedException();
        }


        public IValue ProcessMemberReference(MemberExpression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
