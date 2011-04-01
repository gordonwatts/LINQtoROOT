using System;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Represents some value (variable, constant, etc.).
    /// </summary>
    public interface IValue
    {
        /// <summary>
        /// Returns, as a string, the value of this guy. Could be "10", or "j", depending.
        /// </summary>
        string RawValue { get; }

        /// <summary>
        /// The type of this guy - typeof(int), etc.
        /// </summary>
        Type Type { get; }
    }
}
