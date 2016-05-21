
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Variables;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace LINQToTTreeLib.Utils
{
    internal static class ExpressionUtilities
    {
        /// <summary>
        /// Given array info, code a loop over it.
        /// </summary>
        /// <param name="query">The query this loop is associated with</param>
        /// <param name="arrayRef">The reference to the array</param>
        /// <remarks>Will add the query to the code context to forward to the variable that is being dealt with here.</remarks>
        public static IVariableScopeHolder CodeLoopOverArrayInfo(this IArrayInfo arrayRef, IQuerySource query, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            var indexVar = arrayRef.AddLoop(gc, cc, container);
            if (indexVar == null)
                return null;

            ///
            /// Next, make sure the index variable can be used for later references!
            /// 

            var result = cc.Add(query, indexVar.Item1);
            cc.SetLoopVariable(indexVar.Item1, indexVar.Item2);
            return result;
        }

        /// <summary>
        /// Search for a single variable name, that fills the whole string.
        /// </summary>
        static Regex gVarNameFinder = new Regex(@"^\b\w+\b$");

        static Regex gNumberFinder = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");

        /// <summary>
        /// Given a value, see if it is not a single term. If not, add parens.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ApplyParensIfNeeded(this IValue val)
        {
            if (val == null)
                throw new ArgumentNullException("Value must not be null");
            return val.RawValue.ApplyParensIfNeeded();
        }

        /// <summary>
        /// Look at a string and decide if it contains something that looks like one or more terms.
        /// </summary>
        /// <param name="rv"></param>
        /// <returns></returns>
        public static string ApplyParensIfNeeded(this string rv, bool protectNegativeNumbers = false)
        {
            if (string.IsNullOrWhiteSpace(rv))
                throw new ArgumentNullException("Value must not be null");

            // If it is a single variable then we don't need to protect it
            // when used in an expression.

            var match = gVarNameFinder.Match(rv);
            if (match.Success)
                return rv;

            // If it is just a number or similar, then match that
            if (gNumberFinder.Match(rv).Success)
            {
                if (!protectNegativeNumbers || !rv.StartsWith("-"))
                    return rv;
            }

            // Special case where we already have this thing surrounded by parens.
            if (rv[0] == '(')
            {
                int depth = 1;
                int indexer = 1;
                while (depth != 0 && indexer != rv.Length)
                {
                    if (rv[indexer] == '(')
                        depth++;
                    if (rv[indexer] == ')')
                        depth--;
                    indexer++;
                }
                if (indexer == rv.Length)
                    return rv;
            }

            // Protect it from "later" use.

            return "(" + rv + ")";
        }

        /// <summary>
        /// Is this something like:
        ///   obj.jets where jets is an array that points off to a "regrouped" variable? If so, then we
        ///   don't want to do the translation here.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsRootObjectArrayReference(this MemberExpression expression)
        {
            if (!expression.Type.IsArray)
                return false;

            if (expression.Expression.Type.TypeHasAttribute<TranslateToClassAttribute>() == null)
                return false;

            if (TypeUtils.TypeHasAttribute<TTreeVariableGroupingAttribute>(expression.Member) == null)
                return false;

            return true;
        }

        /// <summary>
        /// If this is a parameter of some sort, returns the name. Otherwise, throws.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string ParameterName(this Expression expr)
        {
            var lv = expr as ParameterExpression;
            if (lv != null)
                return lv.Name;

            var dv = expr as DeclarableParameter;
            if (dv == null)
                throw new InvalidOperationException("Unable to look at loop index variable that isn't a parameter");
            return dv.ParameterName;
        }

        /// <summary>
        /// We need to take a look at this item to see if it is an array access. If so,
        /// we want to find out all we can about it.
        /// </summary>
        /// <param name="expression">The expression that does the array access. Flunk out if it doesn't</param>
        /// <returns>A tuple with a list of the expressions to do the lookup and what we are doing the lookup against</returns>
        public static Tuple<List<Expression>, Expression> DetermineArrayIndexInfo(this Expression expression)
        {
            if (expression.NodeType != ExpressionType.ArrayIndex)
            {
                // We have reached teh bottom of the pile!
                return Tuple.Create(new List<Expression>(), expression);
            }

            var br = expression as BinaryExpression;
            var levelDown = DetermineArrayIndexInfo(br.Left);
            levelDown.Item1.Add(br.Right);
            return levelDown;
        }

        /// <summary>
        /// Remove all array references till we get to the root member.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression RemoveArrayReferences(this Expression expr)
        {
            return expr.DetermineArrayIndexInfo().Item2;
        }

        /// <summary>
        /// Test for an expression that is nul.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static bool IsNull(this Expression expr)
        {
            if (expr == null)
                throw new ArgumentNullException("expr");

            var c = expr as ConstantExpression;
            if (c == null)
                return false;

            if (c.Value == null)
                return true;

            return false;
        }

        /// <summary>
        /// See if this class is a leaf class - that is a class that
        /// is on the end of everything - like a TLorentzVector. NOT,
        /// for example, one of the classes we are using do groupings.
        /// CStyle TClonesArrays are also ok to be leaves.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static bool IsLeafType(this Expression expr)
        {
            // If this isn't a class at all, then this is a leaf type.
            if (!expr.Type.IsClass)
                return true;

            // Now, is this a leaf class or not? For example, a
            // ROOT class would be something like this.

            if (expr.Type.IsROOTClass())
                return true;

            //
            // If this is a grouping class, then we are definately not a leaf
            // class.
            //

            if (expr.NodeType == ExpressionType.MemberAccess)
            {
                var ma = expr as MemberExpression;
                if (ma.Member.TypeHasAttribute<TTreeVariableGroupingAttribute>() != null)
                    return false;
                if (ma.Member.TypeHasAttribute<IndexToOtherObjectArrayAttribute>() != null)
                    return false;

                return true;
            }

            //
            // Soem class, and we don't know about it, which means ROOT Must, so we should
            // be ok with it (I hope).
            //

            return false;
        }

        /// <summary>
        /// Return a parameter as an expression.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Expression AsExpression(this IDeclaredParameter param)
        {
            // Some of the expressions are already parameters, so...
            var e = param as Expression;
            if (e != null)
                return e;

            return Expression.Parameter(param.Type, param.RawValue);
        }

    }
}
