using System;
using System.Linq;
using System.Reflection;

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
            return varbasename + "_" + _variableNameCounter.ToString();
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

    }
}
