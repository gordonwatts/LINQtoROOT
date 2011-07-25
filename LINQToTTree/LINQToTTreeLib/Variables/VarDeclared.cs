using System;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Variables
{
    /// <summary>
    /// A simple variable class for something that is already declared and being used
    /// in the code - in short, not something that will have to be used later on.
    /// </summary>
    public class VarDeclared : IVariable
    {
        private IValue _value;
        private string _name;

        public VarDeclared(IValue iValue, string indexName)
        {
            // TODO: Complete member initialization
            _value = iValue;
            _name = indexName;
        }
        public string VariableName
        {
            get { return _name; }
        }

        public IValue InitialValue
        {
            get { return _value; }
            set { _value = value; }
        }

        public bool Declare
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string RawValue
        {
            get { return _value.RawValue; }
        }

        public Type Type
        {
            get { return _value.Type; }
        }

        /// <summary>
        /// Renaming this is bad b/c there are connections to other places in the
        /// code that have already created this guy - so we should fail hard if we need
        /// to rename this w/out checking that it is ok.
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        public void RenameRawValue(string oldname, string newname)
        {
            throw new NotImplementedException();
        }
    }
}
