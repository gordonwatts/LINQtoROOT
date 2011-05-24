using System;

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
    }
}
