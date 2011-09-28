using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Resolve sub-query expressoins. Needs to be done early.
    /// </summary>
    static class SubQueryExpressionResolver
    {
        /// <summary>
        /// Return the expression with sub-query expressions dealt with.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Expression ResolveSubQueries(this Expression source, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            var resolver = new Visitor() { MEFContainer = container, GeneratedCode = gc, CodeContext = cc };
            return resolver.VisitExpression(source);
        }

        /// <summary>
        /// Class that does the work of actually visiting
        /// </summary>
        private class Visitor : ExpressionTreeVisitor
        {
            /// <summary>
            /// Run a sub-query for the user. The sub-query is a full-blown expression that
            /// must usually run in its own loop (or similar).
            /// </summary>
            /// <remarks>
            /// The query visitor must be re-invoked - this ends up being a full-blown
            /// parsing.
            /// </remarks>
            /// <param name="expression"></param>
            /// <returns></returns>
            protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
            {
                if (MEFContainer == null)
                    throw new InvalidOperationException("MEFContainer can't be null if we need to analyze a sub query!");

                QueryVisitor qv = new QueryVisitor(GeneratedCode, CodeContext, MEFContainer);
                qv.SubExpressionParse = true;
                MEFContainer.SatisfyImportsOnce(qv);

                ///
                /// Run it - since this result is out of this loop, we pop-back-out when done.
                /// 

                var scope = GeneratedCode.CurrentScope;
                qv.VisitQueryModel(expression.QueryModel);

                ///
                /// Two possible results from the sub-expression query, and how we proceed depends
                /// on what happened in the sub query
                /// 
                /// 1. <returns a value> - an operator like Count() comes back from the sequence.
                ///    it will get used in some later sequence (like # of jets in each event). So,
                ///    we need to make sure it is declared and kept before it is used. The # that comes
                ///    back needs to be used outside the scope we are sitting in - the one that we were at
                ///    when we started this. Since this is a sub-query expression, the result isn't the final
                ///    result, so we need to reset it so no one notices it.
                /// 2. <return a sequence> - this is weird - What we are actually doing here is putting the
                ///    sequence into code. So the loop variable has been updated with the new sequence iterator
                ///    value. But there isn't really a result! So the result will be null...
                /// 

                var r = GeneratedCode.ResultValue;
                if (r != null)
                {
                    GeneratedCode.CurrentScope = scope;
                    if (r is IDeclaredParameter)
                    {
                        GeneratedCode.Add(r as IDeclaredParameter);
                    }
                    GeneratedCode.ResetResult();
                    return r;
                }

                //
                // The fact that we returned null means we are dealing with a
                // sequence. There really is no translated version of this expression
                // in that case - so we will return null. If someone above is depending
                // on doing something with it they are going to run into some
                // trouble!

                return null;
            }

            /// <summary>
            /// The MEF container building objects.
            /// </summary>
            public CompositionContainer MEFContainer { get; set; }

            public IGeneratedQueryCode GeneratedCode { get; set; }

            public ICodeContext CodeContext { get; set; }
        }
    }
}
