using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.Variables;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Data.Linq.Parsing;
using System.Linq;

namespace LINQToTTreeLib
{
    class ExpressionVisitor : ThrowingExpressionTreeVisitor
    {
        /// <summary>
        /// Helper routine to return the expression as a string.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static IValue GetExpression(Expression expr, IGeneratedCode ce, ICodeContext cc = null, CompositionContainer container = null)
        {
            if (cc == null)
            {
                cc = new CodeContext();
            }
            var visitor = new ExpressionVisitor(ce, cc);
            visitor.MEFContainer = container;

            if (container != null)
            {
                CompositionBatch b = new CompositionBatch();
                b.AddPart(visitor);
                container.Compose(b);
            }

            ///
            /// First, see if there are any parameter replacements that can be done out-of-band
            /// 

            expr = ParameterReplacementExpressionVisitor.ReplaceParameters(expr, cc);

            ///
            /// Next, attempt to translate the expr (if needed)
            /// 

            string oldExpr = "";
            while (expr.ToString() != oldExpr)
            {
                oldExpr = expr.ToString();
                expr = TranslatingExpressionVisitor.Translate(expr);
            }

            ///
            /// Now, generate the C++ code
            /// 

            visitor.VisitExpression(expr);
            return visitor.Result;
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
                _result = new ValSimple(expression.Value.ToString(), expression.Type);
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
            else
            {
                _result = TypeHandlers.ProcessConstantReference(expression, _codeEnv, _codeContext, MEFContainer);
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
                    op = "[]";
                    format = "{0}[{2}]";
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

            var RHS = GetExpression(expression.Right, _codeEnv, _codeContext, MEFContainer);
            var LHS = GetExpression(expression.Left, _codeEnv, _codeContext, MEFContainer);

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

            StringBuilder bld = new StringBuilder();
            bld.AppendFormat(format, sLHS, op, sRHS);
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
                    _result = new ValSimple("-" + GetExpression(expression.Operand, _codeEnv, _codeContext, MEFContainer).AsCastString(), expression.Type);
                    break;

                case ExpressionType.Not:
                    _result = new ValSimple("!" + GetExpression(expression.Operand, _codeEnv, _codeContext, MEFContainer).AsCastString(), expression.Type);
                    break;

                case ExpressionType.Convert:
                    _result = new ValSimple(GetExpression(expression.Operand, _codeEnv, _codeContext, MEFContainer).CastToType(expression.Type), expression.Type);
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
            var arrayBase = GetExpression(expression.Operand, _codeEnv, _codeContext, MEFContainer);
            _result = new ValSimple(arrayBase.AsObjectReference() + ".size()", expression.Type);
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
            _result = _codeContext.GetReplacement(expression.ReferencedQuerySource.ItemName, expression.ReferencedQuerySource.ItemType);

            return expression;
        }

        /// <summary>
        /// We are going to reference a member item - this is a simple "." coding. If this is a reference
        /// to some sort of array, then we need to deal with getting back the proper array type.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            var baseExpr = GetExpression(expression.Expression, _codeEnv, _codeContext, MEFContainer);

            ///
            /// Figure out how to represent the variable type. We base this on the type - enumerables, for
            /// example, know how to loop, other things like "int" just know how to be simpe values. Eventually
            /// this will likely have to be made "common".
            /// 

            _result = null;
            if (expression.Type.IsGenericType)
            {
                if (expression.Type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    _result = new ValEnumerableVector(baseExpr.AsObjectReference() + "." + expression.Member.Name, expression.Type);
                }
                else
                {
                    throw new NotImplementedException("Can't deal with a generic type for iterator for " + expression.Type.Name + ".");
                }
            }
            else if (expression.Type.IsArray)
            {
                //var isA = expression.Type.
                _result = new ValEnumerableVector(baseExpr.AsObjectReference() + "." + expression.Member.Name, expression.Type);
            }

            ///
            /// If we can't figure out what the proper special variable type is from above, then we
            /// need to just fill in the default.
            /// 

            if (_result == null)
            {
                _result = new ValSimple(baseExpr.AsObjectReference() + "." + expression.Member.Name, expression.Type);
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
            _result = _codeContext.GetReplacement(expression.Name, expression.Type);

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
            _result = GetExpression(expression.Body, _codeEnv, _codeContext, MEFContainer);

            return expression;
        }

        /// <summary>
        /// We are doing an inline call to a lambda expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitInvocationExpression(InvocationExpression expression)
        {
            ///
            /// Declare all the parameters for lookup.
            /// 

            if (!(expression.Expression is LambdaExpression))
                throw new NotImplementedException("Do not know how to invoke a non-lambda call like '" + expression.ToString() + "'");
            var lambda = expression.Expression as LambdaExpression;

            var paramArgs = lambda.Parameters.Zip(expression.Arguments, (p, a) => Tuple.Create(p, a));
            var paramDefineToPopers = from pair in paramArgs
                                      select _codeContext.Add(pair.Item1.Name, GetExpression(pair.Item2, _codeEnv, _codeContext, MEFContainer));
            var allParamDefineToPopers = paramDefineToPopers.ToArray();

            ///
            /// Do the work!
            /// 

            _result = GetExpression(lambda, _codeEnv, _codeContext, MEFContainer);

            ///
            /// Now, pop everything off!
            /// 

            foreach (var param in allParamDefineToPopers)
            {
                param.Pop();
            }

            ///
            /// Done!
            /// 

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
            var exprOut = TypeHandlers.ProcessMethodCall(expression, out _result, _codeEnv, _codeContext, MEFContainer);
            return exprOut;
        }

        /// <summary>
        /// The user is making a sub-query. We will run the query and return it using the usual QueryVisitor dude, but unlike
        /// normal we have to run the loop ourselves.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            if (MEFContainer == null)
                throw new InvalidOperationException("MEFContainer can't be null if we need to analyze a sub query!");

            QueryVisitor qv = new QueryVisitor(_codeEnv, _codeContext);
            CompositionBatch b = new CompositionBatch();
            b.AddPart(qv);
            qv.SubExpressionParse = true;
            MEFContainer.Compose(b);
            qv.MEFContainer = MEFContainer;

            ///
            /// Run it - since this result is out of this loop, we pop-back-out when done.
            /// 

            var scope = _codeEnv.CurrentScope;
            qv.VisitQueryModel(expression.QueryModel);

            ///
            /// Two possible results from the sub-expression query, and how we proceed depends
            /// on what happened in the sub query
            /// 
            /// 1. <returns a value> - an operator like Count() comes back from the sequence.
            ///    it will get used in some later sequence (like # of jets in each event). So,
            ///    we need to make sure it is declared and kept before it is used. The # that comes
            ///    back needs to be used outside the scope we are sitting in - the one that we were at
            ///    when we started this.
            /// 2. <return a sequence> - this is weird - What we are actually doing here is putting the
            ///    sequence into code. So the loop variable has been updated with the new sequence iterator
            ///    value. But there isn't really a result! So the result will be null...
            /// 

            if (_codeEnv.ResultValue != null)
            {
                _codeEnv.CurrentScope = scope;
                _codeEnv.Add(_codeEnv.ResultValue);
                _result = _codeEnv.ResultValue;
            }

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
