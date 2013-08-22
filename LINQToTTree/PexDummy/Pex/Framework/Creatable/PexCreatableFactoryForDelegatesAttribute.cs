using System;

namespace Microsoft.Pex.Framework.Creatable
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexCreatableFactoryForDelegatesAttribute : Attribute
    {
        public PexCreatableFactoryForDelegatesAttribute()
        {
        }
    }
}
