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
            if (attr == null)
            {
                return base.VisitMemberExpression(expression);
            }

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
