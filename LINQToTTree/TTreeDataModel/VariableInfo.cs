
using System.Xml.Serialization;
namespace TTreeDataModel
{
    /// <summary>
    /// A single variable and all info associated with it for the user to change.
    /// </summary>
    public class VariableInfo
    {
        [XmlAttribute]
        public string NETName;

        [XmlAttribute]
        public string TTreeName;

        [XmlAttribute]
        public string IndexToGroup;

        /// <summary>
        /// A comment the user wants to show up in the intellisense
        /// </summary>
        [XmlAttribute]
        public string Comment;
    }
}
