using System.Xml.Serialization;

namespace TTreeDataModel
{
    [XmlInclude(typeof(ItemVector))]
    [XmlInclude(typeof(ItemSimpleType))]
    [XmlInclude(typeof(ItemROOTClass))]
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
    }
}
