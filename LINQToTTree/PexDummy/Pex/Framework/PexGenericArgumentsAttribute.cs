using System;

namespace Microsoft.Pex.Framework
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexGenericArgumentsAttribute : Attribute
    {
        public PexGenericArgumentsAttribute(Type p)
        {
        }
    }
}
