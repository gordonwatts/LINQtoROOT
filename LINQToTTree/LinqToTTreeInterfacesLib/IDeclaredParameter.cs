
using System;
namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Interface
    /// </summary>
    public interface IDeclaredParameter : IValue
    {
        /// <summary>
        /// Returns the name of the variable.
        /// </summary>
        string ParameterName { get; }

        /// <summary>
        /// Initial value to set this declared variable to. If
        /// null it shoudl be set to the default value (like "0" for int).
        /// Assume everything is explicitly initalized!
        /// </summary>
        IValue InitialValue { get; set; }

        /// <summary>
        /// Return the type of this parameter.
        /// </summary>
        new Type Type { get; }

        /// <summary>
        /// Rename the parameter - if has the old name
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        void RenameParameter(string oldname, string newname);
    }
}
