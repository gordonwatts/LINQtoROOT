using System.Collections.Generic;

namespace TTreeDataModel
{
    /// <summary>
    /// Represents a root class that isn't built into ROOT, or a
    /// class that is built into the TTree file, but isn't built into ROOT.
    /// </summary>
    public class ROOTClassShell
    {
        List<IClassItem> _items = new List<IClassItem>();
        public ROOTClassShell(string name)
        {
            this.Name = name;
            IsTClonesArrayClass = false;
            IsTopLevelClass = true;
        }

        /// <summary>
        /// To allow for serialization
        /// </summary>
        public ROOTClassShell()
        {
            IsTClonesArrayClass = false;
            IsTopLevelClass = true;
        }

        /// <summary>
        /// Get the name of the class we represent
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Return all the items in this class
        /// </summary>
        public List<IClassItem> Items { get { return _items; } }

        /// <summary>
        /// Add an item to this class.
        /// </summary>
        /// <param name="item"></param>
        public void Add(IClassItem item)
        {
            _items.Add(item);
        }

        /// <summary>
        /// This means this class is a tclones array class - it contains all the arrays in a tclones array. This means
        /// it gets a special attribute on its way out.
        /// </summary>
        public bool IsTClonesArrayClass { get; set; }

        /// <summary>
        /// True if this is a top level class for which proxy info should be written out or for which
        /// we should write the QueriableBLAHBLAH method.
        /// </summary>
        public bool IsTopLevelClass { get; set; }

        /// <summary>
        /// Get/Set the full path name where the user info xml file can be found.
        /// </summary>
        public string UserInfoPath { get; set; }

        /// <summary>
        /// lines that we should feed to the CINT interpreter when we are doing our generation.
        /// </summary>
        public string[] CINTExtraInfo { get; set; }

        /// <summary>
        /// List of classes for whome we need to generate a dictionary.
        /// </summary>
        public ClassForDictionary[] ClassesToGenerate { get; set; }

        /// <summary>
        /// The name of the class we represent.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Mostly for debugging aid!</remarks>
        public override string ToString()
        {
            return Name;
        }
    }
}
