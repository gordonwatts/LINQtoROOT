using System.Xml.Serialization;

namespace TTreeDataModel
{
    public sealed class ItemROOTClass : IClassItem
    {
        public ItemROOTClass(string p, string className)
        {
            Name = p;
            ItemType = "ROOTNET.Interface.N" + className;
            NotAPointer = false;
        }

        public ItemROOTClass()
        {
            NotAPointer = false;
        }


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
