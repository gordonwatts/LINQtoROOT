using System;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Variables
{
    /// <summary>
    /// Holds an object of some sort.
    /// </summary>
    internal class VarObject : IVariable
    {
        public VarObject(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Must have a real type");

            Type = type;
            VariableName = type.CreateUniqueVariableName();
            RawValue = VariableName;
        }

        public string VariableName { get; private set; }
        public string RawValue { get; private set; }
        public Type Type { get; private set; }


        public IValue InitialValue { get; set; }


        public bool Declare { get; set; }


        public void RenameRawValue(string oldname, string newname)
        {
            if (InitialValue != null)
                InitialValue.RenameRawValue(oldname, newname);
            if (RawValue == oldname)
            {
                RawValue = newname;
                VariableName = newname;
            }
        }
    }
}
