using System;

namespace Microsoft.Pex.Framework.Using
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexUseTypeAttribute : Attribute
    {
        public PexUseTypeAttribute(Type positionalString, string assname = "dude")
        {
        }
    }
}
