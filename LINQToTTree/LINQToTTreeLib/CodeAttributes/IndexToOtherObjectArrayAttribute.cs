using System;

namespace LINQToTTreeLib.CodeAttributes
{
    /// <summary>
    /// Used when an object (which is an index refernece) points to another collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class IndexToOtherObjectArrayAttribute : Attribute
    {
        public IndexToOtherObjectArrayAttribute(Type baseType, string arrayName)
        {
            BaseType = baseType;
            ArrayName = arrayName;
        }

        /// <summary>
        /// The base type of for the array reference
        /// </summary>
        public Type BaseType { get; set; }

        /// <summary>
        /// The name of the array
        /// </summary>
        public string ArrayName { get; set; }

    }
}
