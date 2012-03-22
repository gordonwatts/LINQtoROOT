using System;

namespace LINQToTTreeLib.CodeAttributes
{
    /// <summary>
    /// An attribute to dectorate a C++ style array - and indicate the
    /// leaf name that contains the length we "care" about.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class ArraySizeIndexAttribute : Attribute
    {
        readonly string _arraySizeLeaf;

        // This is a positional argument
        public ArraySizeIndexAttribute(string leafName)
        {
            this._arraySizeLeaf = leafName;
            IsConstantExpression = false;
        }

        /// <summary>
        /// Get the leaf name associated with this array index.
        /// </summary>
        public string LeafName
        {
            get { return _arraySizeLeaf; }
        }

        /// <summary>
        /// Get/Set if this should be treated as a constant expressiln ("20").
        /// </summary>
        public bool IsConstantExpression { get; set; }

        /// <summary>
        /// Get/Set which array coordinate this refers to. 0 is the left most
        /// array index.
        /// </summary>
        public int Index { get; set; }
    }
}
