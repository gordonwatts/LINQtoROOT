﻿using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using Remotion.Data.Linq.Clauses.Expressions;

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
        public static IArrayInfo ParseArrayExpression(Expression expr, IGeneratedCode gc, ICodeContext cc, CompositionContainer container)
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

                ExpressionVisitor.GetExpression(expr, gc, cc, container);
                return null;
            }

            ///
            /// We have no idea how to deal with this!
            /// 

            throw new ArgumentException("Type '" + expr.Type.Name + "' ('" + expr.ToString() + "') is not an array we know how to deal with");
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
