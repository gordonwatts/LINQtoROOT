using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Visit an expression and translate it to another expression. We are driven by attributes
    /// on the objects we are trying to translate.
    /// </summary>
    public class TranslatingExpressionVisitor : RelinqExpressionVisitor
    {
        /// <summary>
        /// Translate a fully formed expression. Partial expressions
        /// are not trnaslated, however!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="cookies">List of cookies - a trail of variable renames we've done</param>
        /// <returns></returns>
        public static Expression Translate(Expression expr, List<string> cookies, Func<Expression, Expression> resolver)
        {
            // Remove Tuple's and custom objects.
            var exprObjsRemoved = ObjectPropertyExpressionVisitor.RemoveObjectAndTupleReferences(expr);

            // Now, do our custom translations.
            var oldResolver = _impl.Value.Resolver;
            _impl.Value.Resolver = resolver;

            var oldRenameList = _impl.Value.RenameList;
            _impl.Value.RenameList = new List<string>();

            try {
                var result = _impl.Value.Visit(exprObjsRemoved);

                // Track all cookies going out!
                cookies.AddRange(_impl.Value.RenameList);
                return result;
            }
            finally
            {
                // Reset to previous state.
                _impl.Value.Resolver = oldResolver;
                _impl.Value.RenameList = oldRenameList;
            }
        }

        /// <summary>
        /// Cache the implementation
        /// </summary>
        private static Lazy<TranslatingExpressionVisitor> _impl = new Lazy<TranslatingExpressionVisitor>(() => new TranslatingExpressionVisitor());

        /// <summary>
        /// Get the minor stuff up and running
        /// </summary>
        private TranslatingExpressionVisitor()
        {
            RenameList = new List<string>();
        }

        /// <summary>
        /// Top level parser for an expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override Expression Visit(Expression expression)
        {
            ///
            /// Make sure this member reference is for a "leaf". For exmaple, if we have the valid expression
            /// for translation obj.jets[0].muons[1].pt and we are looking at obj.jets[0].muons - then we don't
            /// want to try to translate this! We check for this by looking for any translation instruction
            /// attributes assocated with the final type!
            /// 

            if (expression != null && !expression.IsLeafType())
                return expression;

            return VisitExpressionImplemented(expression);
        }

        /// <summary>
        /// See if we can't parse this expression, but do this w/out protection.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression VisitExpressionImplemented(Expression expression)
        {
            return base.Visit(expression);
        }

        /// <summary>
        /// Accessing a member of a class. The translation is a little tricky. For example:
        ///   arr.jets - don't translate that!
        ///   arr.jets[0].member - do translate that
        ///   [Where jets is a grouped variable]
        ///   
        ///   arr.mc.all_data[0] - translate this when "mc" is a rename, and all_data is a leaf.
        ///   
        ///   arr.jets[0].muonindex.pt - translate that to the muon array
        ///   arr.jets[0].muonsindex[0].pt - translate to the first index of the muon array.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression expression)
        {
            ///
            /// If this is a bare array that is connected to the main object, then we want
            /// to not translate this. It will be delt with further above us.
            /// 

            if (expression.IsRootObjectArrayReference())
                return expression;

            ///
            /// If this is an object member on a sub-query expression, then it is too early for
            /// us to look at it. It will come back through here when it gets translated by
            /// lower level loop unroller code.
            /// 

            if (expression.Expression is Remotion.Linq.Clauses.Expressions.SubQueryExpression)
                return expression;

            ///
            /// See if the source has a "translated-to" class on it?
            /// 

            var attr = expression.Expression.Type.TypeHasAttribute<TranslateToClassAttribute>();
            if (attr != null)
            {
                return RecodeClass(expression, attr);
            }

            ///
            /// Ok, next see if this is an array recoding
            /// 

            var attrV = expression.Member.TypeHasAttribute<TTreeVariableGroupingAttribute>();
            if (attrV != null)
            {
                ///
                /// Sometimes we are deep in the middle of a lambda translation, or similar. In that case,
                /// we may not be able to walk all the way back. There is a pattern we can detect, however.
                /// 

                if (expression.Expression is ParameterExpression)
                    return expression;

                //
                // If we are looking at a [subqueryexpression].[grouping-obj].[member], which below should be translating
                // soem how, we need to wait until the subqueryexpression has been totally translated. Which will happen
                // later on, and then come back through here.
                //

                if (expression.Expression is MemberExpression)
                {
                    var mbaccess = expression.Expression as MemberExpression;
                    if (mbaccess is MemberExpression)
                    {
                        if (mbaccess.Expression is SubQueryExpression)
                            return expression;
                    }
                }

                ///
                /// Regular array recoding - so this is at the top level and is the most
                /// common kind
                /// 

                var recodedExpression = RecodeArrayGrouping(expression);
                if (recodedExpression != null)
                    return recodedExpression;

                ///
                /// The only other possibility is if this is in another object that
                /// we can decode as an array reference. In short - this guy is a 
                /// pointer from another guy to this guy.
                /// 

                var recoded = RecodeArrayPointer(expression);
                if (recoded != null)
                    return recoded;

                ///
                /// If this has an recode attribute, but we can't do it. Fail!
                /// 

                throw new NotImplementedException("Member '" + expression.Member.Name + "' is marked for array recoding, but we can't figure out what to do.");
            }

            ///
            /// Hopefully the default behavior is the right thing to do here!
            /// 

            return base.VisitMember(expression);
        }

        /// <summary>
        /// Info form parsing a array pointer expression.
        /// </summary>
        class ArrayPointerInfo
        {
            /// <summary>
            /// The index expression that you would use to look up in teh TargetMemberExpression.
            /// It is in the translated world.
            /// </summary>
            public Expression TargetIndexExpression { get; internal set; }

            /// <summary>
            /// The member pointer for the object that is the array we are pointing to.
            /// </summary>
            public MemberInfo TargetMember { get; internal set; }

            /// <summary>
            /// The target array that we wnat to index into. Add the array index onto this, and then
            /// resolve it to get what you actually want. This is still in the translated world.
            /// </summary>
            public MemberExpression TargetMemberExpression { get; internal set; }
        }

        /// <summary>
        /// Given an expression that looks like "arr.jets[0].muonIndex" or "arr.jets[0].muons[0]" it will
        /// parse the expression. Note that you need to strip off the member reference that usually comes first.
        /// </summary>
        /// <param name="expression">Expression that refers to the ItemArray pointer</param>
        /// <returns>Null if this doesn't boil down to a member expression, a throw if it is a malformed one, or all the info you need to reconstruct the expression.</returns>
        private ArrayPointerInfo DecodeArrayPointerExpression(Expression expression)
        {
            // First, get expression down to a member expression. The only things that
            // we are allowed to strip off are array references.
            var arrayLookups = new List<Expression>();
            while (expression.NodeType == ExpressionType.ArrayIndex)
            {
                var aind = expression as BinaryExpression;
                arrayLookups.Add(aind.Right);
                expression = aind.Left;
            }

            // We should be left with a member access that has the correct attributes attached to it!
            var indexMemberExpression = expression as MemberExpression;
            if (indexMemberExpression == null)
                return null;

            var indexMember = indexMemberExpression.Member;
            var attrIndexReferences = TypeUtils.TypeHasAttribute<IndexToOtherObjectArrayAttribute>(indexMember);
            if (attrIndexReferences == null)
                throw new NotImplementedException("Index variable '" + indexMember.Name + "' was not marked with the IndexToOtherObjectArray attribute");

            var indexTargetMember = attrIndexReferences.BaseType.GetMember(attrIndexReferences.ArrayName).FirstOrDefault();
            if (indexTargetMember == null)
                throw new NotImplementedException("Could nto find member '" + attrIndexReferences.ArrayName + "' in type " + attrIndexReferences.BaseType.Name);

            // Get the array that we are going to reference in our translated world.
            var rootObject = FindObjectOfType(expression, attrIndexReferences.BaseType);

            // Next, the source expression should be an index, so make sure it translates to
            // an integer... And if there were multiple array references then we need to unwind them here.
            var sourceIndex = VisitExpressionImplemented(expression);
            if (sourceIndex == null)
                throw new NotImplementedException("Failed to translate expression '" + expression.ToString() + "' of '" + expression.ToString() + "'");

            foreach (var arrayExpression in arrayLookups)
            {
                sourceIndex = Expression.ArrayIndex(sourceIndex, arrayExpression);
            }

            // The array indexer must be an integer. Force the issue if it wasn't already. The reason this
            // may not have been previously done is sometimes the indexer contains a fairly complex query expression
            // for which simple resolution isn't possible.
            if (sourceIndex.Type != typeof(int))
            {
                sourceIndex = VisitExpressionImplemented(Resolver(sourceIndex));
                if (sourceIndex.Type != typeof(int))
                    throw new NotImplementedException("Array index expression is not an integer (it is a '" + sourceIndex.Type.Name + "') - failed with '" + expression.ToString() + "'");
            }

            // Now we are ready to build up our results and return the information as a summary.
            return new ArrayPointerInfo()
            {
                TargetMemberExpression = Expression.MakeMemberAccess(rootObject, indexTargetMember),
                TargetIndexExpression = sourceIndex,
                TargetMember = indexTargetMember
            };
        }

        /// <summary>
        /// See if this is of the form something.index.value - where index we can track back to a pointer to an actualy item.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression RecodeArrayPointer(MemberExpression expression)
        {
            var targetMemberName = expression.Member.Name;
            var sourceExpression = expression.Expression;

            ///
            /// Make sure that Name is a member
            /// 

            var extendedType = sourceExpression.Type;
            var targetMember = extendedType.GetMember(targetMemberName).First();
            if (targetMember == null)
                return null;

            // Do basic parsing
            var arrayInfo = DecodeArrayPointerExpression(sourceExpression);
            if (arrayInfo == null)
                return null;

            // Build up the final expression, and bail.
            var targetIndexedAccessExpression = Expression.ArrayIndex(arrayInfo.TargetMemberExpression, arrayInfo.TargetIndexExpression);
            var targetValueIndexedAccessExpression = Expression.MakeMemberAccess(targetIndexedAccessExpression, targetMember);
            return VisitExpressionImplemented(targetValueIndexedAccessExpression);
        }

        /// <summary>
        /// Recursively go down and find the expression this is of this type...
        /// </summary>
        /// <param name="sourceExpression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Expression FindObjectOfType(Expression sourceExpression, Type type)
        {
            // Are we done and don't know it?
            if (sourceExpression.Type == type)
                return sourceExpression;

            // Now, go down a level as best we can.
            if (sourceExpression.NodeType == ExpressionType.MemberAccess)
            {
                var memberAccess = sourceExpression as MemberExpression;
                return FindObjectOfType(memberAccess.Expression, type);
            }

            if (sourceExpression.NodeType == ExpressionType.ArrayIndex)
            {
                var arrayAccess = sourceExpression as BinaryExpression;
                return FindObjectOfType(arrayAccess.Left, type);
            }

            if (sourceExpression is SubQueryExpression)
            {
                var sqe = sourceExpression as SubQueryExpression;
                return FindObjectOfType(sqe.QueryModel.MainFromClause.FromExpression, type);
            }

            throw new NotImplementedException($"Don't know how to get back into '{sourceExpression.NodeType.ToString()}' ({sourceExpression.GetType().FullyQualifiedName()})");
        }

        /// <summary>
        /// We have an array grouping. We need to pop-it up one.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="attrV"></param>
        /// <returns></returns>
        private Expression RecodeArrayGrouping(MemberExpression expression)
        {
            ///
            /// First do some base checks. Pull appart the array index. If any of that
            /// doesn't work, then we just return a null.
            /// 

            if (expression.Expression.NodeType != ExpressionType.ArrayIndex)
                return null;
            var arrayIndexOperation = expression.Expression as BinaryExpression;

            var memberAccessArray = arrayIndexOperation.Left as MemberExpression;
            if (memberAccessArray == null)
                return null;

            var translateAttribute = TypeUtils.TypeHasAttribute<TranslateToClassAttribute>(memberAccessArray.Expression.Type);
            if (translateAttribute == null)
                return null;

            var targetMemberInfo = ResolveMemberName(translateAttribute.TargetClassType, expression.Member);
            if (targetMemberInfo == null)
                return null;

            ///
            /// Ok, at this point we are ready to start rebuilding the array access
            /// 

            var targetArray = Expression.MakeMemberAccess(TranslateRootObject(memberAccessArray.Expression, translateAttribute.TargetClassType), targetMemberInfo);
            var arrayLookup = Expression.MakeBinary(ExpressionType.ArrayIndex, targetArray, arrayIndexOperation.Right);

            return arrayLookup;
        }

        /// <summary>
        /// Converts a MemeberInfo into a name for lookup. Take into account
        /// any codeing attributes that might "change" the name.
        /// </summary>
        /// <param name="memberInfo">The info of the member we want to find on the other type</param>
        /// <param name="fromType">The type where we want to do the lookup</param>
        /// <returns></returns>
        private MemberInfo ResolveMemberName(Type fromType, MemberInfo memberInfo)
        {
            string targetMemberName = memberInfo.Name;

            var attr = TypeUtils.TypeHasAttribute<RenameVariableAttribute>(memberInfo);
            if (attr != null)
            {
                targetMemberName = attr.RenameTo;
                RenameList.Add(string.Format("{0}->{1}", memberInfo.Name, targetMemberName));
            }

            return fromType.GetMember(targetMemberName).FirstOrDefault();
        }

        private Expression RecodeClass(MemberExpression expression, TranslateToClassAttribute attr)
        {
            ///
            /// Ok. Now we need to see if this member has a translation request attached to it.
            /// 

            Expression result = RecodeWithRename(expression, attr.TargetClassType);
            if (result != null)
                return result;

            ///
            /// Ok, it doesn't. So what we need to look for, then, is the same member in the final class.
            /// 

            result = RecodeWithSameName(expression, attr.TargetClassType);
            if (result == null)
            {
                throw new InvalidOperationException("Unable to find member '" + expression.Member.Name + "' in target class '" + attr.TargetClassType.Name + "'.");
            }

            return result;
        }

        /// <summary>
        /// Parse any unary expressions we are supposed to be dealing with
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.ArrayLength:
                    return VisitArrayLength(expression);
                default:
                    return base.VisitUnary(expression);
            }
        }

        /// <summary>
        /// Deal with a special case for an index redirection where we are looking for
        /// an integer or some simply type. There is only one very special case where this
        /// shows up. Unlikely to be used by physics, actually. :-) The second case we
        /// handle is dealing with != for two object compares.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression expression)
        {
            //
            // If we are comparing a "new" against a null, then we don't support that.
            //

            if (expression.Left.IsNull() || expression.Right.IsNull())
            {
                Expression nonNull = expression.Left.IsNull() ? expression.Right : expression.Left;
                if (nonNull.NodeType == ExpressionType.New)
                {
                    throw new InvalidOperationException(string.Format("Doing a null comparison to a temporary object created in the query is very likely to generate incorrect code - not supported ({0})", expression.ToString()));
                }
            }

            // If this is an array index, then...
            if (expression.NodeType == ExpressionType.ArrayIndex && expression.IsLeafType())
            {
                var rootExpr = expression.Left as MemberExpression;
                if (rootExpr != null)
                {
                    return Expression.ArrayIndex(VisitExpressionImplemented(rootExpr), expression.Right);
                }
            }

            // If this is a comparison, and these are objects that have indicies (in whatever we are looping through)...
            if (expression.NodeType == ExpressionType.Equal || expression.NodeType == ExpressionType.NotEqual)
            {
                // Are we pointing to something we can deal with?
                var indexLeft = ExtractIndexReference(expression.Left);
                if (indexLeft != null)
                {
                    var indexRight = ExtractIndexReference(expression.Right);
                    if (indexRight != null)
                    {
                        if (expression.NodeType == ExpressionType.NotEqual)
                        {
                            return Expression.NotEqual(indexLeft, indexRight);
                        } else
                        {
                            return Expression.Equal(indexLeft, indexRight);
                        }
                    }
                }
            }

            return base.VisitBinary(expression);
        }

        /// <summary>
        /// Given an expression, see if we can decode it into an array pointer of one sort or another.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>or null if it can't be decoded as an array</returns>
        private Expression ExtractIndexReference(Expression expr)
        {
            try {
                if (expr.NodeType == ExpressionType.ArrayIndex)
                {
                    var indexExpr = (expr as BinaryExpression).Right;
                    return VisitExpressionImplemented(indexExpr);
                }
                else
                {
                    var arrayInfo = DecodeArrayPointerExpression(expr);
                    if (arrayInfo == null)
                        return null;

                    return VisitExpressionImplemented(arrayInfo.TargetIndexExpression);
                }
            } catch
            {
                return null;
            }
        }

        /// <summary>
        /// Array index can be a little rough b/c it can be traning to make a translation. This is actually quite tricking
        /// - especially in teh case of an array grouping - we have to go find a variable we can use as a proxy to get a size
        /// operator on! :-)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private Expression VisitArrayLength(UnaryExpression expression)
        {
            ///
            /// The key to this is what the operand is. If it isn't a member
            /// expression (like a functino call, etc.) then forget it.
            /// 

            var rootExpression = expression.Operand;
            if (rootExpression is MemberExpression)
            {
                var memberExpr = rootExpression as MemberExpression;
                var attrClassTranslate = TypeUtils.TypeHasAttribute<TranslateToClassAttribute>(memberExpr.Expression.Type);
                var attrMemberTypeGrouping = TypeUtils.TypeHasAttribute<TTreeVariableGroupingAttribute>(memberExpr.Member);
                var attrMemberIsIndex = TypeUtils.TypeHasAttribute<IndexToOtherObjectArrayAttribute>(memberExpr.Member);

                ///
                /// If this is a deep level index re-direct, and we are here, that means we have a 2D index that we are trying
                /// to take the length of (obj.jets[0].muonredirect.Length, where muondirectect is an array of indicies. We need
                /// to get this into "arr.muonredirect[0].Length". Note this is very different from doing the indirect lookup
                /// we usually do with an index! SO some special code is required.
                /// 

                if (attrClassTranslate == null && attrMemberIsIndex != null)
                {
                    return IndexedArrayLengthLookup(memberExpr, attrMemberIsIndex);
                }

                ///
                /// Is this something like a grouping variable. For index redirection - well that would be odd if we ever
                /// ended up here as an index operation is just a single pointer right now!
                /// 

                if (attrMemberTypeGrouping != null)
                {
                    ///
                    /// Ok. This a little complex - it is an array of some sort - a group of some sort, or something "deep". The problem is that
                    /// we don't parse or translate arrays - we parse references to arrays (with an index on them). So, what we do here is build up
                    /// an access to one of these guys and then run it, and then pull appart the answer. Evil. I know. :-)
                    /// 

                    var nonArrayMember = FindSubObjectMember(memberExpr.Type.GetElementType());
                    if (nonArrayMember != null)
                    {
                        var arrayLookup = Expression.ArrayIndex(memberExpr, Expression.Variable(typeof(int), "d"));
                        var leafLookup = Expression.MakeMemberAccess(arrayLookup, nonArrayMember);

                        var translated = VisitExpressionImplemented(leafLookup);
                        if (translated.NodeType != ExpressionType.ArrayIndex)
                            throw new InvalidOperationException("Tried to translate '" + leafLookup.ToString() + "' into an array index, but it didn't come back an array index - it came back '" + translated.ToString() + "'");

                        var lastItemIndex = translated as BinaryExpression;
                        return Expression.ArrayLength(lastItemIndex.Left);
                    }

                    ///
                    /// The next option is that this is an array that is sitting in an object that points back. So to deal with this we will
                    /// need to translate the root of this guy.
                    /// 

                    var translatedInterior = VisitExpressionImplemented(memberExpr);
                    var length = Expression.ArrayLength(translatedInterior);
                    return length;
                }

                ///
                /// Is this a simple straight-up class mapping?
                /// 

                if (attrClassTranslate != null)
                {
                    var result = RecodeClass(memberExpr, attrClassTranslate);
                    return Expression.ArrayLength(result);
                }
            }

            ///
            /// If we are here, we don't know how to translate. So just pop-it-up
            /// 

            return expression;
        }

        /// <summary>
        /// We have a member index like arr.jet[0].muonidnex.Length that has been called. We are called with just the muonidnex. We need to
        /// do the translation now.
        /// 
        ///   arr.jet[0].muonindex => obj.muonindex[0].Length
        ///   arr.jet.muonindex => obj.muonindex.Length
        /// 
        /// </summary>
        /// <param name="memberExpr"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        private Expression IndexedArrayLengthLookup(MemberExpression memberExpr, IndexToOtherObjectArrayAttribute attr)
        {
            ///
            /// First, we need to find the base class that does the translation for us.
            /// 

            var classToTranslateTo = TypeUtils.TypeHasAttribute<TranslateToClassAttribute>(attr.BaseType);
            if (classToTranslateTo == null)
                throw new NotImplementedException("Unable to translate '" + memberExpr + "' for an array length because the translation base type '" + attr.BaseType.Name + "' doesn't have a translate class attribute");

            ///
            /// Now, find, with renames, what the "muonindex" points to, and build access to it from
            /// a translated root.
            /// 

            var targetMember = ResolveMemberName(classToTranslateTo.TargetClassType, memberExpr.Member);

            var root = FindObjectOfType(memberExpr, attr.BaseType);
            var transRoot = TranslateRootObject(root, classToTranslateTo.TargetClassType);
            var memberAccess = Expression.MakeMemberAccess(transRoot, targetMember);

            ///
            /// See if the "jet" guy has an index on it.
            /// 

            Expression arrayLength = memberAccess;

            if (memberExpr.Expression.NodeType == ExpressionType.ArrayIndex)
            {
                var oldArrayIndex = memberExpr.Expression as BinaryExpression;
                arrayLength = Expression.ArrayIndex(arrayLength, oldArrayIndex.Right);
                if (!arrayLength.Type.IsArray)
                    throw new NotImplementedException("Unable to translate array length for '" + memberExpr + "' because we saw an index and adding it gave us '" + arrayLength + "' which isn't an array!");
            }

            arrayLength = Expression.ArrayLength(arrayLength);
            return arrayLength;
        }

        /// <summary>
        /// Look at a function that is a place holder for some code connected with this
        /// variable translation.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            // IsGoodIndex: make sure that the indexed parameter points to something interesting.
            var method = expression.Method;
            if (method.Name == "IsGoodIndex" && method.DeclaringType == typeof(Helpers))
            {
                // Make sure we are pointing to something that is an index into another object.
                // Arg 0 (the only arg) is the proper argument here.
                ArrayPointerInfo arrayInfo = null;
                try {
                    arrayInfo = DecodeArrayPointerExpression(expression.Arguments[0]);
                    if (arrayInfo == null)
                    {
                        throw new InvalidOperationException(string.Format("The Helpers.IsGoodIndex function can be applied only to members that are makred as an IndexToGroup in the data."));
                    }
                } catch (NotImplementedException e)
                {
                    throw new InvalidOperationException(e.Message, e);
                }

                // There should be no errors below now, we just have to lift out the size of the array we are pointing to and
                // go from there.
                var lengthExpr = VisitExpressionImplemented(Expression.ArrayLength(arrayInfo.TargetMemberExpression));
                var zeroExpr = Expression.Constant(0, typeof(int));

                var indexExpr = VisitExpressionImplemented(arrayInfo.TargetIndexExpression);

                var indexCheck = Expression.AndAlso(
                    Expression.GreaterThanOrEqual(indexExpr, zeroExpr),
                    Expression.LessThan(indexExpr, lengthExpr)
                    );

                // If the array index is an array leaf itself, then make sure that it also exists.
                if (indexExpr.NodeType == ExpressionType.ArrayIndex)
                {
                    var indexArray = (indexExpr as BinaryExpression).Left;
                    var indexArrayIndex = (indexExpr as BinaryExpression).Right;
                    var indexArraySize = Expression.ArrayLength(indexArray);
                    var checkIndexArray = Expression.GreaterThan(indexArraySize, indexArrayIndex);
                    indexCheck = Expression.AndAlso(checkIndexArray, indexCheck);
                }

                return indexCheck;
            }

            return base.VisitMethodCall(expression);
        }

        /// <summary>
        /// Given an expression like arr.jet[0].muonindex, which is pointing to obj.muons[obj.jet[0].muonindex], return
        /// an expression which is obj.muons.Length.
        /// </summary>
        /// <param name="memberExpr"></param>
        /// <param name="attrMemberIsIndex"></param>
        /// <returns></returns>
        private Expression TotalLengthOfTargetArray(MemberExpression memberExpr, IndexToOtherObjectArrayAttribute attr)
        {
            // Get where we are going, as this will be the basis of what we want here.
            var classToTranslateTo = TypeUtils.TypeHasAttribute<TranslateToClassAttribute>(attr.BaseType);
            if (classToTranslateTo == null)
                throw new NotImplementedException("Unable to translate '" + memberExpr + "' for an array length because the translation base type '" + attr.BaseType.Name + "' doesn't have a translate class attribute");

            // Get the array we are being redirected to - this is what we have to take the length of.
            var indexTargetMember = attr.BaseType.GetMember(attr.ArrayName).FirstOrDefault();
            if (indexTargetMember == null)
            {
                throw new InvalidOperationException(string.Format("Can't find array '{0}' on {1}.", attr.ArrayName, attr.BaseType));
            }

            // Now, build the array up
            var root = FindObjectOfType(memberExpr, attr.BaseType);
            var indexTargetAccess = Expression.MakeMemberAccess(root, indexTargetMember);
            return VisitExpressionImplemented(Expression.ArrayLength(indexTargetAccess));
        }

        /// <summary>
        /// Givne the current type see if we can find some variable that isn't an array...
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private MemberInfo FindSubObjectMember(Type type)
        {
            var firstNonArray = from member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public)
                                let field = member as FieldInfo
                                where field != null
                                orderby field.Name ascending
                                select field;

            // Look to see if anything is marked as a prefered index varaiable.
            var specialArrayLengthVars = firstNonArray
                .Where(m => m.GetCustomAttribute<UseAsArrayLengthVariableAttribute>() != null)
                .FirstOrDefault();
            if (specialArrayLengthVars != null)
            {
                return specialArrayLengthVars;
            }

            // Ok, just take the first one in that case.
            return firstNonArray.FirstOrDefault();
        }

        /// <summary>
        /// See if this is a rename or not
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Expression RecodeWithRename(MemberExpression expression, Type type)
        {
            var targetMember = ResolveMemberName(type, expression.Member);
            if (targetMember == null)
                throw new InvalidOperationException("Need to rename member '" + expression.Member.Name + "' on type '" + expression.Type.Name + "' but can't find it on type '" + type.Name + "'.");
            return Expression.MakeMemberAccess(TranslateRootObject(expression.Expression, type), targetMember);
        }

        /// <summary>
        /// Re-encode the expression to a new type, but with a member of the same name. Return null
        /// if we can't find the member.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Expression RecodeWithSameName(MemberExpression expression, Type type)
        {
            ///
            /// Find the member in the other class
            /// 

            var targetMember = type.GetMember(expression.Member.Name).FirstOrDefault();
            if (targetMember == null)
                return null;

            return Expression.MakeMemberAccess(TranslateRootObject(expression.Expression, type), targetMember);
        }

        /// <summary>
        /// Little holder object to wrap from one translated type to another
        /// </summary>
        class ExpressionHolder
        {
            public ExpressionHolder(Expression central)
            {
                HeldValue = central;
            }

            Expression HeldValue { get; set; }
        }

        /// <summary>
        /// Given the base of a guy we are doing this translation for, we should convert it
        /// for use down the line. We do this by putting it in the holder object.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Expression TranslateRootObject(Expression expression, Type type)
        {
            var ctor = type.GetConstructor(new Type[] { typeof(Expression) });
            if (ctor == null)
                throw new InvalidOperationException("Type '" + type.Name + "' doesn't have a ctor that takes an Expressin");

            return Expression.Constant(ctor.Invoke(new object[] { expression }));
        }

        /// <summary>
        /// Check to see if the class passed to us needs to
        /// be translated.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool NeedsTranslation(Type type)
        {
            return TypeUtils.TypeHasAttribute<TranslateToClassAttribute>(type) != null;
        }

        /// <summary>
        /// Keep track of a list of the renames we've done. This is so, in the end,
        /// we can make sure nothing has shifted out from under us!
        /// </summary>
        public List<string> RenameList { get; private set; }

        /// <summary>
        /// Get/Set the resolver function that is used to further
        /// resolve things.
        /// </summary>
        public Func<Expression, Expression> Resolver { get; set; }
    }
}
