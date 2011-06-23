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


            /// <summary>
            /// We are an actual number, so we don't care.
            /// </summary>
            /// <param name="oldname"></param>
            /// <param name="newname"></param>
            public void RenameRawValue(string oldname, string newname)
            {
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

        /// <summary>
        /// Rename the variable if we need to. We have total control, so it is
        /// easy to deal with.
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        public void RenameRawValue(string oldname, string newname)
        {
            if (RawValue == oldname)
                RawValue = newname;
            InitialValue.RenameRawValue(oldname, newname);
        }
    }
}
