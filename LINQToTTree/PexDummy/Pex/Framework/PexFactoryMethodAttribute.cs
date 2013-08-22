using System;

namespace Microsoft.Pex.Framework
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexFactoryMethodAttribute : Attribute
    {
        public PexFactoryMethodAttribute(Type c, string n = "")
        {
        }
    }
}
