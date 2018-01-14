
using System.Xml.Serialization;
namespace TTreeDataModel
{
    public sealed class ItemSimpleType : IClassItem
    {
        public ItemSimpleType(string name, string itemtype)
        {
            Name = name;
            ItemType = itemtype;
            NotAPointer = false;
        }

        /// <summary>
        /// Ctor for serialization
        /// </summary>
        public ItemSimpleType()
        {
            NotAPointer = false;
        }

        /// <summary>
        /// Return the type of this guy
        /// </summary>
        [XmlAttribute]
        public override string ItemType { get; set; }


        [XmlAttribute]
        public override string Name { get; set; }
        /// <summary>
        /// Get/Set this not being a ponter.
        /// </summary>
        [XmlAttribute]
        public override bool NotAPointer { get; set; }
    }
}
