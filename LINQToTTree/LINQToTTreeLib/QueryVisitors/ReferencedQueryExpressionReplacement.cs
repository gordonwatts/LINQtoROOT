using Remotion.Linq;
using Remotion.Linq.Parsing;
using System;
using System.Collections.Generic;

namespace LINQToTTreeLib.QueryVisitors
{
    /// <summary>
    /// Attempt to replace reference expressions so we don't have to deal with them later.
    /// </summary>
    class ReferencedQueryExpressionReplacement : QueryModelVisitorBase
    {
        /// <summary>
        /// Limit how this guy is setup to run.
        /// </summary>
        private ReferencedQueryExpressionReplacement()
        {

        }

        /// <summary>
        /// Info of the current object we know something about.
        /// </summary>
        class ObjectInfo
        {
            public Type ObjectType;
        }

        /// <summary>
        /// The stack of the objects we are evaluating right now.
        /// </summary>
        private Stack<ObjectInfo> _objectDefinitionStack = new Stack<ObjectInfo>();

        /// <summary>
        /// Look for a "new", which will put something new on the stack, or a member property deref which
        /// will pull it off.
        /// </summary>
        class ObjectExpressionRemover : ExpressionTreeVisitor
        {
        }

        /// <summary>
        /// External just do it.
        /// </summary>
        /// <param name="qm"></param>
        internal static void Replace(QueryModel qm)
        {
            var transform = new ReferencedQueryExpressionReplacement();
            transform.VisitQueryModel(qm);
        }
    }
}
