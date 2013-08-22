using System;

namespace Microsoft.Pex.Framework
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexClassAttribute : Attribute
    {
        // This is a positional argument
        public PexClassAttribute(Type c = null)
        {
        }
    }
}
