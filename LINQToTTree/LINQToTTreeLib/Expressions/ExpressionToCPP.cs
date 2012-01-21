﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Parsing;

namespace LINQToTTreeLib.Expressions
{
    class ExpressionToCPP : ThrowingExpressionTreeVisitor
    {
        /// <summary>
        /// Helper routine to return the expression as a string.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static IValue GetExpression(Expression expr, IGeneratedQueryCode ce, ICodeContext cc, CompositionContainer container)
        {
            if (expr == null)
                return null;

            if (cc == null)
            {
                cc = new CodeContext();
            }
            return InternalGetExpression(expr.Resolve(ce, cc, container), ce, cc, container);
        }

        /// <summary>
        /// Internal expression resolver. This routine is temporary - needed as we move
        /// to the new system...
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="ce"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static IValue InternalGetExpression(Expression expr, IGeneratedQueryCode ce, ICodeContext cc, CompositionContainer container)
        {
            if (expr == null)
                return null;

            if (cc == null)
            {
                cc = new CodeContext();
            }

            var visitor = new ExpressionToCPP(ce, cc);
            visitor.MEFContainer = container;

            if (container != null)
            {
                container.SatisfyImportsOnce(visitor);
            }

            visitor.VisitExpression(expr);
            return visitor.Result;
        }

        /// <summary>
        /// Local version of get expression that passes on all of our information. This is basically
        /// a syntatic shortcut.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private IValue GetExpression(Expression expr)
        {
            return InternalGetExpression(expr, _codeEnv, _codeContext, MEFContainer);
        }

        /// <summary>
        /// Hold onto the result that we will eventually pass back
        /// </summary>
        private IValue Result
        {
            get { return _result; }
        }

        /// <summary>
        /// This is where the result is put
        /// </summary>
        IValue _result = null;

        /// <summary>
        /// Sometimes we get sub-query expressions and other things that require us to write more than
        /// just plane functional code.
        /// </summary>
        private IGeneratedQueryCode _codeEnv;

        /// <summary>
        /// Keep the context (scope, parameters, etc.)
        /// </summary>
        private ICodeContext _codeContext;

        /// <summary>
        /// ctor - only called by our helper routine above.
        /// </summary>
        /// <param name="ce"></param>
        private ExpressionToCPP(IGeneratedQueryCode ce, ICodeContext cc)
        {
            _codeEnv = ce;
            _codeContext = cc;
        }

        /// <summary>
        /// List of the type handlers that we can use to process things.
        /// </summary>
        [Import]
        private TypeHandlerCache TypeHandlers { get; set; }

        /// <summary>
        /// Deal with a constant expression. Exactly how this is dealt with depends on the value. We process
        /// the mmost important ones directly, and the MEF-off the others. :-)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitConstantExpression(ConstantExpression expression)
        {
            if (expression.Type == typeof(int))
            {
                _result = new ValSimple(expression.Value.ToString(), expression.Type);
            }
            else if (expression.Type == typeof(float)
              || expression.Type == typeof(double))
            {
                var s = expression.Value.ToString();
                if (!s.Contains("."))
                    s += ".0";
                _result = new ValSimple(s, expression.Type);
            }
            else if (expression.Type == typeof(bool))
            {
                if ((bool)expression.Value)
                {
                    _result = new ValSimple("true", typeof(bool));
                }
                else
                {
                    _result = new ValSimple("false", typeof(bool));
                }
            }
            else if (expression.Type == typeof(string))
            {
                _result = new ValSimple(expression.Value as string, typeof(string));
            }
            else
            {
                _result = TypeHandlers.ProcessConstantReference(expression, _codeEnv, MEFContainer);
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
            string format = "{0}{1}{2}";

            Type resultType = null;
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                    op = "+";
                    resultType = expression.Type;
                    break;
                case ExpressionType.AndAlso:
                    CastToFinalType = true;
                    op = "&&";
                    resultType = typeof(bool);
                    break;
                case ExpressionType.Divide:
                    op = "/";
                    resultType = expression.Type;
                    break;
                case ExpressionType.Equal:
                    op = "==";
                    resultType = typeof(bool);
                    break;
                case ExpressionType.GreaterThan:
                    op = ">";
                    resultType = typeof(bool);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    op = ">=";
                    resultType = typeof(bool);
                    break;
                case ExpressionType.LessThan:
                    op = "<";
                    resultType = typeof(bool);
                    break;
                case ExpressionType.LessThanOrEqual:
                    op = "<=";
                    resultType = typeof(bool);
                    break;

                case ExpressionType.NotEqual:
                    op = "!=";
                    resultType = typeof(bool);
                    break;

                case ExpressionType.Multiply:
                    op = "*";
                    resultType = expression.Type;
                    break;
                case ExpressionType.OrElse:
                    CastToFinalType = true;
                    op = "||";
                    resultType = typeof(bool);
                    break;
                case ExpressionType.Subtract:
                    op = "-";
                    resultType = expression.Type;
                    break;


                case ExpressionType.ArrayIndex:
                    resultType = expression.Type;
                    op = "at";
                    format = "{0}.at({2})";
                    break;

                case ExpressionType.And:
                    break;
                case ExpressionType.Modulo:
                    op = "%";
                    resultType = expression.Type;
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

            //
            // Comparing expressions anonymous expressions are not supported.
            //

            if (expression.Right.Type.Name.Contains("Anonymous"))
                throw new ArgumentException(string.Format("Binary operators on Anonymous types are not supported: '{0}' - '{1}'", expression.Right.Type.Name, expression.Right.ToString()));

            ///
            /// Run the expression
            /// 

            var RHS = GetExpression(expression.Right);
            var LHS = GetExpression(expression.Left);

            string sRHS, sLHS;
            if (CastToFinalType)
            {
                sRHS = RHS.CastToType(expression);
                sLHS = LHS.CastToType(expression);
            }
            else
            {
                sRHS = RHS.CastToType(expression.Right);
                sLHS = LHS.CastToType(expression.Left);
            }

            StringBuilder bld = new StringBuilder();
            bld.AppendFormat(format, sLHS.ApplyParensIfNeeded(), op, sRHS.ApplyParensIfNeeded());
            _result = new ValSimple(bld.ToString(), resultType);

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
                    _result = new ValSimple("-" + GetExpression(expression.Operand).CastToType(expression).ApplyParensIfNeeded(), expression.Type);
                    break;

                case ExpressionType.Not:
                    _result = new ValSimple("!" + GetExpression(expression.Operand).CastToType(expression).ApplyParensIfNeeded(), expression.Type);
                    break;

                case ExpressionType.Convert:
                    _result = new ValSimple(GetExpression(expression.Operand).CastToType(expression), expression.Type);
                    break;

                case ExpressionType.ArrayLength:
                    VisitArrayLength(expression);
                    break;

                default:
                    return base.VisitUnaryExpression(expression);
            }

            return expression;
        }

        /// <summary>
        /// The expression is trying to figure out how long this array is.
        /// </summary>
        /// <param name="expression"></param>
        private void VisitArrayLength(UnaryExpression expression)
        {
            var arrayBase = GetExpression(expression.Operand);
            _result = new ValSimple(arrayBase.AsObjectReference(expression.Operand) + ".size()", expression.Type);
        }

        /// <summary>
        /// We are going to reference a member item - this is a simple "." coding. If this is a reference
        /// to some sort of array, then we need to deal with getting back the proper array type.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            //
            // See if we have special handling for this.
            //

            var r = TypeHandlers.TryMemberReference(expression, _codeEnv, _codeContext, MEFContainer);
            if (r != null)
            {
                _result = r;
                return expression;
            }

            //
            // Ok - we need to do this the normal way - we split things in
            // two - and do the base expression and then try to apply the
            // member to that.
            //

            var baseExpr = GetExpression(expression.Expression);

            ///
            /// Figure out how to represent the variable type. We base this on the type - enumerables, for
            /// example, know how to loop, other things like "int" just know how to be simpe values. Eventually
            /// this will likely have to be made "common".
            /// 

            _result = null;
            var leafName = expression.Member.Name;
            _codeEnv.AddReferencedLeaf(leafName);

            if (expression.Type.IsGenericType)
            {
                if (expression.Type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    _result = new ValSimple(baseExpr.AsObjectReference() + "." + leafName, expression.Type);
                }
                else
                {
                    throw new NotImplementedException("Can't deal with a generic type for iterator for " + expression.Type.Name + ".");
                }
            }
            else if (expression.Type.IsArray)
            {
                _result = new ValSimple(baseExpr.AsObjectReference() + "." + leafName, expression.Type);
            }

            ///
            /// If we can't figure out what the proper special variable type is from above, then we
            /// need to just fill in the default.
            /// 

            if (_result == null)
            {
                _result = new ValSimple(baseExpr.AsObjectReference() + "." + leafName, expression.Type);
            }

            return expression;
        }

        /// <summary>
        /// Look at some parameter - since the name should be already defined, this is actually quite easy!
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitParameterExpression(ParameterExpression expression)
        {
            _result = new ValSimple(expression.Name, expression.Type);

            return expression;
        }

        /// <summary>
        /// Look at one of our extension expressions - see if we can deal with it whatever we are looking at here.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitExtensionExpression(Remotion.Linq.Clauses.Expressions.ExtensionExpression expression)
        {
            if (expression.NodeType == DeclarableParameter.ExpressionType)
            {
                var decl = expression as DeclarableParameter;
                _result = new ValSimple(decl.ParameterName, decl.Type);
                return expression;
            }
            else
            {
                return base.VisitExtensionExpression(expression);
            }
        }

        /// <summary>
        /// We have to process a lambda function. Put it inline (i.e. we make the assumption that no statements
        /// are used here). That would have to be version 10.0 or something like that.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitLambdaExpression(LambdaExpression expression)
        {
            _result = GetExpression(expression.Body);

            return expression;
        }

        /// <summary>
        /// Someone is doing a new in the middle of this LINQ operation... we need to handle that, I guess,
        /// and translate it to a new in C++.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitNewExpression(NewExpression expression)
        {
            var exprOut = TypeHandlers.ProcessNew(expression, out _result, _codeEnv, MEFContainer);
            return exprOut;
        }

        /// <summary>
        /// Some method is being called. Offer plug-ins a chance to transform this method call.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            _result = TypeHandlers.CodeMethodCall(expression, _codeEnv, MEFContainer);
            return expression;
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

        /// <summary>
        /// Get/Set the MEF container used when we create new objects (like a QV).
        /// </summary>
        public CompositionContainer MEFContainer { get; set; }
    }
}
