
namespace TTreeDataModel
{
    /// <summary>
    /// A group of items that are 
    /// </summary>
    public class ArrayGroup
    {
        /// <summary>
        /// The name of the group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of variable names that this group contains.
        /// </summary>
        public VariableInfo[] Variables { get; set; }
    }
}
