using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Variables
{
    /// <summary>
    /// A constant that holds onto a string
    /// </summary>
    class ConstantString : IValue
    {
        /// <summary>
        /// Very simple! :-)
        /// </summary>
        /// <param name="v"></param>
        public ConstantString(string v)
        {
            if (v == null)
                throw new ArgumentNullException("There is no such thing as a null value!");

            RawValue = $"\"{v}\"";
        }

        /// <summary>
        /// The value we should report!
        /// </summary>
        public string RawValue { get; private set; }

        /// <summary>
        /// Return a string type
        /// </summary>
        public Type Type { get { return typeof(string); } }

        /// <summary>
        /// Print out basic info - helpful for debugging.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + Type.Name + ") " + RawValue;
        }

        /// <summary>
        /// Since this is a constant string, we do nothing with it here.
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        public void RenameRawValue(string oldname, string newname)
        {
        }
    }
}