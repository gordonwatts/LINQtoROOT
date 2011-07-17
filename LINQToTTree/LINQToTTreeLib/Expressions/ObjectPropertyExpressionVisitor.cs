using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Parsing;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Sometimes you'll find things like "new Tuple<int, int>(5, 10).Item1" in our code.
    /// The job of this translator is to replace the above with "5". Called by
    /// the Translating Expression Visitor to automatically deal with this sort of thing.
    /// This also works with the temp objects that LINQ will generate with the anonymous
    /// new statements.
    /// 
    /// Note this simply threads through an expression - so using these sorts of objects is
    /// not a way to optimize how things are calculated for the user!
    /// </summary>
    public class ObjectPropertyExpressionVisitor : ExpressionTreeVisitor
    {
        /// <summary>
        /// Property access. Look to see if the parent type is one of these type of things.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override System.Linq.Expressions.Expression VisitMemberExpression(System.Linq.Expressions.MemberExpression expression)
        {
            //
            // If this is a property referencing an object, perhaps we can decode a short-circut of what was actually
            // meant.
            //

            var exprType = expression.Expression.Type;
            if (expression.Expression.NodeType == ExpressionType.New)
            {
                // Is it a Tule?
                if (exprType.Name.StartsWith("Tuple`"))
                {
                    int itemIndex = Convert.ToInt32(expression.Member.Name.Substring(4));
                    var newExpr = expression.Expression as NewExpression;
                    return newExpr.Arguments[itemIndex - 1];
                }
                if (exprType.Name.StartsWith("<>f__AnonymousType"))
                {
                    return TranslateAnonymousPropertyReference(expression);
                }

            }

            return base.VisitMemberExpression(expression);
        }

        /// <summary>
        /// LINQ has generated an anonymous type property reference. Our job is to see if we can figure out
        /// what expression it actually is.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression TranslateAnonymousPropertyReference(MemberExpression expression)
        {
            //
            // Anonymous type. So we need to loop through the members and find the name of
            // the guy that we want to access (this is the only way according to the
            // NewExpression.Member documentation).
            //

            var newExpr = expression.Expression as NewExpression;
            var propName = expression.Member.Name;

            var memIndex = (from index in Enumerable.Range(0, newExpr.Members.Count)
                            where newExpr.Members[index].Name == propName
                            select index).First();

            //
            // That index tells us the argument order, and which argument to grab
            //

            return newExpr.Arguments[memIndex];
        }
    }
}
