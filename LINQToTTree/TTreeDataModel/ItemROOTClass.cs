using System.Xml.Serialization;

namespace TTreeDataModel
{
    public class ItemROOTClass : IClassItem
    {
        public ItemROOTClass(string p, string className)
        {
            Name = p;
            ItemType = "ROOTNET.Interface.N" + className;
        }

        public ItemROOTClass()
        {
        }


        [XmlAttribute]
        public override string ItemType { get; set; }
        [XmlAttribute]
        public override string Name { get; set; }
    }
}
