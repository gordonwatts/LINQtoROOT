using System;

namespace Microsoft.Pex.Linq
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexLinqPackageAttribute : Attribute
    {
        public PexLinqPackageAttribute()
        {
        }
    }
}
