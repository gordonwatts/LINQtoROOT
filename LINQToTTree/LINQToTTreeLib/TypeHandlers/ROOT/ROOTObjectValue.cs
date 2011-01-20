using System;
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
            InitialValue = null;
        }
        public string RawValue { get; private set; }

        public Type Type { get; private set; }

        public string VariableName { get; private set; }

        public IValue InitialValue { get; set; }


        public bool Declare
        {
            get { throw new NotImplementedException(); }
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
