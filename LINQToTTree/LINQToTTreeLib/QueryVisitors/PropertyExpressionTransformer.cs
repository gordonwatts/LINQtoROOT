﻿using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.QueryVisitors
{
    /// <summary>
    /// When a member expression is referenced, see if there is an expression replacement
    /// we can make. If so, substitute it into the expression tree.
    /// </summary>
    /// <remarks>
    /// This is built to be used for things like "pt" => "pt_real/1000.0" or similar, so every
    /// reference can have the easy units and one doesn't have to keep re-writing it.
    /// </remarks>
    class PropertyExpressionTransformer : IExpressionTransformer<MemberExpression>
    {
        /// <summary>
        /// We should be called when dealing with a property access only.
        /// </summary>
        public ExpressionType[] SupportedExpressionTypes
        {
            get { return new ExpressionType[] { ExpressionType.MemberAccess }; }
        }

        /// <summary>
        /// Search for an expression property that matches the thing that is being referenced.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Expression Transform(MemberExpression expression)
        {
            // Get the name of the property and see if there is a static guy of the same name
            // attached to the object.
            var pname = expression.Member.Name;

            var pnameExpression = pname + "Expression";
            var minfo = expression.Expression.Type.GetField(pnameExpression);
            if (minfo == null || (minfo.Attributes | System.Reflection.FieldAttributes.Static) == 0)
            {
                return expression;
            }
            
            // Check to see if the signature is correct
            var funcType = typeof(Func<,>).MakeGenericType(expression.Expression.Type, expression.Type);
            var exprType = typeof(Expression<>).MakeGenericType(funcType);
            if (exprType != minfo.FieldType)
            {
                return expression;
            }

            // Get the expression that we will use in the replacement.
            var expr = minfo.GetValue(null);

            // Build it up as an Invoke guy.
            var exprToInvoke = Expression.Constant(expr, exprType);
            var methodGeneric = typeof(Helpers).GetMethods()
                .Where(m => m.Name == "Invoke" && m.GetParameters().Length == 2)
                .First();
            var method = methodGeneric.MakeGenericMethod(expression.Expression.Type, expression.Type);
            var invokeExpression = Expression.Call(null, method, exprToInvoke, expression.Expression);
            return invokeExpression;
        }
    }
}
