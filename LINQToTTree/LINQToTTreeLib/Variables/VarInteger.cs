using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Variables
{
    /// <summary>
    /// An integer variable
    /// </summary>
    public class VarInteger : IVariable
    {
        private class IntVal : IValue
        {
            public string RawValue { get; set; }

            public System.Type Type
            {
                get { return typeof(int); }
            }
        }

        public VarInteger()
        {
            VariableName = TypeUtils.CreateUniqueVariableName("anint");
            InitialValue = new IntVal() { RawValue = "0" };
            Declare = true;
            RawValue = VariableName;
        }
        public string RawValue { get; private set; }

        public System.Type Type
        {
            get { return typeof(int); }
        }

        public string VariableName { get; private set; }

        public IValue InitialValue { get; set; }


        public bool Declare { get; set; }
    }
}
