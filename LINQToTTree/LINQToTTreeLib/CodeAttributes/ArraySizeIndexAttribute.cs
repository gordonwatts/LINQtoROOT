using System;

namespace LINQToTTreeLib.CodeAttributes
{
    /// <summary>
    /// An attribute to dectorate a C++ style array - and indicate the
    /// leaf name that contains the length we "care" about.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class ArraySizeIndexAttribute : Attribute
    {
        readonly string _arraySizeLeaf;

        // This is a positional argument
        public ArraySizeIndexAttribute(string leafName)
        {
            this._arraySizeLeaf = leafName;
        }

        /// <summary>
        /// Get the leaf name associated with this array index.
        /// </summary>
        public string LeafName
        {
            get { return _arraySizeLeaf; }
        }
    }
}
