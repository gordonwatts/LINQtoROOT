using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LINQToTTreeLib.Utils
{
    /// <summary>
    /// Some helpers for types
    /// </summary>
    public static class TypeUtils
    {
        /// <summary>
        /// Keep track fo the variable name counter to make unique names.
        /// </summary>
        public static int _variableNameCounter = 0;

        /// <summary>
        /// Given a type, generate a unique variable name for use.
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static string CreateUniqueVariableName(this Type sourceType)
        {
            return ("a" + sourceType.Name).CreateUniqueVariableName();
        }

        /// <summary>
        /// Given a string, use it as a variable name...
        /// </summary>
        /// <param name="varbasename"></param>
        /// <returns></returns>
        public static string CreateUniqueVariableName(this string varbasename)
        {
            _variableNameCounter += 1;
            return (varbasename + "_" + _variableNameCounter.ToString()).Replace("`", "_").Replace("[", "_").Replace("]", "_");
        }

        /// <summary>
        /// Check to see if the class has the type attached to it (as an attribute). If so,
        /// return it.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="type_2"></param>
        /// <returns></returns>
        public static T TypeHasAttribute<T>(this System.Type classType)
            where T : class
        {
            if (classType == null)
                throw new ArgumentNullException("classType");

            var attr = classType.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
            return attr;
        }

        /// <summary>
        /// Check to see if the method has a custom attribute.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static T TypeHasAttribute<T>(this MemberInfo memberInfo)
            where T : class
        {
            var attr = memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
            return attr;
        }

        /// <summary>
        /// Check to see if the method has a custom attribute.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static T[] TypeHasAttributes<T>(this MemberInfo memberInfo)
            where T : class
        {
            var attr = memberInfo.GetCustomAttributes(typeof(T), false).Cast<T>().ToArray();
            return attr;
        }

        /// <summary>
        /// A type we know how to deal with as a number. For example, we can sum it, etc.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool IsNumberType(this Type v)
        {
            return v == typeof(int)
                || v == typeof(short)
                || v == typeof(double)
                || v == typeof(float);
        }

        /// <summary>
        /// We can trivially write this type out into a tree or similar.
        /// </summary>
        /// <param name="objectToDump"></param>
        /// <returns></returns>
        public static bool TypeIsEasilyDumped(this Type objectToDump)
        {
            return IsNumberType(objectToDump);
        }

        /// <summary>
        /// Extract a generic method burried inside an expression. Allows us to write somethign that makes sense,
        /// and have the computer pull everything out.
        /// </summary>
        /// <param name="methodReference"></param>
        /// <returns></returns>
        public static MethodInfo GetSupportedMethod(Expression<Action> methodReference)
        {
            return (methodReference.Body as MethodCallExpression)
                .ThrowIfNull(() => new InvalidOperationException("Passed a method reference that isn't a method reference!"))
                .Method
                .GetGenericMethodDefinition();
        }

        /// <summary>
        /// Simple cast operator for functional programming, man! :-)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="F"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TResult CastTo<TResult>(this object source)
            where TResult : class
        {
            return source as TResult;
        }

        /// <summary>
        /// Fetch the fields of an object, and return them in declaration order.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetFieldsInDeclOrder (this Type obj)
        {
            return obj.GetFields()
                .OrderBy(f => f.MetadataToken);
        }

        /// <summary>
        /// Returns the fully qualified name, including for generic parameters.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string FullyQualifiedName(this Type obj)
        {
            if (!obj.IsGenericType && !obj.IsGenericTypeDefinition)
            {
                return obj.Name;
            }

            var bld = new StringBuilder();
            bld.Append(obj.Name.Substring(0, obj.Name.IndexOf("`")));
            bld.Append("<");
            bool first = true;
            foreach (var arg in obj.GenericTypeArguments)
            {
                if (!first)
                {
                    bld.Append(",");
                }
                first = false;
                bld.Append(arg.FullyQualifiedName());
            }
            bld.Append(">");

            return bld.ToString();
        }
    }
}
