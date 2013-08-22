using System;

namespace Microsoft.Pex.Framework.Generated
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexGeneratedByAttribute : Attribute
    {
        // This is a positional argument
        public PexGeneratedByAttribute(Type c)
        {
        }
    }
}
