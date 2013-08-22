using System;

namespace Microsoft.Pex.Framework.Suppression
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexSuppressUninstrumentedMethodFromTypeAttribute : Attribute
    {
        public PexSuppressUninstrumentedMethodFromTypeAttribute(Type c)
        {
        }
    }
}
