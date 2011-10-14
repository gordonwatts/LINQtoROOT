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
            /// The MEF container building objects.
            /// </summary>
            public CompositionContainer MEFContainer { get; set; }

            public IGeneratedQueryCode GeneratedCode { get; set; }

            public ICodeContext CodeContext { get; set; }
        }
    }
}
