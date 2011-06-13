using System;

namespace LINQToTTreeLib.CodeAttributes
{
    /// <summary>
    /// Attached to a method this guy will insert the C++ code statements here.
    /// The return value can be used in the code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class CPPCodeAttribute : Attribute
    {
        /// <summary>
        /// Create the attribute
        /// </summary>
        /// <param name="code"></param>
        /// <param name="includeFiles"></param>
        public CPPCodeAttribute()
        {
        }

        /// <summary>
        /// A list of lines of C++ code. Result shoudl be called the same name as the method. Do not use
        /// that string anywhere else in your code as a string replacement will be done on
        /// the string "result"! Also, straight string replacements are done on arguments - so make them unique too!
        /// </summary>
        public string[] Code { get; set; }

        /// <summary>
        /// A list of include files. Just specify the filename - no need for "<", ">" or quotes!
        /// </summary>
        public string[] IncludeFiles { get; set; }
    }
}
