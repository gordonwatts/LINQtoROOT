using System;

namespace Microsoft.Pex.Framework.Generated
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexDescriptionAttribute : Attribute
    {
        public PexDescriptionAttribute(string positionalString)
        {
        }
    }
}
