using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using Remotion.Linq.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Convert an expression to C++... everything else should have been done by now.
    /// </summary>
    class ExpressionToCPP : ThrowingExpressionVisitor
    {
        /// <summary>
        /// Helper routine to return the expression as a string.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static IValue GetExpression(Expression expr, IGeneratedQueryCode ce, ICodeContext cc, CompositionContainer container)
        {
            try
            {
                // Get setup and deal with simple cases.
                Debug.WriteLine("ExpressionToCPP: Parsing {0}{1}", expr.ToString(), "");
                Debug.Indent();
                if (expr == null)
                    return null;

                if (cc == null)
                {
                    cc = new CodeContext();
                }

                // Special case we can deal with seperately - when there is a bool and an AndAlso or OrElse.
                // We want to make sure we guard the second expression so it isn't executed if it isn't needed.
                if (expr.Type == typeof(bool)
                    && (expr.NodeType == ExpressionType.AndAlso || expr.NodeType == ExpressionType.OrElse))
                {
                    return GetExpressionForBoolAndOr(expr, ce, cc, container);
                }

                // Cache the list of variables that need to be eliminated when CPP is done.
                var cachedScopedVariables = cc.ResetCachedVariableList();

                var r = InternalGetExpression(expr.Resolve(ce, cc, container), ce, cc, container).PerformAllSubstitutions(cc);
                Debug.WriteLine("ExpressionToCPP: Returning value {0}{1}", r.ToString(), "");

                cc.PopCachedVariableList();

                cc.LoadCachedVariableList(cachedScopedVariables);
                return r;
            }
            finally
            {
                Debug.Unindent();
            }
        }

        /// <summary>
        /// We are looking at a&&b or a||b. We wnat to make sure we evaluate b iff we need it, depending on the result of a.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="ce"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        /// <remarks>
        /// To prevent us from updating variables (which makes optmization harder), we will implement the code as follows for a&&b:
        /// bool_1 = false; bool_2 = false; bool_3 = false;
        /// bool_1 = a
        /// if (bool_1) bool_2 = b
        /// bool_3 = bool_1 && bool_2
        ///</remarks>
        private static IValue GetExpressionForBoolAndOr(Expression expr, IGeneratedQueryCode ce, ICodeContext cc, CompositionContainer container)
        {
            // Svae to make sure we can get back.
            var outterScope = ce.CurrentScope;

            // Create a variable to hold the result of this test
            var resultBool3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            resultBool3.InitialValue = new ValSimple("false", typeof(bool));
            ce.Add(resultBool3);

            // Create and evaluate bool_1
            var binaryExpression = expr as BinaryExpression;
            DeclarableParameter resultBool1 = AssignExpreaaionToEvaluationIfNeededBool(ce, cc, container, binaryExpression.Left);

            // Now, see if we need to evalute the right hand operand.
            if (expr.NodeType == ExpressionType.AndAlso)
            {
                ce.Add(new Statements.StatementFilter(resultBool1));
            }
            else
            {
                var notYet = new ValSimple($"!{resultBool1.RawValue}", typeof(bool), new IDeclaredParameter[] { resultBool1 });
                ce.Add(new Statements.StatementFilter(notYet));
            }

            // Create and evaluate bool 1.
            var resultBool2 = AssignExpreaaionToEvaluationIfNeededBool(ce, cc, container, binaryExpression.Right, outterScope);
            ce.CurrentScope = outterScope;

            // Finally, evaluate bool3.
            var termEvaluation = expr.NodeType == ExpressionType.AndAlso
                ? $"{resultBool1.RawValue}&&{resultBool2.RawValue}"
                : $"{resultBool1.RawValue}||{resultBool2.RawValue}";
            ce.Add(new Statements.StatementAssign(resultBool3, new ValSimple(termEvaluation, typeof(bool), new[] { resultBool1, resultBool2 })));

            // Return the value we've now filled.
            return resultBool3;
        }

        /// <summary>
        /// Evaluate an expression. If the result is just DeclareableParameter, return that, otherwise make an assignment.
        /// </summary>
        /// <param name="ce"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <param name="exprToHandIn"></param>
        /// <returns></returns>
        private static DeclarableParameter AssignExpreaaionToEvaluationIfNeededBool(IGeneratedQueryCode ce, ICodeContext cc, CompositionContainer container, Expression exprToHandIn, IScopeInfo whereToDeclare = null)
        {
            IValue exprEvaluation = GetExpression(exprToHandIn, ce, cc, container);
            if (exprEvaluation is DeclarableParameter p)
            {
                // We can only return this if the variable is declared at the place we want it to be!
                var currentScope = ce.CurrentScope;
                try
                {
                    if (whereToDeclare != null)
                    {
                        ce.CurrentScope = whereToDeclare;
                    }
                    if (ce.CodeBody.AllDeclaredVariables.Where(dp => dp == p).Any())
                    {
                        return p;
                    }
                } finally
                {
                    ce.CurrentScope = currentScope;
                }
            }

            // Create and assign an expression.
            var result = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            result.InitialValue = new ValSimple("false", typeof(bool));

            if (whereToDeclare == null)
            {
                ce.Add(result);
            }
            else
            {
                var currentScope = ce.CurrentScope;
                ce.CurrentScope = whereToDeclare;
                ce.Add(result);
                ce.CurrentScope = currentScope;
            }
            ce.Add(new Statements.StatementAssign(result, exprEvaluation));
            return result;
        }

        /// <summary>
        /// Internal expression resolver. 
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="ce"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        /// <remarks>
        /// Cache and do cache lookup of expressions to try to short-circuit the expression resolution.
        /// </remarks>
        public static IValue InternalGetExpression(Expression expr, IGeneratedQueryCode ce, ICodeContext cc, CompositionContainer container)
        {
            // If we are looking at a null expression, then we resolve to a null value
            if (expr == null)
                return null;

            // If this is a known sub-expression, then we should return
            // the cached value.
            if (ce != null)
            {
                var v = ce.LookupSubexpression(expr);
                if (v != null)
                    return v;
            }

            // We are going to do the visit. Create the resolver we are going to use
            // and configure it.
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

            // Do the visit
            visitor.Visit(expr);

            // Cache the result.
            if (ce != null)
            {
                ce.RememberSubexpression(expr, visitor.Result);
            }

            return visitor.Result;
        }

        /// <summary>
        /// Local version of get expression that passes on all of our information. This is basically
        /// a syntactic shortcut.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private IValue GetExpression(Expression expr)
        {
            var r = InternalGetExpression(expr, _codeEnv, _codeContext, MEFContainer);
            return r;
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
        /// constructor - only called by our helper routine above.
        /// </summary>
        /// <param name="ce"></param>
        private ExpressionToCPP(IGeneratedQueryCode ce, ICodeContext cc)
        {
            _codeEnv = ce;
            _codeContext = cc;
        }

        /// <summary>
        /// Deal with an inline conditional expression (test ? ans1 : ans2). We can translate this directly to C++, fortunately!
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <remarks>
        /// We turn this into a real if statement, rather than a fake if statement. This is to try to keep any code
        /// associated with the side that won't be executed, not being executed.
        /// Note that Resolver has some good code already to special case handle a bool result.
        /// </remarks>
        protected override Expression VisitConditional(ConditionalExpression expression)
        {
            var testExpression = expression.Test;
            var trueExpression = expression.IfTrue;
            var falseExpression = expression.IfFalse;

            // Run the test.
            var testBoolInCode = AssignExpreaaionToEvaluationIfNeededBool(_codeEnv, _codeContext, MEFContainer, testExpression);
            //var testBoolInCode = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            //_codeEnv.Add(testBoolInCode);
            //_codeEnv.Add(new Statements.StatementAssign(testBoolInCode,
            //    GetExpression(testExpression, _codeEnv, _codeContext, MEFContainer)
            //    ));

            // Next, do the result cache.
            var resultInCode = DeclarableParameter.CreateDeclarableParameterExpression(expression.Type);
            _codeEnv.Add(resultInCode);
            _result = resultInCode;

            // Get the result if the test is true.
            var topScope = _codeEnv.CurrentScope;
            _codeEnv.Add(new Statements.StatementFilter(testBoolInCode));
            _codeEnv.Add(new Statements.StatementAssign(resultInCode, GetExpression(trueExpression, _codeEnv, _codeContext, MEFContainer)));
            _codeEnv.CurrentScope = topScope;

            _codeEnv.Add(new Statements.StatementFilter(GetExpression(Expression.Not(testBoolInCode), _codeEnv, _codeContext, MEFContainer)));
            _codeEnv.Add(new Statements.StatementAssign(resultInCode, GetExpression(falseExpression, _codeEnv, _codeContext, MEFContainer)));
            _codeEnv.CurrentScope = topScope;

            // Result is set. Continue to process other items in the tree.
            return expression;
        }

        /// <summary>
        /// List of the type handlers that we can use to process things.
        /// </summary>
        [Import]
        private TypeHandlerCache TypeHandlers { get; set; }

        /// <summary>
        /// Deal with a constant expression. Exactly how this is dealt with depends on the value. We process
        /// the most important ones directly, and the MEF-off the others. :-)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression expression)
        {
            if (expression.Type == typeof(int)
                || expression.Type == typeof(uint))
            {
                _result = new ValSimple(expression.Value.ToString(), expression.Type, null);
            }
            else if (expression.Type == typeof(float)
              || expression.Type == typeof(double))
            {
                // Make sure it is a legal expression. 
                if ((expression.Type == typeof(double) && (double.IsNaN((double)expression.Value) || double.IsInfinity((double)expression.Value)))
                    || (expression.Type == typeof(float) && (float.IsNaN((float)expression.Value) || float.IsInfinity((float)expression.Value)))) {
                    throw new ArgumentException($"Can't translate {expression.Value.ToString()} to C++. Illegal number.");
                }

                var s = expression.Value.ToString();
                if (!s.Contains("."))
                    s += ".0";
                _result = new ValSimple(s, expression.Type, null);
            }
            else if (expression.Type == typeof(bool))
            {
                if ((bool)expression.Value)
                {
                    _result = new ValSimple("true", typeof(bool), null);
                }
                else
                {
                    _result = new ValSimple("false", typeof(bool), null);
                }
            }
            else if (expression.Type == typeof(string))
            {
                _result = new ConstantString(expression.Value as string);
            }
            else if (expression.Value == null)
            {
                _result = new ValSimple("0", expression.Type, null);
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
        protected override Expression VisitBinary(BinaryExpression expression)
        {
            string op = "";
            bool CastToFinalType = false;
            string format = "{0}{1}{2}";
            bool treatAsPointers = false;
            bool protectRHS = false;

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
                    treatAsPointers = expression.Left.IsNull() || expression.Right.IsNull();
                    break;
                case ExpressionType.NotEqual:
                    op = "!=";
                    resultType = typeof(bool);
                    treatAsPointers = expression.Left.IsNull() || expression.Right.IsNull();
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
                    protectRHS = true;
                    break;

                // How we do array lookup depends on the array type we are looking up!
                case ExpressionType.ArrayIndex:
                    resultType = expression.Type;
                    if (ArrayAccessDoesNotSupportAt(expression))
                    {
                        op = "[]";
                        format = "{0}[{2}]";
                    }
                    else
                    {
                        op = "at";
                        format = "{0}.at({2})";
                    }
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

            if (string.IsNullOrWhiteSpace(op))
            {
                return base.VisitBinary(expression);
            }

            //
            // Comparing expressions anonymous expressions are not supported.
            //

            if (expression.Right.Type.Name.Contains("Anonymous"))
                throw new ArgumentException(string.Format("Binary operators on Anonymous types are not supported: '{0}' - '{1}'", expression.Right.Type.Name, expression.Right.ToString()));

            // Get the LHS and RHS processed
            var LHS = GetExpression(expression.Left);
            var RHS = GetExpression(expression.Right);

            // Now build the expression as a string
            string sRHS, sLHS;
            if (CastToFinalType)
            {
                sRHS = RHS.CastToType(expression, ignorePointer: treatAsPointers);
                sLHS = LHS.CastToType(expression, ignorePointer: treatAsPointers);
            }
            else
            {
                sRHS = RHS.CastToType(expression.Right, ignorePointer: treatAsPointers);
                sLHS = LHS.CastToType(expression.Left, ignorePointer: treatAsPointers);
            }

            StringBuilder bld = new StringBuilder();
            bld.AppendFormat(format, sLHS.ApplyParensIfNeeded(), op, sRHS.ApplyParensIfNeeded(protectRHS));
            _result = new ValSimple(bld.ToString(), resultType, RHS.Dependants.Concat(LHS.Dependants));

            return expression;
        }

        /// <summary>
        /// Determine if this expression (which is an array access) when translated supports the "at" operator,
        /// which is a little slower, but definately safer.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private bool ArrayAccessDoesNotSupportAt(BinaryExpression expression)
        {
            var arrInfo = expression.DetermineArrayIndexInfo();
            if (arrInfo.Item2.NodeType != ExpressionType.MemberAccess)
                return false;
            var me = arrInfo.Item2 as MemberExpression;

            //
            // If it has an array index attribute, then we should use the normal []
            //

            if (me.Member.TypeHasAttribute<ArraySizeIndexAttribute>() != null)
                return true;

            //
            // If this type is an array index attribute, and this is the first index, then 
            // we should also be using the [] access to get past the TClonesArray stuff (which
            // does not support .at).
            //

            if (arrInfo.Item1.Count == 1 && me.Expression.Type.TypeHasAttribute<TClonesArrayImpliedClassAttribute>() != null)
                return true;

            return false;
        }

        /// <summary>
        /// Deal with a unary expression (like Not, for example).
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Negate:
                    var e1 = GetExpression(expression.Operand);
                    _result = new ValSimple("-" + e1.CastToType(expression).ApplyParensIfNeeded(), expression.Type, e1.Dependants);
                    break;

                case ExpressionType.Not:
                    var e2 = GetExpression(expression.Operand);
                    _result = new ValSimple("!" + e2.CastToType(expression).ApplyParensIfNeeded(), expression.Type, e2.Dependants);
                    break;

                case ExpressionType.Convert:
                    var e3 = GetExpression(expression.Operand);
                    _result = new ValSimple(e3.CastToType(expression), expression.Type, e3.Dependants);
                    break;

                case ExpressionType.ArrayLength:
                    VisitArrayLength(expression);
                    break;

                default:
                    return base.VisitUnary(expression);
            }

            return expression;
        }

        /// <summary>
        /// The expression is trying to figure out how long this array is.
        /// 
        /// 1) if this is a member access and the member has an ArrayIndex attribute, then use
        ///    that to determine what size array we are dealing with.
        /// 2) All else fails, assume this is a vector<> and use that.
        /// </summary>
        /// <param name="expression"></param>
        private void VisitArrayLength(UnaryExpression expression)
        {
            //
            // This may be a multi-index array. In order to get the actual
            // length we need to look down as many levels as we can do do the lookup.
            //

            var arrInfo = expression.Operand.DetermineArrayIndexInfo();
            if (arrInfo.Item2.NodeType == ExpressionType.MemberAccess)
            {
                var ma = arrInfo.Item2 as MemberExpression;
                var attrs = ma.Member.TypeHasAttributes<ArraySizeIndexAttribute>();
                if (attrs != null && attrs.Length != 0)
                {
                    // Determine which index this is.
                    var attr = (from a in attrs
                                where a.Index == arrInfo.Item1.Count
                                select a).FirstOrDefault();
                    if (attr == null)
                        throw new InvalidOperationException(string.Format("Unable to find index info for index 0 for the expression {0}.", expression.ToString()));

                    if (attr.IsConstantExpression)
                    {
                        // The array size is constant every event - so we just use the raw number for the size.
                        var v = Int32.Parse(attr.LeafName);
                        _result = GetExpression(Expression.Constant(v));
                    }
                    else
                    {
                        // The array is indexed by another raw variable leaf (like "n").
                        var arraySize = Expression.Field(ma.Expression, attr.LeafName);
                        if (!arraySize.Type.IsNumberType())
                            throw new InvalidOperationException(string.Format("Array size leaf '{0}' is not a number ({1})", attr.LeafName, arraySize.Type.Name));

                        _result = GetExpression(arraySize);
                    }
                    return;
                }

                //
                // See if it is a TClonesArray generated object. If that is the case, then the length is on the parent type, with a GetEntries() call, as specified.
                // and only do the first as a get entries.
                //

                if (arrInfo.Item1.Count == 0)
                {
                    var cattrs = ma.Expression.Type.TypeHasAttribute<TClonesArrayImpliedClassAttribute>();
                    if (cattrs != null)
                    {
                        var e = GetExpression(ma.Expression);
                        _result = new ValSimple(string.Format("{0}.GetEntries()", e.AsObjectReference(ma.Expression)), typeof(int), e.Dependants);
                        return;
                    }
                }
            }

            //
            // If we fall through here, then we assume this is a vector<> array
            //

            var arrayBase = GetExpression(expression.Operand);
            _result = new ValSimple(arrayBase.AsObjectReference(expression.Operand) + ".size()", expression.Type, arrayBase.Dependants);
        }

        /// <summary>
        /// We are going to reference a member item - this is a simple "." coding. If this is a reference
        /// to some sort of array, then we need to deal with getting back the proper array type.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression expression)
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

            //
            // If this is a generic - there is pretty much only one thing we know how to deal with, IEnumerable.
            //

            if (expression.Type.IsGenericType)
            {
                if (expression.Type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    _result = new ValSimple(baseExpr.AsObjectReference(expression) + "." + leafName, expression.Type, baseExpr.Dependants);
                }
                else
                {
                    throw new NotImplementedException("Can't deal with a generic type for iterator for " + expression.Type.Name + ".");
                }
            }

            ///
            /// If we can't figure out what the proper special variable type is from above, then we
            /// need to just fill in the default.
            /// 

            if (_result == null)
            {
                _result = new ValSimple(baseExpr.AsObjectReference(expression.Expression) + "." + leafName, expression.Type, baseExpr.Dependants);
            }

            return expression;
        }

        /// <summary>
        /// Look at some parameter - since the name should be already defined, this is actually quite easy!
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression expression)
        {
            _result = new ValSimple(expression.Name, expression.Type, null);

            return expression;
        }

        /// <summary>
        /// Look at one of our extension expressions - see if we can deal with it whatever we are looking at here.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitExtension(Expression expression)
        {
            if (expression.NodeType == DeclarableParameter.ExpressionType)
            {
                var decl = expression as DeclarableParameter;
                _result = decl;
                return expression;
            } else if (expression.NodeType == ValueExpression.ExpressionType)
            {
                _result = (expression as ValueExpression).Value;
                return expression;
            }
            else
            {
                return base.VisitExtension(expression);
            }
        }

        /// <summary>
        /// We have to process a lambda function. Put it inline (i.e. we make the assumption that no statements
        /// are used here). That would have to be version 10.0 or something like that.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitLambda<T>(Expression<T> expression)
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
        protected override Expression VisitNew(NewExpression expression)
        {
            var exprOut = TypeHandlers.ProcessNew(expression, out _result, _codeEnv, MEFContainer);
            return exprOut;
        }

        /// <summary>
        /// Some method is being called. Offer plug-ins a chance to transform this method call.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            _result = TypeHandlers.CodeMethodCall(expression, _codeEnv, MEFContainer);

            // Cache this so that we don't have to re-call it later (if need be) if this is a simple type.

            if (_result.Type.IsNumberType() && !_result.IsSimpleTerm())
            {
                var cachedValue = DeclarableParameter.CreateDeclarableParameterExpression(_result.Type);
                _codeEnv.Add(cachedValue);
                var assign = new Statements.StatementAssign(cachedValue, _result);
                _codeEnv.Add(assign);
                _result = cachedValue;
            }

            // Always return the expression

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
            return itemAsExpression.ToString();
            //return itemAsExpression != null ? FormattingExpressionVisitor.Format(itemAsExpression) : unhandledItem.ToString();
        }

        /// <summary>
        /// Get/Set the MEF container used when we create new objects (like a QV).
        /// </summary>
        public CompositionContainer MEFContainer { get; set; }
    }
}
