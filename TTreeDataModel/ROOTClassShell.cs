using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTreeDataModel
{
    /// <summary>
    /// Represents a root class that isn't built into ROOT.
    /// </summary>
    public class ROOTClassShell
    {
        List<IClassItem> _items = new List<IClassItem>();
        public ROOTClassShell(string name)
        {
            // TODO: Complete member initialization
            this.Name = name;
        }

        /// <summary>
        /// Get the name of the class we represent
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Return all the items in this class
        /// </summary>
        public IEnumerable<IClassItem> Items { get { return _items; } }

        /// <summary>
        /// Add an item to this class.
        /// </summary>
        /// <param name="item"></param>
        public void Add(IClassItem item)
        {
            _items.Add(item);
        }

        public string SubClassName { get; set; }
    }
}
