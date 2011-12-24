
using System.Xml.Serialization;
namespace TTreeDataModel
{
    /// <summary>
    /// A group of items that are 
    /// </summary>
    public class ArrayGroup
    {
        /// <summary>
        /// The name of the group
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// The list of variable names that this group contains.
        /// </summary>
        public VariableInfo[] Variables { get; set; }

        /// <summary>
        /// Any comments the user wants to show up in the intellisense
        /// </summary>
        [XmlAttribute]
        public string Comment { get; set; }

        /// <summary>
        /// A custom class name, rather than the automatically generated one.
        /// </summary>
        [XmlAttribute]
        public string ClassName { get; set; }
    }
}
