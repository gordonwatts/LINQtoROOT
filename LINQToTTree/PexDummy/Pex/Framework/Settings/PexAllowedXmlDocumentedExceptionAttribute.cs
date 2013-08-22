using System;

namespace Microsoft.Pex.Framework.Settings
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexAllowedXmlDocumentedExceptionAttribute : Attribute
    {
        public PexAllowedXmlDocumentedExceptionAttribute()
        {
        }
    }
}
