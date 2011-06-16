using System;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Variables
{
    /// <summary>
    /// A simple variable (like int, etc.).
    /// </summary>
    public class VarSimple : IVariable
    {
        public string VariableName { get; private set; }
        public string RawValue { get; private set; }
        public Type Type { get; private set; }

        public VarSimple(System.Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Must have a good type!");

            Type = type;
            VariableName = type.CreateUniqueVariableName();
            RawValue = VariableName;
            Declare = false;
        }


        public IValue InitialValue { get; set; }

        /// <summary>
        /// Get/Set if this variable needs to be declared.
        /// </summary>
        public bool Declare { get; set; }

        /// <summary>
        /// TO help with debugging...
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return VariableName + " = (" + Type.Name + ") " + RawValue;
        }
    }
}
