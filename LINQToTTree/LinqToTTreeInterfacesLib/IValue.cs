using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Rename a raw value.
        /// </summary>
        /// <param name="newname"></param>
        void RenameRawValue(string oldname, string newname);

        /// <summary>
        /// Returns a list of all declared parameters this expression
        /// uses, if any.
        /// </summary>
        IEnumerable<IDeclaredParameter> Dependants { get; }
    }
}
