using System;

namespace Microsoft.Pex.Framework.Suppression
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexSuppressStaticFieldStoreAttribute : Attribute
    {
        public PexSuppressStaticFieldStoreAttribute(string p1, string p2)
        {
        }

        public PexSuppressStaticFieldStoreAttribute(Type p1, string p2)
        {
        }
    }
}
