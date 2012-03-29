using System;

namespace LINQToTTreeLib.CodeAttributes
{
    /// <summary>
    /// Applied to a field indicates during translation that it shouldn't turn into a
    /// "*" reference, but rather a pointer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class NotAPointerAttribute : Attribute
    {
        public NotAPointerAttribute()
        {
        }

    }
}
