using Microsoft.Pex.Framework.Settings;
using System;

namespace Microsoft.Pex.Framework.Coverage
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexCoverageFilterAssemblyAttribute : Attribute
    {
        public PexCoverageFilterAssemblyAttribute(PexCoverageDomain pc, string dude)
        {
        }
    }
}
