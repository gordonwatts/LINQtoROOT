using System;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// Print out basic info - helpful for debugging.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + Type.Name + ") " + RawValue;
        }

        /// <summary>
        /// Rename everything in teh raw value if need be...
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        public void RenameRawValue(string oldname, string newname)
        {
            RawValue = Regex.Replace(RawValue, @"\b" + oldname + @"\b", newname);
        }
    }
}
