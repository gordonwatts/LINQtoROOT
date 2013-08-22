using System;

namespace Microsoft.Pex.Framework.Instrumentation
{
    public enum PexInstrumentationLevel
    {
        Excluded
    }
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class PexInstrumentTypeAttribute : Attribute
    {
        public PexInstrumentTypeAttribute(Type p1, PexInstrumentationLevel level = PexInstrumentationLevel.Excluded)
        {
        }

        public PexInstrumentationLevel InstrumentationLevel { get; set; }
    }
}
