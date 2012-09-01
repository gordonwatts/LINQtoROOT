using System;
using System.Text;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.TypeHandlers.ROOT
{
    /// <summary>
    /// Hold onto a value that represents a ROOT object. This value is actually something that gets loaded from remote locations!
    /// each time this is referenced we will be creating a new copy of it (i.e. memory leak if you aren't careful).
    /// </summary>
    class ROOTObjectCopiedValue : IValue
    {
        /// <summary>
        /// Create a reference to a ROOT variable that is going to be loaded remotely.
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="rootType"></param>
        public ROOTObjectCopiedValue(string varName, Type rootType, string CPPType, string name, string title)
        {
            if (string.IsNullOrWhiteSpace(varName))
                throw new ArgumentException("invalid variable name");

            if (string.IsNullOrWhiteSpace(CPPType))
                throw new ArgumentException("Invalid C++ type name");

            if (rootType == null)
                throw new ArgumentException("Invalid type");

            //
            // The type and the loader string. The format of the loader string depends on
            // the type of the object. If the type is TNamed then we can go directly. Otherwise
            // we are going to end up depending on a TParameter<>.
            // 

            Type = rootType;
            OriginalName = name;
            OriginalTitle = title;

            StringBuilder loadString = new StringBuilder();
            var cls = rootType.GetROOTClass();
            if (cls.InheritsFrom("TNamed"))
            {
                loadString.AppendFormat("LoadFromInputList<{0}>(\"{1}\")", CPPType, varName);
            }
            else
            {
                loadString.AppendFormat("LoadFromInputListTP<{0}>(\"{1}\")", cls.Name, varName);
            }
            RawValue = loadString.ToString();
        }

        /// <summary>
        /// Return the Raw value - what we should use when we try to access this value.
        /// </summary>
        public string RawValue { get; private set; }

        /// <summary>
        /// The type of this variable
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The name of the original root object - we will reset it to store it correctly.
        /// </summary>
        public string OriginalName { get; private set; }

        /// <summary>
        /// The original title of the root object.
        /// </summary>
        public string OriginalTitle { get; private set; }


        public void RenameRawValue(string oldname, string newname)
        {
            RawValue = Regex.Replace(RawValue, @"\b" + oldname + @"\b", newname);
        }
    }
}
