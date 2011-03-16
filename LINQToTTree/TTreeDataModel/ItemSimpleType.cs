
namespace TTreeDataModel
{
    public class ItemSimpleType : IClassItem
    {
        public ItemSimpleType(string name, string itemtype)
        {
            Name = name;
            ItemType = itemtype;
        }

        /// <summary>
        /// Ctor for serialization
        /// </summary>
        public ItemSimpleType()
        {
        }

        /// <summary>
        /// Return the type of this guy
        /// </summary>
        public override string ItemType { get; set; }
        public override string Name { get; set; }
    }
}
