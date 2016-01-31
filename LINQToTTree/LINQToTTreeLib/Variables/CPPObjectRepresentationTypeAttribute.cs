using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Variables
{
    /// <summary>
    /// Attach to a .NET object that is to have a C++ representation.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CPPObjectRepresentationTypeAttribute : Attribute
    {
        /// <summary>
        /// Name the object's type in C++
        /// </summary>
        /// <param name="CPPTypeName"></param>
        public CPPObjectRepresentationTypeAttribute(string CPPTypeName)
        {
            this.CPPTypeName = CPPTypeName;
        }

        public string CPPTypeName { get; private set; }
    }
}
