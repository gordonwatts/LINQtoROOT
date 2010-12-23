using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        abstract public string ItemType { get; set;}

        /// <summary>
        /// Get/Set the name of the member!
        /// </summary>
        abstract public string Name { get; set; }
    }
}
