using System;

namespace Microsoft.Pex.Framework.Settings
{
    public enum PexCoverageDomain
    {
        UserOrTestCode
    }
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexAssemblySettingsAttribute : Attribute
    {
        public PexAssemblySettingsAttribute(PexCoverageDomain pd = PexCoverageDomain.UserOrTestCode, string name = "hi")
        {
        }

        public string TestFramework { get; set; }
    }
}
