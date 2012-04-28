
namespace PSPROOFUtils
{
    public class ProofDataSetItem
    {
        /// <summary>
        /// Get the name of this dataset.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Create a new dataset with only the name info valid.
        /// </summary>
        /// <param name="name"></param>
        public ProofDataSetItem(string name)
        {
            // TODO: Complete member initialization
            this.Name = name;
        }
    }
}
