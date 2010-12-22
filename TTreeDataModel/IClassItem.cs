using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTreeDataModel
{
    public interface IClassItem
    {
        /// <summary>
        /// Get the item type (in the .NET world).
        /// </summary>
        string ItemType { get; }
    }
}
