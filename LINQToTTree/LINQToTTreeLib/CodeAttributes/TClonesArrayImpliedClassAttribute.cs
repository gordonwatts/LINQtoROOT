using System;

namespace LINQToTTreeLib.CodeAttributes
{
    /// <summary>
    /// Attribute indicating that the class it is attached to is an implied TTClonesArray class. This
    /// means any array index 0 item reference to its leaves refers to the index into the tclones array.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TClonesArrayImpliedClassAttribute : Attribute
    {
        public TClonesArrayImpliedClassAttribute()
        {
        }
    }
}
