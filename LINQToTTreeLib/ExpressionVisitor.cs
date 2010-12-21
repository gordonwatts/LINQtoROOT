using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remotion.Data.Linq.Parsing;
using LinqToTTreeInterfacesLib;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using LINQToTTreeLib.Variables;
using Remotion.Data.Linq.Clauses.Expressions;
using System.Reflection;
using System.ComponentModel.Composition;
using LINQToTTreeLib.TypeHandlers;

namespace LINQToTTreeLib
{
    class ExpressionVisitor : ThrowingExpressionTreeVisitor
    {
        /// <summary>
        /// Helper routine to return the expression as a string.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static IValue GetExpression(Expression expr, IGeneratedCode ce, ICodeContext cc = null)
        {
            if (cc == null)
            {
                cc = new CodeContext();
            }
            var visitor = new ExpressionVisitor(ce, cc);
            visitor.VisitExpression(expr);
            return visitor.Result;
        }

        /// <summary>
        /// Hold onto the result that we will eventually pass back
        /// </summary>
        private IValue Result
        {
            get { return new ValSimple(_expr.ToString(), _exprType); }
        }

        private StringBuilder _expr = new StringBuilder();
        private Type _exprType = null;

        /// <summary>
        /// Sometimes we get sub-query expressions and other things that require us to write more than
        /// just plane functional code.
        /// </summary>
        private IGeneratedCode _codeEnv;

        /// <summary>
        /// Keep the context (scope, parameters, etc.)
        /// </summary>
        private ICodeContext _codeContext;

        /// <summary>
        /// ctor - only called by our helper routine above.
        /// </summary>
        /// <param name="ce"></param>
        private ExpressionVisitor(IGeneratedCode ce, ICodeContext cc)
        {
            _codeEnv = ce;
            _codeContext = cc;
        }

        /// <summary>
        /// List of the type handlers that we can use to process things.
        /// </summary>
        [Import]
        static public TypeHandlerCache TypeHandlers { get; set; }

        /// <summary>
        /// Deal with a constant expression. Exactly how this is dealt with depends on the value. We process
        /// the mmost important ones directly, and the MEF-off the others. :-)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitConstantExpression(ConstantExpression expression)
        {
            if (expression.Type == typeof(int)
                || expression.Type == typeof(float)
                || expression.Type == typeof(double))
            {
                _expr.AppendFormat("{0}", expression.Value);
                _exprType = expression.Type;
            }
            else if (expression.Type == typeof(bool))
            {
                if ((bool)expression.Value)
                {
                    _expr.Append("true");
                }
                else
                {
                    _expr.Append("false");
                }
                _exprType = typeof(bool);
            }
            else
            {
                var v = TypeHandlers.ProcessConstantReference(expression, _codeEnv);
                _expr.Append(v.RawValue);
                _exprType = v.Type;
            }

            return expression;
        }

        /// <summary>
        /// Parse out a binary expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            string op = "";
            bool CastToFinalType = false;

            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                    op = "+";
                    _exprType = expression.Type;
                    break;
                case ExpressionType.AndAlso:
                    CastToFinalType = true;
                    op = "&&";
                    _exprType = typeof(bool);
                    break;
                case ExpressionType.Divide:
                    op = "/";
                    _exprType = expression.Type;
                    break;
                case ExpressionType.Equal:
                    op = "==";
                    _exprType = typeof(bool);
                    break;
                case ExpressionType.GreaterThan:
                    op = ">";
                    _exprType = typeof(bool);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    op = ">=";
                    _exprType = typeof(bool);
                    break;
                case ExpressionType.LessThan:
                    op = "<";
                    _exprType = typeof(bool);
                    break;
                case ExpressionType.LessThanOrEqual:
                    op = "<=";
                    _exprType = typeof(bool);
                    break;
                case ExpressionType.Multiply:
                    op = "*";
                    _exprType = expression.Type;
                    break;
                case ExpressionType.OrElse:
                    CastToFinalType = true;
                    op = "||";
                    _exprType = typeof(bool);
                    break;
                case ExpressionType.Subtract:
                    op = "-";
                    _exprType = expression.Type;
                    break;

                case ExpressionType.And:
                    break;
                case ExpressionType.Modulo:
                    break;
                case ExpressionType.Power:
                    break;
                default:
                    break;
            }

            if (op == "")
            {
                return base.VisitBinaryExpression(expression);
            }
            
            ///
            /// Run the expression
            /// 

            var RHS = GetExpression(expression.Right, _codeEnv, _codeContext);
            var LHS = GetExpression(expression.Left, _codeEnv, _codeContext);

            string sRHS, sLHS;
            if (CastToFinalType)
            {
                sRHS = RHS.CastToType(expression.Type);
                sLHS = LHS.CastToType(expression.Type);
            }
            else
            {
                sRHS = RHS.AsCastString();
                sLHS = LHS.AsCastString();
            }

            _expr.AppendFormat("{0}{1}{2}", sLHS, op, sRHS);

            return expression;
        }

        /// <summary>
        /// Deal with a unary expression (like Not, for example).
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Negate:
                    _exprType = expression.Type;
                    _expr.AppendFormat("-{0}", GetExpression(expression.Operand, _codeEnv, _codeContext).AsCastString());
                    break;

                case ExpressionType.Not:
                    _exprType = expression.Type;
                    _expr.AppendFormat("!{0}", GetExpression(expression.Operand, _codeEnv, _codeContext).AsCastString());
                    break;

                case ExpressionType.Convert:
                    _exprType = expression.Type;
                    _expr.Append(GetExpression(expression.Operand, _codeEnv, _codeContext).CastToType(expression.Type));
                    break;

                default:
                    return base.VisitUnaryExpression(expression);
            }

            return expression;
        }

        /// <summary>
        /// We are referencing one of the query source variables... We are expecting others to do some work for us here. To first
        /// order, they need to define variables that we can use directly in the code. This means, for example, that the outter-most
        /// variable, which is the TTree GetEntry index variable, is defined to point to the TSelector's "this" variable! :-)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            _expr.Append(expression.ReferencedQuerySource.ItemName);
            _exprType = expression.ReferencedQuerySource.ItemType;

            return expression;
        }

        /// <summary>
        /// We are going to reference a member item - this is a simple "." coding.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            var baseExpr = GetExpression(expression.Expression, _codeEnv, _codeContext);
            _exprType = expression.Type;
            _expr.AppendFormat("{0}.{1}", baseExpr.AsObjectReference(), expression.Member.Name);

            return expression;
        }

        /// <summary>
        /// Look at some parameter - since the name should be already defined, this is actually quite easy!
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitParameterExpression(ParameterExpression expression)
        {
            _exprType = expression.Type;

            _expr.AppendFormat("{0}", _codeContext.GetReplacement(expression.Name, expression.Type).RawValue);

            return expression;
        }

        /// <summary>
        /// We have to process a lambda function. Put it inline (i.e. we make the assumption that no statements
        /// are used here). That would have to be version 10.0 or something like that.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitLambdaExpression(LambdaExpression expression)
        {
            _expr.Append(GetExpression(expression.Body, _codeEnv, _codeContext).RawValue);
            _exprType = expression.Body.Type;

            return expression;
        }

        /// <summary>
        /// Some method is being called. Translate this. This is painful b/c there may be method calls that aren't
        /// really method calls. For example, our special Helper functions. Or also there are the ROOT functions that
        /// need to be translated to C++.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            IValue result;
            var exprOut = TypeHandlers.ProcessMethodCall(expression, out result, _codeEnv, _codeContext);
            _expr.Append(result.RawValue);
            _exprType = result.Type;
            return exprOut;
        }

        /// <summary>
        /// If there is something in the expression we can't deal with, this method gets called and we can pop-it-out to warn
        /// the user they are trying to do something we can't deal with yet.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unhandledItem"></param>
        /// <param name="visitMethod"></param>
        /// <returns></returns>
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            string itemText = FormatUnhandledItem(unhandledItem);
            var message = string.Format("The expression '{0}' (type: {1}) is not supported by this LINQ provider.", itemText, typeof(T));
            return new NotSupportedException(message);
        }

        /// <summary>
        /// Helper function to format some info and pass it back as a string to maek the error message "better"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unhandledItem"></param>
        /// <returns></returns>
        private string FormatUnhandledItem<T>(T unhandledItem)
        {
            var itemAsExpression = unhandledItem as Expression;
            return itemAsExpression != null ? FormattingExpressionTreeVisitor.Format(itemAsExpression) : unhandledItem.ToString();
        }
    }
}
