using System;
using System.Linq.Expressions;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Represents some value (variable, constant, etc.).
    /// </summary>
    public interface IValue
    {
        /// <summary>
        /// The expression we represent.
        /// </summary>
        Expression RawValue { get; }

        /// <summary>
        /// The type of this guy - typeof(int), etc. Usually it will just be RawValue.Type.
        /// </summary>
        Type Type { get; }
    }
}
