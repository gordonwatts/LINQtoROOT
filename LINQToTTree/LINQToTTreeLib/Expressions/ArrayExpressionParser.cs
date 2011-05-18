using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
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
        private static IArrayInfo GetIArrayInfo(Expression expr, IGeneratedCode gc, ICodeContext cc, CompositionContainer container)
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
                /// The sub-query expression will just run. We need to parse the result and see what happens. We reutnr
                /// null in the end because it is the "context" that is getting setup.

                var val = ExpressionToCPP.GetExpression(expr, gc, cc, container);
                if (val != null)
                    throw new ArgumentException("What looked like an array (type '" + expr.Type.Name + "') seems to have returned a value: '" + val.RawValue + "'");
                return null;
            }

            ///
            /// We have no idea how to deal with this!
            /// 

            throw new ArgumentException("Type '" + expr.Type.Name + "' ('" + expr.ToString() + "') is not an array we know how to deal with");
        }

        /// <summary>
        /// Given an array expression, turn it into a loop. 
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        public static void ParseArrayExpression(Expression expr, IGeneratedCode gc, ICodeContext cc, CompositionContainer container)
        {
            ParseArrayExpression(null, expr, gc, cc, container);
        }

        /// <summary>
        /// Parse an array expression, and turn it into a loop. Use indexName as the loop variable. Bomb if we can't do it. If you hand in null we will make up our own.
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        public static void ParseArrayExpression(string indexName, Expression expr, IGeneratedCode gc, ICodeContext cc, CompositionContainer container)
        {
            var result = GetIArrayInfo(expr, gc, cc, container);
            if (result == null && !string.IsNullOrEmpty(indexName))
                throw new ArgumentException("When parsing array expression it's iterator was preset, however caller asked us to reset it");

            if (result == null)
                return;

            if (indexName == null)
                indexName = typeof(int).CreateUniqueVariableName();
            result.CodeLoopOverArrayInfo(indexName, gc, cc, container);
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
