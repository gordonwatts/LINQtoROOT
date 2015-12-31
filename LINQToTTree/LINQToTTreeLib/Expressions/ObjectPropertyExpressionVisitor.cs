using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Parsing;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Sometimes you'll find things like "new Tuple<int, int>(5, 10).Item1" or
    /// "new CustomObject(){Val1 = 5}.Val1" in our code.
    /// The job of this translator is to replace the above with "5". Called by
    /// the Translating Expression Visitor to automatically deal with this sort of thing.
    /// This also works with the temp objects that LINQ will generate with the anonymous
    /// new statements.
    /// 
    /// Note this simply threads through an expression - so using these sorts of objects is
    /// not a way to optimize how things are calculated for the user!
    /// </summary>
    public static class ObjectPropertyExpressionVisitor
    {
        /// <summary>
        /// Implementation object, created only once.
        /// </summary>
        private static Lazy<ObjectPropertyExpressionVisitorImpl> _impl = new Lazy<ObjectPropertyExpressionVisitorImpl>(() => new ObjectPropertyExpressionVisitorImpl());

        /// <summary>
        /// Remove tuple and expression implementations
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Expression RemoveObjectAndTupleReferences (Expression expression)
        {
            _impl.Value.DidRemove = true;
            var e = expression;
            while (_impl.Value.DidRemove)
            {
                _impl.Value.DidRemove = false;
                e = _impl.Value.VisitExpression(e);
            }
            return e;
        }

        /// <summary>
        /// Implementation of the visitor pattern
        /// </summary>
        class ObjectPropertyExpressionVisitorImpl : ExpressionTreeVisitor
        {
            /// <summary>
            /// Set to true if we have removed a tuple or custom object.
            /// </summary>
            public bool DidRemove { get; set; }

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
                    // Is it a Tuple?
                    if (exprType.Name.StartsWith("Tuple`"))
                    {
                        int itemIndex = Convert.ToInt32(expression.Member.Name.Substring(4));
                        var newExpr = expression.Expression as NewExpression;
                        DidRemove = true;
                        return VisitExpression(newExpr.Arguments[itemIndex - 1]);
                    }
                    if (exprType.Name.StartsWith("<>f__AnonymousType"))
                    {
                        DidRemove = true;
                        return TranslateAnonymousPropertyReference(expression);
                    }
                }

                //
                // If a user declared object is created and referenced (and the members are inited)
                // then we can use them as something to do translation in.
                //

                if (expression.Expression.NodeType == ExpressionType.MemberInit)
                {
                    var result = TranslateUserObjectMemberInitReference(expression);
                    if (result != null)
                    {
                        DidRemove = true;
                        return result;
                    }
                }

                //
                // Fall through and perform default actions (like looking deeper) if we got
                // no where!
                //

                return base.VisitMemberExpression(expression);
            }

            /// <summary>
            /// The user has something like new CustomObject(){Val1 = 5}.Val1 in their code. This
            /// is just expression carry-through - similar to the tuple and anonymous objects.
            /// </summary>
            /// <param name="expression"></param>
            /// <returns>The translated expression if the lookup succeeded, or null if not</returns>
            /// <remarks>We support only member assignment!</remarks>
            private Expression TranslateUserObjectMemberInitReference(MemberExpression expression)
            {
                var memberInit = expression.Expression as MemberInitExpression;
                var propName = expression.Member.Name;

                var matchingInits = (from mi in memberInit.Bindings
                                     where mi.Member.Name == propName && mi.BindingType == MemberBindingType.Assignment
                                     select mi).ToArray();
                if (matchingInits.Length == 0)
                    return null;

                if (matchingInits.Length != 1)
                    throw new InvalidOperationException(string.Format("Type '{0}' seems to have more than one member named '{1}'!", memberInit.Type.Name, propName));

                var binding = matchingInits[0] as MemberAssignment;
                return VisitExpression(binding.Expression);
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

                return VisitExpression(newExpr.Arguments[memIndex]);
            }
        }
    }
}
