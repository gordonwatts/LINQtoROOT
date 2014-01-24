using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.QueryVisitors;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;

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
        /// <param name="query">The query that this loop is associated with.</param>
        /// <param name="expr">The expression that evaluates to an array.</param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        public static IVariableScopeHolder ParseArrayExpression(IQuerySource query, Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
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

            return result.CodeLoopOverArrayInfo(query, gc, cc, container);
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
            expr = NormalizeExpression(expr);

            //
            // See if we can find something that will handle the array.
            //

            var arInfo = (from h in _handlers
                          let r = h.GetIArrayInfo(expr, gc, cc, container, e => GetIArrayInfo(e, gc, cc, container))
                          where r != null
                          select r).FirstOrDefault();

            return arInfo;
        }

        /// <summary>
        /// Look at the expression and see if we can strip off some extra things around the edges which shouldn't really affect things.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        /// <remarks>Currently only strip away convert statements</remarks>
        private static Expression NormalizeExpression(Expression expr)
        {
            if (expr.NodeType != ExpressionType.Convert)
                return expr;

            var u = expr as UnaryExpression;
            return u.Operand;
        }
    }

    /// <summary>
    /// If we are looking at a reference to a query expression, then look up the reference and try to process that.
    /// </summary>
    [Export(typeof(IArrayInfoFactory))]
    internal class SubQueryExpressionArrayInfoFactory : IArrayInfoFactory
    {
        public IArrayInfo GetIArrayInfo(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container, Func<Expression, IArrayInfo> ReGetIArrayInfo)
        {
            if (!(expr is QuerySourceReferenceExpression))
                return null;

            var preplacements = ParameterReplacementExpressionVisitor.ReplaceParameters(expr, cc);
            var r = TranslatingExpressionVisitor.Translate(preplacements, cc.CacheCookies, e => e);

            //
            // If we don't know what we are doing here, bail!
            //

            if (r == expr)
                return null;

            return ReGetIArrayInfo(r);
        }
    }

    /// <summary>
    /// If this is an array type (like an anonymous type) that isn't an array yet, but might be translated to one,
    /// then we should be looping over that.
    /// </summary>
    [Export(typeof(IArrayInfoFactory))]
    internal class TranslatedArrayInfoFactory : IArrayInfoFactory
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
            var preplacements = ParameterReplacementExpressionVisitor.ReplaceParameters(expr, cc);
            var r = TranslatingExpressionVisitor.Translate(preplacements, cc.CacheCookies, e => e);
            return r as SubQueryExpression;
        }
    }


    /// <summary>
    /// If this is an array type - a simple one (like int[]).
    /// </summary>
    [Export(typeof(IArrayInfoFactory))]
    internal class ArrayArrayInfoFactory : IArrayInfoFactory
    {
        public IArrayInfo GetIArrayInfo(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container, Func<Expression, IArrayInfo> ReGetIArrayInfo)
        {
            if (IsArrayType(expr) && expr.NodeType != ExpressionType.Constant)
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
    /// The person is trying to loop over an Enumerable.Range expression. Bring it.
    /// </summary>
    [Export(typeof(IArrayInfoFactory))]
    internal class EnumerableRangeArrayTypeFactory : IArrayInfoFactory
    {
        /// <summary>
        /// Return the array info for an eumerable range.
        /// </summary>
        class EnumerableRangeArrayInfo : IArrayInfo
        {
            /// <summary>
            /// The min value that the loop starts at
            /// </summary>
            private IValue _minValue;

            /// <summary>
            /// The max value the loop starts at.
            /// </summary>
            private IValue _maxValue;

            /// <summary>
            /// Create a array loop maker.
            /// </summary>
            /// <param name="minValue"></param>
            /// <param name="maxValue"></param>
            public EnumerableRangeArrayInfo(IValue minValue, IValue maxValue)
            {
                // TODO: Complete member initialization
                this._minValue = minValue;
                this._maxValue = maxValue;
            }

            /// <summary>
            /// Actually add the loop to the code and return everything!
            /// </summary>
            /// <param name="env"></param>
            /// <param name="context"></param>
            /// <param name="container"></param>
            /// <returns></returns>
            public Tuple<Expression, IDeclaredParameter> AddLoop(IGeneratedQueryCode env, ICodeContext context, CompositionContainer container)
            {
                // Create the index variable!
                var loopVariable = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
                var floop = new Statements.StatementForLoop(loopVariable, _maxValue, _minValue);
                env.Add(floop);

                return Tuple.Create(loopVariable as Expression, loopVariable as IDeclaredParameter);
            }
        }

        /// <summary>
        /// Do a type check, and then create the range info... which is dirt simple, of course!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <param name="ReGetIArrayInfo"></param>
        /// <returns></returns>
        public IArrayInfo GetIArrayInfo(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container, Func<Expression, IArrayInfo> ReGetIArrayInfo)
        {
            if (expr.NodeType == ExpressionType.Constant)
                return ProcessPossibleConstEnumerableRange(expr, gc, cc, container, ReGetIArrayInfo);

            if (expr.NodeType == EnumerableRangeExpression.ExpressionType)
                return ProcessEnumerableRangeExpression(expr, gc, cc, container, ReGetIArrayInfo);

            return null;
        }

        /// <summary>
        /// We have an enumerable range expression. Process it! :-)
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <param name="ReGetIArrayInfo"></param>
        /// <returns></returns>
        private IArrayInfo ProcessEnumerableRangeExpression(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container, Func<Expression, IArrayInfo> ReGetIArrayInfo)
        {
            var er = expr as EnumerableRangeExpression;

            var minVal = ExpressionToCPP.GetExpression(er.LowBoundary, gc, cc, container);
            var maxVal = ExpressionToCPP.GetExpression(er.HighBoundary, gc, cc, container);

            return new EnumerableRangeArrayInfo(minVal, maxVal);
        }

        /// <summary>
        /// A constant expression may be a IEnumerable type as a constant (in short, the user
        /// already has a pattern implemented). We support only continuous patterns right now,
        /// so look at it, and extract it.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <param name="ReGetIArrayInfo"></param>
        /// <returns></returns>
        private IArrayInfo ProcessPossibleConstEnumerableRange(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container, Func<Expression, IArrayInfo> ReGetIArrayInfo)
        {
            if (expr.NodeType != ExpressionType.Constant)
                return null;
            var ri = (expr as ConstantExpression).Value as IEnumerable<int>;
            if (ri == null)
                return null;

            // We can't actually tell what the heck this thing is, so we are going to have to iterate over it, I'm afraid...
            var e = ri.GetEnumerator();
            int minValue = 0;
            int maxValue = 0;
            if (e.MoveNext())
            {
                minValue = e.Current;
                maxValue = minValue;
                while (e.MoveNext())
                {
                    if (maxValue + 1 != e.Current)
                        throw new InvalidOperationException("Attempt to loop over index array that isn't sequential - not yet supported!");
                    maxValue = e.Current;
                }
                maxValue++; // b/c the loop is <, not <= when we code it in C++.
            }

            return new EnumerableRangeArrayInfo(new ValSimple(minValue.ToString(), typeof(int)),
                new ValSimple(maxValue.ToString(), typeof(int)));
        }
    }


    /// <summary>
    /// Return an array info that does nothing... this is a place holder in the case that the
    /// code has already been generated.
    /// </summary>
    class DummyArrayInfo : IArrayInfo
    {
        public Tuple<Expression, IDeclaredParameter> AddLoop(IGeneratedQueryCode env, ICodeContext context, CompositionContainer container)
        {
            return null;
        }
    }

}
