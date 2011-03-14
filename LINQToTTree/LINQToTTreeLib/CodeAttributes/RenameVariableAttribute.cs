using System;

namespace LINQToTTreeLib.CodeAttributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class RenameVariableAttribute : Attribute
    {
        // This is a positional argument
        public RenameVariableAttribute(string renameTo)
        {
            RenameTo = renameTo;
        }

        public string RenameTo { get; set; }
    }
}
