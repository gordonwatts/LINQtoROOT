using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.CodeAttributes
{
    /// <summary>
    /// Marks a particular member variable as being the one to use as an index array length variable.
    /// </summary>
    /// <remarks>
    /// Normally generated automatically from XML by the parser code
    /// </remarks>
    [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class UseAsArrayLengthVariableAttribute : Attribute
    {
        public UseAsArrayLengthVariableAttribute()
        {
        }
    }
}
