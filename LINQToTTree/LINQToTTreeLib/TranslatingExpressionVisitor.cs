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
                throw new NotImplementedException("Index isn't a member access in '" + sourceExpression.ToString() + "'.");
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

            var targetMemberInfo = translateAttribute.TargetClassType.GetMember(expression.Member.Name).FirstOrDefault();
            if (targetMemberInfo == null)
                return null;

            ///
            /// Ok, at this point we are ready to start rebuilding the array access
            /// 

            var targetArray = Expression.MakeMemberAccess(TranslateRootObject(memberAccessArray, translateAttribute.TargetClassType), targetMemberInfo);
            var arrayLookup = Expression.MakeBinary(ExpressionType.ArrayIndex, targetArray, arrayIndexOperation.Right);

            return arrayLookup;
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
            /// See if there is an array grouping that needs to be dealt with?
            /// 

            result = RecodeWithArray(expression, attr.TargetClassType);
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
        /// Deal with an array index. This is interesting because it might be that
        /// the array access is on an object that needs to be "collapsed".
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            return base.VisitBinaryExpression(expression);
        }

        /// <summary>
        /// See if this is an array recoding....
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Expression RecodeWithArray(MemberExpression expression, Type type)
        {
            var attr = TypeHasAttribute<TTreeVariableGroupingAttribute>(expression.Member);
            if (attr == null)
                return null;



            throw new NotImplementedException();
        }

        /// <summary>
        /// See if this is a rename or not
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Expression RecodeWithRename(MemberExpression expression, Type type)
        {
            ///
            /// See if the rename is there
            /// 

            var attr = TypeHasAttribute<RenameVariableAttribute>(expression.Member);
            if (attr == null)
                return null;

            ///
            /// Now we need to construct the new expression with the rename.
            /// 

            var targetMember = type.GetMember(attr.RenameTo).FirstOrDefault();
            if (targetMember == null)
                throw new InvalidOperationException("Type '" + type.Name + "' doesn't have a member to rename to called '" + attr.RenameTo + "'");

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
