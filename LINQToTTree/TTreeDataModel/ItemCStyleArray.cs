
using System.Xml.Serialization;
namespace TTreeDataModel
{
    /// <summary>
    /// Represents a C style array
    /// </summary>
    public class ItemCStyleArray : IClassItem
    {
        /// <summary>
        /// Craete an item for output that contains a C style array.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="indexItem"></param>
        public ItemCStyleArray(string type, string name, string indexItem)
        {
            ItemType = type;
            Name = name;
            IndexName = indexItem;
        }

        /// <summary>
        /// No parameters b/c that is required for serialization.
        /// </summary>
        public ItemCStyleArray()
        { }

        [XmlAttribute]
        public override string ItemType { get; set; }

        [XmlAttribute]
        public override string Name { get; set; }

        [XmlAttribute]
        public string IndexName { get; set; }
    }
}
