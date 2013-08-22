using System;

namespace Microsoft.Pex.Framework.Validation
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexAllowedExceptionFromTypeUnderTestAttribute : Attribute
    {
        public PexAllowedExceptionFromTypeUnderTestAttribute(Type c, bool AcceptExceptionSubtypes = false)
        {
        }

        public bool AcceptExceptionSubtypes { get; set; }
    }
}
