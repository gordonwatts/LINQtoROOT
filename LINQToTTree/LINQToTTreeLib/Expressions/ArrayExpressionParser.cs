using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
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
        /// Keep a list of array factories that we can pulse to figure out how
        /// to generate a sequence out of the given expression.
        /// </summary>
#pragma warning disable 649
        [ImportMany]
        IEnumerable<IArrayInfoFactory> _handlers;
#pragma warning restore 649

        /// <summary>
        /// Keep track of the singleton we use to do the parsing.
        /// </summary>
        private static ArrayExpressionParser _parser;

        /// <summary>
        /// Used for testing - allows one to reset everything.
        /// </summary>
        internal static void ResetParser()
        {
            _parser = null;
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
            if (_parser == null)
            {
                _parser = new ArrayExpressionParser();
                container.SatisfyImportsOnce(_parser);
            }

            var result = _parser.GetIArrayInfo(expr, gc, cc, container);

            if (result == null)
                throw new InvalidOperationException(string.Format("Unable to turn expression '{0}' (type: '{1}') into a sequence we can deal with in C++", expr.ToString(), expr.Type.Name));

            //
            // Turn it into code - any code that we need.
            //

            result.CodeLoopOverArrayInfo(query, gc, cc, container);
        }

        /// <summary>
        /// Given an array expression return an array info that cna be used
        /// for the various needed things. Throws if it can't figure out how to do
        /// a loop. It might return null, in which case the array index context has
        /// just been "setup".
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>null, if no further setup is required to run the loop, and an IArrayInfo if further work is required.</returns>
        private IArrayInfo GetIArrayInfo(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            //
            // See if we can find something that will handle the array.
            //

            var arInfo = (from h in _handlers
                          let r = h.GetIArrayInfo(expr, gc, cc, container, e => GetIArrayInfo(e, gc, cc, container))
                          where r != null
                          select r).FirstOrDefault();

            return arInfo;
        }
    }

    /// <summary>
    /// If this is an array type (like an anonymous type) that isn't an array yet, but might be translated to one,
    /// then we should be looping over that.
    /// </summary>
    [Export(typeof(IArrayInfoFactory))]
    internal class TranslatedArrayFactory : IArrayInfoFactory
    {
        public IArrayInfo GetIArrayInfo(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container, Func<Expression, IArrayInfo> ReGetIArrayInfo)
        {
            if (expr is SubQueryExpression)
                return null;

            var translated = AttemptTranslationToArray(expr, cc);
            if (translated != null)
            {
                return ReGetIArrayInfo(translated);
            }

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
            var r = TranslatingExpressionVisitor.Translate(preplacements, cc.CacheCookies, e => e);
            return r as SubQueryExpression;
        }
    }


    /// <summary>
    /// If this is an array type - a simple one (like int[]).
    /// </summary>
    [Export(typeof(IArrayInfoFactory))]
    internal class ArrayTypeFactory : IArrayInfoFactory
    {
        public IArrayInfo GetIArrayInfo(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container, Func<Expression, IArrayInfo> ReGetIArrayInfo)
        {
            if (IsArrayType(expr))
                return new ArrayInfoVector(expr);
            return null;
        }

        /// <summary>
        /// Pretty simple array test
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

    /// <summary>
    /// A sub-query expression is actually pretty unique. The act of doing the "resolve" on this
    /// will cause the loop to be generated when the sub-query expression gets called by a recursive
    /// QueryVisitor invokation.
    /// </summary>
    [Export(typeof(IArrayInfoFactory))]
    internal class SubQueryArrayTypeFactory : IArrayInfoFactory
    {
        public IArrayInfo GetIArrayInfo(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container, Func<Expression, IArrayInfo> ReGetIArrayInfo)
        {
            if (!(expr is SubQueryExpression))
                return null;

            var resolved = expr.Resolve(gc, cc, container);
            if (resolved == null)
                return new DummyArrayInfo();

            if (resolved is SubQueryExpression)
                throw new InvalidOperationException(string.Format("Unable to translate '{0}' to something we can loop over!", expr.ToString()));

            return ReGetIArrayInfo(resolved);
        }
    }

    /// <summary>
    /// Return an array info that does nothing... this is a place holder in the case that the
    /// code has already been generated.
    /// </summary>
    class DummyArrayInfo : IArrayInfo
    {
        public Tuple<Expression, Expression> AddLoop(IGeneratedQueryCode env, ICodeContext context, CompositionContainer container)
        {
            return null;
        }
    }

}
