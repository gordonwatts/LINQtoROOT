using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Variables
{
    /// <summary>
    /// An integer variable
    /// </summary>
    public class VarInteger : IVariable
    {
        public string RawValue
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Type Type
        {
            get { return typeof(int); }
        }

        public string VariableName
        {
            get { throw new NotImplementedException(); }
        }


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
    }
}
