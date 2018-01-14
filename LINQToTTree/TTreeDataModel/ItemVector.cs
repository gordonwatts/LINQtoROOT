using System.Xml.Serialization;

namespace TTreeDataModel
{
    /// <summary>
    /// An item that references some sort of a STL vector. This is a STL vector, not a C styl earray.
    /// This is important because you can do things like ask for the "size()" of the thing, how we deal
    /// with those things have to be kept seperate!
    /// </summary>
    public sealed class ItemVector : IClassItem
    {
        private string _vectorType;

        public ItemVector(string vectorCSDecl, string vectorName)
        {
            // TODO: Complete member initialization
            _vectorType = vectorCSDecl;
            ItemType = vectorCSDecl;
            Name = vectorName;
            NotAPointer = false;
        }

        /// <summary>
        /// Create an empty item vector
        /// </summary>
        public ItemVector()
        {
            NotAPointer = false;
        }

        [XmlAttribute]
        public override string ItemType { get; set; }

        [XmlAttribute]
        public override string Name { get; set; }

        /// <summary>
        /// Get/Set this not being a pointer.
        /// </summary>
        [XmlAttribute]
        public override bool NotAPointer { get; set; }
    }
}
