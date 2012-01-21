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
        /// for the various needed things. Throws if it can't figure out how to do
        /// a loop. It might return null, in which case the array index context has
        /// just been "setup".
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>null, if no further setup is required to run the loop, and an IArrayInfo if further work is required.</returns>
        private static IArrayInfo GetIArrayInfo(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            ///
            /// Is it a simple array?
            /// 

            if (IsArrayType(expr))
                return new ArrayInfoVector(expr);

            //
            // Is it a sub-query expression? This will be null if this works out ok. If it doesn't work
            // out to be null, then it returned some sort of object, which we will now have to loop over.
            // 

            if (expr is SubQueryExpression)
            {
                var resolved = expr.Resolve(gc, cc, container);
                if (resolved == null)
                    return null;

                if (resolved is SubQueryExpression)
                    throw new InvalidOperationException(string.Format("Unable to translate '{0}' to something we can loop over!", expr.ToString()));

                return GetIArrayInfo(resolved, gc, cc, container);
            }

            //
            // If this is an array over an object that LINQ has created (like an anonymous type), then
            // we should try to unroll the access to that array.
            //

            var translated = AttemptTranslationToArray(expr, cc);
            if (translated != null)
            {
                return GetIArrayInfo(translated, gc, cc, container);
            }

            //
            // We have no idea how to deal with this!
            // 

            throw new ArgumentException("Type '" + expr.Type.Name + "' ('" + expr.ToString() + "') is not an array we know how to deal with");
        }

        /// <summary>
        /// Some common code to evaluate a sub-query. The sub query is just a loop - so we buidl that context
        /// and leave us in the middle of the loop for re-linq to add later code. Since we setup the context, we
        /// always return null (there is no further context that needs to be done).
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        private static void LoopOverSubQuery(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            // The sub-query expression will just run. We need to parse the result and see what happens. We reutnr
            // null in the end because it is the "context" that is getting setup.

            var val = ExpressionToCPP.GetExpression(expr, gc, cc, container);
            if (val != null)
                throw new ArgumentException("What looked like an array (type '" + expr.Type.Name + "') seems to have returned a value: '" + val.RawValue + "'");
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
            var r = TranslatingExpressionVisitor.Translate(preplacements, cc.CacheCookies, e => e);
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
