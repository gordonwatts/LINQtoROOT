using System;
using LinqToTTreeInterfacesLib;

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

            System.Linq.Expressions.Expression IValue.RawValue
            {
                get { throw new NotImplementedException(); }
            }
        }

        public VarInteger()
        {
            throw new NotImplementedException();
#if fales
            VariableName = VarUtils.CreateUniqueVariableName("anint");
            InitialValue = new IntVal() { RawValue = "0" };
            Declare = true;
            RawValue = VariableName;
#endif
        }
        public string RawValue { get; private set; }

        public System.Type Type
        {
            get { return typeof(int); }
        }

        public string VariableName { get; private set; }

        public IValue InitialValue { get; set; }


        public bool Declare { get; set; }

        System.Linq.Expressions.Expression IValue.RawValue
        {
            get { throw new NotImplementedException(); }
        }
    }
}
