using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remotion.Linq.Clauses.Expressions;
using System.Linq.Expressions;

namespace LINQToTTreeLib.QueryVisitors.RemoteNew
{
    /// <summary>
    /// There are times we want to pass a new object over to the remote guy. For example, this happens when
    /// we want to do something with a histogram like Aggregate - so we create it. Unfortunately, re-linq goes
    /// ahead and does the new for us - and passes in the object. We then serialize the object and send it accross
    /// the link. In most cases, this is not necessary. That is where this guy comes in. It will prevent the "new"
    /// from being processes and force it to occur over in the C++ code rather than locally in the C# code.
    /// </summary>
    class RemoteNewExpression : ExtensionExpression
    {
        /// <summary>
        /// Helps to have an expression number here for fast dispatch later on!
        /// </summary>
        public const ExpressionType ExpressionType = (ExpressionType)110004;

        public RemoteNewExpression(NewExpression expr)
            : base(expr.Type, ExpressionType)
        {
        }

        protected override System.Linq.Expressions.Expression VisitChildren(Remotion.Linq.Parsing.ExpressionTreeVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
