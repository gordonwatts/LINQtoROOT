using System;

namespace Microsoft.Pex.Framework.Generated
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexRaisedExceptionAttribute : Attribute
    {
        public PexRaisedExceptionAttribute(Type positionalString)
        {
        }
    }
}
