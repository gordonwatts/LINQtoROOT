
using System.Collections.Generic;
using System.Xml.Serialization;
namespace TTreeDataModel
{
    /// <summary>
    /// Represents a C style array of some other item type.
    /// </summary>
    public sealed class ItemCStyleArray : IClassItem, IClassItemExtraAttributes
    {
        /// <summary>
        /// Craete an item for output that contains a C style array.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="indexItem"></param>
        public ItemCStyleArray(string type, IClassItem baseItem)
        {
            ItemType = type;
            BaseItem = baseItem;
            Indicies = new List<IndexInfo>();
            NotAPointer = false;
        }

        /// <summary>
        /// No parameters b/c that is required for serialization.
        /// </summary>
        public ItemCStyleArray()
        {
            Indicies = new List<IndexInfo>();
            BaseItem = new ItemSimpleType("dude", "int"); // Dummy so we are immune to set order!
            NotAPointer = false;
        }

        /// <summary>
        /// Add a new index item to our list of indicies.
        /// </summary>
        /// <param name="position">0 is the left most index</param>
        /// <param name="boundName">The const number or other leaf that is the index upper value</param>
        /// <param name="isConst">True if this is a number vs a leaf name</param>
        public void Add(int position, string boundName, bool isConst)
        {
            Indicies.Add(new IndexInfo() { indexPosition = position, indexBoundName = boundName, indexConst = isConst });
        }

        /// <summary>
        /// Get/Set the type of this TTree variable (int[]).
        /// </summary>
        [XmlAttribute]
        public override string ItemType { get; set; }

        /// <summary>
        /// Get/Set the name of this TTree variable. (arr). We are
        /// get/setting the base item type here, to remain consistent.
        /// </summary>
        [XmlAttribute]
        public override string Name
        {
            get { return BaseItem.Name; }
            set { BaseItem.Name = value; }
        }

        /// <summary>
        /// Info for each context.
        /// </summary>
        public class IndexInfo
        {
            public int indexPosition;
            public string indexBoundName;
            public bool indexConst;
            public IndexInfo()
            {
                indexPosition = 0;
                indexBoundName = "";
                indexConst = false;
            }
        }

        /// <summary>
        /// Get/Set the list of indicies for this array.
        /// </summary>
        [XmlElement]
        public List<IndexInfo> Indicies { get; set; }

        /// <summary>
        /// The base item we are making an array of.
        /// </summary>
        [XmlElement]
        public IClassItem BaseItem { get; set; }

        /// <summary>
        /// Return the extra attribute to mark this guy as an index.
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerable<string> GetAttributes()
        {
            //
            // If our base item has any attributes, follow up on that...
            //

            if (BaseItem is IClassItemExtraAttributes)
            {
                foreach (var index in (BaseItem as IClassItemExtraAttributes).GetAttributes())
                {
                    yield return index;
                }
            }

            //
            // Now emit something for each index we are following.
            //

            foreach (var index in Indicies)
            {
                if (!index.indexConst)
                {
                    // Index is implied if we are in the middle of a tclones array class.
                    if (index.indexBoundName != "implied")
                    {
                        yield return string.Format("ArraySizeIndex(\"{0}\", Index = {1})", index.indexBoundName, index.indexPosition);
                    }
                }
                else
                {
                    yield return string.Format("ArraySizeIndex(\"{0}\", IsConstantExpression = true, Index = {1})", index.indexBoundName, index.indexPosition);
                }
            }
        }

        /// <summary>
        /// Get/Set this not being a ponter.
        /// </summary>
        [XmlAttribute]
        public override bool NotAPointer { get; set; }
    }
}
