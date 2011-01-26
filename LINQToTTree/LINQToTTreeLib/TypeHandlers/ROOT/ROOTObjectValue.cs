using System;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.TypeHandlers.ROOT
{
    /// <summary>
    /// Hold onto a variable that represents a root object. This is actually used only once - when we start up - this
    /// is an object that we ship over the wire!
    /// </summary>
    class ROOTObjectValue : IVariable
    {
        private ROOTNET.Interface.NTObject _valueToUse;

        public ROOTObjectValue(ROOTNET.Interface.NTObject nTObject)
        {
            if (nTObject == null)
                throw new ArgumentNullException("nTObject");

            this._valueToUse = nTObject;
            VariableName = nTObject.GetType().CreateUniqueVariableName();
            RawValue = VariableName;
            Type = nTObject.GetType();

            ///
            /// ROOT Objects need to be sent over the wire and loaded. Someone else will, I hope, mark this guy
            /// as going over the wire. In the meantime, here we load it from input.
            /// 

            StringBuilder initialValueString = new StringBuilder();
            initialValueString.AppendFormat("LoadFromInputList<{0}>(\"{1}\")", nTObject.ClassName(), RawValue);
            InitialValue = new ValSimple(initialValueString.ToString(), Type);
        }
        public string RawValue { get; private set; }

        public Type Type { get; private set; }

        public string VariableName { get; private set; }

        public IValue InitialValue { get; set; }


        public bool Declare
        {
            get { return true; }
        }


        bool IVariable.Declare
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
