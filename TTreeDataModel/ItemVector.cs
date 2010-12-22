using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTreeDataModel
{
    /// <summary>
    /// An item that references a ROOT class that is a vector.
    /// </summary>
    public class ItemVector : IClassItem
    {
        private ROOTClassShell result;

        public ItemVector(ROOTClassShell result)
        {
            // TODO: Complete member initialization
            this.result = result;
        }
        public string ItemType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
