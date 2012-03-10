
using System.Collections.Generic;
using System.Xml.Serialization;
namespace TTreeDataModel
{
    /// <summary>
    /// Represents a C style array
    /// </summary>
    public class ItemCStyleArray : IClassItem, IClassItemExtraAttributes
    {
        /// <summary>
        /// Craete an item for output that contains a C style array.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="indexItem"></param>
        public ItemCStyleArray(string type, string name)
        {
            ItemType = type;
            Name = name;
            Indicies = new List<IndexInfo>();
        }

        /// <summary>
        /// No parameters b/c that is required for serialization.
        /// </summary>
        public ItemCStyleArray()
        {
            Indicies = new List<IndexInfo>();
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
        /// Get/Set the name of this TTree variable. (arr).
        /// </summary>
        [XmlAttribute]
        public override string Name { get; set; }

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
        /// Return the extra attribute to mark this guy as an index.
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerable<string> GetAttributes()
        {
            foreach (var index in Indicies)
            {
                if (index.indexConst)
                {
                    yield return string.Format("ArraySizeIndex(\"{0}\", Index = {1})", index.indexBoundName, index.indexPosition);
                }
                else
                {
                    yield return string.Format("ArraySizeIndex(\"{0}\", IsConstantExpression = true, Index = {1})", index.indexBoundName, index.indexPosition);
                }
            }
        }
    }
}
