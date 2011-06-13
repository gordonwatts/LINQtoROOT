using System;

namespace LINQToTTreeLib.CodeAttributes
{
    /// <summary>
    /// Marks a class as a CPP helper class. Satic methods can be addorned with the
    /// CPPCode attribute and that code will be inserted directly into the resulting
    /// query.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class CPPHelperClassAttribute : Attribute
    {
        // This is a positional argument
        public CPPHelperClassAttribute()
        {
        }
    }
    class CPPHelperClass
    {
    }
}
