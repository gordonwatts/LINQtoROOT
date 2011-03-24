using System;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Parse an array expression
    /// </summary>
    internal class ArrayExpressionParser
    {
        /// <summary>
        /// Given an array expression return an array info that cna be used
        /// for the various needed things. Throws or returns a good array reference object!
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static IArrayInfo ParseArrayExpression(Expression expr)
        {
            if (!IsArrayType(expr))
                throw new ArgumentException("Type '" + expr.Type.Name + "' is not an array we know how to deal with");

            return new ArrayInfoVector(expr);
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
