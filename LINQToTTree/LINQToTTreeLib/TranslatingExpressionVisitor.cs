using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LINQToTTreeLib.CodeAttributes;
using Remotion.Data.Linq.Parsing;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Visit an expression and translate it to another expression. We are driven by attributes
    /// on the objects we are trying to translate.
    /// </summary>
    public class TranslatingExpressionVisitor : ExpressionTreeVisitor
    {
        /// <summary>
        /// Translate an 
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression Translate(Expression expr)
        {
            var trans = new TranslatingExpressionVisitor();
            return trans.VisitExpression(expr);
        }

        /// <summary>
        /// Accessing a member of a class. See if 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            ///
            /// If this is an array, short-circut the translation here
            /// 

            if (expression.Type.IsArray)
                return expression;

            ///
            /// See if the source has a "translated-to" class on it?
            /// 

            var attr = TypeHasAttribute<TranslateToClassAttribute>(expression.Expression.Type);
            if (attr != null)
            {
                return RecodeClass(expression, attr);
            }

            ///
            /// Ok, next see if this is an array recoding
            /// 

            var attrV = TypeHasAttribute<TTreeVariableGroupingAttribute>(expression.Member);
            if (attrV != null)
            {
                ///
                /// Sometimes we are deep in the middle of a lambda translation, or similar. In that case,
                /// we may not be able to walk all the way back. There is a pattern we can detect, however.
                /// 

                if (expression.Expression is ParameterExpression)
                    return expression;

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

            return base.VisitMemberExpression(expression);

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

            ///
            /// Next job is to figure out where this index guy is pointing to. In order to
            /// do that look for the other link object. Do the check and make sure the types
            /// are correct so we are "ready" to go.
            /// 

            var indexMemberExpression = sourceExpression as MemberExpression;
            if (indexMemberExpression == null)
                return null;

            var indexMember = indexMemberExpression.Member;
            var attrIndexReferences = TypeHasAttribute<IndexToOtherObjectArrayAttribute>(indexMember);
            if (attrIndexReferences == null)
                throw new NotImplementedException("Index variable '" + indexMember.Name + "' was not marked with the IndexToOtherObjectArray attribute");

            var indexTargetMember = attrIndexReferences.BaseType.GetMember(attrIndexReferences.ArrayName).FirstOrDefault();
            if (indexTargetMember == null)
                throw new NotImplementedException("Could nto find member '" + attrIndexReferences.ArrayName + "' in type " + attrIndexReferences.BaseType.Name);

            ///
            /// Get the source of the expression. It has to be somewhere up our line, so we will dig deep to find it.
            /// 

            var rootObject = FindObjectOfType(sourceExpression, attrIndexReferences.BaseType);

            ///
            /// Next, the source expression should be an index, so make sure it translates to
            /// an integer...
            /// 

            var sourceIndex = Translate(sourceExpression);
            if (sourceIndex == null)
                throw new NotImplementedException("Failed to translate expression '" + sourceExpression.ToString() + "' of '" + expression.ToString() + "'");
            if (sourceIndex.Type != typeof(int))
                throw new NotImplementedException("Array index expression is not an integer (it is a '" + sourceIndex.Type.Name + "') - failed with '" + expression.ToString() + "'");

            ///
            /// Now, build an array index expression!
            /// 

            var accessTargetMemberExpression = Expression.MakeMemberAccess(rootObject, indexTargetMember);
            var targetIndexedAccessExpression = Expression.MakeBinary(ExpressionType.ArrayIndex, accessTargetMemberExpression, sourceIndex);
            var targetValueIndexedAccessExpression = Expression.MakeMemberAccess(targetIndexedAccessExpression, targetMember);

            ///
            /// Ok! Got it! Now, we need to translate this one and we are off to the races! :-)
            /// 

            return Translate(targetValueIndexedAccessExpression);
        }

        /// <summary>
        /// Recursively go down and find the expression this is of this type...
        /// </summary>
        /// <param name="sourceExpression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Expression FindObjectOfType(Expression sourceExpression, Type type)
        {
            ///
            /// Is it easy??
            /// 

            if (sourceExpression.Type == type)
                return sourceExpression;

            ///
            /// ok, check for each type that we know how to unravel...
            /// 

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

            throw new NotImplementedException("Don't know how to get back into '" + sourceExpression.NodeType.ToString() + "'");
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

            var arrayIndexOperation = expression.Expression as BinaryExpression;
            if (arrayIndexOperation == null)
                return null;
            if (arrayIndexOperation.NodeType != ExpressionType.ArrayIndex)
                return null;

            var memberAccessArray = arrayIndexOperation.Left as MemberExpression;
            if (memberAccessArray == null)
                return null;

            var translateAttribute = TypeHasAttribute<TranslateToClassAttribute>(memberAccessArray.Expression.Type);
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

            var attr = TypeHasAttribute<RenameVariableAttribute>(memberInfo);
            if (attr != null)
            {
                targetMemberName = attr.RenameTo;
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
        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.ArrayLength:
                    return VisitArrayLength(expression);
                default:
                    return base.VisitUnaryExpression(expression);
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
                ///
                /// Is this something like a grouping variable. For index redirection - well that would be odd if we ever
                /// ended up here as an index operation is just a single pointer right now!
                /// 

                var memberExpr = rootExpression as MemberExpression;
                if (TypeHasAttribute<TTreeVariableGroupingAttribute>(memberExpr.Member) != null)
                {
                    ///
                    /// Ok. This a little complex - it is an array of some sort - a group of some sort, or something "deep". The problem is that
                    /// we don't parse or translate arrays - we parse references to arrays (with an index on them). So, what we do here is build up
                    /// an access to one of these guys and then run it, and then pull appart the answer. Evil. I know. :-)
                    /// 

                    var nonArrayMember = FindNonArrayMember(memberExpr.Type.GetElementType());
                    var arrayLookup = Expression.ArrayIndex(memberExpr, Expression.Variable(typeof(int), "d"));
                    var leafLookup = Expression.MakeMemberAccess(arrayLookup, nonArrayMember);

                    var translated = Translate(leafLookup);
                    if (translated.NodeType != ExpressionType.ArrayIndex)
                        throw new InvalidOperationException("Tried to translate '" + leafLookup.ToString() + "' into an array index, but it didn't come back an array index - it came back '" + translated.ToString() + "'");

                    var lastItemIndex = translated as BinaryExpression;
                    return Expression.ArrayLength(lastItemIndex.Left);
                }

                ///
                /// Is this a simple straight-up class mapping?
                /// 

                var attr = TypeHasAttribute<TranslateToClassAttribute>(memberExpr.Expression.Type);
                if (attr != null)
                {
                    var result = RecodeClass(memberExpr, attr);
                    return Expression.ArrayLength(result);
                }
            }

            ///
            /// If we are here, we don't know how to translate. So just pop-it-up
            /// 

            return expression;
        }

        /// <summary>
        /// Givne the current type see if we can find some variable that isn't an array...
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private MemberInfo FindNonArrayMember(Type type)
        {
            var firstNonArray = from member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public)
                                let field = member as FieldInfo
                                where field != null
                                where !field.FieldType.IsArray
                                select field;
            return firstNonArray.First();
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
        /// Check to see if the class has the type attached to it (as an attribute). If so,
        /// return it.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="type_2"></param>
        /// <returns></returns>
        private static T TypeHasAttribute<T>(System.Type classType)
            where T : class
        {
            var attr = classType.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
            return attr;
        }

        /// <summary>
        /// Check to see if the method has a custom attribute.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private T TypeHasAttribute<T>(MemberInfo memberInfo)
            where T : class
        {
            var attr = memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
            return attr;
        }

        /// <summary>
        /// Check to see if the class passed to us needs to
        /// be translated.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool NeedsTranslation(Type type)
        {
            return TypeHasAttribute<TranslateToClassAttribute>(type) != null;
        }
    }
}
