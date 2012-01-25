using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
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
            //
            // Basic checks
            //

            var groupOp = resultOperator as GroupResultOperator;
            if (groupOp == null)
                throw new ArgumentNullException("resultOperator");

            if (!groupOp.KeySelector.Type.IsNumberType())
                throw new InvalidOperationException(string.Format("Don't know how to group by type '{0}'.", groupOp.KeySelector.Type.Name));

            //
            // First, record all the indicies and the values. This is what we are going to be grouping.
            // 

            var mapRecord = DeclarableParameter.CreateDeclarableParameterMapExpression(groupOp.KeySelector.Type, typeof(int).MakeArrayType());
            gc.AddOutsideLoop(mapRecord);

            var savePairValues = new StatementRecordPairValues(mapRecord,
                ExpressionToCPP.GetExpression(groupOp.KeySelector, gc, cc, container),
                ExpressionToCPP.GetExpression(cc.LoopIndexVariable, gc, cc, container));
            gc.Add(savePairValues);

            gc.Pop();

            //
            // Now create the object that will be handed back for later parsing. This should contian the info that is needed to do the
            // actual looping over the groups when it is requested.
            //

            var t_return = typeof(GroupByTypeTagEnum<int, int>).GetGenericTypeDefinition().MakeGenericType(new Type[] { groupOp.KeySelector.Type, groupOp.ElementSelector.Type });
            var ctor = t_return.GetConstructor(new Type[] { });
            var o = ctor.Invoke(new object[] { }) as BaseGroupInfo;

            o.MapRecord = mapRecord;
            o.TargetExpression = groupOp.ElementSelector;
            o.TargetExpressionLoopVariable = cc.LoopIndexVariable;

            return Expression.Constant(o, t_return);
        }
    }

    /// <summary>
    /// Helper class that is used later on in the code to deal with
    /// the array decoding.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    class GroupByTypeTagEnum<TKey, TElement> : BaseGroupInfo, IEnumerable<IGrouping<TKey, TElement>>
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

    /// <summary>
    /// Deal with the grouping elements.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    class GroupByType<TKey, TElement> : BaseGroupInfo, IGrouping<TKey, TElement>
    {

        public TKey Key
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Common class that holds the info we need about the created grouping info.
    /// </summary>
    class BaseGroupInfo
    {
        /// <summary>
        /// Keep track the map that holds the groups and the items in the group.
        /// </summary>
        public DeclarableParameter MapRecord { get; set; }

        /// <summary>
        /// The variable that is used to index the group itself - so the index we use to iterate over the group.
        /// </summary>
        public DeclarableParameter GroupIndexVariable { get; set; }

        /// <summary>
        /// The statement being used to do the actual looping over a group
        /// </summary>
        public StatementLoopOverGroups GroupLoopStatement { get; set; }

        /// <summary>
        /// The expression that we are putting into the group.
        /// </summary>
        public Expression TargetExpression { get; set; }

        /// <summary>
        /// The expression that is used in the target to select it out.
        /// </summary>
        public Expression TargetExpressionLoopVariable { get; set; }
    }

    /// <summary>
    /// Deal with the group by type. Const References come down here (because we create them above).
    /// </summary>
    [Export(typeof(IArrayInfoFactory))]
    internal class GroupByArrayFactor : IArrayInfoFactory
    {
        /// <summary>
        /// See if we can't resolve a group-by object into a looper of some sort.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <param name="ReGetIArrayInfo"></param>
        /// <returns></returns>
        public IArrayInfo GetIArrayInfo(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container, Func<Expression, IArrayInfo> ReGetIArrayInfo)
        {
            if (expr.Type.Name != "GroupByTypeTagEnum`2")
                return null;
            var param = expr as ConstantExpression;
            if (param == null)
                return null;

            var groupObj = param.Value as BaseGroupInfo;
            if (groupObj == null)
                throw new InvalidOperationException("Group object has a null value - should never happen!");

            //
            // Loop over the groups. groupIndex represents the actual group index.
            //

            var loopOverGroups = new Statements.StatementLoopOverGroups(groupObj.MapRecord);
            gc.Add(loopOverGroups);

            //
            // Finally, the loop index variable and we have to create the index object now, which is the grouping
            // (which can also be iterated over).
            //

            var t_return = typeof(GroupByType<int, int>).GetGenericTypeDefinition().MakeGenericType(param.Value.GetType().GetGenericArguments());
            var ctor = t_return.GetConstructor(new Type[] { });
            var o = ctor.Invoke(new object[] { }) as BaseGroupInfo;
            o.MapRecord = groupObj.MapRecord;
            o.TargetExpression = groupObj.TargetExpression;
            o.TargetExpressionLoopVariable = groupObj.TargetExpressionLoopVariable;
            o.GroupIndexVariable = loopOverGroups.IndexVariable;
            o.GroupLoopStatement = loopOverGroups;

            var loopVar = Expression.Constant(o);

            return new SimpleLoopVarSetting(loopVar, loopOverGroups.IndexVariable);
        }

    }

    /// <summary>
    /// Just set to the values we are given.
    /// </summary>
    class SimpleLoopVarSetting : IArrayInfo
    {
        private Expression _loopVariable;
        private Expression _loopIndex;

        public SimpleLoopVarSetting(Expression o, Expression groupIndex)
        {
            this._loopVariable = o;
            this._loopIndex = groupIndex;
        }
        public Tuple<Expression, Expression> AddLoop(IGeneratedQueryCode env, ICodeContext context, CompositionContainer container)
        {
            return new Tuple<Expression, Expression>(_loopVariable, _loopIndex);
        }
    }

    [Export(typeof(ITypeHandler))]
    internal class HandleGroupType : ITypeHandler
    {
        public bool CanHandle(Type t)
        {
            return t.Name == "GroupByType`2";
        }

        public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedQueryCode codeEnv, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        public Expression ProcessConstantReferenceExpression(ConstantExpression expr, CompositionContainer container)
        {
            return expr;
        }

        public Expression ProcessMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        public IValue CodeMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        public Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// We are accessing a member of this group object.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public IValue ProcessMemberReference(MemberExpression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            if (expr.Member.Name != "Key")
                throw new InvalidOperationException(string.Format("Unknown access to the member '{0}' of a LINQ GroupBy object.", expr.Member.Name));

            var cexpr = expr.Expression as ConstantExpression;
            if (cexpr == null)
                throw new InvalidOperationException("Group by reference to Key is null");
            var groupObj = cexpr.Value as BaseGroupInfo;
            if (groupObj == null)
                throw new InvalidOperationException("Group object reference is null");

            //
            // Extract the main object that we are iterating over.
            //

            return groupObj.GroupLoopStatement.GroupKeyReference.PerformAllSubstitutions(cc);
        }
    }

    /// <summary>
    /// The group-by is also enumerable - so we need to be able to deal with that here...
    /// </summary>
    [Export(typeof(IArrayInfoFactory))]
    internal class GroupByFactory : IArrayInfoFactory
    {
        /// <summary>
        /// Parse the group by enum into an arrary. For us, this is a loop over the vector array of the second argument of
        /// the iterator that was created in the above array info factory. :-)
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <param name="ReGetIArrayInfo"></param>
        /// <returns></returns>
        public IArrayInfo GetIArrayInfo(Expression expr, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container, Func<Expression, IArrayInfo> ReGetIArrayInfo)
        {
            //
            // Make sure this is something we want
            //

            BaseGroupInfo groupObj;

            if (expr.Type.IsGenericType
                && expr.Type.GetGenericTypeDefinition() == typeof(IGrouping<int, int>).GetGenericTypeDefinition()
                && cc.LoopVariable.Type.IsGenericType
                && cc.LoopVariable.Type.GetGenericTypeDefinition() == typeof(GroupByType<int, int>).GetGenericTypeDefinition())
            {
                var param = cc.LoopVariable as ConstantExpression;
                if (param == null)
                    return null;

                groupObj = param.Value as BaseGroupInfo;
            }
            else if (expr is ConstantExpression
                && expr.Type.IsGenericType
                && expr.Type.GetGenericTypeDefinition() == typeof(GroupByType<int, int>).GetGenericTypeDefinition())
            {
                groupObj = (expr as ConstantExpression).Value as BaseGroupInfo;
            }
            else
            {
                return null;
            }

            if (groupObj == null)
                throw new InvalidOperationException("Group by type object has a null value - should never happen!");

            //
            // Ok, now code up the loop over the interior.
            //

            var counter = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s = new Statements.StatementLoopOverGroupItems(groupObj.GroupLoopStatement.GroupItemsReference.PerformAllSubstitutions(cc), counter);
            gc.Add(s);

            //
            // Finally, return the loop index variable.
            // We queue up a replacement as well - so we can make sure that when the origianl loop variable is referenced, we
            // are actually referencing the interior one.
            //

            var loopExpression = groupObj.TargetExpression;
            cc.Add((groupObj.TargetExpressionLoopVariable as ParameterExpression).Name, Expression.Parameter(typeof(int), s.LoopItemIndex));

            return new SimpleLoopVarSetting(loopExpression, counter);
        }
    }
}
