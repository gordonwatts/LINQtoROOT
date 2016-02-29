using System;
using System.Linq.Expressions;
using Remotion.Linq.Parsing;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Replace a sub-expression in an expression.
    /// </summary>
    static class SubExpressionReplacement
    {
        /// <summary>
        /// Replace all instances of "pattern" in source with replacement.
        /// </summary>
        /// <remarks>No error if no replacement is done.
        /// 
        /// The expression pattern must be an exact match - not just shape, but also names of parameters, etc.
        /// The replacement is currently based on the text value.</remarks>
        /// <param name="source"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        public static Expression ReplaceSubExpression(this Expression source, Expression pattern, Expression replacement)
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern cannot be null");
            if (replacement == null)
                throw new ArgumentNullException("Replacement expression can't be null");

            var t = new ReplaceDriver(pattern, replacement);
            return t.Visit(source);
        }

        /// <summary>
        /// Internal class that will do the real work.
        /// </summary>
        class ReplaceDriver : RelinqExpressionVisitor
        {
            private Expression _pattern;
            private Expression _replacement;
            private string _patternString;

            public ReplaceDriver(Expression pattern, Expression replacement)
            {
                // TODO: Complete member initialization
                _pattern = pattern;
                _replacement = replacement;

                _patternString = pattern.ToString();
            }

            /// <summary>
            /// See if the replacement is ready to be done here!
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public override Expression Visit(Expression expression)
            {
                if (expression != null)
                    if (expression.ToString() == _patternString)
                        return _replacement;

                return base.Visit(expression);
            }
        }

    }
}
