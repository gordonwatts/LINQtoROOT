using System;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Variables
{
    /// <summary>
    /// An integer variable
    /// </summary>
    public class VarInteger : IVariable
    {
        public VarInteger()
        {
            VariableName = VarUtils.CreateUniqueVariableName("anint");
        }
        public string RawValue { get; private set; }

        public Type Type
        {
            get { return typeof(int); }
        }

        public string VariableName { get; private set; }

        public IValue InitialValue { get; set; }
    }
}
