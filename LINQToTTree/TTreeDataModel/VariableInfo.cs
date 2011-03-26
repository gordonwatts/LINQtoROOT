﻿
using System.Xml.Serialization;
namespace TTreeDataModel
{
    /// <summary>
    /// A single variable and all info associated with it for the user to change.
    /// </summary>
    public class VariableInfo
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string RenameTo;

        [XmlAttribute]
        public string IndexToGroup;
    }
}