using System;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Variables
{
    /// <summary>
    /// A variable that represents an array. Translates to a vector<>. So we can
    /// cleanly pass things around inside our code.
    /// </summary>
    public class VarArray : IVariable
    {
        /// <summary>
        /// Returns the name of the variable.
        /// </summary>
        public string VariableName { get; private set; }

        /// <summary>
        /// Returns the variable that we need to return
        /// </summary>
        public IValue InitialValue { get; set; }

        /// <summary>
        /// Needs to be declared
        /// </summary>
        public bool Declare
        {
            get;
            set;
        }

        /// <summary>
        /// Get the text value of this variable.
        /// </summary>
        public string RawValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the type of this array
        /// </summary>
        public Type Type
        {
            get;
            private set;
        }

        public VarArray(System.Type type)
        {
            this.Type = type.MakeArrayType(1);
            RawValue = string.Format("{0}Array", type.CreateUniqueVariableName());
            VariableName = RawValue;
            Declare = false;
            InitialValue = null;
        }


        public void RenameRawValue(string oldname, string newname)
        {
            throw new NotImplementedException();
        }
    }
}
