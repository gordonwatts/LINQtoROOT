
using System.Xml.Serialization;
namespace TTreeDataModel
{
    public class ItemSimpleType : IClassItem
    {
        public ItemSimpleType(string name, string itemtype)
        {
            Name = name;
            ItemType = itemtype;
        }

        /// <summary>
        /// Ctor for serialization
        /// </summary>
        public ItemSimpleType()
        {
        }

        /// <summary>
        /// Return the type of this guy
        /// </summary>
        [XmlAttribute]
        public override string ItemType { get; set; }


        [XmlAttribute]
        public override string Name { get; set; }
    }
}
