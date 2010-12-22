using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTreeDataModel
{
    public class ItemSimpleType : IClassItem
    {
        private string _name;

        public ItemSimpleType(string name, string itemtype)
        {
            // TODO: Complete member initialization
            this._name = name;

            switch (itemtype)
            {
                case "int":
                    break;

                case "float":
                    break;

                case "double":
                    break;

                case "short":
                    break;

                case "unsigned int":
                    break;

                default:
                    throw new ArgumentException("Type '" + itemtype + "' is either not a simple type or is not known!");
            }
            ItemType = itemtype;
        }

        /// <summary>
        /// Return the type of this guy
        /// </summary>
        public string ItemType {get; private set; }
    }
}
