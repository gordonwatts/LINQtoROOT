using System;

namespace Microsoft.Pex.Framework
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexMethodAttribute : Attribute
    {
        // This is a positional argument
        public PexMethodAttribute(int MaxBranches = 100)
        {
        }

        public int MaxBranches { get; set; }

        public int MaxConditions { get; set; }

        public int MaxRunsWithoutNewTests { get; set; }
    }
}
