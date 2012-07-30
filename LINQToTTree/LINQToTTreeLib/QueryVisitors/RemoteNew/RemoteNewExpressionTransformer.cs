using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;
using System.Linq.Expressions;

namespace LINQToTTreeLib.QueryVisitors.RemoteNew
{
    /// <summary>
    /// Scan a new Expression to see if it is a candidate for being translated to something on the other side, rather than
    /// a local new.
    /// </summary>
    class RemoteNewExpressionTransformer : IExpressionTransformer<NewExpression>
    {
        /// <summary>
        /// The list of types for high speed dispatch.
        /// </summary>
        public ExpressionType[] SupportedExpressionTypes
        {
            get { return new ExpressionType[] { ExpressionType.New }; }
        }

        /// <summary>
        /// We have a new expression. If it statisfies our requirements, then run the transform on it!
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Expression Transform(NewExpression expression)
        {
            throw new NotImplementedException();
        }
    }
}
