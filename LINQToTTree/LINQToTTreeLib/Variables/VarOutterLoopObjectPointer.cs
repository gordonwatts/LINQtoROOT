using System;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Variables
{
    /// <summary>
    /// There should, I hope, be only one of these. This is the outter loop variable. it is defined to point to
    /// *this - that way we can reference things in a uniform way in the code.
    /// </summary>
    internal class VarOutterLoopObjectPointer : IVariable
    {
        public VarOutterLoopObjectPointer(string varName, Type type)
        {
            VariableName = varName;
            Type = type;
            RawValue = "this";
            Declare = false;
        }

        public string RawValue { get; private set; }
        public Type Type { get; private set; }
        public string VariableName { get; private set; }


        public IValue InitialValue
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Declare { get; set; }
    }
}
