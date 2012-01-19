using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    [Export(typeof(IQVScalarResultOperator))]
    class ROGroup : IQVScalarResultOperator
    {
        /// <summary>
        /// The LINQ Group By operator - does grouping (with key, sorting, etc.)
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(GroupResultOperator);
        }

        /// <summary>
        /// Process the grouping operator. We have to sort through the items, group them, and then
        /// create an object we can be translated later to access the items or the Key. We need to return
        /// an IEnumerable<IGrouping<TKey, TElement>>... As a result, this is one of those operators that has
        /// a fair amount of implementation in other parts of the re-linq structure.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        /// <remarks>
        /// re-linq blog post that shows the format of the query we are dealing with: https://www.re-motion.org/blogs/mix/2009/09/01/re-linq-how-to-support-ldquogroup-intordquo-with-aggregates
        /// Not as useful:
        /// Code for the result operator (including in-memory execution): https://svn.re-motion.org/svn/Remotion/trunk/Relinq/Core/Clauses/ResultOperators/GroupResultOperator.cs
        /// Unit tests for the result operator: https://www.re-motion.org/fisheye/browse/~raw,r=17871/Remotion/trunk/Remotion/Data/Linq.UnitTests/Linq/SqlBackend/SqlPreparation/ResultOperatorHandlers/GroupResultOperatorHandlerTest.cs
        /// </remarks>
        System.Linq.Expressions.Expression IQVScalarResultOperator.ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            var groupOp = resultOperator as GroupResultOperator;
            if (groupOp == null)
                throw new ArgumentNullException("resultOperator");

            //
            // Now create the object that will be handed back for later parsing.
            //

            var t_return = typeof(GroupByTypeTag<int, int>).GetGenericTypeDefinition().MakeGenericType(new Type[] { groupOp.KeySelector.Type, groupOp.ElementSelector.Type });

            return Expression.Constant(null, t_return);
        }
    }

    /// <summary>
    /// Helper class that is used later on in the code to deal with
    /// the array decoding.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    class GroupByTypeTag<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
    {
        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
