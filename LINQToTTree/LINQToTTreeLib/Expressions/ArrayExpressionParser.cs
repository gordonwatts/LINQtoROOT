using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Parse an array expression. Think of this as an extension of ExpressionVisitor, but meant
    /// for a specific purpose (i.e. we have pulled it out to keep the code in one place).
    /// </summary>
    internal class ArrayExpressionParser
    {
        /// <summary>
        /// Given an array expression return an array info that cna be used
        /// for the various needed things. Throws or returns a good array reference object!
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private static IArrayInfo GetIArrayInfo(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            ///
            /// Is it a simple array?
            /// 

            if (IsArrayType(expr))
                return new ArrayInfoVector(expr);

            ///
            /// Is it a sub-query expression?
            /// 

            if (expr is SubQueryExpression)
            {
                return LoopOverSubQuery(expr, gc, cc, container);
            }

            //
            // If this is an array over an object that LINQ has created (like an anonymous type), then
            // we should try to unroll the access to that array.
            //

            var translated = AttemptTranslationToArray(expr, cc);
            if (translated != null)
                return LoopOverSubQuery(translated, gc, cc, container);

            ///
            /// We have no idea how to deal with this!
            /// 

            throw new ArgumentException("Type '" + expr.Type.Name + "' ('" + expr.ToString() + "') is not an array we know how to deal with");
        }

        private static IArrayInfo LoopOverSubQuery(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            /// The sub-query expression will just run. We need to parse the result and see what happens. We reutnr
            /// null in the end because it is the "context" that is getting setup.

            var val = ExpressionToCPP.GetExpression(expr, gc, cc, container);
            if (val != null)
                throw new ArgumentException("What looked like an array (type '" + expr.Type.Name + "') seems to have returned a value: '" + val.RawValue + "'");
            return null;
        }

        /// <summary>
        /// See if this sequence type is actually a normal array or sub query expression hidden by an anonymous array index or similar.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private static Expression AttemptTranslationToArray(Expression expr, ICodeContext cc)
        {
            List<string> cookies = new List<string>();
            var preplacements = ParameterReplacementExpressionVisitor.ReplaceParameters(expr, cc);
            var r = TranslatingExpressionVisitor.Translate(preplacements, cc.CacheCookies, e=>e);
            return r as SubQueryExpression;
        }

        /// <summary>
        /// Parse an array expression, and turn it into a loop. Use indexName as the loop variable. Bomb if we can't do it. If you hand in null we will make up our own.
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        public static void ParseArrayExpression(IQuerySource query, Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            var result = GetIArrayInfo(expr, gc, cc, container);

            if (result == null)
                return;

            result.CodeLoopOverArrayInfo(query, gc, cc, container);
        }

        /// <summary>
        /// Check to see if this guy is an array that we know how to handle.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private static bool IsArrayType(Expression expr)
        {
            if (expr.Type.IsArray)
                return true;
            return false;
        }
    }
}
