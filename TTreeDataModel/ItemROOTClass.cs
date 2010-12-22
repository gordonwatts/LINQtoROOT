using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTreeDataModel
{
    public class ItemROOTClass : IClassItem
    {
        private string _name;

        public ItemROOTClass(string p, string className)
        {
            this._name = p;
            ItemType = "ROOTNET.Interface.N" + className;
        }

        public string ItemType {get; private set; }
    }
}
