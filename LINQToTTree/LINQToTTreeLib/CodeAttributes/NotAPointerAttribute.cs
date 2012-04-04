using System;

namespace LINQToTTreeLib.CodeAttributes
{
    /// <summary>
    /// Applied to a field in an object that is being translated. It says it is a reference,
    /// and not a pointer. This means that when you "." off it, it should be a "." and not
    /// a -> in C++.
    /// </summary>
    /// <remarks>
    /// Say you have:
    ///   class bogus {
    ///     [NotAPointer] int[] bogus;
    ///   }
    /// and you ask for bogus a, "a.bogus.Length" you will get back "a->bogus.size()", and if you
    /// didn't have NotAPointer above, you'd get back "a->bogus->size()".
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class NotAPointerAttribute : Attribute
    {
        public NotAPointerAttribute()
        {
        }

    }
}
