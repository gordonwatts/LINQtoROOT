using System;

namespace Microsoft.Pex.Framework
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexAllowedExceptionAttribute : Attribute
    {
        public PexAllowedExceptionAttribute(Type e)
        {
        }
    }
}
