using System;

namespace Microsoft.Pex.Framework.Instrumentation
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexAssemblyUnderTestAttribute : Attribute
    {
        public PexAssemblyUnderTestAttribute(string positionalString)
        {
        }
    }
}
