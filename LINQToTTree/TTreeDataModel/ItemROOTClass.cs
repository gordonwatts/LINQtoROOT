using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


        public override string ItemType {get; set;}
        public override string Name { get; set; }
    }
}
