﻿using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Replaces parameter references with known parameters
    /// </summary>
    internal class ParameterReplacementExpressionVisitor : ExpressionTreeVisitor
    {
        private ICodeContext _context;

        /// <summary>
        /// Creat the object and cahce the context for later parameter lookup.
        /// </summary>
        /// <param name="context"></param>
        protected ParameterReplacementExpressionVisitor(ICodeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return a new expression with the parameters replaced.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Expression ReplaceParameters(Expression expr, ICodeContext context)
        {
            var prep = new ParameterReplacementExpressionVisitor(context);
            return prep.VisitExpression(expr);
        }

        /// <summary>
        /// We are looking at some sort of parameter. If there is a replacement, then make it!
        /// </summary>
        /// <param name="paramExpr"></param>
        /// <returns></returns>
        protected override Expression VisitParameterExpression(ParameterExpression paramExpr)
        {
            var replaceit = _context.GetReplacement(paramExpr.Name);
            if (replaceit == null)
                return base.VisitParameterExpression(paramExpr);

            return replaceit;
        }

        /// <summary>
        /// See if we can find the main sub query parameter for lookup here.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            var replaceit = _context.GetReplacement(expression.ReferencedQuerySource.ItemName);
            if (replaceit == null)
                return expression;
            return replaceit;
        }

        /// <summary>
        /// We have to protect the lambda expression's parameters from accidental translation.
        /// 
        /// If a parameter is called "t" to a lambda function, and the parameter is defined above us in some other
        /// context, then we need to make sure that we hide it here so as we translate the expression we don't
        /// accidentally translate the rest of the stuff!
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitLambdaExpression(LambdaExpression expression)
        {
            var popers = (from a in expression.Parameters
                          select _context.Remove(a.Name)).ToArray();

            var result = base.VisitLambdaExpression(expression);

            foreach (var p in popers)
            {
                p.Pop();
            }

            return result;
        }
    }
}
