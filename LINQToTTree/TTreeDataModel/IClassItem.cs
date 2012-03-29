using System.Xml.Serialization;

namespace TTreeDataModel
{
    [XmlInclude(typeof(ItemVector))]
    [XmlInclude(typeof(ItemSimpleType))]
    [XmlInclude(typeof(ItemROOTClass))]
    [XmlInclude(typeof(ItemCStyleArray))]
    public abstract class IClassItem
    {
        /// <summary>
        /// Get the item type (in the .NET world).
        /// </summary>
        [XmlAttribute]
        abstract public string ItemType { get; set; }

        /// <summary>
        /// Get/Set the name of the member!
        /// </summary>
        [XmlAttribute]
        abstract public string Name { get; set; }

        /// <summary>
        /// Get/Set if this is a pointer to another object.
        /// </summary>
        [XmlAttribute]
        abstract public bool NotAPointer { get; set; }
    }
}
