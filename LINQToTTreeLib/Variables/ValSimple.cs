using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Variables
{
    /// <summary>
    /// A simple value holder - keeps whatever was put into it. Used by the ExpressionVisitor, for example.
    /// </summary>
    class ValSimple : IValue
    {
        /// <summary>
        /// Very simple! :-)
        /// </summary>
        /// <param name="v"></param>
        public ValSimple(string v, Type t)
        {
            if (v == null)
                throw new ArgumentNullException("There is no such thing as a null value!");
            if (t == null)
                throw new ArgumentNullException("The type for the variable must be set!");

            RawValue = v;
            Type = t;
        }

        /// <summary>
        /// The value we should report!
        /// </summary>
        public string RawValue { get; private set; }


        public Type Type { get; private set; }
    }
}
