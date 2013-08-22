using System;

namespace Microsoft.Pex.Framework
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexAssumeNotNullAttribute : Attribute
    {
        public PexAssumeNotNullAttribute()
        {
        }
    }
}
