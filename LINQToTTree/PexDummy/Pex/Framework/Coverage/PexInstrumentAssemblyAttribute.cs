using System;

namespace Microsoft.Pex.Framework.Coverage
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexInstrumentAssemblyAttribute : Attribute
    {
        public PexInstrumentAssemblyAttribute(string name)
        {
        }
    }
}
